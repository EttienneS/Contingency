using System;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public partial class Game1
    {
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            DrawSprite(SpriteList.ContentSprites["background"], new Vector2(0, 0) + ViewOffset, 1.0f);

            if (!_menuMode)
            {
                DrawBlocks();

                DrawProjectiles();

                DrawUnits();

                DrawExplosions();

                DrawMenu();
            }

            DrawText();

            DrawButtons();

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawBlocks()
        {
            foreach (Block b in _gameState.Blocks)
            {
                if (b != null)
                    _spriteBatch.Draw(b.GetSprite(), b.Location + ViewOffset, null, Color.White, b.CurrentAngle, new Vector2(b.Width / 2, b.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawButtons()
        {
            foreach (Button button in _buttons)
            {
                if (button.Visible)
                {
                    DrawSprite(button.Sprite, button.Location, 1.0f);
                    _spriteBatch.DrawString(SpriteList.Font, button.Text, button.Location + new Vector2(10, 3), button.TextColor);
                }
            }
        }

        private void DrawExplosions()
        {
            foreach (Explosion exp in _gameState.Explosions)
            {
                if (exp != null)
                    _spriteBatch.Draw(exp.SpriteSheet, new Rectangle((int)exp.Location.X - 32 + (int)ViewOffset.X, (int)exp.Location.Y - 32 + (int)ViewOffset.Y, exp.SpriteWidth, exp.SpriteHeight), exp.SpriteRect, Color.White);
            }
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, int width)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(_lineTexture, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), width), null, color, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        private void DrawMenu()
        {
            if (_menu.Visible)
            {
                _spriteBatch.Draw(_menu.GetSprite(), _menu.Location, Color.White);
            }
        }

        private void DrawProjectiles()
        {
            foreach (Projectile sprite in _gameState.Projectiles)
            {
                if (sprite != null)
                    _spriteBatch.Draw(sprite.GetSprite(), sprite.Location + ViewOffset, null, Color.White, sprite.CurrentAngle, new Vector2(sprite.Width / 2, sprite.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawSprite(Texture2D sprite, Vector2 location, float rotation)
        {
            _spriteBatch.Draw(sprite, location, null, Color.White, 0f, new Vector2(0, 0), rotation, SpriteEffects.None, 0f);
        }

        private void DrawText()
        {
            if (_inPlanningMode)
            {
                //   _spriteBatch.DrawString(SpriteList.Font, TimeSpan.FromMilliseconds(_remainingPlanningTime).ToString(), new Vector2(300, 0), Color.LimeGreen);
            }
            else
            {
                _spriteBatch.DrawString(SpriteList.Font, "TURN RUNNING: " + TimeSpan.FromMilliseconds(_remainingPlayTime), new Vector2(300, 0), Color.Red);
                //   _spriteBatch.DrawString(SpriteList.Font, TimeSpan.FromMilliseconds(_remainingPlayTime).ToString(), new Vector2(300, 0), Color.Red);
            }

            foreach (var message in _messages)
            {
                _spriteBatch.DrawString(SpriteList.Font, message.Key, message.Value, Color.White);
            }
        }

        private void DrawUnits()
        {
            foreach (Unit u in _gameState.Units)
            {
                if (u != null)
                {
                    _spriteBatch.Draw(u.GetSprite(), u.Location + ViewOffset, null, Color.White, u.CurrentAngle, new Vector2(u.Width / 2, u.Height / 2), 1.0f, SpriteEffects.None, 0f);

                    float hpLoc = u.Location.Y - 15;
                    Vector2 startPoint = new Vector2(u.Location.X - u.Width / 2, hpLoc);
                    Vector2 endPoint = new Vector2(startPoint.X + u.Width, hpLoc);
                    Vector2 endPointHp = new Vector2(startPoint.X + (u.Width * (u.CurrentHP / (float)u.MaxHP)), hpLoc);

                    DrawLine(_spriteBatch, startPoint + ViewOffset, endPoint + ViewOffset, Color.DarkRed, 3);
                    DrawLine(_spriteBatch, startPoint + ViewOffset, endPointHp + ViewOffset, Color.LimeGreen, 3);

                    float spesLoc = u.Location.Y + u.Height - 9;
                    Vector2 startPointSpes = new Vector2(u.Location.X - u.Width / 2, spesLoc);
                    Vector2 endPointSpes = new Vector2(startPoint.X + u.Width, spesLoc);
                    Vector2 endPointSpeTot = new Vector2(startPoint.X + (u.Width * (u.Special.Elapsed / u.Special.CoolDown)), spesLoc);

                    DrawLine(_spriteBatch, startPointSpes + ViewOffset, endPointSpes + ViewOffset, Color.DarkBlue, 3);
                    DrawLine(_spriteBatch, startPointSpes + ViewOffset, endPointSpeTot + ViewOffset, Color.Cyan, 3);
                }
            }

            // draw orders
            foreach (Unit u in _gameState.Units)
            {
                if (u.Team != CurrentTeam)
                {
                    continue;
                }

                for (int i = 0; i < u.OrderQueue.Count; i++)
                {
                    Vector2 source = u.Location;

                    if (i > 0)
                    {
                        // if this is not the first move in the sequence set the origin point to the last one in the sequence
                        Vector2 lastMoveLocation = u.Location;
                        for (int l = 0; l < i; l++)
                        {
                            if (u.OrderQueue[l].Type == OrderType.Move || (u.OrderQueue[l].Type == OrderType.Special && u.Special.Type.Equals("blink")))
                            {
                                lastMoveLocation = u.OrderQueue[l].Target;
                            }
                        }
                        source = lastMoveLocation;
                        source += new Vector2(5f);
                    }

                    Order o = u.OrderQueue[i];
                    Texture2D sprite = SpriteList.ContentSprites["targetAttack"];
                    Color color = Color.Red;

                    if (o.Type == OrderType.Move)
                    {
                        sprite = SpriteList.ContentSprites["targetMove"];
                        color = Color.LimeGreen;
                    }

                    if (o.Type == OrderType.Special)
                    {
                        sprite = SpriteList.ContentSprites["targetMove"];
                        color = Color.Aquamarine;

                        switch (u.Special.Type.ToLower())
                        {
                            case "blink":
                                color = Color.Purple;
                                float angle = (float)Math.Atan2(source.Y - o.Target.Y, source.X - o.Target.X);

                                float distance = Vector2.Distance(source, o.Target);
                                if (distance > u.Special.Power)
                                {
                                    o.Target = new Vector2((float)Math.Cos(angle) * -u.Special.Power, (float)Math.Sin(angle) * -u.Special.Power) + source;
                                }

                                DrawSprite(sprite, o.Target + ViewOffset, 1.0f);
                                DrawLine(_spriteBatch, source + ViewOffset, o.Target + new Vector2(5) + ViewOffset, color, 2);

                                continue;
                        }
                    }

                    _spriteBatch.Draw(sprite, o.Target + ViewOffset, null, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                    DrawLine(_spriteBatch, source + ViewOffset, o.Target + new Vector2(5) + ViewOffset, color, 2);
                }
            }
        }

    }
}