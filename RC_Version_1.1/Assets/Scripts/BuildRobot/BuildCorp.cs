using UnityEngine;
using System.Collections;

public class BuildCorp : MonoBehaviour {
    public GameObject loadedCorp;
    public GameObject selectedCorp;
    public GameObject originalCorp;
    public GameObject character;

    public int selectedID;
    public int originalID;

    public bool firstCorpPart;


    void Start () {
        firstCorpPart = true;
	}
	
	void Update () {
	    if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Button>().myPartType.ToString() == "Corp")
                {
                    selectedID = DetectCorpID(hit);
                    selectedCorp = hit.transform.gameObject.GetComponent<Button>().myPart;
                    if (firstCorpPart)
                    {
                        GenerateFirstPart();
                        firstCorpPart = !firstCorpPart;
                    }
                    else if (selectedID != originalID)
                    {
                        ReplacePart(selectedCorp, originalCorp);
                    }
                }
            }
        }
	}
    int DetectCorpID(RaycastHit part)
    {
        return part.transform.gameObject.GetComponent<Button>().myPartID;
    }

    void GenerateFirstPart()
    {
        originalCorp = (GameObject)Instantiate(selectedCorp, character.transform);
        originalCorp.transform.position = character.transform.position;
        originalID = selectedID;
        Camera.main.GetComponent<CharDesc>().characterHP += originalCorp.GetComponent<Item>().itemHP;
        Camera.main.GetComponent<CharDesc>().characterSpeed += originalCorp.GetComponent<Item>().itemSpeed;
        Camera.main.GetComponent<CharDesc>().characterAttack += originalCorp.GetComponent<Item>().itemAttack;
    }

    void ReplacePart(GameObject go1, GameObject go2)
    {
        originalCorp = (GameObject)Instantiate(go1, character.transform);
        originalCorp.transform.position = character.transform.position;
        originalID = selectedID;
        Camera.main.GetComponent<CharDesc>().characterHP = Camera.main.GetComponent<CharDesc>().characterHP + go1.GetComponent<Item>().itemHP - go2.GetComponent<Item>().itemHP;
        Camera.main.GetComponent<CharDesc>().characterSpeed = Camera.main.GetComponent<CharDesc>().characterSpeed + go1.GetComponent<Item>().itemSpeed - go2.GetComponent<Item>().itemSpeed;
        Camera.main.GetComponent<CharDesc>().characterAttack = Camera.main.GetComponent<CharDesc>().characterAttack + go1.GetComponent<Item>().itemAttack - go2.GetComponent<Item>().itemAttack;
        Destroy(go2);
    }
}
