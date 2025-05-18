using Azure.Core;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServerCommonData
{
    public class CookBookClient
    {
        private readonly string _serverIp;
        private readonly int _serverPort;

        public CookBookClient(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        public async Task<T> SendRequestAsync<T>(string action, string entityType, object data = null)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(_serverIp, _serverPort);

                    var request = new Request
                    {
                        Action = action,
                        EntityType = entityType,
                        Data = data != null ? JsonConvert.SerializeObject(data) : null
                    };

                    var requestJson = JsonConvert.SerializeObject(request);
                    var requestBytes = Encoding.UTF8.GetBytes(requestJson);

                    var stream = client.GetStream();
                    await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

                    var buffer = new byte[1024];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var response = JsonConvert.DeserializeObject<Response>(responseJson);

                    if (!response.Success)
                        throw new Exception(response.Error);

                    return JsonConvert.DeserializeObject<T>(response.Data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
