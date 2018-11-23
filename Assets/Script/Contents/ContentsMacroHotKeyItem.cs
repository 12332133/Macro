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
    private Func<int, BitMexCommandType, bool> onCommandChangedChanged;
    private Func<int, List<RawKey>, BitMexCommandType, bool> onResisterCommand;

    private List<BitMexCommandType> commandTypes;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
    }

    public ContentsMacroHotKeyItem Initialized(
        int index,
        Func<int, List<RawKey>, bool> onKeyChanged,
        Func<int, BitMexCommandType, bool> onCommandChangedChanged, 
        Func<int, List<RawKey>, BitMexCommandType, bool> onResisterCommand,
        Dictionary<BitMexCommandType, IBitMexCommand> commands,
        Macro macro)
    {
        this.commandTypes = new List<BitMexCommandType>();

        this.index = index;

        this.onKeyChanged = onKeyChanged;
        this.onResisterCommand = onResisterCommand;
        this.onCommandChangedChanged = onCommandChangedChanged;
        this.inputHotkey.OnKeyChanged = OnKeyChanged;

        this.dropdown.options.Clear();

        foreach (var command in commands)
        {
            this.dropdown.options.Add(new Dropdown.OptionData(command.Value.GetCommandText()));
            this.commandTypes.Add(command.Key);

            if (macro != null && command.Value.CommandType == macro.Command.CommandType)
            {
                this.dropdown.value = this.dropdown.options.Count - 1;
                this.inputHotkey.CombinationKey.AddRange(macro.Keys);
                this.inputHotkey.RefreshCombinationString();
            }
        }

        this.dropdown.onValueChanged.AddListener(OnValueChanged);

        return this;
    }

    private bool OnKeyChanged(List<RawKey> keys)
    {
        return onKeyChanged(this.index, keys);
    }

    private void OnValueChanged(int index)
    {
        var commandType = this.commandTypes[index];

        if (this.onCommandChangedChanged(this.index, commandType) == false)
        {

        }

        Debug.Log(commandType);

        //if (onCompleteCombinationMacro(this.index, this.inputHotkey.CombinationKey, commandType) == false)
        //{
        //    this.inputHotkey.ResetCombinationKey();
        //    this.dropdown.value = 0;
        //}
    }
}
