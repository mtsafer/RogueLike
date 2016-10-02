using UnityEngine;
using System.Collections;

public class HallTriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            GetComponentInParent<HallCoverScript>().entered = true;
        }
    }
}
