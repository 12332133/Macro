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
        public List<Macro> Macros { get; private set; }
        private readonly string dir = Resource.Dir + "macro.json";

        public BitMexMacro()
        {
            Macros = new List<Macro>();
        }

        public Macro Resister(List<RawKey> keys, IBitMexCommand command)
        {
            var macro = new Macro();
            macro.Index = this.Macros.Count;
            macro.Keys.AddRange(keys);
            macro.Command = command; 

            Macros.Add(macro);
            return macro;
        }

        /// <summary>
        /// 인덱스 재 배치
        /// </summary>
        private void Restore()
        {
            for (int i = 0; i < this.Macros.Count; i++)
            {
                this.Macros[i].Index = i;
            }
        }

        public bool RemoveAt(int index)
        {
            if (this.Macros[index] == null)
            {
                return false;
            }

            this.Macros.RemoveAt(index);

            Restore();
            return true;
        }

        public bool RemoveByCommand(IBitMexCommand command)
        {
            bool bRemoved = false;
            foreach (var macro in this.Macros)
            {
                if (macro.Command == command)
                {
                    this.Macros.RemoveAt(macro.Index);
                    bRemoved = true;
                }
            }

            Restore();
            return bRemoved;
        }

        public bool ModifyRawKeys(int index, List<RawKey> keys)
        {
            if (IsEqualKeys(keys) == false)
            {
                return false;
            }

            var macro = Macros[index];
            macro.Keys.Clear();
            macro.Keys.AddRange(keys);

            return true;
        }

        public bool ModifyCommand(int index, IBitMexCommand command)
        {
            var macro = Macros[index];
            if (macro == null)
            {
                return false;
            }

            macro.Command = command;
            return true;
        }

        public bool IsEqualKeys(List<RawKey> keys)
        {
            foreach (var macro in Macros)
            {
                if (macro.Keys.SequenceEqual(keys) == true)
                {
                    return false;
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

                    var commandIndex = (int)jobjectMacro["CommandIndex"];
                    var commandType = (BitMexCommandType)((ushort)jobjectMacro["CommandType"]);

                    var command = commandTable.FindCommand(commandIndex);
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

            foreach (var macro in this.Macros)
            {
                var jobjectMacro = new JObject();

                var jarrayRawKeys = new JArray();
                foreach (var rawKey in macro.Keys)
                {
                    jarrayRawKeys.Add((ushort)rawKey);
                }

                jobjectMacro.Add("RawKeys", jarrayRawKeys);
                jobjectMacro.Add("CommandIndex", (ushort)macro.Command.RefCommandTableIndex);
                jobjectMacro.Add("CommandType", (ushort)macro.Command.CommandType);

                jarray.Add(jobjectMacro);
            }

            File.WriteAllText(this.dir, jarray.ToString());
        }
    }
}
