using UnityEngine;
using System.Collections;

public class EnemyAttributesScript : MonoBehaviour {

	public float health;
	public float speed;
	public float agroRange;
	public float fireRatePerMinute;
	public bool stunned;
	public bool canBeKnockedBack;
	public bool stunnable;
	public float timeSinceStun;
	public GameObject[] coins;


	// Use this for initialization
	void Start () {
		stunned = false;
		timeSinceStun = 1;
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceStun += Time.deltaTime;

	}
}
