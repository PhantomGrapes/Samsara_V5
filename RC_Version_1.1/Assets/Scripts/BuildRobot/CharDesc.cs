using UnityEngine;
using System.Collections;

public class CharDesc : MonoBehaviour {
    public int descPannelX;
    public int descPannelY;

    public string characterPannel;
    public string characterName;
    public string characterDesc;

    public int characterAttack;
    public int characterSpeed;
    public int characterHP;

    void Start()
    {
        characterName = "Name";
        characterDesc = "NonDescription";
        characterAttack = 0;
        characterSpeed = 0;
        characterHP = 0;
        descPannelX = 200;
        descPannelY = 200;
    }

    void Update()
    {
        CreatCharacPannel();
    }

    string CreatCharacPannel()
    {
        characterPannel = "<color=#ffffff>" + characterName + "</color>\n\n";
        characterPannel += "<color=#ffa700>" + characterDesc + "</color>\n\n";
        characterPannel += "<color=#d62d20>" + "HP: " + characterHP.ToString() + "</color>\n\n";
        characterPannel += "<color=#0057e7>" + "Attack: " + characterAttack.ToString() + "</color>\n\n";
        characterPannel += "<color=#ffa700>" + "Speed: " + characterSpeed.ToString() + "</color>\n\n";
        return characterPannel;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(350, 150, descPannelX, descPannelY), characterPannel);
    }
}
