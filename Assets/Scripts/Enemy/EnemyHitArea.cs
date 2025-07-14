using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵の各当たり判定で受け取ったプレイヤー攻撃について、EnemyControllerへダメージ量と当たった部位を伝えるクラス。
// このスクリプトを付与下オブジェクトは必ず敵オブジェクト(EnemyControllerがついてるオブジェ)の子にすること！
public class EnemyHitArea : MonoBehaviour
{
    [SerializeField] private PlayerController playerController1;
    [SerializeField] private PlayerController playerController2;
    public string hit_area_type = "critical"; //ここで、このあたり判定の部位の設定を行うこと！

    private void Start()
    {
        if (playerController1 == null)
            playerController1 = GameObject.Find("CharaBody1_prot").GetComponent<PlayerController>();

        if (playerController2 == null)
            playerController2 = GameObject.Find("CharaBody2_prot").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //プレイヤーの攻撃を感知したときの処理
        //この当たり判定がどの当たり判定かに応じて処理を変える（防御力とか、敵をひよりにさせるかなど）
        if (collider.tag == "AttackPlayer1")
        {

            // ここでプレイヤーの攻撃判定から攻撃力の値を受け取る
            PlayerAttack player_attack = collider.GetComponent<PlayerAttack>();

            //この当たり判定がどの当たり判定かに応じて処理を変える（防御力とか、敵をひよりにさせるかなど
            if (hit_area_type == "resist")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitResist(player_attack.GetCalculatedDamage());
                playerController1.Attack();
            }
            else if (hit_area_type == "normal")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitNormal(player_attack.GetCalculatedDamage());
                playerController1.Attack();
            }
            else if (hit_area_type == "weak")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitWeak(player_attack.GetCalculatedDamage());
                playerController1.Attack();
            }
            else if (hit_area_type == "critical")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitCritical(player_attack.GetCalculatedDamage());
                playerController1.Attack();
            }


        }
        if (collider.tag == "AttackPlayer2")
        {

            // ここでプレイヤーの攻撃判定から攻撃力の値を受け取る
            PlayerAttack player_attack = collider.GetComponent<PlayerAttack>();

            //この当たり判定がどの当たり判定かに応じて処理を変える（防御力とか、敵をひよりにさせるかなど
            if (hit_area_type == "resist")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitResist(player_attack.GetCalculatedDamage());
                playerController2.Attack();
            }
            else if (hit_area_type == "normal")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitNormal(player_attack.GetCalculatedDamage());
                playerController2.Attack();

            }
            else if (hit_area_type == "weak")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitWeak(player_attack.GetCalculatedDamage());
                playerController2.Attack();
            }
            else if (hit_area_type == "critical")
            {
                transform.root.gameObject.GetComponent<EnemyController>().HitCritical(player_attack.GetCalculatedDamage());
                playerController2.Attack();
            }

        }
    }

}
