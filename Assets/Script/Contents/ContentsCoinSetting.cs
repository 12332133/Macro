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


    private List<ContentsMacroCoinItem> listCoins = new List<ContentsMacroCoinItem>();

    private void Reset()
    {
        this.svCoins = transform.Find("Panel/svCoins").GetComponent<ScrollRect>();
        this.goCoinItem = Resources.Load<GameObject>("MacroCoinItem");
        
        this.txtFixCoin = transform.Find("Panel/Setting/FixCoin/Text").GetComponent<Text>();
        this.inputFixCoin = transform.Find("Panel/Setting/FixCoin/InputField").GetComponent<InputField>();
        this.txtFastCoin = transform.Find("Panel/Setting/FastCoin/Text").GetComponent<Text>();
        this.inputFastCoin = transform.Find("Panel/Setting/FastCoin/InputField").GetComponent<InputField>();
    }

    private void Awake()
    {
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);

        using (var e = bitmexMain.CoinTable.Coins.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var go = Instantiate(this.goCoinItem);
                this.listCoins.Add(go.GetComponent<ContentsMacroCoinItem>().Initialized(e.Current.Value));
                go.transform.SetParent(this.svCoins.content.transform);
            }
        }
    }
}
