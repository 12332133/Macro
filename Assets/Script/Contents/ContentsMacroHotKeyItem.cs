using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.KeyBoardHook;
using System.Collections.Generic;

public class ContentsMacroHotKeyItem : MonoBehaviour
{
    public enum eType
    {
        Fix = 1,
        Fast,
    }

    [SerializeField] private Button btnHotKey;
    [SerializeField] private Text txtHotKey;
    [SerializeField] private Dropdown dropdown;

    private int index;
    private Action<int, eType, string> actionEnablePopup;
    private float fixValue = 0f;
    private float fastValue = 12.5f;

    private void Reset()
    {
        this.btnHotKey = transform.Find("Button").GetComponent<Button>();
        this.txtHotKey = transform.Find("Button/Text").GetComponent<Text>();
        this.dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
    }

    public ContentsMacroHotKeyItem Initialized(int index, Action<int, eType, string> callBack)
    {
        this.index = index;
        this.actionEnablePopup = callBack;

        this.btnHotKey.onClick.AddListener(OnClickHotKey);
        this.txtHotKey.text = "입력";

        this.dropdown.options.Clear();
        this.dropdown.options.Add(new Dropdown.OptionData(""));
        this.dropdown.options.Add(new Dropdown.OptionData("사용가능 xbt 고정"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 설정"));
        this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 100% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 50% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 25% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 10% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 100% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 50% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 25% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("시장가 10% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 100% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 50% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 25% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 10% 매수"));
        this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 100% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 50% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 25% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("빠른 지정가 10% 매도"));
        this.dropdown.options.Add(new Dropdown.OptionData("---------------"));
        this.dropdown.options.Add(new Dropdown.OptionData("최상위 포지션 취소"));
        this.dropdown.options.Add(new Dropdown.OptionData("최상위 주문 취소"));
        this.dropdown.options.Add(new Dropdown.OptionData("전체 주문 취소"));

        this.dropdown.onValueChanged.AddListener(OnValueChanged);

        return this;
    }

    public void SetInputValue(eType type, string value)
    {
        this.dropdown.options[(int)type].text = value;
        this.dropdown.RefreshShownValue();
    }

    private void OnClickHotKey()
    {
        Debug.Log("ContentsMacroHotKeyItem.OnClickHotKey()");

        this.txtHotKey.text = "입력";

        if (KeyboardHooker.IsRunning() == false && KeyboardHooker.Start() == false)
        {
            return;
        }

        Debug.Log("ContentsMacroHotKeyItem.KeyboardHooker resister callback");

        KeyboardHooker.OnKeyUp += OnKeyUp;
        KeyboardHooker.OnKeyDown += OnKeyDown;
    }

    private void OnKeyDown(RawKey key)
    {
        Debug.Log("ContentsMacroHotKeyItem.OnKeyDown()");

        if (this.txtHotKey.text.Equals("입력") == true)
            this.txtHotKey.text = key.ToString();
        else
            this.txtHotKey.text += "+" + key.ToString();
    }

    private void OnKeyUp(RawKey key)
    {
        if (KeyboardHooker.IsRunning() == true)
        {
            KeyboardHooker.OnKeyUp -= OnKeyUp;
            KeyboardHooker.OnKeyDown -= OnKeyDown;
            Debug.Log("ContentsMacroHotKeyItem.KeyboardHooker unresister callback");
        }
    }

    private void OnValueChanged(int index)
    {
        Debug.Log("ContentsMacroItem.OnValueChanged(" + this.index + ")");

        switch ((eType)index)
        {
            case eType.Fix  : actionEnablePopup(this.index, eType.Fix, this.fixValue.ToString()); break;
            case eType.Fast : actionEnablePopup(this.index, eType.Fast, this.fastValue.ToString()); break;
        }
    }
}
