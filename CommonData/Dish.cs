using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Comment { get; set; }    // необязательный комментарий
        public bool IsFavorite { get; set; }   // избранное (да/нет)
        public string Recipe { get; set; } = null!; // рецепт
        public byte[]? Image { get; set; }     // картинка (необязательно)

        // Связи
        public List<DishProduct> DishProducts { get; set; } = new(); // продукты в блюде
        public List<DishTag> DishTags { get; set; } = new();         // теги блюда
    }
}
