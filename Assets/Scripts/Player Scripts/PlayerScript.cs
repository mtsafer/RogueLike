using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public float health;

	public float maxHealth;
	public int luck;
	private float timeSinceHit;
	private int enemyLayer;
	private int playerLayer;
	public bool recentlyHit;

	public Slider reloadSlider;

	private GameObject healthHUD;

	public void takeDamage () {
		if(timeSinceHit > 2) {
			health -= 1;
			timeSinceHit = 0;
			Physics.IgnoreLayerCollision (enemyLayer, playerLayer, true);
			recentlyHit = true;
			healthHUD.GetComponent<HeartScript> ().renderHealth (this.gameObject);
		}
	}

	void blinkCharacter() {
		if (Time.fixedTime % 0.8f < 0.5f) {
			GetComponent<MeshRenderer> ().enabled = false;
		} else {
			GetComponent<MeshRenderer> ().enabled = true;
		}
	}

	// Use this for initialization
	void Start () {
		timeSinceHit = 0;
		//enemy layer is 9
		enemyLayer = 9;
		//player layer is 10
		playerLayer = 10;
		recentlyHit = false;

		maxHealth = health;

		healthHUD = GameObject.Find ("HealthHUDCanvas");
		healthHUD.GetComponent<HeartScript> ().renderHealth (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {

		if (health > maxHealth) {
			maxHealth = health;
		}

		transform.position = new Vector3 (transform.position.x, 1, transform.position.z);

		if (timeSinceHit > 2) {
			Physics.IgnoreLayerCollision (enemyLayer, playerLayer, false);
			recentlyHit = false;
		}

		if (recentlyHit) {
			blinkCharacter ();
		} else if (!GetComponent<MeshRenderer>().enabled) {
			GetComponent<MeshRenderer> ().enabled = true;
		}

		timeSinceHit += Time.deltaTime;
	}
}
