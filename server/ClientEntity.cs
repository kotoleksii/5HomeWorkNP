using System;
using System.Net.Sockets;
using System.Text;

namespace server
{
    class ClientEntity
    {
        public string Id { get; private set; }
        string _username;
        ServerEntity _server;
        TcpClient _client;
        NetworkStream _stream;

        public ClientEntity(TcpClient client, ServerEntity server)
        {
            Id = Guid.NewGuid().ToString();
            _client = client;
            _server = server;
        }

        public void Processing()
        {
            try
            {
                _stream = _client.GetStream();
                string message = DecodeMessage();

                _username = message;

                message += " joined to chat!";
                _server.BroadCast(message, Id);
                Console.WriteLine(message);
                

                while (true) 
                {
                    try
                    {    
                        message = DecodeMessage();                                                 
                        _server.BroadCast($"{_username}: {message}", Id);
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\t{_username} " +
                            $"send message:\t{message}");
                                                         
                    }
                    catch (Exception ex)
                    {
                        message = $"{_username} left the chat :( bye, bye ...";
                        _server.BroadCast(message, Id);
                        Console.WriteLine(message);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                Disconnect();
                _server.DisconnectClient(Id);
            }        
        }

        private string DecodeMessage()
        {         
            byte[] buff = new byte[512];
            int bytesCount = 0;
            string message = string.Empty;

            do
            {
                bytesCount = _stream.Read(buff, 0, buff.Length);
                message += Encoding.UTF8.GetString(buff, 0, bytesCount);
            } while (_stream.DataAvailable);

            return message;
        }

        public void SendMessage(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}
