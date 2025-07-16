using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScenarioData")]
public class ScenarioData : ScriptableObject
{
    public List<Scenario> scenario = new List<Scenario>();
}

[System.Serializable]
public class Scenario
{
    public Sprite BackGround;
    public Sprite CharacterImage, characterImageTwo;
    [TextArea]
    public string ScenarioText;
    public string CharacterName;
    public bool twoCharaflag;//‚à‚¤ˆêl‚ª“oê‚µ‚Ä‚¢‚é‚©


}

