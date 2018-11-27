using Assets.BitMex;
using Assets.BitMex.Commands;
using Assets.CombinationKey;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ContentsMacro : ContentsBase
{
    [SerializeField] private ScrollRect svHotKey;
    [SerializeField] private GameObject goHotKeyItem;

    [SerializeField] private GameObject goPopup;
    [SerializeField] private InputField inputPopup;
    [SerializeField] private Button btnPopup;    

    [SerializeField] private ScrollRect svLog;
    [SerializeField] private GameObject goLogItem;
    [SerializeField] private Button btnAdd;
    [SerializeField] private Button btnDel;

    [SerializeField] private Button btnSave;

    private List<ContentsMacroHotKeyItem> listHotKeys = new List<ContentsMacroHotKeyItem>();

    private void Reset()
    {
        this.svHotKey = transform.Find("Panel/svHotKey").GetComponent<ScrollRect>();
        this.goHotKeyItem = Resources.Load<GameObject>("MacroHotKeyItem");

        this.goPopup = transform.Find("Panel/Popup").gameObject;
        this.inputPopup = transform.Find("Panel/Popup/InputField").GetComponent<InputField>();
        this.btnPopup = transform.Find("Panel/Popup/Button").GetComponent<Button>();

        this.svLog = transform.Find("Panel/LogRoot/svLog").GetComponent<ScrollRect>();
        this.goLogItem = Resources.Load<GameObject>("MacroLogItem");
        this.btnAdd = transform.Find("Panel/LogRoot/btnAdd").GetComponent<Button>();
        this.btnDel = transform.Find("Panel/LogRoot/btnDel").GetComponent<Button>();

        this.btnSave = transform.Find("Panel/btnSave").GetComponent<Button>();
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        OnRefreshAllMacroItem();

        this.btnPopup.onClick.AddListener(OnClickPopupOK);

        this.btnAdd.onClick.AddListener(OnClickAdd);
        this.btnDel.onClick.AddListener(OnClickDel);

        this.btnSave.onClick.AddListener(OnClickSave);
        this.btnSave.interactable = false;
    }

    public void WriteMacroLog(string log)
    {
        var go = Instantiate(goLogItem);
        go.GetComponent<ContentsMacroLogItem>().Initialized();
        go.GetComponent<ContentsMacroLogItem>().SetLogText(log);
        go.transform.SetParent(this.svLog.content.transform);
    }

    private void OnRefreshAllDropdown()
    {
        foreach (var item in this.listHotKeys)
        {
            item.RefreshCommandDropdown();
        }
    }

    private void OnRefreshAllMacroItem()
    {
        this.listHotKeys.Clear();

        if (bitmexMain.Macro.Macros.Count < 5)
        {
            for (int i = 0; i < bitmexMain.Macro.Macros.Count; i++)
            {
                var go = Instantiate(this.goHotKeyItem);

                this.listHotKeys.Add(
                    go.GetComponent<ContentsMacroHotKeyItem>().Initialized(
                        OnModifyCommandParameters,
                        OnRefreshAllDropdown,
                        OnRefreshAllMacroItem,
                        bitmexMain,
                        bitmexMain.Macro.Macros[i]));

                go.transform.SetParent(this.svHotKey.content.transform);
            }

            for (int i = 0; i < 5 - bitmexMain.Macro.Macros.Count; i++)
            {
                var go = Instantiate(this.goHotKeyItem);

                this.listHotKeys.Add(
                    go.GetComponent<ContentsMacroHotKeyItem>().Initialized(
                        OnModifyCommandParameters,
                        OnRefreshAllDropdown,
                        OnRefreshAllMacroItem,
                        bitmexMain,
                        null));

                go.transform.SetParent(this.svHotKey.content.transform);
            }
        }
        else
        {
            foreach (var macro in bitmexMain.Macro.Macros)
            {
                var go = Instantiate(this.goHotKeyItem);

                this.listHotKeys.Add(
                    go.GetComponent<ContentsMacroHotKeyItem>().Initialized(
                        OnModifyCommandParameters,
                        OnRefreshAllDropdown,
                        OnRefreshAllMacroItem,
                        bitmexMain,
                        macro));

                go.transform.SetParent(this.svHotKey.content.transform);
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

    private void OnClickAdd()
    {
        Debug.Log("ContentsMacro.OnClickAdd()");
    }

    private void OnClickDel()
    {
        Debug.Log("ContentsMacro.OnClickDel()");
    }

    private void OnClickSave()
    {
        foreach (var item in this.listHotKeys)
        {
            item.ResisterCompleteMaro();
        }

        this.bitmexMain.CommandTable.SaveLocalCache();
        this.bitmexMain.Macro.SaveLocalCache();
    }
}
