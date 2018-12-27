using Assets.BitMex;
using Assets.BitMex.Commands;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAlarm : ContentsBase
{
    public class ReservationAlram : IBitMexSchedule
    {
        public ContentsMacroAlarmItem Item { get; set; }
        public decimal TargetPrice { get; set; }
        public decimal MomentPrice { get; set; }
        public ExecuteType ExecuteType { get; set; }
        public string CoinName { get; set; }
        public int AlramCount { get; set; }
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

    [SerializeField] private Button btnAdd;
    [SerializeField] private Text txtAdd;

    [SerializeField] private ScrollRect svAlarm;
    [SerializeField] private GameObject goAlarmItem;

    [SerializeField] private GameObject goPopup;

    //private ModifyCommandCoinTypePopup<object> popupDropdown;
    //private ContentsPopupMessage popupMessage;

    private Dictionary<string, List<ReservationAlram>> schedules;

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

    private void OnEnable()
    {
    }

    private void OnApplicationQuit()
    {
    }

    public override void Save()
    {
        SaveLocalCache();
    }

    public void Update()
    {
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        SetSchedule();

        OnRefreshReservationItem();

        btnAdd.onClick.AddListener(OnClickAdd);
    }

    private void SetSchedule()
    {
        this.schedules = new Dictionary<string, List<ReservationAlram>>();

        foreach (var symbol in this.bitmexMain.Session.ActivateSymbols)
        {
            this.schedules.Add(symbol, new List<ReservationAlram>());
        }

        LoadLocalCache(); 
    }

    private ContentsMacroAlarmItem CreateHotKeyItem(ReservationAlram alram)
    {
        var go = Instantiate(this.goAlarmItem);

        var item = go.GetComponent<ContentsMacroAlarmItem>().Initialized();

        item.RefAlram = alram;
        if (alram != null) alram.Item = item;

        item.OnChangePrice = OnChangePrice;
        item.OnChangeRunning = OnChangeRunning;
        item.OnChangeCoinType = OnChangeCoinType;
        item.OnChangeAlramCount = OnChangeAlramCount;
        item.OnRemoveItem = OnRemoveItem;

        item.RefreshCoinDropdown(this.bitmexMain.Session.ActivateSymbols);
        item.RefreshMarketPrice();
        item.RefreshAlramCountDropdown(5);
        item.RefreshStart();

        go.transform.SetParent(this.svAlarm.content.transform);
        return item;
    }

    public void AddSchedule(ReservationAlram alram)
    {
        if (this.schedules.ContainsKey(alram.CoinName) == false)
        {
            this.schedules.Add(alram.CoinName, new List<ReservationAlram>());
        }

        this.schedules[alram.CoinName].Add(alram);
    }

    public ReservationAlram ResisterAlram(string coinName, decimal targetPrice, decimal marketPrice, int alramCount, ContentsMacroAlarmItem item)
    {
        var schedule = new ReservationAlram()
        {
            CoinName = coinName,
            ExecuteType = targetPrice > marketPrice ? ExecuteType.PriceOver : ExecuteType.PriceUnder,
            MomentPrice = marketPrice,
            TargetPrice = targetPrice,
            AlramCount = alramCount,
            IsStart = false,
            Item = item,
        };

        Debug.Log(string.Format("resister alram marketprice {0}", marketPrice.ToString()));
        Debug.Log(string.Format("resister alram executeType {0}", schedule.ExecuteType.ToString()));

        AddSchedule(schedule);

        return schedule;
    }

    private void OnClickAdd()
    {
        CreateHotKeyItem(null);
        Debug.Log(string.Format("OnClickAdd"));
    }

    public void OnRefreshReservationItem()
    {
        foreach (var item in this.svAlarm.content.transform.GetComponentsInChildren<ContentsMacroAlarmItem>())
        {
            Destroy(item.gameObject);
        }

        int count = 0;
        foreach (var alrams in this.schedules.Values)
        {
            foreach (var alram in alrams)
            {
                if (alram.IsRemove == false)
                {
                    CreateHotKeyItem(alram);
                    count++;
                }
            }
        }

        for (int i = 0; i < 5 - count; i++)
        {
            CreateHotKeyItem(null);
        }
    }

    public void RemoveAlram(ReservationAlram alram)
    {
        alram.IsStart = false;
        alram.IsRemove = true;
        Debug.Log(string.Format("remove alram"));
    }

    private void OnRemoveItem(ContentsMacroAlarmItem item)
    {
        if (item.RefAlram != null)
        {
            RemoveAlram(item.RefAlram);
        }

        Destroy(item.gameObject);
    }

    /// <summary>
    /// 시작/정지
    /// </summary>
    /// <param name="item"></param>
    private void OnChangeRunning(ContentsMacroAlarmItem item)
    {
        if (item.RefAlram != null)
        {
            //if (this.bitmexMain.Session.IsLogined == false)
            //{
            //    this.bitmexMain.PopupMessage.OnEnablePopup("비트맥스에 로그인 해주세요");
            //    return;
            //}

            if (item.RefAlram.IsStart == true)
            {
                item.RefAlram.IsStart = false;
                item.RunningState = "시작";
            }
            else
            {
                //var coin = this.bitmexMain.CoinTable.GetCoin(item.RefAlram.CoinName);
                //if (item.RefAlram.IsVaildMomentPrice(coin.MarketPrice) == false)
                //{
                //    this.bitmexMain.PopupMessage.OnEnablePopup("설정 시점의 시장가와 현재 시장가의 차이가 큽니다. 목표 시장가를 다시 설정해 주세요");
                //    return;
                //}

                item.RefAlram.IsStart = true;
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
    private void OnChangePrice(ContentsMacroAlarmItem item)
    {
        var trade = this.bitmexMain.Session.Trades[item.CoinName];

        if (item.RefAlram == null)
        {
            // 커맨드 선택, 코인 선택이 끝났으면
            if (item.CoinName.Equals(string.Empty) == false)
            {
                item.RefAlram = ResisterAlram(
                    item.CoinName,
                    decimal.Parse(item.Price, System.Globalization.NumberStyles.Any),
                    trade.Price,
                    item.AlramCount,
                    item);

                OnRefreshReservationItem();
            }
        }
        else
        {
            item.RefAlram.TargetPrice = decimal.Parse(item.Price, System.Globalization.NumberStyles.Any);
            item.RefAlram.ExecuteType = item.RefAlram.TargetPrice > trade.Price ? ExecuteType.PriceOver : ExecuteType.PriceUnder;
            item.RefAlram.MomentPrice = trade.Price;
        }
    }

    /// <summary>
    /// 코인 타입 변경
    /// </summary>
    /// <param name="item"></param>
    private void OnChangeCoinType(ContentsMacroAlarmItem item)
    {
        if (item.RefAlram == null)
        {
            var coin = this.bitmexMain.Session.Trades[item.CoinName];

            // 시장가 입력, 커맨드 선택 완료 
            if (item.Price.Equals(string.Empty) == false)
            {
                item.RefAlram = ResisterAlram(
                    item.CoinName,
                    decimal.Parse(item.Price, System.Globalization.NumberStyles.Any),
                    coin.Price,
                    item.AlramCount,
                    item);

                OnRefreshReservationItem();
            }
        }
        else
        {
            item.RefAlram.CoinName = item.CoinName;
        }
    }

    /// <summary>
    /// 알랏 횟수 수정
    /// </summary>
    /// <param name="item"></param>
    private void OnChangeAlramCount(ContentsMacroAlarmItem item)
    {
        if (item.RefAlram == null)
        {
            var trade = this.bitmexMain.Session.Trades[item.CoinName];

            // 시장가 입력, 커맨드 선택 완료 
            if (item.Price.Equals(string.Empty) == false)
            {
                item.RefAlram = ResisterAlram(
                    item.CoinName,
                    decimal.Parse(item.Price, System.Globalization.NumberStyles.Any),
                    trade.Price,
                    item.AlramCount,
                    item);

                OnRefreshReservationItem();
            }
        }
        else
        {
            item.RefAlram.AlramCount = item.AlramCount;
        }
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
                        Task.Run(() =>
                        {
                            for (int i = 0; i < schedule.AlramCount; i++)
                            {
                                Thread.Sleep(300);
                            }
                        });

                        this.bitmexMain.PopupMessage.OnEnablePopup(string.Format("execute alram {0}", trade.Price));
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
