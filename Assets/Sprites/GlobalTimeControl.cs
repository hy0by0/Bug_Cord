using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GlobalTimeControl : MonoBehaviour
{
    public static GlobalTimeControl Instance { get; private set; }

    public float totalTime_Static = 300f;
    public float globalTimeInSeconds;

    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            globalTimeInSeconds = totalTime_Static; // 初始化当前时间
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        if (globalTimeInSeconds > 0f)
        {
            globalTimeInSeconds -= Time.deltaTime;
            if (globalTimeInSeconds < 0f)
                globalTimeInSeconds = 0f;
        }
    }
}
