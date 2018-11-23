using Assets.BitMex;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAlarm : ContentsBase
{
    public enum AlramType
    {
        Over,
        Under,
    }

    public class MarketPriceAlram : IBitMexSchedule
    {
        public AlramType Type { get; private set; }
        public decimal Price { get; private set; }
        public int AlramCount { get; private set; }
        public BitMexCoin Coin { get; set; }

        public MarketPriceAlram(AlramType type, decimal price, int alramCount)
        {
            Type = type;
            Price = price;
            AlramCount = alramCount;
        }

        public bool IsCompletePriceConditions
        {
            get
            {
                switch (Type)
                {
                    case AlramType.Over:
                        return Coin.MarketPrice < Price;
                    case AlramType.Under:
                        return Coin.MarketPrice > Price;
                }
                return false;
            }
        }

        public bool Execute()
        {
            return IsCompletePriceConditions;
        }
    }

    private Thread updator;
    private BlockingCollection<MarketPriceAlram> alrams;

    private void Reset()
    {
    }

    private void Awake()
    {
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        this.alrams = new BlockingCollection<MarketPriceAlram>();

        //this.updator = new Thread(() =>
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            var alram = this.alrams.Take();
        //            if (alram.IsCompletePriceConditions == false)
        //            {
        //                Thread.Sleep(20);
        //                this.alrams.Add(alram);
        //            }
        //            else
        //            {
        //                Task.Run(() =>
        //                {
        //                    for (int i = 0; i < alram.AlramCount; i++)
        //                    {
        //                        Debug.Log(string.Format("run alram {0}", alram.Price));
        //                    }
        //                });
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //    }
        //});
        //updator.IsBackground = true;
        //updator.Start();
    }

    public void AddSchedule(AlramType type, decimal price, int alramCount)
    {
        this.alrams.Add(new MarketPriceAlram(type, price, alramCount));
    }
}
