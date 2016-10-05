using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClipScript : MonoBehaviour {
	public GameObject bottomClipHolder;
	public GameObject topClipHolder;
	public GameObject liveRound;
	public GameObject emptyRound;
	private GameObject anchor;
	private int yPosition;
	private int yIncrement;
	private Text bulletCount;
	private string infinity;
	private string currentAmmoString;
	private string maxAmmoString;

	public void renderAmmo(int clipSize, int currentClip, float maxAmmo, float currentAmmo){
		//destroy all present clips images and then reload with new images
		GameObject[] clips = GameObject.FindGameObjectsWithTag ("Clip");
		for (int i = 0; i < clips.Length; i++) {
			Destroy (clips [i].gameObject);
		}
		//create bottom clip
		GameObject bottomClip = Instantiate (bottomClipHolder, bottomClipHolder.transform.position, bottomClipHolder.transform.rotation) as GameObject;
		bottomClip.transform.SetParent (anchor.transform);
		bottomClip.transform.localPosition = new Vector3 (-17, yPosition, 0);
		bottomClip.transform.localScale = new Vector3 (.25f, .25f, .25f);
		bottomClip.transform.localRotation = new Quaternion (0, 0, 0, bottomClip.transform.rotation.w);
		yPosition += yIncrement;

		//create live rounds
		for (int i = 0; i < currentClip; i++) {
			GameObject round = Instantiate (liveRound, liveRound.transform.position, liveRound.transform.rotation) as GameObject;
			round.transform.SetParent (anchor.transform);
			round.transform.localPosition = new Vector3 (-17, yPosition, 0);
			round.transform.localScale = new Vector3 (.25f, .25f, .25f);
			round.transform.localRotation = new Quaternion (0, 0, 0, round.transform.rotation.w);
			yPosition += yIncrement;
		}

		//create live rounds
		for (int i = 0; i < clipSize - currentClip; i++) {
			GameObject round = Instantiate (emptyRound, emptyRound.transform.position, emptyRound.transform.rotation) as GameObject;
			round.transform.SetParent (anchor.transform);
			round.transform.localPosition = new Vector3 (-17, yPosition, 0);
			round.transform.localScale = new Vector3 (.25f, .25f, .25f);
			round.transform.localRotation = new Quaternion (0, 0, 0, round.transform.rotation.w);
			yPosition += yIncrement;
		}

		//create top clip
		GameObject topClip = Instantiate (topClipHolder, topClipHolder.transform.position, topClipHolder.transform.rotation) as GameObject;
		topClip.transform.SetParent (anchor.transform);
		topClip.transform.localPosition = new Vector3 (-17, yPosition, 0);
		topClip.transform.localScale = new Vector3 (.25f, .25f, .25f);
		topClip.transform.localRotation = new Quaternion (180, 0, 0, topClip.transform.rotation.w);
		yPosition += yIncrement;

		//assign the text bullet counter
		if (currentAmmo == Mathf.Infinity) {
			currentAmmoString = infinity;
		} else {
			currentAmmoString = currentAmmo + "";
		}
		if (maxAmmo == Mathf.Infinity) {
			maxAmmoString = infinity;
		} else {
			maxAmmoString = maxAmmo + "";
		}
		bulletCount.text = currentAmmoString + " / " + maxAmmoString;

		//reset yPosition for next round of rendering
		yPosition = 10;

	}

	// Use this for initialization
	void Start () {
		anchor = GameObject.Find ("AmmoAnchor");
		yPosition = 10;
		yIncrement = 13;
		infinity = "\u221E";
		bulletCount = anchor.GetComponent<AmmoCountScript>().ammoCounter;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
