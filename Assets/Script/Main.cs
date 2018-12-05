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
        var response = BitMexApiHelper.GetAccount(session, BitMexDomain);
        var jobject = JObject.Parse(response);
        session.Nickname = jobject["username"].ToString();

        this.session = session;
        gameObject.SetActive(true);
    }

    private void SetBitMexService()
    {
        this.service = new BitMexDriverService();
        this.service.CoinTable.LoadActiveCoins(BitMexDomain);
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
            StartCoroutine(CheckBitmexDriverAccount());
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

                    if (this.session.IsLogined == true)
                    {
                        GetContent<ContentsBreakThrough>(1).UpdateSchedules();
                        GetContent<ContentsAlarm>(2).UpdateSchedules();
                    }

                    //foreach (var schedule in this.schedules)
                    //{
                    //    if (schedule.Execute() == true)
                    //    {
                    //        IBitMexSchedule outi;
                    //        this.schedules.TryDequeue(out outi);
                    //        //Debug.Log(string.Format("execute price schedule"));
                    //    }
                    //}

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
                if (this.service.IsAuthenticatedAccount(this.session.Nickname) == true)
                {
                    this.session.IsLogined = true;
                }
                else
                {
                    this.session.IsLogined = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return new WaitForSeconds(0.5f);
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

        GetContent<ContentsMacro>(0).ExecuteMacro(this.inputRawKeys);

        this.time = DateTime.Now;
        this.inputRawKeys.Clear();
        this.isCombination = false;
    }

    private T GetContent<T>(int index) where T : ContentsBase
    {
        return this.contents[index] as T;
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
}
