using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using CommonData;
using Microsoft.EntityFrameworkCore;

namespace Client_CookBookApp
{
    /// <summary>
    /// Interaction logic for RecipesManagementWindow.xaml
    /// </summary>
    public partial class RecipesManagementWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<DishProduct> _currentDishProducts = new();
        private Dish _currentDish = null;

        public RecipesManagementWindow()
        {
            InitializeComponent();
            InitializeDataBindings();
            LoadData();
        }

        private async  void LoadData()
        {
            try
            {
                await AppData.InitializeAsync();
                RecipesGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void InitializeDataBindings()
        {
            RecipesGrid.ItemsSource = AppData.Dishes;
            ProductsComboBox.ItemsSource = AppData.Products;
            ProductsComboBox.DisplayMemberPath = "Name";
            IngredientsListBox.ItemsSource = _currentDishProducts;
            IngredientsListBox.DisplayMemberPath = "Product.Name";
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion


        #region UI Methods

        private void ResetForm()
        {
            _currentDish = null;
            _currentDishProducts.Clear();
            RecipeNameTextBox.Text = "";
            DescriptionTextBox.Text = "";
        }


        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт из списка", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = (Product)ProductsComboBox.SelectedItem;

            if (_currentDishProducts.Any(dp => dp.ProductId == selectedProduct.Id))
            {
                MessageBox.Show("Этот ингредиент уже добавлен", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var weightInput = new WeightInputWindow(selectedProduct.Name);
            if (weightInput.ShowDialog() == true)
            {
                _currentDishProducts.Add(new DishProduct
                {
                    ProductId = selectedProduct.Id,
                    Product = selectedProduct,
                    Weight = weightInput.Weight
                });
            }
        }
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(RecipeNameTextBox.Text))
            {
                MessageBox.Show("Введите название блюда", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (_currentDishProducts.Count == 0)
            {
                MessageBox.Show("Добавьте ингредиенты", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                // 1. Сохраняем основное блюдо
                var dishAction = _currentDish?.Id == 0 ? "Add" : "Update";
                var savedDish = await AppData.Client.SendRequest<Dish>(
                    dishAction,
                    "Dish",
                    new Dish
                    {
                        Id = _currentDish?.Id ?? 0,
                        Name = RecipeNameTextBox.Text,
                        Recipe = DescriptionTextBox.Text
                    });

                // 2. Удаляем старые ингредиенты (если редактирование)
                if (dishAction == "Update")
                {
                    var existingIngredients = await AppData.Client.SendRequest<List<DishProduct>>(
                        "GetAll",
                        "DishProduct",
                        new { DishId = savedDish.Id });

                    foreach (var ingredient in existingIngredients)
                    {
                        await AppData.Client.SendRequest<bool>(
                            "Delete",
                            "DishProduct",
                            new { ingredient.DishId, ingredient.ProductId });
                    }
                }

                // 3. Добавляем новые ингредиенты
                foreach (var ingredient in _currentDishProducts)
                {
                    await AppData.Client.SendRequest<bool>(
                        "Add",
                        "DishProduct",
                        new DishProduct
                        {
                            DishId = savedDish.Id,
                            ProductId = ingredient.ProductId,
                            Weight = ingredient.Weight
                        });
                }

                // 4. Обновляем локальные данные
                await AppData.InitializeAsync();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipesGrid.SelectedItem is Dish selectedDish)
            {
                try
                {
                    // Загружаем ингредиенты с сервера
                    var ingredients = await AppData.Client.SendRequest<List<DishProduct>>(
                        "GetAll",
                        "DishProduct",
                        new { DishId = selectedDish.Id });

                    _currentDish = selectedDish;
                    _currentDishProducts.Clear();

                    foreach (var ingredient in ingredients)
                    {
                        _currentDishProducts.Add(new DishProduct
                        {
                            DishId = ingredient.DishId,
                            ProductId = ingredient.ProductId,
                            Product = AppData.Products.First(p => p.Id == ingredient.ProductId),
                            Weight = ingredient.Weight
                        });
                    }

                    RecipeNameTextBox.Text = _currentDish.Name;
                    DescriptionTextBox.Text = _currentDish.Recipe;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipesGrid.SelectedItem is not Dish selectedDish) return;

            try
            {
                // 1. Удаляем ингредиенты
                var ingredients = await AppData.Client.SendRequest<List<DishProduct>>(
                    "GetAll",
                    "DishProduct",
                    new { DishId = selectedDish.Id });

                foreach (var ingredient in ingredients)
                {
                    await AppData.Client.SendRequest<bool>(
                        "Delete",
                        "DishProduct",
                        new { ingredient.DishId, ingredient.ProductId });
                }

                // 2. Удаляем блюдо
                await AppData.Client.SendRequest<bool>("Delete", "Dish", selectedDish.Id);

                // 3. Обновляем локальные данные
                AppData.Dishes.Remove(selectedDish);
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }

        #endregion
    }

    // Вспомогательное окно для ввода веса
    public class WeightInputWindow : Window
    {
        public decimal Weight { get; private set; }

        public WeightInputWindow(string productName)
        {
            Title = $"Введите количество {productName} (г)";
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            var label = new Label { Content = $"Введите количество {productName} в граммах:" };
            var textBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new Button
            {
                Content = "OK",
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 5, 10, 5),
                IsDefault = true
            };

            var cancelButton = new Button
            {
                Content = "Отмена",
                Padding = new Thickness(10, 5, 10, 5),
                IsCancel = true
            };

            okButton.Click += (s, ev) =>
            {
                if (decimal.TryParse(textBox.Text, out var weight) && weight > 0)
                {
                    Weight = weight;
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Введите корректное количество (положительное число)", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            cancelButton.Click += (s, ev) => DialogResult = false;

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(buttonPanel);

            Content = stackPanel;
        }
    }
}
