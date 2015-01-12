using UnityEngine;

public class Cage : MonoBehaviour {

    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    public GameObject[] Ghosts;
    public Vector3 ReturnPoint;
    public Vector3 ExitPoint;

    // ---------- PUBLIC SCRIPTING INTERFACE -----------------

    /// <summary>
    /// To be called by any ghost that collided with pacman.
    /// </summary>
    public void GotPacman()
    {
        this._gameController.SubstractLive();
    }

    private readonly int[] _killWorth = { 200, 400, 800, 1600 };
    /// <summary>
    /// Called, when the palyer ate a ghost (while in frightened mode)
    /// </summary>
    public void GotGhost(Ghost ghost) {
        this._ghostKillCombo++;
        int worth = _killWorth[this._ghostKillCombo - 1];
        this._gameController.GhostConsumed(worth, ghost.transform.position);
    }

    /// <summary>
    /// Called by the <see cref="GameController"/> to reset all Ghosts.
    /// </summary>
    public void ResetGhosts() {
        foreach (GhostHolder ghost in this._allGhosts) {
            ghost.AI.Reset(ghost.ResetPosition);
        }
    }

    /// <summary>
    /// Starts a new round and the countdowns to unleash the ghosts.
    /// </summary>
    public void StartGhosts()
    {
        this._allGhosts[BLINKY_INDEX].AI.Unleash(Vector3.left, Ghost.Mode.CHASE, false);
        this._allGhosts[PINKY_INDEX].AI.Unleash(Vector3.forward, Ghost.Mode.CHASE);
        // Start the mode changes:
        this._currentDuration = 0;
        this._currentModeIteration = 0;
        this._ghostKillCombo = 0;
    }

    /// <summary>
    /// Get the point where the ghost's should go to, when they we're consumed.
    /// </summary>
    public Vector3 GetReturnPoint()
    {
        return this.ReturnPoint;
    }

    /// <summary>
    /// The point that ghosts should target, when leaving the cage.
    /// </summary>
    public Vector3 GetExitPoint()
    {
        return this.ExitPoint;
    }

    /// <summary>
    /// Makes all ghosts eatable and run away from the player.
    /// </summary>
    public void EnergizerConsumed()
    {
        foreach (GhostHolder ghost in this._allGhosts) {
            ghost.AI.SetMode(Ghost.Mode.FRIGHTENED);
        }
        this._frightenedTimer = FRIGHTENED_TIME;
    }

    /// <summary>
    /// The player has consumed a new Dot.
    /// </summary>
    public void DotConsumed(int totalConsumed) {
        if (totalConsumed == 30)
        {
            this._allGhosts[INKY_INDEX].AI.Unleash(Vector3.forward, Ghost.Mode.CHASE);
        }
        if (totalConsumed == 60)
        {
            this._allGhosts[CLYDE_INDEX].AI.Unleash(Vector3.forward, Ghost.Mode.CHASE);
        }
    }

    /// <summary>
    /// The maximum time a ghost is in frightened mode, before changing back to his previous mode.
    /// </summary>
    public const float FRIGHTENED_TIME = 6f;

    // ---------- PRIVATE SCRIPTING INTERFACE -----------------
    private const int BLINKY_INDEX = 0;
    private const int PINKY_INDEX = 1;
    private const int INKY_INDEX = 2;
    private const int CLYDE_INDEX = 3;
    private int _ghostKillCombo = 0;
    private GhostHolder[] _allGhosts;
    private GameController _gameController;

	// Use this for initialization
	void Start () {
        this._allGhosts = new GhostHolder[Ghosts.Length];
	    foreach (GameObject g in Ghosts)
	    {
	        Ghost ghost = g.GetComponent<Ghost>();
	        if (ghost is Blinky)
	        {
	            this._allGhosts[BLINKY_INDEX] = new GhostHolder(ghost, g.transform.position);
            }
            else if (ghost is Pinky) {
                this._allGhosts[PINKY_INDEX] = new GhostHolder(ghost, g.transform.position);
            }
            else if (ghost is Inky)
            {
                this._allGhosts[INKY_INDEX] = new GhostHolder(ghost, g.transform.position);
            }
            else if (ghost is Clyde)
            {
                this._allGhosts[CLYDE_INDEX] = new GhostHolder(ghost, g.transform.position);
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

    /// <summary>
    /// A change of the global Ghost mode, followed by waiting for the given duration.
    /// </summary>
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
            if (_frightenedTimer > 0 && _frightenedTimer - Time.deltaTime <= 0) {
                this._ghostKillCombo = 0;
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
                foreach (GhostHolder ghost in this._allGhosts) {
                    ghost.AI.SetMode(change.Mode);
                }
                // new Countdown:
                this._currentDuration = change.Duration;
                this._currentModeIteration++;

                Debug.Log("Time over, next mode "+change.Mode+" for "+change.Duration+"s");
            }
        }
    }
}
