using Avalonia.Controls;
using MySQLConnect.ViewModel.Add;

namespace MySQLConnect.View.Add
{
    public partial class AddTeacher : Window
    {
        public AddTeacher()
        {
            InitializeComponent();
            DataContext = new AddTeacherVM() { thisWindow = this };
        }
    }
}
