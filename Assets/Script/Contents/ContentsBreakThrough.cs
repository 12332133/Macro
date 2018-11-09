using Assets.BitMex;
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
        public decimal Price { get; private set; }
        public IBitMexActionCommand Command { get; private set; }
        public TradeType Type { get; private set; }

        public BreakThroughTrade(TradeType type, decimal price, IBitMexActionCommand command)
        {
            Type = type;
            Price = price;
            Command = command;
        }
    }

    private Thread thread;
    private ConcurrentQueue<BreakThroughTrade> trades;
    private List<BitMexCommandType> overCommandTypes;
    private List<BitMexCommandType> underCommandTypes;
    private decimal marketPrice;
    private decimal beforePrice;
    private decimal inputPrice;

    private void Reset()
    {
    }

    private void Awake()
    {
        this.trades = new ConcurrentQueue<BreakThroughTrade>();

        //this.thread = new Thread(DoWork);
        //this.thread.IsBackground = true;
        //this.thread.Start();
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

        //StartCoroutine(UpdateMarketPrice());
    }

    IEnumerator UpdateMarketPrice() // main으로 이동 ?
    {
        while (true)
        {
            if (this.bitmexMain.DriverService.IsInvaildEmail(this.bitmexMain.Session.Email) == true)
            {
                this.marketPrice = this.bitmexMain.DriverService.OperationGetMarketPrice();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool IsCompletePriceConditions(BreakThroughTrade trade)
    {
        switch (trade.Type)
        {
            case TradeType.Over:
                return this.marketPrice < trade.Price;
            case TradeType.Under:
                return this.marketPrice > trade.Price;
        }

        return false;
    }

    private void DoWork()
    {
        BreakThroughTrade trade = null;

        while (true)
        {
            if (this.trades.TryDequeue(out trade) == true)
            {
                if (IsCompletePriceConditions(trade) == true)
                {
                    if (this.bitmexMain.CommandExecutor.AddCommand(trade.Command) == false)
                    {
                        //log
                    }
                }
                else
                {
                    this.trades.Enqueue(trade);
                }
            }

            Thread.Sleep(50);
        }
    }

    public void AddTrade(TradeType type, decimal inputPrice, BitMexCommandType commandType)
    {
        if (commandType == BitMexCommandType.None)
        {
            return;
        }

        var command = this.bitmexMain.CommandRepository.CreateCommand(commandType);
        var trade = new BreakThroughTrade(type, inputPrice, command);
        this.trades.Enqueue(trade);
    }

    public void Test()
    {
        if (this.marketPrice == this.inputPrice)
        {
            return;
        }

        if (this.marketPrice > this.inputPrice)
        {
            var commandType = this.underCommandTypes[0];
            AddTrade(TradeType.Under, this.inputPrice, commandType);
        }
        else
        {
            var commandType = this.overCommandTypes[0];
            AddTrade(TradeType.Over, this.inputPrice, commandType);
        }

    }
}
