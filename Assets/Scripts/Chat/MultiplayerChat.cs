using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class MultiplayerChat : NetworkBehaviour
{
    public static MultiplayerChat Instance;

    [SerializeField] GameObject chatBox;
    [SerializeField] TextMeshProUGUI chatHistory;
    [SerializeField] TMP_InputField enterMessage;

    private Dictionary<int, string> playerUsernames = new Dictionary<int, string>();
    private bool isChatBoxActive = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
        {
            isChatBoxActive = !isChatBoxActive;
            chatBox.SetActive(isChatBoxActive);
        }
    }

    public void SetUsername(int playerId)
    {
        if (!playerUsernames.ContainsKey(playerId))
        {
            string username = $"Player_{playerId}";
            playerUsernames.Add(playerId, username);
            Debug.Log($"Username set to: {username} for Player ID: {playerId}");
        }
    }

    public void CallMessageRPC()
    {
        string message = enterMessage.text;
        int localPlayerId = Runner.LocalPlayer.PlayerId;
        if (playerUsernames.TryGetValue(localPlayerId, out string username))
        {
            Debug.Log($"Sending message from: {username}");
            RPC_SendMessage(username, message);
        }
        else
        {
            Debug.LogError("Player username not found!");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string username, string message, RpcInfo rpcInfo = default)
    {
        chatHistory.text += $"{username}: {message}\n";
    }
}
