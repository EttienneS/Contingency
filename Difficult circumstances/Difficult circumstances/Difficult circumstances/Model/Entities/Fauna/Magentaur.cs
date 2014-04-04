using System;
using System.Collections.Generic;
using System.Linq;
using Difficult_circumstances.Model.Entities.Flora;
using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Fauna
{
    internal class Magentaur : Creature
    {
        private readonly List<Tile> _memory = new List<Tile>();

        private Tile _foodSource;
        private Tile _targetTile;

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

        private void Amble()
        {
            // move around to find a tile with food on it
            int counter = 0;
            Tile t = AdjacentTiles[MathHelper.Random.Next(0, AdjacentTiles.Count)];
            while (t.Biome == Biome.Water || _memory.Contains(t))
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
                    IFeeder feeder = e as IFeeder;
                    if (feeder.DesiredFood.HasFlag(ProvidesFoodType))
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
                foreach (Tile t in from t in VisibleTiles where t.TileContents.Count > 0 from e in t.TileContents where e is Grass select t)
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
                    Move(AdjacentTiles.First(tile => tile.X == x && tile.Y == y));
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
                foreach (IEntity e in CurrentTile.TileContents)
                {
                    if (e is Grass)
                    {
                        Grass g = e as Grass;
                        CurrentHunger -= g.GetEaten();
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