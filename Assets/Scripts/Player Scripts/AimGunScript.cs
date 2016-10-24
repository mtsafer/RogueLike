using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//THIS WHOLE CLASS IS A HUGE SHITSHOW!
public class AimGunScript : MonoBehaviour {

	//stuff for aiming
	private float accuracy;
	private GameObject[] enemies;
	private GameObject target;

	//stuff for shooting
	private float fireRatePerMinute;
	private float timeBetweenShots;
	private float timeSinceLastShot;
	private int clipSize;
	private float currentClip;
	private float ammo;
	private bool reloading;
	private float reloadTime;
	private GameObject reloadAnchor;
	private Slider builtSlider;
	private GameObject canvas;
	private GameObject controller;

	void targetClosestEnemy(ArrayList enemies){
		float currentDistance;
		float minDistance = 9999;
		foreach (GameObject enemy in enemies) {
			currentDistance = Vector3.Distance (enemy.transform.position, transform.position);
			if (currentDistance < minDistance){
				target = enemy;
				minDistance = currentDistance;
			}
		}
		transform.LookAt (target.transform.position);
	}

	void trackEnemies() {
		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		ArrayList trackableEnemies = new ArrayList();
		foreach (GameObject enemy in enemies) {
			Vector3 targetDir = enemy.transform.position - transform.position;
			float angle = Vector3.Angle (targetDir, transform.forward);
			if (angle < accuracy){
				trackableEnemies.Add (enemy);
			}
		}
		if (trackableEnemies.Count > 0){
			targetClosestEnemy(trackableEnemies);
		}
	}

