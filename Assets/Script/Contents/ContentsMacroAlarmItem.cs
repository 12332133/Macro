using Assets.BitMex;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroAlarmItem : MonoBehaviour
{
    [SerializeField] private Button btnStop;
    [SerializeField] private Text txtStop;

    [SerializeField] private Text txtAlarm;

    [SerializeField] private Dropdown dropdownLeft;
    [SerializeField] private Dropdown dropdownRight;

    [SerializeField] private Button btnDelete;

    private void Awake()
    {
    }

    private void Reset()
    {
        this.btnStop = transform.Find("btnStop").GetComponent<Button>();
        this.txtStop = transform.Find("btnStop/Text").GetComponent<Text>();

        this.txtAlarm = transform.Find("Text").GetComponent<Text>();

        this.dropdownLeft = transform.Find("DropdownL").GetComponent<Dropdown>();
        this.dropdownRight = transform.Find("DropdownR").GetComponent<Dropdown>();

        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroAlarmItem Initialized()
    {
        btnStop.onClick.AddListener(OnClickStop);
        dropdownLeft.onValueChanged.AddListener(OnValueChangedAlarmLeft);
        dropdownRight.onValueChanged.AddListener(OnValueChangedAlarmRight);
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
