using UnityEngine;
using System.Collections;

public class TreasureSpawnScript : MonoBehaviour {

	public GameObject[] treasures;

	// Use this for initialization
	void Start () {
		GameObject treasure = treasures[Random.Range(0, treasures.Length)];
		Instantiate (treasure, new Vector3(transform.position.x, transform.position.y + 0.55f, transform.position.z), transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
