using UnityEngine;
using System.Collections;
using Pacman.Map;

public class Clyde : Ghost {

	protected override Vector3 GetTargetTile(PacmanController player)
	{
	    float distance = Vector3.Distance(this.transform.position, player.transform.position);
	    if ((distance / base.GetMap().TileSize) >= 8)
	    {
	        // We are too far away, attack pacman directly:
	        return player.transform.position;
	    }
	    else
	    {
	        // We're too close, retreat to home corner:
	        return this.HomeCorner;
	    }
	}
}
