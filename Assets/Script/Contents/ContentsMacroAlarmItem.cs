using Assets.BitMex;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroAlarmItem : MonoBehaviour
{
    [SerializeField] private Button btnEnable;
    [SerializeField] private Text txtEnable;

    [SerializeField] private Dropdown dropName;

    [SerializeField] private InputField inputValue;

    [SerializeField] private Dropdown dropCommand;

    [SerializeField] private Dropdown dropCount;

    [SerializeField] private Button btnDelete;

    private void Awake()
    {
    }

    private void Reset()
    {
        this.btnEnable = transform.Find("btnEnable").GetComponent<Button>();
        this.txtEnable = transform.Find("btnEnable/Text").GetComponent<Text>();

        this.dropCommand = transform.Find("dropName").GetComponent<Dropdown>();

        this.inputValue = transform.Find("inputValue").GetComponent<InputField>();

        this.dropCommand = transform.Find("dropCommand").GetComponent<Dropdown>();

        this.dropCount = transform.Find("dorpCount").GetComponent<Dropdown>();

        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroAlarmItem Initialized()
    {
        btnEnable.onClick.AddListener(OnClickStop);
        dropCommand.onValueChanged.AddListener(OnValueChangedAlarmLeft);
        dropCount.onValueChanged.AddListener(OnValueChangedAlarmRight);
        btnDelete.onClick.AddListener(OnClickDelete);
        return this;
    }

    private void OnClickStop()
    {
        Debug.Log("ContentsMacroAlarmItem.OnClickStop()");
    }

    private void OnValueChangedAlarmLeft(int index)
    {
        Debug.Log("ContentsMacroAlarmItem.OnValueChangedAlarmLeft(" + index + ")");
    }

    private void OnValueChangedAlarmRight(int index)
    {
        Debug.Log("ContentsMacroAlarmItem.OnValueChangedAlarmRight(" + index + ")");
    }

    private void OnClickDelete()
    {
        Debug.Log("ContentsMacroAlarmItem.OnClickDelete()");
    }
}
