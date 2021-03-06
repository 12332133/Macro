﻿using Assets.BitMex.Commands;
using Assets.CombinationKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexCommandTable
    {
        private Dictionary<BitMexCommandTableType, List<IBitMexCommand>> commands;
        private Dictionary<BitMexCommandType, IBitMexCommand> factory;
        private readonly string dir = Resource.Dir + "commandtable.json";

        public BitMexCommandTable(string name)
            :this()
        {
            this.dir = Resource.Dir + name + ".json";
        }

        public BitMexCommandTable()
        {
            this.factory = new Dictionary<BitMexCommandType, IBitMexCommand>();

            this.commands = new Dictionary<BitMexCommandTableType, List<IBitMexCommand>>();
            commands.Add(BitMexCommandTableType.Percent, new List<IBitMexCommand>());
            commands.Add(BitMexCommandTableType.Quantity, new List<IBitMexCommand>());
            commands.Add(BitMexCommandTableType.Etc, new List<IBitMexCommand>());
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

        public void Insert(IBitMexCommand command)
        {
            this.commands[command.CommandTableType].Add(command);

            // 정렬
            Sort(command);

            // 인덱스 재배치
            SortIndex(command);
        }

        public void ModifyCommand(IBitMexCommand command)
        {
            // 정렬
            Sort(command);

            // 인덱스 재배치
            SortIndex(command);
        }

        private void SortIndex(IBitMexCommand command)
        {
            // 인덱스 재 배치
            for (int i = 0; i < this.commands[command.CommandTableType].Count; i++)
            {
                this.commands[command.CommandTableType][i].RefCommandTableIndex = i;
            }
        }

        private void Sort(IBitMexCommand command)
        {
            var dict = new Dictionary<BitMexCommandType, List<IBitMexCommand>>();

            foreach (var originCommand in this.commands[command.CommandTableType])
            {
                if (originCommand.CommandType == BitMexCommandType.None)
                {
                    continue;
                }

                if (dict.ContainsKey(originCommand.CommandType) == false)
                {
                    dict.Add(originCommand.CommandType, new List<IBitMexCommand>());
                }

                dict[originCommand.CommandType].Add(originCommand);
            }

            this.commands[command.CommandTableType].Clear();

            foreach (var originCommands in dict)
            {
                var noneCommand = CreateCommand(BitMexCommandType.None).Clone();
                noneCommand.CommandTableType = command.CommandTableType;
                this.commands[command.CommandTableType].Add(noneCommand);

                switch (command.CommandTableType)
                {
                    case BitMexCommandTableType.Percent:
                    case BitMexCommandTableType.Quantity:
                        var sorted = originCommands.Value.OrderByDescending(x => Convert.ToInt32(x.Parameters[0])).ToList();
                        this.commands[command.CommandTableType].AddRange(sorted);
                        break;
                    case BitMexCommandTableType.Etc:
                        this.commands[command.CommandTableType].AddRange(originCommands.Value);
                        break;
                }
            }
        }

        public bool Remove(IBitMexCommand command)
        {
            if (command.CommandType == BitMexCommandType.None)
            {
                return false;
            }
            
            if (GetCommandCount(command.CommandTableType, command.CommandType) == 1)
            {
                return false;
            }

            this.commands[command.CommandTableType].Remove(command);

            // 정렬
            Sort(command);

            // 인덱스 재배치
            SortIndex(command);

            return true;
        }

        public List<IBitMexCommand> GetCommandsByTableType(BitMexCommandTableType tableType)
        {
            return this.commands[tableType];
        }

        public IBitMexCommand FindCommand(BitMexCommandTableType tableType, int index)
        {
            var command = this.commands[tableType][index];
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

        public int GetCommandCount(BitMexCommandTableType tableType, BitMexCommandType type)
        {
            int count = 0;

            foreach (var command in this.commands[tableType])
            {
                if (command.CommandType == type)
                {
                    count++;
                }
            }

            return count;
        }

        public IBitMexCommand CreateCommand(BitMexCommandType commandType)
        {
            if (this.factory.ContainsKey(commandType) == true)
            {
                var command = this.factory[commandType].Clone();
                command.Parameters.Clear();
                return command;
            }
            return null;
        }

        public void LoadLocalCache()
        {
            if (File.Exists(this.dir) == true)
            {
                var loadCommands = new List<IBitMexCommand>();

                foreach (var commandTable in this.commands)
                {
                    commandTable.Value.Clear();
                }

                foreach (var item in JArray.Parse(File.ReadAllText(this.dir)))
                {
                    var jobjectCommand = JObject.Parse(item.ToString());

                    var commandTableType = (BitMexCommandTableType)((ushort)jobjectCommand["TableType"]);
                    var commandType = (BitMexCommandType)((ushort)jobjectCommand["CommandType"]);
                    var commandIndex = (int)jobjectCommand["CommandIndex"];

                    var parameters = new List<object>();
                    var jelementParameters = jobjectCommand["Parameters"].ToString();
                    foreach (var parameter in JArray.Parse(jelementParameters))
                    {
                        parameters.Add(parameter.ToObject<object>());
                    }

                    var command = CreateCommand(commandType);

                    if (command == null)
                    {
                        continue;
                    }

                    command.Parameters.AddRange(parameters);
                    command.RefCommandTableIndex = commandIndex;
                    command.CommandType = commandType;
                    command.CommandTableType = commandTableType;

                    this.commands[commandTableType].Add(command);
                }

                // 인덱스 재 배치
                foreach (var commandType in this.commands)
                {
                    // 인덱스 재 배치
                    for (int i = 0; i < commandType.Value.Count; i++)
                    {
                        this.commands[commandType.Key][i].RefCommandTableIndex = i;
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
                    jobjectCommand.Add("CommandType", (ushort)command.CommandType);
                    jobjectCommand.Add("CommandIndex", command.RefCommandTableIndex);

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
