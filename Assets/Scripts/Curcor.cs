using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curcor : MonoBehaviour
{
    private Vector3 screenPoint; // offsetに必要な変数。カメラにおけるオブジェクトの位置を記憶するための
    private Vector3 offset; // クリック位置を導くための補正値用の変数
    public  bool ClickPosition = false;  // クリック位置を反映させるか否か。falseなら中央をつかむようになる
    public GameObject hitbox; //攻撃判定となるオブジェクトを入力(子オブジェクトのやつ)

    //public PlayerCheck player_statecheck;
    public bool goAtack = false; //攻撃可能かどうか
    public bool shoot_reload_Flag = true; //攻撃インターバルは終わっているか
    public bool shoot_allow = true; //攻撃できる状況か(プレイヤーがひるみ状態なら不可になる)

    //コントローラー時の速さ
    float moveSpeed = 5f;

    void Start()
    {
        //
        transform.position = Vector3.zero;
    }


    void Update()
    {
        // プレイヤーが気絶状態かをチェックする
        float LX = Input.GetAxis("Horizontal");
        float LY = Input.GetAxis("Vertical");
        // shoot_allow = player_statecheck;
        //以下四行はマウスの位置に応じてオブジェクトを動かすコード
        Vector3 stickInput = new Vector3 (LX,LY,0)/*Input.mousePosition*/;  //ここでマウス位置取得
        Vector3 thisPosition = Input.mousePosition;  //ここでマウス位置取得
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(thisPosition);// カメラに合わせた座標を算出
        worldPosition.z = 0f;

        if (Input.GetJoystickNames().Length > 0 && !string.IsNullOrEmpty(Input.GetJoystickNames()[0]))
        {
            if (stickInput.magnitude > 0.1f)
            {
                transform.position += stickInput * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            this.transform.position = worldPosition; //ここで位置を反映。
        }
        // 攻撃ボタンをここで変更してください。
        if (Input.GetMouseButtonDown(0)||Input.GetButton("btnA"))
        {
            Invoke("ActivateHitbox", 0.2f); // 入力してからＮ秒後に攻撃判定を有効化
            Invoke("DeactivateHitbox", 0.25f); // 入力してからＭ秒後に攻撃判定を無効化
            //ここは攻撃があたるまでの時間をどうするか調整してください。あと、有効化から無効化の間は一瞬にしてください
        }
    }

    //　攻撃判定オブジェクトを有効化
    void ActivateHitbox()
    {
        hitbox.SetActive(true);
    }

    //　攻撃判定オブジェクトを無効化
    void DeactivateHitbox()
    {
        hitbox.SetActive(false);
    }
}
