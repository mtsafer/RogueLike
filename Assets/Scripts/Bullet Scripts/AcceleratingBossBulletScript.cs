using UnityEngine;
using System.Collections;

public class AcceleratingBossBulletScript : MonoBehaviour {

	public Rigidbody rb;

	public float accelerationRate;

	// Use this for initialization
	void Start () {
		rb.GetComponent<Rigidbody> ();
		Destroy (this.gameObject, 8f);
	}

	// Update is called once per frame
	void Update () {
		rb.AddForce (transform.forward * accelerationRate);
		//transform.Translate (0, 0, accelerationRate * Time.deltaTime);
		//if (accelerationRate < 100) {
		//	accelerationRate += 0.0001f;
		//}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player"){
			if (!other.GetComponent<PlayerMoveScript> ().invulnerable && !other.GetComponent<PlayerScript>().recentlyHit) {
				Destroy (this.gameObject);
				other.GetComponent<PlayerScript> ().takeDamage ();
			}
		} else if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Door" || other.gameObject.tag == "Floor") {
			Destroy (this.gameObject);
		}
	}
}
