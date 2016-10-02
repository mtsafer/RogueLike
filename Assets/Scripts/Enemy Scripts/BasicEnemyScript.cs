using UnityEngine;
using System.Collections;

public class BasicEnemyScript : MonoBehaviour {


	private GameObject player;

	public Rigidbody rb;

	private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		rb = gameObject.GetComponent<Rigidbody> ();
		agent = gameObject.GetComponent<NavMeshAgent> ();
	}

	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (transform.position, player.transform.position) < 1.1f) {
			player.GetComponent<PlayerScript> ().takeDamage ();
		}
		transform.position = new Vector3 (transform.position.x, 1, transform.position.z);
		if (GetComponent<EnemyAttributesScript>().timeSinceStun > 0.3f) {
			GetComponent<EnemyAttributesScript> ().stunned = false;
		}
		agent.SetDestination (player.transform.position);
		//transform.LookAt(player.transform.position);
		//rb.velocity = transform.forward * 3;


		if (gameObject.GetComponent<EnemyAttributesScript> ().health <= 0) {
			Destroy (this.gameObject);
		}

	}
}
