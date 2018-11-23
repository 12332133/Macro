﻿using System;
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
        public string RootCoinName { get; set; } //대표 코인 이름
        public string CoinName { get; set; } //서브 코인 이름
        public decimal MarketPrice { get; set; } //현재 시장가 
        public decimal FixedAvailableXbt { get; set; } //사용 가능 고정XBT
        public decimal SpecifiedAditional { get; set; } //지정가
    }

    public class BitMexCoinTable
    {
        private readonly string CachePath = "CoinTable.json";
        private Dictionary<string, BitMexCoin> coins;

        public BitMexCoinTable()
        {
            this.coins = new Dictionary<string, BitMexCoin>();
            CachePath = Application.dataPath + "/Resources/Config/cointable.json";
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
                    var defaultSpecifiedAditional = decimal.Parse(tick, System.Globalization.NumberStyles.Any) * 25;

                    var coin = new BitMexCoin()
                    {
                        RootCoinName = rootCoinName,
                        CoinName = coinName,
                        SpecifiedAditional = defaultSpecifiedAditional,
                        MarketPrice = 0,
                    };

                    this.coins.Add(coinName, coin);
                }
            }

            //load local cache then sync local cache by active coins
            if (File.Exists(CachePath) == true)
            {
                var json = File.ReadAllText(CachePath);

                foreach (var item in JArray.Parse(File.ReadAllText(CachePath)))
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

            File.WriteAllText(CachePath, jarray.ToString());
            //PlayerPrefs.SetString("BitMexCoinTable", jarray.ToString());
            //PlayerPrefs.Save();
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
