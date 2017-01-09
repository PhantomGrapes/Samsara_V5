using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public void OnPreRender()
    {
        Shader.EnableKeyword("DISTORT_OFF");
    }
    public void OnPostRender()
    {
        Shader.DisableKeyword("DISTORT_OFF");
    }
}
