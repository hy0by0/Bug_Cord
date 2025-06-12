using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵に関する処理全般を担うスクリプト。以下に２つのクラスがある。

// 敵の行動パターンの一覧のenum ここに追加したりしてください。これは実装しきれませんでした。

public enum EnemyState
{
    Normal,
    Stan,
    Alert,
    Dead,
    Attack,
    Cooldown,
    Special
}


// 出現させるQRヒントとそれに対応する当たり判定オブジェクトを格納する場所 inspector上に追加・変更をしていってください。
[System.Serializable]
public class QRHintPair
{
    public string pairID;  // 呼びだすQRヒントと当たり判定オブジェクトのペアの識別用変数
    public GameObject QRObject;         // 出現させるQRコードオブジェクト
    public GameObject hitboxObject;       // 出現させる当たり判定
    public Transform spawnPoint;          // 出現させる位置（Transform参照）
}



// EnemyHitAreaスクリプトからダメージを受け取る。HPに関してはUIManagerクラスを呼びだして変更を反映させたりもしている。
public class EnemyController : MonoBehaviour
{

    private Animator animator; // Animatorコンポーネントを取得するための変数

    private SpriteRenderer spriteRenderer; // オブジェクトの画像コンポーネントを保持するための変数
    private Color originalColor;

    private Coroutine flashCoroutine;    // 実行中のコルーチンを保持するための変数

    public int enemy_hp = 10000; //敵の最大ＨＰを入力してね
    [SerializeField] UIManager uimanager; //UIManagerクラスのメソッドを呼びだしたいため
    public static string enemy_state = "Normal"; //敵の状態。普通、ひより、死亡、の３段階は最低限考えたい。

    public int count_hit = 0; //受けた攻撃回数をカウントするための変数。攻撃回数を条件にした処理を起こすため。
    public int count_hit_QRFlag = 5; //QRヒントが出てくるまでの攻撃回数。攻撃回数がこれに達するとＱＲとクリティカルが発生するようにする

    public List<QRHintPair> spawnPairs;     // 複数の「QRコード＋当たり判定オブジェクト」の組
    public float activeDuration_QR = 5f;        // QRヒントが出現している時間
    public float activeDuration_HitBox = 8f;        // クリティカル当たり判定が出現している時間
    private bool hasSpawned = false;         // 一度だけ実行されるように制御 (連続で発生してしまわないようにするために必要)
    

    // Start is called before the first frame update
    void Start()
    {
        //UIManagerという名前のオブジェクトから取得してもらう。
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();// オブジェクトの画像コンポーネントを取得(攻撃時に赤色へ敵を変えるため)
        originalColor = spriteRenderer.color;


        //UIManagerへ最大ＨＰの値を反映させる(比率を合わせるために)
        uimanager.enemy_hp_max = enemy_hp;
        uimanager.enemy_hp_remain = enemy_hp;

        // 受けた攻撃回数の初期化
        count_hit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSpawned && count_hit >= count_hit_QRFlag)
        {
            //呼びだしたいQRヒントの組み合わせをメソッドから呼びだす。特定のタイミングで呼びだしたいときもこの記述を使ってください。
            SpawnPairByID("Test_pattern");
        }

