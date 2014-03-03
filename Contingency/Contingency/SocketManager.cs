using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Contingency
{
    public class GameStateDataEventArgs : EventArgs
    {
        public string Message { get; set; }

        public byte[] Data { get; set; }

        public GameStateDataEventArgs(byte[] data)
        {
            Data = data;
        }

        public GameStateDataEventArgs(string message)
        {
            Message = message;
        }
    }

    public static class SocketManager
    {
        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);

        public static bool Stop = false;

        public static event EventHandler DataRecieved;

        public static void OnDatarecieved(GameStateDataEventArgs e)
        {
            EventHandler handler = DataRecieved;
            if (handler != null)
            {
                handler(null, e);
            }
        }


        private static void AcceptCallback(IAsyncResult ar)
        {
            AllDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.Builder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                String content = state.Builder.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    try
                    {
                        OnDatarecieved(new GameStateDataEventArgs(Convert.FromBase64String(content.Replace("<EOF>", string.Empty))));
                    }
                    catch (Exception)
                    {
                        OnDatarecieved(new GameStateDataEventArgs(content.Replace("<EOF>", string.Empty)));
                    }

                    SendServer(handler, content);
                }
                else
                {
                    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                }
            }
        }

        public static void StartListening(string ip, int port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (!Stop)
                {
                    AllDone.Reset();
                    listener.BeginAccept(AcceptCallback, listener);
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                ConnectDone.Set();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject { WorkSocket = client };
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.Builder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                SendDone.Set();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static void SendCallbackServer(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void SendServer(Socket handler, string data)
        {
            byte[] byteData = Encoding.Unicode.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallbackServer, handler);
        }

        public static Socket GetClient(string ip, int port)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(remoteEP, ConnectCallback, client);
            ConnectDone.WaitOne();

            return client;
        }
    }

    public class StateObject
    {
        public const int BufferSize = 256;
        public readonly byte[] Buffer = new byte[BufferSize];
        public readonly StringBuilder Builder = new StringBuilder();
        public Socket WorkSocket;
    }
}