using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public float health;
	public float energy;
	public float maxHealth;
	public int luck;
	public int gold;
	private float timeSinceHit;
	private int enemyLayer;
	private int playerLayer;
	public bool recentlyHit;

	public Slider reloadSlider;

	public float maxEnergy;
	private GameObject healthHUD;
	private Image damageImage;
	private Slider energyBar;
	public Canvas canvas;

	public void takeDamage () {
		if(timeSinceHit > 2) {
			health -= 1;
			timeSinceHit = 0;
			Physics.IgnoreLayerCollision (enemyLayer, playerLayer, true);
			recentlyHit = true;
			healthHUD.GetComponent<HeartScript> ().renderHealth (this.gameObject);
			damageImage.color = new Color (damageImage.color.r, damageImage.color.g, damageImage.color.b, 0.3f);
		}
	}

	void fadeOutDamageImage() {
		if (damageImage.color.a > 0) {
			damageImage.color = new Color (damageImage.color.r, damageImage.color.g, damageImage.color.b, damageImage.color.a - 0.03f);
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
		damageImage = healthHUD.GetComponent<CanvasScript>().damageImage;
		maxEnergy = energy;
		energyBar = GameObject.FindGameObjectWithTag ("Energy Bar").GetComponent<Slider> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (canvas.transform.parent != null){
			canvas.transform.SetParent(null);
		}

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

		fadeOutDamageImage();

		timeSinceHit += Time.deltaTime;
		if (energy < maxEnergy) {
			energy += Time.deltaTime * 40;
		} else {
			energy = maxEnergy;
		}

		energyBar.value = energy / maxEnergy;
	}
}
