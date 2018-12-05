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
    BitMexSession BitMexSession { get; }
    ContentsBase.ModifyCommandPercentPopup<IBitMexCommand> PopupInput { get; }
    ContentsBase.ModifyCommandCoinTypePopup<IBitMexCommand> PopupDropdown { get; }
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
        public ContentsMacroBreakThroughItem Item { get; set; }
        public decimal TargetPrice { get; set; }
        public decimal MomentPrice { get; set; }
        public IBitMexCommand Command { get; set; }
        public ExecuteType ExecuteType { get; set; }
        public string CoinName { get; set; }
        public bool IsStart { get; set; }
        public bool IsRemove { get; set; }

        public bool IsCompletePriceConditions(decimal marketPrice)
        {
            switch (this.ExecuteType)
            {
                case ExecuteType.PriceOver:
                    return this.TargetPrice <= marketPrice;
                case ExecuteType.PriceUnder:
                    return this.TargetPrice >= marketPrice;
            }
            return false;
        }

        // 예약을 작성한 시점의 시장가와 실제 예약을 실행시킬 시점에서의 시장가의 위치가 맞는가
        public bool IsVaildMomentPrice(decimal marketPrice)
        {
            switch (this.ExecuteType)
            {
                case ExecuteType.PriceOver:
                    return this.MomentPrice >= marketPrice;
                case ExecuteType.PriceUnder:
                    return this.MomentPrice <= marketPrice;
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

    private ModifyCommandPercentPopup<IBitMexCommand> popupInput;
    private ModifyCommandCoinTypePopup<IBitMexCommand> popupDropdown;
    private ContentsPopupMessage popupMessage;

    private BitMexCommandTable commandTable;
    private Dictionary<string, List<ReservationTrade>> schedules;

    // interface impl
    public BitMexCommandTable CommandTable { get { return this.commandTable; } }
    public BitMexCoinTable CoinTable { get { return this.bitmexMain.CoinTable; } }
    public BitMexSession BitMexSession { get { return this.bitmexMain.Session; } }
    public ModifyCommandPercentPopup<IBitMexCommand> PopupInput { get { return this.popupInput; } }
    public ModifyCommandCoinTypePopup<IBitMexCommand> PopupDropdown { get { return this.popupDropdown; } }
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
        
        this.popupInput = new ModifyCommandPercentPopup<IBitMexCommand>(this.goPopup.transform.GetChild(0));
        this.popupDropdown = new ModifyCommandCoinTypePopup<IBitMexCommand>(this.goPopup.transform.GetChild(1), this.bitmexMain.CoinTable);
        this.popupMessage = new ContentsPopupMessage(this.goPopup.transform.GetChild(2));

        SetSchedule();
        SetCommand();

        OnRefreshReservationItem();

        btnAdd.onClick.AddListener(OnClickAdd);
    }

    private void SetSchedule()
    {
        this.schedules = new Dictionary<string, List<ReservationTrade>>()
        {
            { "XBT", new List<ReservationTrade>() },
            { "ADA", new List<ReservationTrade>() },
            { "BCH", new List<ReservationTrade>() },
            { "EOS", new List<ReservationTrade>() },
            { "ETH", new List<ReservationTrade>() },
            { "LTC", new List<ReservationTrade>() },
            { "TRX", new List<ReservationTrade>() },
            { "XRP", new List<ReservationTrade>() },
        };

        // load 
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

        int count = 0;
        foreach (var trades in this.schedules.Values)
        {
            foreach (var trade in trades)
            {
                if (trade.IsRemove == false)
                {
                    CreateHotKeyItem(trade);
                    count++;
                }
            }
        }

        for (int i = 0; i < 5 - count; i++)
        {
            CreateHotKeyItem(null);
        }
    }

    public void OnRefreshDropdown()
    {
        foreach (var item in this.svBreakThrough.content.transform.GetComponentsInChildren<ContentsMacroBreakThroughItem>())
        {
            item.RefreshCommandDropdown();
        }
    }

    private void AddSchedule(ReservationTrade trade)
    {
        var coin = this.bitmexMain.CoinTable.GetCoin(trade.CoinName);

        if (this.schedules.ContainsKey(coin.RootCoinName) == false)
        {
            this.schedules.Add(coin.RootCoinName, new List<ReservationTrade>());
        }

        this.schedules[coin.RootCoinName].Add(trade);
    }

    public ReservationTrade ResisterTrade(string coinName, decimal price, IBitMexCommand command, ContentsMacroBreakThroughItem item)
    {
        var coin = this.bitmexMain.CoinTable.GetCoin(coinName);
        
        if (coin.MarketPrice == 0)
        {
            this.PopupAlret.OnEnablePopup("현재 시장가 가격 정보 에러");
            return null;
        }

        var schedule = new ReservationTrade()
        {
            CoinName = coinName,
            ExecuteType = price > coin.MarketPrice ? ExecuteType.PriceOver : ExecuteType.PriceUnder,
            MomentPrice = coin.MarketPrice,
            TargetPrice = price,
            Command = command,
            IsStart = false,
            Item = item,
        };

        Debug.Log(string.Format("resister trade marketprice {0}", coin.MarketPrice.ToString()));
        Debug.Log(string.Format("resister trade executeType {0}", schedule.ExecuteType.ToString()));

        AddSchedule(schedule);

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
        foreach (var schedules in this.schedules.Values)
        {
            foreach (var schedule in schedules)
            {
                if (schedule.Command == command)
                {
                    RemoveTrade(schedule);
                }
            }
        }
    }

    public void UpdateSchedules()
    {
        foreach (var schedules in this.schedules.Values)
        {
            foreach (var schedule in schedules)
            {
                var coin = this.bitmexMain.CoinTable.GetCoin(schedule.CoinName);

                if (schedule.IsStart == true && schedule.IsRemove == false)
                {
                    //Debug.Log(string.Format("reservate b {0},{1},{2}",
                    //    schedule.ExecuteType, 
                    //    schedule.TargetPrice,
                    //    schedule.MomentPrice));
                    
                    if (schedule.IsCompletePriceConditions(coin.MarketPrice) == true)
                    {
                        this.PopupAlret.OnEnablePopup("execute price schedule");

                        Debug.Log(string.Format("reservate e {0}",
                        coin.MarketPrice));

                        var newCommand = schedule.Command.Clone();
                        newCommand.Parameters.Add(coin.RootCoinName);
                        newCommand.Parameters.Add(coin.CoinName);

                        //if (this.bitmexMain.CommandExecutor.AddCommand(newCommand) == true)
                        //{
                        //    Debug.Log(string.Format("execute price schedule"));
                        //}

                        schedule.Item.OnClickDelete();
                    }
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
