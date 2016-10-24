using UnityEngine;
using System.Collections;

public class CameraLerpScript : MonoBehaviour {

	private GameObject player;
	private float timeSinceFire;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		timeSinceFire = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		Vector3 position = new Vector3 (Input.mousePosition.x - Screen.width / 2, 0, Input.mousePosition.y - Screen.height / 2);
		if (!Input.GetButton ("Fire")) {
			if (timeSinceFire >= 0.3f) {	
				position = -transform.localPosition * 60;
				transform.localPosition = Vector3.Lerp (transform.localPosition, position, .0002f);
			} else {
				transform.localPosition = Vector3.Lerp (transform.localPosition, position, .0002f);
			}
		} else {
			timeSinceFire = 0;
			transform.localPosition = Vector3.Lerp (transform.localPosition, position, .0002f);
		}

		if (transform.localPosition.z > 5) {
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, 5);
		}
		if (transform.localPosition.z < -5) {
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -5);
		}
		if (transform.localPosition.x > 10) {
			transform.localPosition = new Vector3 (10, transform.localPosition.y, transform.localPosition.z);
		}
		if (transform.localPosition.x < -10) {
			transform.localPosition = new Vector3 (-10, transform.localPosition.y, transform.localPosition.z);
		}
		timeSinceFire += Time.deltaTime;
	}
}
