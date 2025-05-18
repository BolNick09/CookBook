using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Response
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Data { get; set; } // JSON-строка с результатом
    }
}
