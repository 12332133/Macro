using Assets.BitMex;
using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroCoinItem : MonoBehaviour
{
    [SerializeField] private Button btnCoin;
    [SerializeField] private Text txtCoin;

    private BitMexCoin coin;

    private void Awake()
    {
        this.btnCoin = transform.Find("Button").GetComponent<Button>();
        this.txtCoin = transform.Find("Button/Text").GetComponent<Text>();
    }

    private void Reset()
    {
    }

    public ContentsMacroCoinItem Initialized(BitMexCoin coin)
    {
        this.coin = coin;

        this.txtCoin.text = this.coin.CoinName;

        return this;
    }
}
