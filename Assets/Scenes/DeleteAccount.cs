using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DeleteAccount : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button deleteButton;
    [SerializeField] private TextMeshProUGUI notificationResult;

    private Http httpScript;

    private void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteAccount);
        notificationResult.text = " ";
        httpScript = FindObjectOfType<Http>();
    }

    /*public void SetReferences(TMP_InputField emailField, TMP_InputField passwordField, Button deleteBtn, TextMeshProUGUI notifMessage)
    {
        emailInputField = emailField;
        passwordInputField = passwordField;
        deleteButton = deleteBtn;
        notificationResult = notifMessage;
    }*/

    public void OnDeleteAccount()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            notificationResult.text = "Missing Fields";
            StartCoroutine(ClearNotificationAfterDelay(2));
            return;
        }

        // Validate email and password
        StartCoroutine(ValidateAndDeleteAccount(email, password));
    }

    private IEnumerator ValidateAndDeleteAccount(string email, string password)
    {
        string url = $"http://localhost:3000/users/email/{UnityWebRequest.EscapeURL(email)}";
        bool isValid = false;
        int userId = -1;

        // Fetch user details
        yield return StartCoroutine(httpScript.UnityGetRequest(url, (success, result) =>
        {
            if (success)
            {
                var users = JsonConvert.DeserializeObject<List<Users>>(result);
                if (users.Count > 0)
                {
                    // Check password
                    if (users[0].email == email && users[0].password == password)
                    {
                        isValid = true;
                        userId = users[0].id;
                    }
                    else
                    {
                        notificationResult.text = "Email or Password is incorrect.";
                        StartCoroutine(ClearNotificationAfterDelay(2));
                    }
                }
                else
                {
                    Debug.LogError("No user found with the provided email.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch user details.");
            }
        }));

        if (isValid && userId != -1)
        {
            // Delete the account
            string deleteUrl = $"http://localhost:3000/users/{userId}";
            yield return StartCoroutine(httpScript.UnityDeleteRequest(deleteUrl, (success, _) =>
            {
                if (success)
                {
                    Debug.Log("Account deleted successfully.");

                }
                else
                {
                    Debug.LogError("Failed to delete account.");
                }
            }));
        }
    }

    private IEnumerator ClearNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationResult.text = "";
    }
}
