// EnemyHitArea2D.cs
using UnityEngine;

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
            return;  // AttackPlayer1/2 以外は無視
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

        controller.Attack();

        // ★弱点攻撃(weak)のときだけ通知
        if (hit_area_type == "weak")
        {
            ItemSpawner_main.NotifyWeakHit(playerIndex == 0 ? PlayerID.P1 : PlayerID.P2);

        }
    }
}
