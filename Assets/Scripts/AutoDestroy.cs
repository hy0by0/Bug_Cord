using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // エフェクトが消えるまでの時間（秒）
    [SerializeField] private float lifetime = 0.1f;


    void Start()
    {
        // lifetime秒後に、このオブジェクト自身を破壊（シーンから削除）する
        Destroy(gameObject, lifetime);
    }
}