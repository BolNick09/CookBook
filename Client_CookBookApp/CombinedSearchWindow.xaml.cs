using CommonData;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Client_CookBookApp
{
    public partial class CombinedSearchWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Dish> _filteredDishes = new();
        private ObservableCollection<Product> _filteredProducts = new();

        public ObservableCollection<Dish> FilteredDishes
        {
            get => _filteredDishes;
            set { _filteredDishes = value; OnPropertyChanged(nameof(FilteredDishes)); }
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(nameof(FilteredProducts)); }
        }

        public ObservableCollection<TagItemViewModel> DishTags { get; } = new();

        public CombinedSearchWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += CombinedSearchWindow_Loaded;
        }

        private async void CombinedSearchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await AppData.InitializeAsync();
                InitializeFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void InitializeFilters()
        {
            // Инициализация тегов
            DishTags.Clear();
            foreach (var tag in AppData.Tags)
            {
                DishTags.Add(new TagItemViewModel { Tag = tag });
            }

            // Первоначальная фильтрация
            ApplyAllFilters();
        }

        private void ApplyAllFilters()
        {
            ApplyDishesFilters();
            ApplyProductFilters();
        }

        public void ApplyDishesFilters()
        {
            var query = AppData.Dishes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtDishSearch.Text))
            {
                var searchText = txtDishSearch.Text.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(searchText));
            }

            var selectedTags = DishTags.Where(t => t.IsSelected).Select(t => t.Tag.Id);
            if (selectedTags.Any())
            {
                query = query.Where(d => d.DishTags.Any(dt => selectedTags.Contains(dt.TagId)));
            }

            FilteredDishes = new ObservableCollection<Dish>(query);
        }

        private void ApplyProductFilters()
        {
            var query = AppData.Products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtProductSearch.Text))
            {
                var searchText = txtProductSearch.Text.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchText));
            }

            FilteredProducts = new ObservableCollection<Product>(query);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppData.InitializeAsync().Wait();
                ApplyAllFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}");
            }
        }

        private void txtProductSearch_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyProductFilters();

        private void txtDishesSearch_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyDishesFilters();

        private void ProductResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtProductSearch.Text = string.Empty;
            ApplyProductFilters();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}