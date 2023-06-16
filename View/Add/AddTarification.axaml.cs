using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MySQLConnect.ViewModel.Add;

namespace MySQLConnect.View.Add;

public partial class AddTarification : Window
{
    public AddTarification()
    {
        InitializeComponent();
        DataContext = new AddTarificationVM() { thisWindow = this };
    }
}