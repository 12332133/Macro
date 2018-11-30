using Assets.BitMex;
using Assets.BitMex.Commands;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAlarm : ContentsBase
{
    public class MarketPriceAlram : IBitMexSchedule
    {
        public ExecuteType ExecuteType { get; set; }
        public decimal Price { get; set; }
        public int AlramCount { get; set; }
        public BitMexCoin Coin { get; set; }
        public IBitMexMainAdapter BitmexMain { get; set; }

        public bool IsCompletePriceConditions
        {
            get
            {
                switch (this.ExecuteType)
                {
                    case ExecuteType.PriceOver:
                        return this.Price > this.Coin.MarketPrice;
                    case ExecuteType.PriceUnder:
                        return this.Price < this.Coin.MarketPrice;
                }
                return false;
            }
        }

        public bool Execute()
        {
            if (IsCompletePriceConditions == true)
            {
                Task.Run(() => 
                {
                    for (int i = 0; i < this.AlramCount; i++)
                    {
                        EditorApplication.Beep();
                        Thread.Sleep(300);
                    }
                });
            }
            return false;
        }
    }

    private void Reset()
    {
    }

    private void Awake()
    {
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        /*
        new Thread(e => {

            var commands = this.bitmexMain.CommandTable.GetCommands(BitMexCommandTableType.Percent);
            var command = commands[3];
            var coin = this.bitmexMain.CoinTable.GetCoin("XBTUSD");

            while (true)
            {
                AddSchedule(coin, ExecuteType.PriceUnder, 0, 3);
                Thread.Sleep(1000);
            }
        })
        {
            IsBackground = true,
        }.Start();
        */
    }

    public void AddSchedule(BitMexCoin coin, ExecuteType type, decimal price, int alramCount)
    {
        var schedule = new MarketPriceAlram()
        {
            Coin = coin,
            ExecuteType = type,
            Price = price,
            AlramCount = alramCount,
            BitmexMain = bitmexMain,
        };

        this.bitmexMain.ResisterSchedule(schedule);
    }
}
