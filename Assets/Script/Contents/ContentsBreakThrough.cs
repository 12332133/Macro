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
    public class BreakThroughTrade : IBitMexSchedule
    {
        public decimal Price { get; set; }
        public ExecuteType ExecuteType { get; set; }
        public IBitMexCommand Command { get; set; }
        public IBitMexMainAdapter BitmexMain { get; set; }
        public BitMexCoin Coin { get; set; }

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
            if (IsCompletePriceConditions)
            {
                return true;
                //return BitmexMain.CommandExecutor.AddCommand(Command);
            }

            return false;
        }
    }

    [SerializeField] private Text txtCurrentTitle;
    [SerializeField] private Text txtCurrentValue;

    [SerializeField] private Button btnAdd;
    [SerializeField] private Text txtAdd;

    [SerializeField] private ScrollRect svBreakThrough;
    [SerializeField] private GameObject goBreakThroughItem;
    
    [SerializeField] private GameObject goPopup;

    private ContentsPopupInput popupInput;
    private ContentsPopupDropdown popupDropdown;
    private ContentsPopupMessage popupMessage;

    private void Reset()
    {
        this.txtCurrentTitle = transform.Find("Panel/Current/Title").GetComponent<Text>();
        this.txtCurrentValue = transform.Find("Panel/Current/Value").GetComponent<Text>();

        this.btnAdd = transform.Find("Panel/List/Add").GetComponent<Button>();
        this.txtAdd = transform.Find("Panel/List/Add/Text").GetComponent<Text>();

        this.svBreakThrough = transform.Find("Panel/List/Scroll View").GetComponent<ScrollRect>();
        this.goBreakThroughItem = Resources.Load<GameObject>("MacroBreakThroughItem");

        this.goPopup = transform.Find("Panel/Popup").gameObject;
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

        /*
        new Thread(e => {

            var commands = this.bitmexMain.CommandTable.GetCommands(BitMexCommandTableType.Percent);
            var command = commands[3];
            var coin = this.bitmexMain.CoinTable.GetCoin("XBTUSD");

            while (true)
            {
                AddSchedule(coin, ExecuteType.PriceUnder, 0, command);
                Thread.Sleep(1000);
            }
        })
        {
            IsBackground = true,
        }.Start();
        */

        this.popupInput = new ContentsPopupInput(this.goPopup.transform.GetChild(0));
        this.popupDropdown = new ContentsPopupDropdown(this.goPopup.transform.GetChild(1), this.bitmexMain.CoinTable);
        this.popupMessage = new ContentsPopupMessage(this.goPopup.transform.GetChild(2));

        btnAdd.onClick.AddListener(OnClickAdd);
    }

    //private void Produce() 
    //{
    //    decimal price = decimal.Parse("5493.5", System.Globalization.NumberStyles.Any);

    //    while (true)
    //    {
    //        try
    //        {
    //            if (this.bitmexMain.DriverService.IsDriverOpen() == true &&
    //                this.bitmexMain.DriverService.IsTradingPage() == true)
    //            {
    //                //var wc = new System.Diagnostics.Stopwatch();
    //                //wc.Start();

    //                foreach (var coin in this.bitmexMain.CoinTable.Coins)
    //                {
    //                    if (coin.Key.Equals("XBTUSD") == true)
    //                    {
    //                        if (coin.Value.MarketPrice == price)
    //                        {
    //                            break;
    //                        }

    //                        if (coin.Value.MarketPrice > price)
    //                        {
    //                            var commandType = this.underCommands[0];
    //                            AddTrade(coin.Value, TradeType.Under, price, commandType);

    //                        }
    //                        else
    //                        {
    //                            var commandType = this.overCommands[0];
    //                            AddTrade(coin.Value, TradeType.Over, price, commandType);
    //                        }
    //                    }
    //                }

    //                //wc.Stop();
    //                //Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //        }

    //        System.Threading.Thread.Sleep(300);
    //    }
    //}

    public void AddSchedule(BitMexCoin coin, ExecuteType type, decimal price, IBitMexCommand command)
    {
        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        var schedule = new BreakThroughTrade()
        {
            Coin = coin,
            ExecuteType = type,
            Price = price,
            Command = command,
            BitmexMain = bitmexMain,
        };

        this.bitmexMain.ResisterSchedule(schedule);
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

    private void OnClickAdd()
    {
        var go = Instantiate(this.goBreakThroughItem);
        var item = go.GetComponent<ContentsMacroBreakThroughItem>().Initialized();
        go.transform.SetParent(this.svBreakThrough.content.transform);
    }
}
