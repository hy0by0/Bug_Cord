using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3x3のグリッド（マス目）上を移動するキャラクターを制御するためのスクリプトです。
/// プレイヤーのキー入力に応じて、キャラクターの論理的な位置（座標）を管理し、
/// 見た目の上では、位置と大きさが滑らかに変化する移動を表現します。
/// </summary>
public class GridCharacterController : MonoBehaviour
{
    // --- インスペクター（Unityエディタ）で調整する公開変数 ---

    [Header("■ グリッドと移動設定")]
    [Tooltip("シーンに配置した9個の移動先マーカー（Transform）を、手前左(0,0)から奥右(2,2)の順で設定してください")]
    public List<Transform> positionMarkers; // キャラクターが移動する目標地点のリスト

    [Tooltip("1マス移動するのにかかる時間（秒）です。小さい値ほど速く移動します")]
    public float moveDuration = 0.3f; // 移動と拡縮のアニメーションにかかる時間

    [Header("■ 遠近法（パース）の設定")]
    [Tooltip("一番手前のマス（Y=0）での大きさの倍率です。1.0でキャラクターの元のサイズ（100%）になります")]
    public float frontRowScale = 1.0f; // 手前のマスでのスケール倍率

    [Tooltip("一番奥のマス（Y=2）での大きさの倍率です。0.7でキャラクターの元のサイズの70%になります")]
    public float backRowScale = 0.7f; // 奥のマスでのスケール倍率

    [Header("■ 開始位置")]
    [Tooltip("ゲーム開始時のX座標です（0:左, 1:中央, 2:右）")]
    public int startX = 1;

    [Tooltip("ゲーム開始時のY座標です（0:手前, 1:中央, 2:奥）")]
    public int startY = 0;


    // --- プログラム内部で管理する非公開変数 ---

    // インスペクターで設定された1次元リストを、[x, y]形式で簡単にアクセスできるように変換して格納する2次元配列
    private Transform[,] gridPositions = new Transform[3, 3];

    // キャラクターの「論理的な」現在位置を記憶する変数。見た目ではなく、データ上の位置
    private int currentX;
    private int currentY;

    // キャラクターが移動アニメーションの再生中かどうかを判定するフラグ。移動中に新たな入力を受け付けないようにする役割
    private bool isMoving = false;

    // 遠近法を適用するための、キャラクターの「本来の大きさ」を記憶しておく変数
    private Vector3 originalScale;


