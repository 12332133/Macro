using Assets.BitMex;
using Assets.BitMex.Commands;
using Assets.KeyBoardHook;
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

        for (int i = 0; i < 10; ++i)
        {
            var go = Instantiate(this.goHotKeyItem);
            this.listHotKeys.Add(go.GetComponent<ContentsMacroHotKeyItem>().Initialized(i, OnCombinationMacro, bitmexMain.CommandRepository.GetCommands()));
            go.transform.SetParent(this.svHotKey.content.transform);
        }

        this.btnPopup.onClick.AddListener(OnClickPopupOK);

        //for (int i = 0; i < 10; ++i)
        //{
        //    var go = Instantiate(goLogItem);
        //    go.GetComponent<ContentsMacroLogItem>().Initialized();
        //    go.GetComponent<ContentsMacroLogItem>().SetLogText("aaaa\nbbbb");
        //    go.transform.SetParent(this.svLog.content.transform);
        //}

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

    private bool OnCombinationMacro(int index, List<RawKey> rawKeys, BitMexCommandType commandType)
    {
        var command = this.bitmexMain.CommandRepository.CreateCommand(commandType);
        command.Parameters.Add(100); //< set percent, count
        return this.bitmexMain.Session.ResisterMacro(rawKeys, command);
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
        Debug.Log("ContentsMacro.OnClickSave()");
    }
}
