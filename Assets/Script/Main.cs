using UnityEngine;
using UnityEngine.UI;
using Assets.BitMex;
using Assets.BitMex.WebDriver;
using Assets.KeyBoardHook;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System;
using System.Linq;
using Assets.BitMex.Commands;
using System.Collections;
using Newtonsoft.Json.Linq;

public class Main : MonoBehaviour, IBitMexMainAdapter
{
    [SerializeField] private Button btnBitMex;
    [SerializeField] private Text txtBitMax;
    
    [SerializeField] private Button btnMacro;
    [SerializeField] private Text txtMacro;
    
    [SerializeField] private Text[] txtTabs;
    [SerializeField] private Toggle[] toggleTabs;

    [SerializeField] private ContentsBase[] contents;

    private BitMexSession session;
    private BitMexDriverService service;
    private const string BitMexDomain = "https://testnet.bitmex.com";
    //private const string BitMexDomain = "https://www.bitmex.com/";

    private bool isCombination = false;
    private bool isEnableMacro = false;
    private List<RawKey> inputRawKeys;
    private Dictionary<RawKey, int> allowSpecialKeys;

    private void Reset()
    {
        this.btnBitMex = transform.Find("Canvas/btnBitMax").GetComponent<Button>();
        this.txtBitMax = transform.Find("Canvas/btnBitMax/Text").GetComponent<Text>();

        this.btnMacro = transform.Find("Canvas/btnMacro").GetComponent<Button>();
        this.txtMacro = transform.Find("Canvas/btnMacro/Text").GetComponent<Text>();

        this.txtTabs = transform.Find("Canvas/Tab").GetComponentsInChildren<Text>();
        this.toggleTabs = transform.Find("Canvas/Tab").GetComponentsInChildren<Toggle>();

        this.contents = transform.Find("Canvas/Contents").GetComponentsInChildren<ContentsBase>();
    }

    private void OnApplicationQuit()
    {
        KeyboardHooker.Stop();
        this.service.CloseDriver();
    }

