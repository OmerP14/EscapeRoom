using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public float startTime = 600f;
    public GameObject barrier1;
    public GameObject barrier2;
    public GameObject barrier3;
    public Transform exitPoint;
    public float exitRadius = 1.5f;

    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>();
    public NetworkVariable<int> gameState = new NetworkVariable<int>(0);
    public NetworkVariable<bool> isPaused = new NetworkVariable<bool>(false);

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timeRemaining.Value = startTime;
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetPausedRpc(bool paused)
    {
        isPaused.Value = paused;
    }

    void Update()
    {
        if (!IsServer) return;
        if (gameState.Value != 0) return;
        if (isPaused.Value) return;

        timeRemaining.Value -= Time.deltaTime;

        if (timeRemaining.Value <= 0f)
        {
            timeRemaining.Value = 0f;
            gameState.Value = 2;
            return;
        }

        bool allSolved = !barrier1.activeSelf && !barrier2.activeSelf && !barrier3.activeSelf;
        if (allSolved)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject == null) continue;
                Vector3 pos = client.PlayerObject.transform.position;
                Vector3 posFlat = new Vector3(pos.x, 0f, pos.z);
                Vector3 exitFlat = new Vector3(exitPoint.position.x, 0f, exitPoint.position.z);
                if (Vector3.Distance(posFlat, exitFlat) <= exitRadius)
                {
                    gameState.Value = 1;
                    return;
                }
            }
        }
    }
}