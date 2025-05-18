using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonData
{
    public class Request
    {
        public string Action { get; set; } // "GetAll", "Add", "Update", "Delete"
        public string EntityType { get; set; } // "Dish", "Tag", "Product", etc.
        public string Data { get; set; } // JSON-строка с объектом
    }
}
