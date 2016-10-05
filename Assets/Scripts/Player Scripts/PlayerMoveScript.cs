using UnityEngine;
using System.Collections;

public class PlayerMoveScript : MonoBehaviour {

	//vars
	public float speed;
	public Rigidbody rb;
	public float accuracy;
	public bool dodging;
	public bool invulnerable;

	private Vector3 movementVector;
	private float diagonalCoef = 1.4f;
	private float timeSinceDodge;

	private GameObject controller;


	bool movingDiagonal() {
		if (Input.GetButton ("Left") && Input.GetButton ("Up") || Input.GetButton ("Left") && Input.GetButton ("Down") || Input.GetButton ("Up") && Input.GetButton ("Right") || Input.GetButton ("Right") && Input.GetButton ("Down")) {
			return true;
		} else {
			return false;
		}
	}

	bool moving(){
		if (Input.GetButton ("Left") || Input.GetButton ("Right") || Input.GetButton ("Down") || Input.GetButton ("Up") || movingDiagonal()) {
			return true;
		} else {
			return false;
		}
	}

	void dodge(){
		if (Input.GetButton ("Left")) {
			movementVector.Set (-speed*1.75f, 0, movementVector.z);
		}

		//move right
		if (Input.GetButton ("Right")) {
			movementVector.Set (speed*1.75f, 0, movementVector.z);
		}

		//move up
		if (Input.GetButton ("Up")) {
			movementVector.Set (movementVector.x, 0, speed*1.75f);
		}

		if (Input.GetButton ("Down")) {
			movementVector.Set (movementVector.x, 0, -speed*1.75f);
		}

		if (movingDiagonal ()) {
			movementVector.Set (movementVector.x / diagonalCoef, 0, movementVector.z / diagonalCoef);
		}
		dodging = true;

	}

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		timeSinceDodge = 1;
		dodging = false;
		controller = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	// Update is called once per frame
	void Update () {
		if(!controller.GetComponent<PauseScript>().paused){
			movementVector.Set (rb.velocity.x, 0, rb.velocity.z);
			if (timeSinceDodge > 0.45f) {
				invulnerable = false;
			} else {
				invulnerable = true;
			}
			if (timeSinceDodge > 0.6f) {
				dodging = false;

				//move left
				if (Input.GetButtonUp ("Left")) {
					movementVector.Set (0, 0, movementVector.z);
				} else if (Input.GetButton ("Left")) {
					movementVector.Set (-speed, 0, movementVector.z);
				}

				//move right
				if (Input.GetButtonUp ("Right")) {
					movementVector.Set (0, 0, movementVector.z);
				} else if (Input.GetButton ("Right")) {
					movementVector.Set (speed, 0, movementVector.z);
				}

				//move up
				if (Input.GetButtonUp ("Up")) {
					movementVector.Set (movementVector.x, 0, 0);
				} else if (Input.GetButton ("Up")) {
					movementVector.Set (movementVector.x, 0, speed);
				}

				//move down
				if (Input.GetButtonUp ("Down")) {
					movementVector.Set (movementVector.x, 0, 0);
				} else if (Input.GetButton ("Down")) {
					movementVector.Set (movementVector.x, 0, -speed);
				}

				//no movement pressed
				if (!Input.GetButton ("Left") && !Input.GetButton ("Up") && !Input.GetButton ("Right") && !Input.GetButton ("Down")) {
					movementVector.Set (0, 0, 0); //stop movement
				} else if (movingDiagonal ()) {
					movementVector.Set (movementVector.x / diagonalCoef, 0, movementVector.z / diagonalCoef);
				}

				if (moving () && Input.GetButtonDown ("Dodge") && timeSinceDodge > 0.8f) {
					dodge ();
					timeSinceDodge = 0;
				}
			}
			timeSinceDodge += Time.deltaTime;
			rb.velocity = movementVector;
		}
	}
}
