using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexSpecificCoinVariable
    {
        public const decimal DefaultFixedAvailableXbt = 0;
        public const decimal DefaultSpecifiedAditional = 12.5M;
        private string[] activeCoins;

        private Dictionary<string, BitMexCoinVariable> variables;

        public BitMexSpecificCoinVariable()
        {
            this.variables = new Dictionary<string, BitMexCoinVariable>();
        }

        public BitMexCoinVariable ResisterCoin(string coinName)
        {
            var variable = new BitMexCoinVariable()
            {
                CoinName = coinName,
                FixedAvailableXbt = DefaultFixedAvailableXbt,
                SpecifiedAditional = DefaultSpecifiedAditional
            };

            this.variables.Add(coinName, variable);
            return variable;
        }
        
        public BitMexCoinVariable GetCoinVariable(string coinName)
        {
            if (this.variables.ContainsKey(coinName) == true)
            {
                return this.variables[coinName];
            }

            return ResisterCoin(coinName);
        }

        public string[] ActivatedCoins
        {
            get
            {
                if (this.activeCoins == null)
                {
                    this.activeCoins = this.variables.Keys.ToArray();
                }
                return this.activeCoins;
            }
        }

        public Dictionary<string, BitMexCoinVariable> Variables
        {
            get
            {
                return this.variables;
            }
        }
    }

    public class BitMexCoinVariable
    {
        public string CoinName { get; set; }
        public decimal FixedAvailableXbt { get; set; }
        public decimal SpecifiedAditional { get; set; }
        public string MarketPrice { get; set; }

        public BitMexCoinVariable()
        {
            MarketPrice = "0";
        }
    }
}
