using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �ݒ肳�ꂽ��`�͈͓��������_���Ɉړ����A���E�ɒB����������Ɍ�����ς���B
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRandomMove_Boundary : MonoBehaviour
{
    [Header("�� �ړ��ݒ�")]
    [Tooltip("�G�̈ړ����x")]
    public float moveSpeed = 3f;
    [Tooltip("������ς���܂ł̎��ԁi�b�j")]
    public float directionChangeInterval = 3.0f;

    [Header("�� �����͈͐ݒ�")]
    [Tooltip("�����ʒu����̐��������̍ő�ړ�����")]
    public float horizontalRange = 5f;
    [Tooltip("�����ʒu����̐��������̍ő�ړ�����")]
    public float verticalRange = 3f;

    // --- �����Ŏg���ϐ� ---
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 startPosition; // �����ʒu���L��
    private bool isTurning = false; // �����]�����t���O

    // 8�����̈ړ��x�N�g��
    private readonly List<Vector2> allDirections = new List<Vector2>
    {
        new Vector2(0, 1).normalized, new Vector2(1, 1).normalized,
        new Vector2(1, 0).normalized, new Vector2(1, -1).normalized,
        new Vector2(0, -1).normalized, new Vector2(-1, -1).normalized,
        new Vector2(-1, 0).normalized, new Vector2(-1, 1).normalized
    };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // �Q�[���J�n���̈ʒu�������ʒu�Ƃ��ċL��
        startPosition = transform.position;

        ChooseNewDirection();
        StartCoroutine(DirectionChangeRoutine());
    }

    void Update()
    {
        // ���E�`�F�b�N�ƕ����]��
        CheckBounds();
    }

    void FixedUpdate()
    {
        // �������Z�ňړ�
        rb.velocity = moveDirection * moveSpeed;
    }

    /// <summary>
    /// ���E�̊O�ɏo�����ǂ������`�F�b�N���A�o�Ă���Ό�����ς���
    /// </summary>
    private void CheckBounds()
    {
        // �����]�����͘A���Ŕ��肵�Ȃ�
        if (isTurning) return;

        Vector2 currentPosition = transform.position;
        Vector2 boundaryNormal = Vector2.zero; // ���z�I�ȕǂ̌���

        // ���������̋��E�`�F�b�N
        if (currentPosition.x > startPosition.x + horizontalRange)
        {
            boundaryNormal = Vector2.left; // �E�̕ǂɓ��������̂ŁA�@���͍�����
        }
        else if (currentPosition.x < startPosition.x - horizontalRange)
        {
            boundaryNormal = Vector2.right; // ���̕ǂɓ��������̂ŁA�@���͉E����
        }

        // ���������̋��E�`�F�b�N
        if (currentPosition.y > startPosition.y + verticalRange)
        {
            boundaryNormal = Vector2.down; // ��̕ǂɓ��������̂ŁA�@���͉�����
        }
        else if (currentPosition.y < startPosition.y - verticalRange)
        {
            boundaryNormal = Vector2.up; // ���̕ǂɓ��������̂ŁA�@���͏����
        }

        // �������E�ɐڐG���Ă�����iboundaryNormal���ݒ肳��Ă�����j
        if (boundaryNormal != Vector2.zero)
        {
            ChooseNewDirection(boundaryNormal);
            // �����ɍĔ��肵�Ȃ��悤�ɒZ���N�[���^�C����݂���
            StartCoroutine(TurnCooldown());
        }
    }

    /// <summary>
    /// �V�����ړ������������_���Ɍ��肷��
    /// </summary>
    private void ChooseNewDirection(Vector2? boundaryNormal = null)
    {
        List<Vector2> validDirections = new List<Vector2>();

        if (boundaryNormal.HasValue)
        {
            foreach (Vector2 dir in allDirections)
            {
                // ���E�̓����������������������ɓ����
                if (Vector2.Dot(dir, boundaryNormal.Value) > 0)
                {
                    validDirections.Add(dir);
                }
            }
        }
        else
        {
            validDirections = allDirections;
        }

        if (validDirections.Count == 0)
        {
            moveDirection = -moveDirection;
            return;
        }

        int index = Random.Range(0, validDirections.Count);
        moveDirection = validDirections[index];
    }

    // �Z�������]���N�[���_�E��
    private IEnumerator TurnCooldown()
    {
        isTurning = true;
        yield return new WaitForSeconds(0.1f);
        isTurning = false;
    }

    // ���Ԍo�߂ɂ������]��
    private IEnumerator DirectionChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(directionChangeInterval);
            ChooseNewDirection();
        }
    }
}