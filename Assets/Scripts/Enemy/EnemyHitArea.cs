using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵の各当たり判定で受け取ったプレイヤー攻撃について、EnemyControllerへダメージ量と当たった部位を伝えるクラス。
// このスクリプトを付与下オブジェクトは必ず敵オブジェクト(EnemyControllerがついてるオブジェ)の子にすること！
public class EnemyHitArea : MonoBehaviour
{
    public string hit_area_type = "critical"; //ここで、このあたり判定の部位の設定を行うこと！

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //プレイヤーの攻撃を感知したときの処理
        //この当たり判定がどの当たり判定かに応じて処理を変える（防御力とか、敵をひよりにさせるかなど）
        if (collider.tag == "Atack_player")
        {
            // ここでプレイヤーの攻撃判定から攻撃力の値を受け取る
            PlayerAtack player_atack = collider.GetComponent<PlayerAtack>();

            //この当たり判定がどの当たり判定かに応じて処理を変える（防御力とか、敵をひよりにさせるかなど
            if (hit_area_type == "resist")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitResist(player_atack.atack_damage);
            }
            else if(hit_area_type == "normal")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitNormal(player_atack.atack_damage);
            }
            else if(hit_area_type == "weak")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitWeak(player_atack.atack_damage);
            }
            else if(hit_area_type == "critical")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitCritical(player_atack.atack_damage);
            }
        }
    }
}
