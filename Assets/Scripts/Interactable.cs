using Unity.Netcode;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    public void Interact()
    {
        InteractServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void InteractServerRpc()
    {
        OnInteractServer();
    }

    protected abstract void OnInteractServer();
}