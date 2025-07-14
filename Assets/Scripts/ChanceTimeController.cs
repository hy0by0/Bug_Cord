using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ChanceTimeシーンのタイマーを管理し、Mainシーンに戻す役割を持つ。
/// </summary>
public class ChanceTimeController : MonoBehaviour
{
    [Tooltip("チャンスタイムの継続時間（秒）")]
    public float chanceTimeDuration = 30f;
    [Tooltip("戻り先のメインシーン名")]
    public string mainSceneName = "Main";

    void Start()
    {
        // タイマースタート
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        Debug.Log($"チャンスタイム開始！ 残り{chanceTimeDuration}秒");
        yield return new WaitForSeconds(chanceTimeDuration);
        Debug.Log("チャンスタイム終了。メインシーンに戻ります。");
        SceneManager.LoadScene(mainSceneName);
    }
}