using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Difficult_circumstances.Model;
using Difficult_circumstances.Model.Entities;
using Difficult_circumstances.Model.Entities.Fauna;
using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Difficult_circumstances.Controller
{
    internal static class Controller
    {
        // only force update every 1000 ms
        private const double Interval = 1000;

        private const int ScrollSpeed = 15;

        private static readonly Stopwatch DiagStopwatch = new Stopwatch();

        // for debug purposes to check performance on update loop
        private static readonly List<TimeSpan> Spans = new List<TimeSpan>();

        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _lastKeyboardState;

        public static bool KeyPressed(Keys key)
        {
            if (!_lastKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyDown(key))
            {
                return true;
            }

            return false;
        }

        internal static bool ParseInput(WorldModel worldModel)
        {
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            bool turnPassed = false;

            if (_currentKeyboardState.IsKeyDown(Keys.Down))
            {
                worldModel.ViewOffset += new Vector2(0, ScrollSpeed);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Up))
            {
                worldModel.ViewOffset -= new Vector2(0, ScrollSpeed);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Left))
            {
                worldModel.ViewOffset -= new Vector2(ScrollSpeed, 0);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Right))
            {
                worldModel.ViewOffset += new Vector2(ScrollSpeed, 0);
            }

            if (KeyPressed(Keys.Tab))
            {
                worldModel.ViewOffset = new Vector2(worldModel.Player.CurrentTile.X * worldModel.TileSize - 8 * worldModel.TileSize, worldModel.Player.CurrentTile.Y * worldModel.TileSize - 8 * worldModel.TileSize);
            }

            if (worldModel.Menu == null)
            {
                if (KeyPressed(Keys.F))
                {
                    worldModel.Menu = GetAllActions(worldModel);
                }
                else
                {
                    if (KeyPressed(Keys.A) || KeyPressed(Keys.S)
                        || KeyPressed(Keys.D) || KeyPressed(Keys.W))
                    {
                        int x = worldModel.Player.CurrentTile.X;
                        int y = worldModel.Player.CurrentTile.Y;

                        if (KeyPressed(Keys.A))
                            x -= 1;

                        if (KeyPressed(Keys.S))
                            y += 1;

                        if (KeyPressed(Keys.D))
                            x += 1;

                        if (KeyPressed(Keys.W))
                            y -= 1;

                        if (x > 0 || x < worldModel.Tiles.Length - 1 || y > 0 || y < worldModel.Tiles.Length - 1)
                        {
                            turnPassed = true;

                            Tile target = worldModel.Tiles[x, y];
                            worldModel.Player.Move(target);
                        }
                    }
                }
            }
            else
            {
                if (KeyPressed(Keys.Enter))
                {
                    worldModel.Menu = null;
                    worldModel.ActiveMenu = null;
                }
                else
                {
                    if (KeyPressed(Keys.F))
                    {
                        Menu selectedOption = worldModel.ActiveMenu.MenuOptions[worldModel.Menu.SelectedIndex];

                        if (selectedOption.MenuOptions.Count == 0)
                        {
                            selectedOption.Action.Invoke(selectedOption.Context);
                        }
                        else
                        {
                            if (selectedOption.Action == null)
                            {
                                selectedOption.Selected.Action.Invoke(selectedOption.Selected.Context);
                            }
                            else
                            {
                                selectedOption.Action.Invoke(selectedOption.Context);
                            }
                        }

                        worldModel.Menu = null;
                        turnPassed = true;
                    }

                    if (KeyPressed(Keys.W))
                        worldModel.ActiveMenu.SelectedIndex -= 1;

                    if (KeyPressed(Keys.S))
                        worldModel.ActiveMenu.SelectedIndex += 1;

                    if (KeyPressed(Keys.D))
                    {
                        if (worldModel.ActiveMenu.MenuOptions.Count > 1)
                        {
                            worldModel.ActiveMenu = worldModel.Menu.Selected;
                        }
                    }

                    if (KeyPressed(Keys.A))
                    {
                        if (worldModel.ActiveMenu.Parent != null)
                        {
                            worldModel.ActiveMenu = worldModel.ActiveMenu.Parent;
                        }
                    }
                }
            }

            return turnPassed;
        }

        private static Menu GetAllActions(WorldModel worldModel)
        {
            Tile current = worldModel.Player.CurrentTile;

            Menu menu = new Menu();

            Menu atackOption = new Menu(menu, "Attack", null);
            Menu eatOption = new Menu(menu, "Eat", null);
            Menu pickUpOption = new Menu(menu, "Pick Up", null);

            List<IEntity> entities = current.TileContents.Where(f => !(f is Player)).ToList();
            entities.AddRange(worldModel.Player.Inventory);
            foreach (IEntity entity in entities)
            {
                if (entity is IEdible)
                {
                    if (entity is IAnimate)
                    {
                        // if the entity is animate it can fight back, so kill it before eating it
                        IAnimate i = entity as IAnimate;

                        if (i.Health > 0)
                        {
                            atackOption.AddOption(entity.Name, worldModel.Player.Attack, entity);
                        }
                        else
                        {
                            // entity is dead, can be eaten
                            CheckIfCanBeEaten(worldModel, entity, eatOption);
                        }
                    }
                    else
                    {
                        // entity is not animate so it cannot attack, eat it without struggle
                        CheckIfCanBeEaten(worldModel, entity, eatOption);
                    }
                }

                if (entity is IItem)
                {
                    IItem item = entity as IItem;
                    if (!worldModel.Player.Inventory.Contains(entity) && item.CanLoot())
                    {
                        pickUpOption.AddOption(entity.Name, worldModel.Player.PickUp, entity);
                    }
                }
            }


            if (atackOption.MenuOptions.Count > 0)
                menu.MenuOptions.Add(atackOption);

            if (eatOption.MenuOptions.Count > 0)
                menu.MenuOptions.Add(eatOption);

            if (pickUpOption.MenuOptions.Count > 0)
                menu.MenuOptions.Add(pickUpOption);

            menu.MenuOptions.Add(new Menu(menu, "Stay", worldModel.Player.Stay));

            worldModel.ActiveMenu = menu;

            return menu;
        }

        private static void CheckIfCanBeEaten(WorldModel worldModel, IEntity entity, Menu eatOption)
        {
            IEdible e = entity as IEdible;
            if (e != null && worldModel.Player.DesiredFood.HasFlag(e.ProvidesFoodType) && e.CanBeEaten())
            {
                eatOption.AddOption(e.FoodName, worldModel.Player.Eat, entity);
            }
        }

        internal static void Update(WorldModel worldModel, GameTime gameTime)
        {
            DiagStopwatch.Reset();
            DiagStopwatch.Start();

            List<IEntity> processedEntities = new List<IEntity>();

            foreach (Tile tile in worldModel.Tiles)
            {
                UpdateTile(tile, processedEntities, worldModel);
            }

            DiagStopwatch.Stop();
            Spans.Add(DiagStopwatch.Elapsed);

            worldModel.UpdateInterval = GetAverageUpdateTime().TotalMilliseconds;

            worldModel.Turn++;
        }

        private static TimeSpan GetAverageUpdateTime()
        {
            double f = 0;
            foreach (var x in Spans)
            {
                f += x.TotalMilliseconds;
            }

            return TimeSpan.FromMilliseconds(f / Spans.Count);
        }

        private static void UpdateTile(Tile tile, List<IEntity> processedEntities, WorldModel worldModel)
        {
            for (int i = 0; i < tile.TileContents.Count; i++)
            {
                LivingEntity e = tile.TileContents[i] as LivingEntity;

                if (e != null)
                {
                    if (!processedEntities.Contains(e))
                    {
                        IAnimate animate = e as IAnimate;
                        if (animate != null)
                        {
                            if (animate.Health <= 0)
                            {
                                // unit is dead
                                continue;
                            }
                        }

                        ISighted sighted = e as ISighted;
                        if (sighted != null)
                        {
                            sighted.VisibleTiles = worldModel.GetVisibleTiles(tile.X, tile.Y, sighted);
                        }

                        IMobile mobile = e as IMobile;
                        if (mobile != null)
                        {
                            mobile.AdjacentTiles = worldModel.GetAdjacentTiles(tile.X, tile.Y, mobile);
                        }

                        IFeeder feeder = e as IFeeder;
                        if (feeder != null)
                        {
                            feeder.CurrentHunger += feeder.HungerRate;

                            ILiving living = e as ILiving;
                            if (feeder.CurrentHunger > 50 && living != null)
                            {
                                living.Health -= feeder.HungerRate;
                            }
                        }

                        e.Update();

                        // entity moved or died
                        if (e.CurrentTile != tile)
                        {
                            i--;
                        }

                        processedEntities.Add(e);
                    }
                }
            }
        }

    }
}