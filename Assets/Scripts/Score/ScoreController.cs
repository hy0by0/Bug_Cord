using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    [Header("現在の合計スコア(表示専用)")]
    [SerializeField] private float inspector_TotalScore;


    [Header("スコアをカウントする変数")]
    public static float m_CountScorePlayer;
    [Header("Boyスコアをカウントする変数")]
    public static float m_BoyCountScore;
    [Header("Girlスコアをカウントする変数")]
    public static float m_GirlCountScore;

    [Header("スコアの画像を制御するスクリプト(Boy)")]
    public ScoreImage m_BoyScoreImage;
    [Header("スコアの画像を制御するスクリプト(Girl)")]
    public ScoreImage m_GirlScoreImage;

    [Header("時間を加算するために必要なスクリプト")]
    public GlobalTimeControl m_CountDownClock;

    public static int m_Time; // ← 追加

    private static bool created = false;

    private static ScoreController instance;

    private void Update()
    {
        inspector_TotalScore = m_BoyCountScore + m_GirlCountScore;
    }
    private void OnEnable()
    {
        // シーン切り替え後に呼ばれるイベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 登録解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // オブジェクト名で探す
        var boyObj = GameObject.Find("BoyScore");
        var girlObj = GameObject.Find("GirlScore");

        if (boyObj != null)
            m_BoyScoreImage = boyObj.GetComponent<ScoreImage>();
        else
            Debug.LogError("BoyScore オブジェクトが見つかりません");

        if (girlObj != null)
            m_GirlScoreImage = girlObj.GetComponent<ScoreImage>();
        else
            Debug.LogError("GirlScore オブジェクトが見つかりません");

        // タイマーは型で探す
        m_CountDownClock = FindObjectOfType<GlobalTimeControl>();
        if (m_CountDownClock == null)
            Debug.LogError("GlobalTimeControl が再取得できませんでした");

        // シーン切り替え後のスコア再表示
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }
    private void Awake()
    {
        if (instance == null)
        {
            // 最初にロードされたインスタンス
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // 既にインスタンスが存在していたら自分自身を破棄
            Destroy(gameObject);
            return;
        }

        if (m_BoyScoreImage == null) m_BoyScoreImage = GetComponent<ScoreImage>();
        if (m_GirlScoreImage == null) m_GirlScoreImage = GetComponent<ScoreImage>();
        if (m_CountDownClock == null) m_CountDownClock = GetComponent<GlobalTimeControl>();

        if (m_BoyScoreImage == null || m_GirlScoreImage == null)
        {
            Debug.LogError("ScoreImage が設定されていません");
        }
        else if (m_CountDownClock == null)
        {
            Debug.LogError("CountDownClock が設定されていません");
        }

        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    private void Start()
    {
        m_CountScorePlayer = 0;
        m_BoyCountScore = 0;
        m_GirlCountScore = 0;
        m_Time = 0;

        m_BoyScoreImage.ShowNumber((int)m_CountScorePlayer);
        m_GirlScoreImage.ShowNumber((int)m_CountScorePlayer);
    }

    public void PlusScore(int damage, PlayerID playerID)
    {
        if (playerID == PlayerID.P2)
        {
            m_BoyCountScore += damage;
        }
        else if (playerID == PlayerID.P1)
        {
            m_GirlCountScore += damage;
        }

        m_CountScorePlayer = m_BoyCountScore + m_GirlCountScore;

        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} 合計:{m_CountScorePlayer}");
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }

    public void MinusScore(int damage, PlayerID playerID)
    {
        if (playerID == PlayerID.P2)
        {
            m_BoyCountScore -= damage;
        }
        else if (playerID == PlayerID.P1)
        {
            m_GirlCountScore -= damage;
        }

        m_CountScorePlayer = m_BoyCountScore + m_GirlCountScore;

        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} 合計:{m_CountScorePlayer}");
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }

    public void FinalScore()
    {
        m_Time = (int)m_CountDownClock.globalTimeInSeconds * 100; // 必要ならタイマーから取得する
        Debug.Log($"FinalScore: Damage:{m_CountScorePlayer} Time:{m_Time}");
    }
}
