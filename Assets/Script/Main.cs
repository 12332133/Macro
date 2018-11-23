using UnityEngine;
using UnityEngine.UI;
using Assets.BitMex;
using Assets.BitMex.WebDriver;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System;
using System.Linq;
using Assets.BitMex.Commands;
using System.Collections;
using Newtonsoft.Json.Linq;
using Assets.CombinationKey;

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
    private ConcurrentQueue<IBitMexSchedule> schedules;

    private const string BitMexDomain = "https://testnet.bitmex.com";
    //private const string BitMexDomain = "https://www.bitmex.com/";

    private bool isCombination = false;
    private bool isEnableMacro = false;
    private List<RawKey> inputRawKeys;

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

        this.session.Save();

        this.service.CloseDriver();
    }

    private void Awake()
    {
        SetBitMexService();
        SetInputKey();
        SetContents();
    }

    public void Show(BitMexSession session)
    {
        this.session = session;
        gameObject.SetActive(true);
    }

    private void SetBitMexService()
    {
        this.schedules = new ConcurrentQueue<IBitMexSchedule>();
        this.service = new BitMexDriverService();
        this.service.CoinTable.LoadActiveCoins(BitMexDomain);

        //command 
        this.service.Repository.Resister(BitMexCommandType.None,
            new SampleCommand(this));

        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuyMagnification1, 
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuyMagnification2,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuyMagnification3,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuyMagnification4,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(20); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceBuyMagnificationCustom,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add("설정"); }));

        this.service.Repository.Resister(BitMexCommandType.MarketPriceSellMagnification1, 
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSellMagnification2,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSellMagnification3,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSellMagnification4,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(20); }));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSellMagnificationCustom,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add("설정"); }));

        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceBuy1, 
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceBuy2,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceBuy3,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceBuy4,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(20); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceBuyCustom,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add("설정"); }));

        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceSell1, 
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceSell2,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceSell3,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceSell4,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(20); }));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedPriceSellCustom,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add("설정"); }));

        this.service.Repository.Resister(BitMexCommandType.MarketPriceSpecifiedQuantityBuy, 
            new MarketPriceSpecifiedQuantityBuyCommand(this));
        this.service.Repository.Resister(BitMexCommandType.MarketPriceSpecifiedQuantitySell, 
            new MarketPriceSpecifiedQuantitySellCommand(this));

        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedQuantityBuy, 
            new MarketSpecifiedQuantityBuyCommand(this));
        this.service.Repository.Resister(BitMexCommandType.MarketSpecifiedQuantitySell, 
            new MarketSpecifiedQuantitySellCommand(this));

        this.service.Repository.Resister(BitMexCommandType.ChangeCoinTap, 
            new ChangeCoinTapCommand(this, (parameters) => { parameters.Add("설정"); }));

        this.service.Repository.Resister(BitMexCommandType.ClearPosition, new PositionClearCommand(this));
        this.service.Repository.Resister(BitMexCommandType.CancleTopActivateOrder, new TopActivateOrderCancleCommand(this));
        this.service.Repository.Resister(BitMexCommandType.CancleAllActivateOrder, new ActivateOrderCancleCommand(this));

        this.session.Macro.LoadLocalCache(this.service.Repository);
    }

    private void SetInputKey()
    {
        this.inputRawKeys = new List<RawKey>();
    }

    private void SetContents()
    {
        this.btnBitMex.onClick.AddListener(OnOpenBitMex);
        this.btnMacro.onClick.AddListener(OnEnableMacro);

        this.toggleTabs[0].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[1].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[2].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[3].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[4].onValueChanged.AddListener(OnToggleTab);

        foreach (var content in this.contents)
        {
            content.Initialize(this);
        }

        OnToggleTab(true);
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
        //if (this.service.IsDriverOpen() == false)
        //{
        //    var driver = DriverFactory.CreateDriver(
        //          DriverType.Chrome,
        //          Application.streamingAssetsPath,
        //          false);

        //    this.service.OpenService(driver, BitMexDomain);

        //    StartCoroutine(SyncCointPrices());
        //}
        //else
        //{
        //    try
        //    {
        //        var wc = new System.Diagnostics.Stopwatch();
        //        wc.Start();

        //        var coin = this.CoinTable.GetCoin("XBTUSD");
        //        if (this.service.HandleChangeCoinTab(coin.RootCoinName, coin.CoinName) == false)
        //        {
        //            Debug.Log("not found tab");
        //        }

        //        wc.Stop();
        //        Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));

        //        //foreach (var coin in this.CoinTable.Coins.Values.ToList())
        //        //{

        //        //}

        //        //var command = this.service.Repository.CreateCommand(BitMexCommandType.ClearPosition);
        //        //command.Execute();
        //    }
        //    catch (BitMexDriverServiceException exception)
        //    {
        //        Debug.Log(exception.ToString());
        //    }
        //}

        if (this.service.IsDriverOpen() == false)
        {
            var driver = DriverFactory.CreateDriver(
                    DriverType.Chrome,
                    Application.streamingAssetsPath,
                    false);

            this.service.OpenService(driver, BitMexDomain);

            StartCoroutine(SyncCointPrices());
            //StartCoroutine(CheckBitmexDriverAccount());
        }
    }

    private IEnumerator SyncCointPrices() 
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

                    var enumerator = this.schedules.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Execute() == true)
                        {
                            IBitMexSchedule bsc;
                            this.schedules.TryDequeue(out bsc);
                            Debug.Log(string.Format("execute price schedule"));
                        }
                    }

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

    private IEnumerator CheckBitmexDriverAccount()
    {
        while (true)
        {
            try
            {
                if (this.service.IsAuthenticatedAccount(this.session.Email) == false)
                {
                    Debug.Log("threc account");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator CheckSchedule()
    {
        while (true)
        {
            try
            {
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
            if (this.service.IsDriverOpen() == false)
            {
                Debug.Log("not found chrome driver");
                return;
            }

            //if (this.service.IsAuthenticatedAccount(session.Email) == false)
            //{
            //    Debug.Log("invaild login account");
            //    return;
            //}

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

        if (AllowedModifire.IsAllowed(key) == true)
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

        foreach (var macro in Macro.Macros)
        {
            if (this.inputRawKeys.SequenceEqual(macro.Keys) == true)
            {
                if (this.service.Executor.AddCommand(macro.Command.Clone()) == false)
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
    public BitMexMacro Macro
    {
        get
        {
            return this.session.Macro;
        }
    }


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

    public void ResisterSchedule(IBitMexSchedule schedule)
    {
        this.schedules.Enqueue(schedule);
    }

    public void WriteMacroLog(string log)
    {
        var content = this.contents[0] as ContentsMacro;
        content.WriteMacroLog(log);
    }
}
