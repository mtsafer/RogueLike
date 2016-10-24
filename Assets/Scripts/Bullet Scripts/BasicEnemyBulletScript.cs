using UnityEngine;
using System.Collections;

public class BasicEnemyBulletScript : MonoBehaviour {

	public Rigidbody rb;

	public float speed;

	// Use this for initialization
	void Start () {
		rb.GetComponent<Rigidbody> ();
		Destroy (this.gameObject, 2f);
	}

	// Update is called once per frame
	void Update () {
		rb.velocity = transform.up * speed;
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
