using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public TMP_Text timerText;
    public GameObject winPanel;
    public GameObject losePanel;

    private bool subscribed = false;

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientStopped += HandleNetworkStopped;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientStopped -= HandleNetworkStopped;
        }
    }

    private void HandleNetworkStopped(bool wasHost)
    {
        StartCoroutine(ReloadSceneRoutine());
    }

    private System.Collections.IEnumerator ReloadSceneRoutine()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (!subscribed)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.timeRemaining.OnValueChanged += OnTimeChanged;
                GameManager.Instance.gameState.OnValueChanged += OnStateChanged;
                UpdateTimerText(GameManager.Instance.timeRemaining.Value);
                subscribed = true;
            }
        }
    }

    private void OnTimeChanged(float previous, float current)
    {
        UpdateTimerText(current);
    }

    private void UpdateTimerText(float time)
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnStateChanged(int previous, int current)
    {
        if (current == 1) winPanel.SetActive(true);
        else if (current == 2) losePanel.SetActive(true);

        if (current != 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ReturnToMenu()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        NetworkManager.Singleton.Shutdown();
    }
}