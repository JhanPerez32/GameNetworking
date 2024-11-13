using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;

public class Http : MonoBehaviour
{
    const string GET_USERS = "http://localhost:3000/users";

    [SerializeField] private GameObject userPrefab;
    [SerializeField] private Transform userContainer;

    private bool isProfilesLoaded = false;

    public void LoggedIn()
    {
        if (!isProfilesLoaded)
        {
            StartCoroutine(UnityGetRequest(GET_USERS, (success, result) =>
            {
                if (success)
                {
                    var users = JsonConvert.DeserializeObject<List<Users>>(result);

                    foreach (var user in users)
                    {
                        GameObject userEntry = Instantiate(userPrefab, userContainer);

                        var idText = userEntry.transform.Find("IdText").GetComponent<TextMeshProUGUI>();
                        var usernameText = userEntry.transform.Find("UsernameText").GetComponent<TextMeshProUGUI>();
                        var emailText = userEntry.transform.Find("EmailText").GetComponent<TextMeshProUGUI>();

                        idText.text = user.id.ToString();
                        usernameText.text = user.name;
                        emailText.text = user.email;
                    }
                    isProfilesLoaded = true;
                }
                else
                {
                    Debug.LogError("Failed to fetch users");
                }
            }));
        }
    }

    //Display all users
    private static IEnumerator UnityGetRequest(string url, Action<bool, string> callback = null)
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

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            callback?.Invoke(false, request.error);
        }
        else
        {
            Debug.Log("User added to localhost: " + request.downloadHandler.text);
            callback?.Invoke(true, request.downloadHandler.text);
        }
    }

    #region Other
    private IEnumerator UnityPutRequest(string url, string jsonData, Action<bool, string> callback = null)
    {
        jsonData = "{\"name\":\"John\", \"job\":\"Senior Developer\"}";

        var request = UnityWebRequest.Put(url, jsonData);

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

    private IEnumerator UnityDeleteRequest(string url, Action<bool, string> callback = null)
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

    private IEnumerator DownloadImage(string imageUrl, Action<Texture2D> Callback)
    {
        var request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Texture Found");
            var texture = DownloadHandlerTexture.GetContent(request);
            Callback?.Invoke(texture);
        }
    }
    #endregion
}

[System.Serializable]
public struct Users
{
    public int id;
    public string name;
    public string email;
}
