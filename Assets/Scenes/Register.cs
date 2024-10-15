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
    //private Dictionary<string, int> UserInfo = new Dictionary<string, int>();

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
        register.onClick.AddListener(RegisterUser);
        login.onClick.AddListener(LoginUser);
    }

    public void RegisterUser()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || 
            string.IsNullOrEmpty(passwordInput.text) || 
            string.IsNullOrEmpty(rewritePasswordInput.text))
        {
            resultRegistration.text = "Missing fields!";
            return;
        }

        if (passwordInput.text == rewritePasswordInput.text)
        {
            // Create a UserAccount object
            var newUser = new UserAccount()
            {
                username = usernameInput.text,
                password = passwordInput.text
            };

            Debug.Log(JsonConvert.SerializeObject(newUser));

            string userJson = JsonConvert.SerializeObject(newUser);
            PlayerPrefs.SetString("UserAccount", userJson);
            PlayerPrefs.Save();

            Debug.Log("User registered: " + userJson);
            resultRegistration.text = "Registration Successful!";
        }
        else
        {
            resultRegistration.text = "Passwords do not match!";
        }
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(usernameLogin.text) ||
            string.IsNullOrEmpty(passwordLogin.text))
        {
            resultLogin.text = "Missing fields!";
            return;
        }

        if (PlayerPrefs.HasKey("UserAccount"))
        {
            string storedUserJson = PlayerPrefs.GetString("UserAccount");
            UserAccount storedUser = JsonConvert.DeserializeObject<UserAccount>(storedUserJson);

            if (storedUser.username == usernameLogin.text && storedUser.password == passwordLogin.text)
            {
                Debug.Log("Login successful!");
                resultLogin.text = "Login Successful!";
            }
            else
            {
                Debug.Log("Invalid username or password.");
                resultLogin.text = "Invalid Username or Password.";
            }
        }
        else
        {
            Debug.Log("No user registered.");
            resultLogin.text = "No registered user found.";
        }
    }
}

[Serializable]
public struct UserAccount
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public string username;
    public string password;
}

