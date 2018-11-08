using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.KeyBoardHook;
using System.Collections.Generic;
using Assets.BitMex;

public class ContentsMacroHotKeyItem : MonoBehaviour
{
    [SerializeField] private MacroInputField inputHotkey;
    [SerializeField] private Dropdown dropdown;

    private int index;
    private Func<int, List<RawKey>, BitMexCommandType, bool> onCompleteCombinationMacro;
    private List<BitMexCommandType> commandTypes;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
    }

    public ContentsMacroHotKeyItem Initialized(int index, Func<int, List<RawKey>, BitMexCommandType, bool> onCompleteCombinationMacro, Dictionary<BitMexCommandType, IBitMexActionCommand> commands)
    {
        this.commandTypes = new List<BitMexCommandType>();

        this.index = index;
        this.onCompleteCombinationMacro = onCompleteCombinationMacro;
        this.dropdown.options.Clear();

        foreach (var command in commands)
        {
            this.dropdown.options.Add(new Dropdown.OptionData(command.Value.DropBoxText));
            this.commandTypes.Add(command.Key);
        }

        this.dropdown.onValueChanged.AddListener(OnValueChanged);
        return this;
    }

    private void OnValueChanged(int index)
    {
        var commandType = this.commandTypes[index];
        
        if (onCompleteCombinationMacro(this.index, this.inputHotkey.CombinationKey, commandType) == false)
        {
            this.inputHotkey.ResetCombinationKey();
        }

        Debug.Log(commandType);
    }
}
