using Assets.BitMex;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ContentsAlarm;

public class ContentsMacroAlarmItem : MonoBehaviour
{
    [SerializeField] private Button btnEnable;
    [SerializeField] private Text txtEnable;

    [SerializeField] private Dropdown dropName;

    [SerializeField] private InputField inputValue;

    [SerializeField] private Dropdown dropCommand;

    [SerializeField] private Dropdown dropCount;

    [SerializeField] private Button btnDelete;

    public Action<ContentsMacroAlarmItem> OnChangePrice { get; set; }
    public Action<ContentsMacroAlarmItem> OnChangeRunning { get; set; }
    public Action<ContentsMacroAlarmItem> OnChangeCoinType { get; set; }
    public Action<ContentsMacroAlarmItem> OnChangeAlramCount { get; set; }
    public Action<ContentsMacroAlarmItem> OnRemoveItem { get; set; }

    public string Price { get { return this.inputValue.text; } }
    public string CoinName { get { return this.dropName.captionText.text; } }
    public string RunningState { get { return this.txtEnable.text; } set { this.txtEnable.text = value; } }
    public int AlramCount { get { return Int32.Parse(this.dropCount.captionText.text); } }
    public ReservationAlram RefAlram { get; set; }

    private void Awake()
    {
    }

    private void Reset()
    {
        this.btnEnable = transform.Find("btnEnable").GetComponent<Button>();
        this.txtEnable = transform.Find("btnEnable/Text").GetComponent<Text>();

        this.dropName = transform.Find("dropName").GetComponent<Dropdown>();

        this.inputValue = transform.Find("inputValue").GetComponent<InputField>();

        this.dropCommand = transform.Find("dropCommand").GetComponent<Dropdown>();

        this.dropCount = transform.Find("dorpCount").GetComponent<Dropdown>();

        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroAlarmItem Initialized()
    {
        btnDelete.onClick.AddListener(OnClickDelete);
        btnEnable.onClick.AddListener(OnClickStop);
        return this;
    }

    public void RefreshAlramCountDropdown(int limitCount)
    {
        for (int i = 0; i < limitCount; i++)
        {
            this.dropCount.options.Add(new Dropdown.OptionData(i.ToString()));

            if (this.RefAlram != null && this.RefAlram.AlramCount == i)
            {
                this.dropCount.value = this.dropCount.options.Count - 1;
                this.dropCount.Select();
                this.dropCount.RefreshShownValue();
            }
        }

        this.dropCount.onValueChanged.AddListener(OnAlramCountChanged);
    }

    public void RefreshCoinDropdown(Dictionary<string, BitMexCoin> coins)
    {
        this.dropName.options.Add(new Dropdown.OptionData(string.Empty));
        foreach (var coin in coins)
        {
            this.dropName.options.Add(new Dropdown.OptionData(coin.Key));

            if (this.RefAlram != null && this.RefAlram.CoinName == coin.Key)
            {
                this.dropName.value = this.dropName.options.Count - 1;
                this.dropName.Select();
                this.dropName.RefreshShownValue();
            }
        }

        this.dropName.onValueChanged.AddListener(OnCoinValueChanged);
    }

    public void RefreshMarketPrice()
    {
        if (this.RefAlram != null)
        {
            this.inputValue.text = this.RefAlram.TargetPrice.ToString();
        }
        else
        {
            this.inputValue.text = "시장가 입력";
        }

        this.inputValue.onEndEdit.AddListener(OnMarketPriceChanged);
    }

    public void RefreshStart()
    {
        if (this.RefAlram != null)
        {
            this.txtEnable.text = this.RefAlram.IsStart == true ? "정지" : "시작";
        }
        else
        {
            this.txtEnable.text = "시작";
        }
    }

    private void OnClickStop()
    {
        OnChangeRunning(this);
    }

    private void OnMarketPriceChanged(string price)
    {
        OnChangePrice(this);
    }

    private void OnCoinValueChanged(int index)
    {
        if (index == 0)
        {
            return;
        }

        OnChangeCoinType(this);
    }

    private void OnAlramCountChanged(int index)
    {
        OnChangeAlramCount(this);
    }

    public void OnClickDelete()
    {
        OnRemoveItem(this);
    }
}
