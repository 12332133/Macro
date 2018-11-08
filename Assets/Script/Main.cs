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
    private BitMexCommandExecutor executor;
    private BitMexCommandRepository repository;
    //private const string BitMexDomain = "https://testnet.bitmex.com/";
    private const string BitMexDomain = "https://www.bitmex.com/";

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
        SetTemplateCommand();
        SetBitMexService();
        SetAllowSpecialKey();
        SetInputKey();

        this.toggleTabs[0].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[1].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[2].onValueChanged.AddListener(OnToggleTab);

        this.btnBitMex.onClick.AddListener(OnOpenBitMex);
        this.btnMacro.onClick.AddListener(OnEnableMacro);

        foreach (var content in this.contents)
        {
            content.Initialize(this);
        }
    }

    private void SetBitMexService()
    {
        this.service = new BitMexDriverService();

        this.executor = new BitMexCommandExecutor();

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

    private void SetTemplateCommand()
    {
        this.repository = new BitMexCommandRepository();

        this.repository.Resister(BitMexCommandType.Test, new DefaultSampleCommand(this, "테스트"));

        this.repository.Resister(BitMexCommandType.FixedAvailableXbt, new FixedAvailableXbtCommand(this, "사용가능 xbt 고정"));
        this.repository.Resister(BitMexCommandType.SpecifiedAditional, new FixedAvailableXbtCommand(this, "빠른 지정가 설정"));

        this.repository.Resister(BitMexCommandType.MarketPriceBuy10Magnification, new MarketPriceBuyCommand(this, "시장가 10% 매수"));
        this.repository.Resister(BitMexCommandType.MarketPriceBuy25Magnification, new MarketPriceBuyCommand(this, "시장가 25% 매수"));
        this.repository.Resister(BitMexCommandType.MarketPriceBuy50Magnification, new MarketPriceBuyCommand(this, "시장가 50% 매수"));
        this.repository.Resister(BitMexCommandType.MarketPriceBuy100Magnification, new MarketPriceBuyCommand(this, "시장가 100% 매수"));

        this.repository.Resister(BitMexCommandType.MarketPriceSell10Magnification, new MarketPriceSellCommand(this, "시장가 10% 매도"));
        this.repository.Resister(BitMexCommandType.MarketPriceSell25Magnification, new MarketPriceSellCommand(this, "시장가 25% 매도"));
        this.repository.Resister(BitMexCommandType.MarketPriceSell50Magnification, new MarketPriceSellCommand(this, "시장가 50% 매도"));
        this.repository.Resister(BitMexCommandType.MarketPriceSell100Magnification, new MarketPriceSellCommand(this, "시장가 100% 매도"));
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
        var driver = DriverFactory.CreateDriver(
                            DriverType.Chrome,
                            Application.streamingAssetsPath,
                            false);

        this.service.OpenService(driver, BitMexDomain);
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
            //if (this.service.IsDriverOpen() == false)
            //{
            //    Debug.Log("not found chrome driver");
            //    return;
            //}

            ////var response = BitMexApiHelper.GetReferral(session, BitMexDomain); //json deserialize

            ////if (this.session.ReferrerAccount.Equals(response) == false)
            ////{
            ////    Debug.Log("invaild referral account");
            ////    return;
            ////}

            //if (this.service.IsInvaildEmail(session.Email) == false)
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
                if (this.executor.AddCommand(macro.Value) == false)
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

    // bitmexmainadapter impl

    public BitMexCommandExecutor CommandExecutor
    {
        get
        {
            return this.executor;
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

    public bool ResisterMacro(List<RawKey> keys, BitMexCommandType type)
    {
        Debug.Log("resister macro complete");
        return this.session.ResisterMacro(keys, this.repository.CreateCommand(type));
    }

    public BitMexCommandRepository CommandRepository
    {
        get
        {
            return this.repository;
        }
    }

    public void WriteMacroLog(string log)
    {
        var content = this.contents[0] as ContentsMacro;
        content.WriteMacroLog(log);
    }
}
