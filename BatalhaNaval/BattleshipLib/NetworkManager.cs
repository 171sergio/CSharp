using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BattleshipLib
{
    public class NetworkManager : IDisposable
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private TcpListener? listener;
        private bool isServer;

        public bool IsConnected => client?.Connected ?? false;

        public void StartServer(int port)
        {
            isServer = true;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Aguardando Player2 na porta {port}...");
            client = listener.AcceptTcpClient();
            stream = client.GetStream();
            Console.WriteLine("Player2 conectado!");
        }

        public void ConnectToServer(string host, int port)
        {
            isServer = false;
            client = new TcpClient();
            client.Connect(host, port);
            stream = client.GetStream();
            Console.WriteLine("Conectado ao servidor!");
        }

        public void Send(string message)
        {
            if (stream == null)
                throw new InvalidOperationException("Not connected");

            var data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public string Receive()
        {
            if (stream == null)
                throw new InvalidOperationException("Not connected");

            var buffer = new byte[32];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public void Dispose()
        {
            stream?.Dispose();
            client?.Dispose();
            if (isServer)
                listener?.Stop();
        }
    }
} 