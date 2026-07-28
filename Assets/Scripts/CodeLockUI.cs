using UnityEngine;

public class CodeLockUI : MonoBehaviour
{
    public static CodeLockUI Instance;
    private CodeLock currentLock;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCurrentLock(CodeLock lockRef)
    {
        currentLock = lockRef;
    }

    public void Submit()
    {
        
        if (currentLock != null) currentLock.SubmitCode();
    }
    public void Cancel()
    {
        if (currentLock != null) currentLock.ClosePanel();
    }
}