using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AimGunScript : MonoBehaviour {

	//stuff for aiming
	private float accuracy;
	private GameObject[] enemies;
	private GameObject target;

	//stuff for shooting
	private GameObject bullet;
	private float fireRatePerMinute;
	private float timeBetweenShots;
	private float timeSinceLastShot;
	private Vector3 bulletSpawnLocation;
	private GameObject muzzle;
	private int clipSize;
	private int currentClip;
	private bool reloading;
	private float reloadTime;
	private GameObject reloadAnchor;
	private Slider builtSlider;

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

	void muzzleFlashStart(){
		float x = muzzle.transform.localScale.x;
		float y = muzzle.transform.localScale.y;
		float z = muzzle.transform.localScale.z;
		if (muzzle.transform.localScale.x < 10) {
			x += Time.deltaTime / 0.08f;
			y += Time.deltaTime / 0.08f;
			z += Time.deltaTime / 0.08f;
		} 
		muzzle.transform.localScale = new Vector3 (x, y, z);
	}

	void muzzleFlashEnd(){
		muzzle.transform.localScale = new Vector3 (0, 0, 0);
	}

	void reload(){
		GetComponentInChildren<GunScript> ().ammo -= (clipSize - currentClip);
		currentClip = clipSize;
	}

	// Use this for initialization
	void Start () {
		//aiming stuff
		accuracy = GetComponentInParent<PlayerMoveScript> ().accuracy;

		//shooting stuff
		fireRatePerMinute = GetComponentInChildren<GunScript> ().fireRatePerMinute;
		timeBetweenShots = 60 / fireRatePerMinute;
		timeSinceLastShot = timeBetweenShots;
		bullet = GetComponentInChildren<GunScript> ().bullet;
		muzzle = GameObject.FindGameObjectWithTag ("Player Muzzle");
		clipSize = GetComponentInChildren<GunScript> ().clipSize;
		currentClip = clipSize;
		reloading = false;

		reloadAnchor = GameObject.Find ("ReloadAnchor");
	}
	
	// Update is called once per frame
	void Update () {

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
			print (currentClip);
			bulletSpawnLocation = GameObject.FindGameObjectWithTag("Bullet Spawner").transform.position;
			Quaternion bulletSpawnRotation = GameObject.FindGameObjectWithTag ("Bullet Spawner").transform.rotation;
			Instantiate (bullet, bulletSpawnLocation, bulletSpawnRotation);
			timeSinceLastShot = 0;
			currentClip -= 1;
		}

		if (Input.GetButtonDown ("Reload") && !reloading && currentClip < clipSize) {
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
				builtSlider.transform.localPosition = new Vector3 (0, 30, 0);
				builtSlider.transform.localRotation = new Quaternion (25, 0, 0, reloadSlider.transform.rotation.w);
				builtSlider.transform.localScale = new Vector3 (1, 1, 1);
			} else {
				builtSlider.fillRect.transform.localPosition = new Vector3(reloadTime / GetComponentInChildren<GunScript> ().reloadTime * 55 -  25 ,0, 0);
			}
		} else {
			if (builtSlider) {
				GameObject slider = GameObject.FindGameObjectWithTag ("Reload Slider");
				Destroy (slider.gameObject);
				builtSlider = null;
			}
		}

		timeSinceLastShot += Time.deltaTime;

		if (timeSinceLastShot < 0.05f) {
			muzzleFlashStart ();
		} else {
			muzzleFlashEnd ();
		}

		reloadTime += Time.deltaTime;

	}
}
