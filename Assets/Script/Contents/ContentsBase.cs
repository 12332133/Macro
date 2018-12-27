using Assets.BitMex;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentsBase : MonoBehaviour
{
    public class ModifyCommandPercentPopup<T>
    {
        public GameObject Root;
        public Button btnPopupBack;
        public InputField inputPopup;
        public Button btnAdd;
        public Button btnEdit;
        public Button btnDel;

        private Action<T, int> add;
        private Action<T, int> modify;
        private Action<T, int> remove;
        private T obj;

        public ModifyCommandPercentPopup(Transform root)
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

        public void OnEnablePopup(T obj, string input, Action<T, int> add, Action<T, int> modify, Action<T, int> remove)
        {
            this.obj = obj;
            this.add = add;
            this.modify = modify;
            this.remove = remove;

            this.inputPopup.text = input;
            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
        }

        private void OnClickAdd()
        {
            this.add(this.obj, Int32.Parse(this.inputPopup.text));
            this.Root.SetActive(false);
        }

        private void OnClickEdit()
        {
            this.modify(this.obj, Int32.Parse(this.inputPopup.text));
            this.Root.SetActive(false);
        }

        private void OnClickDel()
        {
            this.remove(this.obj, Int32.Parse(this.inputPopup.text));
            this.Root.SetActive(false);
        }
    }

    public class ModifyCommandCoinTypePopup<T>
    {
        public GameObject Root;
        public Button btnPopupBack;
        public Dropdown dropPopup;
        public Button btnAdd;
        public Button btnEdit;
        public Button btnDel;

        private Action<T, string> add;
        private Action<T, string> modify;
        private Action<T, string> remove;
        private T obj;

        public ModifyCommandCoinTypePopup(Transform root, List<string> coins)
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

            foreach (var coin in coins)
            {
                this.dropPopup.options.Add(new Dropdown.OptionData(coin));
            }
        }

        public void OnEnablePopup(T obj, string input, Action<T, string> add, Action<T, string> modify, Action<T, string> remove)
        {
            this.obj = obj;
            this.add = add;
            this.modify = modify;
            this.remove = remove;

            this.dropPopup.value = 0;
            this.dropPopup.captionText.text = string.Empty;

            for (int i = 0; i < this.dropPopup.options.Count; i++)
            {
                if (this.dropPopup.options[i].text.Equals(input) == true)
                {
                    this.dropPopup.value = i;
                    this.dropPopup.captionText.text = this.dropPopup.options[i].text;
                }
            }

            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
        }

        private void OnClickAdd()
        {
            this.add(this.obj, this.dropPopup.options[this.dropPopup.value].text);
            this.Root.SetActive(false);
        }

        private void OnClickEdit()
        {
            this.modify(this.obj, this.dropPopup.options[this.dropPopup.value].text);
            this.Root.SetActive(false);
        }

        private void OnClickDel()
        {
            this.remove(this.obj, this.dropPopup.options[this.dropPopup.value].text);
            this.Root.SetActive(false);
        }
    }

    public class ContentsPopupMessage
    {
        public GameObject Root;
        public Button btnPopupBack;
        public Text txtPopup;
        public Button btnPopup;
        private bool isAppClose;

        public ContentsPopupMessage(Transform root)
        {
            this.Root = root.gameObject;
            this.btnPopupBack = root.Find("BackPanel").GetComponent<Button>();
            this.txtPopup = root.Find("Text").GetComponent<Text>();
            this.btnPopup = root.Find("Button").GetComponent<Button>();

            this.btnPopupBack.onClick.AddListener(OnClickPopupBack);
            this.btnPopup.onClick.AddListener(OnClickPopupOK);
        }

        public void OnEnablePopup(string original, bool isAppClose = false)
        {
            txtPopup.text = original;
            this.isAppClose = isAppClose;
            this.Root.SetActive(true);
        }

        private void OnClickPopupBack()
        {
        }

        private void OnClickPopupOK()
        {
            this.Root.SetActive(false);

            if (this.isAppClose == true)
            {
                Application.Quit();
            }
        }
    }

    protected IBitMexMainAdapter bitmexMain;

    public virtual void Initialize(IBitMexMainAdapter bitmexMain)
    {
        this.bitmexMain = bitmexMain;
    }

    public virtual void Save()
    {
    }
}
