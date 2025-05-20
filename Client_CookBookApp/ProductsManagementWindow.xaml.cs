using CommonData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Client_CookBookApp;

/// <summary>
/// Interaction logic for ProductsManagementWindow.xaml
/// </summary>
public partial class ProductsManagementWindow : Window
{
    private Product _selectedProduct;

    public ProductsManagementWindow()
    {
        InitializeComponent();
        ProductsGrid.ItemsSource = AppData.Products;
        LoadProducts();

    }

    // Очистка формы ввода
    private void ClearForm()
    {
        ProductNameTextBox.Clear();
        CaloriesTextBox.Clear();
        ProteinsTextBox.Clear();
        FatsTextBox.Clear();
        CarbsTextBox.Clear();
        _selectedProduct = null;
    }

    // Валидация введенных данных
    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
        {
            MessageBox.Show("Введите название продукта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!decimal.TryParse(CaloriesTextBox.Text, out _) ||
            !decimal.TryParse(ProteinsTextBox.Text, out _) ||
            !decimal.TryParse(FatsTextBox.Text, out _) ||
            !decimal.TryParse(CarbsTextBox.Text, out _))
        {
            MessageBox.Show("Поля калорий и БЖУ должны содержать числовые значения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }


    private void AddProduct_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
    }

    private void EditBtn_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsGrid.SelectedItem == null)
        {
            MessageBox.Show("Выберите продукт для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _selectedProduct = (Product)ProductsGrid.SelectedItem;

        // Заполняем форму данными выбранного продукта
        ProductNameTextBox.Text = _selectedProduct.Name;
        CaloriesTextBox.Text = _selectedProduct.Calories.ToString();
        ProteinsTextBox.Text = _selectedProduct.Proteins.ToString();
        FatsTextBox.Text = _selectedProduct.Fats.ToString();
        CarbsTextBox.Text = _selectedProduct.Carbohydrates.ToString();
    }

    private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsGrid.SelectedItem is not Product selectedProduct)
        {
            MessageBox.Show("Выберите продукт!");
            return;
        }

        var confirm = MessageBox.Show("Удалить продукт?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            var result = await AppData.Client.SendRequest<bool>("Delete", "Product", selectedProduct.Id);
            if (result)
            {
                AppData.Products.Remove(selectedProduct);
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка удаления: {ex.Message}");
        }
    }

    private async void LoadProducts()
    {
        try
        {
            await AppData.InitializeAsync(); // Используем общую инициализацию
            ProductsGrid.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки: {ex.Message}");
        }
    }

    private async void SaveProduct_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput()) return;

        try
        {
            var product = new Product
            {
                Id = _selectedProduct?.Id ?? 0,
                Name = ProductNameTextBox.Text,
                Calories = decimal.Parse(CaloriesTextBox.Text),
                Proteins = decimal.Parse(ProteinsTextBox.Text),
                Fats = decimal.Parse(FatsTextBox.Text),
                Carbohydrates = decimal.Parse(CarbsTextBox.Text)
            };

            var action = _selectedProduct == null ? "Add" : "Update";
            var result = await AppData.Client.SendRequest<bool>(action, "Product", product);

            if (result)
            {
                await AppData.InitializeAsync();
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
        }
    }
    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
    }
}
