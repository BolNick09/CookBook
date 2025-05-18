using System.Collections.ObjectModel;
using System.Linq;
using CommonData;
using Microsoft.EntityFrameworkCore;

namespace CookBookClient
{
    public static class AppData
    {
        public static ObservableCollection<Product> Products { get; } = new();
        public static ObservableCollection<Dish> Dishes { get; } = new();
        public static ObservableCollection<Tag> Tags { get; } = new();
        public static CookBookDbContext db = new();

        static AppData()
        {
            // Инициализируем или загружаем данные
            if (!db.Tags.Any())
                InitializeData();
            else
                LoadDataFromDatabase();
        }

        private static void InitializeData()
        {
            #region Теги
            var tags = new List<Tag>
            {
                new Tag {  Name = "Мясное" },
                new Tag {  Name = "Постное" },
                new Tag {  Name = "Рыбное" },
                new Tag {  Name = "Вегетарианское" }
            };
            db.Tags.AddRange(tags);
            db.SaveChanges();
            foreach (var tag in tags)
                Tags.Add(tag);
            #endregion

            #region Продукты
            var products = new List<Product>
            {
                new Product {  Name = "Яйца", Calories = 155, Proteins = 13m, Fats = 11m, Carbohydrates = 1.1m },
                new Product {  Name = "Молоко", Calories = 60, Proteins = 3.2m, Fats = 3.6m, Carbohydrates = 4.8m },
                new Product {  Name = "Мука", Calories = 364, Proteins = 10.3m, Fats = 1m, Carbohydrates = 76.9m },
                new Product {  Name = "Сахар", Calories = 387, Proteins = 0m, Fats = 0m, Carbohydrates = 100m },
                new Product {  Name = "Масло", Calories = 717, Proteins = 0.5m, Fats = 81m, Carbohydrates = 0.8m },
                new Product {  Name = "Яблоки", Calories = 52, Proteins = 0.3m, Fats = 0.2m, Carbohydrates = 14m }
            };
            db.Products.AddRange(products);
            db.SaveChanges();
            foreach (var product in products)
                Products.Add(product);
            #endregion

            #region Блюда и связи

            // Перезапрашиваем объекты из БД, чтобы EF Core мог отслеживать их корректно
            var egg = db.Products.Single(p => p.Name == "Яйца");
            var milk = db.Products.Single(p => p.Name == "Молоко");
            var flour = db.Products.Single(p => p.Name == "Мука");
            var sugar = db.Products.Single(p => p.Name == "Сахар");
            var butter = db.Products.Single(p => p.Name == "Масло");
            var apple = db.Products.Single(p => p.Name == "Яблоки");

            var leanTag = db.Tags.Single(t => t.Name == "Постное");
            var vegTag = db.Tags.Single(t => t.Name == "Вегетарианское");

            var dishes = new List<Dish>
{
    new Dish
    {
        Name = "Блины",
        Recipe = "1. Смешать яйца с молоком\n2. Добавить муку и сахар\n3. Жарить на сковороде",
        IsFavorite = true,
        ImagePath = ".images/pancakes.jpg",
        DishProducts = new List<DishProduct>
        {
            new DishProduct { ProductId = egg.Id, Weight = 100 },
            new DishProduct { ProductId = milk.Id, Weight = 200 },
            new DishProduct { ProductId = flour.Id, Weight = 150 },
            new DishProduct { ProductId = butter.Id, Weight = 30 }
        },
        DishTags = new List<DishTag>
        {
            new DishTag { TagId = leanTag.Id },
            new DishTag { TagId = vegTag.Id }
        }
    }
};

            db.Dishes.AddRange(dishes);
            db.SaveChanges();
            foreach (var dish in dishes)
                Dishes.Add(dish);

            #endregion
        }

        private static void LoadDataFromDatabase()
        {
            // Полностью загружаем все данные из БД
            var loadedTags = db.Tags.ToList();
            foreach (var tag in loadedTags)
                Tags.Add(tag);

            var loadedProducts = db.Products.ToList();
            foreach (var product in loadedProducts)
                Products.Add(product);

            var loadedDishes = db.Dishes
                .Include(d => d.DishProducts).ThenInclude(dp => dp.Product)
                .Include(d => d.DishTags).ThenInclude(dt => dt.Tag)
                .ToList();

            foreach (var dish in loadedDishes)
                Dishes.Add(dish);
        }
    }
}