        if (enemy_hp <= 0)
        {
            enemy_state = "Dead";
        }
    }

    // 特定ＩＤのＱＲヒントの組み合わせを出現させることを要請するメソッド
    public void SpawnPairByID(string id)
    {
        if (hasSpawned) return;

        QRHintPair targetPair = spawnPairs.Find(p => p.pairID == id);

        if (targetPair != null)
        {
            // 以下のコルーチンを呼びだし、特定の(ID:targetPair)QRと当たり判定の出現処理を実行させる
            StartCoroutine(SpawnQRAndHitboxCoroutine(targetPair));
        }
        else
        {
            Debug.LogWarning($"ID '{id}' のペアが見つかりませんでした。");
        }
    }

    // 入力されたペアのQRコードと当たり判定オブジェクトを出現させるコルーチン
    private IEnumerator SpawnQRAndHitboxCoroutine(QRHintPair pair)
    {
        hasSpawned = true;
        if (pair.QRObject != null && pair.hitboxObject != null && pair.spawnPoint != null)
        {
            // 指定位置に移動＆表示
            pair.QRObject.transform.position = pair.spawnPoint.position;
            //pair.hitboxObject.transform.position = pair.spawnPoint.position;

            pair.QRObject.SetActive(true);
            pair.hitboxObject.SetActive(true);
        }

        yield return new WaitForSeconds(activeDuration_QR);

        // 一定時間後にＱＲヒントを非表示
        if (pair.QRObject != null)
        {
            pair.QRObject.SetActive(false);
        }

        // 一定時間後にクリティカル当たり判定を非表示
        yield return new WaitForSeconds(activeDuration_HitBox - activeDuration_QR);

        if (pair.QRObject != null)
        {
            pair.hitboxObject.SetActive(false);
        }

        // 以上の全てが終わった後、ヒットカウントをリセット

        count_hit = 0;
        hasSpawned = false; // 次の出現に備える
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


    //以下、あたった部位ごとに受ける処理を変えるための処理群。必要に応じてメソッドを変更したり、コピペして追加してください。
    // 防御が硬いところにあたった時に呼び出す
    public void HitResist(int playerAtackdamage)
    {
        Debug.Log("効果はいまひとつ");

        //受けるダメージを計算し、敵のＨＰとＵＩに反映
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 0.5f); //ダメージ式は、ここを変更してください。
        enemy_hp -= enemy_damaged;
        //UIのＨＰバーにUIManagerのメソッドを呼び出すことで反映させる。受けたダメージ量を入力
        uimanager.DamagedBar(enemy_damaged);

        //攻撃回数をカウントしておく
        count_hit ++;
        
    }

    // 普通の当たり判定のところにあたった時に呼び出す
    public void HitNormal(int playerAtackdamage)
    {
        Debug.Log("ダメージが入った");

        if (flashCoroutine != null)        // もし既に色の変更コルーチンが実行中なら、それを停止する
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashColorCoroutine());        // 新しく色の変更コルーチンを開始し、その情報を変数に保存する

        //受けるダメージを計算し、敵のＨＰとＵＩに反映
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.0f); //ダメージ式は、ここを変更してください。
        enemy_hp -= enemy_damaged;
        //UIのＨＰバーにUIManagerのメソッドを呼び出すことで反映させる。受けたダメージ量を入力
        uimanager.DamagedBar(enemy_damaged);

        //攻撃回数をカウントしておく
        count_hit ++;
    }

    // 攻撃が通りやすい当たり判定のところにあたった時に呼び出す
    public void HitWeak(int playerAtackdamage)
    {
        Debug.Log("急所に当たった");

        //受けるダメージを計算し、敵のＨＰとＵＩに反映
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.5f); //ダメージ式は、ここを変更してください。
        enemy_hp -= enemy_damaged;
        //UIのＨＰバーにUIManagerのメソッドを呼び出すことで反映させる。受けたダメージ量を入力
        uimanager.DamagedBar(enemy_damaged);

        //攻撃回数をカウントしておく
        count_hit ++;
    }



    // 敵の致命的弱点当たり判定のところにあたった時に呼び出す。
    // ヒットした時、敵を「ひるみ状態」へ遷移させる。
    public void HitCritical(int playerAtackdamage)
    {
        Debug.Log("効果は抜群だ！今がチャンスだ！");

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Critical"); // "Trigger"にはパラメータ名が入ります

        //受けるダメージを計算し、敵のＨＰとＵＩに反映
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 2.0f); //ダメージ式は、ここを変更してください。
        enemy_hp -= enemy_damaged;
        //UIのＨＰバーにUIManagerのメソッドを呼び出すことで反映させる。受けたダメージ量を入力
        uimanager.DamagedBar(enemy_damaged);

        // 敵の状態を「ひるみ状態」へ遷移させるための処理を以下に。
        enemy_state = "stan"; //まだ未実装。
    }


    
}