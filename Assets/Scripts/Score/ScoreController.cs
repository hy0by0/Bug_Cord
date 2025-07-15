using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("スコアをカウントする変数")]
    public float m_CountScorePlayer;

    [Header("プレイヤー1の攻撃を取得するスクリプト")]
    public PlayerAttack m_Player1Attack;
    [Header("プレイヤー2の攻撃を取得するスクリプト")]
    public PlayerAttack m_Player2Attack;

    [Header("スコアの画像を制御するスクリプト")]
    public ScoreImage m_ScoreImage;

    private void Awake()
    {
        if (m_Player1Attack == null) m_Player1Attack = GetComponent<PlayerAttack>();
        if (m_Player2Attack == null) m_Player2Attack = GetComponent<PlayerAttack>();
        if (m_ScoreImage == null) m_ScoreImage = GetComponent<ScoreImage>();

        if (m_Player1Attack == null || m_Player2Attack == null || m_ScoreImage == null)
        {
            Debug.LogError("PlayerAttack または ScoreImage スクリプトが入っていません", this);
        }
    }

    private void Start()
    {
        m_CountScorePlayer = 0;
        m_ScoreImage.ShowNumber((int)m_CountScorePlayer);
    }
    /// <summary>
    /// プレイヤーのスコアの加算関数
    /// (ポジションの位置の数値)
    /// </summary>
    public void PlusScore()
    {
        m_CountScorePlayer += m_Player1Attack.GetCalculatedDamage();
            m_CountScorePlayer += m_Player2Attack.GetCalculatedDamage();
        Debug.Log(m_CountScorePlayer + "プラス");
        m_ScoreImage.ShowNumber((int)m_CountScorePlayer);
    }

    /// <summary>
    /// プレイヤーのスコアの減算関数
    /// (ポジションの位置の数値)
    /// </summary>
    public void MinusScore()
    {
        m_CountScorePlayer -= m_Player1Attack.GetCalculatedDamage();
        m_CountScorePlayer -= m_Player2Attack.GetCalculatedDamage();
        Debug.Log(m_CountScorePlayer + "マイナス");
        m_ScoreImage.ShowNumber((int)m_CountScorePlayer);
    }

    public void FinalScore()
    {
        PlayerPrefs.SetInt("DamageScore", (int)m_CountScorePlayer);
        //PlayerPrefs.SetInt("TimeScore", (int)???);
        PlayerPrefs.Save();
    }
}
