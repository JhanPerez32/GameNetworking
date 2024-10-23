using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Http : MonoBehaviour
{
    const string GET_USERS = "https://reqres.in/api/users/";

    [SerializeField] private GameObject userPrefab;
    [SerializeField] private Transform userContainer;

    private bool isProfilesLoaded = false;

    /*[SerializeField] private RawImage imagetoChange;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI firstNameText;
    [SerializeField] private TextMeshProUGUI lastNameText;*/

    public void LoggedIn()
    {
        if (!isProfilesLoaded)
        {
            StartCoroutine(UnityGetRequest(GET_USERS, (success, result) =>
            {
                if (success)
                {
                    var data = JsonConvert.DeserializeObject<Datas>(result);

                    foreach (var user in data.data)
                    {
                        GameObject userEntry = Instantiate(userPrefab, userContainer);

                        var avatarImage = userEntry.transform.Find("Avatar").GetComponent<RawImage>();
                        var emailText = userEntry.transform.Find("EmailText").GetComponent<TextMeshProUGUI>();
                        var firstNameText = userEntry.transform.Find("FirstNameText").GetComponent<TextMeshProUGUI>();
                        var lastNameText = userEntry.transform.Find("LastNameText").GetComponent<TextMeshProUGUI>();

                        emailText.text = user.email;
                        firstNameText.text = user.first_name;
                        lastNameText.text = user.last_name;

                        StartCoroutine(DownloadImage(user.avatar, (texture) =>
                        {
                            if (avatarImage != null)
                            {
                                avatarImage.texture = texture;
                            }
                        }));
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

    #region Other
    private static IEnumerator UnityPostRequest(string url, string jsonData, Action<bool, string> callback = null)
    {
        jsonData = "{\"name\":\"John\", \"job\":\"Developer\"}";

        var request = UnityWebRequest.Post(url, jsonData, "application/json");
        /*
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        */

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

    private IEnumerator UnityPutRequest(string url, string jsonData, Action<bool, string> callback = null)
    {
        jsonData = "{\"name\":\"John\", \"job\":\"Senior Developer\"}";

        var request = UnityWebRequest.Put(url, jsonData);

        /*
         var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        */

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
    #endregion

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
}

[System.Serializable]
public struct Datas
{
    public int page;
    public int per_page;
    public int total;
    public int total_pages;
    public Users[] data;
}

[System.Serializable]
public struct Users
{
    public int id;
    public string email;
    public string first_name;
    public string last_name;
    public string avatar;
}
