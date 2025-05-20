using CommonData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_CookBookApp;

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
