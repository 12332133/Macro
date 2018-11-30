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
        private Dictionary<BitMexCommandTableType, List<IBitMexCommand>> commands;
        private Dictionary<BitMexCommandType, IBitMexCommand> factory;
        private readonly string dir = Resource.Dir + "commandtable.json";

        public BitMexCommandTable()
        {
            this.commands = new Dictionary<BitMexCommandTableType, List<IBitMexCommand>>();
            commands.Add(BitMexCommandTableType.Etc, new List<IBitMexCommand>());
            commands.Add(BitMexCommandTableType.Percent, new List<IBitMexCommand>());
            commands.Add(BitMexCommandTableType.Quantity, new List<IBitMexCommand>());

            this.factory = new Dictionary<BitMexCommandType, IBitMexCommand>();
        }

        public void Resister(BitMexCommandTableType tableType, BitMexCommandType commandType, IBitMexCommand command)
        {
            command.CommandTableType = tableType;
            command.CommandType = commandType;
            command.RefCommandTableIndex = this.commands[tableType].Count;

            if (this.factory.ContainsKey(commandType) == false)
            {
                this.factory.Add(commandType, command);
            }

            this.commands[tableType].Add(command);
        }

        public void InsertAt(IBitMexCommand command)
        {
            this.commands[command.CommandTableType].Insert(command.RefCommandTableIndex, command);

            // 인덱스 재 배치
            for (int i = 0; i < this.commands[command.CommandTableType].Count; i++)
            {
                this.commands[command.CommandTableType][i].RefCommandTableIndex = i;
            }
        }

        public IBitMexCommand CreateByCreator(BitMexCommandTableType tableType, int creatorIndex)
        {
            var original = this.commands[tableType][creatorIndex - 1];
            var command = original.Clone();
            command.RefCommandTableIndex += 1;
            return command;
        }

        public IBitMexCommand CreateByCreator(IBitMexCommand creatorCommand)
        {
            var original = this.commands[creatorCommand.CommandTableType][creatorCommand.RefCommandTableIndex - 1];
            var command = original.Clone();
            command.RefCommandTableIndex += 1;
            return command;
        }

        public bool Remove(IBitMexCommand command)
        {
            if (command.CommandType == BitMexCommandType.None)
            {
                return false;
            }

            this.commands[command.CommandTableType].RemoveAt(command.RefCommandTableIndex);

            // 인덱스 재 배치
            for (int i = 0; i < this.commands[command.CommandTableType].Count; i++)
            {
                this.commands[command.CommandTableType][i].RefCommandTableIndex = i;
            }

            return true;
        }

        public List<IBitMexCommand> GetCommands(BitMexCommandTableType tableType)
        {
            return this.commands[tableType];
        }
       
        public bool HasCommand(BitMexCommandTableType tableType, int index)
        {
            return this.commands[tableType][index] != null;
        }

        public IBitMexCommand FindCommand(BitMexCommandTableType tableType, int index)
        {
            var command = this.commands[tableType][index];
            //if (command.CommandType == BitMexCommandType.OrderCommandCreate ||
            //    command.CommandType == BitMexCommandType.None)
            //    return null;
            return command;
        }

        public IBitMexCommand FindCommand(BitMexCommandTableType tableType, BitMexCommandType type)
        {
            foreach (var command in this.commands[tableType])
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
            if (File.Exists(this.dir) == true)
            {
                var loadCommands = new List<IBitMexCommand>();

                foreach (var item in JArray.Parse(File.ReadAllText(this.dir)))
                {
                    var jobjectCommand = JObject.Parse(item.ToString());

                    var tableType = (BitMexCommandTableType)((ushort)jobjectCommand["TableType"]);
                    var commandType = (BitMexCommandType)((ushort)jobjectCommand["CommandType"]);
                    var commandIndex = (int)jobjectCommand["CommandIndex"];

                    if (commandType == BitMexCommandType.None)
                    {
                        continue;
                    }

                    var parameters = new List<object>();
                    var jelementParameters = jobjectCommand["Parameters"].ToString();
                    foreach (var parameter in JArray.Parse(jelementParameters))
                    {
                        parameters.Add(parameter);
                    }

                    var command = FindCommand(tableType, commandIndex);
                    if (command == null)
                    {
                        var newCommand = Create(commandType);
                        newCommand.Parameters.Clear();
                        newCommand.Parameters.AddRange(parameters);
                        newCommand.RefCommandTableIndex = commandIndex;
                        this.commands[tableType].Insert(newCommand.RefCommandTableIndex, newCommand);
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

            foreach (var table in this.commands)
            {
                foreach (var command in table.Value)
                {
                    var jobjectCommand = new JObject();
                    jobjectCommand.Add("TableType", (ushort)command.CommandTableType);
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
            }

            File.WriteAllText(this.dir, jarray.ToString());
        }
    }
}
