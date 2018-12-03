using Assets.BitMex;
using Assets.BitMex.Commands;
using System;
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


    private Action<IBitMexCommand, Action<IBitMexCommand>> commandChange;
    private Func<string, decimal, IBitMexCommand, ReservationTrade> resisterTrade;

    private BitMexCommandTableType commandTableType;
    private BitMexCommandTable commandTable;

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

    private void OnApplicationQuit()
    {
        this.commandTable.SaveLocalCache();
    }

    public ContentsMacroBreakThroughItem Initialized(
        BitMexCommandTableType commandTableType,
        Action<IBitMexCommand, Action<IBitMexCommand>> commandChange,
        Func<string, decimal, IBitMexCommand, ReservationTrade> resisterTrade,
        BitMexCommandTable commandTable, 
        ReservationTrade trade)
    {
        this.commandTableType = commandTableType;
        this.commandTable = commandTable;
        this.trade = trade;
        this.resisterTrade = resisterTrade;


        btnDelete.onClick.AddListener(OnClickDelete);
        btnEnable.onClick.AddListener(OnClickStop);

        RefreshCommandDropdown();

        return this;
    }

    public void RefreshCommandDropdown()
    {
        this.dropCommand.onValueChanged.RemoveAllListeners();
        this.dropCommand.ClearOptions();
        this.dropCommand.value = 0;

        foreach (var command in this.commandTable.GetCommands(BitMexCommandTableType.Percent))
        {
            this.dropCommand.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }

        if (this.trade != null)
        {
            this.dropCommand.value = this.trade.Command.RefCommandTableIndex;
            this.dropCommand.captionText.text = this.dropCommand.options[this.trade.Command.RefCommandTableIndex].text;
            this.txtEnable.text = this.trade.IsStart == true ? "Stop" : "Start";
        }
        else
        {
            this.txtEnable.text = "Start";
        }
    
        this.dropCommand.onValueChanged.AddListener(OnCommandValueChanged);

        Debug.Log("RefreshCommandDopdown");
    }

    private void OnClickStop()
    {
        if (this.trade == null)
        {
            this.trade = this.resisterTrade(string.Empty, 0, this.tempCommand);
            this.tempCommand = null;
        }
        else
        {
       
        }

        Debug.Log("ContentsMacroBreakThroughItem.OnClickStop()");
    }

    private void OnCommandValueChanged(int index)
    {
        var command = this.commandTable.FindCommand(BitMexCommandTableType.Percent, index);

        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        this.commandChange(command, (modifyedCommand) => {

            if (this.trade == null) // 최초 생성이면 
            {
                this.tempCommand = command;
            }
            else // 기존 매크로 수정이면 새로 선택/수정 한 커맨드를 바로 참조
            {
                this.trade.Command = command;
            }

        });

        Debug.Log("ContentsMacroBreakThroughItem.OnValueChangedBreakThrough(" + index + ")");
    }

    private void OnClickDelete()
    {
        Destroy(this.gameObject);
        Debug.Log("ContentsMacroBreakThroughItem.OnClickDelete()");
    }
}
