using System.ComponentModel;

namespace CookBookClient;

public class DishDisplay : INotifyPropertyChanged
{
    private string _name;
    private string _requiredProducts;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string RequiredProducts
    {
        get => _requiredProducts;
        set
        {
            _requiredProducts = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}