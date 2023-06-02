using Avalonia.Controls;
using MySQLConnect.ViewModel.Add;

namespace MySQLConnect.View.Add
{
    public partial class AddSpeciality : Window
    {
        public AddSpeciality()
        {
            InitializeComponent();
            DataContext = new AddSpecialityVM() { thisWindow = this };
        }
    }
}
