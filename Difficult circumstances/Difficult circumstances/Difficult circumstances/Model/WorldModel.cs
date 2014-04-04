using System;
using System.Collections.Generic;
using Difficult_circumstances.Model.Entities.Fauna;
using Difficult_circumstances.Model.Entities.Flora;
using Difficult_circumstances.Model.Entities.Objects;
using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;
using Microsoft.Xna.Framework;

namespace Difficult_circumstances.Model
{
    public class WorldModel
    {
        public readonly Tile[,] Tiles = new Tile[TilesHeight, TilesWidth];
        public readonly short TileSize;
        public readonly Player Player;
        public Vector2 ViewOffset;
        private const int TilesHeight = 200;
        private const int TilesWidth = 200;

        public WorldModel(short tileSize)
        {
            TileSize = tileSize;

            int biomes = Enum.GetNames(typeof(Biome)).Length;
            for (short x = 0; x < TilesHeight; x++)
            {
                for (short y = 0; y < TilesWidth; y++)
                {
                    Tiles[x, y] = new Tile(x, y, (Biome)MathHelper.Random.Next(0, biomes), TileSize, TileSize);

                    if (Tiles[x, y].Biome == Biome.Temperate && MathHelper.Random.Next(1, 100) > 75)
                    {
                        Tiles[x, y].AddContent(new Grass());
                    }

                    if (Tiles[x, y].Biome == Biome.Forest && MathHelper.Random.Next(1, 100) > 95)
                    {
                        Tiles[x, y].AddContent(new Tree());
                    }

                    if (MathHelper.Random.Next(1, 100) > 80)
                    {
                        Tiles[x, y].AddContent(new Rock());
                    }
                }
            }

            for (short i = 0; i < 1000; i++)
            {
                Tiles[MathHelper.Random.Next(1, TilesWidth - 1), MathHelper.Random.Next(1, TilesHeight - 1)].AddContent(new Magentaur());
            }

            Player = new Player();

            Tiles[MathHelper.Random.Next(1, TilesWidth - 1), MathHelper.Random.Next(1, TilesHeight - 1)].AddContent(Player);
            Tiles[Player.CurrentTile.X, Player.CurrentTile.Y - 1].AddContent(new Bluerior());
            Tiles[Player.CurrentTile.X, Player.CurrentTile.Y - 1].AddContent(new Bluerior());
            Tiles[Player.CurrentTile.X, Player.CurrentTile.Y - 1].AddContent(new Bluerior());

            ViewOffset = new Vector2(Player.CurrentTile.X * TileSize - 8 * tileSize, Player.CurrentTile.Y * TileSize - 8 * TileSize);
        }

        public Menu Menu { get; set; }

        public Menu ActiveMenu { get; set; }

        public double UpdateInterval { get; set; }

        public void AddTile(List<Tile> tiles, Tile tile)
        {
            if (tile == null)
                return;

            if (!tiles.Contains(tile))
                tiles.Add(tile);
        }

        public List<Tile> GetAdjacentTiles(short tileX, short tileY, IMobile mobile)
        {
            List<Tile> adjacentTiles = new List<Tile>();

            AddTile(adjacentTiles, GetTile(tileX - 1, tileY));
            AddTile(adjacentTiles, GetTile(tileX + 1, tileY));
            AddTile(adjacentTiles, GetTile(tileX, tileY - 1));
            AddTile(adjacentTiles, GetTile(tileX, tileY + 1));

            AddTile(adjacentTiles, GetTile(tileX - 1, tileY - 1));
            AddTile(adjacentTiles, GetTile(tileX + 1, tileY + 1));
            AddTile(adjacentTiles, GetTile(tileX + 1, tileY - 1));
            AddTile(adjacentTiles, GetTile(tileX - 1, tileY + 1));

            return adjacentTiles;
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x > TilesWidth - 1 || y > TilesHeight - 1)
                return null;

            return Tiles[x, y];
        }

        internal List<Tile> GetVisibleTiles(short tileX, short tileY, ISighted sighted)
        {
            List<Tile> visibleTiles = new List<Tile>();

            for (int i = sighted.VisionRadius; i > 0; i--)
            {
                AddTile(visibleTiles, GetTile(tileX - i, tileY));
                AddTile(visibleTiles, GetTile(tileX + i, tileY));
                AddTile(visibleTiles, GetTile(tileX, tileY - i));
                AddTile(visibleTiles, GetTile(tileX, tileY + i));

                if (i != sighted.VisionRadius)
                {
                    for (int s = 1; s <= i; s++)
                    {
                        AddTile(visibleTiles, GetTile(tileX - i, tileY - s));
                        AddTile(visibleTiles, GetTile(tileX + i, tileY - s));
                        AddTile(visibleTiles, GetTile(tileX - s, tileY - i));
                        AddTile(visibleTiles, GetTile(tileX - s, tileY + i));
                        AddTile(visibleTiles, GetTile(tileX - i, tileY + s));
                        AddTile(visibleTiles, GetTile(tileX + i, tileY + s));
                        AddTile(visibleTiles, GetTile(tileX + s, tileY - i));
                        AddTile(visibleTiles, GetTile(tileX + s, tileY + i));
                    }
                }

                visibleTiles.Remove(GetTile(tileX + sighted.VisionRadius - 1, tileY + sighted.VisionRadius - 1));
                visibleTiles.Remove(GetTile(tileX - sighted.VisionRadius + 1, tileY - sighted.VisionRadius + 1));
                visibleTiles.Remove(GetTile(tileX - sighted.VisionRadius + 1, tileY + sighted.VisionRadius - 1));
                visibleTiles.Remove(GetTile(tileX + sighted.VisionRadius - 1, tileY - sighted.VisionRadius + 1));
            }

            return visibleTiles;
        }

        public int Turn { get; set; }
    }
}