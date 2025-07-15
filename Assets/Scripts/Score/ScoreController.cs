using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("�X�R�A���J�E���g����ϐ�")]
    public float m_CountScorePlayer;

    [Header("�v���C���[1�̍U�����擾����X�N���v�g")]
    public PlayerAttack m_Player1Attack;
    [Header("�v���C���[2�̍U�����擾����X�N���v�g")]
    public PlayerAttack m_Player2Attack;

    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g")]
    public ScoreImage m_ScoreImage;

    private void Awake()
    {
        if (m_Player1Attack == null) m_Player1Attack = GetComponent<PlayerAttack>();
        if (m_Player2Attack == null) m_Player2Attack = GetComponent<PlayerAttack>();
        if (m_ScoreImage == null) m_ScoreImage = GetComponent<ScoreImage>();

        if (m_Player1Attack == null || m_Player2Attack == null || m_ScoreImage == null)
        {
            Debug.LogError("PlayerAttack �܂��� ScoreImage �X�N���v�g�������Ă��܂���", this);
        }
    }

    private void Start()
    {
        m_CountScorePlayer = 0;
        m_ScoreImage.ShowNumber((int)m_CountScorePlayer);
    }
    /// <summary>
    /// �v���C���[�̃X�R�A�̉��Z�֐�
    /// (�|�W�V�����̈ʒu�̐��l)
    /// </summary>
    public void PlusScore()
    {
        m_CountScorePlayer += m_Player1Attack.GetCalculatedDamage();
            m_CountScorePlayer += m_Player2Attack.GetCalculatedDamage();
        Debug.Log(m_CountScorePlayer + "�v���X");
        m_ScoreImage.ShowNumber((int)m_CountScorePlayer);
    }

    /// <summary>
    /// �v���C���[�̃X�R�A�̌��Z�֐�
    /// (�|�W�V�����̈ʒu�̐��l)
    /// </summary>
    public void MinusScore()
    {
        m_CountScorePlayer -= m_Player1Attack.GetCalculatedDamage();
        m_CountScorePlayer -= m_Player2Attack.GetCalculatedDamage();
        Debug.Log(m_CountScorePlayer + "�}�C�i�X");
        m_ScoreImage.ShowNumber((int)m_CountScorePlayer);
    }

    public void FinalScore()
    {
        PlayerPrefs.SetInt("DamageScore", (int)m_CountScorePlayer);
        //PlayerPrefs.SetInt("TimeScore", (int)???);
        PlayerPrefs.Save();
    }
}
