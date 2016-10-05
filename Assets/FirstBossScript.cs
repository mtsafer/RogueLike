using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FirstBossScript : MonoBehaviour {

	public Slider healthBar;

	private Canvas canvas;
	private GameObject player;
	private NavMeshAgent agent;
	private float maxHealth;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		agent = GetComponent<NavMeshAgent> ();
		canvas = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<Canvas>();
		healthBar = Instantiate (healthBar, canvas.transform) as Slider;
		maxHealth = GetComponent<EnemyAttributesScript> ().health;
		healthBar.transform.localRotation = new Quaternion (0,0,0,healthBar.transform.localRotation.w);
		healthBar.transform.localScale = new Vector3 (1,1,1);
		healthBar.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 40);
		healthBar.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, 30, 0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x, 1, transform.position.z);
		agent.SetDestination (player.transform.position);

		healthBar.value = GetComponent<EnemyAttributesScript> ().health / maxHealth;

		if (gameObject.GetComponent<EnemyAttributesScript> ().health <= 0) {
			Destroy (healthBar.gameObject);
			Destroy (this.gameObject);
		}

	}
}
