using UnityEngine;
using System.Collections;

public class PlayerMinimapIconScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = new Quaternion(0,0,0,transform.rotation.w);
	}
}
