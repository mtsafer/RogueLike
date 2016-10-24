using UnityEngine;
using System.Collections;


// This script is attached to the main level controller, which is responsible for pulling in all of the necessary prefabs to create a working level.
// This script specifically proceduraly generates a playable floor, consisting of connected rooms, each with random waves of enemies (in the RoomScript).
// Below, the public GameObject[] variables are the different sets of rooms. Each room "type" has 15 prebuilt rooms, with the 15 different possible door direction combinations.
// They are assigned in the Unity Editor.
// minNumberOfRooms and maxNumberOfRooms are responsible for determining the size of a valid floor, and are assigned in the Unity Editor.

// Disclaimer -- This is my first attempt at a procedural level generator.

public class FloorGeneratorScript : MonoBehaviour {

	public GameObject[] startingRooms;
	public GameObject[] basicRooms;
	public GameObject[] centerRooms;
	public GameObject[] xRooms;
	public GameObject[] diamondRooms;
	public GameObject[] diamondCenterRooms;
	public GameObject[] bossRooms;
	public GameObject[] treasureRooms;
	public GameObject player;
	public int treasureRoomCount;
	public int minNumberOfRooms;
	public int maxNumberOfRooms;

	private GameObject[][] allRooms;
	private GameObject startingRoom;
	private GameObject[] roomSpawns;
	private Vector3 playerPosition;
	private int roomCount;
	private int currentTreasureRoomCount;
	private float roomSize;
	private bool buildIsComplete;
	private bool newSpawns;
	private bool hasBossRoom;
	private bool playerSpawned;


	// Selects a random element from a GameObject[].
	GameObject selectRandomFrom(GameObject[] list){
		return list [Random.Range (0, list.Length)];
	}

	// Selects a random element from a GameObject[][].
	GameObject[] selectRoomSet(GameObject[][] list){
		return list [Random.Range (0, list.Length)];
	}

	// Returns true if word is found anywhere in array, false otherwise.
	// Essentialy a custom .contains() method.
	bool matchIn (string[] array, string word){
		foreach(string element in array){
			if (element == word) {
				return true;
			}
		}
		return false;
	}

	// Returns false if the room (which hasn't yet been created), does not line up with its neighbors, or returns true if it does line up.
	// This method is called from within isInLine(), which casts a ray North, South, East, and West.
	// If the ray hits a target, this method is called to determine if the room in question would line up with the neighbor room.
	// -- room is the room to be created -- neighborDoors is doorList from the neighbor room in question -- hit is the object the raycast hit.
	// -- neighborDoorMatch is the door direction to check for the neighbor -- roomDoorMatch is the door direction to check for the room.
	bool isInLineHelper(GameObject room, string[] neighborDoors, RaycastHit hit, string neighborDoorMatch, string roomDoorMatch){
		
		// If the neighbor is a boss room, and the room in question has a door facing it, the room is not in line,
		// because all boss rooms are generated with no external doors.

		if (matchIn (neighborDoors, neighborDoorMatch) && hit.transform.parent.name.Substring(0,9) == "Boss Room") {
			// DoorList is a string[] which contains the directions of doors for a room. For instance, a room with a north facing door will have "North" in doorList.
			if (matchIn (room.GetComponent<RoomScript> ().doorList, roomDoorMatch)) {
				return false;
			}
			// If neighbor has a south facing door (and it's a northerly neighbor), and the room doesn't have a north facing door, the room is not in line. 
		} else if (matchIn (neighborDoors, neighborDoorMatch)) {
			if (!matchIn (room.GetComponent<RoomScript> ().doorList, roomDoorMatch)) {
				return false;
			}
			// If neighbor doesn't have a south facing door (and it's a northerly neighbor), and the room has a north facing door, the room is not in line.
		} else if (!matchIn (neighborDoors, neighborDoorMatch)) {
			if (matchIn (room.GetComponent<RoomScript> ().doorList, roomDoorMatch)) {
				return false;
			}
		}
		return true;
	}

