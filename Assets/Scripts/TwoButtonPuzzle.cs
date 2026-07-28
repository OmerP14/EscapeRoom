using Unity.Netcode;
using UnityEngine;

public class TwoButtonPuzzle : NetworkBehaviour
{
    public PressurePlate plateA;
    public PressurePlate plateB;
    public GameObject barrier;
    public float pressRadius = 0.8f;

    private NetworkVariable<bool> isSolved = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        isSolved.OnValueChanged += OnSolvedChanged;
        ApplyState(isSolved.Value);
    }

    void Update()
    {
        if (NetworkManager.Singleton == null || !IsServer) return;
        if (isSolved.Value) return;

        bool aPressed = false;
        bool bPressed = false;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null) continue;
            Vector3 pos = client.PlayerObject.transform.position;

            Vector3 posFlat = new Vector3(pos.x, 0f, pos.z);
            Vector3 plateAFlat = new Vector3(plateA.transform.position.x, 0f, plateA.transform.position.z);
            Vector3 plateBFlat = new Vector3(plateB.transform.position.x, 0f, plateB.transform.position.z);

            if (Vector3.Distance(posFlat, plateAFlat) <= pressRadius) aPressed = true;
            if (Vector3.Distance(posFlat, plateBFlat) <= pressRadius) bPressed = true;
        }

        plateA.isPressed.Value = aPressed;
        plateB.isPressed.Value = bPressed;

        if (aPressed && bPressed)
        {
            isSolved.Value = true;
        }
    }

    private void OnSolvedChanged(bool previous, bool current)
    {
        ApplyState(current);
    }

    private void ApplyState(bool solved)
    {
        if (barrier != null)
        {
            barrier.SetActive(!solved);
        }
    }
}