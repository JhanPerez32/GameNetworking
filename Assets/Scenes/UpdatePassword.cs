using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdatePassword : MonoBehaviour
{
    [SerializeField] private TMP_InputField newPasswordInput;
    [SerializeField] private TMP_InputField correctEmailInput;
    [SerializeField] private Button updateButton;
    [SerializeField] private TextMeshProUGUI notificationResult;

    private Http httpScript;

    private void Start()
    {
        updateButton.onClick.AddListener(OnUpdatePassword);
        notificationResult.text = " ";
        httpScript = FindObjectOfType<Http>();
    }

    public void OnUpdatePassword()
    {
        string newPassword = newPasswordInput.text;
        string enteredEmail = correctEmailInput.text;
        string loggedInEmail = PlayerPrefs.GetString("LoggedInEmail", string.Empty); // Fetch the logged-in email

        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(enteredEmail))
        {
            notificationResult.text = "Fields cannot be empty.";
            StartCoroutine(ClearNotificationAfterDelay(2));
            return;
        }

        if (enteredEmail != loggedInEmail)
        {
            notificationResult.text = "Email does not match the logged-in user.";
            StartCoroutine(ClearNotificationAfterDelay(2));
            return;
        }

        string url = $"http://localhost:3000/users/email/{UnityWebRequest.EscapeURL(loggedInEmail)}";
        StartCoroutine(httpScript.UnityGetRequest(url, (success, result) =>
        {
            if (success)
            {
                var user = JsonConvert.DeserializeObject<List<Users>>(result);
                if (user.Count > 0)
                {
                    int userId = user[0].id;
                    string updateUrl = $"http://localhost:3000/users/{userId}";

                    var updatedData = new { password = newPassword };
                    string jsonData = JsonConvert.SerializeObject(updatedData);

                    StartCoroutine(httpScript.UnityPutRequest(updateUrl, jsonData, (updateSuccess, updateResult) =>
                    {
                        if (updateSuccess)
                        {
                            notificationResult.text = "Password updated successfully.";
                        }
                        else
                        {
                            notificationResult.text = "Failed to update the password.";
                        }

                        StartCoroutine(ClearNotificationAfterDelay(2));
                    }));
                }
                else
                {
                    notificationResult.text = "User not found.";
                    StartCoroutine(ClearNotificationAfterDelay(2));
                }
            }
            else
            {
                notificationResult.text = "Failed to fetch user details.";
                StartCoroutine(ClearNotificationAfterDelay(2));
            }
        }));
    }

    private IEnumerator ClearNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationResult.text = "";
    }
}
