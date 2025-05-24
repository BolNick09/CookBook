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
using Azure;
using CommonData;
using ServerCommonData;
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
        private readonly AppData appData;

        public RecipesManagementWindow(MainWindow owner)
        {
            InitializeComponent();
            appData = owner.appData;
            InitializeDataBindings();            
            LoadData();
        }

        private async  void LoadData()
        {
            //    try
            //    {
            //        await AppData.InitializeAsync();
            //        RecipesGrid.Items.Refresh();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            //    }
        }

        private void InitializeDataBindings()
        {
            RecipesGrid.ItemsSource = appData.Dishes;
            ProductsComboBox.ItemsSource = appData.Products;
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

        private async void AddDish()
        {

            //var newDish = new Dish(RecipeNameTextBox.Text, DescriptionTextBox.Text, _currentDishProducts.ToList());

            var newDish = new Dish
            {
                Name = RecipeNameTextBox.Text,
                Recipe = DescriptionTextBox.Text,
                DishProducts = _currentDishProducts.Select(dp => new DishProduct
                {
                    ProductId = dp.ProductId,  // Только ID, без Product!
                    Weight = dp.Weight
                }).ToList()
            };

            var result = await appData.Client.SendRequestAsync<ServerCommonData.Response>("Add", "Dish", newDish);
            if (result.Success)
                await appData.GetAllData();
            else
                MessageBox.Show(result.Error);
        }
        private async void ModDish()
        {
            if (_currentDish == null)
                return;

            var result = await appData.Client.SendRequestAsync<ServerCommonData.Response>("Update", "Dish", _currentDish);
            if (result.Success)
                await appData.GetAllData();
            else
                MessageBox.Show(result.Error);
            
        }
        private async void DelDish()
        {
            if (_currentDish == null)
                return;

            var result = await appData.Client.SendRequestAsync<ServerCommonData.Response>("Delete", "Dish", _currentDish);
            if (result.Success)
                await appData.GetAllData();
            else
                MessageBox.Show(result.Error);
        }     

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddDish();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ModDish();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DelDish();
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
