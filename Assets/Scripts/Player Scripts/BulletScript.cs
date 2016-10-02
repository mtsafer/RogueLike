using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public Rigidbody rb;

	private float speed;
	private float knockback;
	private float damage;
	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		rb.GetComponent<Rigidbody> ();
		Destroy (this.gameObject, 1.5f);
		damage = player.GetComponentInChildren<GunScript>().damage;
		speed = player.GetComponentInChildren<GunScript> ().projectileSpeed;
		knockback = player.GetComponentInChildren<GunScript> ().knockback;
	}
	
	// Update is called once per frame
	void Update () {
		rb.velocity = transform.up * speed;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Enemy") {
			
			//stun stuff
			if (other.GetComponent<EnemyAttributesScript> ().stunnable) {
				other.GetComponent<EnemyAttributesScript> ().stunned = true;
				other.GetComponent<EnemyAttributesScript> ().timeSinceStun = 0;
			}
			//knockback stuff
			if (other.GetComponent<EnemyAttributesScript> ().canBeKnockedBack) {
				other.GetComponent<Rigidbody> ().AddForce(other.gameObject.transform.forward * -knockback);
			}

			Destroy (this.gameObject);
			other.GetComponent<EnemyAttributesScript> ().health -= damage;

		} else if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Door") {
			Destroy (this.gameObject);
		}
	}
}
