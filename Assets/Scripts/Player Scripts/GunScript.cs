using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {

	public float fireRatePerMinute;
	public int damage;
	public float projectileSpeed;
	public float knockback;
	public float ammo;
	public float maxAmmo;
	public int clipSize;
	public int currentClip;
	public float reloadTime;
	public GameObject bullet;
	public GameObject bulletSpawn;

	public virtual void shoot() {
		Instantiate (bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
	}

	void Awake(){
		if (gameObject.name == "Starting Gun") {
			ammo = Mathf.Infinity;
		}
		maxAmmo = ammo;
		currentClip = clipSize;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
