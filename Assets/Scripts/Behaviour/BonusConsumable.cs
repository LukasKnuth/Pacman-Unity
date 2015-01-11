using UnityEngine;
using System.Collections;

public class BonusConsumable : Consumable {

    protected override void Consumed()
    {
        base.controller.BonusConsumed(base.ScoreValue, transform.position);
        Destroy(gameObject);
    }
}