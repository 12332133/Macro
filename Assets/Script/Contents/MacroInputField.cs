using Assets.KeyBoardHook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MacroInputField : InputField
{
    private const string SelectString = "input macro key...";

    private bool isCombination;
    private List<RawKey> inputRawKeys;
    private List<RawKey> combinationRawKeys;
    private Dictionary<RawKey, int> allowSpecialKeys;

    public List<RawKey> CombinationKey
    {
        get
        {
            return this.combinationRawKeys;
        }
    }

    protected override void Awake()
    {
        this.inputRawKeys = new List<RawKey>();
        this.combinationRawKeys = new List<RawKey>();

        this.allowSpecialKeys = new Dictionary<RawKey, int>();
        this.allowSpecialKeys.Add(RawKey.LeftShift, 1);
        this.allowSpecialKeys.Add(RawKey.Shift, 1);
        this.allowSpecialKeys.Add(RawKey.RightShift, 1);
        this.allowSpecialKeys.Add(RawKey.Control, 1);
        this.allowSpecialKeys.Add(RawKey.LeftControl, 1);
        this.allowSpecialKeys.Add(RawKey.RightControl, 1);
        this.allowSpecialKeys.Add(RawKey.Menu, 1);
        this.allowSpecialKeys.Add(RawKey.LeftMenu, 1);
        this.allowSpecialKeys.Add(RawKey.RightMenu, 1);

        this.readOnly = true;
        this.isCombination = false;

        this.text = GetCombinationKeyName(this.combinationRawKeys);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        if (KeyboardHooker.IsRunning() == false && KeyboardHooker.Start() == false)
        {
            Debug.Log("hooker resister failed");
            return;
        }

        this.text = GetCombinationKeyName(null);

        KeyboardHooker.OnKeyUp += OnKeyUp;
        KeyboardHooker.OnKeyDown += OnKeyDown;

        Debug.Log("hooker start");
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        if (KeyboardHooker.IsRunning() == true)
        {
            KeyboardHooker.OnKeyUp -= OnKeyUp;
            KeyboardHooker.OnKeyDown -= OnKeyDown;
        }

        this.text = GetCombinationKeyName(this.combinationRawKeys);

        Debug.Log("hooker end");
    }

    private void OnKeyDown(RawKey key)
    {
        if (this.isCombination == true)
        {
            this.inputRawKeys.Add(key);
            return;
        }

        if (this.allowSpecialKeys.ContainsKey(key) == true)
        {
            this.inputRawKeys.Add(key);
            this.isCombination = true;
        }
    }

    private void OnKeyUp(RawKey key)
    {
        if (this.isCombination == false)
        {
            return;
        }

        if (this.inputRawKeys.Count <= 1)
        {
            this.inputRawKeys.Clear();
            this.isCombination = false;
            return;
        }

        this.combinationRawKeys.Clear();
        this.combinationRawKeys.AddRange(this.inputRawKeys);
        this.text = GetCombinationKeyName(this.combinationRawKeys);

        this.inputRawKeys.Clear();
        this.isCombination = false;
    }

    private string GetCombinationKeyName(List<RawKey> rawKeys)
    {
        if (rawKeys == null || rawKeys.Count <= 1)
        {
            return SelectString;
        }

        var builder = new StringBuilder();

        for (int i = 0; i < rawKeys.Count; i++)
        {
            builder.Append(rawKeys[i].ToString());

            if (i < rawKeys.Count - 1)
            {
                builder.Append(" + ");
            }
        }

        return builder.ToString();
    }

    public void ResetCombinationKey()
    {
        this.combinationRawKeys.Clear();
        this.text = GetCombinationKeyName(this.combinationRawKeys);
    }
}
