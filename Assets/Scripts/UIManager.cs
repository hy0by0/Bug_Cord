using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UIを扱うため必須。

// UIにまつわる処理を担う。他のスクリプトから呼び出して用いられることが多い。
public class UIManager : MonoBehaviour
{
    public GameObject enemy_hp_bar; //ＨＰバーのＵＩ画像オブジェクトを入力 (入れるのは長さが変化する部分のパーツ！)
    public int enemy_hp_max = 10000; //敵の最大ＨＰ　EnemyControllerスクリプトから値が反映・優先されます(ここからは変更不可)
    public float enemy_hp_remain = 10000; //敵の残りのＨＰ　EnemyControllerスクリプトから値が反映・優先されます(ここからは変更不可)
    //[SerializeField]ScoreManager score_manager;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //　敵のHPに合わせてバーの長さを変更する 入力：受けたダメージ量
    public void DamagedBar(int Atack)
    {
        enemy_hp_remain -= Atack;
        // ScoreManagerのシングルトンInstanceを使う
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.getScore += Atack;
            Debug.Log(ScoreManager.Instance.getScore);
        }
        else
        {
            Debug.LogWarning("ScoreManager Instance が見つかりません");
        }

        float gauge = enemy_hp_remain/ enemy_hp_max;
        enemy_hp_bar.GetComponent<Image>().fillAmount = gauge;
    }
}
