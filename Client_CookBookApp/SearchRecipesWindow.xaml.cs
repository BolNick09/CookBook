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
using CommonData;

namespace Client_CookBookApp;

/// <summary>
/// Interaction logic for SearchRecipesWindow.xaml
/// </summary>
public partial class SearchRecipesWindow : Window
{
    private ObservableCollection<SelectableProduct> _availableProducts = new ObservableCollection<SelectableProduct>();
    private ObservableCollection<Dish> _allDishes = AppData.Dishes; // Используем данные из AppData
    private ObservableCollection<DishDisplay> _matchingRecipes = new ObservableCollection<DishDisplay>();

    public SearchRecipesWindow()
    {
        InitializeComponent();
        InitializeSampleData();

        AvailableProductsGrid.ItemsSource = _availableProducts;
        MatchingRecipesGrid.ItemsSource = _matchingRecipes;
    }

    private void InitializeSampleData()
    {
        // Заполняем доступные продукты из AppData
        foreach (var product in AppData.Products)
        {
            _availableProducts.Add(new SelectableProduct { Product = product });
        }
    }

    private void FindRecipesButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedProductIds = _availableProducts
            .Where(p => p.IsSelected)
            .Select(p => p.Product.Id)
            .ToList();

        if (selectedProductIds.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы один продукт", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var matchingDishes = _allDishes
            .Where(dish => dish.DishProducts.All(dp => selectedProductIds.Contains(dp.Product.Id)))
            .Select(dish => new DishDisplay
            {
                Name = dish.Name,
                RequiredProducts = string.Join(", ", dish.DishProducts.Select(dp => $"{dp.Product.Name} ({dp.Weight}г)"))
            })
            .ToList();

        _matchingRecipes.Clear();
        foreach (var recipe in matchingDishes)
        {
            _matchingRecipes.Add(recipe);
        }
    }
}