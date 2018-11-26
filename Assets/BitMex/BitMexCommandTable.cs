using Assets.BitMex.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexCommandFactory
    {
        private Dictionary<BitMexCommandType, IBitMexCommand> factory;

        public BitMexCommandFactory()
        {
            this.factory = new Dictionary<BitMexCommandType, IBitMexCommand>();
        }

        public void Resister(BitMexCommandType type, IBitMexCommand command)
        {
        }
    }

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
            command.Index = this.commands.Count;

            if (this.factory.ContainsKey(type) == false)
            {
                this.factory.Add(type, command);
            }

            this.commands.Add(command);
        }

        public IBitMexCommand CreateCommandByCreator(int index)
        {
            var original = this.commands[index - 1];
            var command = original.Clone();
            return command;
        }

        public List<IBitMexCommand> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public void ModifyCommandParameters(int index, List<object> parameters)
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
            if (command.CommandType == BitMexCommandType.OrderCommandCreate)
                return null;
            return this.commands[index];
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

        public IBitMexCommand CreateCommand(BitMexCommandType commandType)
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
                foreach (var item in JArray.Parse(File.ReadAllText(this.DataPath)))
                {
                    var jobjectCommand = JObject.Parse(item.ToString());
                    
                }
            }
        }

        public void SaveLocalCache()
        {
            var jarray = new JArray();

            foreach (var command in this.commands)
            {
                var jobjectCommand = new JObject();

                jobjectCommand.Add("CommandType", (ushort)command.CommandType);

                //var jarrayParameters = new JArray();
                //foreach (var parameter in macro.Command.Parameters)
                //{
                //    jarrayParameters.Add(parameter);
                //}

                //jobjectMacro.Add("Parameters", jarrayParameters);

                //jarray.Add(jobjectMacro);
            }

            File.WriteAllText(this.DataPath, jarray.ToString());
        }
    }
}
