using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_CookBookApp;


public partial class MainWindow : Window
{
    public AppData appData = new AppData();
    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (s, e) => await appData.InitializeAsync("127.0.0.1", 2024);
    }

    private void OpenSearchRecipes_Click(object sender, RoutedEventArgs e)
    {
        var searchWindow = new SearchRecipesWindow();
        searchWindow.Owner = this;
        searchWindow.Show();
    }

    private void OpenRecipesManagement_Click(object sender, RoutedEventArgs e)
    {
        var recipesWindow = new RecipesManagementWindow(this);
        recipesWindow.Show();
    }

    private void OpenProductsManagement_Click(object sender, RoutedEventArgs e)
    {
        var productsWindow = new ProductsManagementWindow(this);
        productsWindow.Show();
    }

    private void OpenCalorieCalculator_Click(object sender, RoutedEventArgs e)
    {
        var calculatorWindow = new CalorieCalculatorWindow(this);
        calculatorWindow.Show();
    }

    private void favorite_Click(object sender, RoutedEventArgs e)
    {
        var favoriteWindow = new Favorite(this);
        favoriteWindow.Show();
    }

    private void OpenProductsSearch_Click(object sender, RoutedEventArgs e)
    {

        var window = new CombinedSearchWindow(this);
        window.Show();

    }
}