using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChanceTimeController : MonoBehaviour
{
    [Tooltip("�`�����X�^�C���̌p�����ԁi�b�j")]
    public float chanceTimeDuration = 20f; // �� 20�b�ɐݒ�
    [Tooltip("�߂��̃��C���V�[����")]
    public string mainSceneName = "Main";

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        Debug.Log($"�`�����X�^�C���J�n�I �c��{chanceTimeDuration}�b");
        yield return new WaitForSeconds(chanceTimeDuration);

        Debug.Log("�`�����X�^�C���I���B���C���V�[���ɖ߂�܂��B");
        SceneManager.LoadScene(mainSceneName);
    }
}