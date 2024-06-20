using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using System;
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
                foreach (string row in MySqlHandler.TypeDropdwn("roomtypes")) typeData.Add(new ComboBoxItem() { Content = row });
                return typeData;
            }
        }
        public ICommand AddRoom
        {
            get
            {
                return new ActionCommand(() =>
                {
                    Console.WriteLine("AddRoom command is called");
                    if (!MySqlHandler.CheckDupl("rooms", "number", Number)) MySqlHandler.WriteRoom(Name, Number, (string)selectItem.Content);
                    thisWindow.Close();
                });
            }
        }
    }
}
