using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("�X�R�A���J�E���g����ϐ�")]
    public static int m_CountScorePlayer;
    [Header("Boy�X�R�A���J�E���g����ϐ�")]
    public static int m_BoyCountScore;
    [Header("Girl�X�R�A���J�E���g����ϐ�")]
    public static int m_GirlCountScore;

    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g(Boy)")]
    public ScoreImage m_BoyScoreImage;
    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g(Girl)")]
    public ScoreImage m_GirlScoreImage;

    [Header("���Ԃ����Z���邽�߂ɕK�v�ȃX�N���v�g")]
    public GlobalTimeControl m_CountDownClock;

    public static int m_Time; // �� �ǉ�

    private void Awake()
    {
        if (m_BoyScoreImage == null) m_BoyScoreImage = GetComponent<ScoreImage>();
        if (m_GirlScoreImage == null) m_GirlScoreImage = GetComponent<ScoreImage>();
        if (m_CountDownClock == null) m_CountDownClock = GetComponent<GlobalTimeControl>();

        if (m_BoyScoreImage == null || m_GirlScoreImage == null)
        {
            Debug.LogError("ScoreImage ���ݒ肳��Ă��܂���");
        }
        else if (m_CountDownClock == null)
        {
            Debug.LogError("CountDownClock ���ݒ肳��Ă��܂���");
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

        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} ���v:{m_CountScorePlayer}");
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

        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} ���v:{m_CountScorePlayer}");
        m_BoyScoreImage.ShowNumber(m_BoyCountScore);
        m_GirlScoreImage.ShowNumber(m_GirlCountScore);
    }

    public void FinalScore()
    {
        m_Time = 100; // �K�v�Ȃ�^�C�}�[����擾����
        Debug.Log($"FinalScore: Damage:{m_CountScorePlayer} Time:{m_Time}");
    }
}