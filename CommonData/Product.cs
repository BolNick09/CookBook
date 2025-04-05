using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Proteins { get; set; }  // белки на 100г
        public decimal Fats { get; set; }      // жиры на 100г
        public decimal Carbohydrates { get; set; } // углеводы на 100г
        public decimal Calories { get; set; }  // калории на 100г
        public byte[]? Image { get; set; }     // картинка (необязательно)

        // Связь с блюдами (многие-ко-многим через DishProduct)
        public List<DishProduct> DishProducts { get; set; } = new();
    }
}
