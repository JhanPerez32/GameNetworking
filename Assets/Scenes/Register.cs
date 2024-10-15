using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json.Converters;
using UnityEngine.TextCore.Text;

public class Register : MonoBehaviour
{
    private Dictionary<string, int> UserInfo = new Dictionary<string, int>();

    [Header("Register")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField rewritePasswordInput;
    public Button register;

    public TextMeshProUGUI resultRegistration;

    [Header("Login")]
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;
    public Button login;

    public TextMeshProUGUI resultLogin;

    private void Start()
    {
        
    }


    //Access by the register button
    public void RegisterUser()
    {
        var character = new UserAccount()
        {
            username = usernameInput.text,
            password = passwordInput.text,
            rewritePassword = rewritePasswordInput.text,
        };

        Debug.Log(JsonConvert.SerializeObject(character));

        if (passwordInput.text == rewritePasswordInput.text)
        {
            Debug.Log("Register");
            resultRegistration.text = "Registered";
        }
        else
        {
            resultRegistration.text = "Password not Match";
        }


    }
}

[Serializable]
public struct UserAccount
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public string username;
    public string password;
    public string rewritePassword;
}

