using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("�X�R�A")]
    Text damaggeScore;

    public static ScoreManager Instance;

    //[SerializeField] EnemyController enemyController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject); // ���łɂ���Ȃ�j��
        }
    }


    public int getScore = 0;



    // Start is called before the first frame update
    void Start()
    {
        //SetupTextReference();
    }


    void SetupTextReference()
    {
        GameObject obj = GameObject.Find("DamageScoreText");
        if (obj != null)
        {
            damaggeScore = obj.GetComponent<Text>();
            Debug.Log("DamageScoreText ���擾���܂���: " + damaggeScore.name);
        }
        else
        {
            Debug.LogWarning("DamageScoreText ��������܂���ł����i�Ċm�F���Ă��������j");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // UI�̎Q�Ƃ��擾
        SetupTextReference();

        // title�Ȃ�X�R�A�����Z�b�g
        if (scene.name == "Title")
        {
            getScore = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //getScore=enemyController.enemy_hp;

        if (damaggeScore != null)
        {
            damaggeScore.text = "Score" + getScore.ToString();
        }
    }
}
