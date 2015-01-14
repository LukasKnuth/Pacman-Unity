using System.Collections.Generic;
using Pacman.Map;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public abstract class Ghost : MonoBehaviour {

    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    public Vector3 HomeCorner;
    public float Speed;
    public Material FrightenedMaterial;

    // ---------- PUBLIC SCRIPTING INTERFACE -----------------
    public enum Mode
    {
        CAGED, SCATTER, FRIGHTENED, CHASE
    }

    /// <summary>
    /// Ghost implementations have to call this method to tell the path-finding algorithm, where it should lead the ghost to.
    /// </summary>
    protected abstract Vector3 GetTargetTile(PacmanController player);

    /// <summary>
    /// Unleashes this ghost into the maze.
    /// </summary>
    public void Unleash(Vector3 direction, Mode mode, bool exitCage = true)
    {
        this._currentDirection = direction;
        if (exitCage)
        {
            // The ghost is starting from the cage, move out first:
            this._leavingCage = true;
        }
        SetMode(mode, false);
    }

    /// <summary>
    /// Reset this Ghost to the given position.
    /// </summary>
    public void Reset(Vector3 position)
    {
        this._currentDirection = Vector3.zero;
        SetMode(Mode.CAGED, false);
        transform.position = position;
        renderer.material = _originalMaterial;
        this._currentTile = this._map.GetTileAt(position);
        this._leavingCage = false;
        this._isReturning = false;
    }

    /// <summary>
    /// Change the Mode of this Ghost.
    /// </summary>
    public void SetMode(Mode new_mode, bool force_reversal = true)
    {
        // TODO If the ghost is in the cage, don't change the mode
        this._previousMode = this._currentMode;
        this._currentMode = new_mode;
        if (!this._isReturning && force_reversal)
        {
            this._currentDirection = -this._currentDirection;
            // We need to force a re-calculation of the target-tile:
            this._currentTile = null;
        }
        if (new_mode == Mode.FRIGHTENED) {
            this._frightenedTimerStart = Time.time;
            renderer.material = FrightenedMaterial;
        }
        Debug.Log("New Mode is: " + _currentMode);
    }

    // ---------- PROTECTED SCRIPTING INTERFACE ----------------
    protected GameField GetMap()
    {
        return this._map;
    }

    // ---------- PRIVATE SCRIPTING INTERFACE ----------------
    private PacmanController _pacman;
    private Cage _cage;
    private GameField _map;
    private Vector3 _currentDirection;
    private Tile _currentTile;
    private Mode _currentMode;
    private Mode _previousMode;
    private Material _originalMaterial;
    
    private float _frightenedTimerStart;
    private bool _inTeleporter = false;
    private bool _leavingCage = false;
    private bool _isReturning = false;

    private const float FRIGHTENED_SPEED_PENALTY = 0.5f; // -50%
    private const float TELEPORTER_SPEED_PENALTY = 0.6f; // -40%
    private const float RETURNING_SPEED_PENALTY = 1.5f;  // +50%

	protected void Start ()
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
	    if (this.FrightenedMaterial == null)
	    {
	        Debug.LogError("No material for Frightened mode was added!");
	    }
	    this._originalMaterial = renderer.material;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (this._isReturning) {
                // Already dead, ignore.
                return;
            }
            else if (this._currentMode == Mode.FRIGHTENED) {
                // He got me:
                this._isReturning = true;
                this._frightenedTimerStart = 0;
                this._cage.GotGhost(this);
            }
            else {
                // Got him!
                this._cage.GotPacman();
            }
        }
    }
	
	void Update () {
        // Check the frightened timer:
	    if (this._frightenedTimerStart > 0
	        && (this._frightenedTimerStart + Cage.FRIGHTENED_TIME <= Time.time))
	    {
	        SetMode(this._previousMode, false);
	        this._frightenedTimerStart = 0;
            renderer.material = _originalMaterial;
	    }
        // If we're returning, check if we are close enough to the return point:
	    if (this._isReturning)
	    {
            float distance = Vector3.Distance(transform.position, this._cage.GetReturnPoint());
            if (distance < 8)
            {
                this._isReturning = false;
                this._currentDirection = Vector3.forward;
                renderer.material = _originalMaterial;
            }
	    }
        // If we're leaving the cage, check if we're out yet:
	    if (_leavingCage)
	    {
	        float distance = Vector3.Distance(transform.position, this._cage.GetExitPoint());
	        if (distance < 5)
	        {
	            this._leavingCage = false;
	        }
	    }
        // Choose the target based on the current mode:
        Vector3 targetTile = Vector3.zero;
        float currentSpeedPenalty = 1f;
	    if (_leavingCage)
	    {
	        targetTile = this._cage.GetExitPoint();
        }
        else if (this._isReturning)
        {
            currentSpeedPenalty = RETURNING_SPEED_PENALTY;
            targetTile = this._cage.GetReturnPoint();
        }
	    else
	    {
            switch (this._currentMode) {
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
                    currentSpeedPenalty = FRIGHTENED_SPEED_PENALTY;
                    targetTile = this._map.GetRandomTile().Position;
                    break;
            }
	    }
        // Extra slowdown if we're in the Teleporter:
	    if (_inTeleporter)
	    {
	        currentSpeedPenalty = TELEPORTER_SPEED_PENALTY;
	    }
        // Navigate there!
        transform.Translate(
            GetMoveDirection(targetTile) * (Speed * currentSpeedPenalty) * Time.deltaTime
        );
	}

    private Vector3 GetMoveDirection(Vector3 TargetTile)
    {
        GameField.TileType exclude = GameField.TileType.Wall | GameField.TileType.CageDoor;
        if (this._isReturning || this._leavingCage)
        {
            // We need to be able to pass through the Cage-Door when Returning/Leaving
            exclude ^= GameField.TileType.CageDoor;
        }
        // Where are we?
        float shortestDistance = float.MaxValue;
        Vector3 nextDirection = Vector3.zero;
        Tile currentTile = this._map.GetTileAt(transform.position);
        foreach (KeyValuePair<Vector3, Tile> direction in currentTile.Around())
        {
            if (direction.Key != -this._currentDirection)
            {
                // It's not the opposite direction
                if ((exclude & direction.Value.Type) != direction.Value.Type)
                {
                    // The tile is a valid option:
                    float distance = Vector3.Distance(direction.Value.Position, TargetTile);
                    if (distance < shortestDistance)
                    {
                        nextDirection = direction.Key;
                        shortestDistance = distance;
                        _inTeleporter = (direction.Value.Type == GameField.TileType.Teleporter);
                    }
                }
            }
        }
        // Only turn if we're around the middle of the hallway
        if (transform.position.IsCenteredOn(currentTile, this._currentDirection))
        {
            // Check if we moved to the next field:
            if (this._currentTile == this._map.GetTileAt(transform.position))
            {
                // A change of direction is only allowed once per field!
                return _currentDirection;
            }
            else
            {
                this._currentTile = this._map.GetTileAt(transform.position);
                this._currentDirection = nextDirection;
            }
        }
        return this._currentDirection;
    }
}