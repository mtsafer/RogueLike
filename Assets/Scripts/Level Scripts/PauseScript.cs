using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public bool paused;

	// Use this for initialization
	void Start () {
		paused = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown ("Pause") && !GetComponent<OpenMinimapScript>().minimapOpened) {
			if (Time.timeScale == 1) {
				Time.timeScale = 0;
				paused = true;
			} else {
				Time.timeScale = 1;
				paused = false;
			}
		}
	}
}
