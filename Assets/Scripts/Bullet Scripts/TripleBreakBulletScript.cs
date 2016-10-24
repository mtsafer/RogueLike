using UnityEngine;
using System.Collections;

public class TripleBreakBulletScript : MonoBehaviour {

	public Rigidbody rb;

	public float speed;
	public GameObject aimer;
	public GameObject breakBullet;

	private float timeSinceFired;

	// Use this for initialization
	void Start () {
		rb.GetComponent<Rigidbody> ();
		Destroy (this.gameObject, 8f);
		timeSinceFired = 0;
	}

	// Update is called once per frame
	void Update () {
		rb.velocity = transform.forward * speed;
		timeSinceFired += Time.deltaTime;
		if (timeSinceFired > 2) {
			for (int i = -1; i < 2; i += 1) {
				GameObject spreadBullet = Instantiate (breakBullet, transform.position, aimer.transform.rotation) as GameObject;
				Quaternion originalRotation = spreadBullet.transform.rotation;
				spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i * 15, Vector3.up);
			}
			Destroy (this.gameObject);
		}
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
