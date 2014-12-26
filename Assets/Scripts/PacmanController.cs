using UnityEngine;
using System.Collections;

public class PacmanController : MonoBehaviour
{

    public float Speed;
    private Vector3 CurrentDirection;
    private float RayLength = 10f;
    private GameField map;

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
    }

	// Update is called once per frame
	void Update ()
	{
        // Move the character:
	    transform.Translate(GetDirection() * Speed * Time.deltaTime);
	}

    private Vector3 GetDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal > 0)
        {
            // Going Right
            if (IsDirectionChangePossible(CurrentDirection, Vector3.right))
            {
                CurrentDirection = Vector3.right;
            }
        }
        else if (horizontal < 0)
        {
            // Going left
            if (IsDirectionChangePossible(CurrentDirection, Vector3.left))
            {
                CurrentDirection = Vector3.left;
            }
        }
        else if (vertical > 0)
        {
            // Going Up:
            if (IsDirectionChangePossible(CurrentDirection, Vector3.forward))
            {
                CurrentDirection = Vector3.forward;
            }
        }
        else if (vertical < 0)
        {
            // Going Down:
            if (IsDirectionChangePossible(CurrentDirection, Vector3.back))
            {
                CurrentDirection = Vector3.back;
            }
        }
        // Now, check if we can continue on this direction:
        switch (map.getNextTile(transform.position, CurrentDirection))
        {
            case GameField.Tile.WALL:
            case GameField.Tile.CAGE_DOOR:
                CurrentDirection = Vector3.zero;
                break;
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
                Debug.Log("Opposite!");
                return true;
            }
            else
            {
                GameField.Tile tile = map.getNextTile(transform.position, newDirection);
                switch (tile)
                {
                    case GameField.Tile.WALL:
                    case GameField.Tile.CAGE_DOOR:
                        return false;
                    case GameField.Tile.DOT:
                    case GameField.Tile.ENERGIZER:
                    case GameField.Tile.FREE:
                    case GameField.Tile.TELEPORTER:
                        return true;
                    default:
                        Debug.LogError("Unknown Tile type: "+tile);
                        return false;
                }
                /*RaycastHit hit;
                Ray ray = new Ray(transform.position, newDirection);
                int layerMask = 1 << 8;
                if (Physics.Raycast(ray, out hit, RayLength, layerMask))
                {
                    // Can't change directions here, we would run into a Wall.
                    Debug.Log("Ray hit "+hit.collider.gameObject.name);
                    return false;
                }
                return true;*/
            }
        }
    }
}
