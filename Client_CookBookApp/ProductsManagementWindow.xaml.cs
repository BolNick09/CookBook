using CommonData;
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

        // Загрузка данных из AppData
        ProductsGrid.ItemsSource = AppData.Products;
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

    private void SaveProduct_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput()) return;

        using var db = new CookBookDbContext();

        try
        {
            if (_selectedProduct == null)
            {
                // Добавление нового продукта
                var product = new Product
                {
                    Name = ProductNameTextBox.Text,
                    Calories = decimal.Parse(CaloriesTextBox.Text),
                    Proteins = decimal.Parse(ProteinsTextBox.Text),
                    Fats = decimal.Parse(FatsTextBox.Text),
                    Carbohydrates = decimal.Parse(CarbsTextBox.Text)
                };

                db.Products.Add(product);
                db.SaveChanges();

                // Добавляем в AppData
                AppData.Products.Add(product);
            }
            else
            {
                // Редактирование существующего продукта
                var existingProduct = db.Products.Find(_selectedProduct.Id);
                if (existingProduct != null)
                {
                    existingProduct.Name = ProductNameTextBox.Text;
                    existingProduct.Calories = decimal.Parse(CaloriesTextBox.Text);
                    existingProduct.Proteins = decimal.Parse(ProteinsTextBox.Text);
                    existingProduct.Fats = decimal.Parse(FatsTextBox.Text);
                    existingProduct.Carbohydrates = decimal.Parse(CarbsTextBox.Text);

                    db.Products.Update(existingProduct);
                    db.SaveChanges();

                    // Обновляем в AppData
                    var appProduct = AppData.Products.FirstOrDefault(p => p.Id == existingProduct.Id);
                    if (appProduct != null)
                    {
                        appProduct.Name = existingProduct.Name;
                        appProduct.Calories = existingProduct.Calories;
                        appProduct.Proteins = existingProduct.Proteins;
                        appProduct.Fats = existingProduct.Fats;
                        appProduct.Carbohydrates = existingProduct.Carbohydrates;
                    }
                }
            }

            // Обновляем DataGrid
            ProductsGrid.Items.Refresh();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

    private void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsGrid.SelectedItem == null)
        {
            MessageBox.Show("Выберите продукт для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedProduct = (Product)ProductsGrid.SelectedItem;

        var result = MessageBox.Show("Вы уверены, что хотите удалить этот продукт?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            using var db = new CookBookDbContext();

            var productToDelete = db.Products.Find(selectedProduct.Id);
            if (productToDelete != null)
            {
                db.Products.Remove(productToDelete);
                db.SaveChanges();
            }

            // Удаляем из AppData
            var appProduct = AppData.Products.FirstOrDefault(p => p.Id == selectedProduct.Id);
            if (appProduct != null)
            {
                AppData.Products.Remove(appProduct);
            }

            // Обновляем DataGrid
            ProductsGrid.Items.Refresh();
            ClearForm();
        }
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
    }
}
