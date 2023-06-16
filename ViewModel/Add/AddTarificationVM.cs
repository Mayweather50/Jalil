using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel.Add
{
    public class AddTarificationVM : INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private string _path;
        public string FilePath
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("FilePath");
            }
        }
        private bool _budjet;
        public bool Budjet
        {
            get { return _budjet; }
            set
            {
                _budjet = value;
                OnPropertyChanged("Budjet");
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
                    else foreach(string path in responce) FilePath = path;
                });
            }
        }
        public ICommand LoadTarification
        {
            get
            {
                return new ActionCommand(() =>
                {
                    Tarification.Read(_path, _budjet);
                    thisWindow.Close();
                });
            }
        }
    }
}
