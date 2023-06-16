using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel.Add
{
    public class LoadPlanVM :  INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private string path;
        public string PathToFile
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged("PathToFile");
            }
        }
        public ComboBoxItem selectGroup { get; set; }
        public List<ComboBoxItem> grpList // Выпадающий список групп
        {
            get
            {
                List<ComboBoxItem> grpData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.GroupsDropdwn()) grpData.Add(new ComboBoxItem() { Content = row });
                return grpData;
            }
        }
        public ICommand ChooseFile
        {
            get
            {
                return new RelayCommand<Window>(async obj =>
                {
                    string[] responce = await Explorer.OpenExplorer(thisWindow, "csv");
                    if (responce == null) return;
                    else foreach (string path in responce) PathToFile = path;
                });
            }
        }
        public ICommand Load
        {
            get
            {
                return new ActionCommand(() =>
                {
                    CSVReader.Read(path, selectGroup.Content.ToString());
                    thisWindow.Close();
                });
            }
        }
    }
}
