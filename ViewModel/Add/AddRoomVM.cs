using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel.Add
{
    public class AddRoomVM : INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public string Name { get; set; }
        public ushort Number { get; set; } = 5;
        public ComboBoxItem selectItem { get; set; }
        public List<ComboBoxItem> typeList
        {
            get
            {
                List<ComboBoxItem> typeData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.TypeDropdwn("_roomtypes")) typeData.Add(new ComboBoxItem() { Content = row });
                return typeData;
            }
        }
        public ICommand AddRoom
        {
            get
            {
                return new ActionCommand(() =>
                {
                    if(!MySqlHandler.CheckDupl("_rooms", "number", Number)) MySqlHandler.WriteRoom(Name, Number, (string)selectItem.Content);
                    thisWindow.Close();
                });
            }
        }
    }
}
