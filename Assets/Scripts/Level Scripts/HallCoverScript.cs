using UnityEngine;
using System.Collections;

public class HallCoverScript : MonoBehaviour {

    public bool entered;

	// Use this for initialization
	void Start () {
        entered = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (entered) {
            Destroy(this.gameObject);
        }
	}
}
