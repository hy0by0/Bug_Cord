using UnityEngine;


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
            globalTimeInSeconds = totalTime_Static; // 初始化当前时紒E
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
