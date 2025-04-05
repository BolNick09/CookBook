using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class DishProduct
    {
        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public decimal Weight { get; set; } // масса продукта в граммах
    }
}
