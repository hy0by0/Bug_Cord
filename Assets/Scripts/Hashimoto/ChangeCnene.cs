using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeCnene : MonoBehaviour
{
    private NewActions inputActions;


    // Start is called before the first frame update
    void Start()
    {
        int connectedGamepads = Gamepad.all.Count;
        Debug.Log("接続されているゲームパッドの数: " + connectedGamepads);
    }

    void Awake()
    {
        inputActions = new NewActions();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Result")
        {

                inputActions.Player.Enable();
                inputActions.Player.Shot.performed += changeScene;
              

            //if (Input.GetMouseButtonDown(0) || Input.GetButton("btnA")) 
            //{
            //    ScreenFader screenFader = FindObjectOfType<ScreenFader>();

            //    if (screenFader != null)
            //    {
            //        StartCoroutine(screenFader.BackBlack());
            //    }

            //    Invoke(nameof(GoTItle), 2.0f);

            //}
        }
    }


    void changeScene(InputAction.CallbackContext ctx)
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
