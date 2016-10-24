using UnityEngine;
using System.Collections;

public class DropSpinScript : MonoBehaviour {

	public GameObject minimapIcon;

	// Use this for initialization
	void Start () {
		minimapIcon = Instantiate (minimapIcon, new Vector3 (transform.position.x, 3, transform.position.z), minimapIcon.transform.rotation) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(0, 0, 2));
	}
}
