using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ChangeScene changeSceneManager; // ChangeSceneスクリプトを参照
    [SerializeField] private string nextSceneName = "Result"; // 遷移先のシーン名

    public float time_changescene = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyController.enemy_state == "Dead")
        {
            Debug.Log("敵の討伐を感知！");
            StartCoroutine(HandleDeathAndSceneChange());
        }
    }

    // シーン遷移前の演出付き処理
    private IEnumerator HandleDeathAndSceneChange()
    {
        // 演出を入れる（ここは好きに作ってOK）
        Debug.Log("敵が倒れた... 演出開始！");

        // 例：数秒の演出待ち時間（フェードやSEを想定）
        yield return new WaitForSeconds(time_changescene);

        // シーン遷移（ChangeSceneスクリプトのLoadを使用）
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
