using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FTsample_ClickToSpawn : MonoBehaviour {

	public GameObject camObject;
	public GameObject lightObject;
	public GameObject robotObject;
	public Text prefabName;
	public GameObject[] particlePrefab;
	public int particleNum = 0;

	GameObject effectPrefab;
	bool checkEffect = false;
	bool checkChara = true;
	bool checkLight = true;
	bool checkCamera = true;
	Animator camAnim;

	void Start () {
		camAnim = camObject.GetComponent<Animator>();
	}
	

	void Update () {
		if (checkEffect == false) {
			effectPrefab = Instantiate (particlePrefab [particleNum],
			new Vector3 (0, 0, 0), Quaternion.Euler (0, 0, 0))as GameObject;
			checkEffect = true;
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow)){
			Destroy(effectPrefab);
			particleNum -= 1;
			if( particleNum < 0) {
				particleNum = particlePrefab.Length-1;
			}
			checkEffect = false;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)){
			Destroy(effectPrefab);
			particleNum += 1;
			if(particleNum >(particlePrefab.Length - 1)) {
				particleNum = 0;
			}
			checkEffect = false;
		}		
		prefabName.text= particlePrefab[particleNum].name;
	
	}

	public void OnClick_cam() {
		if(checkCamera == true){
			camAnim.speed = 0f;
			checkCamera = false;
			return;
		}
		if(checkCamera == false){
			camAnim.speed = 1f;
			checkCamera = true;
			return;
		}
	}

	public void OnClick_light() {
		if(checkLight == true){
			lightObject.SetActive(false);
			checkLight = false;
			return;
		}
		if(checkLight == false){
			lightObject.SetActive(true);
			checkLight = true;
			return;
		}
	}

	public void OnClick_chara() {
		if(checkChara == true){
			robotObject.SetActive(false);
			checkChara = false;
			return;
		}
		if(checkChara == false){
			robotObject.SetActive(true);
			checkChara = true;
			return;
		}
	}
}
