using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3x3のグリッド上を移動するキャラクターを制御します。
/// プレイヤーの入力、移動アニメーション、そしてスタン状態（行動不能）を管理します。
/// </summary>
public class PlayerController : MonoBehaviour
{
    // --- インスペクターで設定する項目 ---
    [Header("■ グリッドと移動設定")]
    [Tooltip("シーンに配置した9個の移動先マーカー（Transform）")]
    public List<Transform> positionMarkers;
    [Tooltip("1マス移動するのにかかる時間（秒）")]
    public float moveDuration = 0.3f;

    [Header("■ 遠近法（パース）の設定")]
    [Tooltip("一番手前のマスでの大きさの倍率 (1.0 = 100%)")]
    public float frontRowScale = 1.0f;
    [Tooltip("一番奥のマスでの大きさの倍率 (0.7 = 70%)")]
    public float backRowScale = 0.7f;

    [Header("■ 開始位置")]
    [Tooltip("ゲーム開始時のX座標 (0-2)")]
    public int startX = 1;
    [Tooltip("ゲーム開始時のY座標 (0-2)")]
    public int startY = 0;

    // --- 内部で管理する変数 ---
    private Transform[,] gridPositions = new Transform[3, 3];
    private int currentX, currentY; // 論理的な現在座標
    private bool isMoving = false; // 移動アニメーション中かどうかのフラグ
    private bool isStunned = false; // スタン（行動不能）中かどうかのフラグ
    private Vector3 originalScale; // 本来の大きさ

    //// 以下、プレイヤーグラフィック関連の変数

    private SpriteRenderer spriteRenderer; // オブジェクトの画像コンポーネントを保持するための変数
    private Color originalColor;

    private Coroutine flashCoroutine;    // 実行中の色変更コルーチンを保持するための変数

    private NewActions inputActions;

    public int playerNumber = 1; // 1=Player1, 2=Player2
    Vector2 move;

    /// <summary>
    /// ゲーム開始時に一度だけ呼ばれる初期化処理
    /// </summary>
    void Start()
    {
        inputActions = new NewActions();

        Debug.Log("番号" + playerNumber);

        if (playerNumber == 1)
        {
            inputActions.Player.Enable();
        }else if (playerNumber == 2)
        {
            inputActions.Player2.Enable();
        }

        originalScale = transform.localScale; // 基準となる大きさを記憶

        // インスペクターのリストを2次元配列に変換
        if (positionMarkers.Count == 9)
        {
            for (int y = 0; y < 3; y++) for (int x = 0; x < 3; x++)
                    gridPositions[x, y] = positionMarkers[y * 3 + x];
        }
        else
        {
            Debug.LogError("エラー: positionMarkersに9個のTransformが設定されていません！");
            enabled = false;
            return;
        }

        // 初期位置とスケールを設定
        currentX = startX;
        currentY = startY;
        transform.position = gridPositions[currentX, currentY].position;
        float initialScaleMultiplier = Mathf.Lerp(frontRowScale, backRowScale, currentY / 2.0f);
        transform.localScale = originalScale * initialScaleMultiplier;


        spriteRenderer = GetComponent<SpriteRenderer>();// オブジェクトの画像コンポーネントを取得(攻撃時に赤色へ敵を変えるため)
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// 毎フレーム実行される更新処理
    /// </summary>
    void Update()
    {
        // 移動中、またはスタン中は、キー入力を一切受け付けずに処理を終了する
        if (isMoving || isStunned)
        {
            return;
        }
        if (playerNumber == 1)
        {
            move = inputActions.Player.Move.ReadValue<Vector2>();
        }
        else if (playerNumber == 2)
        {
            move = inputActions.Player2.Move2.ReadValue<Vector2>();
        }


        if (move.y > 0.5f)
            AttemptMove(0, 1);        // 上

        else if (move.y < -0.5f)
            AttemptMove(0, -1);       // 下

        else if (move.x < -0.5f)
            AttemptMove(-1, 0);       // 左

        else if (move.x > 0.5f)
            AttemptMove(1, 0);        // 右

        if (playerNumber == 1)
        {
            // キーが「押された瞬間」に入力を検知
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) AttemptMove(0, 1);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) AttemptMove(0, -1);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) AttemptMove(-1, 0);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) AttemptMove(1, 0);
        }
    }

    /// <summary>
    /// 移動を試みるロジック
    /// </summary>
    private void AttemptMove(int xOffset, int yOffset)
    {
        int targetX = currentX + xOffset;
        int targetY = currentY + yOffset;

        // 移動先がグリッド範囲内なら移動処理を開始
        if (targetX >= 0 && targetX < 3 && targetY >= 0 && targetY < 3)
        {
            currentX = targetX;
            currentY = targetY;
            float targetScaleMultiplier = Mathf.Lerp(frontRowScale, backRowScale, targetY / 2.0f);
            Vector3 targetScale = originalScale * targetScaleMultiplier;
            StartCoroutine(MoveCoroutine(gridPositions[currentX, currentY].position, targetScale));
        }
    }

    /// <summary>
    /// 見た目を滑らかに動かすコルーチン
    /// </summary>
    private IEnumerator MoveCoroutine(Vector3 targetPosition, Vector3 targetScale)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            float easedT = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, easedT);
            yield return null;
        }
        transform.position = targetPosition;
        transform.localScale = targetScale;
        isMoving = false;
    }

    // --- 外部から呼び出される公開関数 ---

    /// <summary>
    /// このキャラクターにスタン効果を適用します。
    /// </summary>
    /// <param name="duration">スタンさせる時間（秒）</param>
    public void ApplyStun(float duration)
    {
        if (!isStunned) // すでにスタン中でなければ
        {
            StartCoroutine(StunCoroutine(duration));
        }
    }

    /// <summary>
    /// 指定された時間、キャラクターを行動不能にするコルーチン
    /// </summary>
    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true; // スタン状態フラグを立て、入力をブロック
        Debug.Log("スタン開始！");


        if (flashCoroutine != null)        // もし既に色の変更コルーチンが実行中なら、それを停止する
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashColorCoroutine());        // 新しく色の変更コルーチンを開始し、その情報を変数に保存する


        yield return new WaitForSeconds(duration); // 指定時間、待機

        // ここでスタン演出を元に戻す

        isStunned = false; // スタン状態フラグを下ろし、再び入力可能にする
        Debug.Log("スタン終了！");
    }


    // 色を一定時間変更して元に戻すコルーチン
    private IEnumerator FlashColorCoroutine()
    {
        // 1. スプライトの色を赤に変更する
        spriteRenderer.color = Color.red;

        // 2. 指定した秒数だけ処理を待つ
        yield return new WaitForSeconds(0.15f);

        // 3. Start()で保存しておいた「本来の色」に戻す
        spriteRenderer.color = originalColor;

        // 4. 処理が終わったので、保持していたコルーチン情報をnullにする
        flashCoroutine = null;
    }

    /// <summary>
    /// 現在のX座標を外部に教える
    /// </summary>
    public int GetCurrentX() { return currentX; }

    /// <summary>
    /// 現在のY座標を外部に教える
    /// </summary>
    public int GetCurrentY() { return currentY; }
}