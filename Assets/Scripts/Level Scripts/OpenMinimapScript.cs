using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OpenMinimapScript : MonoBehaviour {

	public GameObject playerIcon;
	private GameObject player;
	public bool minimapOpened;
	private Camera miniCam;
	private GameObject playerImage;
	public bool canOpenMap;

	// Use this for initialization
	void Start () {
		minimapOpened = false;
		miniCam = GameObject.FindGameObjectWithTag ("Minimap Camera").GetComponent<Camera> ();
		miniCam.rect = new Rect(0.8f, 0.7f, 1, 1);
		player = GameObject.FindGameObjectWithTag ("Player");
		canOpenMap = true;
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetButtonDown("Minimap") && canOpenMap) {
			if (minimapOpened) {
				Time.timeScale = 1;
				minimapOpened = false;
				miniCam.rect = new Rect(0.8f, 0.7f, 1, 1);
				miniCam.GetComponent<MinimapScript> ().enabled = true;
				Destroy (playerImage.gameObject);

			} else {
				Time.timeScale = 0;
				minimapOpened = true;
				miniCam.rect = new Rect(0, 0, 1, 1);
				miniCam.GetComponent<MinimapScript> ().enabled = false;
				playerImage = Instantiate (playerIcon, new Vector3(player.transform.position.x, 1, player.transform.position.z), new Quaternion(0,0,0,transform.rotation.w)) as GameObject;
			}
		}

		if (minimapOpened){
			if (Input.GetButton ("Right")) {
				miniCam.transform.position = new Vector3 (miniCam.transform.position.x + 2, miniCam.transform.position.y, miniCam.transform.position.z);
			}
			if (Input.GetButton ("Left")) {
				miniCam.transform.position = new Vector3 (miniCam.transform.position.x - 2, miniCam.transform.position.y, miniCam.transform.position.z);
			}
			if (Input.GetButton ("Up")) {
				miniCam.transform.position = new Vector3 (miniCam.transform.position.x, miniCam.transform.position.y, miniCam.transform.position.z + 2);
			}
			if (Input.GetButton ("Down")) {
				miniCam.transform.position = new Vector3 (miniCam.transform.position.x, miniCam.transform.position.y, miniCam.transform.position.z - 2);
			}
		}
	}
}
