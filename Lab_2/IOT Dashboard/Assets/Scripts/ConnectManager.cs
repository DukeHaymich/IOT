using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviour
{
    [Header("User interface")]
    public InputField textBrokerAddress;
    public InputField textUsername;
    public InputField textPassword;
    public Text textInfo;
    public Button buttonConnect;
    public Button buttonQuit;

    // Start is called before the first frame update
    void Start()
    {
        this.SetUpScene();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetUpScene()
    {
        textInfo = GameObject.Find("/Canvas/InfoText").GetComponent<Text>();
        textBrokerAddress = GameObject.Find("/Canvas/Input/BrokerAddress").GetComponent<InputField>();
        textUsername = GameObject.Find("/Canvas/Input/Username").GetComponent<InputField>();
        textPassword = GameObject.Find("/Canvas/Input/Password").GetComponent<InputField>();
        buttonConnect = GameObject.Find("/Canvas/Connect").GetComponent<Button>();
        buttonConnect.onClick.AddListener(() => this.OnConnect());
        buttonQuit = GameObject.Find("/Canvas/Quit").GetComponent<Button>();
        buttonQuit.onClick.AddListener(() => this.onQuit());
    }

    private void OnConnect()
    {
        if (textInfo != null)
        {
            if (textBrokerAddress != null && textBrokerAddress.text.Length == 0)
            {
                textInfo.text = "The broker URI field cannot be empty!";
                return;
            }
            else if (textUsername != null && textUsername.text.Length == 0)
            {
                textInfo.text = "Please fill in the username field!";
                return;
            }
        }
        DataSession.brokerAddress = textBrokerAddress.text;
        DataSession.username = textUsername.text;
        DataSession.password = textPassword.text;

        SceneManager.LoadScene("Dashboard");
    }

    private void onQuit()
    {
        Application.Quit();
    }
}
