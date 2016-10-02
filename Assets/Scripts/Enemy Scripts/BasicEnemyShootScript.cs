using UnityEngine;
using System.Collections;

public class BasicEnemyShootScript : MonoBehaviour {

	public GameObject bullet;
	private float fireRatePerMinute;

	private float timeBetweenShots;
	private float timeSinceLastShot;

	private GameObject player;

	private Vector3 bulletSpawnLocation;


	bool pathIsClear() {
		RaycastHit hit;
		//enemy is on layer 9
		int layermask = 1 << 9;
		layermask = ~layermask;
		if (Physics.Raycast(transform.position, transform.up, out hit, Mathf.Infinity, layermask)){
			if (hit.collider.gameObject.tag == ("Player")){
				return true;
			} else {
				return false;
			}
		}
		//no walls between enemy and player
		return false;
	}

	void chanceToStop(){

	}

	// Use this for initialization
	void Start () {
		fireRatePerMinute = GetComponentInParent<EnemyAttributesScript> ().fireRatePerMinute;
		timeBetweenShots = 60 / fireRatePerMinute;
		timeSinceLastShot = 0;
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		if (timeSinceLastShot > timeBetweenShots && !GetComponentInParent<EnemyAttributesScript>().stunned){
			if (Vector3.Distance(player.transform.position, transform.position) < GetComponentInParent<EnemyAttributesScript>().agroRange ) {
				if (pathIsClear()){	
					chanceToStop ();
					bulletSpawnLocation = transform.position;
					Instantiate (bullet, bulletSpawnLocation, transform.rotation);
					timeSinceLastShot = 0;
				}
			}
		}
		timeSinceLastShot += Time.deltaTime;
	}
}
