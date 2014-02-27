using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Xna.Framework.Net;

namespace Contingency
{
    public partial class Game1
    {
        private GameState _lastRecievedState;

        private string _oponentData;

        public int Port
        {
            get
            {
                return Server ? 12000 : 11000;
            }
        }

        public string MyIP()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        public void Send(string data)
        {
            Send(_oponentData, data);
        }

        public void Send(string target, string data)
        {
            SocketManager.Send(SocketManager.GetClient(target, Port), data + "<EOF>");
        }

        private GameState ConsolidateOponentData()
        {
            while (_lastRecievedState == null)
            {
                Thread.Sleep(100);
            }

            GameState consolidated = GameState.ConsolidateGamesState(_gameState, _lastRecievedState, "red");
            Send(_oponentData, Convert.ToBase64String(SaveState(consolidated).ToArray()));

            _lastRecievedState = null;

            return consolidated;
        }

        private GameState ConsolidateServerData()
        {
            MemoryStream steam = SaveState(_gameState);

            Send(_oponentData, Convert.ToBase64String(steam.ToArray()));

            while (_lastRecievedState == null)
            {
                Thread.Sleep(100);
            }

            return _lastRecievedState;
        }

        private void JoinServer(string serverIp)
        {
            Send(serverIp, MyIP());

            while (string.IsNullOrEmpty(_oponentData))
            {
                Thread.Sleep(100);
            }

            while (_lastRecievedState == null)
            {
                Thread.Sleep(100);
            }

            _gameState = _lastRecievedState;
            _lastRecievedState = null;

            _messages.Remove("Joining server: 192.168.137.1:11000");
        }

        private GameState LoadState(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return (GameState)new BinaryFormatter().Deserialize(stream);
        }

        private MemoryStream SaveState(GameState state)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, state);

            return stream;
        }
        private void SocketManager_DataRecieved(object sender, EventArgs e)
        {
            GameStateDataEventArgs arg = (GameStateDataEventArgs)e;

            if (!string.IsNullOrEmpty(arg.Message))
            {
                _oponentData = arg.Message;
                if (Server)
                    Send(MyIP());
            }
            else
            {
                MemoryStream stream = new MemoryStream(((GameStateDataEventArgs)e).Data);
                _lastRecievedState = LoadState(stream);
            }
        }

        private void WaitForOpponentToJoin()
        {
            while (string.IsNullOrEmpty(_oponentData))
            {
                Thread.Sleep(100);
            }

            Send(_oponentData, Convert.ToBase64String(SaveState(_gameState).ToArray()));

            _messages.Remove("Waiting for clients on: 192.168.137.1:11000");
        }
    }
}