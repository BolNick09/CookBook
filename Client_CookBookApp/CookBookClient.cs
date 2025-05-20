using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_CookBookApp;


    public class CookBookClient
    {
        private readonly ServerCommonData.CookBookClient _client;

        public CookBookClient()
        {
            _client = new ServerCommonData.CookBookClient("127.0.0.1", 2024);
        }

        public async Task<T> SendRequest<T>(string action, string entityType, object data = null)
        {
            return await _client.SendRequestAsync<T>(action, entityType, data);
        }
    }

