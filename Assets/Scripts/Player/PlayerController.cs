using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3x3グリッド上でのプレイヤーの移動、状態（スタン、クールタイム）、見た目を制御します。
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region インスペクターで設定する項目
    // =============== インスペクターで設定する項目 ===============

    [Header("グリッドと移動")]
    [Tooltip("9個の移動先マーカー（Transform）を格納します。")]
    public List<Transform> positionMarkers;
    [Tooltip("1マス移動するのにかかる時間（秒）")]
    public float moveDuration = 0.3f;

    [Header("移動クールタイム (秒)")]
    [Tooltip("手前の行(Y=0)に移動した後のクールタイム")]
    public float frontRowCooldown = 2.0f;
    [Tooltip("中央の行(Y=1)に移動した後のクールタイム")]
    public float middleRowCooldown = 3.0f;
    [Tooltip("奥の行(Y=2)に移動した後のクールタイム")]
    public float backRowCooldown = 5.0f;

    [Header("遠近法（パース）")]
    [Tooltip("一番手前のマスでの大きさの倍率")]
    public float frontRowScale = 1.0f;
    [Tooltip("一番奥のマスでの大きさの倍率")]
    public float backRowScale = 0.7f;

    [Header("初期設定")]
    [Tooltip("ゲーム開始時のX座標 (0-2)")]
    [Range(0, 2)] public int startX = 1;
    [Tooltip("ゲーム開始時のY座標 (0-2)")]
    [Range(0, 2)] public int startY = 0;

    [Header("関連オブジェクト")]
    [Tooltip("スタン時に非表示にする照準オブジェクトなど")]
    [SerializeField] private GameObject aimObject;
    #endregion


    #region 内部で管理する状態変数
    // =============== 内部で管理する状態変数 ===============

    private Transform[,] gridPositions = new Transform[3, 3];
    private int currentX, currentY;
    private Vector3 originalScale;

    // --- キャラクターの状態フラグ ---
    private bool isMoving = false;      // 移動アニメーション中か
    private bool isStunned = false;     // スタン（行動不能）中か
    private bool isCoolingDown = false; // 移動後のクールタイム中か

    // --- コルーチン管理 ---
    private Coroutine flashCoroutine;
    #endregion


    #region キャッシュするコンポーネント
    // =============== キャッシュするコンポーネント ===============
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    #endregion

    #region コントローラー関連

    //インプットアクション
    private NewActions inputActions;

    //プレイヤーの番号
    public int playerNumber = 1;

    //移動量
    Vector2 move;
    #endregion

    #region Unityのライフサイクルメソッド
    // =============== Unityのライフサイクルメソッド ===============

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
        }
        else if (playerNumber == 2)
        {
            inputActions.Player2.Enable();
        }

        if (playerNumber == 1)
        {
            move = inputActions.Player.Move.ReadValue<Vector2>();
        }
        else if (playerNumber == 2)
        {
            move = inputActions.Player2.Move2.ReadValue<Vector2>();
        }


        // 最初にコンポーネントを取得して、毎回GetComponentを呼ばないようにする
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // キャラクターの基本サイズを記憶
        originalScale = transform.localScale;

        // positionMarkersリストを2次元配列に変換して扱いやすくする
        if (positionMarkers.Count == 9)
        {
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    gridPositions[x, y] = positionMarkers[y * 3 + x];
        }
        else
        {
            Debug.LogError("エラー: positionMarkersに9個のTransformが設定されていません！", this);
            enabled = false; // このコンポーネントを無効化
            return;
        }

        // 初期位置とスケールを設定
        currentX = startX;
        currentY = startY;
        transform.position = gridPositions[currentX, currentY].position;
        UpdateScaleBasedOnRow(currentY);
    }

    /// <summary>
    /// 毎フレーム実行される更新処理
    /// </summary>
    void Update()
    {
        // 何らかの理由で行動不能な場合は、キー入力を受け付けずに処理を終了
        if (isMoving || isStunned || isCoolingDown)
        {
            return;
        }

        // キー入力を処理
        HandleInput();
    }
    #endregion


    #region 外部から呼び出すための公開メソッド
    // =============== 外部から呼び出すための公開メソッド ===============

    /// <summary>
    /// このキャラクターにスタン効果を与えます。
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
    /// 攻撃アニメーションを再生します。
    /// </summary>
    public void Attack()
    {
        animator.SetTrigger("Attack1"); // Animatorの"Attack1"トリガーを起動
    }

    /// <summary> 現在のX座標（列）を取得します (0-2) </summary>
    public int GetCurrentX() => currentX;

    /// <summary> 現在のY座標（行）を取得します (0-2) </summary>
    public int GetCurrentY() => currentY;
    #endregion


    #region 内部ロジックとコルーチン
    // =============== 内部ロジックとコルーチン ===============

    /// <summary>
    /// キー入力を検知して移動を試みます。
    /// </summary>
    private void HandleInput()
    {


        if (move.y > 0.5f)
            AttemptMove(0, 1);        // 上

        else if (move.y < -0.5f)
            AttemptMove(0, -1);       // 下

        else if (move.x < -0.5f)
            AttemptMove(-1, 0);       // 左

        else if (move.x > 0.5f)
            AttemptMove(1, 0);        // 右

        // GetKeyDownはキーが押されたそのフレームのみtrueを返す
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) AttemptMove(0, 1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) AttemptMove(0, -1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) AttemptMove(-1, 0);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) AttemptMove(1, 0);
    }

    /// <summary>
    /// 指定された方向への移動を開始します。
    /// </summary>
    private void AttemptMove(int xOffset, int yOffset)
    {
        int targetX = currentX + xOffset;
        int targetY = currentY + yOffset;

        // 移動先がグリッドの範囲内かチェック
        if (targetX >= 0 && targetX < 3 && targetY >= 0 && targetY < 3)
        {
            currentX = targetX;
            currentY = targetY;
            Vector3 targetPosition = gridPositions[currentX, currentY].position;
            Vector3 targetScale = CalculateScaleForRow(currentY);

            // 移動アニメーションのコルーチンを開始
            StartCoroutine(MoveCoroutine(targetPosition, targetScale));
        }
    }

    /// <summary>
    /// キャラクターを目標地点まで滑らかに動かすコルーチン。
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
            float t = Mathf.Clamp01(elapsedTime / moveDuration); // 0.0～1.0の進捗率
            float easedT = t * t * (3f - 2f * t); // 滑らかな加減速（SmoothStep）

            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, easedT);
            yield return null; // 1フレーム待機
        }

        // 最終的な位置とスケールを正確に設定
        transform.position = targetPosition;
        transform.localScale = targetScale;
        isMoving = false;

        // 移動が完了したので、クールタイムを開始する
        StartCoroutine(CooldownCoroutine(currentY));
    }

    /// <summary>
    /// 移動後のクールタイムを処理するコルーチン。
    /// </summary>
    private IEnumerator CooldownCoroutine(int rowY)
    {
        isCoolingDown = true;
        float cooldownDuration = 0f;

        // Y座標（行）に応じてクールタイムの長さを決定
        switch (rowY)
        {
            case 0: cooldownDuration = frontRowCooldown; break;
            case 1: cooldownDuration = middleRowCooldown; break;
            case 2: cooldownDuration = backRowCooldown; break;
        }

        if (cooldownDuration > 0)
        {
            yield return new WaitForSeconds(cooldownDuration);
        }

        isCoolingDown = false;
    }

    /// <summary>
    /// 指定された時間、キャラクターをスタン状態にするコルーチン。
    /// </summary>
    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        if (aimObject != null) aimObject.SetActive(false); // 照準をオフ

        // 実行中のフラッシュがあれば停止し、新しいフラッシュを開始
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashColorOnceCoroutine());

        // 指定時間待機
        yield return new WaitForSeconds(duration);

        // --- スタン終了処理 ---
        spriteRenderer.color = originalColor; // 念のため色を元に戻す
        if (aimObject != null) aimObject.SetActive(true);  // 照準をオン
        isStunned = false;
    }

    /// <summary>
    /// スタン時にキャラクターを一瞬だけ赤くする演出コルーチン。
    /// </summary>
    private IEnumerator FlashColorOnceCoroutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f); // 赤色でいる時間
        spriteRenderer.color = originalColor;
        flashCoroutine = null; // 自身の処理が終わったので参照をクリア
    }

    /// <summary>
    /// 現在のY座標に基づいてスケールを計算します。
    /// </summary>
    private Vector3 CalculateScaleForRow(int y)
    {
        float t = y / 2.0f; // Y座標(0,1,2)を割合(0, 0.5, 1)に変換
        float scaleMultiplier = Mathf.Lerp(frontRowScale, backRowScale, t);
        return originalScale * scaleMultiplier;
    }

    /// <summary>
    /// キャラクターのスケールを即時更新します。
    /// </summary>
    private void UpdateScaleBasedOnRow(int y)
    {
        transform.localScale = CalculateScaleForRow(y);
    }
    #endregion
}