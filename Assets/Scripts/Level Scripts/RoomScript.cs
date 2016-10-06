using UnityEngine;
using System.Collections;

public class RoomScript : MonoBehaviour {

	public int maxWaves;
	public int minWaves;
	public int maxEnemiesInWave;
	public int minEnemiesInWave;
	public float xRadiusSpawn;
	public float zRadiusSpawn;
	public bool playerEntered;

	public GameObject[] enemies;
	public GameObject[] doors;

	public string[] doorList;

	public GameObject[] drops;

	private GameObject player;

	private Camera minimap;

	private int[] enemiesInWaves;
	private int waves;
	private int enemiesLeft;
	private int xSpawn;
	private int zSpawn;
	private bool roomCleared;
	private float distanceFromPlayer;
	private GameObject controller;

	//set random coodinates for spawn location
	void generateSpawnLocation(){
		xSpawn = Random.Range ((int)-xRadiusSpawn, (int)xRadiusSpawn + 1);
		zSpawn = Random.Range ((int)-zRadiusSpawn, (int)zRadiusSpawn + 1);
		//calculate the distance from the player when spawning
		//Use this number to redo spawn if it's too close
		distanceFromPlayer = Vector3.Distance (new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), player.transform.position);
	}

	void openDoor(GameObject door){
		Color color = door.GetComponent<MeshRenderer> ().material.color;
		if (door.transform.position.y < 10) {
			door.transform.position = new Vector3 (door.transform.position.x, door.transform.position.y + 0.3f, door.transform.position.z);
		}
		if (color.a > 0) {
			color.a -= Time.deltaTime/0.5f;
			door.GetComponent<MeshRenderer> ().material.color = color;
		}
	}

	void closeDoor(GameObject door) {
		Color color = door.GetComponent<MeshRenderer> ().material.color;
		if (door.transform.position.y > 1) {	
			door.transform.position = new Vector3 (door.transform.position.x, door.transform.position.y - 1.5f, door.transform.position.z);
		}
		if (color.a < 1) {
			color.a += Time.deltaTime/0.25f;
			door.GetComponent<MeshRenderer> ().material.color = color;
		}
	}

	void chanceToSpawnDrop(){
		int roll = Random.Range (0,10);
		if (roll < player.GetComponent<PlayerScript>().luck) {
			print ("Spawn a drop!");
			generateSpawnLocation ();
			if (drops.Length > 0) {
				GameObject drop = drops [Random.Range (0, drops.Length)];
				//check the spawn spot to make sure it's clear of objects
				Vector3 checkBoxDimensions = new Vector3 (drop.transform.lossyScale.x / 2f - 0.02f, drop.transform.lossyScale.y / 2f - 0.02f, drop.transform.lossyScale.z / 2f - 0.02f);
				while (Physics.CheckBox (new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), checkBoxDimensions) || distanceFromPlayer < 1.5f || distanceFromPlayer > 10) {
					generateSpawnLocation ();
				}
				Instantiate (drop, new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), drop.transform.rotation);
			}
		}
	}

	// Use this for initialization
	void Start () {
		minimap = GameObject.FindGameObjectWithTag("Minimap Camera").GetComponent<Camera> ();
		playerEntered = false;
		roomCleared = false;

		xRadiusSpawn = xRadiusSpawn * transform.lossyScale.x;
		zRadiusSpawn = zRadiusSpawn * transform.lossyScale.z;

		//find the player
		player = GameObject.FindGameObjectWithTag ("Player");

		//set number of waves
		waves = Random.Range(minWaves, maxWaves + 1);

		//set number of units in each wave
		enemiesInWaves = new int[waves];
		for (int i = 0; i < waves; i++){
			enemiesInWaves[i] = Random.Range (minEnemiesInWave, maxEnemiesInWave+1);
		}
		controller = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	// Update is called once per frame
	void Update () {

		if (playerEntered) {
			//calculate the enemies remaining
			enemiesLeft = GameObject.FindGameObjectsWithTag ("Enemy").Length;

			//if there are waves remaining, close the doors.
			if (!roomCleared){
				foreach (GameObject door in doors){
					closeDoor (door);
				}
				minimap.enabled = false;
				controller.GetComponent<OpenMinimapScript> ().canOpenMap = false;
			}

			//all enemies defeated and all waves defeates, open the doors
			if (waves == 0 && enemiesLeft == 0){
				if (!roomCleared) {
					minimap.enabled = true;
					controller.GetComponent<OpenMinimapScript> ().canOpenMap = true;
					if (gameObject.name.Substring(0,13) != "Starting Room") {	
						chanceToSpawnDrop ();
					}
				}
				roomCleared = true;
				foreach (GameObject door in doors) {
					openDoor (door);
				}
			}

			//if there are no enemies remaining, but there are still waves left, spawn the next wave.
			if (enemiesLeft == 0 && waves > 0) {
				for (int i = 0; i < enemiesInWaves [waves - 1]; i++) {
					generateSpawnLocation ();
					GameObject enemy = enemies [Random.Range (0, enemies.Length)];
					//check the spawn spot to make sure it's clear of objects
					Vector3 checkBoxDimensions = new Vector3 (enemy.transform.lossyScale.x / 2f - 0.02f, enemy.transform.lossyScale.y / 2f - 0.02f, enemy.transform.lossyScale.z / 2f - 0.02f);
					while (Physics.CheckBox (new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), checkBoxDimensions) || distanceFromPlayer < 5) {
						generateSpawnLocation ();
					}
					Instantiate (enemy, new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), transform.rotation);
				}
				waves -= 1;
			}
		}
	}
}
