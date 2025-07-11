using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    //[SerializeField] private GameObject choicePanel;

    //[SerializeField] private Button[] choiceButtons;

    [SerializeField] private ScenarioData[] scenarioDatas;

    [SerializeField] private Image backGround;

    [SerializeField] private Image characterImage, characterImageTwo;

    [SerializeField] private Text scenarioText;

    [SerializeField] private Text characterName;

    //[SerializeField] private SceneController scenecontroller;

    public int scenarioIndex;

    public int textIndex;

    private NewActions testAction;

    public Animator CharacterAnimator;

    //[SerializeField, AddComponentMenu("音声チェンジャー")] private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        //SetScenarioElement(scenarioIndex, textIndex);
        //testAction = new NewActions();
        //testAction.Enable();

        testAction = new NewActions();   // ① 先に作る
        testAction.Enable();             // ② 有効化する

        SetScenarioElement(scenarioIndex, textIndex); // ③ その後にUI等をいじる
    }

    // Update is called once per frame
    void Update()
    {
        if (testAction.Player.Shot.triggered)
        {
            if (scenarioDatas[scenarioIndex].scenario[textIndex].choiceflag == false)
            {
                textIndex++;
                SetScenarioElement(scenarioIndex, textIndex);
                ProgressinCheck(scenarioIndex);
            }
        }
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    textIndex++;
        //    SetScenarioElement(scenarioIndex, textIndex);
        //}
    }

    private void SetScenarioElement(int _scenarioIndex, int _textIndex)
    {
        Debug.Log($"ScenarioIndex: {_scenarioIndex}, TextIndex: {_textIndex}, animflag: {scenarioDatas[_scenarioIndex].scenario[_textIndex].misakiflag}");

        backGround.sprite = scenarioDatas[_scenarioIndex].scenario[_textIndex].BackGround;

        characterImage.sprite = scenarioDatas[_scenarioIndex].scenario[_textIndex].CharacterImage;

        characterImageTwo.sprite = scenarioDatas[_scenarioIndex].scenario[_textIndex].characterImageTwo;


        scenarioText.text = scenarioDatas[_scenarioIndex].scenario[_textIndex].ScenarioText;

        characterName.text = scenarioDatas[_scenarioIndex].scenario[_textIndex].CharacterName;

        if (scenarioDatas[_scenarioIndex].scenario[_textIndex].misakiflag == true)//unityちゃんのアニメーション実行,およびミサキちゃん出現中
        {
            CharacterAnimator.SetBool("moveFlag", true);
            characterImageTwo.gameObject.SetActive(true);
        }
        if (scenarioDatas[_scenarioIndex].scenario[_textIndex].misakiflag == false)//unityちゃんのアニメーション実行,およびミサキちゃん出現中
        {
            CharacterAnimator.SetBool("moveFlag", false);

            characterImageTwo.gameObject.SetActive(false);
        }

        if (scenarioDatas[_scenarioIndex].scenario[_textIndex].choiceflag == true)//選択肢パネルが開かれた
        {
            ShowChoices(scenarioDatas[_scenarioIndex].scenario[_textIndex].choices);
        }


        //if (scenarioDatas[_scenarioIndex].scenario[_textIndex].bgmchangeflag == true)//曲が変わる
        //{
        //    soundManager.ChangeBGM();
        //}
    }

    private void ShowChoices(string[] choices)//選択肢を表示させる
    {
        //choicePanel.SetActive(true);

        //for (int i = 0; i < choiceButtons.Length; i++)
        //{
        //    if (i < choices.Length)
        //    {
        //        choiceButtons[i].gameObject.SetActive(true);
        //        choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];

        //        int index = i;

        //        choiceButtons[i].onClick.RemoveAllListeners();
        //        choiceButtons[i].onClick.AddListener(() => OnChoiceClicked(index));
        //    }
        //    else
        //    {
        //        choiceButtons[i].gameObject.SetActive(false);
        //    }
        //}
    }

    private void OnChoiceClicked(int choiceindex)//ボタンがクリックされたときの処理
    {
        switch (choiceindex)
        {
            case 0://選択肢1が選ばれた
                textIndex++;
                //SetScenarioElement(scenarioIndex, textIndex);
                ProgressinCheck(scenarioIndex);
                break;
            case 1://選択肢2が選ばれたよ
                textIndex = 0;
                scenarioIndex++;

                SetScenarioElement(scenarioIndex, textIndex);
                break;
            case 2://選択肢3が選ばれたよ
                break;

        }

        //choicePanel.SetActive(false);
    }

    private void ProgressinCheck(int _scenarioIndex)
    {
        if (textIndex < scenarioDatas[_scenarioIndex].scenario.Count)
        {
            SetScenarioElement(_scenarioIndex, textIndex);
        }
        else
        {
            //scenecontroller.FadeOut();

            //textIndex = 0;
            //scenarioIndex++;

            //SetScenarioElement(scenarioIndex, textIndex);


        }
    }
}
