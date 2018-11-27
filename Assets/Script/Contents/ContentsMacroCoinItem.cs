using Assets.BitMex;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroCoinItem : MonoBehaviour
{
    [SerializeField] private Button btnCoin;
    [SerializeField] private Text txtCoin;

    private BitMexCoin coin;

    private void Awake()
    {
    }

    private void Reset()
    {
        this.btnCoin = transform.Find("Button").GetComponent<Button>();
        this.txtCoin = transform.Find("Button/Text").GetComponent<Text>();
    }

    //public ContentsMacroCoinItem Initialized(BitMexCoin coin, UnityEngine.Events.UnityAction callBack)
    public ContentsMacroCoinItem Initialized(BitMexCoin coin, Action<BitMexCoin> callBack)
    {
        this.coin = coin;

        this.txtCoin.text = this.coin.CoinName;
        this.btnCoin.onClick.AddListener(() => { callBack(this.coin); });

        return this;
    }
}
