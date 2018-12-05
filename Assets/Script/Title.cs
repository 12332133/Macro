using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Assets.BitMex;
using System.IO;
using System;

public class Title : MonoBehaviour
{
    [SerializeField] private Text txtID;
    [SerializeField] private InputField inputID;

    [SerializeField] private Text txtPW;
    [SerializeField] private InputField inputPW;

    [SerializeField] private Text txtCode;
    [SerializeField] private InputField inputCode;

    [SerializeField] private Text txtRemember;
    [SerializeField] private Toggle toggleRemember;

    [SerializeField] private Button btnLogin;
    [SerializeField] private Text txtLogin;

    [SerializeField] private Main main;
                                                     
    private string ShareBitDomain = "http://110.13.14.108:8185/ext/user";

    private void Reset()
    {
        this.txtID = transform.Find("Canvas/ID/Text").GetComponent<Text>();
        this.inputID = transform.Find("Canvas/ID/InputField").GetComponent<InputField>();

        this.txtPW = transform.Find("Canvas/PW/Text").GetComponent<Text>();
        this.inputPW = transform.Find("Canvas/PW/InputField").GetComponent<InputField>();

        this.txtCode = transform.Find("Canvas/Code/Text").GetComponent<Text>();
        this.inputCode = transform.Find("Canvas/Code/InputField").GetComponent<InputField>();

        this.txtRemember = transform.Find("Canvas/toggleRemember/Label").GetComponent<Text>();
        this.toggleRemember = transform.Find("Canvas/toggleRemember").GetComponent<Toggle>();

        this.btnLogin = transform.Find("Canvas/btnLogin").GetComponent<Button>();
        this.txtLogin = transform.Find("Canvas/btnLogin/Text").GetComponent<Text>();

        this.main = GameObject.Find("Main").GetComponent<Main>();
    }

    private void Awake()
    {
        SetUnityOptions();

        this.txtID.text = "아이디";
        this.txtPW.text = "패스워드";
        this.txtCode.text = "코드";
        this.txtRemember.text = "기억";
        this.txtLogin.text = "로그인";

        this.toggleRemember.onValueChanged.AddListener(OnToggleRemember);
        this.btnLogin.onClick.AddListener(OnClickLogin);

        toggleRemember.isOn = PlayerPrefs.GetInt("_toggleRemember", 0) == 1 ? true : false;

        if (toggleRemember.isOn)
        {
            this.inputID.text = PlayerPrefs.GetString("_inputID", string.Empty);
            this.inputPW.text = PlayerPrefs.GetString("_inputPW", string.Empty);
        }
    }

    private void SetUnityOptions()
    {
        Application.runInBackground = true;
    }

    private void OnToggleRemember(bool state)
    {
        Debug.Log("기억 : " + state);

        toggleRemember.isOn = state;

        PlayerPrefs.SetInt("_toggleRemember", state ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnClickLogin()
    {
        Debug.Log("아이디 : " + this.inputID.text + "\t패스워드 : " + this.inputPW.text);

        //var resp = Login("test", "test123!", ShareBitDomain);
        //{ "data":{ "user":{ "apiKey":"s33nLZ6j7qYFFtxPYDt0NF41","resultCode":1,"apiSecret":"5511ZeUQSZtzH4My3E76jYtXVPuNTqh8Lejg6qiGVMaNNLja","id":2,"email":"kissingsky@naver.com"} },"success":true}
        // if (success == true)

        //real
        //var session = new BitMexSession()
        //{
        //    ApiKey = "TE3O0NLo8pmwAkzsv66UamVr",
        //    ApiSecret = "yVjWPBWEVmwWZ39bRJ23aLJu5h69Eq4cyQHM6utd-O7Z8qZx",
        //    Nickname = "condemonkey@gmail.com",
        //};

        //test
        var session = new BitMexSession()
        {
            ApiKey = "SYHilkT0Lmp4I4eHBV4woHe9",
            ApiSecret = "xbj8CNU-uHJl2Ff2W5TPWkA4MWgAglIXJRbnxAQGpAgIBya1",
        };

        OnSuccessLogin(session);
    }

    public string Login(string id, string pass, string url)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url + "?id=" + id + "&pw=" + pass);
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;
            request.Proxy = null;
            request.CookieContainer = null;
            request.Timeout = 1000 * 60 * 10;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return string.Empty;
                }

                return reader.ReadToEnd();
            }
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private void OnSuccessLogin(BitMexSession session)
    {
        if (toggleRemember.isOn)
        {
            PlayerPrefs.SetString("_inputID", this.inputID.text);
            PlayerPrefs.SetString("_inputPW", this.inputPW.text);
            PlayerPrefs.Save();
        }

        main.Show(session);
        gameObject.SetActive(false);
    }
}
