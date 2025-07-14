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

    public ChangeScene changeSceneManager;

    //[SerializeField, AddComponentMenu("�����`�F���W���[")] private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        //SetScenarioElement(scenarioIndex, textIndex);
        //testAction = new NewActions();
        //testAction.Enable();

        testAction = new NewActions();   // �@ ��ɍ��
        testAction.Enable();             // �A �L��������

        SetScenarioElement(scenarioIndex, textIndex); // �B ���̌��UI����������
    }

    // Update is called once per frame
    void Update()
    {
        if (testAction.Player.Shot.triggered)
        {
            if (textIndex < scenarioDatas[scenarioIndex].scenario.Count -1)
            {
                if (scenarioDatas[scenarioIndex].scenario[textIndex].choiceflag == false)
                {
                    textIndex++;
                    SetScenarioElement(scenarioIndex, textIndex);
                    ProgressinCheck(scenarioIndex);
                }
                
            }
            else //もしインデックスがすべて表示しきったら、導入が終わったら
            {
                Debug.Log("導入終わり！！");
                changeSceneManager.Load("Main"); //ゲーム画面へ遷移させる
            }

        }else if (testAction.Player.Back.triggered)
        {
            if (textIndex >0)
            {
                if (scenarioDatas[scenarioIndex].scenario[textIndex].choiceflag == false)
                {
                    textIndex--;
                    SetScenarioElement(scenarioIndex, textIndex);
                    ProgressinCheck(scenarioIndex);
                }

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

        if (scenarioDatas[_scenarioIndex].scenario[_textIndex].misakiflag == true)//unity�����̃A�j���[�V�������s,����у~�T�L�����o����
        {
            CharacterAnimator.SetBool("moveFlag", true);
            characterImageTwo.gameObject.SetActive(true);
        }
        if (scenarioDatas[_scenarioIndex].scenario[_textIndex].misakiflag == false)//unity�����̃A�j���[�V�������s,����у~�T�L�����o����
        {
            CharacterAnimator.SetBool("moveFlag", false);

            characterImageTwo.gameObject.SetActive(false);
        }

        if (scenarioDatas[_scenarioIndex].scenario[_textIndex].choiceflag == true)//�I�����p�l�����J���ꂽ
        {
            ShowChoices(scenarioDatas[_scenarioIndex].scenario[_textIndex].choices);
        }


        //if (scenarioDatas[_scenarioIndex].scenario[_textIndex].bgmchangeflag == true)//�Ȃ��ς��
        //{
        //    soundManager.ChangeBGM();
        //}
    }

    private void ShowChoices(string[] choices)//�I������\��������
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

    private void OnChoiceClicked(int choiceindex)//�{�^�����N���b�N���ꂽ�Ƃ��̏���
    {
        switch (choiceindex)
        {
            case 0://�I����1���I�΂ꂽ
                textIndex++;
                //SetScenarioElement(scenarioIndex, textIndex);
                ProgressinCheck(scenarioIndex);
                break;
            case 1://�I����2���I�΂ꂽ��
                textIndex = 0;
                scenarioIndex++;

                SetScenarioElement(scenarioIndex, textIndex);
                break;
            case 2://�I����3���I�΂ꂽ��
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
