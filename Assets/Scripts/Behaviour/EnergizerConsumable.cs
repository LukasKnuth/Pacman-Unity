using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Consumable))]
public class EnergizerConsumable : MonoBehaviour {

    private Cage _cage;

    // Use this for initialization
    void Start()
    {
        // Find the GameController.
        GameObject obj = GameObject.FindWithTag("Cage");
        if (obj != null)
        {
            this._cage = obj.GetComponent<Cage>();
        }
        if (this._cage == null)
        {
            Debug.LogError("Can't find Cage!");
        }
    }
	
	// Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this._cage.EnergizerConsumed();
        }
    }
}
