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
        public string CoinName { get; set; }
        public bool IsStart { get; set; }

        public bool IsCompletePriceConditions(decimal marketPrice)
        {
            switch (this.ExecuteType)
            {
                case ExecuteType.PriceOver:
                    return this.Price > marketPrice;
                case ExecuteType.PriceUnder:
                    return this.Price < marketPrice;
            }
            return false;
        }

        //public bool IsCompletePriceConditions
        //{
        //    get
        //    {
        //        switch (this.ExecuteType)
        //        {
        //            case ExecuteType.PriceOver:
        //                return this.Price > this.Coin.MarketPrice;
        //            case ExecuteType.PriceUnder:
        //                return this.Price < this.Coin.MarketPrice;
        //        }
        //        return false;
        //    }
        //}

        //public bool Execute()
        //{
        //    if (IsCompletePriceConditions == true)
        //    {
        //        Task.Run(() => 
        //        {
        //            for (int i = 0; i < this.AlramCount; i++)
        //            {
        //                EditorApplication.Beep();
        //                Thread.Sleep(300);
        //            }
        //        });
        //    }
        //    return false;
        //}
    }

    [SerializeField] private Button btnAdd;
    [SerializeField] private Text txtAdd;

    [SerializeField] private ScrollRect svAlarm;
    [SerializeField] private GameObject goAlarmItem;

    [SerializeField] private GameObject goPopup;

    private ContentsPopupInput popupInput;
    private ContentsPopupDropdown popupDropdown;
    private ContentsPopupMessage popupMessage;

    private void Reset()
    {
        this.btnAdd = transform.Find("Panel/List/Add").GetComponent<Button>();
        this.txtAdd = transform.Find("Panel/List/Add/Text").GetComponent<Text>();

        this.svAlarm = transform.Find("Panel/List/Scroll View").GetComponent<ScrollRect>();
        this.goAlarmItem = Resources.Load<GameObject>("MacroAlarmItem");

        this.goPopup = transform.Find("Panel/Popup").gameObject;
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

        this.popupInput = new ContentsPopupInput(this.goPopup.transform.GetChild(0));
        this.popupDropdown = new ContentsPopupDropdown(this.goPopup.transform.GetChild(1), this.bitmexMain.CoinTable);
        this.popupMessage = new ContentsPopupMessage(this.goPopup.transform.GetChild(2));

        btnAdd.onClick.AddListener(OnClickAdd);
    }

    public void AddSchedule(string coinName, ExecuteType type, decimal price, int alramCount)
    {
        var schedule = new MarketPriceAlram()
        {
            CoinName = coinName,
            ExecuteType = type,
            Price = price,
            AlramCount = alramCount,
            IsStart = false,
        };
    }

    private void OnClickAdd()
    {
        var go = Instantiate(this.goAlarmItem);
        var item = go.GetComponent<ContentsMacroAlarmItem>().Initialized();
        go.transform.SetParent(this.svAlarm.content.transform);
    }

    public void UpdateSchedules()
    {
    }
}
