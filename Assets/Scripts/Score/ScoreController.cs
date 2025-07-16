using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("スコアをカウントする変数")]
    public static int m_CountScorePlayer;
    [Header("Boyスコアをカウントする変数")]
    public static int m_BoyCountScore;
    [Header("Girlスコアをカウントする変数")]
    public static int m_GirlCountScore;

    [Header("スコアの画像を制御するスクリプト(Boy)")]
    public ScoreImage m_BoyScoreImage;
    [Header("スコアの画像を制御するスクリプト(Girl)")]
    public ScoreImage m_GirlScoreImage;

    [Header("時間を加算するために必要なスクリプト")]
    public GlobalTimeControl m_CountDownClock;

    public static int m_Time; // ← 追加

    private void Awake()
    {
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
    }

    private void Start()
    {
        m_CountScorePlayer = 0;
        m_BoyCountScore = 0;
        m_GirlCountScore = 0;
        m_Time = 0;

        m_BoyScoreImage.ShowNumber(m_CountScorePlayer);
        m_GirlScoreImage.ShowNumber(m_CountScorePlayer);
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
        m_BoyScoreImage.ShowNumber(m_BoyCountScore);
        m_GirlScoreImage.ShowNumber(m_GirlCountScore);
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
        m_BoyScoreImage.ShowNumber(m_BoyCountScore);
        m_GirlScoreImage.ShowNumber(m_GirlCountScore);
    }

    public void FinalScore()
    {
        m_Time = 100; // 必要ならタイマーから取得する
        Debug.Log($"FinalScore: Damage:{m_CountScorePlayer} Time:{m_Time}");
    }
}