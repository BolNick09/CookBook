using CommonData;
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

namespace Client_CookBookApp
{
    /// <summary>
    /// Interaction logic for CombinedSearchWindow.xaml
    /// </summary>
    public partial class CombinedSearchWindow : Window, INotifyPropertyChanged
    {
        // Для блюд
        private ObservableCollection<Dish> _filteredDishes = new();
        public ObservableCollection<Dish> FilteredDishes
        {
            get => _filteredDishes;
            set
            {
                _filteredDishes = value;
                OnPropertyChanged(nameof(FilteredDishes));
            }
        }

        public ObservableCollection<TagItemViewModel> DishTags { get; set; } = new();

        // Для продуктов
        private ObservableCollection<Product> _filteredProducts = new();
        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set
            {
                _filteredProducts = value;
                OnPropertyChanged(nameof(FilteredProducts));
            }
        }

        // Singleton для доступа из TagItemViewModel
        public static CombinedSearchWindow Instance { get; private set; }

        public CombinedSearchWindow()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;

            InitializeDishFilters();
            InitializeProductFilters();
        }

        #region Блюда

        private void InitializeDishFilters()
        {
            // Инициализация тегов
            var tags = AppData.Tags.Select(t => new TagItemViewModel { Tag = t }).ToList();
            DishTags = new ObservableCollection<TagItemViewModel>(tags);

            // Начальная загрузка всех блюд
            FilteredDishes = new ObservableCollection<Dish>(AppData.Dishes);
        }


        public void ApplyDishesFilters()
        {
            var query = AppData.Dishes.AsEnumerable();

            // Фильтр по названию
            if (!string.IsNullOrWhiteSpace(txtDishSearch.Text))
            {
                string searchText = txtDishSearch.Text.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(searchText));
            }

            // Фильтр по тегам
            var selectedTagIds = DishTags
                .Where(vm => vm.IsSelected)
                .Select(vm => vm.Tag.Id)
                .ToList();

            if (selectedTagIds.Count > 0)
            {
                query = query.Where(d =>
                    d.DishTags.Any(dt => selectedTagIds.Contains(dt.TagId)));
            }

            FilteredDishes = new ObservableCollection<Dish>(query);
        }

        private void DishTagFilter_Click(object sender, RoutedEventArgs e)
        {
            ApplyDishesFilters();
        }

        #endregion

        #region Продукты

        private void InitializeProductFilters()
        {
            FilteredProducts = new ObservableCollection<Product>(AppData.Products);
        }

        private void ApplyProductFilters()
        {
            var query = AppData.Products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtProductSearch.Text))
            {
                string searchText = txtProductSearch.Text.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchText));
            }

            FilteredProducts = new ObservableCollection<Product>(query);
        }

        private void ProductResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtProductSearch.Text = "";
            ApplyProductFilters();
        }

        #endregion

        #region Обработчики событий

        private void txtProductSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyProductFilters();
        private void txtDishesSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyDishesFilters();

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
