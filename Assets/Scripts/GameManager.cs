using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
#else
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
#endif
    }

    void Update()
    {
        Timer.ProcessTimers();
    }
} 