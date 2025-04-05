using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Связь с блюдами (многие-ко-многим через DishTag)
        public List<DishTag> DishTags { get; set; } = new();
    }
}
