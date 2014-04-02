using System.Collections.Generic;
using Difficult_circumstances.Model;
using Difficult_circumstances.Model.Entity;
using Difficult_circumstances.Model.Entity.Creatures;
using Difficult_circumstances.Model.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Difficult_circumstances.View
{
    public static class View
    {
        public static double ScreenWidth;
        public static double ScreenHeight;

        private static readonly Dictionary<string, Texture2D> CachedTextures = new Dictionary<string, Texture2D>();

        public static FontRenderer FontRenderer;

        private static Texture2D GetTexture(Tile tile, GraphicsDevice device)
        {
            string name = "Tile:" + tile.Biome;

            if (!CachedTextures.ContainsKey(name))
            {
                Texture2D texture = new Texture2D(device, tile.Width, tile.Height);

                Color color;
                switch (tile.Biome)
                {
                    case Biome.Water:
                        color = Color.Blue;
                        break;

                    case Biome.Temperate:
                        color = Color.Green;
                        break;

                    case Biome.Desert:
                        color = Color.SandyBrown;
                        break;

                    case Biome.Forest:
                        color = Color.ForestGreen;
                        break;

                    default:
                        color = Color.White;
                        break;
                }

                Color[] texData = new Color[tile.Width * tile.Height];

                bool toggle = true;
                for (int i = 0; i < texData.Length; i++)
                {
                    if (i % tile.Width == 0 || i >= texData.Length - tile.Height)
                    {
                        if (toggle)
                        {
                            texData[i] = color;
                            toggle = false;
                        }
                        else
                        {
                            texData[i] = Color.Black;
                            toggle = true;
                        }
                    }
                    else
                    {
                        texData[i] = color;
                    }
                }

                texture.SetData(texData);

                CachedTextures.Add(name, texture);
            }

            return CachedTextures[name];
        }

        private static Texture2D GetTexture(Creature creature, GraphicsDevice device)
        {
            string name = "Entity:" + creature.GetType().Name;

            if (!CachedTextures.ContainsKey(name))
            {
                Texture2D texture = new Texture2D(device, creature.Width, creature.Height);

                Color color = Color.White;

                if (creature.GetType().Name == typeof(Magentaur).Name)
                    color = Color.Magenta;
                else if (creature.GetType().Name == typeof(Bluerior).Name)
                    color = Color.DarkBlue;
                else if (creature.GetType().Name == typeof(Player).Name)
                    color = Color.DarkRed;

                Color[] texData = new Color[creature.Width * creature.Height];

                bool toggle = true;
                for (int i = 0; i < texData.Length; i++)
                {
                    if (i % creature.Width == 0 || i >= texData.Length - creature.Height)
                    {
                        if (toggle)
                        {
                            texData[i] = color;
                            toggle = false;
                        }
                        else
                        {
                            texData[i] = Color.Black;
                            toggle = true;
                        }
                    }
                    else
                    {
                        texData[i] = color;
                    }
                }

                texture.SetData(texData);

                CachedTextures.Add(name, texture);
            }

            return CachedTextures[name];
        }

        private static Texture2D GetTexture(int width, int height, Color color, string name, GraphicsDevice device)
        {
            if (!CachedTextures.ContainsKey(name))
            {
                Texture2D texture = new Texture2D(device, width, height);

                Color[] texData = new Color[width * height];

                bool toggle = true;
                for (int i = 0; i < texData.Length; i++)
                {
                    texData[i] = color;
                }

                texture.SetData(texData);

                CachedTextures.Add(name, texture);
            }

            return CachedTextures[name];
        }

        public static void UpdateView(WorldModel worldModel, GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
        {
            device.Clear(Color.Black);
            spriteBatch.Begin();

            foreach (Tile tile in worldModel.Tiles)
            {
                Vector2 location = tile.GetLocation(worldModel.TileSize) - worldModel.ViewOffset;
                if (OnScreen(location, tile.Width, tile.Height))
                {
                    spriteBatch.Draw(GetTexture(tile, device), location, Color.White);

                    for (int i = 0; i < tile.TileContents.Count; i++)
                    {
                        EntityBase e = tile.TileContents[i];

                        Creature creature = e as Creature;
                        if (creature != null)
                        {
                            if (creature.Health > 0)
                            {
                                float offsetX = (worldModel.TileSize - creature.Width) / 2;
                                float offsetY = (worldModel.TileSize - creature.Height) / 2;
                                spriteBatch.Draw(GetTexture(creature, device), location + new Vector2(offsetX,offsetY)  , Color.White);
                                FontRenderer.DrawText(spriteBatch, (int)location.X, (int)location.Y, creature.CurrentHunger.ToString());
                            }
                            else
                            {
                                spriteBatch.Draw(GetTexture(creature.Height, creature.Width, Color.Black, "DEAD" + creature.Name, device), location, Color.White);
                            }
                        }

                        Grass g = e as Grass;
                        if (g != null)
                        {
                            spriteBatch.Draw(GetTexture(worldModel.TileSize, g.Lenght, Color.LimeGreen, "Grass" + g.Lenght, device), location, Color.White);
                        }
                    }
                }
            }

#if DEBUG
            //foreach (Tile tile in worldModel.Tiles)
            //{
            //    Vector2 location = tile.GetLocation(worldModel.TileSize) - worldModel.ViewOffset;
            //    if (OnScreen(location, tile.Width, tile.Height))
            //    {
            //        for (int i = 0; i < tile.TileContents.Count; i++)
            //        {
            //            EntityBase e = tile.TileContents[i];

            //            if (e is Creature)
            //            {
            //                Creature creature = e as Creature;
            //                // debug, draw vision of creture on tile
            //                if (creature.VisibleTiles != null)
            //                    foreach (var x in creature.VisibleTiles)
            //                    {
            //                        spriteBatch.Draw(GetTexture(10, 10, Color.DarkViolet, "Vision", device),
            //                            x.GetLocation(worldModel.TileSize) - worldModel.ViewOffset + new Vector2(5,5), Color.White);
            //                    }

            //                // debug, draw adjacent of creture on tile
            //                if (creature.AdjacentTiles != null)
            //                    foreach (var x in creature.AdjacentTiles)
            //                    {
            //                        spriteBatch.Draw(GetTexture(5, 5, Color.Blue, "Movement", device),
            //                            x.GetLocation(worldModel.TileSize) - worldModel.ViewOffset, Color.White);
            //                    }
            //            }
            //        }
            //    }
            //}

            FontRenderer.DrawText(spriteBatch, 150, 1, "Average Update Time: " + worldModel.UpdateInterval);

            FontRenderer.DrawText(spriteBatch, 1, 1, "HP: " + worldModel.Player.Health + "/" + worldModel.Player.MaxHealth);
            FontRenderer.DrawText(spriteBatch, 1, 25, "Hunger: " + worldModel.Player.CurrentHunger);
            FontRenderer.DrawText(spriteBatch, 1, 45, "Turn: " + worldModel.Turn);

#endif

            Vector2 menuLoc = new Vector2((worldModel.Player.CurrentTile.X * worldModel.TileSize) + (worldModel.TileSize / 2),
                                               (worldModel.Player.CurrentTile.Y * worldModel.TileSize) + (worldModel.TileSize / 2))
                                                - worldModel.ViewOffset;
            DrawMenu(worldModel.Menu, menuLoc, spriteBatch, device);

            spriteBatch.End();
        }

        public static void DrawMenu(Menu menu, Vector2 startLocation, SpriteBatch spriteBatch, GraphicsDevice device)
        {
            if (menu != null)
            {
                foreach (Menu m in menu.MenuOptions)
                {
                    int max = 0;
                    foreach (var option in menu.MenuOptions)
                    {
                        if (option.Name.Length > max)
                        {
                            max = option.Name.Length;
                        }
                    }

                    Texture2D texture = new Texture2D(device, (max * 14) + 25, 40 * menu.MenuOptions.Count);

                    Color color = Color.White;

                    Color[] texData = new Color[texture.Width * texture.Height];

                    for (int i = 0; i < texData.Length; i++)
                    {
                        texData[i] = color;
                    }

                    texture.SetData(texData);

                    spriteBatch.Draw(texture, startLocation, Color.White);
                    for (int i = 0; i < menu.MenuOptions.Count; i++)
                    {
                        if (i == menu.SelectedIndex)
                        {
                            spriteBatch.Draw(GetTexture(15, 15, Color.Black, "Pointer", device), startLocation + new Vector2(5, 10 + i * 40), Color.White);
                            DrawMenu(menu.MenuOptions[i], startLocation + new Vector2((max * 14) + 25, 10 + i * 40), spriteBatch, device);
                        }
                        FontRenderer.DrawText(spriteBatch, (int)startLocation.X + 25, (int)startLocation.Y + i * 40 + 10, menu.MenuOptions[i].Name);
                    }
                }
            }
        }

        private static bool OnScreen(Vector2 location, int width, int height)
        {
            if ((location.X + width < 0 || location.X - width > ScreenWidth) || (location.Y + height < 0 || location.Y - height > ScreenHeight))
            {
                return false;
            }
            return true;
        }
    }
}