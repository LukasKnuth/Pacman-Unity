using UnityEngine;
using System.Collections;

class Blinky : Ghost
{
    protected override Vector3 GetTargetTile(PacmanController player)
    {
        // Simply follow pacman:
        return player.transform.position;
    }
}