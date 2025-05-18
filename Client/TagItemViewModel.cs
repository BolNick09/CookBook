using CommonData;
using System.ComponentModel;

namespace CookBookClient
{
    public class TagItemViewModel : INotifyPropertyChanged
    {
        public Tag Tag { get; set; }

        public bool IsSelected
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(IsSelected));
                    CombinedSearchWindow.Instance?.ApplyDishesFilters();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}