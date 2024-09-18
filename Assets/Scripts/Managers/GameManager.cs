using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using GNW2.Input;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace GNW2.GameManager
{
    public class GameManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private GameObject _platform;

        private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

        private bool _isMouseButton0Pressed;

        public Button hostGameButton;
        public Button joinGameButton;
        public GameObject UICanvas;

        #region NetworkRunner Callbacks
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Vector3 customLocation = new Vector3(1 * runner.SessionInfo.PlayerCount, 0, 0);
                NetworkObject playerNetworkObject = runner.Spawn(_playerPrefab, customLocation, Quaternion.identity);
                playerNetworkObject.AssignInputAuthority(player);
                _spawnedPlayers.Add(player, playerNetworkObject);
                ChangePlatformColor();
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedPlayers.TryGetValue(player, out NetworkObject playerNetworkObject))
            {
                runner.Despawn(playerNetworkObject);
                _spawnedPlayers.Remove(player);
            }
            ChangePlatformColor();

            if (_spawnedPlayers.Count == 0)
            {
                SetCursorState(false);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_runner == null) return;

            var data = new NetworkInputData();
            if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                data.Direction += Vector3.forward;
            }
            if (UnityEngine.Input.GetKey(KeyCode.A))
            {
                data.Direction += Vector3.left;
            }
            if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                data.Direction += Vector3.back;
            }
            if (UnityEngine.Input.GetKey(KeyCode.D))
            {
                data.Direction += Vector3.right;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                data.Jump = true;
            }
            data.buttons.Set(button: NetworkInputData.MOUSEBUTTON0, _isMouseButton0Pressed);

            input.Set(data);

        }
        
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){ }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){ }
        public void OnConnectedToServer(NetworkRunner runner){ }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){ }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){ }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){ }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){ }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){ }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){ }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){ }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){ }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ }

        public void OnSceneLoadDone(NetworkRunner runner){ }

        public void OnSceneLoadStart(NetworkRunner runner){ }
        #endregion

        private void Start()
        {
            if (_runner == null)
            {
                if (hostGameButton != null)
                {
                    hostGameButton.onClick.AddListener(() =>
                    {
                        StartGame(GameMode.Host);
                        HideUI();
                    });
                }

                if (joinGameButton != null)
                {
                    joinGameButton.onClick.AddListener(() =>
                    {
                        StartGame(GameMode.Client);
                        HideUI();
                    });
                }
            }
                
        }

        void HideUI()
        {
            if (UICanvas != null)
            {
                UICanvas.SetActive(false);
            }
        }

        private void Update()
        {
            _isMouseButton0Pressed = UnityEngine.Input.GetMouseButton(0);
        }

        public async void StartGame(GameMode mode)
        {
            // lets fusion know that we will be sending input
            _runner = this.gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true; 

            //create the scene info to send to fusion
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            await _runner.StartGame(new StartGameArgs()
                {
                    GameMode = mode,
                    SessionName = "TestRoom",
                    Scene = scene,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()

                }
            );

            ChangePlatformColor();

            SetCursorState(true);
        }

        private void ChangePlatformColor()
        {
            if (_platform != null)
            {
                Renderer renderer = _platform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color randomColor = UnityEngine.Random.ColorHSV();
                    renderer.material.color = randomColor;
                }
            }
        }

        private void SetCursorState(bool lockCursor)
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
