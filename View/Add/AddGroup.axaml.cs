using Avalonia.Controls;
using MySQLConnect.ViewModel.Add;

namespace MySQLConnect.View
{
    public partial class AddGroup : Window
    {
        public AddGroup()
        {
            InitializeComponent();
            DataContext = new AddGroupVM() { thisWindow = this };
        }
    }
}
