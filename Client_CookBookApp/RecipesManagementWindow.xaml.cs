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

            // Инициализация элементов управления
            RecipesGrid.ItemsSource = AppData.Dishes;
            ProductsComboBox.ItemsSource = AppData.Products;
            ProductsComboBox.DisplayMemberPath = "Name";
            IngredientsListBox.ItemsSource = _currentDishProducts;
            IngredientsListBox.DisplayMemberPath = "Product.Name";

            // Сброс формы
            ResetForm();
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RecipeNameTextBox.Text))
            {
                MessageBox.Show("Введите название блюда", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentDishProducts.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один ингредиент", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentDish == null)
                _currentDish = new Dish();

            _currentDish.Name = RecipeNameTextBox.Text;
            _currentDish.Recipe = DescriptionTextBox.Text;

            using var db = new CookBookDbContext();

            if (_currentDish.Id == 0)
            {
                db.Dishes.Add(_currentDish);
                db.SaveChanges(); // Получаем Id
                AppData.Dishes.Add(_currentDish);
            }
            else
            {
                db.Dishes.Update(_currentDish);
                db.SaveChanges();
            }

            if (_currentDish.Id != 0)
            {
                var existingProducts = db.DishProducts.Where(dp => dp.DishId == _currentDish.Id).ToList();
                db.DishProducts.RemoveRange(existingProducts);
                db.SaveChanges();
            }

            foreach (var dp in _currentDishProducts)
            {
                db.DishProducts.Add(new DishProduct
                {
                    DishId = _currentDish.Id,
                    ProductId = dp.ProductId,
                    Weight = dp.Weight
                });
            }

            db.SaveChanges();

            // Обновляем связи в объекте
            _currentDish.DishProducts = _currentDishProducts.ToList();

            // Оповещаем UI об изменениях
            OnPropertyChanged(nameof(RecipesGrid)); // или обновите через Items.Refresh()

            RecipesGrid.Items.Refresh();
            ResetForm();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите блюдо для редактирования", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDish = (Dish)RecipesGrid.SelectedItem;
            _currentDish = selectedDish;
            _currentDishProducts.Clear();

            foreach (var dishProduct in selectedDish.DishProducts)
            {
                _currentDishProducts.Add(new DishProduct
                {
                    DishId = dishProduct.DishId,
                    ProductId = dishProduct.ProductId,
                    Product = dishProduct.Product,
                    Weight = dishProduct.Weight
                });
            }

            RecipeNameTextBox.Text = _currentDish.Name;
            DescriptionTextBox.Text = _currentDish.Recipe;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите блюдо для удаления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDish = (Dish)RecipesGrid.SelectedItem;

            var result = MessageBox.Show("Вы уверены, что хотите удалить это блюдо?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using var db = new CookBookDbContext();
                var dishToRemove = db.Dishes
                    .Include(d => d.DishProducts)
                    .FirstOrDefault(d => d.Id == selectedDish.Id);

                if (dishToRemove != null)
                {
                    db.Dishes.Remove(dishToRemove);
                    db.SaveChanges();
                }

                AppData.Dishes.Remove(selectedDish);

                if (_currentDish != null && _currentDish.Id == selectedDish.Id)
                {
                    ResetForm();
                }
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
