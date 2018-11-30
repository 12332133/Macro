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

    private Action<IBitMexCommand, Action> modifyCommandParameters;
    private Action<BitMexCommandTableType> refreshDropdown;
    private Action<BitMexCommandTableType> refreshMacroItem;

    private IBitMexMainAdapter bitmexMain;
    private IBitMexCommand tempCommand;
    private BitMexCommandTableType commandTableType;
    private Macro macro;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroHotKeyItem Initialized(
        BitMexCommandTableType commandTableType,
        Action<IBitMexCommand, Action> modifyCommandParameters,
        Action<BitMexCommandTableType> refreshDropdown,
        Action<BitMexCommandTableType> refreshMacroItem,
        IBitMexMainAdapter bitmexMain,
        Macro macro)
    {
        this.commandTableType = commandTableType;
        this.bitmexMain = bitmexMain;
        this.macro = macro;
        this.inputHotkey.OnKeyChanged = OnKeyChanged;
        this.modifyCommandParameters = modifyCommandParameters;
        this.refreshDropdown = refreshDropdown;
        this.refreshMacroItem = refreshMacroItem;

        this.btnDelete.onClick.AddListener(OnClickDelete);

        RefreshCommandDropdown();
        return this;
    }

    public void RefreshCommandDropdown()
    {
        this.dropdown.onValueChanged.RemoveAllListeners();

        this.dropdown.ClearOptions();
        this.dropdown.value = 0;

        foreach (var command in this.bitmexMain.CommandTable.GetCommands(this.commandTableType))
        {
            this.dropdown.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }

        if (this.macro != null)
        {
            this.dropdown.value = this.macro.Command.RefCommandTableIndex;
            this.dropdown.captionText.text = this.dropdown.options[this.macro.Command.RefCommandTableIndex].text;
            this.inputHotkey.CombinationKey.Clear();
            this.inputHotkey.CombinationKey.AddRange(this.macro.Keys);
            this.inputHotkey.RefreshCombinationString();
        }
        else 
        {
            if (this.tempCommand != null) 
            {
                this.dropdown.value = this.tempCommand.RefCommandTableIndex;
                this.dropdown.captionText.text = this.dropdown.options[this.tempCommand.RefCommandTableIndex].text;
            }
        }

        this.dropdown.onValueChanged.AddListener(OnValueChanged);

        Debug.Log("RefreshCommandDropdown");
    }

    private bool OnKeyChanged(List<RawKey> keys)
    {
        if (this.macro == null) // 최초 생성이면 중복 단축키 검사만 
        {
            if (this.bitmexMain.Macro.IsEqualKeys(keys) == true)
            {
                if (this.tempCommand != null) // 매크로 키 채크 + 기존에 선택한 커맨드가 있으면 매크로 등록
                {
                    this.macro = this.bitmexMain.Macro.Resister(this.inputHotkey.CombinationKey, this.tempCommand);
                    this.tempCommand = null; 
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        return this.bitmexMain.Macro.ModifyRawKeys(this.macro, keys); // 기존 매크로 수정이면 바로 수정
    }

    private void OnValueChanged(int index)
    {
        var command = this.bitmexMain.CommandTable.FindCommand(this.commandTableType, index);

        if (command.CommandType == BitMexCommandType.None)
        {
            return;
        }

        this.modifyCommandParameters(command, () =>
        {
            if (this.macro == null) // 최초 생성이면 
            {
                if (this.inputHotkey.CombinationKey.Count > 0) // 기존 완성 조합키 + 커맨드 선택이면 매크로 등록
                {
                    this.macro = this.bitmexMain.Macro.Resister(this.inputHotkey.CombinationKey, command);
                    this.tempCommand = null;
                }
                else // 완성 조합키 없이 커맨드만 선택 했으면 
                {
                    this.tempCommand = command;
                }
            }
            else // 기존 매크로 수정이면 새로 선택/수정 한 커맨드를 바로 참조
            {
                this.bitmexMain.Macro.ModifyCommand(this.commandTableType, this.macro.Index, command);
            }

            this.refreshDropdown(this.commandTableType);
        });

        Debug.Log(command.CommandType);
    }

    private void OnClickDelete()
    {
        if (this.macro != null) // 기존 등록된 매크로 삭제
        {
            this.bitmexMain.Macro.RemoveAt(this.commandTableType, this.macro.Index);
        }

        Destroy(this.gameObject);

        Debug.Log("OnClickDelete");
    }

    private void OnRemoveCommand(int index)
    {
        var command = this.bitmexMain.CommandTable.FindCommand(this.commandTableType, index);

        // CommandTable의 커스텀 커맨드만 삭제한다.
        if (this.bitmexMain.CommandTable.Remove(command) == false)
        {
            // 삭제 불가능한 커맨드 팝업창 출력.
            return;
        }

        // Macro에서 커맨드를 참조중인놈을 찾아서 삭제한다.
        if (this.bitmexMain.Macro.RemoveByCommand(command) == false)
        {
            return;
        }

        // 매크로 아이템을 모두 재 배치한다.
        this.refreshMacroItem(this.commandTableType);
    }

    //public void ResisterMacro()
    //{
    //    // 현재 캐시중인 정보를 저장한다. macro 변수에 생성된 macro를 참조시킨다.
    //    if (this.macro == null)
    //    {
    //        if (this.command != null && this.inputHotkey.CombinationKey.Count > 0)
    //        {
    //            //if (this.bitmexMain.Macro.IsEqualKeys(this.inputHotkey.CombinationKey) == false)
    //            //{
    //            //    // 중복 키, 키 재설정 팝업창 출력
    //            //    return;
    //            //}

    //            this.macro = this.bitmexMain.Macro.Resister(this.inputHotkey.CombinationKey, this.command);
    //        }
    //    }
    //}
}
