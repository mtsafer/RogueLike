using UnityEngine;
using System.Collections;

public class FullHeartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			if (other.GetComponent<PlayerScript> ().health < other.GetComponent<PlayerScript> ().maxHealth) {
				Destroy (this.gameObject);
			}
			other.GetComponent<PlayerScript> ().health += 2;
			if (other.GetComponent<PlayerScript> ().health > other.GetComponent<PlayerScript> ().maxHealth) {
				other.GetComponent<PlayerScript> ().health = other.GetComponent<PlayerScript> ().maxHealth;
			}
			GameObject.Find ("HealthHUDCanvas").GetComponent<HeartScript> ().renderHealth (other.gameObject);
		}
	}
}
