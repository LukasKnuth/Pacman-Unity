using UnityEngine;
using System.Collections;

public class Cage : MonoBehaviour {

    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    public GameObject[] Ghosts;
    public Vector3 ReturnPoint;

    // ---------- PUBLIC SCRIPTING INTERFACE -----------------

    /// <summary>
    /// To be called by any ghost that collided with pacman.
    /// </summary>
    public void GotPacman()
    {
        this._gameController.SubstractLive();
    }

    /// <summary>
    /// Called by the <see cref="GameController"/> to reset all Ghosts.
    /// </summary>
    public void ResetGhosts()
    {
        this._blinky.AI.Reset(_blinky.ResetPosition);
    }

    /// <summary>
    /// Start the countdowns to unleash the ghosts.
    /// </summary>
    public void StartGhosts()
    {
        _blinky.AI.Unleash(Vector3.left, Ghost.Mode.CHASE);
        // TODO Start countdowns for other ghosts.
        // Start the mode changes:
        this._currentDuration = 0;
        this._currentModeIteration = 0;
    }

    /// <summary>
    /// Get the point where the ghost's should go to, when they we're consumed.
    /// </summary>
    public Vector3 GetReturnPoint()
    {
        return this.ReturnPoint;
    }

    /// <summary>
    /// Makes all ghosts eatable and run away from the player.
    /// </summary>
    public void EnergizerConsumed()
    {
        this._blinky.AI.SetMode(Ghost.Mode.FRIGHTENED);
        // TODO Add the others!
        this._frightenedTimer = FRIGHTENED_TIME;
    }

    /// <summary>
    /// The maximum time a ghost is in frightened mode, before changing back to his previous mode.
    /// </summary>
    public const float FRIGHTENED_TIME = 6f;

    // ---------- PRIVATE SCRIPTING INTERFACE -----------------
    private GhostHolder _blinky;
    private GameController _gameController;

	// Use this for initialization
	void Start () {
	    foreach (GameObject g in Ghosts)
	    {
	        Ghost ghost = g.GetComponent<Ghost>();
	        if (ghost is Blinky)
	        {
	            this._blinky = new GhostHolder(ghost, g.transform.position);
	        }
	    }
	    GameObject controllerGameObject = GameObject.FindWithTag("GameController");
	    this._gameController = controllerGameObject.GetComponent<GameController>();
	    if (this._gameController == null)
	    {
	        Debug.LogError("Couldn't find the game-controller!");
	    }

	}
	
    /// <summary>
    /// A holder for everything related to a single ghost.
    /// </summary>
    private class GhostHolder
    {
        public Ghost AI { get; private set; }
        public Vector3 ResetPosition { get; private set; }

        public GhostHolder(Ghost ai, Vector3 resetPosition)
        {
            AI = ai;
            ResetPosition = resetPosition;
        }
    }

    private class ModeChange
    {
        public int Duration { get; private set; }
        public Ghost.Mode Mode { get; private set; }

        public ModeChange(Ghost.Mode mode, int duration)
        {
            Duration = duration;
            Mode = mode;
        }
    }
    private readonly ModeChange[] MODE_CHANGES =
    {
        new ModeChange(Ghost.Mode.SCATTER, 7),
        new ModeChange(Ghost.Mode.CHASE, 20),
        new ModeChange(Ghost.Mode.SCATTER, 7), 
        new ModeChange(Ghost.Mode.CHASE, 20),
        new ModeChange(Ghost.Mode.SCATTER, 5), 
        new ModeChange(Ghost.Mode.CHASE, 20),
        new ModeChange(Ghost.Mode.SCATTER, 5),
        new ModeChange(Ghost.Mode.CHASE, 0), 
    };

    // TODO also, check if stopping the game by setting timeScale = 0 stops the timers, too!

    private int _currentModeIteration = 0;
    private float _currentDuration = 0;
    private float _frightenedTimer = 0;
    void Update()
    {
        if (this._frightenedTimer > 0)
        {
            if (_frightenedTimer > 0 && _frightenedTimer - Time.deltaTime <= 0)
            {
                Debug.Log("Fright time over!");
            }
            // Ghosts are frightened, do the frightened timer:
            this._frightenedTimer -= Time.deltaTime;
        }
        else
        {
            // Ghosts are not frightened, do the mode-timer:
            if (this._currentDuration > 0)
            {
                this._currentDuration -= Time.deltaTime;
            } else {
                // Check if there are changes left:
                if (this._currentModeIteration >= MODE_CHANGES.Length) return;
                // Countdown is over:
                ModeChange change = MODE_CHANGES[_currentModeIteration];
                _blinky.AI.SetMode(change.Mode);
                // TODO Add the others!
                // new Countdown:
                this._currentDuration = change.Duration;
                this._currentModeIteration++;

                Debug.Log("Time over, next mode "+change.Mode+" for "+change.Duration+"s");
            }
        }
    }
}
