using System.ComponentModel;
using CommonData;

namespace CookBookClient
{
    public class SelectableProduct : INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public Product? Product
        {
            get;

            set
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name => Product?.Name ?? string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    



 
}