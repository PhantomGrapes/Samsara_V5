using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemData : MonoBehaviour {

    public int id;
    public GameObject item;
    public int amount=0;
    InformationController infoBoard;
    LevelUpButtonController[] levelUpButtons;
    MakeEssenceButtonController makeEssenceButton;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
        infoBoard = FindObjectOfType<InformationController>();
        levelUpButtons = FindObjectsOfType<LevelUpButtonController>();
        makeEssenceButton = FindObjectOfType<MakeEssenceButtonController>();
    }

    void TaskOnClick()
    {
        infoBoard.info.text = infoBoard.inventory.database.FetchItemById(id).Description;
        infoBoard.itemSelected = id;
        if (id > -1 && id < 6) {
            for(int i = 0; i<levelUpButtons.Length;i++)
            {
                levelUpButtons[i].gameObject.SetActive(true);
            }
        }
        if (id == 12) {
            makeEssenceButton.gameObject.SetActive(true);
        }
    }
}
