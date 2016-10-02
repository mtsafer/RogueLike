using UnityEngine;
using System.Collections;

public class CemeraScript : MonoBehaviour {

	private GameObject target;
	public float distance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("Player");
		}
		Vector3 position = new Vector3 (target.transform.position.x, distance, target.transform.position.z-7.5f);
		transform.position = position;
	}
}
