using CommonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_CookBookApp;

public class CalculatedProduct
{
    public Product Product { get; set; }  // Добавляем ссылку на продукт
    public string Name { get; set; }
    public double Grams { get; set; }
    public double Calories { get; set; }

    // Вычисляемые свойства для отображения в DataGrid
    public double Proteins { get => (double)(Product.Proteins * (decimal)Grams / 100); set; }
    public double Fats { get => (double)(Product.Fats * (decimal)Grams / 100); set; }
    public double Carbohydrates { get => (double)(Product.Carbohydrates * (decimal)Grams / 100); set; }
    public CalculatedProduct(Product product, double grams)
    {
        Product = product;
        Grams = grams;
    }
}
