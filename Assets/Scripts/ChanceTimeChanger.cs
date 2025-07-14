using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �N���e�B�J���q�b�g�̃����[���Ǘ����A���v�񐔂ɉ����ă`�����X�^�C���ւ̃V�[���J�ڂ�S���B
/// </summary>
public class ChanceTimeChanger : MonoBehaviour
{
    // ���̃N���X�̗B��̃C���X�^���X��ێ�����i�V���O���g���p�^�[���j
    public static ChanceTimeChanger Instance { get; private set; }

    [Header("�� �����[�ݒ�")]
    [Tooltip("�`�����X�^�C���Ɉڍs���邽�߂ɕK�v�ȃq�b�g��")]
    public int requiredHitsForChanceTime = 10;
    [Tooltip("�J�ڐ�̃`�����X�^�C���V�[����")]
    public string chanceTimeSceneName = "ChanceTime";

    [Header("�� �֘A�I�u�W�F�N�g")]
    [Tooltip("P1�`�[���̓G")]
    public EnemyController enemyP1;
    [Tooltip("P2�`�[���̓G")]
    public EnemyController enemyP2;

    // --- �����ϐ� ---
    private int totalCriticalHits = 0;
    private bool isRallyActive = false;
    private bool isSceneTransitioning = false;

    // --- ���J�v���p�e�B ---
    /// <summary>
    /// �O�����烉���[�����m�F�ł���悤�ɂ���
    /// </summary>
    public bool IsRallyActive => isRallyActive;

    void Awake()
    {
        // �V���O���g���̐ݒ�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����܂����ő��݂�����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �N���e�B�J�������[�̊J�n��錾����
    /// </summary>
    public void StartRally(PlayerID initialTargetID)
    {
        if (isRallyActive) return;

        isRallyActive = true;
        totalCriticalHits = 0;
        Debug.Log("�N���e�B�J�������[�J�n�I");

        // �ŏ��̃N���e�B�J���|�C���g���o��������
        SwitchCriticalTarget(initialTargetID);
    }

    /// <summary>
    /// �N���e�B�J���q�b�g��񍐂����A�^�[�Q�b�g��؂�ւ���
    /// </summary>
    public void ReportCriticalHitAndSwitch(PlayerID idOfEnemyJustHit)
    {
        if (!isRallyActive || isSceneTransitioning) return;

        totalCriticalHits++;
        Debug.Log($"�N���e�B�J���q�b�g�I ���v: {totalCriticalHits} / {requiredHitsForChanceTime}��");

        // �K�v�ȉ񐔂ɒB������`�����X�^�C���ֈڍs
        if (totalCriticalHits >= requiredHitsForChanceTime)
        {
            StartCoroutine(TransitionToChanceTime());
            return;
        }

        // ���̃^�[�Q�b�g�����肵�āA�N���e�B�J���|�C���g���ړ�������
        PlayerID nextTargetID = (idOfEnemyJustHit == PlayerID.P1) ? PlayerID.P2 : PlayerID.P1;
        SwitchCriticalTarget(nextTargetID);
    }

    /// <summary>
    /// �w�肳�ꂽ�^�[�Q�b�g�̃N���e�B�J���|�C���g��L��������
    /// </summary>
    private void SwitchCriticalTarget(PlayerID targetID)
    {
        if (targetID == PlayerID.P1 && enemyP1 != null)
        {
            enemyP1.ShowCriticalPoint();
            if (enemyP2 != null) enemyP2.HideCriticalPoint();
        }
        else if (targetID == PlayerID.P2 && enemyP2 != null)
        {
            enemyP2.ShowCriticalPoint();
            if (enemyP1 != null) enemyP1.HideCriticalPoint();
        }
    }

    /// <summary>
    /// �`�����X�^�C���V�[���ւ̑J�ڏ������s���R���[�`��
    /// </summary>
    private IEnumerator TransitionToChanceTime()
    {
        isSceneTransitioning = true;
        Debug.Log("�ڕW�B���I�`�����X�^�C���Ɉڍs���܂��B");

        ScreenFader screenFader = FindObjectOfType<ScreenFader>();
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.BackBlack());
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene(chanceTimeSceneName);
        isSceneTransitioning = false;
    }
}