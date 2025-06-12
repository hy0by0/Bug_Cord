using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敵のアニメーションに当たり判定が追従するようにするスクリプトです

// 必要なコンポーネントを自動でアタッチし、存在を保証する
[RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
public class AnimatePolygonCollider : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    // 1フレーム前のスプライトを記憶しておくための変数
    private Sprite lastSprite;

    void Start()
    {
        // コンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        // 現在のスプライトと前のフレームのスプライトを比較
        if (spriteRenderer.sprite != lastSprite)
        {
            // スプライトが変更されていたら、コライダーの形状を更新する
            UpdateColliderShape();
            // 現在のスプライトを「前のスプライト」として記憶する
            lastSprite = spriteRenderer.sprite;
        }
    }

    /// <summary>
    /// 現在のスプライトに合わせてPolygonCollider2Dの形状を更新する
    /// </summary>
    private void UpdateColliderShape()
    {
        // スプライトに設定されているPhysics Shapeの数を取得
        int shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
        polygonCollider.pathCount = shapeCount;

        // Physics Shapeの形状をリストとして取得し、コライダーに設定する
        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < shapeCount; i++)
        {
            path.Clear();
            spriteRenderer.sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path);
        }
    }
}