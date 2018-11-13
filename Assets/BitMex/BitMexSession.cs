using Assets.BitMex.Commands;
using Assets.KeyBoardHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexSession
    {
        public string ApiSecret;
        public string ApiKey;
        public string Email;
        public string ReferrerAccount;
        public string ReferrerEmail;
        public DateTime UpdateDateTime;
        public List<KeyValuePair<List<RawKey>, IBitMexCommand>> Macros { get; private set; }

        public BitMexSession()
        {
            Macros = new List<KeyValuePair<List<RawKey>, IBitMexCommand>>();
        }

        public bool ResisterMacro(List<RawKey> rawKeys, IBitMexCommand command)
        {
            foreach (var macro in Macros)
            {
                if (macro.Key.SequenceEqual(rawKeys) == true)
                {
                    return false;
                }
            }

            Macros.Add(new KeyValuePair<List<RawKey>, IBitMexCommand>(rawKeys, command));

            return true;
        }

        public void SaveMacros()
        {
        }
    }
}
