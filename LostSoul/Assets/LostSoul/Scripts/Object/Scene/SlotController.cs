using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlotController : MonoBehaviour {
    float width;
    float spacing;

	
	public void Adjust () {
        spacing = GetComponent<RectTransform>().rect.width*7.1f/162;
        width = (GetComponent<RectTransform>().rect.width - 3*spacing)/4;
        
        Vector2 newSize = new Vector2(width, width);
        Vector2 newSpacing = new Vector2(spacing, spacing);
        GetComponent<GridLayoutGroup>().cellSize = newSize;
        GetComponent<GridLayoutGroup>().spacing = newSpacing;
    }

}
