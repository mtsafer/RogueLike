using UnityEngine;
using System.Collections;

public class BossRoomTextureAdjust : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//GetComponent<Renderer> ().material.SetTextureScale ("stone_floor_d_normal", new Vector2(12,12));
		GetComponent<Renderer>().material.mainTextureScale = new Vector2(12,12);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
