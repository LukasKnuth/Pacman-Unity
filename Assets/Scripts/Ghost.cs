using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Ghost : MonoBehaviour {

    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    public Vector3 HomeCorner;
    public float Speed;

    // ---------- PUBLIC SCRIPTING INTERFACE -----------------
    public enum Mode
    {
        CAGED, SCATTER, FRIGHTENED, CHASE, RETURNING
    }

    /// <summary>
    /// Ghost implementations have to call this method to tell the path-finding algorithm, where it should lead the ghost to.
    /// </summary>
    protected abstract Vector3 GetTargetTile(PacmanController player);

    /// <summary>
    /// Unleashes this ghost into the maze.
    /// </summary>
    public void Unleash(Vector3 direction, Mode mode)
    {
        this._currentDirection = direction;
        SetMode(mode);
    }

    /// <summary>
    /// Reset this Ghost to the given position.
    /// </summary>
    public void Reset(Vector3 position)
    {
        this._currentDirection = Vector3.zero;
        SetMode(Mode.CAGED);
        transform.position = position;
        this._currentField = this._map.findField(transform.position);
    }

    /// <summary>
    /// Change the Mode of this Ghost.
    /// </summary>
    public void SetMode(Mode new_mode)
    {
        this._previousMode = this._currentMode;
        this._currentMode = new_mode;
    }

    // ---------- PRIVATE SCRIPTING INTERFACE ----------------
    private PacmanController _pacman;
    private Cage _cage;
    private GameField _map;
    private Vector3 _currentDirection;
    private GameField.Field _currentField;
    private Mode _currentMode;
    private Mode _previousMode;

	void Start ()
	{
        GameObject pacmanGameObject = GameObject.FindWithTag("Player");
	    if (pacmanGameObject == null)
	    {
	        Debug.LogError("Couldn't find the Player!");
	    }
	    this._pacman = pacmanGameObject.GetComponent<PacmanController>();
	    if (this._pacman == null)
	    {
	        Debug.LogError("Couldn't find Script on pacman!");
	    }
	    GameObject mapGameObject = GameObject.FindWithTag("Map");
	    this._map = mapGameObject.GetComponent<GameField>();
	    if (this._map == null)
	    {
	        Debug.Log("Couldn't find the Map!");
	    }
        GameObject cageObject = GameObject.FindWithTag("Cage");
        if (cageObject != null)
        {
            this._cage = cageObject.GetComponent<Cage>();
        }
        if (this._cage == null)
        {
            Debug.LogError("Can't find the Cage!");
        }
        this.Reset(transform.position);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this._cage.GotPacman();
        }
    }
	
	void Update () {
        // Choose the target based on the current mode:
        Vector3 targetTile = Vector3.zero;
	    switch (this._currentMode)
	    {
	        case Mode.CAGED:
                // TODO Move up and down in the cage...
                targetTile = Vector3.zero;
                break;
            case Mode.SCATTER:
	            targetTile = HomeCorner;
                break;
            case Mode.CHASE:
	            targetTile = GetTargetTile(this._pacman);
                break;
            case Mode.FRIGHTENED:
                // TODO Random tile...
                targetTile = Vector3.zero;
	            break;
            case Mode.RETURNING:
	            targetTile = this._cage.GetReturnPoint();
                break;
	    }
        // Navigate there!
        transform.Translate(GetMoveDirection(targetTile) * Speed * Time.deltaTime);
	}

    private Vector3 GetMoveDirection(Vector3 TargetTile)
    {
        GameField.Tile exclude = GameField.Tile.WALL | GameField.Tile.CAGE_DOOR;
        // Where are we?
        float shortest_distance = float.MaxValue;
        Vector3 next_direction = Vector3.zero;
        Dictionary<Vector3, GameField.RadarResult> radar = _map.getTilesAround(transform.position);
        foreach (KeyValuePair<Vector3, GameField.RadarResult> direction in radar)
        {
            if (direction.Key != -this._currentDirection)
            {
                // It's not the opposite direction
                if ((exclude & direction.Value.Tile) != direction.Value.Tile)
                {
                    // The tile is a valid option:
                    float distance = Vector3.Distance(direction.Value.Field, TargetTile);
                    if (distance < shortest_distance)
                    {
                        next_direction = direction.Key;
                        shortest_distance = distance;
                    }
                }
            }
        }
        // Only turn if we're around the middle of the hallway
        if (this._map.canChangeDirection(transform.position, this._currentDirection, next_direction, exclude))
        {
            // Check if we moved to the next field:
            if (this._currentField == this._map.findField(transform.position))
            {
                // A change of direction is only allowed once per field!
                return _currentDirection;
            }
            else
            {
                this._currentField = this._map.findField(transform.position);
                this._currentDirection = next_direction;
            }
        }
        return this._currentDirection;
    }
}