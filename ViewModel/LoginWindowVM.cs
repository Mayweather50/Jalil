using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using MySQLConnect.View;
using MySQLConnect.View.Config;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel
{
    public class LoginWindowVM : INotifyPropertyChanged
    {
        public LoginWindow loginWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string? IP { get; set; }
        public string? Port { get; set; }
        public string? Db { get; set; }
        public string? Name { get; set; }
        public string? Pass { get; set; }

        public ICommand ConnectClick
        {
            get
            {
                return new ActionCommand(() =>
                {
                    //if(IP == null || Port == null || Db == null || Name == null || Pass == null)
                    //{
                    //    Model.Config.CallNotification(loginWindow, "Не все поля заполнены!");
                    //    return;
                    //}
                    //string connStr = $"Server={IP};Port={Port};Database={Db};Uid={Name};Pwd={Pass};";
                    string connStr = "Server=127.0.0.1;Port=3306;Database=tabletimedb;Uid=root;Pwd=root;";
                    Model.Config.LoadData();
                    if (MySqlHandler.Connect(connStr))
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        loginWindow.Close();
                    }
                    else Model.Config.CallNotification(loginWindow);
                });
            }
        }
    }
}
