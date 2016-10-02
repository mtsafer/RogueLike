using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {

	public float fireRatePerMinute;
	public int damage;
	public float projectileSpeed;
	public float knockback;
	public float ammo;
	public int clipSize;
	public float reloadTime;
	public GameObject bullet;

	// Use this for initialization
	void Start () {
		if (gameObject.name == "Starting Gun") {
			ammo = Mathf.Infinity;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
