using UnityEngine;
using System.Collections;

public class PacmanController : MonoBehaviour
{

    public float Speed;
    private Vector3 CurrentDirection;
    private Vector3 NextDirection;
    private GameField map;
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
            map = obj.GetComponent<GameField>();
        }
        if (map == null)
        {
            Debug.LogError("Can't find GameField!");
        }
        // Initialisation:
        CurrentDirection = Vector3.zero;
        NextDirection = Vector3.zero;
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
            NextDirection = Vector3.forward;
        }
        else if (vertical < 0)
        {
            // Going Down:
            NextDirection = Vector3.back;
        }
        else if (horizontal > 0)
        {
            NextDirection = Vector3.right;
        }
        else if (horizontal < 0)
        {
            NextDirection = Vector3.left;
        }

        // Check if we can go there:
        if (NextDirection != Vector3.zero && IsDirectionChangePossible(CurrentDirection, NextDirection))
        {
            CurrentDirection = NextDirection;
            NextDirection = Vector3.zero;
        }
        // Now, check if we can continue on this direction:
        else if (map.isColliding(transform.position, CurrentDirection, this.collision_flags))
        {
            CurrentDirection = Vector3.zero;
        }
        return CurrentDirection;
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
                return map.canChangeDirection(transform.position, currentDirection, newDirection, collision_flags);
            }
        }
    }
}
