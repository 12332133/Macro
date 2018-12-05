using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexCoin
    {
        /// <summary>
        /// 대표 코인 이름
        /// </summary>
        public string RootCoinName { get; set; }
        /// <summary>
        /// 코인 이름
        /// </summary>
        public string CoinName { get; set; }
        /// <summary>
        /// 현재 시장가
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 사용 가능 고정XBT
        /// </summary>
        public decimal FixedAvailableXbt { get; set; }
        /// <summary>
        /// 지정가
        /// </summary>
        public decimal SpecifiedAditional { get; set; } 
    }

    public class BitMexCoinTable
    {
        private readonly string dir = Resource.Dir + "cointable.json";
        private Dictionary<string, BitMexCoin> coins;
        private Dictionary<string, List<BitMexCoin>> templateSortedCoins;

        public BitMexCoinTable()
        {
            this.coins = new Dictionary<string, BitMexCoin>();
            this.templateSortedCoins = new Dictionary<string, List<BitMexCoin>>()
            {
                { "XBT", new List<BitMexCoin>() },
                { "ADA", new List<BitMexCoin>() },
                { "BCH", new List<BitMexCoin>() },
                { "EOS", new List<BitMexCoin>() },
                { "ETH", new List<BitMexCoin>() },
                { "LTC", new List<BitMexCoin>() },
                { "TRX", new List<BitMexCoin>() },
                { "XRP", new List<BitMexCoin>() },
            };
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

        public void LoadActiveCoins(string bitMexDomain)
        {
            var res = BitMexApiHelper.GetActiveInstruments(bitMexDomain);

            foreach (var item in JArray.Parse(res))
            {
                var jobject = JObject.Parse(item.ToString());

                if (jobject["state"].ToString().Equals("Open") == true)
                {
                    var rootCoinName = jobject["rootSymbol"].ToString();
                    var coinName = jobject["symbol"].ToString();
                    var tick = jobject["tickSize"].ToString();
                    var marketPrice = jobject["lastPrice"].ToString();

                    var coin = new BitMexCoin()
                    {
                        RootCoinName = rootCoinName,
                        CoinName = coinName,
                        SpecifiedAditional = decimal.Parse(tick, System.Globalization.NumberStyles.Any) * 25,
                        MarketPrice = decimal.Parse(marketPrice, System.Globalization.NumberStyles.Any),
                    };

                    this.coins.Add(coinName, coin);
                }
            }

            //load local cache then sync local cache by active coins
            if (File.Exists(this.dir) == true)
            {
                var json = File.ReadAllText(this.dir);

                foreach (var item in JArray.Parse(File.ReadAllText(this.dir)))
                {
                    var jobject = JObject.Parse(item.ToString());

                    var rootCoinName = jobject["RootCoinName"].ToString();
                    var coinName = jobject["CoinName"].ToString();
                    var fixedAvailableXbt = decimal.Parse(jobject["FixedAvailableXbt"].ToString(), System.Globalization.NumberStyles.Any);
                    var specifiedAditional = decimal.Parse(jobject["SpecifiedAditional"].ToString(), System.Globalization.NumberStyles.Any);

                    if (this.coins.ContainsKey(coinName) == true)
                    {
                        var coin = this.coins[coinName];
                        coin.FixedAvailableXbt = fixedAvailableXbt;
                        coin.SpecifiedAditional = specifiedAditional;
                    }
                }

                SaveLocalCache();
            }

            SortByCoinName();
        }

        private void SortByCoinName()
        {
            foreach (var coin in this.coins)
            {
                if (this.templateSortedCoins.ContainsKey(coin.Value.RootCoinName) == false)
                {
                    this.templateSortedCoins.Add(coin.Value.RootCoinName, new List<BitMexCoin>());
                }
                this.templateSortedCoins[coin.Value.RootCoinName].Add(coin.Value);
            }
            
            this.coins.Clear();
            foreach (var coins in this.templateSortedCoins)
            {
                foreach (var coin in coins.Value)
                {
                    this.coins.Add(coin.CoinName, coin);
                }
            }
        }

        public void SaveLocalCache()
        {
            var jarray = new JArray();

            foreach (var coin in this.coins.Values.ToArray())
            {
                var jobject = new JObject();
                jobject.Add("RootCoinName", coin.RootCoinName);
                jobject.Add("CoinName", coin.CoinName);
                jobject.Add("FixedAvailableXbt", coin.FixedAvailableXbt.ToString());
                jobject.Add("SpecifiedAditional", coin.SpecifiedAditional.ToString());
                jarray.Add(jobject);
            }

            File.WriteAllText(this.dir, jarray.ToString());
        }
        
    }
}
