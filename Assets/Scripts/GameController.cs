using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{

    public Text PointsDisplay;
    public Text LivesDisplay;
    private int points;
    private int lives;

	// Use this for initialization
	void Start ()
	{
	    points = 0;
	    lives = 3;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddPoints(int points)
    {
        this.points += points;
        this.PointsDisplay.text = "Points: " + this.points;
    }

    private void SubstractLive()
    {
        this.lives--;
        LivesDisplay.text = "Lives: " + lives;
    }

    private void AddLive()
    {
        this.lives++;
        LivesDisplay.text = "Lives: " + lives;
    }
}
