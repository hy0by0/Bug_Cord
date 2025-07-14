using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

/// <summary>
/// マウスカーソルやコントローラーの入力に応じて照準を動かし、攻撃処理を行うクラス。
/// タイトル画面とゲーム画面で、ボタンを押した際の挙動が変わる。（タイトル画面では、シーン遷移を行う処理を行う）
/// （New Input System 対応版 / 攻撃クールダウン＆速度バフ機能付き）
/// </summary>
public class Curcor : MonoBehaviour
{
    // --- 未使用の変数 ---
    // private Vector3 screenPoint;
    // private Vector3 offset;
    // public bool ClickPosition = false;

    // 1=Player1, 2=Player2
    public int playerNumber = 1; // 1=Player1, 2=Player2
    public GameObject hitbox;

    public bool ClickPosition = false;
    public bool goAtack = false;
    public bool shoot_reload_Flag = true;
    public bool shoot_allow = true;

    float moveSpeed = 50f;

    // --- シーン遷移関連---
    //遷移フラグ
    private static bool isSceneTransitioning = false;
    // タイトル画面で入力後、次のシーンへ遷移させるまでの待機時間
    public float time_wait_sceneChange = 0.7f;

    // --- 攻撃関連 ---
    // 攻撃時に有効化する当たり判定オブジェクト
    // public GameObject hitbox;   // ←重複するので上に統合
    // 一度攻撃してから、次に攻撃できるまでの基本時間（秒）
    public float attackCooldown = 1.0f;
    // 攻撃速度バフがかかった時の攻撃間隔の倍率（例: 0.5にすると2倍速）
    public float buffedAttackSpeedMultiplier = 0.5f;

    // --- 内部で管理する変数 ---
    // 攻撃の可否を管理するフラグ
    // private bool shoot_reload_Flag = true; // ←重複するので上に統合
    // private bool goAtack = false;          // 未使用
    // private bool shoot_allow = true;       // 未使用

    // 現在適用されている攻撃間隔の倍率（通常時は1.0）
    private float currentAttackSpeedMultiplier = 1.0f;

    private Coroutine attackSpeedRoutine; // 攻撃速度バフのコルーチンを管理する変数

    private NewActions inputActions;
    private Vector2 stickInput;

    // ★ 新たに追加：共通で扱う InputAction 参照
    private InputAction cursorAction;
    private InputAction shotAction;

    // =================== Unity Callback ===================
    private void Start()
    {
        int connectedGamepads = Gamepad.all.Count;
        Debug.Log("接続されているゲームパッドの数: " + connectedGamepads);

        // カーソルの初期位置を画面中央（X:0, Y:0, Z:0）に設定
        transform.position = Vector3.zero;
    }

    private void OnCursorPerformed(InputAction.CallbackContext ctx)
    {
        stickInput = ctx.ReadValue<Vector2>();
    }

    private void OnCursorCanceled(InputAction.CallbackContext ctx)
    {
        stickInput = Vector2.zero;
    }

    private void OnShotPerformed(InputAction.CallbackContext ctx)
    {
        OnAttack();
    }

    void Awake()
    {
        inputActions = new NewActions();

        // ゲームパッドを取得
        if (Gamepad.all.Count >= playerNumber)
        {
            var gamepad = Gamepad.all[playerNumber - 1];

            // InputUserとGamepadを紐づけ
            var user = InputUser.CreateUserWithoutPairedDevices();
            user.AssociateActionsWithUser(inputActions);
            InputUser.PerformPairingWithDevice(gamepad, user);
        }

        // ------------------------------- //
        // ★ if / else でマップを分岐させる
        // ------------------------------- //
        if (playerNumber == 1)
        {
            inputActions.Player.Enable();

            cursorAction = inputActions.Player.Cursor;
            shotAction = inputActions.Player.Shot;
        }
        else // playerNumber == 2 以降
        {
            inputActions.Player2.Enable();

            cursorAction = inputActions.Player2.Cursor;
            shotAction = inputActions.Player2.Shot;
        }

        // 共通の InputAction 型でハンドラ登録
        cursorAction.performed += OnCursorPerformed;
        cursorAction.canceled += OnCursorCanceled;
        shotAction.performed += OnShotPerformed;
    }

    /// <summary>
    /// このオブジェクトが有効になった（再表示された）時に毎回呼ばれるメソッド
    /// </summary>
    void OnEnable()
    {
        // 攻撃関連のフラグを初期状態（攻撃可能）にリセットする
        shoot_reload_Flag = true;
        currentAttackSpeedMultiplier = 1.0f; // 念のためバフもリセット
    }

