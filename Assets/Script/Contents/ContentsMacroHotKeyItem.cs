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

    private Func<int, List<RawKey>, bool> onKeyChanged;
    private Func<int, IBitMexCommand, bool> onCommandChanged;
    private Func<List<RawKey>, IBitMexCommand, bool> onResisterCommand;
    private Action<IBitMexCommand> modifyCommandParameters;
    private Action refreshAllDropwon;
    private Action refreshAllMacroItem;
    private IBitMexMainAdapter bitmexMain;
    private Macro macro;
    private IBitMexCommand command;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();

        this.btnDelete = transform.Find("btnDelete").GetComponent<Button>();
    }

    public ContentsMacroHotKeyItem Initialized(
        Action<IBitMexCommand> modifyCommandParameters,
        Action refreshAllDropwon,
        Action refreshAllMacroItem,
        IBitMexMainAdapter bitmexMain,
        Macro macro)
    {
        this.bitmexMain = bitmexMain;
        this.macro = macro;
        this.inputHotkey.OnKeyChanged = OnKeyChanged;
        this.modifyCommandParameters = modifyCommandParameters;
        this.refreshAllDropwon = refreshAllDropwon;
        this.refreshAllMacroItem = refreshAllMacroItem;

        this.dropdown.onValueChanged.AddListener(OnValueChanged);
        this.btnDelete.onClick.AddListener(OnClickDelete);

        RefreshCommandDropdown();
        return this;
    }

    private void OnClickDelete()
    {
        Debug.Log("OnClickDelete()");
    }

    public void RefreshCommandDropdown()
    {
        this.dropdown.options.Clear();
        foreach (var command in this.bitmexMain.CommandTable.Commands)
        {
            this.dropdown.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }

        if (this.macro != null)
        {
            this.dropdown.value = this.macro.Command.RefCommandTableIndex;
            this.inputHotkey.CombinationKey.AddRange(this.macro.Keys);
            this.inputHotkey.RefreshCombinationString();
        }
    }

    private bool OnKeyChanged(List<RawKey> keys)
    {
        if (this.macro == null) // 최초 생성이면 중복 단축키 검사만 
        {
           return this.bitmexMain.Macro.IsEqualKeys(keys);
        }

        return this.bitmexMain.Macro.ModifyRawKeys(this.macro.Index, keys); // 기존 매크로 수정이면 바로 수정
    }

    private void OnValueChanged(int index)
    {
        var command = this.bitmexMain.CommandTable.FindCommand(index);
        
        switch (command.CommandType)
        {
            case BitMexCommandType.None:
                break;
            case BitMexCommandType.OrderCommandCreate: // 커맨드 생성만 후 추가(신규 커맨드는 참조중인 매크로가 없기 때문에 드랍다운만 갱신한다)
                var newCommand = this.bitmexMain.CommandTable.CreateByCreator(command.RefCommandTableIndex);
                this.modifyCommandParameters(newCommand);
                this.bitmexMain.CommandTable.InsertAt(newCommand);
                this.refreshAllDropwon();
                break;
            default:
                this.modifyCommandParameters(command);
                if (this.macro == null) // 최초 생성이면 캐시만  
                {
                    this.command = command;
                }
                else // 기존 매크로 수정이면 새로 선택/수정 한 커맨드를 참조
                {
                    this.bitmexMain.Macro.ModifyCommand(this.macro.Index, command);
                }
                this.refreshAllDropwon();
                break;
        }

        Debug.Log(command.CommandType);
    }

    private void OnRemoveCommand(int index)
    {
        var command = this.bitmexMain.CommandTable.FindCommand(index);

        // CommandTable의 커스텀 커맨드만 삭제한다.
        if (this.bitmexMain.CommandTable.Remove(command) == false)
        {
            // 삭제 불가능한 커맨드 팝업창 올려줌.
            return;
        }

        // Macro에서 커맨드를 참조중인놈을 찾아서 삭제한다.
        if (this.bitmexMain.Macro.RemoveAt(command) == false)
        {
            return;
        }

        // 매크로 아이템을 모두 재 배치한다.
        this.refreshAllMacroItem();
    }

    public void ResisterCompleteMaro()
    {
        // 현재 캐시중인 정보를 저장한다. macro 변수에 생성된 macro를 참조시킨다.
        if (this.macro == null)
        {
            if (this.command != null && this.inputHotkey.CombinationKey.Count > 0)
            {
                this.macro = this.bitmexMain.Macro.Resister(this.inputHotkey.CombinationKey, this.command);
            }
        }
    }
}
