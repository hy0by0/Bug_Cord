using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreImage : MonoBehaviour
{
    [Header("����Image:�ォ�牺(���`��)")]
    [SerializeField]
    List<Image> m_ScoreImages;

    public ScoreSprite m_Sprite;

    private void Awake()
    {
        if (m_Sprite == null) m_Sprite = GetComponent<ScoreSprite>();
        if (m_Sprite == null)
            Debug.Log("m_Sprite�ɃX�N���v�g�������Ă��܂���");
    }
    /// <summary>
    /// ��ʏ�ɐ����X�v���C�g��\��������
    /// </summary>
    /// <param name="m_number"></param>
    public void ShowNumber(int m_number)
    {
        string m_str = m_number.ToString("D5");

        for (int i = 0; i < m_ScoreImages.Count; i++)
        {
            int m_digit = int.Parse(m_str[i].ToString());
            m_ScoreImages[i].sprite = m_Sprite.m_ScoreSprite[m_digit];
        }
    }
}
