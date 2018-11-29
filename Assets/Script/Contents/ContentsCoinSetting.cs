using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using Assets.BitMex;

public class ContentsCoinSetting : ContentsBase
{
    [SerializeField] private ScrollRect svCoins;
    [SerializeField] private GameObject goCoinItem;

    [SerializeField] private Text txtFixCoin;
    [SerializeField] private InputField inputFixCoin;
    [SerializeField] private Text txtFastCoin;
    [SerializeField] private InputField inputFastCoin;

    [SerializeField] private Button btnSave;
    [SerializeField] private Text txtSave;

    private List<ContentsMacroCoinItem> listCoins = new List<ContentsMacroCoinItem>();
    private BitMexCoin curCoin;

    private void Reset()
    {
        this.svCoins = transform.Find("Panel/svCoins").GetComponent<ScrollRect>();
        this.goCoinItem = Resources.Load<GameObject>("MacroCoinItem");

        this.txtFixCoin = transform.Find("Panel/Setting/FixCoin/Text").GetComponent<Text>();
        this.inputFixCoin = transform.Find("Panel/Setting/FixCoin/InputField").GetComponent<InputField>();
        this.txtFastCoin = transform.Find("Panel/Setting/FastCoin/Text").GetComponent<Text>();
        this.inputFastCoin = transform.Find("Panel/Setting/FastCoin/InputField").GetComponent<InputField>();

        this.btnSave = transform.Find("Panel/Setting/btnSaveCoin").GetComponent<Button>();
        this.txtSave = transform.Find("Panel/Setting/btnSaveCoin/Text").GetComponent<Text>();
    }

    private void Awake()
    {
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        foreach (var coin in this.bitmexMain.CoinTable.Coins.Values)
        {
            var go = Instantiate(this.goCoinItem);
            this.listCoins.Add(go.GetComponent<ContentsMacroCoinItem>().Initialized(coin, OnClickCoinSettingItem));
            go.transform.SetParent(this.svCoins.content.transform);
        }

        btnSave.onClick.AddListener(OnClickCoinSettingSave);
    }

    public void OnClickCoinSettingItem(BitMexCoin coin)
    {
        curCoin = coin;
        Debug.Log("OnClickCoinSettingItem - " + curCoin.CoinName);
        this.inputFixCoin.text = coin.FixedAvailableXbt.ToString();
        this.inputFastCoin.text = coin.SpecifiedAditional.ToString();
    }

    private void OnClickCoinSettingSave()
    {
        this.bitmexMain.CoinTable.SaveLocalCache();
        Debug.Log("OnClickCoinSettingSave - " + curCoin.CoinName);
    }
}
