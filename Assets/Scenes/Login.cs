using System;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Login : MonoBehaviour
{
    [SerializeField] GameObject closeRegLogin;
    [SerializeField] GameObject profileSection;
    [SerializeField] Http http;

    [Header("Login")]
    public TMP_InputField emailAddInput;
    public TMP_InputField passwordLogin;
    public Button login;

    [Header("Text Message")]
    public TextMeshProUGUI notificationResult;

    private void Start()
    {
        login.onClick.AddListener(LoginUser);

        notificationResult.text = "";
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(emailAddInput.text) ||
            string.IsNullOrEmpty(passwordLogin.text))
        {
            notificationResult.text = "Missing fields!";
            return;
        }

        string url = $"http://localhost:3000/users/email/{UnityWebRequest.EscapeURL(emailAddInput.text)}";

        StartCoroutine(http.UnityGetRequest(url, (success, result) =>
        {
            if (success)
            {
                var users = JsonConvert.DeserializeObject<List<Users>>(result);
                if (users.Count > 0)
                {
                    var user = users[0];
                    if (user.password == passwordLogin.text)
                    {
                        notificationResult.text = "Login Successful!";

                        PlayerPrefs.SetString("LoggedInEmail", user.email);
                        PlayerPrefs.SetString("UserAccount", JsonConvert.SerializeObject(user));
                        PlayerPrefs.Save();

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
                    notificationResult.text = "User not found.";
                }
            }
            else
            {
                notificationResult.text = "Failed to connect to the server.";
            }

            StartCoroutine(ClearNotificationAfterDelay(2));
        }));
    }

    private IEnumerator ClearNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationResult.text = "";
    }
}
