using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace server
{
    class ServerEntity
    {
        List<ClientEntity> _clients = new List<ClientEntity>();
        TcpListener _listener;
        string _ip;
        int _port;

        public ServerEntity(string ip = "127.0.0.1", int port = 8080)
        {
            _ip = ip;
            _port = port;
        }

        public void Listen()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse(_ip), _port);
                _listener.Start();

                Console.WriteLine($"Server started at {_ip}:{_port}");

                while (true)
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();

                    ClientEntity client = new ClientEntity(tcpClient, this);
                     _clients.Add(client);

                    new Thread(client.Processing).Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");

                DisconnectAll();
            }
        }

        public void BroadCast(string message, string id)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            for (int i = 0; i < _clients.Count; ++i)
            {
                if (id != _clients[i].Id)
                    _clients[i].SendMessage(data);
            }
        }

        public void DisconnectClient(string id)
        {
            ClientEntity client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                _clients.Remove(client);
        }

        public void DisconnectAll()
        {
            _listener?.Stop();

            for (int i = 0; i < _clients.Count; ++i)
                _clients[i].Disconnect();

            //Environment.Exit(0); //TODO
        }
    }
}
