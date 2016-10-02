using UnityEngine;
//using UnityEditor;
using System.Collections;

public class FloorGeneratorScript : MonoBehaviour {

	public GameObject[] startingRooms;
	public GameObject[] basicRooms;
	public GameObject[] centerRooms;
	public GameObject[] xRooms;
	public GameObject[] diamondRooms;
	public GameObject[] diamondCenterRooms;

	public GameObject player;
	private GameObject[][] allRooms;

	public int minNumberOfRooms;
	public int maxNumberOfRooms;

	private GameObject startingRoom;

	private GameObject[] roomSpawns;

	private Vector3 playerPosition;

	private int roomCount;

	private bool buildIsComplete;
	private bool navMeshBuilt;
	private bool newSpawns;

	GameObject selectRandomFrom(GameObject[] list){
		return list [Random.Range (0, list.Length)];
	}

	GameObject[] selectRoomSet(GameObject[][] list){
		return list [Random.Range (0, list.Length)];
	}

	bool matchIn (string[] array, string word){
		foreach(string element in array){
			if (element == word) {
				return true;
			}
		}
		return false;
	}

	bool isInLine(GameObject room, GameObject spawn){

		string[] neighborDoors;
		RaycastHit hit;

		//North
		if(Physics.Raycast(spawn.transform.position, transform.forward, out hit, 40)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (matchIn (neighborDoors, "South")) {
				if (!matchIn (room.GetComponent<RoomScript> ().doorList, "North")) {
					return false;
				}
			}else if (!matchIn (neighborDoors, "South")) {
				if (matchIn (room.GetComponent<RoomScript> ().doorList, "North")) {
					return false;
				}
			}
		}
		//East
		if(Physics.Raycast(spawn.transform.position, transform.right, out hit, 40)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (matchIn (neighborDoors, "West")) {
				if (!matchIn (room.GetComponent<RoomScript> ().doorList, "East")) {
					return false;
				}
			}else if (!matchIn (neighborDoors, "West")) {
				if (matchIn (room.GetComponent<RoomScript> ().doorList, "East")) {
					return false;
				}
			}
		}
		//South
		if(Physics.Raycast(spawn.transform.position, transform.forward*-1, out hit, 40)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (matchIn (neighborDoors, "North")) {
				if (!matchIn (room.GetComponent<RoomScript> ().doorList, "South")) {
					return false;
				}
			}else if (!matchIn (neighborDoors, "North")) {
				if (matchIn (room.GetComponent<RoomScript> ().doorList, "South")) {
					return false;
				}
			}
		}
		//West
		if(Physics.Raycast(spawn.transform.position, transform.right*-1, out hit, 40)){
			neighborDoors = (hit.transform.gameObject.GetComponentInParent<RoomScript>().doorList);
			if (matchIn (neighborDoors, "East")) {
				if (!matchIn (room.GetComponent<RoomScript> ().doorList, "West")) {
					return false;
				}
			}else if (!matchIn (neighborDoors, "East")) {
				if (matchIn (room.GetComponent<RoomScript> ().doorList, "West")) {
					return false;
				}
			}
		}


		return true;

	}

	//returns true if the room in question only has one door
	bool hasOneDoor(GameObject room){
		return (room.GetComponent<RoomScript> ().doors.Length == 1);
	}

	//returns true if there are no outward facing doors, false otherwise
	bool noNewSpawn(GameObject room, GameObject spawn){

		//North
		if(!Physics.Raycast(spawn.transform.position, transform.forward, 40)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "North")) {
				return false;
			}
		}
		//East
		if(!Physics.Raycast(spawn.transform.position, transform.right, 40)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "East")) {
				return false;
			}
		}
		//South
		if(!Physics.Raycast(spawn.transform.position, transform.forward*-1, 40)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "South")) {
				return false;
			}
		}
		//West
		if(!Physics.Raycast(spawn.transform.position, transform.right*-1, 40)){
			if (matchIn (room.GetComponent<RoomScript> ().doorList, "West")) {
				return false;
			}
		}
		return true;
	}

	void restartBuild(){
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//delete all rooms, reset bools, set roomCount to 0
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
		for (int i = 0; i < rooms.Length; i++) {
			Destroy (rooms[i]);
		}
		buildIsComplete = false;
		navMeshBuilt = false;
		startingRoom = selectRandomFrom (startingRooms);
		Instantiate (startingRoom, transform.position, transform.rotation);
		roomCount = 1;
	}

	// Use this for initialization
	void Start () {
		allRooms = new GameObject[][] { basicRooms, centerRooms, xRooms, diamondRooms, diamondCenterRooms };
		buildIsComplete = false;
		navMeshBuilt = false;
		startingRoom = selectRandomFrom (startingRooms);
		Instantiate (startingRoom, transform.position, transform.rotation);
		roomCount = 1;
		playerPosition = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
		Instantiate (player, playerPosition, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		newSpawns = false;

		roomSpawns = GameObject.FindGameObjectsWithTag ("Room Spawner");
		foreach (GameObject spawn in roomSpawns) {

			//check to see if there are any new room Spawners. If there are, the build is not done.
			if (spawn.GetComponent<RoomSpawnerScript>().newSpawn){
				newSpawns = true;
			}


			if (roomCount < minNumberOfRooms) {
				if (!Physics.CheckSphere (spawn.transform.position, 1)) {
					GameObject newRoom = selectRandomFrom (selectRoomSet(allRooms));
					if (isInLine (newRoom, spawn)) {
						Instantiate (newRoom, spawn.transform.position, transform.rotation);
						roomCount++;
					}
				}

			} else {
				if (!Physics.CheckSphere (spawn.transform.position, 1)) {
					GameObject newRoom = selectRandomFrom (selectRoomSet(allRooms));
					while(!noNewSpawn(newRoom, spawn) || !isInLine (newRoom, spawn)){
						newRoom = selectRandomFrom (selectRoomSet(allRooms));
					}
					if (isInLine (newRoom, spawn)) {
						Instantiate (newRoom, spawn.transform.position, transform.rotation);
						roomCount++;
					}
				}
			}

			spawn.GetComponent<RoomSpawnerScript> ().newSpawn = false;
		}

		if (!newSpawns){
			buildIsComplete = true;
		}


		if (buildIsComplete) {
			if (roomCount >= minNumberOfRooms && roomCount <= maxNumberOfRooms) {
				if (!navMeshBuilt) {
					//NavMeshBuilder.BuildNavMesh ();
					navMeshBuilt = true;
				}
			} else {
				restartBuild ();
			}
		}
	}
}
