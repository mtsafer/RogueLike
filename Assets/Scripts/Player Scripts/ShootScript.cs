using UnityEngine;
using System.Collections;

public class ShootScript : MonoBehaviour {

	public GameObject bullet;
	private float fireRatePerMinute;

	private float timeBetweenShots;
	private float timeSinceLastShot;

	private Vector3 bulletSpawnLocation;

	bool aiming() {
		if (Input.GetKey ("left") || Input.GetKey ("right") || Input.GetKey ("up") || Input.GetKey ("down")) {
			return true;
		} else {
			return false;
		}
	}
		

	// Use this for initialization
	void Start () {
		fireRatePerMinute = GetComponentInParent<GunScript> ().fireRatePerMinute;
		timeBetweenShots = 60 / fireRatePerMinute;
		timeSinceLastShot = timeBetweenShots;
	}

	// Update is called once per frame
	void Update () {
		
		if (aiming() && timeSinceLastShot >= timeBetweenShots){
			bulletSpawnLocation = transform.position;
			Instantiate (bullet, bulletSpawnLocation, transform.rotation);
			timeSinceLastShot = 0;
		}
		timeSinceLastShot += Time.deltaTime;
	}
}
