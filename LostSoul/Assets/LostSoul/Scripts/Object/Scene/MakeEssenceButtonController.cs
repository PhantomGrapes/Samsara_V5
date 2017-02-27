using UnityEngine;
using System.Collections;

public class MakeEssenceButtonController : MonoBehaviour {

    InformationController infoBoard;
    // Use this for initialization
    void Start()
    {
        infoBoard = FindObjectOfType<InformationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (infoBoard.itemSelected != 12)
            this.gameObject.SetActive(false);
    }
}
