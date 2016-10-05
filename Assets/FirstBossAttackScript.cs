using UnityEngine;
using System.Collections;

public class FirstBossAttackScript : MonoBehaviour {

	public GameObject bullet;
	public GameObject acceleratingBullet;
	public GameObject tripleBreakBullet;
	public GameObject homingBullet;

	public GameObject aimer;
	private float fireRatePerMinute;

	private float timeBetweenShots;
	private float timeSinceLastShot;
	private float maxHealth;

	private GameObject player;

	private int attackPattern;

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

	//spray bullets at player using randomized launch angle
	void sprayBullets(){
		if(Time.fixedTime % 0.3f < 0.1){
			int spread = Random.Range (-60, 61);
			GameObject spreadBullet = Instantiate (bullet, transform.position, aimer.transform.rotation) as GameObject;
			//spreadBullet.transform.LookAt (player.transform);
			Quaternion originalRotation = spreadBullet.transform.rotation;
			spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (spread, Vector3.up);
		}
	}

	//launch 3 circles of bullets in quick succession. Each bullet should get faster as it moves
	void circleBullets(){
		if (Time.fixedTime % 1 < 0.0001f) {
			for (int i = 0; i < 360; i += 10) {
				GameObject spreadBullet = Instantiate (acceleratingBullet, transform.position, transform.rotation) as GameObject;
				Quaternion originalRotation = spreadBullet.transform.rotation;
				spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i, Vector3.up);
			}
		}
	}

	//should fire 3 blast of 3 bullets each, which each break into 3 bullets, and each of those breaks into 3 bullets
	//each round of shots should emit 27 bullets, making 81 bullets total
	void tripleBreakBullets(){
		print ("triple");
		if (Time.fixedTime % 2 < 0.0001f) {
			for (int i = -1; i < 2; i += 1) {
				GameObject spreadBullet = Instantiate (tripleBreakBullet, transform.position, transform.rotation) as GameObject;
				Quaternion originalRotation = spreadBullet.transform.rotation;
				spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i * 90, Vector3.up);
			}
		}
	}

	//should fire a srpread of bullets which have slight homing capabilites
	void homingBullets(){
		print ("Homing");
		if (Time.fixedTime % 1 < 0.0001f) {
			Instantiate (homingBullet, transform.position, transform.rotation);
		}
	}

	//do 2 or more of the other attacks at once
	void bulletHell(){
		print ("BulletHell");
		int numberOfAttacks = Random.Range (1, 3);
		int[] attackList = new int[numberOfAttacks];
		for (int i = 0; i < numberOfAttacks; i++) {
			int attack = Random.Range (0, 4);
			while (contains (attackList, attack)) {
				attack = Random.Range (0, 4);
			}
			attackList [i] = attack;
		}
		foreach (int attack in attackList) {
			if (Time.fixedTime % 1 < 0.0001f) {
				if (attack == 0) {
					int spread = Random.Range (-60, 61);
					GameObject spreadBullet = Instantiate (bullet, transform.position, aimer.transform.rotation) as GameObject;
					//spreadBullet.transform.LookAt (player.transform);
					Quaternion originalRotation = spreadBullet.transform.rotation;
					spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (spread, Vector3.up);
				} else if (attack == 1) {
					for (int i = 0; i < 360; i += 10) {
						GameObject spreadBullet = Instantiate (acceleratingBullet, transform.position, transform.rotation) as GameObject;
						Quaternion originalRotation = spreadBullet.transform.rotation;
						spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i, Vector3.up);
					}
				} else if (attack == 2) {
					for (int i = -1; i < 2; i += 1) {
						GameObject spreadBullet = Instantiate (tripleBreakBullet, transform.position, transform.rotation) as GameObject;
						Quaternion originalRotation = spreadBullet.transform.rotation;
						spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i * 90, Vector3.up);
					}
				} else if (attack == 3) {
					Instantiate (homingBullet, transform.position, transform.rotation);
				}
			}
		}
	}

	bool contains(int[] list, int number){
		foreach (int attack in list) {
			if (attack == number) {
				return true;
			}
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		fireRatePerMinute = GetComponentInParent<EnemyAttributesScript> ().fireRatePerMinute;
		timeBetweenShots = 60 / fireRatePerMinute;
		timeSinceLastShot = 0;
		player = GameObject.FindGameObjectWithTag ("Player");
		maxHealth = gameObject.GetComponent<EnemyAttributesScript> ().health;
		attackPattern = Random.Range(0,5);
	}

	// Update is called once per frame
	void Update () {
		//player is on 10
		int layermask = 1 << 10;
		if (Physics.CheckBox(transform.position, new Vector3(2.6f, 0.5f, 2.6f), transform.rotation, layermask)) {
			player.GetComponent<PlayerScript> ().takeDamage ();
		}
		if (timeSinceLastShot > timeBetweenShots){
			chanceToStop ();
			attackPattern = Random.Range (0,5);

			if (gameObject.GetComponent<EnemyAttributesScript> ().health < maxHealth / 3) {
				attackPattern = 4;
			} else if (attackPattern == 4) {
				while (attackPattern == 4) {
					attackPattern = Random.Range (0,5);
				}
			}
		
			timeSinceLastShot = 0;

			print ("New pattern selected!");

			//bulletSpawnLocation = transform.position;
			//Instantiate (bullet, bulletSpawnLocation, transform.rotation);
			//timeSinceLastShot = 0;
		}

		if (attackPattern == 0) {
			sprayBullets ();
		} else if (attackPattern == 1) {
			circleBullets ();
		} else if (attackPattern == 2) {
			tripleBreakBullets ();
		} else if (attackPattern == 3) {
			homingBullets ();
		} else if (attackPattern == 4 && gameObject.GetComponent<EnemyAttributesScript> ().health < maxHealth / 3) {
			bulletHell ();
		}

		timeSinceLastShot += Time.deltaTime;
	}
}
