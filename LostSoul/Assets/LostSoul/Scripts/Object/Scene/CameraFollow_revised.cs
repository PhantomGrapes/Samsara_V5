using UnityEngine;
using System.Collections;

public class CameraFollow_revised : MonoBehaviour
{

	public Transform target;
	Camera myCam;

	public bool zoom;

	public float zoomRatio;


	float defaultRatio;

	// Use this for initialization
	void Start ()
	{
		myCam = GetComponent<Camera> ();
		defaultRatio = myCam.orthographicSize;
	}
	
	// Update is called once per frame
	void Update ()
	{

		DecideZoom ();




	}


	void DecideZoom ()
	{
		BossGreenLotus lei = FindObjectOfType<BossGreenLotus> ();


		if (target) {
			if (lei.inBattle) {
				zoom = true;
				zoomRatio = Mathf.Max(lei.targetDistance / 2f, defaultRatio/1.5f);

				Vector3 leiPosition = lei.transform.position;
				Vector3 camPosition = new Vector3 ((target.position.x + leiPosition.x) / 2f, (target.position.y + leiPosition.y) / 2f
					, (target.position.z + leiPosition.z) / 2f);
					
				transform.position = Vector3.Lerp (transform.position, camPosition, 0.05f) - new Vector3 (0, 0, 10f);
			} else {
				zoom = false;
				transform.position = Vector3.Lerp (transform.position, target.position, 0.05f) - new Vector3 (0, 0, 10f);
			}


			if (zoom) {
				Zoom ();
			} else {
				Unzoom ();
			}

		}
	}

	void Zoom ()
	{
		myCam.orthographicSize = Mathf.Lerp (myCam.orthographicSize, zoomRatio, 0.05f);
	}


	void Unzoom ()
	{
		myCam.orthographicSize = Mathf.Lerp (myCam.orthographicSize, defaultRatio, 0.05f);
	}
}