	void reload(){
		if(GetComponentInChildren<GunScript> ().ammo < clipSize) {
			GetComponentInChildren<GunScript>(). currentClip = (int)GetComponentInChildren<GunScript> ().ammo;
			GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1] = (int)GetComponentInChildren<GunScript> ().ammo;
		} else {
			GetComponentInChildren<GunScript>(). currentClip = clipSize;
			GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1] = clipSize;
		}

		currentClip = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1];
		ammo = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0];

		canvas.GetComponent<ClipScript> ().renderAmmo (clipSize, (int)currentClip, GetComponentInChildren<GunScript> ().maxAmmo, ammo);
	}

	public void updateStats(GameObject currentGun){
		//aiming stuff
		accuracy = GetComponentInParent<PlayerMoveScript> ().accuracy;

		//shooting stuff
		fireRatePerMinute = currentGun.GetComponentInChildren<GunScript> ().fireRatePerMinute;
		timeBetweenShots = 60 / fireRatePerMinute;
		timeSinceLastShot = timeBetweenShots;
		clipSize = currentGun.GetComponentInChildren<GunScript> ().clipSize;
		reloading = false;
		currentClip = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1];
		ammo = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0];
		canvas.GetComponent<ClipScript> ().renderAmmo (clipSize, (int)currentClip, currentGun.GetComponentInChildren<GunScript> ().maxAmmo, ammo);
	}

	// Use this for initialization
	void Start () {
		//aiming stuff
		accuracy = GetComponentInParent<PlayerMoveScript> ().accuracy;

		//shooting stuff
		fireRatePerMinute = GetComponentInChildren<GunScript> ().fireRatePerMinute;
		timeBetweenShots = 60 / fireRatePerMinute;
		timeSinceLastShot = timeBetweenShots;
		clipSize = GetComponentInChildren<GunScript> ().clipSize;

		reloading = false;
		reloadAnchor = GameObject.FindGameObjectWithTag ("Player Canvas");
		canvas = GameObject.Find ("HealthHUDCanvas");
		controller = GameObject.FindGameObjectWithTag ("GameController");
		GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1] = GetComponentInChildren<GunScript>(). currentClip;
		currentClip = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1];
		GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0] = GetComponentInChildren<GunScript> ().ammo;
		ammo = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0];
		canvas.GetComponent<ClipScript> ().renderAmmo (clipSize, (int)currentClip, GetComponentInChildren<GunScript> ().maxAmmo, ammo);
	}
	
	// Update is called once per frame
	void Update () {
		currentClip = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1];
		ammo = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0];
		if (!controller.GetComponent<PauseScript>().paused) {
			// Generate a plane that intersects the transform's position with an upwards normal.
			Plane playerPlane = new Plane(Vector3.up, transform.position);

			// Generate a ray from the cursor position
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			// Determine the point where the cursor ray intersects the plane.
			// This will be the point that the object must look towards to be looking at the mouse.
			// Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
			//   then find the point along that ray that meets that distance.  This will be the point
			//   to look at.
			float hitdist = 0.0f;
			// If the ray is parallel to the plane, Raycast will return false.
			if (playerPlane.Raycast (ray, out hitdist)) 
			{
				// Get the point along the ray that hits the calculated distance.
				Vector3 targetPoint = ray.GetPoint(hitdist);

				// Determine the target rotation.  This is the rotation if the transform looks at the target point.
				Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

				// Smoothly rotate towards the target point.
				transform.rotation = targetRotation;
			}
				
			trackEnemies ();

			if (Input.GetButton("Fire") && timeSinceLastShot >= timeBetweenShots && !GetComponent<PlayerMoveScript>().dodging && currentClip > 0 && !reloading){
				GetComponent<GunHolderScript>().currentGun.GetComponentInChildren<GunScript> ().shoot ();
				timeSinceLastShot = 0;
				GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1] -= 1;
				GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0] -= 1;
				currentClip = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [1];
				ammo = GetComponent<GunHolderScript> ().gunStats [GetComponent<GunHolderScript> ().currentGunIndex] [0];
				GetComponentInChildren<GunScript>(). currentClip -= 1;
				GetComponentInChildren<GunScript> ().ammo -= 1;
				canvas.GetComponent<ClipScript> ().renderAmmo (clipSize, (int)currentClip, GetComponentInChildren<GunScript> ().maxAmmo, ammo);
			}

			if (Input.GetButtonDown ("Reload") && !reloading && currentClip < clipSize && currentClip != ammo) {
				reloadTime = 0;
				reloading = true;
			}

			if (reloading && reloadTime > GetComponentInChildren<GunScript> ().reloadTime) {
				reload ();
				reloading = false;
			}

			if (reloading) {
				//Color color = GameObject.Find ("ReloadBackground").GetComponent<Slider>().colors;
				//GameObject.Find ("ReloadBackground").GetComponent<Slider>().colors[0] = new Color (color.r, color.g, color.b, 1);
				//color = GameObject.Find ("ReloadFill").GetComponent<Material> ().color;
				//GameObject.Find ("Reloadfill").GetComponent<Material> ().color = new Color (color.r, color.g, color.b, 1);
				if (builtSlider == null) {
					Slider reloadSlider = GetComponent<PlayerScript> ().reloadSlider;
					builtSlider = Instantiate (reloadSlider, reloadSlider.transform.position, reloadSlider.transform.rotation) as Slider;
					builtSlider.transform.SetParent (reloadAnchor.transform);
					builtSlider.transform.localPosition = new Vector3 (0, 40, 0);
					builtSlider.transform.localRotation = new Quaternion (25, 0, 0, reloadSlider.transform.rotation.w);
					builtSlider.transform.localScale = new Vector3 (1, 1, 1);
				} else {
					builtSlider.value = reloadTime / GetComponentInChildren<GunScript> ().reloadTime;
				}
			} else {
				if (builtSlider) {
					GameObject slider = GameObject.FindGameObjectWithTag ("Reload Slider");
					Destroy (slider.gameObject);
					builtSlider = null;
				}
			}

			timeSinceLastShot += Time.deltaTime;

			reloadTime += Time.deltaTime;
		}
	}
}
