using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Transform cameraHolder;

    private CharacterController characterController;
    private float verticalRotation = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            cameraHolder.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (GameManager.Instance != null && GameManager.Instance.gameState.Value != 0) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraHolder.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * h + transform.forward * v) * moveSpeed;
        characterController.Move(move * Time.deltaTime);
    }
}