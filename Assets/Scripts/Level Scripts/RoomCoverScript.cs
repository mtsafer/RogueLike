using UnityEngine;
using System.Collections;

public class RoomCoverScript : MonoBehaviour
{

    private bool reveal;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        reveal = gameObject.GetComponentInParent<RoomScript>().playerEntered;
        if (reveal)
        {
            Destroy(this.gameObject);
        }
    }
}
