using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace Contingency
{
    public partial class Game1
    {
        private void CatchInputs()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Root/>");
                doc.DocumentElement.AppendChild(doc.ImportNode(_gameState.Serialize(), true));
                doc.Save("c:\\test.xml");

                _gameState = GameState.Deserialize(doc.DocumentElement.ChildNodes[0]);
            }

            CatchViewMove();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _waitJoin.Abort();
                _messages.Clear();
            }

            if (_waitJoin != null && _waitJoin.IsAlive)
            {
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (_menuMode)
                {
                    ToggleMenuMode(false);
                }
                else if (_inPlanningMode)
                {
                    EndTurnClicked(this, EventArgs.Empty);
                }
            }

            _mouseStateCurrent = Mouse.GetState();

            if (_inPlanningMode)
            {
                if (_mouseStateCurrent.LeftButton == ButtonState.Pressed && _mouseStatePrevious.LeftButton == ButtonState.Released)
                {
                    MouseClickedGame();
                }
                else if (_mouseStateCurrent.RightButton == ButtonState.Pressed && _mouseStatePrevious.RightButton == ButtonState.Released)
                {
                    Unit selected = GetSelctedUnit();

                    if (selected != null)
                    {
                        selected.Selected = false;
                        _menu.Visible = false;
                    }
                }
            }

            _mouseStatePrevious = _mouseStateCurrent;
        }

        private void CatchViewMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (_view.Y < 0)
                    _view.Y += 10;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (Math.Abs(_view.Y) + _view.Height < _map.Height)
                    _view.Y -= 10;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (_view.X < 0)
                    _view.X += 10;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (Math.Abs(_view.X) + _view.Width < _map.Width)
                    _view.X -= 10;
            }
        }

        private void EndTurnClicked(object sender, EventArgs e)
        {
         //   _messages.Add("Waiting for opponent", new Vector2(100, 100));

            _inPlanningMode = false;
            _remainingPlayTime = 5000;
        }

        private void MouseClickedGame()
        {
            Unit selectedUnit = GetSelctedUnit();

            Vector2 clickLocation = new Vector2(_mouseStateCurrent.X, _mouseStateCurrent.Y) - ViewOffset;

            if (selectedUnit == null)
            {

                foreach (Unit u in _gameState.Units)
                {
                    if (u.Touches(clickLocation, 2.0) && u.Team == CurrentTeam)
                    {
                        u.Selected = !u.Selected;
                        break;
                    }
                }

                foreach (Button b in _buttons)
                {
                    if (b.Visible)
                        b.CheckClicked(clickLocation);
                }
            }
            else
            {
                if (!_menu.Visible)
                {
                    _menu.Visible = true;
                    _menu.Location = new Vector2(_mouseStateCurrent.X - _menu.Width / 2, _mouseStateCurrent.Y - _menu.Height / 2);
                }
                else
                {
                    Vector2 menuClick = clickLocation + ViewOffset;
                    if (_menu.TouchesWithOffset(menuClick, 1.0, _menu.Width / 2))
                    {
                        Vector2 menuCenter = new Vector2(_menu.Location.X + _menu.Width / 2, _menu.Location.Y + _menu.Height / 2);
                        switch (_menu.GetMenuSelection(menuClick))
                        {
                            case "Attack":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Attack, menuCenter - ViewOffset));
                                break;

                            case "Move":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Move, menuCenter - ViewOffset));
                                break;

                            case "Special":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Special, menuCenter - ViewOffset));
                                break;

                            case "Stop":
                                Vector2 target = selectedUnit.Location;

                                if (selectedUnit.OrderQueue.Count > 1)
                                {
                                    target = selectedUnit.OrderQueue.Last().Target;
                                }
                                selectedUnit.OrderQueue.Add(new Order(OrderType.None, target));
                                break;
                        }
                    }
                }
            }
        }
    }
}