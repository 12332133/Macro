using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.BitMex.Commands;
using Assets.CombinationKey;
using Assets.BitMex;

public class ContentsMacroHotKeyItem : MonoBehaviour
{
    [SerializeField] private MacroInputField inputHotkey;
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private Button btnDelete;
    
    private BitMexCommandTableType commandTableType;
    private IContentsMacro content;

    private IBitMexCommand tempCommand;
    private Macro macro;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroHotKeyItem Initialized(
        BitMexCommandTableType commandTableType,
        IContentsMacro content,
        Macro macro)
    {
        this.commandTableType = commandTableType;
        this.content = content;
        this.macro = macro;

        this.inputHotkey.OnKeyChanged = OnKeyChanged;

        this.btnDelete.onClick.AddListener(OnClickDelete);

        RefreshCommandDropdown();

        return this;
    }

    public void RefreshCommandDropdown()
    {
        this.dropdown.onValueChanged.RemoveAllListeners();
        this.dropdown.options.Clear();
        this.dropdown.value = 0;

        foreach (var command in this.content.CommandTable.GetCommandsByTableType(this.commandTableType))
        {
            this.dropdown.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }

        if (this.macro != null)
        {
            this.dropdown.value = this.macro.Command.RefCommandTableIndex;
            this.dropdown.Select();
            this.dropdown.RefreshShownValue();

            this.inputHotkey.CombinationKey.Clear();
            this.inputHotkey.CombinationKey.AddRange(this.macro.Keys);
            this.inputHotkey.RefreshCombinationString();
        }
        else
        {
            if (this.tempCommand != null)
            {
                this.dropdown.value = this.tempCommand.RefCommandTableIndex;
                this.dropdown.Select();
                this.dropdown.RefreshShownValue();
            }
        }

        this.dropdown.onValueChanged.AddListener(OnValueChanged);

        Debug.Log("RefreshCommandDropdown");
    }

    private bool OnKeyChanged(List<RawKey> keys)
    {
        if (this.macro == null) // 최초 생성이면 중복 단축키 검사만 
        {
            if (this.content.MacroTable.IsEqualKeys(keys) == true)
            {
                if (this.tempCommand != null) // 매크로 키 채크 + 기존에 선택한 커맨드가 있으면 매크로 등록
                {
                    this.macro = this.content.MacroTable.Resister(keys, this.tempCommand);
                    this.tempCommand = null;
                }
                return true;
            }
            else
            {
                this.content.PopupAlret.OnEnablePopup("중복 단축키");
                return false;
            }
        }

        return this.content.MacroTable.ModifyRawKeys(this.macro, keys); // 기존 매크로 수정이면 바로 수정
    }

    private void OnValueChanged(int index)
    {
        var command = this.content.CommandTable.FindCommand(this.commandTableType, index); // 선택한 커맨드

        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        switch (command.CommandType)
        {
            case BitMexCommandType.ChangeCoinTap:
                this.content.PopupDropdown.OnEnablePopup(
                    command,
                    command.Parameters[0].ToString(), 
                    OnAddCommand<string>, 
                    OnModifyCommand<string>, 
                    OnRemoveCommand<string>);
                break;
            case BitMexCommandType.CancleAllActivateOrder:
            case BitMexCommandType.CancleTopActivateOrder:
            case BitMexCommandType.ClearPosition:
                OnModifyCommand<int>(command, 0);
                break;
            default:
                this.content.PopupInput.OnEnablePopup(
                    command,
                    command.Parameters[0].ToString(),
                    OnAddCommand<int>,
                    OnModifyCommand<int>,
                    OnRemoveCommand<int>);
                break;
        }

        Debug.Log(command.CommandType);
    }

    private void SetCommand(IBitMexCommand command)
    {
        if (this.macro == null) // 최초 생성이면 
        {
            if (this.inputHotkey.CombinationKey.Count > 0) // 기존 완성 조합키 + 커맨드 선택이면 매크로 등록
            {
                this.macro = this.content.MacroTable.Resister(this.inputHotkey.CombinationKey, command);
                this.tempCommand = null;
            }
            else // 완성 조합키 없이 커맨드만 선택 했으면 
            {
                this.tempCommand = command;
            }
        }
        else // 기존 매크로 수정이면 새로 선택/수정 한 커맨드를 바로 참조
        {
            this.macro.Command = command;
        }
    }

    private void OnAddCommand<T>(IBitMexCommand command, T value)
    {
        var newCommand = command.Clone();
        newCommand.Parameters.Clear();
        newCommand.Parameters.Add(value);

        this.content.CommandTable.Insert(newCommand);

        SetCommand(newCommand);

        this.content.OnRefreshDropdown(command.CommandTableType);
    }

    private void OnModifyCommand<T>(IBitMexCommand command, T value)
    {
        command.Parameters.Clear();
        command.Parameters.Add(value);

        this.content.CommandTable.ModifyCommand(command);

        SetCommand(command);

        this.content.OnRefreshDropdown(command.CommandTableType);
    }

    private void OnRemoveCommand<T>(IBitMexCommand command, T value)
    {
        // CommandTable의 커스텀 커맨드만 삭제한다.
        if (this.content.CommandTable.Remove(command) == false)
        {
            // 삭제 불가능한 커맨드 팝업창 출력.
            this.content.PopupAlret.OnEnablePopup("삭제 불가능한 명령");
            return;
        }

        // Macro에서 커맨드를 참조중인놈을 찾아서 삭제한다.
        this.content.MacroTable.RemoveByCommand(command);

        this.content.OnRefreshMacroItem(command.CommandTableType);
    }

    private void OnClickDelete()
    {
        if (this.macro != null) // 기존 등록된 매크로 삭제
        {
            this.content.MacroTable.Remove(this.commandTableType, this.macro);
        }

        Destroy(this.gameObject);

        Debug.Log("OnClickDelete");
    }
}
