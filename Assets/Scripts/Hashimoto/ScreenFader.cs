using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [Header("<=== Image ===>")]
    [SerializeField] Image image;

    float fadeDuration = 2f;  // フェード時間
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator BackBlack()
    {
        image.gameObject.SetActive(true);
        float timer = 0.0f;

        Color color = image.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // 完全に黒に
        image.color = new Color(color.r, color.g, color.b, 1f);
    }
}
