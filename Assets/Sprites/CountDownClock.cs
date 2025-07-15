using UnityEngine;
using UnityEngine.UI;

public class CountDownClock : MonoBehaviour
{
    public float timeInSeconds = 300f; // 初始备用值
    public Transform pinSec;           // 秒针
    public Transform pinMin;           // 分针
    public Image fillImage;            // 进度条（Image类型）
    public GameObject GameEndPanel;

    private float maxTime;
    private bool hasEnded = false;

    void Start()
    {
        if (GlobalTimeControl.Instance != null)
        {
            maxTime = GlobalTimeControl.Instance.globalTimeInSeconds;
        }
        else
        {
            maxTime = timeInSeconds;
        }
    }

    void Update()
    {
        

        timeInSeconds = GlobalTimeControl.Instance.globalTimeInSeconds;

        if (timeInSeconds > 0f)
        {
            UpdatePins();
            UpdateFill();
        }

        if (!hasEnded && timeInSeconds <= 0f)
        {
            hasEnded = true;
            OnTimerEnd();
        }
    }

    void UpdatePins()
    {
        float secAngle = (timeInSeconds % 60f) * 6f;
        pinSec.localEulerAngles = new Vector3(0, 0, -secAngle);

        float percent = 1f - (timeInSeconds / maxTime);
        float minAngle = percent * 360f;
        pinMin.localEulerAngles = new Vector3(0, 0, minAngle);
    }

    void UpdateFill()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = timeInSeconds / maxTime;
        }
    }

    public virtual void OnTimerEnd()
    {
        GameEndPanel.SetActive(true);
    }
}
