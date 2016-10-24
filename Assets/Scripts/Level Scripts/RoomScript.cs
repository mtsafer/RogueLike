using UnityEngine;
using System.Collections;

// This is the main room script, where room specific actions are handled. Instantiating waves of enemies,
// handling rooms drops, and locking / unlocking doors.

// The room will generate a random number of waves between minWaves and maxWaves, where each wave is
// of random size, defined by minEnemiesInWave and maxEnemiesInWave. Additionaly, each enemy in each
// wave is randomly selected from the array of enemies.
// When you enter a room, the doors will become locked until you defeat all enemies in all waves.

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
	public GameObject[] drops;
	public string[] doorList;

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

	//set random coodinates for spawn location within the room.
	void generateSpawnLocation(){
		xSpawn = Random.Range ((int)-xRadiusSpawn, (int)xRadiusSpawn + 1);
		zSpawn = Random.Range ((int)-zRadiusSpawn, (int)zRadiusSpawn + 1);
		distanceFromPlayer = Vector3.Distance (new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), player.transform.position);
	}

	// Move a door to the open position, and change the material color to be transparent over time.
	void openDoor(GameObject door){
		Color color = door.GetComponent<MeshRenderer> ().material.color;
		if (door.transform.position.y < 10) {
			door.transform.position = new Vector3 (door.transform.position.x, door.transform.position.y + 0.3f, door.transform.position.z);
		}
		if (color.a > 0) {
			// Fade the door out over time.
			color.a -= Time.deltaTime/0.5f;
			door.GetComponent<MeshRenderer> ().material.color = color;
		}
	}

	// Move a door to the closed position, and change the material color to be opaque over time.
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

	// Has an x% chance to spawn a random drop from the drop array, where x is the players luck stat * 10.
	// For example, the default luck stat of 2, provides a 20% chance to spawn a drop.
	void chanceToSpawnDrop(){
		int roll = Random.Range (0,10);
		if (roll < player.GetComponent<PlayerScript>().luck) {
			generateSpawnLocation ();
			if (drops.Length > 0) {
				GameObject drop = drops [Random.Range (0, drops.Length)];
				// Check the spawn spot to make sure it's clear of objects (-0.02f so that it won't detect the floor )
				Vector3 checkBoxDimensions = new Vector3 (drop.transform.lossyScale.x, drop.transform.lossyScale.y - 0.02f, drop.transform.lossyScale.z);
				while (Physics.CheckBox (new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), checkBoxDimensions) || distanceFromPlayer < 1.5f || distanceFromPlayer > 10) {
					generateSpawnLocation ();
				}
				Instantiate (drop, new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), drop.transform.rotation);
			}
		}
	}

	// Instantiates the number of enemies that are in the current wave,
	// each time selecting a random enemy from the enemies array,
	// and making sure the enemy spawn location is valid.
	void spawnNextWave(){
		for (int i = 0; i < enemiesInWaves [waves - 1]; i++) {
			generateSpawnLocation ();
			GameObject enemy = enemies [Random.Range (0, enemies.Length)];
			Vector3 checkBoxDimensions = new Vector3 (enemy.transform.lossyScale.x / 2f - 0.02f, enemy.transform.lossyScale.y / 2f - 0.02f, enemy.transform.lossyScale.z / 2f - 0.02f);
			while (Physics.CheckBox (new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), checkBoxDimensions) || distanceFromPlayer < 5) {
				generateSpawnLocation ();
			}
			Instantiate (enemy, new Vector3 (transform.position.x + xSpawn, 1, transform.position.z + zSpawn), transform.rotation);
		}
		waves -= 1;
	}

	// Opens all doors, has a chance to spawn a drop, enables the minimap, and enables the map button.
	void handleRoomClear(){
		if (!roomCleared) {
			minimap.enabled = true;
			controller.GetComponent<OpenMinimapScript> ().canOpenMap = true;	
			chanceToSpawnDrop ();
			GameObject[] coins = GameObject.FindGameObjectsWithTag ("Coin");
			foreach (GameObject coin in coins) {
				coin.GetComponent<CoinScript> ().seek ();
			}
		}
		roomCleared = true;
		foreach (GameObject door in doors) {
			openDoor (door);
		}
	}

	// Closes all doors, disables the minimap, and disables the map button.
	void handleRoomEntrance(){
		foreach (GameObject door in doors){
			closeDoor (door);
		}
		minimap.enabled = false;
		controller.GetComponent<OpenMinimapScript> ().canOpenMap = false;
	}

	// Use this for initialization.
	void Start () {
		minimap = GameObject.FindGameObjectWithTag("Minimap Camera").GetComponent<Camera> ();
		playerEntered = false;
		roomCleared = false;
		// Scale radius spawn with the room.
		xRadiusSpawn = xRadiusSpawn * transform.lossyScale.x;
		zRadiusSpawn = zRadiusSpawn * transform.lossyScale.z;
		player = GameObject.FindGameObjectWithTag ("Player");
		// Set the number of waves and the number of units in each wave.
		waves = Random.Range(minWaves, maxWaves + 1);
		enemiesInWaves = new int[waves];
		for (int i = 0; i < waves; i++){
			enemiesInWaves[i] = Random.Range (minEnemiesInWave, maxEnemiesInWave+1);
		}
		controller = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	// Update is called once per frame.
	// Handle the 3 different room states, entered, cleared, and wave defeat.
	void Update () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		if (playerEntered) {
			enemiesLeft = GameObject.FindGameObjectsWithTag ("Enemy").Length;
	
			if (!roomCleared) {
				handleRoomEntrance ();
			} 

			if (waves == 0 && enemiesLeft == 0) {
				handleRoomClear ();
			}
				
			if (enemiesLeft == 0 && waves > 0) {
				spawnNextWave ();
			}
		}
	}
}
