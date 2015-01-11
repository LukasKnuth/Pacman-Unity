using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pacman.Map
{
    public class GameField : MonoBehaviour
    {
        // ------------ INSPECTOR INTERFACE ---------------
        public int TileSize;
        public GameObject DotPrefab;
        public GameObject EnergizerPrefab;

        // ------------- PUBLIC SCRIPTING INTERFACE ----------------
        [Flags]
        public enum TileType
        {
            Free = 1, Wall = 2, Dot = 4, Energizer = 8, Teleporter = 16, CageDoor = 32
        }

        public Tile GetTileAt(Vector3 position)
        {
            return new Tile(this, position);
        }

        public Tile GetRandomTile()
        {
            int col = Random.Range(0, MAP_WIDTH - 1);
            int row = Random.Range(0, MAP_HEIGHT - 1);
            return new Tile(this, row, col);
        }

        // ----------------- PRIVATE INTERFACE -------------------------
        internal const int MAP_HEIGHT = 31;
        internal const int MAP_WIDTH = 28;

        private readonly Vector3[] _compass = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back, Vector3.up, Vector3.down };

        internal Vector3 GetAbsoluteDirection(Vector3 direction)
        {
            // Calculation, see: http://answers.unity3d.com/questions/617076/calculate-the-general-direction-of-a-vector.html
            var maxDot = -Mathf.Infinity;
            var ret = Vector3.zero;

            foreach (var dir in _compass)
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
        void Start()
        {
            InitField();
            PopulateField();
        }

        internal TileType[][] Map;

        private void PopulateField()
        {
            int x = 0, y = 0;
            char current;
            using (StreamReader file = new StreamReader(Application.dataPath + "/Levels/default.txt"))
            {
                do
                {
                    current = (char)file.Read();
                    switch (current)
                    {
                        case '▓':
                            SetField(x, y, TileType.Wall);
                            break;
                        case '.':
                            SetField(x, y, TileType.Dot);
                            // Instanciate a new (eatable) Dot-prefab:
                            Vector3 dot_position = new Vector3(
                                x * TileSize - (TileSize / 2),
                                2,
                                -(y * TileSize - (TileSize / 2))
                            );
                            Instantiate(DotPrefab, dot_position, Quaternion.identity);
                            break;
                        case '0':
                            SetField(x, y, TileType.Energizer);
                            Vector3 energizer_position = new Vector3(
                                x * TileSize - (TileSize / 2),
                                2,
                                -(y * TileSize - (TileSize / 2))
                            );
                            Instantiate(EnergizerPrefab, energizer_position, Quaternion.identity);
                            break;
                        case ' ':
                            SetField(x, y, TileType.Free);
                            break;
                        case 't':
                            SetField(x, y, TileType.Teleporter);
                            break;
                        case 'c':
                            SetField(x, y, TileType.CageDoor);
                            break;
                        case '\r':
                            // Ignore this...
                            break;
                        case '\n':
                            y++;
                            x = -1; // Will be 0 when loop ends
                            break;
                        default:
                            Debug.LogError("Unrecognized Map Character: '" + current + "'");
                            break;
                    }
                    x++;
                } while (!file.EndOfStream);
            }
        }

        private void SetField(int x, int y, TileType type)
        {
            this.Map[y][x] = type;
        }

        private void InitField()
        {
            this.Map = new TileType[MAP_HEIGHT][];
            for (int i = 0; i < Map.Length; i++)
            {
                Map[i] = new TileType[MAP_WIDTH];
            }
        }
    }
}