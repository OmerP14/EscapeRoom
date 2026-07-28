using Unity.Netcode;
using TMPro;
using UnityEngine;

public class CodeLock : Interactable
{
    public string correctCode = "1234";
    public GameObject uiPanel;
    public TMP_InputField codeInputField;
    public GameObject door;

    private NetworkVariable<bool> isUnlocked = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        isUnlocked.OnValueChanged += OnUnlockedChanged;
        ApplyState(isUnlocked.Value);
    }

    public override void Interact()
    {
        if (isUnlocked.Value) return;

        uiPanel.SetActive(true);
        codeInputField.text = "";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    protected override void OnInteractServer()
    {
        // CodeLock kendi Interact() fonksiyonunu kullaniyor, bu bos kalabilir.
    }

    public void SubmitCode()
    {
        if (codeInputField.text == correctCode)
        {
            SubmitCodeRpc();
        }
        ClosePanel();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SubmitCodeRpc()
    {
        isUnlocked.Value = true;
    }

    public void ClosePanel()
    {
        uiPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnUnlockedChanged(bool previous, bool current)
    {
        ApplyState(current);
    }

    private void ApplyState(bool unlocked)
    {
        if (door != null)
        {
            door.SetActive(!unlocked);
        }
    }
}