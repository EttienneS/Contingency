using System;
using System.IO;
using System.Linq;
using System.Threading;
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

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _waitJoin.Abort();
                _messages.Clear();
            }

            if (_waitJoin != null && _waitJoin.IsAlive)
            {
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                // save / load example
                MemoryStream state = SaveState(_gameState);
                state.Seek(0, SeekOrigin.Begin);
                using (FileStream file = new FileStream(@"c:\file.bin", FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = new byte[state.Length];
                    state.Read(bytes, 0, (int)state.Length);
                    file.Write(bytes, 0, bytes.Length);
                    //state.Close();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F8))
            {
                using (MemoryStream ms = new MemoryStream())
                using (FileStream file = new FileStream(@"c:\file.bin", FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);

                    _gameState = LoadState(ms);
                }
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

        private void EndTurnClicked(object sender, EventArgs e)
        {
            _messages.Add("Waiting for opponent", new Vector2(100, 100));

            if (Server)
            {
                GameState state  = ConsolidateOponentData();
                _gameState = state;
            }
            else
            {
                GameState state = ConsolidateServerData();
                _lastRecievedState = null;
                _gameState = state;
            }

            _messages.Remove("Waiting for opponent");

            if (_inPlanningMode)
            {
                _inPlanningMode = false;
                _remainingPlayTime = TotalPlayTime;
            }
        }

        private void MouseClickedGame()
        {
            Unit selectedUnit = GetSelctedUnit();

            if (selectedUnit == null)
            {
                Vector2 clickLocation = new Vector2(_mouseStateCurrent.X, _mouseStateCurrent.Y);
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
                Vector2 mouseVector = new Vector2(_mouseStateCurrent.X, _mouseStateCurrent.Y);
                if (!_menu.Visible)
                {
                    _menu.Visible = true;
                    _menu.Location = new Vector2(_mouseStateCurrent.X - _menu.Width / 2, _mouseStateCurrent.Y - _menu.Height / 2);
                }
                else
                {
                    if (_menu.TouchesWithOffset(mouseVector, 1.0, _menu.Width / 2))
                    {
                        Vector2 menuCenter = new Vector2(_menu.Location.X + _menu.Width / 2, _menu.Location.Y + _menu.Height / 2);
                        switch (_menu.GetMenuSelection(mouseVector))
                        {
                            case "Attack":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Attack, menuCenter));
                                break;

                            case "Move":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Move, menuCenter));
                                break;

                            case "Special":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Special, menuCenter));
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