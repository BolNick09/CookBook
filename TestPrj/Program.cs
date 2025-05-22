using System;
using CommonData;
using ServerCommonData;

namespace TestPrj
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new CookBookClient("127.0.0.1", 2024);

            // Пример 1: Получение всех блюд
            var dishes = await client.SendRequestAsync<List<Dish>>("GetAll", "Dish");
            Console.WriteLine($"Found {dishes.Count} dishes:");
            foreach (var dish in dishes)
                Console.WriteLine($"- {dish.Name}");

            // Пример 2: Добавление нового блюда
            var newDish = new Dish
            {
                Name = "Паста Карбонара",
                Recipe = "Спагетти, яйца, бекон, пармезан...",
                IsFavorite = true
            };

            await client.SendRequestAsync<object>("Add", "Dish", newDish);
            Console.WriteLine("Dish added!");

            // Пример 3: Удаление блюда по ID
            await client.SendRequestAsync<object>("Delete", "Dish", 1); // ID=1
            Console.WriteLine("Dish deleted!");
        }
    }
}
