using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ChanceTime�V�[���̃^�C�}�[���Ǘ����AMain�V�[���ɖ߂����������B
/// </summary>
public class ChanceTimeController : MonoBehaviour
{
    [Tooltip("�`�����X�^�C���̌p�����ԁi�b�j")]
    public float chanceTimeDuration = 30f;
    [Tooltip("�߂��̃��C���V�[����")]
    public string mainSceneName = "Main";

    void Start()
    {
        // �^�C�}�[�X�^�[�g
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