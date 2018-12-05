using Assets.BitMex;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroCoinItem : MonoBehaviour
{
    [SerializeField] private Text txtCoin;

    [SerializeField] private Text txtSetAmount;
    [SerializeField] private InputField inputSetAmount;

    [SerializeField] private Text txtFixAmount;
    [SerializeField] private InputField inputFixAmount;

    private BitMexCoin coin;

    private void Awake()
    {
    }

    private void Reset()
    {
        this.txtCoin = transform.Find("CoinName/Text").GetComponent<Text>();

        this.txtSetAmount = transform.Find("SetAmount/Text").GetComponent<Text>();
        this.inputSetAmount = transform.Find("SetAmount/InputField").GetComponent<InputField>();

        this.txtFixAmount = transform.Find("FixAmount/Text").GetComponent<Text>();
        this.inputFixAmount = transform.Find("FixAmount/InputField").GetComponent<InputField>();
    }

    public ContentsMacroCoinItem Initialized(BitMexCoin coin, Action<BitMexCoin> callBack)
    {
        this.coin = coin;

        this.txtCoin.text = this.coin.CoinName;
        this.inputSetAmount.text = this.coin.SpecifiedAditional.ToString();

        return this;
    }
}
