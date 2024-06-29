using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using MySQLConnect.MySQLConnect;
using MySQLConnect.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.View
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
       

      

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();     
            DataContext = new MainWindowViewModel();
        }
    }
}
