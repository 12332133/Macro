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

    public class BreakThroughTrade : IBitMexSchedule
    {
        public decimal Price { get; set; }
        public TradeType Type { get; set; }
        public BitMexCoin Coin { get; set; }
        public IBitMexCommand Command { get; set; }
        public BitMexCommandExecutor Executor { get; set; }

        public bool IsCompletePriceConditions
        {
            get
            {
                switch (this.Type)
                {
                    case TradeType.Over:
                        return this.Coin.MarketPrice < this.Price;
                    case TradeType.Under:
                        return this.Coin.MarketPrice > this.Price;
                }
                return false;
            }
        }

        public bool Execute()
        {
            if (IsCompletePriceConditions)
            {
                return Executor.AddCommand(Command);
            }

            return false;
        }
    }

    private BlockingCollection<BreakThroughTrade> trades;
    private List<IBitMexCommand> overCommands;
    private List<IBitMexCommand> underCommands;

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

        this.overCommands = new List<IBitMexCommand>();
        this.underCommands = new List<IBitMexCommand>();

        foreach (var command in bitmexMain.CommandTable.Commands)
        {
            switch (command.CommandType)
            {
                //case BitMexCommandType.MarketSpecifiedPriceBuy1:
                //case BitMexCommandType.MarketSpecifiedPriceBuy2:
                //case BitMexCommandType.MarketSpecifiedPriceBuy3:
                //case BitMexCommandType.MarketSpecifiedPriceBuy4:
                case BitMexCommandType.MarketSpecifiedPriceBuy:
                    this.overCommands.Add(command);
                    break;
                //case BitMexCommandType.MarketSpecifiedPriceSell1:
                //case BitMexCommandType.MarketSpecifiedPriceSell2:
                //case BitMexCommandType.MarketSpecifiedPriceSell3:
                //case BitMexCommandType.MarketSpecifiedPriceSell4:
                case BitMexCommandType.MarketSpecifiedPriceSell:
                    this.underCommands.Add(command);
                    break;
            }
        }

        this.trades = new BlockingCollection<BreakThroughTrade>();

        //new Thread(Produce)
        //{
        //    IsBackground = true,
        //}.Start();
    }

    private void Produce() 
    {
        decimal price = decimal.Parse("5493.5", System.Globalization.NumberStyles.Any);

        while (true)
        {
            try
            {
                if (this.bitmexMain.DriverService.IsDriverOpen() == true &&
                    this.bitmexMain.DriverService.HandleIsTradingPage() == true)
                {
                    //var wc = new System.Diagnostics.Stopwatch();
                    //wc.Start();

                    foreach (var coin in this.bitmexMain.CoinTable.Coins)
                    {
                        if (coin.Key.Equals("XBTUSD") == true)
                        {
                            if (coin.Value.MarketPrice == price)
                            {
                                break;
                            }

                            if (coin.Value.MarketPrice > price)
                            {
                                var commandType = this.underCommands[0];
                                AddTrade(coin.Value, TradeType.Under, price, commandType);

                            }
                            else
                            {
                                var commandType = this.overCommands[0];
                                AddTrade(coin.Value, TradeType.Over, price, commandType);
                            }
                        }
                    }

                    //wc.Stop();
                    //Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));
                }
            }
            catch (Exception e)
            {
            }

            System.Threading.Thread.Sleep(300);
        }
    }

    public void AddTrade(BitMexCoin coin, TradeType tradeType, decimal price, IBitMexCommand command)
    {
        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        //var command = this.bitmexMain.CommandRepository.CreateCommand(command);

        this.bitmexMain.ResisterSchedule(new BreakThroughTrade()
        {
            Coin = coin,
            Type = tradeType,
            Price = price,
            Command = command,
            Executor = this.bitmexMain.CommandExecutor,
        });

        //this.trades.Add(new BreakThroughTrade()
        //{
        //    Coin = coin,
        //    Type = tradeType,
        //    Price = price,
        //    Command = command,
        //    Executor = this.bitmexMain.CommandExecutor,
        //});
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
