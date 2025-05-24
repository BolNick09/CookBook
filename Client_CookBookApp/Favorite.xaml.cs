using Azure;
using CommonData;
using ServerCommonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_CookBookApp
{
    /// <summary>
    /// Interaction logic for Favorite.xaml
    /// </summary>
    public partial class Favorite : Window
    {
        private readonly AppData appData;
        public Favorite(MainWindow owner)
        {
            InitializeComponent();
            appData = owner.appData;
            LoadData();
        }

        private void LoadData()
        {
            // Заполняем выпадающий список всеми блюдами
            AllDishesComboBox.ItemsSource = appData.Dishes.ToList();

            // Отображаем текущие избранные блюда
            RefreshFavorites();
        }
        private async void SaveToDatabase(Dish dish)
        {
            try
            {
                var result = await appData.Client.SendRequestAsync<ServerCommonData.Response>("Update", "Dish", dish);
                if (!result.Success)
                    MessageBox.Show(result.Error);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void RefreshFavorites()
        {
            // Фильтруем только избранные блюда
            FavoritesListBox.ItemsSource = appData.Dishes
                .Where(d => d.IsFavorite)
                .ToList();
        }

        private void AddFavoriteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllDishesComboBox.SelectedItem is Dish selectedDish)
            {
                // Устанавливаем флаг избранного
                selectedDish.IsFavorite = true;

                // Обновляем интерфейс
                RefreshFavorites();

                // Сохраняем в БД
                SaveToDatabase(selectedDish);
            }
        }

        private void RemoveFavoriteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FavoritesListBox.SelectedItem is Dish selectedDish)
            {
                // Снимаем флаг избранного
                selectedDish.IsFavorite = false;

                // Обновляем интерфейс
                RefreshFavorites();
                ClearDetails();

                // Сохраняем в БД
                SaveToDatabase(selectedDish);
            }
        }

        private void FavoritesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoritesListBox.SelectedItem is Dish selectedDish)
            {
                RecipeTitle.Text = selectedDish.Name;
                RecipeDescription.Text = selectedDish.Recipe;

                if (!string.IsNullOrEmpty(selectedDish.ImagePath))
                {
                    RecipeImage.Source = new BitmapImage(new Uri(selectedDish.ImagePath, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    RecipeImage.Source = null;
                }
            }
            else
            {
                ClearDetails();
            }
        }

     

        private void ClearDetails()
        {
            RecipeTitle.Text = "Выберите рецепт";
            RecipeDescription.Text = "";
            RecipeImage.Source = null;
        }


    }
}
