using UnityEngine;
using System.Collections;

public class HeartScript : MonoBehaviour {

	public GameObject fullHeart;
	public GameObject halfHeart;
	public GameObject emptyHeart;

	private float initialX;
	private GameObject anchor;

	// Use this for initialization
	void Start () {
		initialX = 30;
		anchor = GameObject.Find ("HeartAnchor");
	}

	void update(){
		
	}
	
	public void renderHealth(GameObject player){

		GameObject[] hearts = GameObject.FindGameObjectsWithTag ("Heart");
		for (int i = 0; i < hearts.Length; i++) {
			Destroy (hearts[i]);
			initialX = 30;
		}

		int fullHearts = (int)player.GetComponent<PlayerScript> ().health / 2;
		int halfHearts = (int)player.GetComponent<PlayerScript> ().health % 2;
		int emptyHearts = ((int)player.GetComponent<PlayerScript> ().maxHealth - (int)player.GetComponent<PlayerScript> ().health) / 2;
		for(int i = 0; i < fullHearts; i++){
			//render full heart
			GameObject heart = Instantiate(fullHeart);
			heart.transform.SetParent(anchor.transform);
			heart.GetComponent<RectTransform>().localPosition = new Vector3 (initialX, -30, 0);
			heart.GetComponent<RectTransform>().localRotation = new Quaternion (0,0,0,0);
			heart.GetComponent<RectTransform>().localScale = new Vector3 (1,1,1);
			initialX += 40;
		}
		for(int i = 0; i < halfHearts; i++){
			GameObject heart = Instantiate(halfHeart);
			heart.transform.SetParent(anchor.transform);
			heart.GetComponent<RectTransform>().localPosition = new Vector3 (initialX, -30, 0);
			heart.GetComponent<RectTransform>().localRotation = new Quaternion (0,0,0,0);
			heart.GetComponent<RectTransform>().localScale = new Vector3 (1,1,1);
			initialX += 40;
		}
		for(int i = 0; i < emptyHearts; i++){
			GameObject heart = Instantiate(emptyHeart);
			heart.transform.SetParent(anchor.transform);
			heart.GetComponent<RectTransform>().localPosition = new Vector3 (initialX, -30, 0);
			heart.GetComponent<RectTransform>().localRotation = new Quaternion (0,0,0,0);
			heart.GetComponent<RectTransform>().localScale = new Vector3 (1,1,1);
			initialX += 40;
		}
	}
}
