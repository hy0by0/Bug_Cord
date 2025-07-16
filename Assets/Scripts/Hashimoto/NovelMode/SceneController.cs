using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeDuration = 1.0f;

    bool fadeflag = true;

    public void FadeIn()
    {
        StartCoroutine(Fade(1, 0));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0, 1));

    }

    // Start is called before the first frame update
    void Start()
    {
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Fade(float startAlpha, float endAlpha)//実際のフェード処理
    {
        float time = 0f;//時間経過を追跡する変数
        Color fadeColor = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = time / fadeDuration;

            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, t);

            fadeImage.color = fadeColor;

            yield return null;
        }

        fadeColor.a = endAlpha;

        fadeImage.color = fadeColor;

        if (fadeflag == false)
        {

            int index = SceneManager.GetActiveScene().buildIndex;

            index++;

            SceneManager.LoadScene(index);
        }

        fadeflag = false;
    }

    public void NewGame()//初めから
    {
        SceneManager.LoadScene("Scene1-1");

    }

    public void LoadGame()//続きから
    {
        int scene_Number = PlayerPrefs.GetInt("SceneNumber", 1);

        SceneManager.LoadScene(scene_Number);
    }
}
