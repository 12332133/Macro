using UnityEngine;
using UnityEngine.UI;
using Assets.BitMex;
using Assets.BitMex.WebDriver;
using System.Collections.Generic;
using System;
using Assets.BitMex.Commands;
using System.Collections;
using Assets.CombinationKey;
using Bitmex.Net;
using Newtonsoft.Json.Linq;
using Bitmex.Net.Model.Param;

public class Main : MonoBehaviour, IBitMexMainAdapter
{
    [SerializeField] private Button btnBitMex;
    [SerializeField] private Text txtBitMax;
    
    [SerializeField] private Button btnMacro;
    [SerializeField] private Text txtMacro;
    
    [SerializeField] private Text[] txtTabs;
    [SerializeField] private Toggle[] toggleTabs;

    [SerializeField] private ContentsBase[] contents;

    [SerializeField] private GameObject goPopup;

    [SerializeField] private CanvasGroup cgFadePanel;
    [SerializeField] private Text txtFadePanel;

    private BitmexSubscribeService subscriber;
    private BitmexApiService api;
    private BitmexAuthorization auth;
    private BitmexSession session;
    private BitMexCoinTable coins;
    private BitMexCommandExecutor executor;

    private bool isCombination = false;
    private bool isEnableMacro = false;
    private List<RawKey> inputRawKeys;
    private DateTime time = DateTime.Now;

    private ContentsBase.ModifyCommandPercentPopup<IBitMexCommand> popupInput;
    private ContentsBase.ModifyCommandCoinTypePopup<IBitMexCommand> popupDropdown;
    private ContentsBase.ContentsPopupMessage popupMessage;

    private Coroutine coroutineFade;

    private void Reset()
    {
        this.btnBitMex = transform.Find("Canvas/btnBitMax").GetComponent<Button>();
        this.txtBitMax = transform.Find("Canvas/btnBitMax/Text").GetComponent<Text>();

        this.btnMacro = transform.Find("Canvas/btnMacro").GetComponent<Button>();
        this.txtMacro = transform.Find("Canvas/btnMacro/Text").GetComponent<Text>();

        this.txtTabs = transform.Find("Canvas/Tab").GetComponentsInChildren<Text>();
        this.toggleTabs = transform.Find("Canvas/Tab").GetComponentsInChildren<Toggle>();

        this.contents = transform.Find("Canvas/Contents").GetComponentsInChildren<ContentsBase>();

        this.goPopup = transform.Find("Canvas/Popup").gameObject;

        this.cgFadePanel = transform.Find("Canvas/FadePanel").GetComponent<CanvasGroup>();
        this.txtFadePanel = transform.Find("Canvas/FadePanel/Text").GetComponent<Text>();
    }

    private void OnApplicationQuit()
    {
        KeyboardHooker.Stop();

        this.executor.Stop();

        foreach (var content in this.contents)
        {
            content.Save();
        }

        this.coins.SaveLocalCache();
    }

    private void Awake()
    {
        SetApiService();
        SetSession();
        SetSubscribeService();
        SetHook();
        SetInputKey();
        SetContents();
    }

    public void Show(BitmexAuthorization auth)
    {
        //var response = BitMexApiHelper.GetAccount(session, BitMexDomain);
        //var jobject = JObject.Parse(response);
        //session.Nickname = jobject["username"].ToString();

        this.auth = auth;
        gameObject.SetActive(true);
    }

    private void SetApiService()
    {
        var option = new BitmexApiServiceOption()
        {
            TimeOut = 5000,
        };

        this.api = new BitmexApiService(option, this.auth);
    }

    private void SetSession()
    {
        // 매크로 실행자
        this.executor = new BitMexCommandExecutor();

        // 활성된 코인 조회
        var instruments = this.api.Execute(BitmexApiActionAttributes.Instrument.GetInstrumentActive, new EmptyParams());

        // 코인 테이블 로드
        this.coins = new BitMexCoinTable();
        this.coins.LoadLocalCache(instruments);

        // 활성된 코인 중에 선택한 상세 코인 셋업 최대 4개 ? 5개 ?
        //load selectedcoin = new[] { "XBTUSD", "XRPZ18", "ETHXBT", "TRXZ18" }
        var selectedCoins = this.coins.ScreenActiveCoins(new[] { "XBTUSD", "XRPZ18", "ETHXBT", "TRXZ18" });
        this.session = new BitmexSession(selectedCoins);
    }

    private void SetSubscribeService()
    {
        var option = new BitmexSubscribeServiceOption()
        {
            IsReconnect = true,
        };

        this.subscriber = new BitmexSubscribeService(option, this.auth,
            (error) =>
            {
                this.popupMessage.OnEnablePopup(error);
            });

        if (this.subscriber.Open() == true)
        {
            this.session.Subscribe(this.subscriber);
        }
    }

    private void SetInputKey()
    {
        this.inputRawKeys = new List<RawKey>();
    }

    private void SetHook()
    {
        if (KeyboardHooker.IsRunning() == false)
        {
            if (KeyboardHooker.Start() == false)
            {
                return;
            }

            KeyboardHooker.OnKeyUp += OnKeyUp;
            KeyboardHooker.OnKeyDown += OnKeyDown;
        }
    }

