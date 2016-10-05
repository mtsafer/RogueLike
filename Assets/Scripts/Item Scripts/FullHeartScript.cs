using UnityEngine;
using System.Collections;

public class FullHeartScript : MonoBehaviour {

	GameObject heartUI;

	// Use this for initialization
	void Start () {
		heartUI = GameObject.FindGameObjectWithTag ("Canvas");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (other.GetComponent<PlayerScript> ().health < other.GetComponent<PlayerScript> ().maxHealth) {
				other.GetComponent<PlayerScript> ().health += 2;
				Destroy (this.gameObject);
				if (other.GetComponent<PlayerScript> ().health > other.GetComponent<PlayerScript> ().maxHealth) {
					other.GetComponent<PlayerScript> ().health = other.GetComponent<PlayerScript> ().maxHealth;
				}
				heartUI.GetComponent<HeartScript> ().renderHealth (other.gameObject);
			}
		}
	}
}
