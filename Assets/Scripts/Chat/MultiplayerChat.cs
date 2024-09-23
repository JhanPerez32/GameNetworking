using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class MultiplayerChat : NetworkBehaviour
{
    [SerializeField] GameObject chatBox;
    [SerializeField] TextMeshProUGUI chatHistory;
    [SerializeField] TMP_InputField enterMessage;
    [SerializeField] TMP_InputField setUsername;

    public string username = "Default";
    private bool isChatBoxActive = false;

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
        {
            isChatBoxActive = !isChatBoxActive;
            chatBox.SetActive(isChatBoxActive);
        }
    }

    public void SetUsername()
    {
        username = setUsername.text;
    }

    public void CallMessageRPC()
    {
        string message = enterMessage.text;
        RPC_SendMessage(username, message);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string username, string message, RpcInfo rpcInfo = default)
    {
        chatHistory.text += $"{username}: {message}\n";
    }


}
