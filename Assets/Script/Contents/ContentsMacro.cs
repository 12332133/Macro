﻿using Assets.BitMex;
using Assets.BitMex.Commands;
using Assets.CombinationKey;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public interface IContentsMacro
{
    BitMexCommandTable CommandTable { get; }
    BitMexMacroTable MacroTable { get; }
    ContentsBase.ContentsPopupInput<IBitMexCommand> PopupInput { get; }
    ContentsBase.ContentsPopupDropdown<IBitMexCommand> PopupDropdown { get; }
    ContentsBase.ContentsPopupMessage PopupAlret { get; }
    void OnRefreshDropdown(BitMexCommandTableType type);
    void OnRefreshMacroItem(BitMexCommandTableType type);
}

public class ContentsMacro : ContentsBase, IContentsMacro
{
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

    private ContentsPopupInput<IBitMexCommand> popupInput;
    private ContentsPopupDropdown<IBitMexCommand> popupDropdown;
    private ContentsPopupMessage popupMessage;

    private BitMexCommandTable commandTable;
    private BitMexMacroTable macroTable;

    // interface impl
    public BitMexCommandTable CommandTable { get { return this.commandTable; } }
    public BitMexMacroTable MacroTable { get { return this.macroTable; } }
    public ContentsPopupInput<IBitMexCommand> PopupInput { get { return this.popupInput; } }
    public ContentsPopupDropdown<IBitMexCommand> PopupDropdown { get { return this.popupDropdown; } }
    public ContentsPopupMessage PopupAlret { get { return this.popupMessage; } }

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

    private void OnApplicationQuit()
    {
        this.commandTable.SaveLocalCache();
        this.macroTable.SaveLocalCache();
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

        this.popupInput = new ContentsPopupInput<IBitMexCommand>(this.goPopup.transform.GetChild(0));
        this.popupDropdown = new ContentsPopupDropdown<IBitMexCommand>(this.goPopup.transform.GetChild(1), this.bitmexMain.CoinTable);
        this.popupMessage = new ContentsPopupMessage(this.goPopup.transform.GetChild(2));

        SetCommand();
        SetMacro();

        OnRefreshMacroItem(BitMexCommandTableType.Percent);
        OnRefreshMacroItem(BitMexCommandTableType.Quantity);
        OnRefreshMacroItem(BitMexCommandTableType.Etc);

        //this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
        //this.btnPopup.onClick.AddListener(OnClickPopupOK);

        this.btnAddLog.onClick.AddListener(OnClickAddLog);
        this.btnDelLog.onClick.AddListener(OnClickDelLog);

        //this.btnSave.onClick.AddListener(OnClickSave);
        //this.btnSave.interactable = false;
    }

    private void SetCommand()
    {
        this.commandTable = new BitMexCommandTable("macro_commands");

        //퍼센트
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceBuyMagnification,
            new MarketPriceBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketPriceSellMagnification,
            new MarketPriceSellCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceBuy,
            new MarketSpecifiedBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(100); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(80); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(50); }));
        this.commandTable.Resister(BitMexCommandTableType.Percent, BitMexCommandType.MarketSpecifiedPriceSell,
            new MarketSpecifiedSellCommand(this.bitmexMain, (parameters) => { parameters.Add(20); }));

        // 수량 
        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketPriceSpecifiedQuantityBuy,
            new MarketPriceSpecifiedQuantityBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(0); }));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketPriceSpecifiedQuantitySell,
            new MarketPriceSpecifiedQuantitySellCommand(this.bitmexMain, (parameters) => { parameters.Add(0); }));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketSpecifiedQuantityBuy,
            new MarketSpecifiedQuantityBuyCommand(this.bitmexMain, (parameters) => { parameters.Add(0); }));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Quantity, BitMexCommandType.MarketSpecifiedQuantitySell,
            new MarketSpecifiedQuantitySellCommand(this.bitmexMain, (parameters) => { parameters.Add(0); }));

        // 기타
        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ChangeCoinTap,
            new ChangeCoinTapCommand(this.bitmexMain, (parameters) => { parameters.Add("XBTUSD"); }));
        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ChangeCoinTap,
            new ChangeCoinTapCommand(this.bitmexMain, (parameters) => { parameters.Add("ETHXBT"); }));
        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ChangeCoinTap,
            new ChangeCoinTapCommand(this.bitmexMain, (parameters) => { parameters.Add("XRPZ18"); }));

        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.None,
            new NoneCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.ClearPosition,
            new PositionClearCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.CancleTopActivateOrder,
            new TopActivateOrderCancleCommand(this.bitmexMain));

        this.commandTable.Resister(BitMexCommandTableType.Etc, BitMexCommandType.CancleAllActivateOrder,
            new ActivateOrderCancleCommand(this.bitmexMain));

        this.commandTable.LoadLocalCache();
    }

    private void SetMacro()
    {
        this.macroTable = new BitMexMacroTable();
        this.macroTable.LoadLocalCache(this.commandTable);
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
                        this,
                        macro);

        go.transform.SetParent(this.svHotKeys[(ushort)type].content.transform);
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

    private BitMexCommandTableType ConvertIndexToType(int index)
    {
        return (BitMexCommandTableType)index;
    }

    public void OnRefreshDropdown(BitMexCommandTableType type)
    {
        foreach (var item in this.svHotKeys[(ushort)type].content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            item.RefreshCommandDropdown();
        }
    }

    public void OnRefreshMacroItem(BitMexCommandTableType type)
    {
        foreach (var item in this.svHotKeys[(ushort)type].content.transform.GetComponentsInChildren<ContentsMacroHotKeyItem>())
        {
            Destroy(item.gameObject);
        }

        foreach (var macro in this.macroTable.GetMacros(type))
        {
            CreateHotKeyItem(type, macro);
        }

        if (this.macroTable.GetMacros(type).Count < 5)
        {
            for (int i = 0; i < 5 - this.macroTable.GetMacros(type).Count; i++)
            {
                CreateHotKeyItem(type, null);
            }
        }
    }

    private void OnClickAddLog()
    {
        Debug.Log("ContentsMacro.OnClickAdd()");
    }

    private void OnClickDelLog()
    {
        Debug.Log("ContentsMacro.OnClickDel()");
    }

    public void ExecuteMacro(List<RawKey> input)
    {
        foreach (var table in this.macroTable.GetMacroTable())
        {
            foreach (var macro in table.Value)
            {
                if (input.SequenceEqual(macro.Keys) == true)
                {
                    if (this.bitmexMain.CommandExecutor.AddCommand(macro.Command.Clone()) == false)
                    {
                        Debug.Log("executor add command timeout");
                    }
                    Debug.Log("executor add command complete");
                    break;
                }
            }
        }
    }
}
