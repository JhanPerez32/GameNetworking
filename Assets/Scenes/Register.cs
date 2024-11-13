using System;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class Register : MonoBehaviour
{
    [SerializeField] GameObject closeRegLogin;
    [SerializeField] GameObject profileSection;
    [SerializeField] Http http;

    [Header("Register")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailAddInput;
    public TMP_InputField passwordInput;
    public TMP_InputField rewritePasswordInput;
    public Button register;

    [Header("Login")]
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;
    public Button login;

    [Header("Text Message")]
    public TextMeshProUGUI notificationResult;

    private void Start()
    {
        register.onClick.AddListener(RegisterUser);
        login.onClick.AddListener(LoginUser);

        notificationResult.text = "";
    }

    public void RegisterUser()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || 
            string.IsNullOrEmpty(emailAddInput.text) ||
            string.IsNullOrEmpty(passwordInput.text) || 
            string.IsNullOrEmpty(rewritePasswordInput.text))
        {
            notificationResult.text = "Missing fields!";
            return;
        }

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(emailAddInput.text, emailPattern))
        {
            notificationResult.text = "Email is not recognized!";
            return;
        }

        if (passwordInput.text == rewritePasswordInput.text)
        {
            var newUser = new UserAccount()
            {
                name = usernameInput.text,
                email = emailAddInput.text,
                password = passwordInput.text
            };

            Debug.Log(JsonConvert.SerializeObject(newUser));

            string userJson = JsonConvert.SerializeObject(newUser);
            PlayerPrefs.SetString("UserAccount", userJson);
            PlayerPrefs.Save();

            StartCoroutine(http.UnityPostRequest("http://localhost:3000/users", userJson, (success, response) =>
            {
                if (success)
                {
                    notificationResult.text = "Registration Successful!";
                }
                else
                {
                    notificationResult.text = "Failed to register on the server.";
                }
            }));

            Debug.Log("User registered: " + userJson);
            notificationResult.text = "Registration Successful!";
        }
        else
        {
            notificationResult.text = "Passwords do not match!";
        }
        StartCoroutine(ClearNotificationAfterDelay(2));
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(emailAddInput.text) ||
            string.IsNullOrEmpty(passwordLogin.text))
        {
            notificationResult.text = "Missing fields!";
            return;
        }

        if (PlayerPrefs.HasKey("UserAccount"))
        {
            string storedUserJson = PlayerPrefs.GetString("UserAccount");
            UserAccount storedUser = JsonConvert.DeserializeObject<UserAccount>(storedUserJson);

            if (storedUser.email == emailAddInput.text && storedUser.password == passwordLogin.text)
            {
                notificationResult.text = "Login Successful!";

                closeRegLogin.SetActive(false);
                profileSection.SetActive(true);

                http.LoggedIn();
            }
            else
            {
                notificationResult.text = "Invalid Username or Password.";
            }
        }
        else
        {
            notificationResult.text = "No registered user found.";
        }
        StartCoroutine(ClearNotificationAfterDelay(2));
    }

    private IEnumerator ClearNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationResult.text = "";
    }
}


[Serializable]
public struct UserAccount
{
    public string name;
    public string email;
    public string password;
}

