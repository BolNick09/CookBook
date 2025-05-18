using System.Windows;
using System.Windows.Controls;

namespace CookBookClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenSearchRecipes_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchRecipesWindow();
            searchWindow.Owner = this;
            searchWindow.Show();
        }

        private void OpenRecipesManagement_Click(object sender, RoutedEventArgs e)
        {
            var recipesWindow = new RecipesManagementWindow();
            recipesWindow.Owner = this;
            recipesWindow.Show();
        }

        private void OpenProductsManagement_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductsManagementWindow();
            productsWindow.Owner = this;
            productsWindow.Show();
        }

        private void OpenCalorieCalculator_Click(object sender, RoutedEventArgs e)
        {
            var calculatorWindow = new CalorieCalculatorWindow();
            calculatorWindow.Owner = this;
            calculatorWindow.Show();
        }

        private void favorite_Click(object sender, RoutedEventArgs e)
        {
            var favoriteWindow = new Favorite();
            favoriteWindow.Owner = this;
            favoriteWindow.Show();
        }

        private void OpenProductsSearch_Click(object sender, RoutedEventArgs e)
        {
            
            var window = new CombinedSearchWindow();
            window.Owner = this;
            window.Show();
        
    }
    }
}