    /// <summary>
    /// ゲームが開始した時に、一番最初に一度だけ実行される初期化処理です。
    /// </summary>
    void Start()
    {
        // (1) 基準サイズの記憶：
        // このスクリプトが適用された瞬間のキャラクターの大きさを「基準」として記憶します。
        // これにより、Unityエディタで設定した見た目のサイズがそのまま基準になります。
        originalScale = transform.localScale;

        // (2) マーカーの整理：
        // インスペクターから設定された1次元リスト(positionMarkers)を、プログラムで扱いやすい2次元配列(gridPositions)に変換します。
        if (positionMarkers.Count == 9)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    // 例: [x=0, y=1] の場合、リストの 1*3+0 = 3番目の要素が対応
                    gridPositions[x, y] = positionMarkers[y * 3 + x];
                }
            }
        }
        else
        {
            // マーカーの設定が不正な場合は、エラーを表示して、このスクリプトが動かないようにします。
            Debug.LogError("エラー: positionMarkersに9個のTransformが設定されていません！インスペクターを確認してください。");
            enabled = false; // このコンポーネントを無効化
            return;
        }

        // (3) 初期位置の設定：
        // インスペクターで設定された開始座標を、キャラクターの論理的な現在地として設定します。
        currentX = startX;
        currentY = startY;

        // キャラクターのゲームオブジェクトを、対応するマーカーの物理的な位置へ瞬間移動させます。
        transform.position = gridPositions[currentX, currentY].position;

        // Y座標（奥行き）に応じて、キャラクターの初期サイズを計算し、適用します。
        float initialScaleMultiplier = Mathf.Lerp(frontRowScale, backRowScale, currentY / 2.0f);
        transform.localScale = originalScale * initialScaleMultiplier;
    }

    /// <summary>
    /// 毎フレーム実行される、ゲーム中のメインループ処理です。
    /// </summary>
    void Update()
    {
        // もしキャラクターが移動アニメーションの再生中なら、新しいキー入力を受け付けずに、このフレームの処理を終了します。
        if (isMoving)
        {
            return;
        }

        // 上下左右のキーが「押された瞬間」を検知します。
        // GetKeyDownを使うことで、キーを押しっぱなしにしても1回しか移動しないようになります。
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            AttemptMove(0, 1); // Y+1方向（奥）への移動を試みる
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            AttemptMove(0, -1); // Y-1方向（手前）への移動を試みる
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AttemptMove(-1, 0); // X-1方向（左）への移動を試みる
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            AttemptMove(1, 0); // X+1方向（右）への移動を試みる
        }
    }

    /// <summary>
    /// 指定された方向への移動を試みます。移動可能であれば、移動アニメーションを開始します。
    /// </summary>
    /// <param name="xOffset">X方向の移動量（-1, 0, 1）</param>
    /// <param name="yOffset">Y方向の移動量（-1, 0, 1）</param>
    private void AttemptMove(int xOffset, int yOffset)
    {
        // (1) 移動先の計算：
        // 現在の論理座標に、入力された移動量を足して、目標となる座標を計算します。
        int targetX = currentX + xOffset;
        int targetY = currentY + yOffset;

        // (2) 境界チェック：
        // 目標の座標がグリッドの範囲内（0〜2）に収まっているかを確認します。
        if (targetX >= 0 && targetX < 3 && targetY >= 0 && targetY < 3)
        {
            // (3) 移動処理の開始：
            // 範囲内であれば、キャラクターの論理的な現在座標を、目標座標で更新します。
            currentX = targetX;
            currentY = targetY;

            // 移動先のY座標に基づいて、目標となる大きさを計算します。
            float targetScaleMultiplier = Mathf.Lerp(frontRowScale, backRowScale, targetY / 2.0f);
            Vector3 targetScale = originalScale * targetScaleMultiplier;

            // 見た目を滑らかに動かすための「コルーチン」を開始します。
            // 引数として、移動先の物理的な「位置」と、最終的な「大きさ」を渡します。
            StartCoroutine(MoveCoroutine(gridPositions[currentX, currentY].position, targetScale));
        }
        // 範囲外への移動しようとした場合は、このメソッドは何もせずに終了します。
    }

    /// <summary>
    /// 目標地点までキャラクターの見た目をスムーズに動かす、中断・再開が可能な特殊な関数（コルーチン）です。
    /// </summary>
    /// <param name="targetPosition">移動先のワールド座標</param>
    /// <param name="targetScale">移動後の最終的な大きさ</param>
    private IEnumerator MoveCoroutine(Vector3 targetPosition, Vector3 targetScale)
    {
        // --- 移動開始の準備 ---
        isMoving = true; // 「移動中」フラグを立て、新たなキー入力をブロックします。

        float elapsedTime = 0f; // 移動開始からの経過時間を記録するタイマー変数
        Vector3 startPosition = transform.position; // 移動開始時の位置を記録
        Vector3 startScale = transform.localScale; // 移動開始時の大きさを記録

        // --- 移動中のループ処理 ---
        // 経過時間が、指定した移動時間（moveDuration）に達するまで、毎フレームこのループを実行します。
        while (elapsedTime < moveDuration)
        {
            // 経過時間に、1フレーム分の時間を加算します。
            elapsedTime += Time.deltaTime;

            // 移動の進捗度合いを計算します（0.0で開始、1.0で完了）。
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // 進捗度tを、緩急のついた滑らかな値(easedT)に変換します（イージング）。
            float easedT = t * t * (3f - 2f * t); // スムーズステップという計算式

            // Lerp（線形補間）を使い、開始点と終了点の間を、計算した進捗度(easedT)の分だけ進めます。
            // 位置と大きさに全く同じ進捗度を使うことで、2つの動きが完全に同期します。
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, easedT);

            // ここで一旦処理を中断し、Unityに制御を戻します。次のフレームでwhileループの先頭から処理を再開します。
            yield return null;
        }

        // --- 移動完了後の仕上げ ---
        // ループを抜けたら、計算誤差をなくすために、位置と大きさを目標値にぴったりと合わせます。
        transform.position = targetPosition;
        transform.localScale = targetScale;

        isMoving = false; // 「移動中」フラグを下ろし、次のキー入力を受け付けられるようにします。
    }
}