using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // �G�t�F�N�g��������܂ł̎��ԁi�b�j
    [SerializeField] private float lifetime = 0.1f;


    void Start()
    {
        // lifetime�b��ɁA���̃I�u�W�F�N�g���g��j��i�V�[������폜�j����
        Destroy(gameObject, lifetime);
    }
}