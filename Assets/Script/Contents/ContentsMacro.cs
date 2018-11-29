using Assets.BitMex;
using Assets.BitMex.Commands;
using Assets.CombinationKey;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ContentsMacro : ContentsBase
{
    public class MacroPopup
    {
        public GameObject Root;
        public Button btnPopupBack;
        public InputField inputPopup;
        public Button btnPopup;
        public Action<string> complete;

        public MacroPopup(Transform root)
        {
            this.Root = root.gameObject;
            this.btnPopupBack = root.Find("BackPanel").GetComponent<Button>();
            this.inputPopup = root.Find("InputField").GetComponent<InputField>();
            this.btnPopup = root.Find("Button").GetComponent<Button>();

            this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
            this.btnPopup.onClick.AddListener(OnClickPopupOK);
        }

        public void OnEnablePopup(string original, Action<string> complete)
        {
            this.inputPopup.text = original;
            this.complete = complete;
            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
            this.complete(this.inputPopup.text);
            this.Root.SetActive(false);
        }

        private void OnClickPopupOK()
        {
            this.complete(this.inputPopup.text);
            this.Root.SetActive(false);
        }
    }

    [SerializeField] private Text[] txtTabs;
    [SerializeField] private Toggle[] toggleTabs;
    [SerializeField] private Button btnAddMacro;

    [SerializeField] private ScrollRect[] svHotKeys;
    [SerializeField] private GameObject goHotKeyItem;

    [SerializeField] private GameObject goPopup;
    //[SerializeField] private Button btnPopupBack;
    //[SerializeField] private InputField inputPopup;
    //[SerializeField] private Button btnPopup;

    [SerializeField] private ScrollRect svLog;
    [SerializeField] private GameObject goLogItem;
    [SerializeField] private Button btnAddLog;
    [SerializeField] private Button btnDelLog;

    [SerializeField] private Button btnSave;

    private MacroPopup macroPopup;

    private void Reset()
    {
        this.txtTabs = transform.Find("Panel/Tab").GetComponentsInChildren<Text>();
        this.toggleTabs = transform.Find("Panel/Tab").GetComponentsInChildren<Toggle>();

        this.btnAddMacro = transform.Find("Panel/btnAddMacro").GetComponent<Button>();

        this.svHotKeys = transform.Find("Panel/HotKeys").GetComponentsInChildren<ScrollRect>();
        this.goHotKeyItem = Resources.Load<GameObject>("MacroHotKeyItem");

        this.goPopup = transform.Find("Panel/Popup").gameObject;
        //this.btnPopupBack = transform.Find("Panel/Popup/BackPanel").GetComponent<Button>();
        //this.inputPopup = transform.Find("Panel/Popup/InputField").GetComponent<InputField>();
        //this.btnPopup = transform.Find("Panel/Popup/Button").GetComponent<Button>();

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

        OnToggleTab(true);

        this.btnAddMacro.onClick.AddListener(OnClickAddMacro);

        this.macroPopup = new MacroPopup(this.goPopup.transform);

        OnRefreshMacroItem(BitMexCommandTableType.Percent);
        OnRefreshMacroItem(BitMexCommandTableType.Quantity);

        //this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
        //this.btnPopup.onClick.AddListener(OnClickPopupOK);

        this.btnAddLog.onClick.AddListener(OnClickAddLog);
        this.btnDelLog.onClick.AddListener(OnClickDelLog);

        this.btnSave.onClick.AddListener(OnClickSave);
        //this.btnSave.interactable = false;
    }

    private void OnToggleTab(bool state)
    {
        if (!state)
        {
            return;
        }

        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            if(this.toggleTabs[i].isOn)
            {
                Debug.Log("OnToggleTab() - " + this.toggleTabs[i].name);
            }

            svHotKeys[i].gameObject.SetActive(this.toggleTabs[i].isOn);
        }
    }

    private ContentsMacroHotKeyItem CreateHotKeyItem(BitMexCommandTableType type, Macro macro)
    {
        var go = Instantiate(this.goHotKeyItem);

        var item = go.GetComponent<ContentsMacroHotKeyItem>().Initialized(
                        type,
                        OnModifyCommandParameters,
                        OnRefreshDropdown,
                        OnRefreshMacroItem,
                        bitmexMain,
                        macro);

        go.transform.SetParent(this.svHotKeys[ConvertTypeToIndex(type)].content.transform);
        return item;
    }

    private void OnClickAddMacro() 
    {
        Debug.Log("OnClickAddMacro()");
        CreateHotKeyItem(GetActivateToggleType(), null);
    }

    public void WriteMacroLog(string log)
    {
        var go = Instantiate(goLogItem);
        go.GetComponent<ContentsMacroLogItem>().Initialized();
        go.GetComponent<ContentsMacroLogItem>().SetLogText(log);
        go.transform.SetParent(this.svLog.content.transform);
    }

    private int GetActivateToggleIndex()
    {
        for (int i = 0; i < this.toggleTabs.Length; ++i)
        {
            if (this.toggleTabs[i].isOn == true)
            {
                Debug.Log(string.Format("current toggle index : {0}", i));
                return i;
            }
        }

        throw new Exception();
    }

    private BitMexCommandTableType GetActivateToggleType()
    {
        return ConvertIndexToType(GetActivateToggleIndex());
    }

    private int ConvertTypeToIndex(BitMexCommandTableType type)
    {
        switch (type)
        {
            case BitMexCommandTableType.Percent:
                return 0;
            case BitMexCommandTableType.Quantity:
                return 1;
        }

        throw new Exception();
    }

    private BitMexCommandTableType ConvertIndexToType(int toggleIndex)
    {
        switch (toggleIndex)
        {
            case 0:
                return BitMexCommandTableType.Percent;
            case 1:
                return BitMexCommandTableType.Quantity;
        }

        throw new Exception();
    }

    private void OnRefreshDropdown(BitMexCommandTableType type)
    {
        foreach (var item in this.svHotKeys[ConvertTypeToIndex(type)].content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            item.RefreshCommandDropdown();
        }
    }

    private void OnRefreshMacroItem(BitMexCommandTableType type)
    {
        foreach (var item in this.svHotKeys[ConvertTypeToIndex(type)].content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            Destroy(item.gameObject);
        }

        foreach (var macro in bitmexMain.Macro.GetMacros(type))
        {
            CreateHotKeyItem(type, macro);
        }

        if (bitmexMain.Macro.GetMacros(type).Count < 5)
        {
            for (int i = 0; i < 5 - bitmexMain.Macro.GetMacros(type).Count; i++)
            {
                CreateHotKeyItem(type, null);
            }
        }
    }

    private void OnModifyCommandParameters(IBitMexCommand command, Action complete)
    {
        switch (command.CommandType)
        {
            case BitMexCommandType.ChangeCoinTap: // 드랍박스 팝업창으로 -> 현재 서비스 중인 코인 리스트업
                this.macroPopup.OnEnablePopup(command.Parameters[0].ToString(), (value) => {
                    // 현재 서비스중인 코인 이름
                    command.Parameters.Clear();
                    command.Parameters.Add(value);
                    complete();
                });
                break;
            default:
                this.macroPopup.OnEnablePopup(command.Parameters[0].ToString(), (value) =>
                {
                    command.Parameters.Clear();
                    command.Parameters.Add(value);
                    complete();
                });
                break;
        }
    }

    //private void OnClickPopupBack()
    //{
    //    Debug.Log("ContentsMacro.OnClickPopupBack()");
    //    this.goPopup.SetActive(false);
    //}

    //private void OnClickPopupOK()
    //{
    //    Debug.Log("ContentsMacro.OnClickPopupOK()");
    //    this.goPopup.SetActive(false);
    //}

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
        foreach (var item in this.svHotKeys[GetActivateToggleIndex()].content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            item.ResisterMacro();
        }

        this.bitmexMain.CommandTable.SaveLocalCache();
        this.bitmexMain.Macro.SaveLocalCache();

        Debug.Log("ContentsMacro.OnClickSave()");
    }
}
