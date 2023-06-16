using Avalonia.Controls;
using MySQLConnect.ViewModel.Add;

namespace MySQLConnect.View.Add;

public partial class LoadPlan : Window
{
    public LoadPlan()
    {
        InitializeComponent();
        DataContext = new LoadPlanVM() { thisWindow = this };
    }
}