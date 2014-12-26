using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections;

public class GameField : MonoBehaviour
{
    // ------------ INSPECTOR INTERFACE ---------------
    public int TileSize;

    // ------------- PUBLIC SCRIPTING INTERFACE ----------------
    [Flags]
    public enum Tile
    {
        FREE=1, WALL=2, DOT=4, ENERGIZER=8, TELEPORTER=16, CAGE_DOOR=32
    }
    

    public Tile getTileAt(Vector3 position)
    {
        Field field = findField(position);
        Tile tile = game_field[field.Y][field.X];
        return tile;
    }

    public Tile getNextTile(Vector3 position, Vector3 direction)
    {
        Field field = findField(position);
        Vector3 dir = getAbsoluteDirection(direction);
        if (dir == Vector3.forward) {
            return game_field[field.Y - 1][field.X];
        } else if (dir == Vector3.back) {
            return game_field[field.Y + 1][field.X];
        } else if (dir == Vector3.left) {
            return game_field[field.Y][field.X - 1];
        } else if (dir == Vector3.right) {
            return game_field[field.Y][field.X + 1];
        } else {
            Debug.LogError("Invalid direction!");
            return Tile.WALL;
        }
    }

    public bool isColliding(Vector3 position, Vector3 direction, Tile collision_tiles)
    {
        Tile next = getNextTile(position, direction);
        if ((collision_tiles & next) == next)
        {
            // We will collide, check if we should stop already:
            return centeredOnTile(position, direction);
        }   
        else
        {
            // Not interested in that kind of field:
            return false;
        }
    }

    public bool canChangeDirection(Vector3 position, Vector3 current_direction, Vector3 next_direction,
        Tile collision_tiles)
    {
        Tile next = getNextTile(position, getAbsoluteDirection(next_direction));
        if ((collision_tiles & next) == next)
        {
            // The Tile in the given direction will colide, no change possible!
            return false;
        }
        else
        {
            return centeredOnTile(position, current_direction);
        }
    }

    // ----------------- PRIVATE INTERFACE -------------------------

    private class Field
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Field(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return String.Format("({0}|{1})", X, Y);
        }
    }

    private Field findField(Vector3 position)
    {
        int x = (int)(Math.Abs(position.x) / TileSize) + 1;
        int y = (int)(Math.Abs(position.z) / TileSize) + 1;
        return new Field(x, y);
    }

    private Field getPositionOnTile(Vector3 position)
    {
        int x = (int)(Math.Abs(position.x) % TileSize);
        int y = (int)(Math.Abs(position.z) % TileSize);
        return new Field(x, y);
    }

    private bool centeredOnTile(Vector3 position, Vector3 direction)
    {
        Field onTile = getPositionOnTile(position);
        Vector3 absolute_direction = getAbsoluteDirection(direction);
        int center_threshold = TileSize / 2;
        if (absolute_direction == Vector3.forward)
        {
            // Up:
            if (onTile.Y <= center_threshold) return true;
        }
        else if (absolute_direction == Vector3.back)
        {
            if (onTile.Y >= center_threshold) return true;
        }
        else if (absolute_direction == Vector3.left)
        {
            if (onTile.X <= center_threshold) return true;
        }
        else if (absolute_direction == Vector3.right)
        {
            if (onTile.X >= center_threshold) return true;
        }
        return false;
    }

    private readonly Vector3[] compass = {Vector3.left, Vector3.right, Vector3.forward, Vector3.back, Vector3.up, Vector3.down};

    private Vector3 getAbsoluteDirection(Vector3 direction)
    {
        // Calculation, see: http://answers.unity3d.com/questions/617076/calculate-the-general-direction-of-a-vector.html
        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        foreach (var dir in compass)
        {
            var t = Vector3.Dot(direction, dir);
            if (t > maxDot)
            {
                ret = dir;
                maxDot = t;
            }
        }
        return ret;
    }

    // Use this for initialization
	void Start ()
	{
	    initField();
	    populateField();
	}

    private Tile[][] game_field;

    private void populateField()
    {
        int x = 0, y = 0;
        char current;
        using (StreamReader file = new StreamReader(Application.dataPath + "/Levels/default.txt"))
        {
            do
            {
                current = (char) file.Read();
                switch (current)
                {
                    case '▓':
                        setField(x, y, Tile.WALL);
                        break;
                    case '.':
                        setField(x, y, Tile.DOT);
                        break;
                    case '0':
                        setField(x, y, Tile.ENERGIZER);
                        break;
                    case ' ':
                        setField(x, y, Tile.FREE);
                        break;
                    case 't':
                        setField(x, y, Tile.TELEPORTER);
                        break;
                    case 'c':
                        setField(x, y, Tile.CAGE_DOOR);
                        break;
                    case '\r':
                        // Ignore this...
                        break;
                    case '\n':
                        y++;
                        x = -1; // Will be 0 when loop ends
                        break;
                    default:
                        Debug.LogError("Unrecognized Map Character: '"+current+"'");
                        break;
                }
                x++;
            } while (!file.EndOfStream);
        }
    }

    private void setField(int x, int y, Tile type)
    {
        this.game_field[y][x] = type;
    }

    private void initField()
    {
        this.game_field = new Tile[31][];
        for (int i = 0; i < game_field.Length; i++)
        {
            game_field[i] = new Tile[28];
        }
    }
}
