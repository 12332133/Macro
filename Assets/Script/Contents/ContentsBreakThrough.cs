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
        public decimal Price { get; set; }
        public IBitMexCommand Command { get; set; }
        public string CoinName { get; set; }
        public bool IsStart { get; set; }

        public bool IsCompletePriceConditions(decimal marketPrice)
        {
            //if (this.Price > marketPrice)

            //switch (this.ExecuteType)
            //{
            //    case ExecuteType.PriceOver:
            //        return this.Price > marketPrice;
            //    case ExecuteType.PriceUnder:
            //        return this.Price < marketPrice;
            //}
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

    private BitMexCommandTable commandTable;
    private ConcurrentQueue<ReservationTrade> schedules;

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

        SetCommand();

        OnRefreshMacroItem();

        btnAdd.onClick.AddListener(OnClickAdd);
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
            OnCommandChange,
            OnResisterSchedule,
            this.commandTable, 
            trade);

        go.transform.SetParent(this.svBreakThrough.content.transform);
        return item;
    }

    private void OnClickAdd()
    {
        CreateHotKeyItem(null);
    }

    private void OnRefreshMacroItem()
    {
        foreach (var item in this.svBreakThrough.content.transform.GetComponentsInChildren<ContentsMacroBreakThroughItem>())
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            CreateHotKeyItem(null);
        }
    }

    public ReservationTrade OnResisterSchedule(string coinName, decimal price, IBitMexCommand command)
    {
        var schedule = new ReservationTrade()
        {
            CoinName = coinName,
            Price = price,
            Command = command,
            IsStart = true,
        };

        this.schedules.Enqueue(schedule);

        return schedule;
    }

    private void OnRefreshDropdown()
    {
        foreach (var item in this.svBreakThrough.content.transform.GetComponentsInChildren<ContentsMacroBreakThroughItem>())
        {
            item.RefreshCommandDropdown();
        }
    }

    private void OnCommandChange(IBitMexCommand command, Action<IBitMexCommand> modify)
    {
        this.popupInput.OnEnablePopup(command.Parameters[0].ToString(),
                   (value) => //add
                    {
                       var newCommand = command.Clone();
                       newCommand.Parameters.Clear();
                       newCommand.Parameters.Add(Int32.Parse(value));
                       this.commandTable.InsertAt(newCommand);
                       modify(newCommand);

                       OnRefreshDropdown();
                   },
                   (value) => //edit
                    {
                       command.Parameters.Clear();
                       command.Parameters.Add(Int32.Parse(value));
                       modify(command);

                       OnRefreshDropdown();
                   },
                   (value) => //del
                    {
                        // CommandTable의 커스텀 커맨드만 삭제한다.
                       if (this.commandTable.Remove(command) == false)
                       {
                            // 삭제 불가능한 커맨드 팝업창 출력.
                            return;
                       }

                        // Macro에서 커맨드를 참조중인놈을 찾아서 삭제한다.
                        // this.macroTable.RemoveByCommand(command);

                       OnRefreshMacroItem();
                   });
    }

    public void UpdateSchedules()
    {
        foreach (var schedule in this.schedules)
        {
            if (schedule.IsStart == true)
            {
                var coin = this.bitmexMain.CoinTable.GetCoin(schedule.CoinName);

                if (schedule.IsCompletePriceConditions(coin.MarketPrice) == true)
                {
                    ReservationTrade outTrade;
                    if (this.schedules.TryDequeue(out outTrade) == true)
                    {
                        var newCommand = outTrade.Command.Clone();
                        newCommand.Parameters.Add(coin.RootCoinName);
                        newCommand.Parameters.Add(coin.CoinName);

                        if (this.bitmexMain.CommandExecutor.AddCommand(newCommand) == true)
                        {
                            Debug.Log(string.Format("execute price schedule"));
                        }
                    }
                }
            }
        }
    }
}
