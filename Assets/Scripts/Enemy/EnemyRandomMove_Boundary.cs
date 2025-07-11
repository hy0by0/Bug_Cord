using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 設定された矩形範囲内をランダムに移動し、境界に達したら内側に向きを変える。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRandomMove_Boundary : MonoBehaviour
{
    [Header("■ 移動設定")]
    [Tooltip("敵の移動速度")]
    public float moveSpeed = 3f;
    [Tooltip("向きを変えるまでの時間（秒）")]
    public float directionChangeInterval = 3.0f;

    [Header("■ 活動範囲設定")]
    [Tooltip("初期位置からの水平方向の最大移動距離")]
    public float horizontalRange = 5f;
    [Tooltip("初期位置からの垂直方向の最大移動距離")]
    public float verticalRange = 3f;

    // --- 内部で使う変数 ---
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 startPosition; // 初期位置を記憶
    private bool isTurning = false; // 方向転換中フラグ

    // 8方向の移動ベクトル
    private readonly List<Vector2> allDirections = new List<Vector2>
    {
        new Vector2(0, 1).normalized, new Vector2(1, 1).normalized,
        new Vector2(1, 0).normalized, new Vector2(1, -1).normalized,
        new Vector2(0, -1).normalized, new Vector2(-1, -1).normalized,
        new Vector2(-1, 0).normalized, new Vector2(-1, 1).normalized
    };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // ゲーム開始時の位置を初期位置として記憶
        startPosition = transform.position;

        ChooseNewDirection();
        StartCoroutine(DirectionChangeRoutine());
    }

    void Update()
    {
        // 境界チェックと方向転換
        CheckBounds();
    }

    void FixedUpdate()
    {
        // 物理演算で移動
        rb.velocity = moveDirection * moveSpeed;
    }

    /// <summary>
    /// 境界の外に出たかどうかをチェックし、出ていれば向きを変える
    /// </summary>
    private void CheckBounds()
    {
        // 方向転換中は連続で判定しない
        if (isTurning) return;

        Vector2 currentPosition = transform.position;
        Vector2 boundaryNormal = Vector2.zero; // 仮想的な壁の向き

        // 水平方向の境界チェック
        if (currentPosition.x > startPosition.x + horizontalRange)
        {
            boundaryNormal = Vector2.left; // 右の壁に当たったので、法線は左向き
        }
        else if (currentPosition.x < startPosition.x - horizontalRange)
        {
            boundaryNormal = Vector2.right; // 左の壁に当たったので、法線は右向き
        }

        // 垂直方向の境界チェック
        if (currentPosition.y > startPosition.y + verticalRange)
        {
            boundaryNormal = Vector2.down; // 上の壁に当たったので、法線は下向き
        }
        else if (currentPosition.y < startPosition.y - verticalRange)
        {
            boundaryNormal = Vector2.up; // 下の壁に当たったので、法線は上向き
        }

        // もし境界に接触していたら（boundaryNormalが設定されていたら）
        if (boundaryNormal != Vector2.zero)
        {
            ChooseNewDirection(boundaryNormal);
            // すぐに再判定しないように短いクールタイムを設ける
            StartCoroutine(TurnCooldown());
        }
    }

    /// <summary>
    /// 新しい移動方向をランダムに決定する
    /// </summary>
    private void ChooseNewDirection(Vector2? boundaryNormal = null)
    {
        List<Vector2> validDirections = new List<Vector2>();

        if (boundaryNormal.HasValue)
        {
            foreach (Vector2 dir in allDirections)
            {
                // 境界の内側を向く方向だけを候補に入れる
                if (Vector2.Dot(dir, boundaryNormal.Value) > 0)
                {
                    validDirections.Add(dir);
                }
            }
        }
        else
        {
            validDirections = allDirections;
        }

        if (validDirections.Count == 0)
        {
            moveDirection = -moveDirection;
            return;
        }

        int index = Random.Range(0, validDirections.Count);
        moveDirection = validDirections[index];
    }

    // 短い方向転換クールダウン
    private IEnumerator TurnCooldown()
    {
        isTurning = true;
        yield return new WaitForSeconds(0.1f);
        isTurning = false;
    }

    // 時間経過による方向転換
    private IEnumerator DirectionChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(directionChangeInterval);
            ChooseNewDirection();
        }
    }
}