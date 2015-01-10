using Pacman.Map;
using UnityEngine;
using System.Collections;

public class Pinky : Ghost {
    private const int TILE_OFFSET = 4;

    protected override Vector3 GetTargetTile(PacmanController player) {
        Tile current = base.GetMap().GetTileAt(player.transform.position);
        return current.InDirection(player.GetCurrentDirection(), TILE_OFFSET).Position;
    }
}
