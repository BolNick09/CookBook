using CommonData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Client_CookBookApp
{
    public partial class CalorieCalculatorWindow : Window
    {
        private readonly BindingList<CalculatedProduct> _selectedProducts = new();

        public CalorieCalculatorWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => await InitializeProductsAsync();
            SetupControls();
        }

        private async Task InitializeProductsAsync()
        {
            try
            {
                if (!AppData.Products.Any())
                    await AppData.InitializeAsync();

                ProductsComboBox.ItemsSource = AppData.Products;
                ProductsComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продуктов: {ex.Message}");
            }
        }

        private void SetupControls()
        {
            SelectedProductsGrid.ItemsSource = _selectedProducts;
            _selectedProducts.ListChanged += (s, e) => UpdateTotals();
        }

        private void UpdateTotals()
        {
            var totals = new
            {
                Calories = _selectedProducts.Sum(p => p.Calories),
                Proteins = _selectedProducts.Sum(p => p.Proteins),
                Fats = _selectedProducts.Sum(p => p.Fats),
                Carbs = _selectedProducts.Sum(p => p.Carbohydrates)
            };

            TotalCaloriesLabel.Content = totals.Calories.ToString("N1");
            TotalProteinsLabel.Content = totals.Proteins.ToString("N1");
            TotalFatsLabel.Content = totals.Fats.ToString("N1");
            TotalCarbsLabel.Content = totals.Carbs.ToString("N1");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsComboBox.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите продукт из списка", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(WeightTextBox.Text, out double weight) || weight <= 0)
            {
                MessageBox.Show("Введите корректный вес (положительное число)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedProducts.Add(new CalculatedProduct(product, weight));
            WeightTextBox.Clear();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedProducts.Clear();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}