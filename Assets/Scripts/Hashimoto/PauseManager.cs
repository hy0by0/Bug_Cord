using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    bool isPause;

    [SerializeField] Text uiPause;
    public float speed = 1.0f;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        isPause = false;

        uiPause.enabled = false; ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseCheck();
        }

        if (isPause)
        {
            uiPause.color = GetAlphaColor(uiPause.color);
        }
    }

    public void PauseCheck()
    {
        if (isPause)
        {
            ResumeGame();


        }
        else
        {
            PauseGame();


        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        uiPause.enabled = true;

        isPause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        uiPause.enabled = false;



        isPause = false;
    }

    Color GetAlphaColor(Color color)
    {

        time += Time.unscaledTime * 3.5f * speed;
        color.a = Mathf.Sin(time);

        return color;
    }
}