    private void Awake()
    {
        SetUnityOptions();
        SetBitMexService();
        SetAllowSpecialKey();
        SetInputKey();
        SetActiveInstruments();

        this.toggleTabs[0].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[1].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[2].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[3].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[4].onValueChanged.AddListener(OnToggleTab);

        this.btnBitMex.onClick.AddListener(OnOpenBitMex);
        this.btnMacro.onClick.AddListener(OnEnableMacro);

        foreach (var content in this.contents)
        {
            content.Initialize(this);
        }

        OnToggleTab(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void SetUnityOptions()
    {
        Application.runInBackground = true;
    }

    private void SetActiveInstruments()
    {
        var response = BitMexApiHelper.GetActiveInstruments(this.session, BitMexDomain);

        var jarray = JArray.Parse(response);

        foreach (var item in jarray)
        {
            var jobject = JObject.Parse(item.ToString());

            if (jobject["state"].ToString().Equals("Open") == true)
            {
                var rootSymbol = jobject["rootSymbol"].ToString();
                var symbol = jobject["symbol"].ToString();
                this.service.CoinTable.ResisterCoin(rootSymbol, symbol);
            }
        }
    }

    private void SetBitMexService()
    {
        this.service = new BitMexDriverService();

        //command 
        this.service.Repository.Resister(BitMexCommandType.Test, new SampleCommand(this, "Test", true));

        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuy10Magnification, new MarketPriceBuyCommand(this, "시장가 10% 매수", true, 10));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuy25Magnification, new MarketPriceBuyCommand(this, "시장가 25% 매수", true, 25));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuy50Magnification, new MarketPriceBuyCommand(this, "시장가 50% 매수", true, 50));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuy100Magnification, new MarketPriceBuyCommand(this, "시장가 100% 매수", true, 100));

        this.service.Repository.Resister(BitMexCommandType.MarketPriceSell10Magnification, new MarketPriceSellCommand(this, "시장가 10% 매도", true, 10));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSell25Magnification, new MarketPriceSellCommand(this, "시장가 25% 매도", true, 25));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSell50Magnification, new MarketPriceSellCommand(this, "시장가 50% 매도", true, 50));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSell100Magnification, new MarketPriceSellCommand(this, "시장가 100% 매도", true, 100));

        this.service.Repository.Resister(BitMexCommandType.MarketSpecified10PriceBuy, new MarketSpecifiedBuyCommand(this, "빠른 지정가 10% 매수", true, 10));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecified25PriceBuy, new MarketSpecifiedBuyCommand(this, "빠른 지정가 25% 매수", true, 25));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecified50PriceBuy, new MarketSpecifiedBuyCommand(this, "빠른 지정가 50% 매수", true, 50));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecified100PriceBuy, new MarketSpecifiedBuyCommand(this, "빠른 지정가 100% 매수", true, 100));

        this.service.Repository.Resister(BitMexCommandType.MarketSpecified10PriceSell, new MarketSpecifiedSellCommand(this, "빠른 지정가 10% 매도", true, 10));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecified25PriceSell, new MarketSpecifiedSellCommand(this, "빠른 지정가 25% 매도", true, 25));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecified50PriceSell, new MarketSpecifiedSellCommand(this, "빠른 지정가 50% 매도", true, 50));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecified100PriceSell, new MarketSpecifiedSellCommand(this, "빠른 지정가 100% 매도", true, 100));

        this.service.Repository.Resister(BitMexCommandType.ClearPosition, new PositionClearCommand(this, "해당 포지션 청산", true));
        this.service.Repository.Resister(BitMexCommandType.CancleTopActivateOrder, new TopActivateOrderCancleCommand(this, "최상위 주문 취소", true));
        this.service.Repository.Resister(BitMexCommandType.CancleAllActivateOrder, new ActivateOrderCancleCommand(this, "전체 주문 취소", true));

        //session
        this.session = new BitMexSession()
        {
            ApiKey = "TE3O0NLo8pmwAkzsv66UamVr",
            ApiSecret = "yVjWPBWEVmwWZ39bRJ23aLJu5h69Eq4cyQHM6utd-O7Z8qZx",
            Email = "condemonkey@gmail.com",
            ReferrerAccount = "462226",
            ReferrerEmail = "",
        };

        //load macro
        //this.session.ResisterMacro(
        //    new List<RawKey>() { (RawKey)1, (RawKey)2, (RawKey)3, (RawKey)4 }, 
        //    this.repository.CreateCommand((BitMexCommandType)1));
    }

    private void SetAllowSpecialKey()
    {
        this.allowSpecialKeys = new Dictionary<RawKey, int>();
        this.allowSpecialKeys.Add(RawKey.LeftShift, 1);
        this.allowSpecialKeys.Add(RawKey.Shift, 1);
        this.allowSpecialKeys.Add(RawKey.RightShift, 1);
        this.allowSpecialKeys.Add(RawKey.Control, 1);
        this.allowSpecialKeys.Add(RawKey.LeftControl, 1);
        this.allowSpecialKeys.Add(RawKey.RightControl, 1);
        this.allowSpecialKeys.Add(RawKey.Menu, 1);
        this.allowSpecialKeys.Add(RawKey.LeftMenu, 1);
        this.allowSpecialKeys.Add(RawKey.RightMenu, 1);
    }

    private void SetInputKey()
    {
        this.inputRawKeys = new List<RawKey>();
    }

    private void OnToggleTab(bool state)
    {
        if (!state) return;

        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            this.contents[i].gameObject.SetActive(this.toggleTabs[i].isOn);
        }
    }

    private void OnOpenBitMex()
    {
        if (this.service.IsDriverOpen() == false)
        {
            var driver = DriverFactory.CreateDriver(
                  DriverType.Chrome,
                  Application.streamingAssetsPath,
                  false);

            this.service.OpenService(driver, BitMexDomain);

            //StartCoroutine(SyncSpecificCoinVariable());
        }
        else
        {
            try
            {
                foreach (var coin in this.CoinTable.Coins.Values.ToList())
                {
                    var wc = new System.Diagnostics.Stopwatch();
                    wc.Start();
                    if (this.service.HandleChangeCoinTab(coin.RootCoinName, coin.CoinName) == false)
                    {
                        Debug.Log("not found tab");
                    }
                    wc.Stop();
                    Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));
                }

                //var command = this.service.Repository.CreateCommand(BitMexCommandType.ClearPosition);
                //command.Execute();
            }
            catch (BitMexDriverServiceException exception)
            {
                Debug.Log(exception.ToString());
            }
        }

        //if (this.service.IsDriverOpen() == false)
        //{
        //    var driver = DriverFactory.CreateDriver(
        //            DriverType.Chrome,
        //            Application.streamingAssetsPath,
        //            false);

        //    this.service.OpenService(driver, BitMexDomain);

        //    StartCoroutine(SyncSpecificCoinVariable());
        //}
    }

    private IEnumerator SyncSpecificCoinVariable() // main으로 이동 ?
    {
        while (true)
        {
            try
            {
                if (this.service.HandleIsTradingPage() == true)
                {
                    //var wc = new System.Diagnostics.Stopwatch();
                    //wc.Start();
                    this.service.HandleSyncCointPrices();
                    //wc.Stop();
                    //Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnEnableMacro()
    {
        if (this.isEnableMacro == true)
        {
            KeyboardHooker.OnKeyUp -= OnKeyUp;
            KeyboardHooker.OnKeyDown -= OnKeyDown;
            this.isEnableMacro = false;
        }
        else
        {
            /* real service
            if (this.service.IsDriverOpen() == false)
            {
                Debug.Log("not found chrome driver");
                return;
            }

            //var response = BitMexApiHelper.GetReferral(session, BitMexDomain); //json deserialize

            //if (this.session.ReferrerAccount.Equals(response) == false)
            //{
            //    Debug.Log("invaild referral account");
            //    return;
            //}

            if (this.service.IsInvaildEmail(session.Email) == false)
            {
                Debug.Log("invaild login account");
                return;
            }
            */

            if (this.service.IsDriverOpen() == false)
            {
                Debug.Log("not found chrome driver");
                return;
            }

            if (this.service.IsAuthenticatedAccount(session.Email) == false)
            {
                Debug.Log("invaild login account");
                return;
            }

            if (KeyboardHooker.IsRunning() == false)
            {
                if (KeyboardHooker.Start() == false)
                {
                    Debug.Log("hotkey resister failed");
                    return;
                }
            }

            KeyboardHooker.OnKeyUp += OnKeyUp;
            KeyboardHooker.OnKeyDown += OnKeyDown;

            this.isEnableMacro = true;
        }
    }

    private void OnKeyDown(RawKey key)
    {
        if (this.isCombination == true)
        {
            this.inputRawKeys.Add(key);
            return;
        }

        if (this.allowSpecialKeys.ContainsKey(key) == true)
        {
            this.inputRawKeys.Add(key);
            this.isCombination = true;
        }
    }

    private void OnKeyUp(RawKey key)
    {
        if (this.isCombination == false)
        {
            return;
        }

        foreach (var macro in this.session.Macros)
        {
            if (this.inputRawKeys.SequenceEqual(macro.Key) == true)
            {
                if (this.service.Executor.AddCommand(macro.Value) == false)
                {
                    Debug.Log("executor add command timeout");
                }
                Debug.Log("executor add command complete");
                break;
            }
        }

        this.inputRawKeys.Clear();
        this.isCombination = false;
    }

    //bitmexmainadapter impl

    public BitMexCoinTable CoinTable
    {
        get
        {
            return this.service.CoinTable;
        }
    }

    public BitMexCommandExecutor CommandExecutor
    {
        get
        {
            return this.service.Executor;
        }
    }

    public BitMexSession Session
    {
        get
        {
            return this.session;
        }
    }

    public BitMexDriverService DriverService    
    {
        get
        {
            return this.service;
        }
    }

    public BitMexCommandRepository CommandRepository
    {
        get
        {
            return this.service.Repository;
        }
    }


    public bool ResisterMacro(List<RawKey> keys, BitMexCommandType type)
    {
        Debug.Log("resister macro complete");
        var command = this.service.Repository.CreateCommand(type);
        return this.session.ResisterMacro(keys, command);
    }

    public void WriteMacroLog(string log)
    {
        var content = this.contents[0] as ContentsMacro;
        content.WriteMacroLog(log);
    }
}
