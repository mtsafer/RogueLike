using UnityEngine;
using System.Collections;

public class MinimapScript : MonoBehaviour {

	private GameObject player;
	public float distance;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		transform.position = new Vector3 (player.transform.position.x, distance, player.transform.position.z );
	}
}
