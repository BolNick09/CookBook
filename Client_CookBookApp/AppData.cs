using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonData;
using ServerCommonData;
using Microsoft.EntityFrameworkCore;

namespace Client_CookBookApp
{
    public class AppData
    {
        public ObservableCollection<Product> Products { get; private set; } 
        public ObservableCollection<Dish> Dishes { get; private set; }
        public ObservableCollection<Tag> Tags { get; private set; }
        public CookBookClient Client { get; private set; }

        public async Task InitializeAsync(string ip, int port)
        {
            Products = new();
            Dishes = new();
            Tags = new();
            Client = new (ip, port);

            GetAllData();



        }
        public async Task GetAllData()
        {
            try
            {
                // Загрузка тегов
                var tags = await Client.SendRequestAsync<ObservableCollection<Tag>>("GetAll", "Tag");
                Tags.Clear();
                foreach (var tag in tags) Tags.Add(tag);

                // Загрузка продуктов
                var products = await Client.SendRequestAsync<ObservableCollection<Product>>("GetAll", "Product");
                Products.Clear();
                foreach (var product in products) Products.Add(product);

                // Загрузка блюд
                var dishes = await Client.SendRequestAsync<ObservableCollection<Dish>>("GetAll", "Dish");
                Dishes.Clear();
                foreach (var dish in dishes) Dishes.Add(dish);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
    }

}