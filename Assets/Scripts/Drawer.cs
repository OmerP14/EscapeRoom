using Unity.Netcode;
using UnityEngine;

public class Drawer : Interactable
{
    public Transform drawerPanel;
    public GameObject key;
    public Vector3 openOffset = new Vector3(0f, 0f, 1f);

    private NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);
    private Vector3 closedPosition;

    private void Awake()
    {
        closedPosition = drawerPanel.localPosition;
        if (key != null) key.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        isOpen.OnValueChanged += OnIsOpenChanged;
        ApplyState(isOpen.Value);
    }

    private void OnIsOpenChanged(bool previous, bool current)
    {
        ApplyState(current);
    }

    private void ApplyState(bool open)
    {
        drawerPanel.localPosition = open ? closedPosition + openOffset : closedPosition;
        if (key != null) key.SetActive(open);
    }

    protected override void OnInteractServer()
    {
        if (!isOpen.Value)
        {
            isOpen.Value = true;
        }
    }
}