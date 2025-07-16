using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [Header("ダメージ")]
    public ScoreCountUP m_DmageScoreCountUP;
    [Header("タイム")]
    public ScoreCountUP m_TimeScoreCountUP;
    [Header("トータル")]
    public ScoreCountUP m_TotalScoreCountUP;
    [Header("1st")]
    public ScoreCountUP m_1stScoreCountUP;
    [Header("2nd")]
    public ScoreCountUP m_2ndScoreCountUP;

    //スコアに使う変数
    int m_DamageScore;
    int m_TimeScore;
    int m_TotalScore;
    int m_1stScore;
    int m_2ndScore;

    private void Start()
    {
        m_DamageScore = PlayerPrefs.GetInt("DamageScore");

        //ScoreControllerスクリプトのTimeが不明なためコメント化
        m_TimeScore=PlayerPrefs.GetInt("TimeScore");

        if (m_1stScore > 0)
            m_1stScore = PlayerPrefs.GetInt("1stScore",0);

        if (m_2ndScore > 0)
            m_2ndScore = PlayerPrefs.GetInt("2ndScore",0);

        StartCoroutine(IndicationScore());
    }

    /// <summary>
    /// スコア表示
    /// </summary>
    IEnumerator IndicationScore()
    {
        yield return new WaitForSeconds(1.5f);
        //Time 表示
        m_TimeScoreCountUP.StartResult(m_TimeScore);
        yield return new WaitForSeconds(1f);

        //Damage 表示
        m_DmageScoreCountUP.StartResult(m_DamageScore);
        yield return new WaitForSeconds(1.5f);

        //Total 計算して表示
        m_TotalScore = m_TimeScore + m_DamageScore;
        m_TotalScoreCountUP.StartResult(m_TotalScore);
        yield return new WaitForSeconds(3f);

        //ランキング比較・更新
        Comparison();

        //ランキングを表示
        m_1stScoreCountUP.StartResult(m_1stScore);
        yield return new WaitForSeconds(0.5f);
        m_2ndScoreCountUP.StartResult(m_2ndScore);
    }

    /// <summary>
    /// ランキングの比較と更新
    /// </summary>
    void Comparison()
    {
        if (m_1stScore == 0)
        {
            // 1st 空 → 登録
            m_1stScore = m_TotalScore;
            PlayerPrefs.SetInt("1stScore", m_1stScore);
        }
        else if (m_2ndScore == 0)
        {
            // 2nd 空 → 登録
            m_2ndScore = m_TotalScore;
            PlayerPrefs.SetInt("2ndScore", m_2ndScore);
        }
        else
        {
            // 既に両方ある → 比較して入れ替え
            if (m_TotalScore >= m_1stScore)
            {
                // 1位に新記録 → 古い1位を2位に下げる
                m_2ndScore = m_1stScore;
                m_1stScore = m_TotalScore;
                PlayerPrefs.SetInt("1stScore", m_1stScore);
                PlayerPrefs.SetInt("2ndScore", m_2ndScore);
            }
            else if (m_TotalScore >= m_2ndScore)
            {
                m_2ndScore = m_TotalScore;
                PlayerPrefs.SetInt("2ndScore", m_2ndScore);
            }
        }

        PlayerPrefs.Save();
    }

}
