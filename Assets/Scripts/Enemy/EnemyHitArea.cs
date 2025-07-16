// EnemyHitArea2D.cs
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 2D版 EnemyHitArea：
/// ・Collider2D＋OnTriggerEnter2D で判定
/// ・弱点を攻撃(hit_area_type=="weak")されたときだけ ItemSpawner_main に通知
/// </summary>
public class EnemyHitArea2D : MonoBehaviour
{
    [SerializeField] private PlayerController playerController1;
    [SerializeField] private PlayerController playerController2;

    [Header("当たり判定タイプ (normal | resist | weak | critical)")]
    public string hit_area_type = "weak";

    private void OnTriggerEnter2D(Collider2D other)
    {
        int playerIndex;
        PlayerAttack attackComp;
        PlayerController controller;

        // 今のシーン名を取得
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Main")
        {
            // Main シーンではオブジェクト名も条件に入れる
            if (other.CompareTag("AttackPlayer1") && gameObject.name.EndsWith("_P1"))
            {
                playerIndex = 0;
                attackComp = other.GetComponent<PlayerAttack>();
                controller = playerController1;
            }
            else if (other.CompareTag("AttackPlayer2") && gameObject.name.EndsWith("_P2"))
            {
                playerIndex = 1;
                attackComp = other.GetComponent<PlayerAttack>();
                controller = playerController2;
            }
            else
            {
                return;  // Main シーンで条件を満たさなければ無視
            }
        }
        else if (sceneName == "SampleScene")
        {
            // SampleScene ではオブジェクト名のチェックなし
            if (other.CompareTag("AttackPlayer1"))
            {
                playerIndex = 0;
                attackComp = other.GetComponent<PlayerAttack>();
                controller = playerController1;
            }
            else if (other.CompareTag("AttackPlayer2"))
            {
                playerIndex = 1;
                attackComp = other.GetComponent<PlayerAttack>();
                controller = playerController2;
            }
            else
            {
                return;  // SampleScene で条件を満たさなければ無視
            }
        }
        else
        {
            return;  // 他のシーンでは処理しない場合
        }

        // ダメージ処理
        var enemyCtrl = GetComponentInParent<EnemyController>();
        int dmg = attackComp.GetCalculatedDamage();
        switch (hit_area_type)
        {
            case "normal": enemyCtrl.HitNormal(dmg); break;
            case "resist": enemyCtrl.HitResist(dmg); break;
            case "weak": enemyCtrl.HitWeak(dmg); break;
            case "critical": enemyCtrl.HitCritical(dmg); break;
            default:
                Debug.LogWarning($"Unknown hit_area_type: {hit_area_type}");
                break;
        }

        if (controller != null)
        {
            controller.Attack();

            // ★弱点攻撃(weak)のときだけ通知
            if (hit_area_type == "weak")
            {
                ItemSpawner_main.NotifyWeakHit(playerIndex == 0 ? PlayerID.P1 : PlayerID.P2);

            }
        }

    }
}
