using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
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

    //遷移フラグ
    private static bool isSceneTransitioning = false;

    private void Start()
    {
        int connectedGamepads = Gamepad.all.Count;
        Debug.Log("接続されているゲームパッドの数: " + connectedGamepads);
    }

    private void OnCursorPerformed(InputAction.CallbackContext ctx)
    {
        stickInput = ctx.ReadValue<Vector2>();
    }

    private void OnCursorCanceled(InputAction.CallbackContext ctx)
    {
        stickInput = Vector2.zero;
    }

    private void OnShotPerformed(InputAction.CallbackContext ctx)
    {
        OnAttack();
    }

    void Awake()
    {
        inputActions = new NewActions();

        // ゲームパッドを取得
        if (Gamepad.all.Count >= playerNumber)
        {
            var gamepad = Gamepad.all[playerNumber - 1];

            // InputUserとGamepadを紐づけ
            var user = InputUser.CreateUserWithoutPairedDevices();
            user.AssociateActionsWithUser(inputActions);
            InputUser.PerformPairingWithDevice(gamepad, user);
        }

        if (playerNumber == 1)
        {
            inputActions.Player.Enable();
            inputActions.Player.Cursor.performed += OnCursorPerformed;
            inputActions.Player.Cursor.canceled += OnCursorCanceled;
            inputActions.Player.Shot.performed += OnShotPerformed;
        }
        else if (playerNumber == 2)
        {
            inputActions.Player2.Enable();
            inputActions.Player2.Cursor.performed += OnCursorPerformed;
            inputActions.Player2.Cursor.canceled += OnCursorCanceled;
            inputActions.Player2.Shot.performed += OnShotPerformed;
        }
    }

    void Update()
    {
        Vector3 newPos = transform.position;

        if (stickInput.magnitude > 0.1f)
        {
            Vector3 move = new Vector3(stickInput.x, stickInput.y, 0f);
            newPos += move * moveSpeed * Time.deltaTime;
        }

        transform.position = newPos;
    }

    void OnAttack()
    {
        //if (!shoot_allow) return;

        //string currentScene = SceneManager.GetActiveScene().name;

        //if (currentScene == "SampleScene")
        //{
        //    StartCoroutine(HitboxRoutine());
        //}
        //else if (currentScene == "Title")
        //{
        //    if (isSceneTransitioning) return; // ← すでに遷移中なら何もしない

        //    isSceneTransitioning = true;

        //    var screenFader = FindObjectOfType<ScreenFader>();
        //    if (screenFader != null) StartCoroutine(screenFader.BackBlack());

        //    StartCoroutine(GoGameAfterDelay(2.0f));
        //}

        //Debug.Log("攻撃が当たりました: " + playerNumber);

        if (!shoot_allow) return;

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"[{playerNumber}] 自身のGameObjectが非アクティブのため、アクティブにします: {gameObject.name}");
            gameObject.SetActive(true);
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "SampleScene")
        {
            StartCoroutine(HitboxRoutine());
        }
        else if (currentScene == "Title")
        {
            if (isSceneTransitioning) return;

            isSceneTransitioning = true;

            var screenFader = FindObjectOfType<ScreenFader>();
            if (screenFader != null) StartCoroutine(screenFader.BackBlack());

            StartCoroutine(GoGameAfterDelay(2.0f));
        }

        Debug.Log("攻撃が当たりました: " + playerNumber);

    }

    //IEnumerator HitboxRoutine()
    //{
    //    if (hitbox != null) hitbox.SetActive(true);
    //    yield return new WaitForSeconds(0.05f);
    //    if (hitbox != null) hitbox.SetActive(false);
    //}

    IEnumerator HitboxRoutine()
    {
        if (hitbox != null)
        {
            Debug.Log("hitbox ON: " + gameObject.name);
            hitbox.SetActive(true);
        }
        else
        {
            Debug.LogWarning("hitbox が設定されていません: " + gameObject.name);
        }

        yield return new WaitForSeconds(0.1f);

        if (hitbox == null)
        {
            hitbox.SetActive(false);
            Debug.Log("hitbox OFF: " + gameObject.name);
        }

        Debug.Log("超えました ");
    }

    IEnumerator GoGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this != null) SceneManager.LoadScene("SampleScene");
    }

    void OnDestroy()
    {
        if (inputActions != null)
        {
            if (playerNumber == 1)
            {
                inputActions.Player.Cursor.performed -= OnCursorPerformed;
                inputActions.Player.Cursor.canceled -= OnCursorCanceled;
                inputActions.Player.Shot.performed -= OnShotPerformed;
                inputActions.Player.Disable();
            }
            else if (playerNumber == 2)
            {
                inputActions.Player2.Cursor.performed -= OnCursorPerformed;
                inputActions.Player2.Cursor.canceled -= OnCursorCanceled;
                inputActions.Player2.Shot.performed -= OnShotPerformed;
                inputActions.Player2.Disable();
            }
        }
    }

}