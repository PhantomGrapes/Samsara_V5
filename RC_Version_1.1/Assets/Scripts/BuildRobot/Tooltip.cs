using UnityEngine;
using System.Collections;

public class Tooltip : MonoBehaviour {
    public string tooltip;
    public string mouseObjcetTag;

    public bool showTooltip;

    public int tooltipX;
    public int tooltipY;


    public GameObject mouseObject;



    void Start()
    {
        showTooltip = false;
        tooltipX = 200;
        tooltipY = 200;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mouseObject = hit.transform.gameObject;
            mouseObjcetTag = mouseObject.GetComponent<Button>().myPartType.ToString();
            if (mouseObjcetTag == "Corp" || mouseObjcetTag == "Weapon" || mouseObjcetTag == "Move")
            {
                tooltip = CreatTooltip(mouseObject);
                showTooltip = true;
            }
        }
        else
            showTooltip = false;
    }

    void OnGUI()
    {
        if (showTooltip)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - 80, tooltipX, tooltipY), tooltip);
        }
    }

    string CreatTooltip(GameObject go)
    {
        tooltip = "<color=#ffffff>" + go.GetComponent<Button>().myPartName + "</color>\n\n";
        tooltip += "<color=#ffa700>" + go.GetComponent<Button>().myPartDesc + "</color>\n\n";
        tooltip += "<color=#d62d20>" + "HP: " + go.GetComponent<Button>().myPartHP.ToString() + "</color>\n\n";
        tooltip += "<color=#0057e7>" + "Attack: " + go.GetComponent<Button>().myPartAttack.ToString() + "</color>\n\n";
        tooltip += "<color=#ffa700>" + "Speed: " + go.GetComponent<Button>().myPartSpeed.ToString() + "</color>\n\n";
        return tooltip;
    }
}
