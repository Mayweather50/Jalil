using Avalonia.Controls;
using MySQLConnect.ViewModel;

namespace MySQLConnect
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginWindowVM() { loginWindow = this};
        }
    }
}