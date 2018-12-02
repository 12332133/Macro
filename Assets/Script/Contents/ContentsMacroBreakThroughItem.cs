using Assets.BitMex;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroBreakThroughItem : MonoBehaviour
{
    [SerializeField] private Button btnStop;
    [SerializeField] private Text txtStop;

    [SerializeField] private Text txtBreakThrough;

    [SerializeField] private Dropdown dropBreakthrough;

    [SerializeField] private Button btnDelete;

    private void Awake()
    {
    }

    private void Reset()
    {
        this.btnStop = transform.Find("btnStop").GetComponent<Button>();
        this.txtStop = transform.Find("btnStop/Text").GetComponent<Text>();

        this.txtBreakThrough = transform.Find("Text").GetComponent<Text>();

        this.dropBreakthrough = transform.Find("Dropdown").GetComponent<Dropdown>();

        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroBreakThroughItem Initialized()
    {
        btnStop.onClick.AddListener(OnClickStop);
        dropBreakthrough.onValueChanged.AddListener(OnValueChangedBreakThrough);
        btnDelete.onClick.AddListener(OnClickDelete);
        return this;
    }

    private void OnClickStop()
    {
        Debug.Log("ContentsMacroBreakThroughItem.OnClickStop()");
    }

    private void OnValueChangedBreakThrough(int index)
    {
        Debug.Log("ContentsMacroBreakThroughItem.OnValueChangedBreakThrough(" + index + ")");
    }

    private void OnClickDelete()
    {
        Debug.Log("ContentsMacroBreakThroughItem.OnClickDelete()");
    }
}
