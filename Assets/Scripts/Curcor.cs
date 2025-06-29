using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Curcor : MonoBehaviour
{
    public int playerNumber = 1; // 1=Player1, 2=Player2
    public GameObject hitbox;

    public bool ClickPosition = false;
    public bool goAtack = false;
    public bool shoot_reload_Flag = true;
    public bool shoot_allow = true;

    float moveSpeed = 50f;

    private NewActions inputActions;
    private Vector2 stickInput;

    private void Start()
    {
        int connectedGamepads = Gamepad.all.Count;
        Debug.Log("接続されているゲームパッドの数: " + connectedGamepads);


    }

    void Awake()
    {
        inputActions = new NewActions();

        if (playerNumber == 1)
        {
            inputActions.Player.Enable();
            inputActions.Player.Cursor.performed += ctx => stickInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Cursor.canceled += ctx => stickInput = Vector2.zero;
            inputActions.Player.Shot.performed += ctx => OnAttack();
        }
        else if (playerNumber == 2)
        {
            inputActions.Player2.Enable(); // キーボード入力も含まれている

            inputActions.Player2.Cursor.performed += ctx =>
            {
                stickInput = ctx.ReadValue<Vector2>();
                Debug.Log("Player2 Input From: " + ctx.control.device.displayName); // 入力デバイスを確認
            };

            inputActions.Player2.Cursor.canceled += ctx => stickInput = Vector2.zero;
            inputActions.Player2.Shot.performed += ctx => OnAttack();
        }
    }

    void Update()
    {
        Vector3 newPos = transform.position;

        if (playerNumber == 1)
        {
            if (stickInput.magnitude > 0.1f)
            {
                Vector3 move = new Vector3(stickInput.x, stickInput.y, 0f);
                newPos += move * moveSpeed * Time.deltaTime;
            }
        }
        else if (playerNumber == 2)
        {
            // マウスカーソル位置に追従
            if (Mouse.current != null)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                mousePosition.z = 10f;
                newPos = Camera.main.ScreenToWorldPoint(mousePosition);
            }
        }

        transform.position = newPos;
    }

    void OnAttack()
    {
        if (!shoot_allow) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "SampleScene")
        {
            Invoke(nameof(ActivateHitbox), 0.2f);
            Invoke(nameof(DeactivateHitbox), 0.25f);
        }
        else if (currentScene == "Title")
        {
            var screenFader = FindObjectOfType<ScreenFader>();
            if (screenFader != null) StartCoroutine(screenFader.BackBlack());
            Invoke(nameof(GoGame), 2.0f);
        }
    }

    void ActivateHitbox() => hitbox?.SetActive(true);
    void DeactivateHitbox() => hitbox?.SetActive(false);
    void GoGame() => SceneManager.LoadScene("SampleScene");

    void OnDestroy()
    {
        if (playerNumber == 1) inputActions.Player.Disable();
        else if (playerNumber == 2)
        {
            inputActions.Player2.Enable();
            inputActions.Player2.Shot.performed += ctx => OnAttack(); // 右クリック時
        }
    }
}