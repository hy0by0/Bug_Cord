using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDownClock : MonoBehaviour
{
    public float timeInSeconds = 300f; // 倒计时总时间
    public Transform pinSec;           // 秒针
    public Transform pinMin;           // 分针
    public Image fillImage;            // 进度条（Image类型）
    public GameObject GameEndPanel;

    private float maxTime;
    private bool hasEnded = false;

    void Start()
    {
        maxTime = timeInSeconds;
    }

    void Update()
    {
        if (timeInSeconds > 0f)
        {
            timeInSeconds -= Time.deltaTime;
            if (timeInSeconds < 0f)
                timeInSeconds = 0f;

            UpdatePins();
            UpdateFill();
        }

        // 倒计时结束，触发一次事件
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

    // 倒计时结束时调用，你可以在子类中重写或在Unity中手动调用其他行为
    public virtual void OnTimerEnd()
    {
        GameEndPanel.SetActive(true);
    }
    public void GameEnd()
    {
        SceneManager.LoadScene("Result");
    }
}
