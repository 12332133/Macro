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

public interface IContentsReservation
{
    BitMexCommandTable CommandTable { get; }
    BitMexCoinTable CoinTable { get; }
    ContentsBase.ContentsPopupInput<IBitMexCommand> PopupInput { get; }
    ContentsBase.ContentsPopupDropdown<IBitMexCommand> PopupDropdown { get; }
    ContentsBase.ContentsPopupMessage PopupAlret { get; }
    ContentsBreakThrough.ReservationTrade ResisterTrade(string coinName, decimal price, IBitMexCommand command, ContentsMacroBreakThroughItem item);
    void RemoveTradeByCommand(IBitMexCommand command);
    void RemoveTrade(ContentsBreakThrough.ReservationTrade trade);
    void OnRefreshDropdown();
    void OnRefreshReservationItem();
}

public class ContentsBreakThrough : ContentsBase, IContentsReservation
{
    public class ReservationTrade : IBitMexSchedule
    {
        public decimal TargetPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public IBitMexCommand Command { get; set; }
        public ExecuteType ExecuteType { get; set; }
        public string CoinName { get; set; }
        public bool IsStart { get; set; }
        public bool IsRemove { get; set; }
        public ContentsMacroBreakThroughItem Item { get; set; }

        public bool IsCompletePriceConditions(decimal marketPrice)
        {
            switch (this.ExecuteType)
            {
                case ExecuteType.PriceOver:
                    return this.TargetPrice >= marketPrice;
                case ExecuteType.PriceUnder:
                    return this.TargetPrice <= marketPrice;
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

    private ContentsPopupInput<IBitMexCommand> popupInput;
    private ContentsPopupDropdown<IBitMexCommand> popupDropdown;
    private ContentsPopupMessage popupMessage;

    private BitMexCommandTable commandTable;
    private ConcurrentQueue<ReservationTrade> schedules;
    private object locked;

    // interface impl
    public BitMexCommandTable CommandTable { get { return this.commandTable; } }
    public BitMexCoinTable CoinTable { get { return this.bitmexMain.CoinTable; } }
    public ContentsPopupInput<IBitMexCommand> PopupInput { get { return this.popupInput; } }
    public ContentsPopupDropdown<IBitMexCommand> PopupDropdown { get { return this.popupDropdown; } }
    public ContentsPopupMessage PopupAlret { get { return this.popupMessage; } }

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
        this.schedules = new ConcurrentQueue<ReservationTrade>();
        
        this.popupInput = new ContentsPopupInput<IBitMexCommand>(this.goPopup.transform.GetChild(0));
        this.popupDropdown = new ContentsPopupDropdown<IBitMexCommand>(this.goPopup.transform.GetChild(1), this.bitmexMain.CoinTable);
        this.popupMessage = new ContentsPopupMessage(this.goPopup.transform.GetChild(2));

        SetCommand();

        OnRefreshReservationItem();

        btnAdd.onClick.AddListener(OnClickAdd);

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
    }

    private void SetCommand()
    {
        this.commandTable = new BitMexCommandTable("schedule_commands");

        //퍼센트
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.LoadLocalCache();
    }

    private ContentsMacroBreakThroughItem CreateHotKeyItem(ReservationTrade trade)
    {
        var go = Instantiate(this.goBreakThroughItem);

        var item = go.GetComponent<ContentsMacroBreakThroughItem>().Initialized(
            BitMexCommandTableType.Percent,
            this,
            trade);

        if (trade != null)
        {
            trade.Item = item;
        }
        
        go.transform.SetParent(this.svBreakThrough.content.transform);
        return item;
    }

    private void OnClickAdd()
    {
        CreateHotKeyItem(null);
    }

    public void OnRefreshReservationItem()
    {
        foreach (var item in this.svBreakThrough.content.transform.GetComponentsInChildren<ContentsMacroBreakThroughItem>())
        {
            Destroy(item.gameObject);
        }

        foreach (var trade in this.schedules)
        {
            if (trade.IsRemove == false)
            {
                CreateHotKeyItem(trade);
            }
        }

        if (this.schedules.Count < 5)
        {
            for (int i = 0; i < 5 - this.schedules.Count; i++)
            {
                CreateHotKeyItem(null);
            }
        }
    }

    public void OnRefreshDropdown()
    {
        foreach (var item in this.svBreakThrough.content.transform.GetComponentsInChildren<ContentsMacroBreakThroughItem>())
        {
            item.RefreshCommandDropdown();
        }
    }

    public ReservationTrade ResisterTrade(string coinName, decimal price, IBitMexCommand command, ContentsMacroBreakThroughItem item)
    {
        var coin = this.bitmexMain.CoinTable.GetCoin(coinName);
        
        if (coin.MarketPrice == 0)
        {
            this.PopupAlret.OnEnablePopup("Chrome 비트맥스가 실행중이 아닙니다.");
            return null;
        }

        var schedule = new ReservationTrade()
        {
            ExecuteType = price > coin.MarketPrice ? ExecuteType.PriceOver : ExecuteType.PriceUnder,
            CoinName = coinName,
            TargetPrice = price,
            Command = command,
            IsStart = false,
            Item = item,
        };

        Debug.Log(string.Format("resister trade marketprice {0}", coin.MarketPrice.ToString()));
        Debug.Log(string.Format("resister trade executeType {0}", schedule.ExecuteType.ToString()));

        this.schedules.Enqueue(schedule);

        return schedule;
    }

    public void RemoveTrade(ReservationTrade trade)
    {
        trade.IsStart = false;
        trade.IsRemove = true;
        Debug.Log(string.Format("remove schedule"));
    }

    public void RemoveTradeByCommand(IBitMexCommand command)
    {
        foreach (var schedule in this.schedules)
        {
            if (schedule.Command == command)
            {
                RemoveTrade(schedule);
            }
        }
    }

    public void UpdateSchedules()
    {
        foreach (var schedule in this.schedules)
        {
            if (schedule.IsStart == true && schedule.IsRemove == false)
            {
                var coin = this.bitmexMain.CoinTable.GetCoin(schedule.CoinName);

                if (schedule.IsCompletePriceConditions(coin.MarketPrice) == true)
                {
                    var newCommand = schedule.Command.Clone();
                    newCommand.Parameters.Add(coin.RootCoinName);
                    newCommand.Parameters.Add(coin.CoinName);

                    //if (this.bitmexMain.CommandExecutor.AddCommand(newCommand) == true)
                    //{
                    //    Debug.Log(string.Format("execute price schedule"));
                    //}

                    schedule.Item.OnClickDelete();

                    Debug.Log(string.Format("execute price schedule"));
                }
            }
        }
    }

    private void SaveLocalCache()
    {
        //isremove == true not save
    }

    private void LoadLocalCache()
    {
    }
}
