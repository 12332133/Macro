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

        public bool IsCompletePriceConditions(decimal currentPrice)
        {
            switch (this.ExecuteType)
            {
                case ExecuteType.PriceOver:
                    return this.TargetPrice <= currentPrice;
                case ExecuteType.PriceUnder:
                    return this.TargetPrice >= currentPrice;
            }
            return false;
        }

        // 예약을 작성한 시점의 시장가와 실제 예약을 실행시킬 시점에서의 시장가의 위치가 맞는가
        public bool IsVaildMomentPrice(decimal currentPrice)
        {
            switch (this.ExecuteType)
            {
                case ExecuteType.PriceOver:
                    return this.TargetPrice < currentPrice;
                case ExecuteType.PriceUnder:
                    return this.TargetPrice > currentPrice;
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
    
    //[SerializeField] private GameObject goPopup;

    //private ModifyCommandPercentPopup<IBitMexCommand> popupInput;
    //private ModifyCommandCoinTypePopup<IBitMexCommand> popupDropdown;
    //private ContentsPopupMessage popupMessage;

    private BitMexCommandTable commandTable;
    private Dictionary<string, List<ReservationTrade>> schedules;

    private void Reset()
    {
        this.txtCurrentTitle = transform.Find("Panel/Current/Title").GetComponent<Text>();
        this.txtCurrentValue = transform.Find("Panel/Current/Value").GetComponent<Text>();

        this.btnAdd = transform.Find("Panel/List/Add").GetComponent<Button>();
        this.txtAdd = transform.Find("Panel/List/Add/Text").GetComponent<Text>();

        this.svBreakThrough = transform.Find("Panel/List/Scroll View").GetComponent<ScrollRect>();
        this.goBreakThroughItem = Resources.Load<GameObject>("MacroBreakThroughItem");

        //this.goPopup = transform.Find("Panel/Popup").gameObject;
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {
    }

    private void OnApplicationQuit()
    {
    }

    public override void Save()
    {
        this.commandTable?.SaveLocalCache();
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);
        
        //this.popupInput = new ModifyCommandPercentPopup<IBitMexCommand>(this.goPopup.transform.GetChild(0));
        //this.popupDropdown = new ModifyCommandCoinTypePopup<IBitMexCommand>(this.goPopup.transform.GetChild(1), this.bitmexMain.CoinTable);
        //this.popupMessage = new ContentsPopupMessage(this.goPopup.transform.GetChild(2));

        SetSchedule();
        SetCommand();

        OnRefreshReservationItem();

        btnAdd.onClick.AddListener(OnClickAdd);
    }

    private void SetSchedule()
    {
        this.schedules = new Dictionary<string, List<ReservationTrade>>();

        foreach (var symbol in this.bitmexMain.Session.ActivateSymbols)
        {
            this.schedules.Add(symbol, new List<ReservationTrade>());
        }

        // load 
        LoadLocalCache();
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

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

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

        var item = go.GetComponent<ContentsMacroBreakThroughItem>().Initialized();

        item.RefTrade = trade;
        if (trade != null) trade.Item = item;

        item.OnChangePrice = OnChangePrice;
        item.OnChangeRunning = OnChangeRunning;
        item.OnChangeCoinType = OnChangeCoinType;
        item.OnChangeCommand = OnChangeCommand;
        item.OnRemoveItem = OnRemoveItem;

        item.RefreshCommandDropdown(this.commandTable.GetCommandsByTableType(BitMexCommandTableType.Percent));
        item.RefreshCoinDropdown(this.bitmexMain.Session.ActivateSymbols);
        item.RefreshMarketPrice();
        item.RefreshStart();

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
            item.RefreshCommandDropdown(this.commandTable.GetCommandsByTableType(BitMexCommandTableType.Percent));
        }
    }

    private void AddSchedule(ReservationTrade trade)
    {
        if (this.schedules.ContainsKey(trade.CoinName) == false)
        {
            this.schedules.Add(trade.CoinName, new List<ReservationTrade>());
        }

        this.schedules[trade.CoinName].Add(trade);
    }

    public ReservationTrade ResisterTrade(string coinName, decimal targetPrice, decimal marketPrice, IBitMexCommand command, ContentsMacroBreakThroughItem item)
    {
        var schedule = new ReservationTrade()
        {
            CoinName = coinName,
            ExecuteType = targetPrice > marketPrice ? ExecuteType.PriceOver : ExecuteType.PriceUnder,
            MomentPrice = marketPrice,
            TargetPrice = targetPrice,
            Command = command,
            IsStart = false,
            Item = item,
        };

        Debug.Log(string.Format("resister trade marketprice {0}", marketPrice.ToString()));
        Debug.Log(string.Format("resister trade executeType {0}", schedule.ExecuteType.ToString()));

        AddSchedule(schedule);

        return schedule;
    }

    public void RemoveTrade(ReservationTrade trade)
    {
        trade.IsStart = false;
        trade.IsRemove = true;
        Debug.Log(string.Format("remove trade"));
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

    private void OnRemoveItem(ContentsMacroBreakThroughItem item)
    {
        if (item.RefTrade != null)
        {
            RemoveTrade(item.RefTrade);
        }

        Destroy(item.gameObject);
    }

    /// <summary>
    /// 시작/정지
    /// </summary>
    /// <param name="item"></param>
    private void OnChangeRunning(ContentsMacroBreakThroughItem item)
    {
        if (item.RefTrade != null)
        {
            //if (this.bitmexMain.Session.IsLogined == false)
            //{
            //    this.bitmexMain.PopupMessage.OnEnablePopup("비트맥스에 로그인 해주세요");
            //    return;
            //}

            if (item.RefTrade.IsStart == true)
            {
                item.RefTrade.IsStart = false;
                item.RunningState = "시작";
            }
            else
            {
                //var coin = this.bitmexMain.CoinTable.GetCoin(item.RefTrade.CoinName);
                //if (item.RefTrade.IsVaildMomentPrice(coin.MarketPrice) == false)
                //{
                //    this.bitmexMain.PopupMessage.OnEnablePopup("설정 시점의 시장가와 현재 시장가의 차이가 큽니다. 목표 시장가를 다시 설정해 주세요");
                //    return;
                //}

                item.RefTrade.IsStart = true;
                item.RunningState = "정지";
            }
        }
        else
        {
            item.RunningState = "시작";
        }
    }

    /// <summary>
    /// 시장가 설정 
    /// </summary>
    /// <param name="item"></param>
    private void OnChangePrice(ContentsMacroBreakThroughItem item)
    {
        var trade = this.bitmexMain.Session.Trades[item.CoinName];

        if (item.RefTrade == null)
        {
            // 커맨드 선택, 코인 선택이 끝났으면
            if (item.TempCommand != null && item.CoinName.Equals(string.Empty) == false) 
            {
                item.RefTrade = ResisterTrade(
                    item.CoinName,
                    decimal.Parse(item.Price, System.Globalization.NumberStyles.Any),
                    trade.Price,
                    item.TempCommand, 
                    item);

                OnRefreshReservationItem();
            }
        }
        else
        {
            item.RefTrade.TargetPrice = decimal.Parse(item.Price, System.Globalization.NumberStyles.Any);
            item.RefTrade.ExecuteType = item.RefTrade.TargetPrice > trade.Price ? ExecuteType.PriceOver : ExecuteType.PriceUnder;
            item.RefTrade.MomentPrice = trade.Price;
        }
    }

    /// <summary>
    /// 코인 타입 변경
    /// </summary>
    /// <param name="item"></param>
    private void OnChangeCoinType(ContentsMacroBreakThroughItem item)
    {
        if (item.RefTrade == null)
        {
            var coin = this.bitmexMain.CoinTable.GetCoin(item.CoinName);

            // 시장가 입력, 커맨드 선택 완료 
            if (item.Price.Equals("시장가 입력") == false && item.TempCommand != null) 
            {
                item.RefTrade = ResisterTrade(
                    item.CoinName,
                    decimal.Parse(item.Price, System.Globalization.NumberStyles.Any),
                    coin.MarketPrice,
                    item.TempCommand,
                    item);

                OnRefreshReservationItem();
            }
        }
        else
        {
            item.RefTrade.CoinName = item.CoinName;
        }
    }

    /// <summary>
    /// 커맨드 변경 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="commandIndex"></param>
    private void OnChangeCommand(ContentsMacroBreakThroughItem item, int commandIndex)
    {
        var command = this.commandTable.FindCommand(BitMexCommandTableType.Percent, commandIndex); // 선택한 커맨드

        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        this.bitmexMain.PopupInput.OnEnablePopup(
              command,
              command.Parameters[0].ToString(),
              (e, v) => 
              {
                  var newCommand = command.Clone();
                  newCommand.Parameters.Clear();
                  newCommand.Parameters.Add(v);

                  this.commandTable.Insert(newCommand);

                  SetCommand(item, newCommand);
              },
              (e, v) =>
              {
                  command.Parameters.Clear();
                  command.Parameters.Add(v);

                  this.commandTable.ModifyCommand(command);

                  SetCommand(item, command);
              },
              (e, v) =>
              {
                  // CommandTable의 커스텀 커맨드만 삭제한다.
                  if (this.commandTable.Remove(command) == false)
                  {
                      // 삭제 불가능한 커맨드 팝업창 출력.
                      this.bitmexMain.PopupMessage.OnEnablePopup("삭제 불가능한 명령");
                      return;
                  }

                  RemoveTradeByCommand(command);

                  OnRefreshReservationItem();
              });
    }

    private void SetCommand(ContentsMacroBreakThroughItem item, IBitMexCommand command)
    {
        if (item.RefTrade == null) // 최초 생성이면 
        {
            // 시장가 입력 + 코인 선택이면 추가
            if (item.Price.Equals("시장가 입력") == false && item.CoinName.Equals(string.Empty) == false) 
            {
                var coin = this.bitmexMain.Session.Trades[item.CoinName];

                item.RefTrade = ResisterTrade(
                    item.CoinName,
                    decimal.Parse(item.Price, System.Globalization.NumberStyles.Any),
                    coin.Price,
                    command,
                    item);

                OnRefreshReservationItem();
                return;
            }
            else // 완성 조합키 없이 커맨드만 선택 했으면 
            {
                item.TempCommand = command;
                Debug.Log(string.Format("cache command"));
            }
        }
        else // 기존 매크로 수정이면 새로 선택/수정 한 커맨드를 바로 참조
        {
            item.RefTrade.Command = command;
            Debug.Log(string.Format("command modifyed"));
        }

        OnRefreshDropdown();
    }

    public void UpdateSchedule()
    {
        foreach (var schedules in this.schedules)
        {
            var trade = this.bitmexMain.Session.Trades[schedules.Key];

            foreach (var schedule in schedules.Value)
            {
                if (schedule.IsStart == true && schedule.IsRemove == false)
                {
                    if (schedule.IsCompletePriceConditions(trade.Price) == true)
                    {

                        //var newCommand = schedule.Command.Clone();
                        //newCommand.Parameters.Add(coin.RootCoinName);
                        //newCommand.Parameters.Add(coin.CoinName);

                        //if (this.bitmexMain.CommandExecutor.AddCommand(newCommand) == true)
                        //{
                        //    Debug.Log(string.Format("execute price schedule"));
                        //}

                        this.bitmexMain.PopupMessage.OnEnablePopup("execute price schedule");
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
