using Assets.BitMex.Commands;
using Assets.CombinationKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexCommandTable
    {
        private readonly string DataPath = Application.dataPath + "/Resources/Config/commandtable.json";
        private List<IBitMexCommand> commands;
        private Dictionary<BitMexCommandType, IBitMexCommand> factory;

        public BitMexCommandTable()
        {
            this.commands = new List<IBitMexCommand>();
            this.factory = new Dictionary<BitMexCommandType, IBitMexCommand>();
        }

        public void Resister(BitMexCommandType type, IBitMexCommand command)
        {
            command.CommandType = type;
            command.RefCommandTableIndex = this.commands.Count;

            if (this.factory.ContainsKey(type) == false)
            {
                this.factory.Add(type, command);
            }

            this.commands.Add(command);
        }

        public void InsertAt(IBitMexCommand command)
        {
            this.commands.Insert(command.RefCommandTableIndex, command);

            // 인덱스 재 배치
            for (int i = 0; i < this.commands.Count; i++)
            {
                this.commands[i].RefCommandTableIndex = i;
            }
        }

        public IBitMexCommand CreateByCreator(int creatorIndex)
        {
            var original = this.commands[creatorIndex - 1];
            var command = original.Clone();
            command.RefCommandTableIndex += 1;
            return command;
        }

        public bool Remove(IBitMexCommand command)
        {
            if (command.CommandType == BitMexCommandType.None ||
                command.CommandType == BitMexCommandType.OrderCommandCreate)
            {
                return false;
            }

            this.commands.RemoveAt(command.RefCommandTableIndex);

            // 인덱스 재 배치
            for (int i = 0; i < this.commands.Count; i++)
            {
                this.commands[i].RefCommandTableIndex = i;
            }

            return true;
        }

        public List<IBitMexCommand> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public void ModifyParameters(int index, List<object> parameters)
        {
            var command = this.commands[index];
            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);
        }

        public bool HasCommand(int index)
        {
            return this.commands[index] != null;
        }

        public IBitMexCommand FindCommand(int index)
        {
            var command = this.commands[index];
            //if (command.CommandType == BitMexCommandType.OrderCommandCreate ||
            //    command.CommandType == BitMexCommandType.None)
            //    return null;
            return command;
        }

        public IBitMexCommand FindCommand(BitMexCommandType type)
        {
            foreach (var command in this.commands)
            {
                if (command.CommandType == type)
                {
                    return command;
                }
            }

            return null;
        }

        public IBitMexCommand Create(BitMexCommandType commandType)
        {
            if (this.factory.ContainsKey(commandType) == true)
            {
                var command = this.factory[commandType].Clone();
                command.Parameters.Clear();
            }
            return null;
        }

        public void LoadLocalCache()
        {
            if (File.Exists(this.DataPath) == true)
            {
                var loadCommands = new List<IBitMexCommand>();

                foreach (var item in JArray.Parse(File.ReadAllText(this.DataPath)))
                {
                    var jobjectCommand = JObject.Parse(item.ToString());

                    var commandType = (BitMexCommandType)((ushort)jobjectCommand["CommandType"]);
                    var commandIndex = (int)jobjectCommand["CommandIndex"];

                    if (commandType == BitMexCommandType.None || 
                        commandType == BitMexCommandType.OrderCommandCreate)
                    {
                        continue;
                    }

                    var parameters = new List<object>();
                    var jelementParameters = jobjectCommand["Parameters"].ToString();
                    foreach (var parameter in JArray.Parse(jelementParameters))
                    {
                        parameters.Add(parameter);
                    }

                    var command = FindCommand(commandIndex);
                    if (command == null)
                    {
                        var newCommand = Create(commandType);
                        newCommand.Parameters.Clear();
                        newCommand.Parameters.AddRange(parameters);
                        newCommand.RefCommandTableIndex = commandIndex;
                        this.commands.Insert(newCommand.RefCommandTableIndex, newCommand);
                    }
                    else
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddRange(parameters);
                    }
                }
            }
        }

        public void SaveLocalCache()
        {
            var jarray = new JArray();

            foreach (var command in this.commands)
            {
                var jobjectCommand = new JObject();
                jobjectCommand.Add("CommandIndex", command.RefCommandTableIndex);
                jobjectCommand.Add("CommandType", (ushort)command.CommandType);

                var jarrayParameters = new JArray();
                foreach (var parameter in command.Parameters)
                {
                    jarrayParameters.Add(parameter);
                }

                jobjectCommand.Add("Parameters", jarrayParameters);

                jarray.Add(jobjectCommand);
            }

            File.WriteAllText(this.DataPath, jarray.ToString());
        }
    }
}
