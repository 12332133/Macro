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

    private int index;
    private Func<int, List<RawKey>, bool> onKeyChanged;
    private Func<int, IBitMexCommand, bool> onCommandChanged;
    private Func<List<RawKey>, IBitMexCommand, bool> onResisterCommand;
    private IBitMexMainAdapter bitmexMain;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
    }

    public ContentsMacroHotKeyItem Initialized(
        int index,
        Func<int, List<RawKey>, bool> onKeyChanged,
        Func<int, IBitMexCommand, bool> onCommandChanged, 
        Func<List<RawKey>, IBitMexCommand, bool> onResisterCommand,
        IBitMexMainAdapter bitmexMain,
        Macro macro)
    {
        this.index = index;
        this.bitmexMain = bitmexMain;

        this.onKeyChanged = onKeyChanged;
        this.onResisterCommand = onResisterCommand;
        this.onCommandChanged = onCommandChanged;
        this.inputHotkey.OnKeyChanged = OnKeyChanged;
       
        this.dropdown.onValueChanged.AddListener(OnValueChanged);

        RefreshCommandDropdown(macro);
        return this;
    }

    public void RefreshCommandDropdown(Macro macro = null)
    {
        this.dropdown.options.Clear();
        foreach (var command in this.bitmexMain.CommandTable.Commands)
        {
            if (macro != null && command.CommandType == macro.Command.CommandType)
            {
                this.dropdown.value = this.dropdown.options.Count;
                this.inputHotkey.CombinationKey.AddRange(macro.Keys);
                this.inputHotkey.RefreshCombinationString();
            }

            this.dropdown.options.Add(new Dropdown.OptionData(command.GetCommandText()));
        }
    }

    private bool OnKeyChanged(List<RawKey> keys)
    {
        return this.bitmexMain.Macro.ModifyRawKeys(this.index, keys);
    }

    private void OnValueChanged(int index)
    {
        var command = this.bitmexMain.CommandTable.FindCommand(index);

        switch (command.CommandType)
        {
            case BitMexCommandType.OrderCommandCreate:
                this.onResisterCommand(this.inputHotkey.CombinationKey, command);
                break;
            default:
                this.onCommandChanged(this.index, command);
                break;
        }

        Debug.Log(command.CommandType);

        //if (onCompleteCombinationMacro(this.index, this.inputHotkey.CombinationKey, commandType) == false)
        //{
        //    this.inputHotkey.ResetCombinationKey();
        //    this.dropdown.value = 0;
        //}
    }
}
