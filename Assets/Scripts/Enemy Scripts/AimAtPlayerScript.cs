using UnityEngine;
using System.Collections;

public class AimAtPlayerScript : MonoBehaviour {

	private GameObject player;
	public Vector3 forward;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		forward = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (player.transform.position);
		forward = transform.forward;
	}
}
