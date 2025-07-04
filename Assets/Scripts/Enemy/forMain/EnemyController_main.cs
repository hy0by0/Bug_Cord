using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��̂̓G�Ɋւ��鏈���S�ʂ�S���N���X�B
/// �G���Ƃɂ��̃R���|�[�l���g��1���K�v�ɂȂ�B
/// </summary>
public class EnemyController_main : MonoBehaviour
{
    [Header("�� �`�[���ݒ�")]
    [Tooltip("���̓G��P1��P2�̂ǂ���ɑΉ����邩��ݒ�")]
    public PlayerID playerID;

    [Header("�� �O���t�B�b�N�֘A")]
    [SerializeField] private GameObject criticalEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("�� �X�e�[�^�X")]
    public int enemy_hp = 2000;
    // �� ���̓G��p��UI�}�l�[�W���[�ւ̎Q��
    [SerializeField] private UIManager_main uimanager;

    // ���y�d�v�z'static'���폜�B����ɂ��A�e�G���������g�̏�Ԃ����悤�ɂȂ�B
    public string enemy_state = "Normal";

    // �� �ő�HP���L�����Ă������߂̓����ϐ�
    private int maxHp;

    [Header("�� �N���e�B�J���q�b�g�֘A")]
    public int count_hit = 0;
    public int count_hit_Flag = 5;
    public List<CriticalHitBox_Spawnded> spawnPairs;
    public float activeDuration_HitBox = 8f;
    private bool hasSpawned = false;

    /// <summary>
    /// �Q�[���J�n���Ɉ�x�����Ă΂�鏉��������
    /// </summary>
    void Start()
    {
        // ���g�̃R���|�[�l���g���擾
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // �� �Q�[���J�n����HP���ő�HP�Ƃ��ċL�����Ă���
        maxHp = enemy_hp;

        // �� �Ή�����UI�}�l�[�W���[�ɁAHP�̏�����Ԃ�`����
        if (uimanager != null)
        {
            uimanager.UpdateHealth(maxHp, enemy_hp);
        }

        count_hit = 0;
    }

    /// <summary>
    /// ���t���[���Ă΂��X�V����
    /// </summary>
    void Update()
    {
        if (!hasSpawned && count_hit >= count_hit_Flag)
        {
            SpawnPairByID("1");
        }

        if (enemy_hp <= 0)
        {
            enemy_state = "Dead";
        }
    }

    // --- �e�q�b�g�������\�b�h ---
    // �i��ȕύX�_�́A�_���[�W���󂯂����UI�֒ʒm���镔���j

    public void HitResist(int playerAtackdamage)
    {
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 0.5f);
        enemy_hp -= enemy_damaged;
        // �� �_���[�W��̍ŐV��HP��Ԃ�UI�}�l�[�W���[�ɒʒm
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        count_hit++;
    }

    public void HitNormal(int playerAtackdamage)
    {
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashColorCoroutine());

        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.0f);
        enemy_hp -= enemy_damaged;
        // �� �_���[�W��̍ŐV��HP��Ԃ�UI�}�l�[�W���[�ɒʒm
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        count_hit++;
    }

    public void HitWeak(int playerAtackdamage)
    {
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.5f);
        enemy_hp -= enemy_damaged;
        // �� �_���[�W��̍ŐV��HP��Ԃ�UI�}�l�[�W���[�ɒʒm
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        count_hit++;
    }

    public void HitCritical(int playerAtackdamage)
    {
        Instantiate(criticalEffectPrefab, transform.position, Quaternion.identity);
        animator.SetTrigger("Critical");
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 2.0f);
        enemy_hp -= enemy_damaged;
        // �� �_���[�W��̍ŐV��HP��Ԃ�UI�}�l�[�W���[�ɒʒm
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        enemy_state = "stan";
    }

    // (���̑��̃��\�b�h�͕ύX�Ȃ�)
    public void SpawnPairByID(string id) { /* ... */ }
    private IEnumerator SpawnHitboxCoroutine(CriticalHitBox_Spawnded pair) { /* ... */ yield return null; }
    private IEnumerator FlashColorCoroutine() { /* ... */ yield return null; }
}