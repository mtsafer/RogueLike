using UnityEngine;
using System.Collections;

public class ShotgunBulletScript : BulletScript{



	// Use this for initialization
	protected override void Start () {
		base.Start();
		speed = Random.Range (35,45);
	}

	protected override void Update() {
		base.Update ();
	}

	protected override void OnTriggerEnter(Collider other){
		base.OnTriggerEnter (other);
	}

}
