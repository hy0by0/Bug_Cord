using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChanceTimeController : MonoBehaviour
{
    [Tooltip("チャンスタイムの継続時間（秒）")]
    public float chanceTimeDuration = 20f; // ★ 20秒に設定
    [Tooltip("戻り先のメインシーン名")]
    public string mainSceneName = "Main";

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        Debug.Log($"チャンスタイム開始！ 残り{chanceTimeDuration}秒");
        yield return new WaitForSeconds(chanceTimeDuration);

        // ★【追加】Mainシーンに戻る直前に、タイマー管理者から時間を引く
        if (CountDownClock.Instance != null)
        {
            CountDownClock.Instance.SubtractTime(chanceTimeDuration);
        }

        Debug.Log("チャンスタイム終了。メインシーンに戻ります。");
        SceneManager.LoadScene(mainSceneName);
    }
}