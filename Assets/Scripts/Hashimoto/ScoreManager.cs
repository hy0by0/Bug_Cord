using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("ÉXÉRÉA")]
    [SerializeField] Text damaggeScore;

    [SerializeField] EnemyController enemyController;


    public int getScore = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //getScore=enemyController.enemy_hp;


        damaggeScore.text = "Score"+getScore.ToString();
    }
}
