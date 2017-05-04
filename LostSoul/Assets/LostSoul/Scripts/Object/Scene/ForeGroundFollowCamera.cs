using UnityEngine;
using System.Collections;

public class ForeGroundFollowCamera : MonoBehaviour {

	public GameObject target;
	float scale;
	
	void FixedUpdate () {
		transform.position = target.transform.position + new Vector3 (0f, 0f, 10f);
		scale = (target.GetComponent<CameraFollow_revised> ().zoomRatio)/target.GetComponent<Camera> ().orthographicSize*25f;
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
