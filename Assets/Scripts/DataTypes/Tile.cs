using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Pacman.Map
{
    /// <summary>
    /// The Tile-class simplyfies access to the <see cref="GameField"/>.
    /// </summary>
    public class Tile
    {
        public GameField.TileType Type { get; private set; }
        public Vector3 Position { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        private readonly GameField _map;

        internal Tile(GameField map, Vector3 player_position)
        {
            this._map = map;
            int col = (int)(Math.Abs(player_position.x) / map.TileSize) + 1;
            int row = (int)(Math.Abs(player_position.z) / map.TileSize) + 1;
            // Set the rest:
            this.Init(row, col);
        }

        internal Tile(GameField map, int row, int column)
        {
            this._map = map;
            this.Init(row, column);
        }

        private void Init(int row, int col) {
            this.Row = row;
            this.Column = col;
            this.Type = this._map.Map[row][col];
            this.Position = new Vector3(
                col * this._map.TileSize + (this._map.TileSize / 2),
                0,
                -(row * this._map.TileSize + (this._map.TileSize / 2))
            );
        }

        public Tile Left(int tileCount = 1)
        {
            return new Tile(this._map,
                this.Row, Math.Max(0, this.Column - tileCount)
            );
        }

        public Tile Right(int tileCount = 1)
        {
            return new Tile(this._map,
                this.Row, Math.Min(GameField.MAP_WIDTH - 1, this.Column + tileCount)
            );
        }

        public Tile Up(int tileCount = 1)
        {
            return new Tile(this._map,
                Math.Max(0, this.Row - tileCount), this.Column
            );
        }

        public Tile Down(int tileCount = 1)
        {
            return new Tile(this._map,
                Math.Min(GameField.MAP_HEIGHT - 1, this.Row + tileCount), this.Column
            );
        }

        public Tile InDirection(Vector3 direction, int tileCount = 1) {
            Vector3 absoluteDirection = this._map.GetAbsoluteDirection(direction);
            if (absoluteDirection == Vector3.left)
            {
                return this.Left(tileCount);
            }
            else if (absoluteDirection == Vector3.right)
            {
                return this.Right(tileCount);
            }
            else if (absoluteDirection == Vector3.forward)
            {
                return this.Up(tileCount);
            }
            else if (absoluteDirection == Vector3.back)
            {
                return this.Down(tileCount);
            }
            else if (absoluteDirection == Vector3.zero) {
                return this;
            }
            else
            {
                Debug.LogError("Given direction was not recognized: "+absoluteDirection);
                return null;
            }
        }

        /// <summary>
        /// Whether or not the given <see cref="position"/> is at the center of this Tile.
        /// </summary>
        public bool IsCenteredOnTile(Vector3 position, Vector3 direction)
        {
            int x = (int)(Math.Abs(position.x) % this._map.TileSize);
            int y = (int)(Math.Abs(position.z) % this._map.TileSize);
            Vector3 absoluteDirection = this._map.GetAbsoluteDirection(direction);
            int centerThreshold = this._map.TileSize / 2;
            if (absoluteDirection == Vector3.forward)
            {
                if (y <= centerThreshold) return true;
            }
            else if (absoluteDirection == Vector3.back)
            {
                if (y >= centerThreshold) return true;
            }
            else if (absoluteDirection == Vector3.left)
            {
                if (x <= centerThreshold) return true;
            }
            else if (absoluteDirection == Vector3.right)
            {
                if (x >= centerThreshold) return true;
            }
            return false;
        }

        /// <summary>
        /// Return all Tiles around this one, mapped to their corresponding directions as <see cref="Vector3"/>
        /// </summary>
        public IEnumerable<KeyValuePair<Vector3, Tile>> Around()
        {
            yield return new KeyValuePair<Vector3, Tile>(Vector3.left, this.Left());
            yield return new KeyValuePair<Vector3, Tile>(Vector3.right, this.Right());
            yield return new KeyValuePair<Vector3, Tile>(Vector3.forward, this.Up());
            yield return new KeyValuePair<Vector3, Tile>(Vector3.back, this.Down());
        }

        protected bool Equals(Tile other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tile)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Column;
            }
        }

        public static bool operator ==(Tile left, Tile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Row: {1}, Column: {2}", Type, Row, Column);
        }
    }

    /// <summary>
    /// Some helpers to make working with <see cref="Vector3"/> and <see cref="Tile"/> easier.
    /// </summary>
    public static class VectorTileExtension {

        public static bool IsCenteredOn(this Vector3 position, Tile tile, Vector3 direction) {
            return tile.IsCenteredOnTile(position, direction);
        }
    }

}
