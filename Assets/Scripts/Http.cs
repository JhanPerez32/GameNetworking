using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class Http : MonoBehaviour
{
    [SerializeField] private GameObject userPrefab;
    [SerializeField] private Transform userContainer;
    [SerializeField] private PlayerSpawner playerSpawner;

    private bool isProfilesLoaded = false;

    public void LoggedIn()
    {
        if (!isProfilesLoaded)
        {
            string loggedInEmail = PlayerPrefs.GetString("LoggedInEmail", string.Empty);
            if (string.IsNullOrEmpty(loggedInEmail))
            {
                Debug.LogError("No logged-in email found!");
                return;
            }

            string url = $"http://localhost:3000/users/email/{UnityWebRequest.EscapeURL(loggedInEmail)}";

            StartCoroutine(UnityGetRequest(url, (success, result) =>
            {
                if (success)
                {
                    var user = JsonConvert.DeserializeObject<List<Users>>(result);
                    if (user.Count > 0)
                    {
                        GameObject userEntry = Instantiate(userPrefab, userContainer);

                        var idText = userEntry.transform.Find("Profile Section/IdText").GetComponent<TextMeshProUGUI>();
                        var usernameText = userEntry.transform.Find("Profile Section/UsernameText").GetComponent<TextMeshProUGUI>();
                        var emailText = userEntry.transform.Find("Profile Section/EmailText").GetComponent<TextMeshProUGUI>();
                        var killText = userEntry.transform.Find("Profile Section/Stats/Kill").GetComponent<TextMeshProUGUI>();
                        var deathText = userEntry.transform.Find("Profile Section/Stats/Death").GetComponent<TextMeshProUGUI>();


                        idText.text = user[0].id.ToString();
                        usernameText.text = user[0].name;
                        emailText.text = user[0].email;
                        killText.text = $"K: {user[0].kills}";
                        deathText.text = $"D: {user[0].deaths}";

                        if (playerSpawner != null)
                        {
                            playerSpawner.UpdateKillDeathText(user[0].kills, user[0].deaths);
                        }

                        isProfilesLoaded = true;
                    }
                    else
                    {
                        Debug.LogError("No user found with the provided email.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to fetch user profile.");
                }
            }));
        }
    }

    //Display user
    public IEnumerator UnityGetRequest(string url, Action<bool, string> callback = null)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            callback?.Invoke(false, request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);          
            callback?.Invoke(true, request.downloadHandler.text);

        }
    }

    //Add the new user
    public IEnumerator UnityPostRequest(string url, string jsonData, Action<bool, string> callback = null)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            //Debug.LogError(request.error);

            if (request.responseCode == 409)
            {
                callback?.Invoke(false, "Conflict: Duplicate username or email.");
            }
            else
            {
                callback?.Invoke(false, request.error);
            }
        }
        else
        {
            Debug.Log("User added to localhost: " + request.downloadHandler.text);
            callback?.Invoke(true, request.downloadHandler.text);
        }
    }

    //Deletes the Account
    public IEnumerator UnityDeleteRequest(string url, Action<bool, string> callback = null)
    {
        var request = UnityWebRequest.Delete(url);

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            callback?.Invoke(false, request.error);
        }
        else
        {
            Debug.Log("User Deleted");
            callback?.Invoke(true, "");
        }
    }

    //Update the Account for new changes
    public IEnumerator UnityPutRequest(string url, string jsonData, Action<bool, string> callback = null)
    {
        var request = new UnityWebRequest(url, "PUT")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            callback?.Invoke(false, request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            callback?.Invoke(true, request.downloadHandler.text);
        }
    }
}

[System.Serializable]
public struct Users
{
    public int id;
    public string name;
    public string email;
    public string password;
    public int kills;
    public int deaths;

    public override string ToString()
    {
        return $"ID: {id}, Name: {name}, Email: {email}, Kills: {kills}, Deaths: {deaths}";
    }
}

