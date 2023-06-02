using Avalonia.Controls;
using MySQLConnect.ViewModel.Config;

namespace MySQLConnect.View.Config
{
    public partial class BreaksWindow : Window
    {
        public BreaksWindow()
        {
            InitializeComponent();
            DataContext = new BreaksWindowVM() { thisWindow = this };
        }
    }
}
