using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MySQLConnect.ViewModel.Config
{
    public class NotificationVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private string _notifText = "Sample text";
        public string NotifText
        {
            get { return _notifText; }
            set
            {
                _notifText = value;
                OnPropertyChanged("NotifText");
            }
        }
    }
}
