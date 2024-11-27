using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class JWTScript : MonoBehaviour
{
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardPanel;
    private const string TOKEN =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJub25hIiwiZW1haWwiOiJub25hQGVtYWlsLmNvbSIsImlhdCI6MTczMjE2OTY0OSwiZXhwIjoxNzM0NzYxNjQ5fQ.56MYwjhKCz2Z6yFhT7MmBAWR1NTZSRI3bSH9fMynKDU";

    private void Start()
    {
        StartCoroutine(GetLeaderboard());
    }

    private IEnumerator GetLeaderboard()
    {
        using (var request = new UnityWebRequest("http://localhost:3000/leaderboard", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {TOKEN}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Request Success: {request.downloadHandler.text}");
                DisplayLeaderboard(request.downloadHandler.text);
            }
            else
            {
                Debug.Log($"Request Error: {request.error}");
            }
        }
        #region defaultCode
        /*using (var request = new UnityWebRequest("http://localhost:3000/users", "GET"))
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
        }*/
        #endregion

    }

    private void DisplayLeaderboard(string json)
    {
        var leaderboardData = JsonUtility.FromJson<LeaderboardList>($"{{\"users\":{json}}}");

        foreach (Transform child in leaderboardPanel)
        {
            Destroy(child.gameObject);
        }

        // Populate the leaderboard with data
        foreach (var user in leaderboardData.users)
        {
            var entry = Instantiate(leaderboardEntryPrefab, leaderboardPanel);

            var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = user.id.ToString(); // ID
            texts[1].text = user.name;         // Name
            texts[2].text = user.kills.ToString(); // Kills
            texts[3].text = user.deaths.ToString(); // Deaths
            texts[4].text = user.kd_ratio.ToString("F2"); // K/D Ratio
        }
    }

    public IEnumerator UpdatePlayerScore(string userId, int kills, int deaths)
    {
        var jsonBody = JsonUtility.ToJson(new { kills, deaths });
        using (var request = new UnityWebRequest($"http://localhost:3000/users/{userId}/score", "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPUT;

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {TOKEN}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Score updated successfully: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Failed to update score: {request.error}");
            }
        }
    }


    [System.Serializable]
    public class LeaderboardList
    {
        public List<User> users;
    }

    [System.Serializable]
    public class User
    {
        public int id;
        public string name;
        public int kills;
        public int deaths;
        public float kd_ratio;
    }
}