    void Update()
    {
        // --- 照準の移動処理 (Gamepad のみ) ---
        if (stickInput.magnitude > 0.1f)
        {
            transform.position += (Vector3)stickInput * moveSpeed * Time.deltaTime;
        }
        // ※ マウス操作は不要のため削除

        //ゲーム終了コマンド
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnAttack()
    {
        if (!shoot_allow) return;
        if (!shoot_reload_Flag) return; // クールダウン中は攻撃不可

        shoot_reload_Flag = false; // 連続攻撃防止！

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"[{playerNumber}] 自身のGameObjectが非アクティブのため、アクティブにします: {gameObject.name}");
            gameObject.SetActive(true);
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "SampleScene" || currentScene == "Main")
        {
            // 攻撃アニメーションのタイミングに合わせて当たり判定を有効化・無効化
            Invoke(nameof(ActivateHitbox), 0.2f);   // 0.2秒後に当たり判定をオン
            Invoke(nameof(DeactivateHitbox), 0.25f); // 0.25秒後に当たり判定をオフ

            // 攻撃後のクールダウンタイマーを開始
            StartCoroutine(AttackCooldownCoroutine());
        }
        else if (currentScene == "Title")
        {
            if (isSceneTransitioning) return;

            isSceneTransitioning = true;

            var screenFader = FindObjectOfType<ScreenFader>();
            if (screenFader != null) StartCoroutine(screenFader.BackBlack());

            StartCoroutine(GoGameAfterDelay(time_wait_sceneChange)); // タイトルシーンで入力後、何秒後にシーン遷移させるかを設定
        }

        Debug.Log("攻撃が当たりました: " + playerNumber);
    }

    // 攻撃判定オブジェクトを有効化する
    private void ActivateHitbox()
    {
        if (hitbox != null) hitbox.SetActive(true);
    }

    // 攻撃判定オブジェクトを無効化する
    private void DeactivateHitbox()
    {
        if (hitbox != null) hitbox.SetActive(false);
    }

    // ゲームシーンに遷移する
    private void GoGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator HitboxRoutine()
    {
        if (hitbox != null)
        {
            Debug.Log("hitbox ON: " + gameObject.name);
            hitbox.SetActive(true);
        }
        else
        {
            Debug.LogWarning("hitbox が設定されていません: " + gameObject.name);
        }

        yield return new WaitForSeconds(0.1f);

        if (hitbox != null)
        {
            hitbox.SetActive(false);
            Debug.Log("hitbox OFF: " + gameObject.name);
        }

        Debug.Log("超えました ");
    }

    IEnumerator GoGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this != null) SceneManager.LoadScene("scenario"); //Titleシーンからの遷移先のシーン名に設定
    }

    // 攻撃のクールダウンを管理するコルーチン
    private IEnumerator AttackCooldownCoroutine()
    {
        // 「基本クールダウン × 現在の速度倍率」秒だけ待機する
        yield return new WaitForSeconds(attackCooldown * currentAttackSpeedMultiplier);
        // 再び攻撃できるようにフラグをtrueに戻す
        shoot_reload_Flag = true;
    }

    /// <summary>
    /// 攻撃速度バフを強制的に解除し、通常状態に戻す
    /// </summary>
    public void ResetAttackBuff()
    {
        if (attackSpeedRoutine != null)
        {
            StopCoroutine(attackSpeedRoutine);
            attackSpeedRoutine = null;
        }
        currentAttackSpeedMultiplier = 1.0f;
        Debug.Log("攻撃速度バフがリセットされました。");
    }

    // --- アイテム効果を受け取るためのメソッド群 ---

    /// <summary>
    /// 【特攻マシンガン用】一定時間、攻撃速度を上昇させる効果を適用します。
    /// </summary>
    public void ApplyAttackSpeedBuff(float duration)
    {
        // もし既に実行中なら、古いタイマーを停止
        if (attackSpeedRoutine != null) StopCoroutine(attackSpeedRoutine);
        // 新しいタイマーを開始し、その参照を保存
        attackSpeedRoutine = StartCoroutine(AttackSpeedRoutine(duration));
    }

    /// <summary>
    /// 攻撃速度上昇効果のタイマー処理を行うコルーチン
    /// </summary>
    private IEnumerator AttackSpeedRoutine(float duration)
    {
        // 攻撃間隔の倍率を、バフ中の値（例: 0.5）に変更
        currentAttackSpeedMultiplier = buffedAttackSpeedMultiplier;
        Debug.Log($"特攻マシンガン効果！ 攻撃間隔が{currentAttackSpeedMultiplier}倍に！");

        // 効果時間だけ待機
        yield return new WaitForSeconds(duration);

        // 倍率を通常の値(1.0f)に戻す
        currentAttackSpeedMultiplier = 1.0f;
        Debug.Log("攻撃速度アップ効果、終了。");
    }

    void OnDestroy()
    {
        // --- コールバック解除 ---
        if (cursorAction != null)
        {
            cursorAction.performed -= OnCursorPerformed;
            cursorAction.canceled -= OnCursorCanceled;
        }
        if (shotAction != null)
        {
            shotAction.performed -= OnShotPerformed;
        }

        // --- マップ無効化 ---
        if (inputActions != null)
        {
            if (playerNumber == 1) inputActions.Player.Disable();
            else inputActions.Player2.Disable();
        }
    }
}