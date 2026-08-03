using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;
    public GameObject pauseMenuPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void TogglePause()
    {
        SetPaused(!pauseMenuPanel.activeSelf);
    }

    public void SetPaused(bool paused)
    {
        pauseMenuPanel.SetActive(paused);
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void Resume()
    {
        SetPaused(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPausedRpc(false);
        }
    }
}