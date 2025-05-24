using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class Dish : INotifyPropertyChanged
    {
        private bool _isFavorite;

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Recipe { get; set; } = null!;
        public string? ImagePath { get; set; }

        public List<DishTag> DishTags { get; set; }
        public List<DishProduct>? DishProducts { get;  set; }

        public Dish() { }

        public Dish(string name, string recipe, List<DishProduct>? dishProducts)
        {
            Name = name;
            Recipe = recipe;
            DishTags = new();
            DishProducts = dishProducts ?? new List<DishProduct>(); 
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
