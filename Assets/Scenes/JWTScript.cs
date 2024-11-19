using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JWTScript : MonoBehaviour
{
    private const string TOKEN =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJuZXd0ZXN0NiIsImVtYWlsIjoidGVzdEBuZXcuY29tIiwiaWF0IjoxNzMyMDE3OTU1LCJleHAiOjE3MzIxMDQzNTV9.zD8IiYs1s9cHNjDTRIjwG2aEHaPeNbk5Rajx4V2aR-s";

    private IEnumerator Start()
    {
        using (var request = new UnityWebRequest("http://localhost:3000/users", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {TOKEN}");
            yield return request.SendWebRequest();
            if(request.result is UnityWebRequest.Result.Success)
            {
                Debug.Log($"Request Success:{request.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"Request Error:{request.error}");
            }
        }
    }
}
