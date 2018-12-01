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
    private DateTime time = DateTime.Now;

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

        this.service.CommandTable.SaveLocalCache();
        this.session.Macro.SaveLocalCache();
        this.service.CoinTable.SaveLocalCache();

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
        
        //퍼센트
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this));

        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification, 
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this, (parameters) => { parameters.Add(20); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification, 
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this, (parameters) => { parameters.Add(20); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy, 
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this, (parameters) => { parameters.Add(20); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell, 
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(100); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(80); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(50); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this, (parameters) => { parameters.Add(20); }));

        // 수량 
        this.service.CommandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.None,
            new NoneCommand(this));

        this.service.CommandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketPriceSpecifiedQuantityBuy,
            new MarketPriceSpecifiedQuantityBuyCommand(this, (parameters) => { parameters.Add(0); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketPriceSpecifiedQuantitySell,
            new MarketPriceSpecifiedQuantitySellCommand(this, (parameters) => { parameters.Add(0); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketSpecifiedQuantityBuy,
            new MarketSpecifiedQuantityBuyCommand(this, (parameters) => { parameters.Add(0); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketSpecifiedQuantitySell,
            new MarketSpecifiedQuantitySellCommand(this, (parameters) => { parameters.Add(0); }));

        // 기타
        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.None,
            new NoneCommand(this));

        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ChangeCoinTap,
            new ChangeCoinTapCommand(this, (parameters) => { parameters.Add("XBTUSD"); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ChangeCoinTap,
            new ChangeCoinTapCommand(this, (parameters) => { parameters.Add("ETHXBT"); }));
        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ChangeCoinTap,
            new ChangeCoinTapCommand(this, (parameters) => { parameters.Add("XRPZ18"); }));

        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ClearPosition,
            new PositionClearCommand(this));

        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.CancleTopActivateOrder,
            new TopActivateOrderCancleCommand(this));

        this.service.CommandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.CancleAllActivateOrder,
            new ActivateOrderCancleCommand(this));

        this.service.CommandTable.LoadLocalCache();
        this.session.Macro.LoadLocalCache(this.service.CommandTable);
    }

    private void SetInputKey()
    {
        this.inputRawKeys = new List<RawKey>();
    }

    private void SetContents()
    {
        this.btnBitMex.onClick.AddListener(OnOpenBitMex);
        this.btnMacro.onClick.AddListener(OnEnableMacro);

        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            this.toggleTabs[i].onValueChanged.AddListener(OnToggleTab);
        }

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
                if (this.service.IsTradingPage() == true)
                {
                    //var wc = new System.Diagnostics.Stopwatch();
                    //wc.Start();

                    this.service.HandleSyncCointPrices();

                    foreach (var schedule in this.schedules)
                    {
                        if (schedule.Execute() == true)
                        {
                            IBitMexSchedule outi;
                            this.schedules.TryDequeue(out outi);
                            //Debug.Log(string.Format("execute price schedule"));
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

            yield return new WaitForSeconds(0.05f);
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
            this.txtMacro.text = "MACRO DISABLE";
        }
        else
        {
            if (this.service.IsDriverOpen() == false)
            {
                Debug.Log("not found chrome driver");
                return;
            }

            if (this.service.IsTradingPage() == false)
            {
                Debug.Log("invaild page");
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
            this.txtMacro.text = "MACRO ENABLE";
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

        if ((DateTime.Now - this.time).TotalMilliseconds < 1000)
        {
            Debug.Log("over fast macro input");
            return;
        }
 
        foreach (var table in Macro.GetMacroTable())
        {
            foreach (var macro in table.Value)
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
        }

        this.time = DateTime.Now;
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

    public BitMexCommandTable CommandTable
    {
        get
        {
            return this.service.CommandTable;
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
