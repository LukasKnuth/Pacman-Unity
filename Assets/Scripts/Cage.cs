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
    }

    /// <summary>
    /// Get the point where the ghost's should go to, when they we're consumed.
    /// </summary>
    public Vector3 GetReturnPoint()
    {
        return this.ReturnPoint;
    }

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
	
}
