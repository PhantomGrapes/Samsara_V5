using UnityEngine;
using System.Collections;

public class BuildMove : MonoBehaviour {
    public GameObject loadedMove;
    public GameObject selectedMove;
    public GameObject originalMove;
    public GameObject character;

    public int selectedID;
    public int originalID;

    public bool firstMovePart;


    void Start()
    {
        firstMovePart = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Button>().myPartType.ToString() == "Move")
                {
                    selectedID = DetectCorpID(hit);
                    selectedMove = hit.transform.gameObject.GetComponent<Button>().myPart;
                    if (firstMovePart)
                    {
                        GenerateFirstPart();
                        firstMovePart = !firstMovePart;
                    }
                    else if (selectedID != originalID)
                    {
                        ReplacePart(selectedMove, originalMove);
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
        originalMove = (GameObject)Instantiate(selectedMove, character.transform);
        originalMove.transform.position = new Vector3(character.transform.position.x, character.transform.position.y - 1f, character.transform.position.z);
        originalID = selectedID;
        Camera.main.GetComponent<CharDesc>().characterHP += originalMove.GetComponent<Item>().itemHP;
        Camera.main.GetComponent<CharDesc>().characterSpeed += originalMove.GetComponent<Item>().itemSpeed;
        Camera.main.GetComponent<CharDesc>().characterAttack += originalMove.GetComponent<Item>().itemAttack;

    }

    void ReplacePart(GameObject go1, GameObject go2)
    {
        originalMove = (GameObject)Instantiate(go1, character.transform);
        originalMove.transform.position = new Vector3(character.transform.position.x, character.transform.position.y - 1f, character.transform.position.z);
        originalID = selectedID;
        Camera.main.GetComponent<CharDesc>().characterHP = Camera.main.GetComponent<CharDesc>().characterHP + go1.GetComponent<Item>().itemHP - go2.GetComponent<Item>().itemHP;
        Camera.main.GetComponent<CharDesc>().characterSpeed = Camera.main.GetComponent<CharDesc>().characterSpeed + go1.GetComponent<Item>().itemSpeed - go2.GetComponent<Item>().itemSpeed;
        Camera.main.GetComponent<CharDesc>().characterAttack = Camera.main.GetComponent<CharDesc>().characterAttack + go1.GetComponent<Item>().itemAttack - go2.GetComponent<Item>().itemAttack;
        Destroy(go2);
    }
}
