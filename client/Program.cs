using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace client
{
    class Program
    {
        static string _userName;
        static string _host = "127.0.0.1";
        static int _port = 8080;
        static TcpClient _client;
        static NetworkStream _stream;

        static void Main(string[] args)
        {
            Console.Write("Enter your username: ");
            _userName = Console.ReadLine();

            try
            {
                _client = new TcpClient();

                _client.Connect(_host, _port);
                _stream = _client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(_userName);
                _stream.Write(data, 0, data.Length);

                new Thread(ReceiveMessages).Start();

                SendMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                _stream?.Close();
                _client?.Close();
            }
        }

        static void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    byte[] buff = new byte[1024];

                    int bytesCount = 0;
                    string message = string.Empty;

                    do
                    {
                        bytesCount = _stream.Read(buff, 0, buff.Length);
                        message += Encoding.UTF8.GetString(buff, 0, bytesCount);
                    }
                    while (_stream.DataAvailable);

                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                _stream?.Close();
                _client?.Close();
            }
        }

        static void SendMessages()
        {          
            while (true)
            {
                Console.Write("Enter the message: ");
                string message = Console.ReadLine();

                byte[] buff = Encoding.UTF8.GetBytes(message);
                _stream.Write(buff, 0, buff.Length);
            }
        }
    }
}
