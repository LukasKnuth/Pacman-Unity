using UnityEngine;
using System.Collections;

public class PacmanController : MonoBehaviour
{
    // ---------- PUBLIC INSPECTOR INTERFACE -----------------
    public float Speed;

    // ---------- PUBLIC SCRIPTING INTERFACE -----------------
    public void Reset()
    {
        _currentDirection = Vector3.zero;
        _nextDirection = Vector3.zero;
        transform.position = this._startPosition;
    }

    // ---------- PRIVATE SCRIPTING INTERFACE -----------------
    private Vector3 _currentDirection;
    private Vector3 _nextDirection;
    private Vector3 _startPosition;
    private GameField _map;
    /// <summary>
    ///  Things that we collide with (can't move over/through)
    /// </summary>
    private GameField.Tile collision_flags = GameField.Tile.WALL | GameField.Tile.CAGE_DOOR;

    void Start()
    {
        // Find the Map.
        GameObject obj = GameObject.FindWithTag("Map");
        if (obj != null)
        {
            _map = obj.GetComponent<GameField>();
        }
        if (_map == null)
        {
            Debug.LogError("Can't find GameField!");
        }
        this._startPosition = transform.position;
        // Initialisation:
        this.Reset();
    }

	// Update is called once per frame
	void Update ()
	{
        // Move the character:
	    transform.Translate(GetDirection() * Speed * Time.deltaTime);
	}

    private Vector3 GetDirection()
    {
        // Find the desired next direction:
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (vertical > 0)
        {
            // Going Up:
            _nextDirection = Vector3.forward;
        }
        else if (vertical < 0)
        {
            // Going Down:
            _nextDirection = Vector3.back;
        }
        else if (horizontal > 0)
        {
            _nextDirection = Vector3.right;
        }
        else if (horizontal < 0)
        {
            _nextDirection = Vector3.left;
        }

        // Check if we can go there:
        if (_nextDirection != Vector3.zero && IsDirectionChangePossible(_currentDirection, _nextDirection))
        {
            _currentDirection = _nextDirection;
            _nextDirection = Vector3.zero;
        }
        // Now, check if we can continue on this direction:
        else if (_map.isColliding(transform.position, _currentDirection, this.collision_flags))
        {
            _currentDirection = Vector3.zero;
        }
        return _currentDirection;
    }

    private bool IsDirectionChangePossible(Vector3 currentDirection, Vector3 newDirection)
    {
        if (currentDirection == newDirection)
        {
            // No change...
            return true;
        }
        else
        {
            if (currentDirection == -newDirection)
            {
                // Opposite direction is always allowed!
                return true;
            }
            else
            {
                return _map.canChangeDirection(transform.position, currentDirection, newDirection, collision_flags);
            }
        }
    }
}
