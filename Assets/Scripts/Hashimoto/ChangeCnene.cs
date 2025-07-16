using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeCnene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {    }


    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Result")
        {
             
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 
            {
                changeSceneEnter();
            }
        }
    }

    void changeSceneEnter()
    {
        ScreenFader screenFader = FindObjectOfType<ScreenFader>();

        if (screenFader != null)
        {
            StartCoroutine(screenFader.BackBlack());
        }

        Invoke(nameof(GoTItle), 2.0f);
    }

    private void GoTItle()
    {
        SceneManager.LoadScene("Title");

    }
}
