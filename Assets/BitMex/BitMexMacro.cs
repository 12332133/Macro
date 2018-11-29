using Assets.BitMex.Commands;
using Assets.CombinationKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.BitMex
{
    public class Macro
    {
        public int Index;
        public List<RawKey> Keys = new List<RawKey>();
        public IBitMexCommand Command;
    }

    public class BitMexMacro
    {
        private Dictionary<BitMexCommandTableType, List<Macro>> macros;
        private readonly string dir = Resource.Dir + "macro.json";

        public BitMexMacro()
        {
            macros = new Dictionary<BitMexCommandTableType, List<Macro>>();
            macros.Add(BitMexCommandTableType.Etc, new List<Macro>());
            macros.Add(BitMexCommandTableType.Percent, new List<Macro>());
            macros.Add(BitMexCommandTableType.Quantity, new List<Macro>());
        }

        public Dictionary<BitMexCommandTableType, List<Macro>> GetMacroTable()
        {
            return this.macros;
        }

        public List<Macro> GetMacros(BitMexCommandTableType type)
        {
            return this.macros[type];
        }

        public Macro Resister(List<RawKey> keys, IBitMexCommand command)
        {
            var macro = new Macro();
            macro.Index = this.macros[command.CommandTableType].Count;
            macro.Keys.AddRange(keys);
            macro.Command = command;

            macros[command.CommandTableType].Add(macro);
            return macro;
        }

        /// <summary>
        /// 인덱스 재 배치
        /// </summary>
        private void Restore(BitMexCommandTableType type)
        {
            for (int i = 0; i < this.macros[type].Count; i++)
            {
                this.macros[type][i].Index = i;
            }
        }

        public bool RemoveAt(BitMexCommandTableType type, int index)
        {
            if (this.macros[type][index] == null)
            {
                return false;
            }

            this.macros[type].RemoveAt(index);

            Restore(type);
            return true;
        }

        public bool RemoveByCommand(IBitMexCommand command)
        {
            bool bRemoved = false;
            foreach (var macro in this.macros[command.CommandTableType])
            {
                if (macro.Command == command)
                {
                    this.macros[command.CommandTableType].RemoveAt(macro.Index);
                    bRemoved = true;
                }
            }

            Restore(command.CommandTableType);
            return bRemoved;
        }

        public bool ModifyRawKeys(Macro macro, List<RawKey> keys)
        {
            if (IsEqualKeys(keys) == false)
            {
                return false;
            }

            macro.Keys.Clear();
            macro.Keys.AddRange(keys);
            return true;
        }

        public bool ModifyRawKeys(BitMexCommandTableType type, int index, List<RawKey> keys)
        {
            if (IsEqualKeys(keys) == false)
            {
                return false;
            }

            var macro = macros[type][index];
            macro.Keys.Clear();
            macro.Keys.AddRange(keys);

            return true;
        }

        public bool ModifyCommand(BitMexCommandTableType type, int index, IBitMexCommand command)
        {
            var macro = macros[type][index];
            if (macro == null)
            {
                return false;
            }

            macro.Command = command;
            return true;
        }

        public bool IsEqualKeys(List<RawKey> keys)
        {
            foreach (var table in macros)
            {
                foreach (var macro in table.Value)
                {
                    if (macro.Keys.SequenceEqual(keys) == true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void LoadLocalCache(BitMexCommandTable commandTable)
        {
            if (File.Exists(this.dir) == true)
            {
                foreach (var item in JArray.Parse(File.ReadAllText(this.dir)))
                {
                    var jobjectMacro = JObject.Parse(item.ToString());
                    var jelementRawKeys = jobjectMacro["RawKeys"].ToString();

                    var rawKeys = new List<RawKey>();
                    foreach (var rawKey in JArray.Parse(jelementRawKeys))
                    {
                        rawKeys.Add((RawKey)((ushort)rawKey));
                    }

                    var commandTableType = (BitMexCommandTableType)((ushort)jobjectMacro["CommandTableType"]);
                    var commandIndex = (int)jobjectMacro["CommandIndex"];
                    var commandType = (BitMexCommandType)((ushort)jobjectMacro["CommandType"]);

                    var command = commandTable.FindCommand(commandTableType, commandIndex);
                    if (command != null)
                    {
                        Resister(rawKeys, command);
                    }
                }
            }
        }

        public void SaveLocalCache()
        {
            var jarray = new JArray();

            foreach (var table in this.macros)
            {
                foreach (var macro in table.Value)
                {
                    var jobjectMacro = new JObject();

                    var jarrayRawKeys = new JArray();
                    foreach (var rawKey in macro.Keys)
                    {
                        jarrayRawKeys.Add((ushort)rawKey);
                    }

                    jobjectMacro.Add("RawKeys", jarrayRawKeys);
                    jobjectMacro.Add("CommandTableType", (ushort)macro.Command.CommandTableType);
                    jobjectMacro.Add("CommandType", (ushort)macro.Command.CommandType);
                    jobjectMacro.Add("CommandIndex", (ushort)macro.Command.RefCommandTableIndex);

                    jarray.Add(jobjectMacro);
                }
            }

            File.WriteAllText(this.dir, jarray.ToString());
        }
    }
}
