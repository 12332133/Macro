using Assets.BitMex;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentsBase : MonoBehaviour
{
    public class ContentsPopupInput
    {
        public GameObject Root;
        public Button btnPopupBack;
        public InputField inputPopup;
        public Button btnAdd;
        public Button btnEdit;
        public Button btnDel;

        public Action<string> add;
        public Action<string> edit;
        public Action<string> del;

        public ContentsPopupInput(Transform root)
        {
            this.Root = root.gameObject;
            this.btnPopupBack = root.Find("BackPanel").GetComponent<Button>();
            this.inputPopup = root.Find("InputField").GetComponent<InputField>();
            this.btnAdd = root.Find("btnAdd").GetComponent<Button>();
            this.btnEdit = root.Find("btnEdit").GetComponent<Button>();
            this.btnDel = root.Find("btnDel").GetComponent<Button>();

            this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
            this.btnAdd.onClick.AddListener(OnClickAdd);
            this.btnEdit.onClick.AddListener(OnClickEdit);
            this.btnDel.onClick.AddListener(OnClickDel);
        }

        public void OnEnablePopup(string original, Action<string> add, Action<string> edit, Action<string> del)
        {
            this.add = add;
            this.edit = edit;
            this.del = del;

            this.inputPopup.text = original;
            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
            this.Root.SetActive(false);
        }

        private void OnClickAdd()
        {
            this.add(this.inputPopup.text);
            this.Root.SetActive(false);
        }

        private void OnClickEdit()
        {
            this.edit(this.inputPopup.text);
            this.Root.SetActive(false);
        }

        private void OnClickDel()
        {
            this.del(this.inputPopup.text);
            this.Root.SetActive(false);
        }
    }

    public class ContentsPopupDropdown
    {
        public GameObject Root;
        public Button btnPopupBack;
        public Dropdown dropPopup;
        public Button btnAdd;
        public Button btnEdit;
        public Button btnDel;

        public Action<string> add;
        public Action<string> edit;
        public Action<string> del;

        public ContentsPopupDropdown(Transform root, BitMexCoinTable coinTable)
        {
            this.Root = root.gameObject;
            this.btnPopupBack = root.Find("BackPanel").GetComponent<Button>();
            this.dropPopup = root.Find("Dropdown").GetComponent<Dropdown>();
            this.btnAdd = root.Find("btnAdd").GetComponent<Button>();
            this.btnEdit = root.Find("btnEdit").GetComponent<Button>();
            this.btnDel = root.Find("btnDel").GetComponent<Button>();

            this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
            this.btnAdd.onClick.AddListener(OnClickAdd);
            this.btnEdit.onClick.AddListener(OnClickEdit);
            this.btnDel.onClick.AddListener(OnClickDel);

            foreach (var coin in coinTable.Coins)
            {
                this.dropPopup.options.Add(new Dropdown.OptionData(coin.Value.CoinName));
            }
        }

        public void OnEnablePopup(string coinName, Action<string> add, Action<string> edit, Action<string> del)
        {
            this.add = add;
            this.edit = edit;
            this.del = del;

            this.dropPopup.value = 0;
            this.dropPopup.captionText.text = string.Empty;

            for (int i = 0; i < this.dropPopup.options.Count; i++)
            {
                if (this.dropPopup.options[i].text.Equals(coinName) == true)
                {
                    this.dropPopup.value = i;
                    this.dropPopup.captionText.text = this.dropPopup.options[i].text;
                }
            }

            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
            this.Root.SetActive(false);
        }

        private void OnClickAdd()
        {
            this.add(this.dropPopup.options[this.dropPopup.value].text);
            this.Root.SetActive(false);
        }

        private void OnClickEdit()
        {
            this.edit(this.dropPopup.options[this.dropPopup.value].text);
            this.Root.SetActive(false);
        }

        private void OnClickDel()
        {
            this.del(this.dropPopup.options[this.dropPopup.value].text);
            this.Root.SetActive(false);
        }
    }

    public class ContentsPopupMessage
    {
        public GameObject Root;
        public Button btnPopupBack;
        public Text txtPopup;
        public Button btnPopup;
        public Action<string> complete;

        public ContentsPopupMessage(Transform root)
        {
            this.Root = root.gameObject;
            this.btnPopupBack = root.Find("BackPanel").GetComponent<Button>();
            this.txtPopup = root.Find("Text").GetComponent<Text>();
            this.btnPopup = root.Find("Button").GetComponent<Button>();

            this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
            this.btnPopup.onClick.AddListener(OnClickPopupOK);
        }

        public void OnEnablePopup(string original, Action<string> complete)
        {
            this.complete = complete;
            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
            this.Root.SetActive(false);
        }

        private void OnClickPopupOK()
        {
            this.Root.SetActive(false);
        }
    }

    protected IBitMexMainAdapter bitmexMain;

    public virtual void Initialize(IBitMexMainAdapter bitmexMain)
    {
        this.bitmexMain = bitmexMain;
    }
}
