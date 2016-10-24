using UnityEngine;
using System.Collections;

// This is the boss attack pattern script, which is attached to the first boss.
// The public GameObjects are assigned in the Unity Editor, and include the different projectiles as well as an aim assist object.

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
	private int attackPattern;
	private GameObject player;
	private Vector3 bulletSpawnLocation;

	// Returns true if there is a clear line of sight between the boss and the player.
	// Returns false otherwise.
	bool pathIsClear() {
		RaycastHit hit;
		// Enemy tag is on layer 9
		int layermask = 1 << 9;
		layermask = ~layermask;
		if (Physics.Raycast(transform.position, transform.up, out hit, Mathf.Infinity, layermask)){
			if (hit.collider.gameObject.tag == ("Player")){
				return true;
			} else {
				return false;
			}
		}
		return false;
	}

	// Spray bullets at player using randomized launch angle.
	// The "if (Time.fixedTime % 0.3f < 0.1)" is used as a rate of fire modifier.
	void sprayBullets(){
		if (Time.fixedTime % 0.3f < 0.1f) {
			int spread = Random.Range (-60, 61);
			GameObject spreadBullet = Instantiate (bullet, transform.position, aimer.transform.rotation) as GameObject;
			//spreadBullet.transform.LookAt (player.transform);
			Quaternion originalRotation = spreadBullet.transform.rotation;
			spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (spread, Vector3.up);
		}
	}

	// Launch circles of 36 bullets in quick succession. Each bullet should get faster as it moves.
	// The "if (Time.fixedTime % 1 < 0.0001f)" is used as a rate of fire modifier, essentially, it should fire once a second,
	// but every so often spawns two circles, which slightly increased the damage area of the attack. This adds to the randomness of the boss.
	void circleBullets(){
		if (Time.fixedTime % 1 < 0.0001f) {
			for (int i = 0; i < 360; i += 10) {
				GameObject spreadBullet = Instantiate (acceleratingBullet, transform.position, transform.rotation) as GameObject;
				Quaternion originalRotation = spreadBullet.transform.rotation;
				spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i, Vector3.up);
			}
		}
	}

	// Fires 3 bullets which each break into 3 bullets, and each of those breaks into 3 bullets.
	// Each round of shots should emit 27 bullets, making 81 bullets total.
	// Each time a bullet breaks into more bullets, the new bullets aim at the player, wherever the player may be.
	void tripleBreakBullets(){
		if (Time.fixedTime % 2 < 0.0001f) {
			for (int i = -1; i < 2; i += 1) {
				GameObject spreadBullet = Instantiate (tripleBreakBullet, transform.position, transform.rotation) as GameObject;
				Quaternion originalRotation = spreadBullet.transform.rotation;
				spreadBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (i * 90, Vector3.up);
			}
		}
	}

	// Fire a spread of bullets which have homing capabilites.
	void homingBullets(){
		if (Time.fixedTime % 1 < 0.0001f) {
			Instantiate (homingBullet, transform.position, transform.rotation);
		}
	}

	// Do 2 to 3 of the other attacks at once, every second.
	void bulletHell(){
		int numberOfAttacks = Random.Range (2, 4);
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

	// Custom contains() method to check if an int[] contains an int.
	bool contains(int[] list, int number){
		foreach (int attack in list) {
			if (attack == number) {
				return true;
			}
		}
		return false;
	}

	// Assigns a random attack pattern, with the rule that if the boss is below 1/3 health,
	// it must use bullet hell, and if it is above or equal to 1/3 health, it can not use bullet hell.
	void selectAttack(){
		if (timeSinceLastShot > timeBetweenShots){
			attackPattern = Random.Range (0,5);

			if (gameObject.GetComponent<EnemyAttributesScript> ().health < maxHealth / 3) {
				attackPattern = 4;
			} else {
				while (attackPattern == 4) {
					attackPattern = Random.Range (0,5);
				}
			}
			timeSinceLastShot = 0;
		}
	}

	// Calls the appropriate attack method based on the selected attack pattern.
	// During the circle of bullets attack, the boss stops moving.
	void attack(){
		if (attackPattern == 1) {
			GetComponent<NavMeshAgent> ().Stop();
		} else {
			GetComponent<NavMeshAgent> ().Resume();
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
		// Player tag is on layer 10.

		// Damage the player if the player gets too close.
		int layermask = 1 << 10;
		if (Physics.CheckBox(transform.position, new Vector3(2.6f, 0.5f, 2.6f), transform.rotation, layermask)) {
			player.GetComponent<PlayerScript> ().takeDamage ();
		}

		selectAttack ();
		attack ();

		timeSinceLastShot += Time.deltaTime;
	}
}
