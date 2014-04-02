using System;
using System.Collections.Generic;
using System.Linq;
using Difficult_circumstances.Model.Entity.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entity.Creatures
{
    internal class Magentaur : Creature
    {
        public readonly List<Tile> Memory = new List<Tile>();

        public Tile TargetTile;

        public override void Update()
        {
            if (CurrentHunger >= 5)
            {
                if (Memory.Count >= 5)
                {
                    Memory.RemoveAt(0);
                }

                Memory.Add(CurrentTile);

                if (TargetTile == null)
                {
                    // search for a tile with food on it
                    Tile closest = null;
                    int distance = int.MaxValue;
                    foreach (Tile t in VisibleTiles)
                    {
                        var gr = t.TileContents.Where(g => g is Grass && (g as Grass).Lenght > 5).ToList();
                        if (gr.Count > 0)
                        {
                            if (closest == null)
                            {
                                closest = t;
                                distance = Math.Abs(CurrentTile.X - t.X) + Math.Abs(CurrentTile.Y - t.Y);
                            }
                            else
                            {
                                int newDistance = Math.Abs(CurrentTile.X - t.X) + Math.Abs(CurrentTile.Y - t.Y);
                                if (newDistance < distance)
                                {
                                    closest = t;
                                    distance = newDistance;
                                }
                            }
                        }
                    }
                    if (closest != null)
                        TargetTile = closest;
                }

                if (TargetTile == null)
                {
                    // move around randomly but do not repeat movements for the last 5 turns
                    Amble();
                }
                else
                {
                    int x = CurrentTile.X - TargetTile.X;
                    int y = CurrentTile.Y - TargetTile.Y;

                    if (x > 0)
                        x = CurrentTile.X - 1;

                    if (x < 0)
                        x = CurrentTile.X + 1;

                    if (y > 0)
                        y = CurrentTile.Y - 1;

                    if (y < 0)
                        y = CurrentTile.Y + 1;

                    if (x == 0 && TargetTile.X != 0)
                        x = CurrentTile.X;

                    if (y == 0 && TargetTile.Y != 0)
                        y = CurrentTile.Y;

                    if (x != CurrentTile.X || y != CurrentTile.Y)
                    {
                        Move(AdjacentTiles.First(tile => tile.X == x && tile.Y == y));
                    }
                }
            }
            else
            {
                if (new Random().Next(1, 10) > 5)
                {
                    Amble();
                }

            }

            if (CurrentHunger > -10)
            {
                foreach (EntityBase e in CurrentTile.TileContents)
                {
                    if (e is Grass)
                    {
                        Grass g = e as Grass;

                        CurrentHunger -= g.GetEaten();
                    }
                }
            }

        }

        public void Amble()
        {
            // move around to find a tile with food on it
            int counter = 0;
            Tile t = AdjacentTiles[MathHelper.Random.Next(0, AdjacentTiles.Count)];
            while (t.Biome == Biome.Water || Memory.Contains(t))
            {
                t = AdjacentTiles[MathHelper.Random.Next(0, AdjacentTiles.Count)];
                counter++;
                if (counter > 8)
                    return;
            }

            Move(t);
        }

        public Magentaur()
        {
            Width = 25;
            Height = 25;
            VisionRadius = 3;

            Health = MaxHealth = 30;
            HungerRate = 2;
            CurrentHunger = 5;
            DesiredFood = Food.Grass;
            ProvidesFoodType = Food.Meat;
            NutritionalValue = 100;
        }
    }
}