using UnityEngine;
using UnityEngine.UI;

public class ContentsMacroLogItem : MonoBehaviour
{
    [SerializeField] private Text txtLog;

    private void Reset()
    {
        this.txtLog = transform.Find("Text").GetComponent<Text>();
    }

    public void Initialized()
    {
        this.txtLog.text = string.Empty;
    }

    public void SetLogText(string log)
    {
        this.txtLog.text = log;
    }
}
