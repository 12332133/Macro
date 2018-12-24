using Assets.BitMex;
using Assets.BitMex.Commands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using static ContentsBreakThrough;

public class ContentsMacroBreakThroughItem : MonoBehaviour
{
    [SerializeField] private Button btnEnable;
    [SerializeField] private Text txtEnable;

    [SerializeField] private Dropdown dropName;

    [SerializeField] private InputField inputValue;

    [SerializeField] private Dropdown dropCommand;

    [SerializeField] private Button btnDelete;

    public Action<ContentsMacroBreakThroughItem> OnChangePrice { get; set; }
    public Action<ContentsMacroBreakThroughItem> OnChangeRunning { get; set; }
    public Action<ContentsMacroBreakThroughItem> OnChangeCoinType { get; set; }
    public Action<ContentsMacroBreakThroughItem, int> OnChangeCommand { get; set; }
    public Action<ContentsMacroBreakThroughItem> OnRemoveItem { get; set; }

    public string Price { get { return this.inputValue.text; } }
    public string CoinName { get { return this.dropName.captionText.text; } }
    public string RunningState { get { return this.txtEnable.text; } set { this.txtEnable.text = value; } }
    public IBitMexCommand TempCommand { get; set; }
    public ReservationTrade RefTrade { get; set; }

    private void Awake()
    {
    }

    private void Reset()
    {
        this.btnEnable = transform.Find("btnEnable").GetComponent<Button>();
        this.txtEnable = transform.Find("btnEnable/Text").GetComponent<Text>();

        this.dropName = transform.Find("dropName").GetComponent<Dropdown>();

        this.inputValue = transform.Find("inputValue").GetComponent<InputField>();

        this.dropCommand = transform.Find("downCommand").GetComponent<Dropdown>();

        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroBreakThroughItem Initialized()
    {
        btnDelete.onClick.AddListener(OnClickDelete);
        btnEnable.onClick.AddListener(OnClickStop);
        return this;
    }

    public void RefreshCommandDropdown(List<IBitMexCommand> commands)
    {
        this.dropCommand.onValueChanged.RemoveAllListeners();
        this.dropCommand.options.Clear();
        this.dropCommand.value = 0;
        foreach (var command in commands)
        {
            this.dropCommand.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }

        if (this.RefTrade != null)
        {
            this.dropCommand.value = this.RefTrade.Command.RefCommandTableIndex;
            this.dropCommand.Select();
            this.dropCommand.RefreshShownValue();
        }
        else
        {
            if (this.TempCommand != null)
            {
                this.dropCommand.value = this.TempCommand.RefCommandTableIndex;
                this.dropCommand.Select();
                this.dropCommand.RefreshShownValue();
            }
        }

        this.dropCommand.onValueChanged.AddListener(OnCommandValueChanged);
    }

    public void RefreshCoinDropdown(List<string> coins)
    {
        this.dropName.options.Add(new Dropdown.OptionData(string.Empty));
        foreach (var coin in coins)
        {
            this.dropName.options.Add(new Dropdown.OptionData(coin));

            if (this.RefTrade != null && this.RefTrade.CoinName == coin)
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
        if (this.RefTrade != null)
        {
            this.inputValue.text = this.RefTrade.TargetPrice.ToString();
        }
        else
        {
            this.inputValue.text = "시장가 입력";
        }

        this.inputValue.onEndEdit.AddListener(OnMarketPriceChanged);
    }

    public void RefreshStart()
    {
        if (this.RefTrade != null)
        {
            this.txtEnable.text = this.RefTrade.IsStart == true ? "정지" : "시작";
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

    private void OnCommandValueChanged(int index)
    {
        OnChangeCommand(this, index);
    }

    public void OnClickDelete()
    {
        OnRemoveItem(this);
    }
}