	// Uses isInLineHelper() (DRY's code) to check each direction, and return the results of all directions. 
	// Returns true if the room is in line, and false if it is not.
	bool isInLine(GameObject room, GameObject spawn){
		
		string[] neighborDoors;
		RaycastHit hit;

		// Physics.Raycast() shoots a ray from the given position, in the given direction, for a set distance, and returns true if it hits something, and false otherwise.
		// hit is assigned to the collider that gets hit.

		// North
		if(Physics.Raycast(spawn.transform.position, transform.forward, out hit, roomSize)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (!isInLineHelper (room, neighborDoors, hit, "South", "North")) {
				return false;
			}
		}
		// East
		if(Physics.Raycast(spawn.transform.position, transform.right, out hit, roomSize)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (!isInLineHelper (room, neighborDoors, hit, "West", "East")){
				return false;
			}
		}
		// South
		if(Physics.Raycast(spawn.transform.position, transform.forward*-1, out hit, roomSize)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (!isInLineHelper (room, neighborDoors, hit, "North", "South")) {
				return false;
			}
		}
		// West
		if(Physics.Raycast(spawn.transform.position, transform.right*-1, out hit, roomSize)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (!isInLineHelper (room, neighborDoors, hit, "East", "West")) {
				return false;
			}
		}
			
		return true;

	}

