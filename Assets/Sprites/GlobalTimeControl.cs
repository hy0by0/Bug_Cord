using UnityEngine;

public class GlobalTimeControl : MonoBehaviour
{
    public static GlobalTimeControl Instance { get; private set; }

    public float globalTimeInSeconds = 300f;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
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
