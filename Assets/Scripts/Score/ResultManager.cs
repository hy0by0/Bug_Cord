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

    int m_DamageScore;
    int m_TimeScore;
    int m_TotalScore;


    private void Start()
    {
        m_DamageScore = (int)ScoreController.m_CountScorePlayer;
        m_TimeScore = ScoreController.m_Time;

   

        StartCoroutine(IndicationScore());
    }

    IEnumerator IndicationScore()
    {
        yield return new WaitForSeconds(1.5f);

        m_TimeScoreCountUP.StartResult(m_TimeScore);
        yield return new WaitForSeconds(1f);

        m_DmageScoreCountUP.StartResult(m_DamageScore);
        yield return new WaitForSeconds(1.5f);

        m_TotalScore = m_TimeScore + m_DamageScore;
        m_TotalScoreCountUP.StartResult(m_TotalScore);
        yield return new WaitForSeconds(3f);


    }

    //void Comparison()
    //{
    //    if (m_1stScore == 0)
    //    {
    //        m_1stScore = m_TotalScore;
    //        PlayerPrefs.SetInt("1stScore", m_1stScore);
    //    }
    //    else if (m_2ndScore == 0)
    //    {
    //        m_2ndScore = m_TotalScore;
    //        PlayerPrefs.SetInt("2ndScore", m_2ndScore);
    //    }
    //    else
    //    {
    //        if (m_TotalScore >= m_1stScore)
    //        {
    //            m_2ndScore = m_1stScore;
    //            m_1stScore = m_TotalScore;
    //            PlayerPrefs.SetInt("1stScore", m_1stScore);
    //            PlayerPrefs.SetInt("2ndScore", m_2ndScore);
    //        }
    //        else if (m_TotalScore >= m_2ndScore)
    //        {
    //            m_2ndScore = m_TotalScore;
    //            PlayerPrefs.SetInt("2ndScore", m_2ndScore);
    //        }
    //    }

    //    PlayerPrefs.Save();
    //}
}
