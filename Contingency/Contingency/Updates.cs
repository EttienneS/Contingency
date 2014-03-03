using System;
using System.Collections.Generic;
using Contingency.Units;
using Microsoft.Xna.Framework;

namespace Contingency
{
    public partial class Game1 : Game
    {
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CatchInputs();

            if (!_inPlanningMode)
            {
                if (_remainingPlayTime <= 0)
                {
                    _inPlanningMode = true;
                    _remainingPlayTime = 0;
                }
                else
                {
                    _remainingPlayTime -= gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            if (!_menuMode)
            {
                if (_inPlanningMode)
                    return;

                UpdateExplosions(gameTime);

                UpdateUnits(gameTime);

                UpdateProjectiles();

                UpdateDeaths();
            }
        }

        private  Unit GetSelctedUnit()
        {
            foreach (Unit unit in _gameState.Units)
            {
                if (unit.Selected)
                {
                    return unit;
                }
            }

            return null;
        }

        private  void UpdateDeaths()
        {
            for (int i = 0; i < _gameState.Units.Count; i++)
            {
                if (_gameState.Units[i].CurrentHP <= 0)
                {
                    _gameState.Explosions.Add(new Explosion(new Vector2(_gameState.Units[i].Location.X - _gameState.Units[i].Width / 2, _gameState.Units[i].Location.Y - _gameState.Units[i].Height / 2)));
                    _gameState.Units.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < _gameState.Blocks.Count; i++)
            {
                if (_gameState.Blocks[i].CurrentHP <= 0)
                {
                    _gameState.Explosions.Add(new Explosion(new Vector2(_gameState.Blocks[i].Location.X - _gameState.Blocks[i].Width / 2, _gameState.Blocks[i].Location.Y - _gameState.Blocks[i].Height / 2)));
                    _gameState.Blocks.RemoveAt(i);
                    i--;
                }
            }
        }

        private  void UpdateExplosions(GameTime gameTime)
        {
            for (int x = 0; x < _gameState.Explosions.Count; x++)
            {
                Explosion exp = _gameState.Explosions[x];
                if (exp.Done)
                {
                    _gameState.Explosions.Remove(exp);
                    x--;
                }
                else
                {
                    exp.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                }
            }
        }

        private void UpdateProjectiles()
        {
            List<Projectile> killSprites = new List<Projectile>();

            foreach (Projectile p in _gameState.Projectiles)
            {
                if (p.Location.X < 0 || p.Location.X > _map.Width || p.Location.Y > _map.Height || p.Location.Y < 0 || (p.Momentum.X == 0 && p.Momentum.Y == 0))
                {
                    killSprites.Add(p);
                    break;
                }
                p.UpdateState();

                bool hitUnit = false;
                foreach (Unit u in _gameState.Units)
                {
                    if (u.Touches(p) && p.Owner != u)
                    {
                        u.Hit(p);
                        hitUnit = true;
                        break;
                    }
                }

                if (!hitUnit)
                {
                    foreach (Block b in _gameState.Blocks)
                    {
                        if (b.Touches(p) && p.Owner != b.Owner)
                        {
                            b.Hit(p);
                            break;
                        }
                    }
                }
            }

            foreach (Projectile x in killSprites)
            {
                _gameState.Projectiles.Remove(x);
            }
        }

        private void UpdateUnits(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            for (int i = 0; i < _gameState.Units.Count; i++)
            {
                Unit u = _gameState.Units[i];
                u.ReloadGun(elapsed);

                switch (u.CurrentOrder.Type)
                {
                    case OrderType.Special:

                        if (u.Special.Complete)
                        {
                            u.OrderComplete();
                            u.Special.Complete = false;
                            u.Special.Elapsed = 0f;
                        }
                        else
                        {
                            u.Special.Execute(elapsed, ref u, ref _gameState);
                        }
                        break;

                    case OrderType.None:
                        u.OrderComplete();
                        break;

                    case OrderType.Move:
                        u.Target(u.CurrentOrder.Target);

                        if (u.CurrentAngle == u.TargetAngle)
                        {
                            u.Momentum = new Vector2((float)Math.Cos(u.TargetAngle) * -1, (float)Math.Sin(u.TargetAngle) * -1);
                        }
                        else
                        {
                            u.Momentum = new Vector2(0f);
                        }

                        if (u.CurrentAngle == u.TargetAngle && Sprite.AlmostEquals(u.Location.X, u.CurrentOrder.Target.X, 2) && Sprite.AlmostEquals(u.Location.Y, u.CurrentOrder.Target.Y, 2))
                        {
                            u.Location = u.CurrentOrder.Target;
                            u.Momentum = new Vector2(0f);
                            u.OrderComplete();
                        }

                        if (u.Momentum.X > 0f || u.Momentum.Y > 0f)
                        {
                            foreach (Block b in _gameState.Blocks)
                            {
                                if (u.Touches(b) && b.CurrentHP > 0)
                                {
                                    u.Momentum = new Vector2(0f);
                                    u.CurrentHP -= u.MaxHP / 15;
                                    b.CurrentHP -= b.MaxHP / 2;
                                    break;
                                }
                            }
                        }

                        break;

                    case OrderType.Attack:
                        u.Momentum = new Vector2(0f);
                        u.Target(u.CurrentOrder.Target);

                        if (u.CurrentAngle == u.TargetAngle)
                        {
                            u.Shoot(ref _gameState.Projectiles);
                        }

                        if (u.ShotCount == 5)
                        {
                            u.ShotCount = 0;
                            u.OrderComplete();
                        }
                        break;
                }

                u.UpdateState();
            }
        }
    }
}