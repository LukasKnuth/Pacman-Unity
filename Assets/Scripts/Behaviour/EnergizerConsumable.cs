using UnityEngine;
using System.Collections;

public class EnergizerConsumable : Consumable {

    private Cage _cage;

    // Use this for initialization
    void Start()
    {
        base.Start();
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

    protected override void Consumed()
    {
        this._cage.EnergizerConsumed();
        base.Consumed();
    }
}
