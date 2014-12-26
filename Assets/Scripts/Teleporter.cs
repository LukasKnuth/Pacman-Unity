using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Teleporter : MonoBehaviour
{

    public GameObject OtherEnd;
    private Teleporter OtherEndTeleporter;

	// Use this for initialization
	void Start () {
	    if (OtherEnd == null)
	    {
	        Debug.LogError("The teleporter needs another end teleport to.");
	    }
	    else
	    {
	        OtherEndTeleporter = OtherEnd.GetComponent<Teleporter>();
	        if (OtherEndTeleporter == null)
	        {
	            Debug.LogError("The other end is NOT a Teleporter!");
	        }
	    }
	}

    private void Teleport(GameObject obj)
    {
        // Find out in which direction we teleported:
        float direction = this.transform.position.x - obj.transform.position.x;
        Vector3 end = this.transform.position;
        if (direction > 0)
        {
            // From left to right:
            end.x -= 10;
        }
        else if (direction < 0)
        {
            // From right to left:
            end.x += 10;
        }
        obj.transform.position = end;
    }

    void OnTriggerEnter(Collider other)
    {
        OtherEndTeleporter.Teleport(other.gameObject);
    }
}
