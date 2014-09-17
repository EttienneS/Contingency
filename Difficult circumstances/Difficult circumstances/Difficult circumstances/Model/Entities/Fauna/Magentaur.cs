using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Difficult_circumstances.Model.Entities.Fauna
{
    internal class Magentaur : Creature
    {
        private readonly List<Tile> _memory = new List<Tile>();

        private Tile _foodSource;
        private Tile _targetTile;

        public Magentaur()
        {
            Width = 15;
            Height = 15;
            VisionRadius = 3;

            Health = MaxHealth = 30;
            HungerRate = 2;
            CurrentHunger = 5;
            DesiredFood = Food.Meat;
            ProvidesFoodType = Food.Meat;
            NutritionalValue = 100;
            Damage = 2;
        }

        private void Amble()
        {
            // move around to find a tile with food on it
            int counter = 0;
            Tile t = AdjacentTiles[MathHelper.Random.Next(0, AdjacentTiles.Count)];
            while (!t.Passable(this) || _memory.Contains(t))
            {
                t = AdjacentTiles[MathHelper.Random.Next(0, AdjacentTiles.Count)];
                counter++;
                if (counter > 8)
                    return;
            }

            Move(t);
        }

        public override void Update()
        {
            if (_memory.Count >= 5)
            {
                _memory.RemoveAt(0);
            }

            _memory.Add(CurrentTile);

            VisibleTiles.Add(CurrentTile);

            LookForFood();

            LookForDanger();

            if (_targetTile == null && CurrentHunger > 5 && _foodSource != null)
            {
                // is hungry and has a known food source with no other targets
                _targetTile = _foodSource;
            }

            MoveToTarget();

            TryToEat();
        }

        private void LookForDanger()
        {
            Tile dangerTile = null;
            foreach (Tile t in VisibleTiles.Where(t => t.TileContents.Count > 0))
            {
                foreach (IEntity e in t.TileContents.Where(f => f is IFeeder))
                {
                    IAnimate animate = e as IAnimate;
                    IFeeder feeder = e as IFeeder;
                    if (feeder.DesiredFood.HasFlag(ProvidesFoodType) && animate != null && animate.Damage > Damage)
                    {
                        // this thing eats and what it eats is me
                        dangerTile = t;
                        break;
                    }
                }
            }

            if (dangerTile != null)
            {
                // run away this thing potentially wants to eat me
                int maxDist = 0;
                foreach (Tile visTile in VisibleTiles)
                {
                    if (visTile == dangerTile) continue;

                    int tileDistance = Math.Abs((visTile.X - CurrentTile.X)) + Math.Abs((visTile.Y - CurrentTile.Y));
                    if (tileDistance > maxDist)
                    {
                        maxDist = tileDistance;
                        _targetTile = visTile;
                    }
                }
            }

            if (_foodSource != null && _targetTile != _foodSource)
            {
                // no danger, amble around
                _targetTile = null;
            }
        }

        private void LookForFood()
        {
            if (_foodSource == null || CurrentHunger > 20)
            {
                // search for a tile with food on it
                Tile closest = null;
                int distance = int.MaxValue;
                foreach (Tile t in VisibleTiles.Where(t => t.TileContents.Count > 0))
                {
                    foreach (IEntity e in t.TileContents)
                    {
                        if (e == this || e.GetType() == GetType())
                        {
                            continue;
                        }

                        IEdible edible = e as IEdible;

                        if (edible == null)
                        {
                            continue;
                        }

                        if (edible.ProvidesFoodType != 0 && DesiredFood.HasFlag(edible.ProvidesFoodType))
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
                }
                if (closest != null)
                    _foodSource = closest;
            }
        }

        private void MoveToTarget()
        {
            if (_targetTile != null)
            {
                int x = CurrentTile.X - _targetTile.X;
                int y = CurrentTile.Y - _targetTile.Y;

                if (x > 0)
                    x = CurrentTile.X - 1;

                if (x < 0)
                    x = CurrentTile.X + 1;

                if (y > 0)
                    y = CurrentTile.Y - 1;

                if (y < 0)
                    y = CurrentTile.Y + 1;

                if (x == 0 && _targetTile.X != 0)
                    x = CurrentTile.X;

                if (y == 0 && _targetTile.Y != 0)
                    y = CurrentTile.Y;

                if (x != CurrentTile.X || y != CurrentTile.Y)
                {
                    Tile target = AdjacentTiles.FirstOrDefault(tile => tile.X == x && tile.Y == y);

                    if (target != null && target.Passable(this))
                    {
                        Move(target);
                    }
                    else
                    {
                        target = null;
                        int max = 0;
                        foreach (Tile t in AdjacentTiles.Where(tile => tile.X == x || tile.Y == y))
                        {
                            if (t.Passable(this))
                            {
                                int distance = Math.Abs(CurrentTile.X - _targetTile.X) + Math.Abs(CurrentTile.Y - _targetTile.Y);
                                if (distance > max)
                                {
                                    max = distance;
                                    target = t;
                                }
                            }
                        }

                        if (target != null)
                        {
                            Move(target);
                        }
                        else
                        {
                            Amble();
                        }
                    }
                }
            }
            else
            {
                Amble();
            }
        }

        private void TryToEat()
        {
            if (CurrentHunger > -10)
            {
                for (int i = 0; i < CurrentTile.TileContents.Count; i++)
                {
                    IEntity e = CurrentTile.TileContents[i];

                    IEdible edible = e as IEdible;

                    if (edible != null && edible.ProvidesFoodType != 0 && DesiredFood.HasFlag(edible.ProvidesFoodType))
                    {
                        IAnimate animate = e as IAnimate;
                        if (animate != null)
                        {
                            if (animate.Health > 0)
                            {
                                // attack
                                animate.Health -= Damage;
                                Health -= animate.Damage;
                            }
                            else
                            {
                                CurrentHunger -= edible.GetEaten();
                            }
                        }
                    }
                }
            }
            else
            {
                // full
                if (_targetTile == _foodSource)
                    _targetTile = null;
            }
        }
    }
}