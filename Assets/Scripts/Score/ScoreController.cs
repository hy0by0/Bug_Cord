using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    [Header("���݂̍��v�X�R�A(�\����p)")]
    [SerializeField] private float inspector_TotalScore;


    [Header("�X�R�A���J�E���g����ϐ�")]
    public static float m_CountScorePlayer;
    [Header("Boy�X�R�A���J�E���g����ϐ�")]
    public static float m_BoyCountScore;
    [Header("Girl�X�R�A���J�E���g����ϐ�")]
    public static float m_GirlCountScore;

    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g(Boy)")]
    public ScoreImage m_BoyScoreImage;
    [Header("�X�R�A�̉摜�𐧌䂷��X�N���v�g(Girl)")]
    public ScoreImage m_GirlScoreImage;

    [Header("���Ԃ����Z���邽�߂ɕK�v�ȃX�N���v�g")]
    public GlobalTimeControl m_CountDownClock;

    public static int m_Time; // �� �ǉ�

    private static bool created = false;

    private static ScoreController instance;

    private void Update()
    {
        inspector_TotalScore = m_BoyCountScore + m_GirlCountScore;
    }
    private void OnEnable()
    {
        // �V�[���؂�ւ���ɌĂ΂��C�x���g��o�^
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // �o�^����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �I�u�W�F�N�g���ŒT��
        var boyObj = GameObject.Find("BoyScore");
        var girlObj = GameObject.Find("GirlScore");

        if (boyObj != null)
            m_BoyScoreImage = boyObj.GetComponent<ScoreImage>();
        else
            Debug.LogError("BoyScore �I�u�W�F�N�g��������܂���");

        if (girlObj != null)
            m_GirlScoreImage = girlObj.GetComponent<ScoreImage>();
        else
            Debug.LogError("GirlScore �I�u�W�F�N�g��������܂���");

        // �^�C�}�[�͌^�ŒT��
        m_CountDownClock = FindObjectOfType<GlobalTimeControl>();
        if (m_CountDownClock == null)
            Debug.LogError("GlobalTimeControl ���Ď擾�ł��܂���ł���");

        // �V�[���؂�ւ���̃X�R�A�ĕ\��
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }
    private void Awake()
    {
        if (instance == null)
        {
            // �ŏ��Ƀ��[�h���ꂽ�C���X�^���X
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // ���ɃC���X�^���X�����݂��Ă����玩�����g��j��
            Destroy(gameObject);
            return;
        }

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

        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} ���v:{m_CountScorePlayer}");
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

        Debug.Log($"Boy:{m_BoyCountScore} Girl:{m_GirlCountScore} ���v:{m_CountScorePlayer}");
        m_BoyScoreImage.ShowNumber((int)m_BoyCountScore);
        m_GirlScoreImage.ShowNumber((int)m_GirlCountScore);
    }

    public void FinalScore()
    {
        m_Time = (int)m_CountDownClock.globalTimeInSeconds * 100; // �K�v�Ȃ�^�C�}�[����擾����
        Debug.Log($"FinalScore: Damage:{m_CountScorePlayer} Time:{m_Time}");
    }
}
