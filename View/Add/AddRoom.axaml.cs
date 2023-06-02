using Avalonia.Controls;
using MySQLConnect.ViewModel.Add;

namespace MySQLConnect.View
{
    public partial class AddRoom : Window
    {
        public AddRoom()
        {
            InitializeComponent();
            DataContext = new AddRoomVM() { thisWindow = this };
        }
    }
}
