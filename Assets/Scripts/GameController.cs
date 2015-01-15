using System;
using Pacman.Map;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    [Header("UI Elements")]
    public Text PointsDisplay;
    public Text LivesDisplay;
    [Header("Bonus Prefabs")]
    public GameObject BonusFruitPrefab;
    [Header("On-Screen UI Prefabs")]
    public Canvas UiCanvas;
    public GameObject FieldPointsPrefab;
    [Header("Pause Menu")]
    public GameObject PauseMenu;
    [Header("GameOver Menu")]
    public GameObject GameOverMenu;
    public Text GameOverPoints;
    public Text GameOverText;

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
        // Check if we won:
        if (_dotsConsumed == 244)
        {
            this.GameOver(true, this._points);
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
            // Player is dead:
            this.GameOver(false, this._points);
        }
    }

    public void AddLive()
    {
        this._lives++;
        LivesDisplay.text = "Lives: " + _lives;
    }

    public void MenuRestart()
    {
        this.Reset();
        this.MenuContinue();
        this._cage.StartGhosts();
    }
    public void MenuContinue()
    {
        Time.timeScale = 1;
        this._isPaused = false;
        this.PauseMenu.SetActive(false);
        this.GameOverMenu.SetActive(false);
    }

    // ---------- PRIVATE SCRIPTING INTERFACE -----------------
    private int _points;
    private int _lives;
    private int _dotsConsumed = 0;
    private Cage _cage;
    private PacmanController _player;
    private GameField _map;

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

    private void GameOver(bool win, int points)
    {
        Time.timeScale = 0;
        GameOverPoints.text = points + " Points";
        GameOverText.text = (win) ? "You Won!" : "Game Over!";
        // TODO Play sound?
        GameOverMenu.SetActive(true);
    }

    /// <summary>
    /// Reset the entire game completely to start over again.
    /// </summary>
    private void Reset() {
        _points = 0;
        this.PointsDisplay.text = "Points: " + this._points;
        _lives = 3;
        this.LivesDisplay.text = "Lives: " + this._lives;
        _dotsConsumed = 0;
        if (this._started)
        {
            _player.Reset();
            _cage.ResetGhosts();
            _map.Reset();
        }
    }

	// Use this for initialization
	void Start ()
	{
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
	    GameObject mapGameObject = GameObject.FindWithTag("Map");
	    this._map = mapGameObject.GetComponent<GameField>();
	    if (this._map == null)
	    {
	        Debug.LogError("Couldn't find the Map!");
	    }
        this.Reset();
	}

    private bool _started = false;
    private bool _isPaused = false;
    void Update()
    {
        if (!_started)
        {
            this._cage.ResetGhosts();
            this._cage.StartGhosts();
            _started = true;
        }
        if (Input.GetButtonUp("Pause"))
        {
            if (this._isPaused)
            {
                this.MenuContinue();
            }
            else
            {
                // Pause:
                Time.timeScale = 0;
                this._isPaused = true;
            }
            this.PauseMenu.SetActive(this._isPaused);
        }
    }
}
