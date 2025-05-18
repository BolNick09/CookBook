using Azure.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private readonly TcpListener _listener;
        private readonly DbService _dbService;

        public Server(string ip, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ip), port);
            _dbService = new DbService();
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine($"Server started on {_listener.LocalEndpoint}");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                _ = HandleClientAsync(client); // Обработка клиента в отдельном потоке
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[1024];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Десериализация запроса (ожидается объект типа Request)
                    var request = JsonConvert.DeserializeObject<Request>(requestJson);

                    // Обработка запроса
                    var response = await _dbService.ProcessRequestAsync(request);

                    // Отправка ответа клиенту
                    var responseJson = JsonConvert.SerializeObject(response);
                    var responseBytes = Encoding.UTF8.GetBytes(responseJson);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
