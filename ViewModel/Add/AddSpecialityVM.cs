using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel.Add
{
    public class AddSpecialityVM :INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public ICommand AddSpec
        {
            get
            {
                return new ActionCommand(() =>
                {
                    if (!MySqlHandler.CheckDupl("specialities", "code", Code)) MySqlHandler.WriteSpec(Code, Name);
                });
            }
        }
    }
}
