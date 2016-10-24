using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

	public int value;

	private GameObject player;
	private NavMeshAgent agent;
	private bool seeking;

	public void seek(){
		agent.Resume ();
		agent.SetDestination (player.transform.position);
		seeking = true;
	}

	public void stop(){
		seeking = false;
		agent.Stop ();
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		agent = gameObject.GetComponent<NavMeshAgent> ();
		seeking = false;
	}

	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		transform.position = new Vector3 (transform.position.x, 1, transform.position.z);
		if (seeking) {
			seek ();
		}
	}

	void OnTriggerEnter( Collider other ){
		if (other.gameObject == player.gameObject) {
			player.GetComponent<PlayerScript>().gold += value;
			Destroy (this.gameObject);
		}
	}
}
