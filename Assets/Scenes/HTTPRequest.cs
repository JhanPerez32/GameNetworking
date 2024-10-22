using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HTTPRequest : MonoBehaviour
{
    private const string MAIN = "https://reqres.in/api";
    private const string USERS = "/users";
    private const string REG = "/register";

    [SerializeField] Image rawImage;

    private IEnumerator Start()
    {
        //var userData = "{\"email\": \"eve.holt@regres.in\", \"password\" : \"pistol\"}";
        //var putRequestData = "{ \"name\": \"morpheus\"
        yield return HttpGetRequest(MAIN + USERS);
        //yield return HttpPostRequest(MAIN + REG, userData);
        //yield return HttpDeleteRequest(MAIN + REG + "/4");
    }

    private IEnumerator HttpGetRequest(string uri)
    {
        var request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if(request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }


    private IEnumerator HttpPostRequest(string uri, string data)
    {
        var request = UnityWebRequest.Post(uri, data, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
            var auth = JsonConvert.DeserializeObject<Authentication>(request.downloadHandler.text);
            Debug.Log(auth.token);
        }
    }

    private IEnumerator HttpPutRequest(string uri, string data)
    {
        var request = UnityWebRequest.Post(uri, data, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            
        }
    }

    private IEnumerator HttpDeleteRequest(string uri)
    {
        var request = UnityWebRequest.Delete(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("User Deleted");
        }
    }

    private IEnumerator GetTextureFromUri(string uri)
    {
        var request = UnityWebRequestTexture.GetTexture(uri);

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            if(rawImage != null)
            {

            }
        }
    }

}

[System.Serializable]
public struct Authentication
{
    public string id;
    public string token;
}
