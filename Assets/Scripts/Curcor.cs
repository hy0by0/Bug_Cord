using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// マウスカーソルやコントローラーの入力に応じて照準を動かし、攻撃処理を行うクラス。
/// タイトル画面とゲーム画面で、ボタンを押した際の挙動が変わる。
/// </summary>
public class Curcor : MonoBehaviour
{
    // --- 未使用の変数 ---
    // private Vector3 screenPoint;
    // private Vector3 offset;
    // public bool ClickPosition = false;

    [Header("■ 攻撃関連")]
    [Tooltip("攻撃時に有効化する当たり判定オブジェクト")]
    public GameObject hitbox;
    [Tooltip("一度攻撃してから、次に攻撃できるまでの基本時間（秒）")]
    public float attackCooldown = 1.0f;
    [Tooltip("攻撃速度バフがかかった時の攻撃間隔の倍率（例: 0.5にすると2倍速）")]
    public float buffedAttackSpeedMultiplier = 0.5f;

    [Header("■ コントローラー設定")]
    [Tooltip("コントローラー使用時の照準の移動速度")]
    public float moveSpeed = 5f;

    // --- 内部で管理する変数 ---
    // 攻撃の可否を管理するフラグ
    private bool shoot_reload_Flag = true; // trueの時だけ攻撃できる
    // private bool goAtack = false; // 未使用
    // private bool shoot_allow = true; // 未使用

    // 現在適用されている攻撃間隔の倍率（通常時は1.0）
    private float currentAttackSpeedMultiplier = 1.0f;

    private Coroutine attackSpeedRoutine; // 攻撃速度バフのコルーチンを管理する変数

    /// <summary>
    /// このオブジェクトが有効になった（再表示された）時に毎回呼ばれるメソッド
    /// </summary>
    void OnEnable()
    {
        // 攻撃関連のフラグを初期状態（攻撃可能）にリセットする
        shoot_reload_Flag = true;
        currentAttackSpeedMultiplier = 1.0f; // 念のためバフもリセット
    }


    /// <summary>
    /// ゲーム開始時に一度だけ呼ばれる初期化処理
    /// </summary>
    void Start()
    {
        // カーソルの初期位置を画面中央（X:0, Y:0, Z:0）に設定
        transform.position = Vector3.zero;
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// </summary>
    void Update()
    {
        // --- 照準の移動処理 ---

        // コントローラーの左スティックの入力を取得 (-1.0 ~ 1.0)
        float LX = Input.GetAxis("Horizontal");
        float LY = Input.GetAxis("Vertical");
        Vector3 stickInput = new Vector3(LX, LY, 0);

        // マウスのスクリーン座標を取得
        Vector3 thisPosition = Input.mousePosition;
        // スクリーン座標をゲーム内のワールド座標に変換
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(thisPosition);
        worldPosition.z = 0f; // Z座標は0に固定

        // コントローラーが接続されているかチェック
        if (Input.GetJoystickNames().Length > 0 && !string.IsNullOrEmpty(Input.GetJoystickNames()[0]))
        {
            // スティックが一定以上傾いている場合のみ移動
            if (stickInput.magnitude > 0.1f)
            {
                transform.position += stickInput * moveSpeed * Time.deltaTime;
            }
        }
        else // コントローラーが接続されていなければマウスで操作
        {
            this.transform.position = worldPosition;
        }

        // --- 攻撃入力処理 ---

        // マウスの左クリック、またはコントローラーのAボタンが押され、かつ攻撃可能な状態なら
        if ((Input.GetMouseButtonDown(0) || Input.GetButton("btnA")) && shoot_reload_Flag)
        {
            // すぐにフラグをfalseにして、連続で攻撃できないようにする
            shoot_reload_Flag = false;

            // 現在のシーンがゲーム画面（SampleScene）の場合
            if (SceneManager.GetActiveScene().name == "SampleScene" || SceneManager.GetActiveScene().name == "Main")
            {
                // 攻撃アニメーションのタイミングに合わせて当たり判定を有効化・無効化
                Invoke(nameof(ActivateHitbox), 0.2f);   // 0.2秒後に当たり判定をオン
                Invoke(nameof(DeactivateHitbox), 0.25f); // 0.25秒後に当たり判定をオフ

                // 攻撃後のクールダウンタイマーを開始
                StartCoroutine(AttackCooldownCoroutine());
            }

            // 現在のシーンがタイトル画面（Title）の場合
            if (SceneManager.GetActiveScene().name == "Title")
            {
                // フェードアウト処理を実行し、2秒後にゲームシーンへ遷移
                ScreenFader screenFader = FindObjectOfType<ScreenFader>();
                if (screenFader != null)
                {
                    StartCoroutine(screenFader.BackBlack());
                }
                Invoke(nameof(GoGame), 2.0f);
            }
        }
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


}