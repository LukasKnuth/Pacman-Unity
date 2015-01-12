using UnityEngine;
using System.Collections;
using Pacman.Map;

public class Inky : Ghost
{

    private GameObject _blinky;

	// Use this for initialization
    new void Start() {
        base.Start();
        // Get Blinky:
        this._blinky = GameObject.FindWithTag("Blinky");
        if (this._blinky == null)
        {
            Debug.LogError("Couldn't find Blinky!");
        }
    }

    protected override Vector3 GetTargetTile(PacmanController player)
    {
        Tile pacmanTile = base.GetMap().GetTileAt(player.transform.position).
            InDirection(player.GetCurrentDirection(), 2); // With added offset of 2 in moving direction.
        Tile blinkyTile = base.GetMap().GetTileAt(_blinky.transform.position);

        // Calculate the Direction-Vector:
        Vector3 direction = pacmanTile.Position - blinkyTile.Position;
        // Add the direction, effectively doubling it's length:
        Vector3 target = pacmanTile.Position + direction;
        Debug.DrawLine(blinkyTile.Position, target, Color.red);
        Debug.DrawLine(blinkyTile.Position, pacmanTile.Position, Color.green);
        return target;
    }
    
}
