using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }

    [Header("�X�R�A���J�E���g����ϐ�")]
    public float m_CountScorePlayer;
    [Header("Boy�X�R�A���J�E���g����ϐ�")]
    public float m_BoyCountScore;
    [Header("Girl�X�R�A���J�E���g����ϐ�")]
    public float m_GirlCountScore;

    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g(Boy)")]
    public ScoreImage m_BoyScoreImage;
    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g(Girl)")]
    public ScoreImage m_GirlScoreImage;

    int m_Time;
    [Header("���Ԃ����Z���邽�߂ɕK�v�ȃX�N���v�g")]
    public GlobalTimeControl m_CountDownClock;
    private void Awake()
    {
        if (m_BoyScoreImage == null) m_BoyScoreImage = GetComponent<ScoreImage>();
        if (m_GirlScoreImage == null) m_GirlScoreImage = GetComponent<ScoreImage>();
        if(m_CountDownClock==null)m_CountDownClock = GetComponent<GlobalTimeControl>();

        if (m_BoyScoreImage == null || m_GirlScoreImage == null)
        {
            Debug.LogError("ScoreImage ���ݒ肳��Ă��܂���");
        }
        else if(m_CountDownClock == null)
        {
            Debug.LogError("CountDownClock ���ݒ肳��Ă��܂���");
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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

    /// <summary>
    /// �v���C���[�̃X�R�A�̉��Z�֐�
    /// (�|�W�V�����̈ʒu�̐��l)
    /// </summary>
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

        //���O�ƕ\��
        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} ���v:{m_CountScorePlayer}");
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }

    /// <summary>
    /// �v���C���[�̃X�R�A�̌��Z�֐�
    /// (�|�W�V�����̈ʒu�̐��l)
    /// </summary>
    public void MinusScore(int damage, PlayerID m_playerID)
    {
        if (m_playerID == PlayerID.P2)
        {
            m_BoyCountScore -= damage;
        }
        else if (m_playerID == PlayerID.P1)
        {
            m_GirlCountScore -= damage;
        }

        m_CountScorePlayer = m_BoyCountScore + m_GirlCountScore;

        //���O�ƕ\��
        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} ���v:{m_CountScorePlayer}");
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }
    
    public void FinalScore()
    {
        m_Time = 100 * (int)m_CountDownClock.globalTimeInSeconds;
        PlayerPrefs.SetInt("DamageScore", (int)m_CountScorePlayer);
        PlayerPrefs.SetInt("BoyDamageScore", (int)m_BoyCountScore);
        PlayerPrefs.SetInt("GirlDamageScore", (int)m_GirlCountScore);
        PlayerPrefs.SetInt("TimeScore", (int)m_Time);
        PlayerPrefs.Save();
    }
}
