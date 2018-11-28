using Assets.BitMex;
using Assets.BitMex.Commands;
using Assets.CombinationKey;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ContentsMacro : ContentsBase
{
    [SerializeField] private Text[] txtTabs;
    [SerializeField] private Toggle[] toggleTabs;
    [SerializeField] private Button btnAddMacro;

    [SerializeField] private ScrollRect svHotKey;
    [SerializeField] private GameObject goHotKeyItem;

    [SerializeField] private GameObject goPopup;
    [SerializeField] private InputField inputPopup;
    [SerializeField] private Button btnPopup;    

    [SerializeField] private ScrollRect svLog;
    [SerializeField] private GameObject goLogItem;
    [SerializeField] private Button btnAddLog;
    [SerializeField] private Button btnDelLog;

    [SerializeField] private Button btnSave;

    private void Reset()
    {
        this.txtTabs = transform.Find("Panel/Tab").GetComponentsInChildren<Text>();
        this.toggleTabs = transform.Find("Panel/Tab").GetComponentsInChildren<Toggle>();

        this.btnAddMacro = transform.Find("Panel/btnAddMacro").GetComponent<Button>();

        this.svHotKey = transform.Find("Panel/svHotKey").GetComponent<ScrollRect>();
        this.goHotKeyItem = Resources.Load<GameObject>("MacroHotKeyItem");

        this.goPopup = transform.Find("Panel/Popup").gameObject;
        this.inputPopup = transform.Find("Panel/Popup/InputField").GetComponent<InputField>();
        this.btnPopup = transform.Find("Panel/Popup/Button").GetComponent<Button>();

        this.svLog = transform.Find("Panel/LogRoot/svLog").GetComponent<ScrollRect>();
        this.goLogItem = Resources.Load<GameObject>("MacroLogItem");
        this.btnAddLog = transform.Find("Panel/LogRoot/btnAdd").GetComponent<Button>();
        this.btnDelLog = transform.Find("Panel/LogRoot/btnDel").GetComponent<Button>();

        this.btnSave = transform.Find("Panel/btnSave").GetComponent<Button>();
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            this.toggleTabs[i].onValueChanged.AddListener(OnToggleTab);
        }
        this.btnAddMacro.onClick.AddListener(OnClickAddMacro);

        OnRefreshMacroItem();

        this.btnPopup.onClick.AddListener(OnClickPopupOK);

        this.btnAddLog.onClick.AddListener(OnClickAddLog);
        this.btnDelLog.onClick.AddListener(OnClickDelLog);

        this.btnSave.onClick.AddListener(OnClickSave);
        //this.btnSave.interactable = false;
    }

    private void OnToggleTab(bool state)
    {
        if (!state) return;

        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            if(this.toggleTabs[i].isOn) Debug.Log("OnToggleTab() - " + this.toggleTabs[i].name);
        }
    }

    private ContentsMacroHotKeyItem CreateHotKeyItem(Macro refMacro)
    {
        var go = Instantiate(this.goHotKeyItem);

        var item = go.GetComponent<ContentsMacroHotKeyItem>().Initialized(
                        OnModifyCommandParameters,
                        OnRefreshDropdown,
                        OnRefreshMacroItem,
                        bitmexMain,
                        refMacro);

        go.transform.SetParent(this.svHotKey.content.transform);
        return item;
    }

    private void OnClickAddMacro() 
    {
        Debug.Log("OnClickAddMacro()");
        CreateHotKeyItem(null);
    }

    public void WriteMacroLog(string log)
    {
        var go = Instantiate(goLogItem);
        go.GetComponent<ContentsMacroLogItem>().Initialized();
        go.GetComponent<ContentsMacroLogItem>().SetLogText(log);
        go.transform.SetParent(this.svLog.content.transform);
    }

    private void OnRefreshDropdown()
    {
        foreach (var item in this.svHotKey.content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            item.RefreshCommandDropdown();
        }
    }

    private void OnRefreshMacroItem()
    {
        foreach (var item in this.svHotKey.content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            Destroy(item.gameObject);
        }

        foreach (var macro in bitmexMain.Macro.Macros)
        {
            CreateHotKeyItem(macro);
        }

        if (bitmexMain.Macro.Macros.Count < 5)
        {
            for (int i = 0; i < 5 - bitmexMain.Macro.Macros.Count; i++)
            {
                CreateHotKeyItem(null);
            }
        }
    }

    private void OnModifyCommandParameters(IBitMexCommand command)
    {
        switch (command.CommandType)
        {
            case BitMexCommandType.ChangeCoinTap:
                command.Parameters.Clear();
                command.Parameters.Add("SAMPLECOIN"); // 선택한 코인 이름 
                break;
            case BitMexCommandType.MarketPriceBuyMagnification:
            case BitMexCommandType.MarketPriceSellMagnification:
            case BitMexCommandType.MarketSpecifiedPriceBuy:
            case BitMexCommandType.MarketSpecifiedPriceSell:
                command.Parameters.Clear();
                command.Parameters.Add(50); // 선택한 퍼센트 
                break;
            case BitMexCommandType.MarketPriceSpecifiedQuantityBuy:
            case BitMexCommandType.MarketPriceSpecifiedQuantitySell:
            case BitMexCommandType.MarketSpecifiedQuantityBuy:
            case BitMexCommandType.MarketSpecifiedQuantitySell:
                command.Parameters.Clear();
                command.Parameters.Add(100); // 선택한 수량
                break;
        }
    }

    private void OnClickPopupOK()
    {
        this.goPopup.SetActive(false);
    }

    private void OnClickAddLog()
    {
        Debug.Log("ContentsMacro.OnClickAdd()");
    }

    private void OnClickDelLog()
    {
        Debug.Log("ContentsMacro.OnClickDel()");
    }

    private void OnClickSave()
    {
        foreach (var item in this.svHotKey.content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            item.ResisterMacro();
        }

        this.bitmexMain.CommandTable.SaveLocalCache();
        this.bitmexMain.Macro.SaveLocalCache();
    }
}
