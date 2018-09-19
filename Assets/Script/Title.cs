using UnityEngine;
using UnityEngine.UI;

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
    }

    private void Awake()
    {
        this.txtID.text = "아이디";
        this.txtPW.text = "패스워드";
        this.txtCode.text = "코드";
        this.txtRemember.text = "기억";
        this.txtLogin.text = "로그인";

        this.toggleRemember.onValueChanged.AddListener(OnToggleRemember);
        this.btnLogin.onClick.AddListener(OnClickLogin);

        int state = PlayerPrefs.GetInt("_toggleRemember", 0);

        if (state == 1)
        {
            this.inputID.text = PlayerPrefs.GetString("_inputID", string.Empty);
            this.inputPW.text = PlayerPrefs.GetString("_inputPW", string.Empty);
        }
    }

    private void OnToggleRemember(bool state)
    {
        Debug.Log("기억 : " + state);

        PlayerPrefs.SetInt("_toggleRemember", state ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnClickLogin()
    {
        Debug.Log("아이디 : " + this.inputID.text + "\t패스워드 : " + this.inputPW.text);

        OnSuccessLogin();
    }

    private void OnSuccessLogin()
    {
        PlayerPrefs.SetString("_inputID", this.inputID.text);
        PlayerPrefs.SetString("_inputPW", this.inputPW.text);
        PlayerPrefs.Save();
    }
}
