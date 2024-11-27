using System;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class Register : MonoBehaviour
{
    [SerializeField] Http http;

    [Header("Register")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailAddInput;
    public TMP_InputField passwordInput;
    public TMP_InputField rewritePasswordInput;
    public Button register;

    [Header("Text Message")]
    public TextMeshProUGUI notificationResult;

    private void Start()
    {
        register.onClick.AddListener(RegisterUser);

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

        if (passwordInput.text != rewritePasswordInput.text)
        {
            notificationResult.text = "Passwords do not match!";
            StartCoroutine(ClearNotificationAfterDelay(2));
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

            StartCoroutine(http.UnityPostRequest("http://localhost:3000/register", userJson, (success, response) =>
            {
                if (success)
                {
                    if (response.Contains("User with the same email or username already exists"))
                    {
                        notificationResult.text = "Username or email already exists!";
                    }
                    else if (response.Contains("Registration Successful"))
                    {
                        notificationResult.text = "Registration Successful!";
                        Debug.Log("User registered: " + userJson);
                    }
                    else
                    {
                        notificationResult.text = "Unknown response from server.";
                    }
                }
                else
                {
                    notificationResult.text = "Username or email already exists!";
                }
            }));
        }
        else
        {
            notificationResult.text = "Passwords do not match!";
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

