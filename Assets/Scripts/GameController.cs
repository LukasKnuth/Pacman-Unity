using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    public Text PointsDisplay;
    public Text LivesDisplay;
    public GameObject FieldPointsPrefab;
    public GameObject BonusFruitPrefab;
    public Canvas UiCanvas;

    // ---------- PUBLIC SCRIPTING INTERFACE -----------------
    /// <summary>
    /// To be called, when a normal Dot or Energizer was eaten by the player.
    /// </summary>
    public void DotConsumed(int points_worth) {
        this.AddPoints(points_worth);
        this._dotsConsumed++;
        this._cage.DotConsumed(_dotsConsumed);
        // Bonus Fruit for extra points!
        if (_dotsConsumed == 70 || _dotsConsumed == 170) {
            Vector3 bonusPosition = new Vector3(130, 0, -166);
            Quaternion bonusRotation = Quaternion.identity;
            Instantiate(BonusFruitPrefab, bonusPosition, bonusRotation);
        }
    }

    /// <summary>
    /// To be called, when a Ghost was eaten by the player.
    /// </summary>
    public void GhostConsumed(int points_worth, Vector3 position) {
        this.AddPoints(points_worth);
        // Display the points this kill was worth:
        this.DisplayOnScreenPoints(position, points_worth.ToString());
    }

    /// <summary>
    /// To be called, when the Player ate the bonus-fruit.
    /// </summary>
    public void BonusConsumed(int points_worth, Vector3 position)
    {
        this.AddPoints(points_worth);
        this.DisplayOnScreenPoints(position, points_worth.ToString());
        // TODO play sound, etz...
    }

    /// <summary>
    /// Get the number of Dots that where consumed until now.
    /// </summary>
    public int GetDotsConsumed()
    {
        return this._dotsConsumed;
    }

    public void SubstractLive()
    {
        this._lives--;
        if (this._lives > 0)
        {
            LivesDisplay.text = "Lives: " + _lives;
            // TODO Player Death animation/sound??
            // Reset erverything:
            this._cage.ResetGhosts();
            this._player.Reset();
            // Start the game again:
            this._cage.StartGhosts();
        }
        else
        {
            // TODO Display game over, reset everything!
        }
    }

    public void AddLive()
    {
        this._lives++;
        LivesDisplay.text = "Lives: " + _lives;
    }

    // ---------- PRIVATE SCRIPTING INTERFACE -----------------
    private int _points;
    private int _lives;
    private int _dotsConsumed = 0;
    private Cage _cage;
    private PacmanController _player;

    private readonly Quaternion _textRotation = Quaternion.AngleAxis(90, Vector3.right);
    private void DisplayOnScreenPoints(Vector3 position, string text)
    {
        GameObject ui = Instantiate(FieldPointsPrefab, position, this._textRotation) as GameObject;
        ui.transform.SetParent(UiCanvas.transform, true);
        Text onscreen_text = ui.GetComponent<Text>();
        if (onscreen_text == null) {
            Debug.LogError("Text not found on kill-display component!");
        }
        onscreen_text.text = text;
    }

    private void AddPoints(int points)
    {
        this._points += points;
        this.PointsDisplay.text = "Points: " + this._points;
    }

	// Use this for initialization
	void Start ()
	{
	    _points = 0;
	    _lives = 3;
	    GameObject cageGameObject = GameObject.FindWithTag("Cage");
	    this._cage = cageGameObject.GetComponent<Cage>();
	    if (this._cage == null)
	    {
	        Debug.LogError("Couldn't find the cage!");
	    }
	    GameObject playerGameObject = GameObject.FindWithTag("Player");
	    this._player = playerGameObject.GetComponent<PacmanController>();
	    if (this._player == null)
	    {
	        Debug.LogError("Didn't find player!");
	    }
	}

    private bool _started = false;
    private bool _paused = false;
    void Update()
    {
        if (!_started)
        {
            this._cage.ResetGhosts();
            this._cage.StartGhosts();
            _started = true;
        }
        
    }
}
