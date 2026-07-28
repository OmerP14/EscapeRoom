using Unity.Netcode;
using UnityEngine;

public class PressurePlate : NetworkBehaviour
{
    public NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false);
    public Renderer plateRenderer;

    public override void OnNetworkSpawn()
    {
        isPressed.OnValueChanged += OnPressedChanged;
        ApplyVisual(isPressed.Value);
    }

    private void OnPressedChanged(bool previous, bool current)
    {
        ApplyVisual(current);
    }

    private void ApplyVisual(bool pressed)
    {
        if (plateRenderer != null)
        {
            plateRenderer.material.color = pressed ? Color.green : Color.red;
        }
    }
}