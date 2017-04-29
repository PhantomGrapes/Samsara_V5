using UnityEngine;
using System.Collections;

public class CameraFollow_revised : MonoBehaviour
{

	public Transform target;
	Camera myCam;

	public bool zoom;

	public float zoomRatio;


	public float defaultRatio;

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
		BossRedLotus rong = FindObjectOfType<BossRedLotus> ();

		if (target) {
			if (lei.inBattle) {
				zoom = true;
				zoomRatio = Mathf.Max(lei.targetDistance / 2f, defaultRatio/1.5f);

				Vector3 leiPosition = lei.transform.position;
				Vector3 camPosition = new Vector3 ((target.position.x + leiPosition.x) / 2f, (target.position.y + leiPosition.y) / 2f
					, target.position.z + 20f);
					
				transform.position = new Vector3(Mathf.Lerp(transform.position.x, camPosition.x, 0.05f), Mathf.Lerp(transform.position.y, camPosition.y, 0.1f), -10f);
			}else if (rong.inBattle) {
				zoom = true;
				zoomRatio = Mathf.Max(lei.targetDistance / 2f, defaultRatio/1.5f);

				Vector3 rongPosition = rong.transform.position;
				Vector3 camPosition = new Vector3 ((target.position.x + rongPosition.x) / 2f, (target.position.y + rongPosition.y) / 2f
					, target.position.z + 20f);

				transform.position = new Vector3(Mathf.Lerp(transform.position.x, camPosition.x, 0.05f), Mathf.Lerp(transform.position.y, camPosition.y, 0.1f), -10f);
			} else {
				zoom = false;
				transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x, 0.05f), Mathf.Lerp(transform.position.y, target.position.y, 0.1f), -10f);
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
