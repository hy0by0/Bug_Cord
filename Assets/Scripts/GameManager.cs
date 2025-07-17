using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startImage;
    public float activeUITime = 1.0f;
    public ChangeScene changeSceneManager;
    [SerializeField] private string nextSceneName = "Result";
    public float time_changescene = 2.5f;

    // ★【追加】監視対象となる2体の敵への参照
    [Header("■ 監視対象の敵")]
    public EnemyController enemy1;
    public EnemyController enemy2;

    void Start()
    {
        // ★【変更】特定の敵インスタンスのHPをチェックする
        // (ここではenemy1のHPを代表としてチェック)
        if (SceneManager.GetActiveScene().name == "SampleScene" ||
           (SceneManager.GetActiveScene().name == "Main" && enemy1 != null && enemy1.enemy_hp == 10000))
        {
            Invoke("InactiveImage", activeUITime);
        }
    }

    void Update()
    {
        // ★【変更】どちらかの敵の状態が"Dead"になったかをチェック
        // (&& 演算子の短絡評価により、nullでもエラーにならない)
        if ((enemy1 != null && enemy1.enemy_state == "Dead") || (enemy2 != null && enemy2.enemy_state == "Dead"))
        {
            Debug.Log("敵の討伐を感知！");
            StartCoroutine(HandleDeathAndSceneChange());
            // ★ 一度検出したら、このコンポーネントを無効にして連続実行を防ぐ
            this.enabled = false;
        }
    }

    void InactiveImage()
    {
        startImage.SetActive(false);
    }

    private IEnumerator HandleDeathAndSceneChange()
    {
        FindObjectOfType<ScoreController>().FinalScore();
        Debug.Log("敵が倒れた... 演出開始！");
        yield return new WaitForSeconds(time_changescene);
        if (changeSceneManager != null)
        {
            changeSceneManager.Load(nextSceneName);
        }
        else
        {
            Debug.LogError("ChangeSceneの参照がされていません！");
        }
    }
}