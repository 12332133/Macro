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
        public decimal FixedAvailableXbt { get; set; }
        public decimal SpecifiedAditional { get; set; }
        public List<KeyValuePair<List<RawKey>, IBitMexActionCommand>> Macros { get; set; }

        public BitMexSession()
        {
            Macros = new List<KeyValuePair<List<RawKey>, IBitMexActionCommand>>();
            FixedAvailableXbt = 0;
            SpecifiedAditional = 12.5M;
        }

        public bool ResisterMacro(List<RawKey> rawKeys, IBitMexActionCommand command)
        {
            foreach (var macro in Macros)
            {
                if (macro.Key.SequenceEqual(rawKeys) == true)
                {
                    return false;
                }
            }

            Macros.Add(new KeyValuePair<List<RawKey>, IBitMexActionCommand>(rawKeys, command));

            return true;
        }

        public void SaveMacros()
        {
        }
    }
}
