using System.Collections.Generic;
using Difficult_circumstances.Model.Entity;
using Microsoft.Xna.Framework;

namespace Difficult_circumstances.Model.Map
{
    public enum Biome
    {
        Temperate, Water, Desert, Forest
    }

    public class Tile
    {
        public Tile(short x, short y, Biome biome, short width, short height)
        {
            X = x;
            Y = y;
            Biome = biome;

            Width = width;
            Height = height;
            TileContents = new List<EntityBase>();
        }

        public Biome Biome { get; private set; }

        public short Height { get; private set; }

        public short Width { get; private set; }

        public short X { get; set; }

        public short Y { get; set; }

        public List<EntityBase> TileContents { get; private set; }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }

        internal Vector2 GetLocation(short tileSize)
        {
            return new Vector2(X * tileSize, Y * tileSize);
        }

        internal void Move(EntityBase entity, Tile tile)
        {
            tile.AddContent(entity);
            TileContents.Remove(entity);
        }

        public void AddContent(EntityBase entity)
        {
            entity.CurrentTile = this;
            TileContents.Add(entity);
        }
    }
}