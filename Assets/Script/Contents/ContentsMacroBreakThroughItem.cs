using Assets.BitMex;
using Assets.BitMex.Commands;
using System;
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


    private BitMexCommandTableType commandTableType;
    private IContentsReservation content;

    private IBitMexCommand tempCommand;

    private ReservationTrade trade;

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

    public ContentsMacroBreakThroughItem Initialized(
        BitMexCommandTableType commandTableType,
        IContentsReservation content,
        ReservationTrade trade)
    {
        this.commandTableType = commandTableType;
        this.content = content;
        this.trade = trade;

        btnDelete.onClick.AddListener(OnClickDelete);
        btnEnable.onClick.AddListener(OnClickStop);

        RefreshCommandDropdown();
        RefreshCoinDropdown();
        RefreshMarketPrice();
        RefreshStart();

        return this;
    }

    public void RefreshCommandDropdown()
    {
        this.dropCommand.onValueChanged.RemoveAllListeners();
        this.dropCommand.options.Clear();
        this.dropCommand.value = 0;
        foreach (var command in this.content.CommandTable.GetCommands(BitMexCommandTableType.Percent))
        {
            this.dropCommand.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }

        if (this.trade != null)
        {
            this.dropCommand.value = this.trade.Command.RefCommandTableIndex;
            this.dropCommand.Select();
            this.dropCommand.RefreshShownValue();
        }
        else
        {
            if (this.tempCommand != null)
            {
                this.dropCommand.value = this.tempCommand.RefCommandTableIndex;
                this.dropCommand.Select();
                this.dropCommand.RefreshShownValue();
            }
        }

        this.dropCommand.onValueChanged.AddListener(OnCommandValueChanged);

        Debug.Log("RefreshCommandDopdown");
    }

    public void RefreshCoinDropdown()
    {
        this.dropName.options.Add(new Dropdown.OptionData(string.Empty));
        foreach (var coin in this.content.CoinTable.Coins)
        {
            this.dropName.options.Add(new Dropdown.OptionData(coin.Key));

            if (this.trade != null && this.trade.CoinName == coin.Key)
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
        if (this.trade != null)
        {
            this.inputValue.text = this.trade.TargetPrice.ToString();
        }
        else
        {
            this.inputValue.text = "시장가 입력";
        }

        this.inputValue.onEndEdit.AddListener(OnMarketPriceChanged);
    }

    public void RefreshStart()
    {
        if (this.trade != null)
        {
            this.txtEnable.text = this.trade.IsStart == true ? "정지" : "시작";
        }
        else
        {
            this.txtEnable.text = "시작";
        }
    }

    private void OnClickStop()
    {
        if (this.trade != null)
        {
            if (this.content.BitMexSession.IsLogined == false)
            {
                this.content.PopupAlret.OnEnablePopup("비트맥스에 로그인 해주세요");
                return;
            }

            if (this.trade != null)
            {
                if (this.trade.IsStart == true)
                {
                    this.trade.IsStart = false;
                    this.txtEnable.text = "시작";
                }
                else
                {
                    //if (this.trade.IsVaildMomentPrice(1000) == false)
                    //{
                    //this.content.PopupAlret.OnEnablePopup("설정 시점의 시장가와 현재 시장가의 차이가 큽니다. 목표 시장가를 다시 설정해 주세요");
                    //return;
                    //}

                    this.trade.IsStart = true;
                    this.txtEnable.text = "정지";
                }
            }
            else
            {
                this.txtEnable.text = "시작";
            }
        }

        Debug.Log("ContentsMacroBreakThroughItem.OnClickStop()");
    }

    private void OnMarketPriceChanged(string price)
    {
        if (this.trade == null)
        {
            if (this.tempCommand != null && this.dropName.value > 0) // 코인 선택 + 커맨드 선택
            {
                this.trade = this.content.ResisterTrade(
                    this.dropName.captionText.text,
                    decimal.Parse(this.inputValue.text, System.Globalization.NumberStyles.Any), 
                    this.tempCommand,
                    this);

                if (this.trade != null)
                {
                    this.content.OnRefreshReservationItem();
                }
            }
        }
        else
        {
            this.trade.TargetPrice = decimal.Parse(this.inputValue.text, System.Globalization.NumberStyles.Any);
        }
    }

    private void OnCoinValueChanged(int index)
    {
        if (index == 0)
        {
            return;
        }

        if (this.trade == null)
        {
            if (this.inputValue.text.Equals("시장가 입력") == false && this.tempCommand != null) // 시장가 입력 + 커맨드 선택 이면 스퀘쥴 등록
            {
                this.trade = this.content.ResisterTrade(
                    this.dropName.captionText.text,
                    decimal.Parse(this.inputValue.text, System.Globalization.NumberStyles.Any), 
                    this.tempCommand,
                    this);

                if (this.trade != null)
                {
                    this.content.OnRefreshReservationItem();
                }
            }
        }
        else
        {
            this.trade.CoinName = this.dropName.captionText.text;
        }
    }

    private void OnCommandValueChanged(int index)
    {
        var command = this.content.CommandTable.FindCommand(this.commandTableType, index); // 선택한 커맨드

        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        this.content.PopupInput.OnEnablePopup(
                      command,
                      command.Parameters[0].ToString(),
                      OnAddCommand,
                      OnModifyCommand,
                      OnRemoveCommand);

        Debug.Log(command.CommandType);
    }

    private void SetCommand(IBitMexCommand command)
    {
        if (this.trade == null) // 최초 생성이면 
        {
            if (this.inputValue.text.Equals("시장가 입력") == false && this.dropName.value > 0) // 시장가 입력 + 코인 선택이면 추가
            {
                this.trade = this.content.ResisterTrade(
                    this.dropName.captionText.text,
                    decimal.Parse(this.inputValue.text, System.Globalization.NumberStyles.Any),
                    command,
                    this);

                if (this.trade != null)
                {
                    this.content.OnRefreshReservationItem();
                    return;
                }
            }
            else // 완성 조합키 없이 커맨드만 선택 했으면 
            {
                this.tempCommand = command;
            }
        }
        else // 기존 매크로 수정이면 새로 선택/수정 한 커맨드를 바로 참조
        {
            this.trade.Command = command;
        }

        this.content.OnRefreshDropdown();
    }

    private void OnAddCommand(IBitMexCommand command, string value)
    {
        var newCommand = command.Clone();
        newCommand.Parameters.Clear();
        newCommand.Parameters.Add(value);

        this.content.CommandTable.InsertAt(newCommand);

        SetCommand(newCommand);
    }

    private void OnModifyCommand(IBitMexCommand command, string value)
    {
        command.Parameters.Clear();
        command.Parameters.Add(value);

        this.content.CommandTable.ModifyCommand(command);

        SetCommand(command);
    }

    private void OnRemoveCommand(IBitMexCommand command, string value)
    {
        // CommandTable의 커스텀 커맨드만 삭제한다.
        if (this.content.CommandTable.Remove(command) == false)
        {
            // 삭제 불가능한 커맨드 팝업창 출력.
            this.content.PopupAlret.OnEnablePopup("삭제 불가능한 명령");
            return;
        }

        this.content.RemoveTradeByCommand(command);

        this.content.OnRefreshReservationItem();
    }

    public void OnClickDelete()
    {
        if (this.trade != null)
        {
            this.content.RemoveTrade(this.trade);
        }

        Destroy(this.gameObject);
        Debug.Log("ContentsMacroBreakThroughItem.OnClickDelete()");
    }
}
