using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CommonData;

namespace CookBookClient
{
    public partial class CalorieCalculatorWindow : Window
    {
        private List<Product> _availableProducts = new List<Product>();
        private BindingList<CalculatedProduct> _selectedProducts = new BindingList<CalculatedProduct>();
        public CalorieCalculatorWindow()
        {
            InitializeComponent();
            InitializeSampleProducts();
            SetupControls();
        }

        private void InitializeSampleProducts()
        {
            foreach (var product in AppData.Products)
            {
                _availableProducts.Add(product);
            }
       
        }

        private void SetupControls()
        {
            ProductsComboBox.ItemsSource = _availableProducts;
            ProductsComboBox.DisplayMemberPath = "Name";
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
            if (ProductsComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт из списка", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(WeightTextBox.Text, out double weight) || weight <= 0)
            {
                MessageBox.Show("Введите корректный вес продукта (положительное число)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = (Product)ProductsComboBox.SelectedItem;
            _selectedProducts.Add(new CalculatedProduct
            {
                Product = selectedProduct,
                Name = selectedProduct.Name,
                Grams = weight,
                Calories = (double)(selectedProduct.Calories * (decimal)weight / 100),
                Proteins = (double)(selectedProduct.Proteins * (decimal)weight / 100),
                Fats = (double)(selectedProduct.Fats * (decimal)weight / 100),
                Carbohydrates = (double)(selectedProduct.Carbohydrates * (decimal)weight / 100)
            });

            WeightTextBox.Clear();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedProducts.Clear();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTotals();
        }
    }
}