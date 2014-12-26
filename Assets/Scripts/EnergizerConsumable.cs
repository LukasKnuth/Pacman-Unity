using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Consumable))]
public class EnergizerConsumable : MonoBehaviour {

    private GameController controller;

    // Use this for initialization
    void Start()
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
	
	// Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // TODO Change the global mode to SCATTER
            Debug.Log("Change Ghost-Mode to SCATTER!!");
        }
    }
}