    private void SetContents()
    {
        this.cgFadePanel.gameObject.SetActive(false);
        this.btnBitMex.onClick.AddListener(OnOpenBitMex);
        this.btnMacro.onClick.AddListener(OnEnableMacro);

        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            this.toggleTabs[i].onValueChanged.AddListener(OnToggleTab);
        }

        this.popupInput = new ContentsBase.ModifyCommandPercentPopup<IBitMexCommand>(this.goPopup.transform.GetChild(0));
        this.popupDropdown = new ContentsBase.ModifyCommandCoinTypePopup<IBitMexCommand>(this.goPopup.transform.GetChild(1), this.session.ActivateSymbols);
        this.popupMessage = new ContentsBase.ContentsPopupMessage(this.goPopup.transform.GetChild(2));

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

        //if (this.service.IsDriverOpen() == false)
        //{
        //    var driver = DriverFactory.CreateDriver(
        //            DriverType.Chrome,
        //            Application.streamingAssetsPath,
        //            false);

        //    this.service.OpenService(driver, BitMexDomain);

        //    StartCoroutine(SyncCointPrices());
        //    //StartCoroutine(CheckBitmexDriverAccount());
        //}
    }

    //private IEnumerator SyncCointPrices() 
    //{
    //    while (true)
    //    {
    //        var account = this.service.GetAutthenticateAccount();

    //        if (account.Equals(string.Empty) == false)
    //        {
    //            //var wc = new System.Diagnostics.Stopwatch();
    //            //wc.Start();

    //            this.service.HandleSyncCointPrices();

    //            if ("게스트".Equals(account) == true)
    //            {
    //                this.session.IsLogined = true;

    //                GetContent<ContentsBreakThrough>(1).UpdateSchedules();
    //                GetContent<ContentsAlarm>(2).UpdateSchedules();
    //            }
    //            else
    //            {
    //                this.session.IsLogined = false;
    //            }

    //            //wc.Stop();
    //            //Debug.Log(string.Format("time : {0}", wc.ElapsedMilliseconds.ToString()));
    //        }

    //        yield return new WaitForSeconds(0.05f);
    //    }
    //}

    //private IEnumerator CheckBitmexDriverAccount()
    //{
    //    while (true)
    //    {
    //        if (this.service.IsAuthenticatedAccount(this.session.Nickname) == true)
    //        {
    //            this.session.IsLogined = true;
    //        }
    //        else
    //        {
    //            this.session.IsLogined = false;
    //        }

    //        Debug.Log(string.Format("login {0}", this.session.IsLogined));
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}

    //private IEnumerator CheckSchedule()
    //{
    //    while (true)
    //    {
    //        try
    //        {
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log(e);
    //        }

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    void OnEnableMacro()
    {
        if (this.isEnableMacro == true)
        {
            this.isEnableMacro = false;
            this.txtMacro.text = "매크로 시작";

            if (coroutineFade != null)
            {
                StopCoroutine(coroutineFade);
                coroutineFade = null;
            }
            coroutineFade = StartCoroutine(FadeAction(false));
        }
        else
        {
            this.isEnableMacro = true;
            this.txtMacro.text = "매크로 정지";

            if (coroutineFade != null)
            {
                StopCoroutine(coroutineFade);
                coroutineFade = null;
            }
            coroutineFade = StartCoroutine(FadeAction(true));
        }
    }

    private IEnumerator FadeAction(bool state)
    {
        cgFadePanel.gameObject.SetActive(true);

        txtFadePanel.text = state ? "매크로 시작" : "매크로 정지";

        cgFadePanel.alpha = 0.01f;
        float fadeValue = 0.01f;
        while (cgFadePanel.alpha > 0)
        {
            yield return new WaitForEndOfFrame();

            cgFadePanel.alpha += fadeValue;

            if (cgFadePanel.alpha >= 1.0f) fadeValue *= -1f;
        }

        cgFadePanel.gameObject.SetActive(false);
    }

    private void OnKeyDown(RawKey key)
    {
        Debug.Log(key);
        if (this.isEnableMacro == false)
        {
            return;
        }

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
        Debug.Log(key);
        if (this.isEnableMacro == false)
        {
            return;
        }

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
            return this.coins;
        }
    }

    public BitmexApiService ApiService
    {
        get
        {
            return this.api;
        }
    }

    public BitmexSession Session
    {
        get
        {
            return this.session;
        }
    }

    public ContentsBase.ModifyCommandPercentPopup<IBitMexCommand> PopupInput
    {
        get
        {
            return this.popupInput;
        }
    }

    public ContentsBase.ModifyCommandCoinTypePopup<IBitMexCommand> PopupDropdown
    {
        get
        {
            return this.popupDropdown;
        }
    }

    public ContentsBase.ContentsPopupMessage PopupMessage
    {
        get
        {
            return this.popupMessage;
        }
    }

    public BitMexCommandExecutor CommandExecutor
    {
        get
        {
            return this.executor;
        }
    }
}
