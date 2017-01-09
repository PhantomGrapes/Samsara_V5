using UnityEngine;
using System.Collections;

public class BuildWeapon : MonoBehaviour {
    public GameObject loadedWeapon;
    public GameObject selectedWeapon;
    public GameObject originalWeapon;
    public GameObject character;

    public int selectedID;
    public int originalID;

    public bool firstWeaponPart;


    void Start()
    {
        firstWeaponPart = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Button>().myPartType.ToString() == "Weapon")
                {
                    selectedID = DetectCorpID(hit);
                    selectedWeapon = hit.transform.gameObject.GetComponent<Button>().myPart;
                    if (firstWeaponPart)
                    {
                        GenerateFirstPart();
                        firstWeaponPart = !firstWeaponPart;
                    }
                    else if (selectedID != originalID)
                    {
                        ReplacePart(selectedWeapon, originalWeapon);
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
        originalWeapon = (GameObject)Instantiate(selectedWeapon, character.transform);
        originalWeapon.transform.position = new Vector3(character.transform.position.x+0.1f, character.transform.position.y, character.transform.position.z);
        originalID = selectedID;
        Camera.main.GetComponent<CharDesc>().characterHP += originalWeapon.GetComponent<Item>().itemHP;
        Camera.main.GetComponent<CharDesc>().characterSpeed += originalWeapon.GetComponent<Item>().itemSpeed;
        Camera.main.GetComponent<CharDesc>().characterAttack += originalWeapon.GetComponent<Item>().itemAttack;

    }

    void ReplacePart(GameObject go1, GameObject go2)
    {
        originalWeapon = (GameObject)Instantiate(go1, character.transform);
        originalWeapon.transform.position = new Vector3(character.transform.position.x+0.1f, character.transform.position.y, character.transform.position.z);
        originalID = selectedID;
        Camera.main.GetComponent<CharDesc>().characterHP = Camera.main.GetComponent<CharDesc>().characterHP + go1.GetComponent<Item>().itemHP - go2.GetComponent<Item>().itemHP;
        Camera.main.GetComponent<CharDesc>().characterSpeed = Camera.main.GetComponent<CharDesc>().characterSpeed + go1.GetComponent<Item>().itemSpeed - go2.GetComponent<Item>().itemSpeed;
        Camera.main.GetComponent<CharDesc>().characterAttack = Camera.main.GetComponent<CharDesc>().characterAttack + go1.GetComponent<Item>().itemAttack - go2.GetComponent<Item>().itemAttack;
        Destroy(go2);
    }
}
