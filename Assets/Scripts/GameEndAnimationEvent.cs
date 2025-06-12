using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndAnimationEvent : MonoBehaviour
{
    
    public void GameEnd()
    {
        SceneManager.LoadScene("Result");
    }
}
