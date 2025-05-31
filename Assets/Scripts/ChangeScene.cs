using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //  シーンの切り替えに必要

public class ChangeScene : MonoBehaviour
{
    public string sceneName; // 読み込むシーン
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ここでシーンを読み込む・遷移させる。 (呼びだして使用する場合:　入力：移動先のシーン名)
    public void Load(string name)
    {
        if (name == null) //他スクリプトから呼び出さずに使用する場合
        {
            SceneManager.LoadScene(sceneName);
        }
        else //他スクリプトから呼び出したい場合
        {
            SceneManager.LoadScene(name);
        }
        
    }
}
