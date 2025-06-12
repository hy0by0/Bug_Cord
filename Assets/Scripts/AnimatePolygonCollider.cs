using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�G�̃A�j���[�V�����ɓ����蔻�肪�Ǐ]����悤�ɂ���X�N���v�g�ł�

// �K�v�ȃR���|�[�l���g�������ŃA�^�b�`���A���݂�ۏ؂���
[RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
public class AnimatePolygonCollider : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    // 1�t���[���O�̃X�v���C�g���L�����Ă������߂̕ϐ�
    private Sprite lastSprite;

    void Start()
    {
        // �R���|�[�l���g���擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        // ���݂̃X�v���C�g�ƑO�̃t���[���̃X�v���C�g���r
        if (spriteRenderer.sprite != lastSprite)
        {
            // �X�v���C�g���ύX����Ă�����A�R���C�_�[�̌`����X�V����
            UpdateColliderShape();
            // ���݂̃X�v���C�g���u�O�̃X�v���C�g�v�Ƃ��ċL������
            lastSprite = spriteRenderer.sprite;
        }
    }

    /// <summary>
    /// ���݂̃X�v���C�g�ɍ��킹��PolygonCollider2D�̌`����X�V����
    /// </summary>
    private void UpdateColliderShape()
    {
        // �X�v���C�g�ɐݒ肳��Ă���Physics Shape�̐����擾
        int shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
        polygonCollider.pathCount = shapeCount;

        // Physics Shape�̌`������X�g�Ƃ��Ď擾���A�R���C�_�[�ɐݒ肷��
        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < shapeCount; i++)
        {
            path.Clear();
            spriteRenderer.sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path);
        }
    }
}