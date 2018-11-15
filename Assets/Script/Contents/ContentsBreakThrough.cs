using Assets.BitMex;
using Assets.BitMex.Commands;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContentsBreakThrough : ContentsBase
{
    public enum TradeType
    {
        Over,
        Under,
    }

    public class BreakThroughTrade
    {
        public decimal Price { get; set; }
        public IBitMexCommand Command { get; set; }
        public TradeType Type { get; set; }
        public BitMexCoin Coin { get; set; }

        public bool IsCompletePriceConditions
        {
            get
            {
                switch (this.Type)
                {
                    case TradeType.Over:
                        return decimal.Parse(this.Coin.MarketPrice) < this.Price;
                    case TradeType.Under:
                        return decimal.Parse(this.Coin.MarketPrice) > this.Price;
                }
                return false;
            }
        }
    }

    private Thread producer;
    private Thread customer;
    private ConcurrentQueue<BreakThroughTrade> trades;
    private List<BitMexCommandType> overCommandTypes;
    private List<BitMexCommandType> underCommandTypes;

    private void Reset()
    {
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        this.overCommandTypes = new List<BitMexCommandType>();
        this.underCommandTypes = new List<BitMexCommandType>();

        foreach (var command in bitmexMain.CommandRepository.GetCommands())
        {
            switch (command.Key)
            {
                case BitMexCommandType.MarketSpecified10PriceBuy:
                case BitMexCommandType.MarketSpecified25PriceBuy:
                case BitMexCommandType.MarketSpecified50PriceBuy:
                case BitMexCommandType.MarketSpecified100PriceBuy:
                    this.overCommandTypes.Add(command.Key);
                    break;
                case BitMexCommandType.MarketSpecified10PriceSell:
                case BitMexCommandType.MarketSpecified25PriceSell:
                case BitMexCommandType.MarketSpecified50PriceSell:
                case BitMexCommandType.MarketSpecified100PriceSell:
                    this.underCommandTypes.Add(command.Key);
                    break;
            }
        }

        this.trades = new ConcurrentQueue<BreakThroughTrade>();
        this.producer = new Thread(CheckCoinPrice);
        this.producer.IsBackground = true;
        this.producer.Start();

        this.customer = new Thread(SyncSpecificCoinVariable);
        this.customer.IsBackground = true;
        this.customer.Start();
    }

    private void CheckCoinPrice() 
    {
        while (true)
        {
            try
            {
                if (this.bitmexMain.DriverService.IsDriverOpen() == true)
                {
                    BreakThroughTrade trade = null;
                    if (this.trades.TryDequeue(out trade) == true)
                    {
                        if (trade.IsCompletePriceConditions == true)
                        {
                            if (this.bitmexMain.CommandExecutor.AddCommand(trade.Command) == false)
                            {
                            }
                        }
                        else
                        {
                            this.trades.Enqueue(trade);
                        }
                    }
                }
            }
            catch(Exception e)
            {
            }

            System.Threading.Thread.Sleep(50);
        }
    }

    private void SyncSpecificCoinVariable() 
    {
        decimal price = decimal.Parse("5320.0");

        while (true)
        {
            try
            {
                if (this.bitmexMain.DriverService.IsDriverOpen() == true)
                {
                    if (this.bitmexMain.CommandHandler.HandleIsTradingPage() == true)
                    {
                        //var wc = new System.Diagnostics.Stopwatch();
                        //wc.Start();

                        foreach (var coin in this.bitmexMain.CoinTable.Coins)
                        {
                            if (coin.Key.Equals("XBTUSD") == true)
                            {
                                if (decimal.Parse(coin.Value.MarketPrice) == price)
                                {
                                    break;
                                }

                                if (decimal.Parse(coin.Value.MarketPrice) > price)
                                {
                                    var commandType = this.underCommandTypes[0];
                                    AddTrade(coin.Value, TradeType.Under, price, commandType);
                                }
                                else
                                {
                                    var commandType = this.overCommandTypes[0];
                                    AddTrade(coin.Value, TradeType.Over, price, commandType);
                                }
                            }
                        }

                        //wc.Stop();
                        //Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));
                    }
                }
            }
            catch (Exception e)
            {
            }

            System.Threading.Thread.Sleep(300);
        }
    }

    //private void DoWork()
    //{
    //    while (true)
    //    {
    //        var trade = this.trades.Take();

    //        if (trade.IsCompletePriceConditions == true)
    //        {
    //            if (this.bitmexMain.CommandExecutor.AddCommand(trade.Command) == false)
    //            {
    //            }
    //        }
    //        else
    //        {
    //            this.trades.Add(trade);
    //        }
    //    }
    //}

    public void AddTrade(BitMexCoin coin, TradeType tradeType, decimal price, BitMexCommandType commandType)
    {
        if (commandType == BitMexCommandType.None)
        {
            return;
        }

        var command = this.bitmexMain.CommandRepository.CreateCommand(commandType);

        this.trades.Enqueue(new BreakThroughTrade()
        {
            Coin = coin,
            Type = tradeType,
            Price = price,
            Command = command,
        });
    }

    //public void Test()
    //{
    //    AddTrade("XBTUSD", TradeType.Over, 2556.1M, BitMexCommandType.MarketSpecified10PriceSell);
    //}

    //public void Test()
    //{
    //    if (this.marketPrice == this.inputPrice)
    //    {
    //        return;
    //    }

    //    if (this.marketPrice > this.inputPrice)
    //    {
    //        var commandType = this.underCommandTypes[0];
    //        AddTrade(TradeType.Under, this.inputPrice, commandType);
    //    }
    //    else
    //    {
    //        var commandType = this.overCommandTypes[0];
    //        AddTrade(TradeType.Over, this.inputPrice, commandType);
    //    }

    //    AddTrade("XBTUSD", TradeType.Over, 2556.1M, BitMexCommandType.MarketSpecified10PriceSell);
    //}

}
