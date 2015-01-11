using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Consumable : MonoBehaviour
{

    public int ScoreValue;
    private GameController controller;

	// Use this for initialization
	void Start ()
	{
        // Find the GameController.
	    GameObject obj = GameObject.FindWithTag("GameController");
	    if (obj != null)
	    {
	        controller = obj.GetComponent<GameController>();
	    }
	    if (controller == null)
	    {
	        Debug.LogError("Can't find GameController!");
	    }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.DotConsumed(this.ScoreValue);
            Destroy(gameObject);
        }
    }

}
