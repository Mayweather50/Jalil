using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using MySQLConnect.ViewModel.Config;

namespace MySQLConnect.View.Config;

public partial class Notification : Window
{
    public NotificationVM data = new NotificationVM();
    public Notification()
    {
        InitializeComponent();
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.CanResize = false;
        DataContext = data;
    }
}