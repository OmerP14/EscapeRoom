using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    public Camera playerCamera;
    public float interactRange = 3f;

    void Update()
    {
        if (!IsOwner) return;
        if (GameManager.Instance != null && GameManager.Instance.gameState.Value != 0) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}