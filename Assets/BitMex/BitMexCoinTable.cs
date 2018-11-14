using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexCoin
    {
        public string RootCoinName { get; set; }
        public string CoinName { get; set; }
        public decimal SpecifiedAditional { get; set; }
        //public decimal MarketPrice { get; set; }
        public string MarketPrice { get; set; }
    }

    public class BitMexCoinTable
    {
        //public const decimal DefaultXBTUSDSpecifiedAditional = 12.5M; 
        private Dictionary<string, BitMexCoin> coins;

        public BitMexCoinTable()
        {
            this.coins = new Dictionary<string, BitMexCoin>();
        }

        public BitMexCoin ResisterCoin(string rootCoinName, string coinName)
        {
            var coin = new BitMexCoin()
            {
                RootCoinName = rootCoinName,
                CoinName = coinName,
                SpecifiedAditional = 0,
                MarketPrice = "0",
            };

            this.coins.Add(coinName, coin);
            return coin;
        }
        
        public BitMexCoin GetCoin(string coinName)
        {
            if (this.coins.ContainsKey(coinName) == false)
            {
                throw new BitMexDriverServiceException();
            }
            return this.coins[coinName];
        }

        public Dictionary<string, BitMexCoin> Coins
        {
            get
            {
                return this.coins;
            }
        }
    }
}
