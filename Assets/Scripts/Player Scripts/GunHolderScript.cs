using UnityEngine;
using System.Collections.Generic;

public class GunHolderScript : MonoBehaviour {

	public GameObject startingGun;
	public GameObject currentGun;
	public GameObject pickupTooltip;

	private bool toolTipSpawned;
	private GameObject toolTip;
	private GameObject anchor;
	public int currentGunIndex;
	private IList<GameObject> guns;
	public IList<float[]> gunStats;

	//HA! Get fucked future Martin!

	public void pickupGun(GameObject newGun){
		guns.Add (newGun);
		gunStats.Add (new float[] {newGun.GetComponentInChildren<GunScript>().ammo, newGun.GetComponentInChildren<GunScript>().currentClip});
		currentGunIndex = guns.Count - 1;
		switchGuns (newGun);
	}

	void resetClip(GameObject current){
		GetComponent<AimGunScript> ().updateStats (current);
	}

	void switchGuns (GameObject gun) {
		if (currentGun != null) {
			Destroy (currentGun.gameObject);
		}
		currentGun = Instantiate (gun, new Vector3 (transform.position.x, transform.position.y, transform.position.z + 1.4f), transform.rotation) as GameObject;
		currentGun.transform.SetParent (transform);
		currentGun.transform.localPosition = new Vector3 (0, 0, 1.4f);
		currentGun.GetComponentInChildren<GunScript> ().ammo = gunStats [currentGunIndex] [0];
		//todo
		// create/update gun gui next to ammo (pic of gun)
	}

	void giveToolTip(){
		if (!toolTipSpawned) {
			toolTip = Instantiate (pickupTooltip, anchor.transform) as GameObject;
			toolTip.transform.localPosition = new Vector3 (55, 120, 0);
			toolTip.transform.localScale = new Vector3 (1, 1, 1);
			toolTip.transform.localRotation = new Quaternion (0, 0, 0, transform.rotation.w);
			toolTipSpawned = true;
		}
	}

	void killToolTip(){
		if (toolTipSpawned) {
			Destroy (toolTip.gameObject);
			toolTipSpawned = false;
		}
	}

	GameObject findClosestPickup(GameObject[] pickups){
		GameObject closest = pickups [0];
		foreach (GameObject pickup in pickups) {
			if (currentGun.transform.name != pickup.transform.name) {
				if (Vector3.Distance (transform.position, pickup.transform.position) < Vector3.Distance (transform.position, closest.transform.position)) {
					closest = pickup;
				}
			}
		}
		return closest;
	}

	void cycleGunsUp(){
		if (currentGunIndex + 1 < guns.Count) {
			currentGunIndex += 1;
			switchGuns (guns[currentGunIndex]);
		} else {
			currentGunIndex = 0;
			switchGuns (guns[currentGunIndex]);
		}
		resetClip (guns[currentGunIndex]);
	}

	void cycleGunsDown(){
		if (currentGunIndex - 1 > -1) {
			currentGunIndex -= 1;
			switchGuns (guns[currentGunIndex]);
		} else {
			currentGunIndex = guns.Count - 1;
			switchGuns (guns[currentGunIndex]);
		}
		resetClip (guns[currentGunIndex]);
	}

	// Use this for initialization
	void Start () {
		guns = new List<GameObject> ();
		gunStats = new List<float[]> ();
		pickupGun (startingGun);
		anchor = GameObject.Find ("ReloadAnchor");
		toolTipSpawned = false;
		currentGunIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		int layermask = 1 << 12;
		GameObject[] pickups = GameObject.FindGameObjectsWithTag ("Pickup");
		if (Physics.CheckSphere (transform.position, 1.4f, layermask)) {
			giveToolTip ();
			if (Input.GetButtonDown ("Action")) {
				GameObject closestPickup = findClosestPickup (pickups);
				if (Vector3.Distance(transform.position, closestPickup.transform.position) < 1.4f) {
					pickupGun (closestPickup);
					closestPickup.transform.position = new Vector3 (closestPickup.transform.position.x, -1, closestPickup.transform.position.y);
					resetClip (guns[currentGunIndex]);
				}
			}
		} else {
			killToolTip();
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			cycleGunsUp ();
		} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			cycleGunsDown ();
		}
	}
}
