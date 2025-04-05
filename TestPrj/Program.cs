using System;
using CommonData;

namespace TestPrj
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new CookBookDbContext();

            // Проверка, что БД создана и таблицы существуют
            db.Database.EnsureCreated();

            // Пример добавления тестовых данных
            if (!db.Products.Any())
            {
                db.Products.Add(new Product { Name = "Яблоко", Proteins = 3, Fats = 2, Carbohydrates = 14, Calories = 52 });
                db.SaveChanges();
                Console.WriteLine("Добавлен тестовый продукт!");
            }

            Console.WriteLine($"Продуктов в БД: {db.Products.Count()}");
        }
    }
}
