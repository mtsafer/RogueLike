using UnityEngine;
using System.Collections;

public class ShotgunScript : GunScript {

	public int spreadSize;

	public override void shoot(){
		for (int i = 0; i < 5; i++) {
			GameObject newBullet = Instantiate (bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation) as GameObject;
			int spread = Random.Range (-spreadSize, spreadSize + 1);
			Quaternion originalRotation = newBullet.transform.rotation;
			newBullet.transform.rotation = originalRotation * Quaternion.AngleAxis (spread, Vector3.up);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
