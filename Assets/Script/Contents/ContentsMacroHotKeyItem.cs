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
    private Func<int, List<RawKey>, BitMexCommandType, bool> completeCombinationMacro;
    private List<BitMexCommandType> commandTypes;

    private void Reset()
    {
        this.inputHotkey = transform.Find("InputField").GetComponent<MacroInputField>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
    }

    public ContentsMacroHotKeyItem Initialized(int index, Func<int, List<RawKey>, BitMexCommandType, bool> completeCombinationMacro, Dictionary<BitMexCommandType, IBitMexActionCommand> commands)
    {
        this.commandTypes = new List<BitMexCommandType>();

        this.index = index;
        this.completeCombinationMacro = completeCombinationMacro;

        this.dropdown.options.Clear();
        foreach (var command in commands)
        {
            this.dropdown.options.Add(new Dropdown.OptionData(command.Value.DropBoxText));
            this.commandTypes.Add(command.Key);
        }

        //this.dropdown.options.Add(new Dropdown.OptionData(""));
        //this.dropdown.options.Add(new Dropdown.OptionData("사용가능 xbt 고정"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 설정"));
        //this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 100% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 50% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 25% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 10% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 100% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 50% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 25% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("시장가 10% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 100% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 50% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 25% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 10% 매수"));
        //this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 100% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 50% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 25% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 10% 매도"));
        //this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        //this.dropdown.options.Add(new Dropdown.OptionData("최상위 포지션 취소"));
        //this.dropdown.options.Add(new Dropdown.OptionData("최상위 주문 취소"));
        //this.dropdown.options.Add(new Dropdown.OptionData("전체 주문 취소"));

        this.dropdown.onValueChanged.AddListener(OnValueChanged);

        return this;
    }

    private void OnValueChanged(int index)
    {
        var commandType = this.commandTypes[index];

        Debug.Log(commandType);

        switch (commandType)
        {
            case BitMexCommandType.FixedAvailableXbt:
            case BitMexCommandType.SpecifiedAditional:
                completeCombinationMacro(this.index, this.inputHotkey.CombinationKey, commandType);
                this.inputHotkey.ResetCombinationKey();
                break;
            default:
                completeCombinationMacro(this.index, this.inputHotkey.CombinationKey, commandType);
                break;
        }
    }
}
