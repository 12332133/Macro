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

public class Main : MonoBehaviour
{
    [SerializeField] private Button btnBitMex;
    [SerializeField] private Text txtBitMax;
    
    [SerializeField] private Button btnMacro;
    [SerializeField] private Text txtMacro;
    
    [SerializeField] private Text[] txtTabs;
    [SerializeField] private Toggle[] toggleTabs;

    [SerializeField] private ContentsBase[] contents;

    private BitMexService service;
    private BitMexSession session;
    private const string BitMexDomain = "https://bitmex.com/";

    private bool isEnableMacro = false;
    private Thread thread;
    private List<RawKey> inputRawKeys;
    private bool isKeyCombinationComplete = false;
    private List<KeyValuePair<List<RawKey>, Action>> macros;
    private BlockingCollection<Action> actions;
    //private BlockingCollection<List<RawKey>> combinationRawKeys = new BlockingCollection<List<RawKey>>();

    private void Reset()
    {
        this.btnBitMex = transform.Find("Canvas/btnBitMax").GetComponent<Button>();
        this.txtBitMax = transform.Find("Canvas/btnBitMax/Text").GetComponent<Text>();

        this.btnMacro = transform.Find("Canvas/btnMacro").GetComponent<Button>();
        this.txtMacro = transform.Find("Canvas/btnMacro/Text").GetComponent<Text>();

        this.txtTabs = transform.Find("Canvas/Tab").GetComponentsInChildren<Text>();
        this.toggleTabs = transform.Find("Canvas/Tab").GetComponentsInChildren<Toggle>();

        this.contents = transform.Find("Canvas/Contents").GetComponentsInChildren<ContentsBase>();

        this.service = new BitMexService();
        this.session = new BitMexSession()
        {
            ApiKey = "TE3O0NLo8pmwAkzsv66UamVr",
            ApiSecret = "yVjWPBWEVmwWZ39bRJ23aLJu5h69Eq4cyQHM6utd-O7Z8qZx",
            Email = "condemonkey@gmail.com",
            ReferrerAccount = "462226",
            ReferrerEmail = "",
        };
        this.actions = new BlockingCollection<Action>();
        this.macros = new List<KeyValuePair<List<RawKey>, Action>>();
        this.macros.Add(new KeyValuePair<List<RawKey>, Action>(
            new List<RawKey>() {
                        RawKey.LeftShift,RawKey.A,
            },
            () => {
                Debug.Log("RawKey.LeftShift,RawKey.A");
            }));
        this.macros.Add(new KeyValuePair<List<RawKey>, Action>(
            new List<RawKey>() {
                RawKey.LeftShift,RawKey.S,
            },
            () => {
                Debug.Log("RawKey.LeftShift,RawKey.S");
            }));
        this.macros.Add(new KeyValuePair<List<RawKey>, Action>(
            new List<RawKey>() {
                RawKey.LeftShift,RawKey.D,
            },
            () => {
                Debug.Log("RawKey.LeftShift,RawKey.D");
            }));
        this.inputRawKeys = new List<RawKey>();
    }

    private void OnApplicationQuit()
    {
        KeyboardHooker.Stop();
        if (this.thread != null) this.thread.Abort();
        this.service.CloseDriver();
    }

    private void Awake()
    {
        this.toggleTabs[0].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[1].onValueChanged.AddListener(OnToggleTab);
        this.toggleTabs[2].onValueChanged.AddListener(OnToggleTab);

        this.btnBitMex.onClick.AddListener(OnOpenBitMex);
        this.btnMacro.onClick.AddListener(OnEnableMacro);

        this.contents[0].Initialize();
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

    private void ActionWork()
    {
        while (true)
        {
            var action = this.actions.Take();
            action();
            //var combinationRawKey = this.combinationRawKeys.Take();

            //foreach (var macro in this.macros)
            //{
            //    if (combinationRawKey.SequenceEqual(macro.Key) == true)
            //    {
            //        break;
            //    }
            //}
        }
    }

    void OnEnableMacro()
    {
        if (this.isEnableMacro == true)
        {
            if (this.thread != null)
            {
                this.thread.Abort();
            }

            KeyboardHooker.OnKeyUp -= OnKeyUp;
            KeyboardHooker.OnKeyDown -= OnKeyDown;
            this.isEnableMacro = false;
        }
        else
        {
            if (this.thread == null)
            {
                this.thread = new Thread(ActionWork);
                this.thread.IsBackground = true;
                this.thread.Start();
            }

            if (this.service.IsDriverOpen() == false)
            {
                Debug.Log("not found chrome driver");
                return;
            }

            var response = BitMexApiHelper.GetReferral(session, BitMexDomain); //json deserialize

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
        this.isKeyCombinationComplete = false;
        this.inputRawKeys.Add(key);
    }

    private void OnKeyUp(RawKey key)
    {
        if (this.isKeyCombinationComplete == true)
        {
            return;
        }

        //if (this.combinationRawKeys.TryAdd(new List<RawKey>(this.inputRawKeys), 100) == false)
        //{
        //    Debug.Log("action add timeout");
        //}

        foreach (var macro in this.macros)
        {
            if (inputRawKeys.SequenceEqual(macro.Key) == true)
            {
                if (this.actions.TryAdd(macro.Value, 100) == false)
                {
                    Debug.Log("action add timeout");
                }
                break;
            }
        }

        this.inputRawKeys.Clear();
        this.isKeyCombinationComplete = true;
    }
}
