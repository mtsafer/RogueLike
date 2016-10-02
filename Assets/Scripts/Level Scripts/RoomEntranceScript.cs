using UnityEngine;
using System.Collections;

public class RoomEntranceScript : MonoBehaviour {


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter( Collider other){
		if(other.gameObject.tag == "Player"){
			gameObject.GetComponentInParent<RoomScript> ().playerEntered = true;
		}
	}
}
