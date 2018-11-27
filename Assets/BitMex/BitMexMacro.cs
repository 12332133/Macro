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
        private readonly string DataPath = Application.dataPath + "/Resources/Config/macro.json";
        public List<Macro> Macros { get; private set; }

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

        public bool RemoveAt(IBitMexCommand command)
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

            // 인덱스 재 배치
            for (int i = 0; i < this.Macros.Count; i++)
            {
                this.Macros[i].Index = i;
            }

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
            if (File.Exists(this.DataPath) == true)
            {
                foreach (var item in JArray.Parse(File.ReadAllText(this.DataPath)))
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

                    //command.Parameters.Clear();

                    //var jelementParameters = jobjectMacro["Parameters"].ToString();
                    //foreach (var parameter in JArray.Parse(jelementParameters))
                    //{
                    //    command.Parameters.Add(parameter);
                    //}
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

                //jobjectMacro.Add("CommandType", (ushort)macro.Command.CommandType);

                //var jarrayParameters = new JArray();
                //foreach (var parameter in macro.Command.Parameters)
                //{
                //    jarrayParameters.Add(parameter);
                //}

                //jobjectMacro.Add("Parameters", jarrayParameters);

                jarray.Add(jobjectMacro);
            }

            File.WriteAllText(this.DataPath, jarray.ToString());
        }
    }
}
