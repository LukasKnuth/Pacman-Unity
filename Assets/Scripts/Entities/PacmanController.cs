using System.Collections.Generic;
using Pacman.Map;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
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

    public Vector3 GetCurrentDirection() {
        return this._currentDirection;
    }

    // ---------- PRIVATE SCRIPTING INTERFACE -----------------
    private Vector3 _currentDirection;
    private Vector3 _nextDirection;
    private Vector3 _startPosition;
    private GameField _map;

    /// <summary>
    ///  Things that we collide with (can't move over/through)
    /// </summary>
    private GameField.TileType collision_flags = GameField.TileType.Wall | GameField.TileType.CageDoor;

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
        else {
            Tile currentTile = this._map.GetTileAt(transform.position);
            Tile nextTile = currentTile.InDirection(this._currentDirection);
            if ((collision_flags & nextTile.Type) == nextTile.Type)
            {
                // We are on collision-course, see if we should stop now (at the center):
                if (transform.position.IsCenteredOn(currentTile, this._currentDirection))
                {
                    this._currentDirection = Vector3.zero;
                }
            }
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
            Tile next = this._map.GetTileAt(transform.position).InDirection(newDirection);
            if (newDirection == -currentDirection)
            {
                // Opposite direction is always allowed, if there is no wall:
                return ((collision_flags & next.Type) != next.Type);
                // TODO This still bugs sometimes (allows you to go through walls when doing it fast). WHY?
            }
            else
            {
                if ((collision_flags & next.Type) == next.Type) {
                    // Can't go in that direction:
                    return false;
                } else {
                    // Check if we're centered:
                    Tile currentTile = this._map.GetTileAt(transform.position);
                    return transform.position.IsCenteredOn(currentTile, this._currentDirection);
                }
            }
        }
    }
}
