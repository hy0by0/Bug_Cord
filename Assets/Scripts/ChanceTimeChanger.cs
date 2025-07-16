using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChanceTimeChanger : MonoBehaviour
{
    public static ChanceTimeChanger Instance { get; private set; }

    [Header("�� �����[�ݒ�")]
    public int requiredHitsForChanceTime = 10;
    public string chanceTimeSceneName = "SampleScene";

    [Header("�� �֘A�I�u�W�F�N�g�iMain�V�[���Őݒ�j")]
    public EnemyController enemyP1_ref;
    public EnemyController enemyP2_ref;

    private int p1HealthBeforeChance;
    private int p2HealthBeforeChance;
    public static int DamageDealtInChanceTime = 0;

    private int totalCriticalHits = 0;
    private bool isRallyActive = false;
    private bool isSceneTransitioning = false;
    // ���y�ǉ��z�`�����X�^�C���Ɉ�x�ł��s���������L�^����t���O
    private bool hasBeenInChanceTime = false;

    public bool IsRallyActive => isRallyActive;

    void Awake()
    {
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���y�ύX�zMain�V�[���ɖ߂�A���A�`�����X�^�C���ɍs�������Ƃ�����ꍇ�̂ݕ���
        if (scene.name == "Main" && hasBeenInChanceTime)
        {
            StartCoroutine(RestoreEnemiesState());
            // ���y�ǉ��z�����������I�������A�t���O�����Z�b�g���Ď���̃`�����X�^�C���ɔ�����
            hasBeenInChanceTime = false;
        }
    }

    private IEnumerator RestoreEnemiesState()
    {
        yield return null;

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        EnemyController p1 = enemies.FirstOrDefault(e => e.playerID == PlayerID.P1);
        EnemyController p2 = enemies.FirstOrDefault(e => e.playerID == PlayerID.P2);

        if (p1 != null && p2 != null)
        {
            Debug.Log("�G�̏�Ԃ𕜌����܂�...");
            p1.SetHealth(p1HealthBeforeChance);
            p2.SetHealth(p2HealthBeforeChance);

            int sharedDamage = DamageDealtInChanceTime / 2;
            Debug.Log($"�`�����X�^�C���{�[�i�X�_���[�W: {sharedDamage} �𗼕��ɓK�p�I");
            p1.TakeDamage(sharedDamage);
            p2.TakeDamage(sharedDamage);

            enemyP1_ref = p1;
            enemyP2_ref = p2;
        }

        DamageDealtInChanceTime = 0;
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

        if (totalCriticalHits >= requiredHitsForChanceTime)
        {
            StartCoroutine(TransitionToChanceTime());
            return;
        }

        PlayerID nextTargetID = (idOfEnemyJustHit == PlayerID.P1) ? PlayerID.P2 : PlayerID.P1;
        SwitchCriticalTarget(nextTargetID);
    }

    /// <summary>
    /// �����[���I���������Ƃ�ʒm����
    /// </summary>
    public void EndRally()
    {
        isRallyActive = false;
    }

    private void SwitchCriticalTarget(PlayerID targetID)
    {
        if (targetID == PlayerID.P1 && enemyP1_ref != null)
        {
            enemyP1_ref.ShowCriticalPoint();
            if (enemyP2_ref != null) enemyP2_ref.HideCriticalPoint();
        }
        else if (targetID == PlayerID.P2 && enemyP2_ref != null)
        {
            enemyP2_ref.ShowCriticalPoint();
            if (enemyP1_ref != null) enemyP1_ref.HideCriticalPoint();
        }
    }

    private IEnumerator TransitionToChanceTime()
    {
        isSceneTransitioning = true;

        if (enemyP1_ref != null) p1HealthBeforeChance = enemyP1_ref.enemy_hp;
        if (enemyP2_ref != null) p2HealthBeforeChance = enemyP2_ref.enemy_hp;

        DamageDealtInChanceTime = 0;

        // ���y�ǉ��z�`�����X�^�C���ɍs�����O�ɁA�t���O�𗧂Ă�
        hasBeenInChanceTime = true;

        Debug.Log($"HP��ۑ�: P1={p1HealthBeforeChance}, P2={p2HealthBeforeChance}");
        Debug.Log("�ڕW�B���I�`�����X�^�C���Ɉڍs���܂��B");

        EndRally();

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