using Avalonia.Controls;
using MySQLConnect.Model.Core;
using MySQLConnect.ViewModel;

namespace MySQLConnect.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Model.Config.rootWindow = this;
            DataContext = new MainWindowVM();
            this.Opened += (s, e) =>
            {
                Tabletime.JsonLoad(this);
            };
            this.Closing += (s, e) => // Отключение от сервера
            {
                Tabletime.JsonSave();
                MySqlHandler.conn.Close();
            };
        }
    }
}
