using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketManager
{
    public static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
    private const int Port = 11000;

    private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
    private static readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);
    private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);

    private static void AcceptCallback(IAsyncResult ar)
    {
        AllDone.Set();

        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        StateObject state = new StateObject();
        state.WorkSocket = handler;
        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
    }

    public static int Main(String[] args)
    {
       // Thread server = new Thread(StartListening);
        Thread client = new Thread(StartClient);

       // server.Start();
        client.Start();

        while (client.IsAlive)
        {
        }
        return 0;
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
                File.Delete(@"c:\test2.bin");
                File.WriteAllBytes(@"c:\test2.bin", Convert.FromBase64String(content.Replace("<EOF>", string.Empty)));

                SendServer(handler, content);
            }
            else
            {
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
        }
    }

    public static void StartListening()
    {
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
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
                //if (state.Builder.Length > 1)
                //{
                //    response = state.Builder.ToString();
                //}
                ReceiveDone.Set();
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private static void Send(Socket client, string data)
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

    public static void StartClient()
    {
        try
        {
            byte[] bytes = File.ReadAllBytes("c:\\file.bin");
            string x = Convert.ToBase64String(bytes);

            IPHostEntry ipHostInfo = Dns.Resolve(Environment.MachineName);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(remoteEP, ConnectCallback, client);
            ConnectDone.WaitOne();

            Send(client, x + "<EOF>");
            SendDone.WaitOne();

            Receive(client);
            ReceiveDone.WaitOne();

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

public class StateObject
{
    public const int BufferSize = 256;
    public readonly byte[] Buffer = new byte[BufferSize];
    public readonly StringBuilder Builder = new StringBuilder();
    public Socket WorkSocket;
}