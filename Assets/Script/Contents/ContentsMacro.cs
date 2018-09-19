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
    private int currHotKeyIndex;
    private ContentsMacroHotKeyItem.eType currHotKeyType;

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

    public override void Initialize()
    {
        base.Initialize();

        for (int i = 0; i < 10; ++i)
        {
            var go = Instantiate(this.goHotKeyItem);
            this.listHotKeys.Add(go.GetComponent<ContentsMacroHotKeyItem>().Initialized(i, EnablePopup));
            go.transform.SetParent(this.svHotKey.content.transform);
        }

        btnPopup.onClick.AddListener(OnClickPopupOK);

        for (int i = 0; i < 10; ++i)
        {
            var go = Instantiate(goLogItem);
            go.GetComponent<ContentsMacroLogItem>().Initialized();
            go.GetComponent<ContentsMacroLogItem>().SetLogText("aaaa\nbbbb");
            go.transform.SetParent(this.svLog.content.transform);
        }

        this.btnAdd.onClick.AddListener(OnClickAdd);
        this.btnDel.onClick.AddListener(OnClickDel);

        this.btnSave.onClick.AddListener(OnClickSave);
        this.btnSave.interactable = false;
    }

    private void EnablePopup(int index, ContentsMacroHotKeyItem.eType type, string value)
    {
        this.currHotKeyIndex = index;
        this.currHotKeyType = type;
        this.inputPopup.text = value;
        this.goPopup.SetActive(true);
    }

    private void OnClickPopupOK()
    {
        this.goPopup.SetActive(false);

        this.listHotKeys[currHotKeyIndex].SetInputValue(this.currHotKeyType, this.inputPopup.text);
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
