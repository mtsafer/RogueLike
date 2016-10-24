using UnityEngine;
using System.Collections;

public class CameraSpawner : MonoBehaviour {

	public GameObject playerCam;
	public Camera miniCam;

	// Use this for initialization
	void Start () {
		Instantiate (playerCam, playerCam.transform.position, playerCam.transform.rotation);
		Instantiate (miniCam, miniCam.transform.position, miniCam.transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