	// Returns true if there are no outward facing doors, false otherwise.
	bool noNewSpawn(GameObject room, GameObject spawn){
		// North
		// If the raycast does NOT hit anything, but the room has a north facing door, return false.
		if(!Physics.Raycast(spawn.transform.position, transform.forward, roomSize)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "North")) {
				return false;
			}
		}
		// East
		if(!Physics.Raycast(spawn.transform.position, transform.right, roomSize)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "East")) {
				return false;
			}
		}
		// South
		if(!Physics.Raycast(spawn.transform.position, transform.forward*-1, roomSize)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "South")) {
				return false;
			}
		}
		// West
		if(!Physics.Raycast(spawn.transform.position, transform.right*-1, roomSize)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "West")) {
				return false;
			}
		}
		return true;
	}

	// Returns true if there is space to place a boss room (which has special dimensions).
	// Returns false if the boss room won't fit.
	// Uses special boss room dimensions, and checks further than needed to be 100% certain the area is clear.
	bool bossRoomFits(GameObject spawn){
		// forward
		if (Physics.Raycast (spawn.transform.position, spawn.transform.forward, 140)) {
			return false;
		}
		// right
		if(Physics.Raycast(spawn.transform.position, spawn.transform.right, 60)){
			return false;
		}
		// left
		if(Physics.Raycast(spawn.transform.position, spawn.transform.right*-1, 60)){
			return false;
		}
		// forward + right
		if(Physics.Raycast(spawn.transform.position, spawn.transform.forward + spawn.transform.right, 180)){
			return false;
		}
		// forward + left
		if(Physics.Raycast(spawn.transform.position, spawn.transform.forward + spawn.transform.right*-1, 180)){
			return false;
		}
		return true;
	}

	// Resets all the necessary variable and deletes all rooms, to restart a floor build.
	// This method is called if the finished floor doesn't meet the guidelines (like if it accidentally makes a floor too small).
	void restartBuild(){
		GameObject oldPlayer = GameObject.FindGameObjectWithTag ("Player");
		GameObject[] pickups = GameObject.FindGameObjectsWithTag ("Pickup");
		GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

		if (oldPlayer != null) {
			Destroy (oldPlayer.gameObject);
		}

		for (int i = 0; i < rooms.Length; i++) {
			Destroy (rooms[i]);
		}
		for (int i = 0; i < pickups.Length; i++) {
			Destroy (pickups[i]);
		}
		buildIsComplete = false;
		hasBossRoom = false;
		playerSpawned = false;
		startingRoom = selectRandomFrom (startingRooms);
		Instantiate (startingRoom, transform.position, transform.rotation);
		roomCount = 1;
		currentTreasureRoomCount = 0;
	}

	// This method is the "generator". It spawns in the approriate rooms as needed.
	// Generates random regular rooms until the floor is larger than the mininum requirement,
	// and then attempts to end the build by generating a boss floor, and only generating new rooms that don't have external doors.
	void generateFloor(GameObject[] roomSpawns){
		foreach (GameObject spawn in roomSpawns) {
			if (spawn.GetComponent<RoomSpawnerScript>().newSpawn){
				newSpawns = true;
			}
			if (roomCount < minNumberOfRooms) {
				if (!Physics.CheckSphere (spawn.transform.position, 1)) {
					GameObject newRoom = selectRandomFrom (selectRoomSet (allRooms));
					while (!isInLine (newRoom, spawn)){
						newRoom = selectRandomFrom (selectRoomSet (allRooms));
					}
					Instantiate (newRoom, spawn.transform.position, transform.rotation);
					roomCount++;
				}
			} else {
				if (!Physics.CheckSphere (spawn.transform.position, 5)) {
					if (currentTreasureRoomCount < treasureRoomCount) {
						GameObject newRoom = selectRandomFrom (treasureRooms);
						if (isInLine (newRoom, spawn)) {
							Instantiate (newRoom, spawn.transform.position, newRoom.transform.rotation);
							roomCount++;
							currentTreasureRoomCount++;
						}
					} else if (bossRoomFits (spawn) && !hasBossRoom) {
						GameObject newRoom = selectRandomFrom (bossRooms);
						if (isInLine (newRoom, spawn)) {
							Instantiate (newRoom, spawn.transform.position + 60 * spawn.transform.forward, newRoom.transform.rotation);
							roomCount++;
							hasBossRoom = true;
						}
					} else {
						GameObject newRoom = selectRandomFrom (selectRoomSet (allRooms));
						while (!noNewSpawn (newRoom, spawn) || !isInLine (newRoom, spawn)) {
							newRoom = selectRandomFrom (selectRoomSet (allRooms));
						}
						// The isInLine() call here is redunant, but I have left it in for clarity.
						if (isInLine (newRoom, spawn)) {
							Instantiate (newRoom, spawn.transform.position, transform.rotation);
							roomCount++;
						}
					}
				}
			}
			spawn.GetComponent<RoomSpawnerScript> ().newSpawn = false;
		}
	}

	// Use this for initialization.
	void Start () {
		allRooms = new GameObject[][] { basicRooms, centerRooms, xRooms, diamondRooms, diamondCenterRooms };
		buildIsComplete = false;
		startingRoom = selectRandomFrom (startingRooms);
		Instantiate (startingRoom, transform.position, transform.rotation);
		roomCount = 1;
		roomSize = 40;
		playerPosition = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
		hasBossRoom = false;
		playerSpawned = false;
		currentTreasureRoomCount = 0;
		Time.timeScale = 0;
	}
	
	// Update is called once per frame.
	void Update () {
		
		newSpawns = false;
		roomSpawns = GameObject.FindGameObjectsWithTag ("Room Spawner");

		generateFloor (roomSpawns);

		// Once there are no longer any new spawns (all spawns have been dealt with), the build is considered complete.
		// This bit of code isn't really necessary, since I could just use !newSpawns instead of buildIsComplete.
		// However, this check may become more complex as I add to the generator, so I have left it in to allow for better readability/maintainability.
		if (!newSpawns){
			buildIsComplete = true;
		}

		// If the build is complete, check to make sure that it complies with the rules, if not, try again.
		if (buildIsComplete) {
			if (roomCount < minNumberOfRooms || roomCount > maxNumberOfRooms || !hasBossRoom || currentTreasureRoomCount != treasureRoomCount) {
				restartBuild ();
			} else if (!playerSpawned) {
				Instantiate (player, playerPosition, transform.rotation);
				playerSpawned = true;
				Time.timeScale = 1;
			}
		}
	}
}
