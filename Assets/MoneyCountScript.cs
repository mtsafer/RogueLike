using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyCountScript : MonoBehaviour {

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		} else {
			GetComponent<Text> ().text = "" + player.GetComponent<PlayerScript> ().gold;
		}
	}
}
