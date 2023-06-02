using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static MySQLConnect.Model.Core.CSVReader;

namespace MySQLConnect.ViewModel.Add
{
    public class AddGroupVM : INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string GrpName { get; set; }
        public ComboBoxItem selectSpec { get; set; }
        public double Count { get; set; } = 5;
        public ComboBoxItem selectType { get; set; }

        public List<ComboBoxItem> specList
        {
            get
            {
                List<ComboBoxItem> specData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.SpecDropdwn()) specData.Add(new ComboBoxItem() { Content = row });
                return specData;
            }
        }
        public List<ComboBoxItem> typeList
        {
            get
            {
                List<ComboBoxItem> typeData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.TypeDropdwn("_grouptypes")) typeData.Add(new ComboBoxItem() { Content = row });
                return typeData;
            }
        }
        #region План
        private string path;
        public string? FilePath
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged("FilePath");
            }
        }
        public ICommand ChooseFile
        {
            get
            {
                return new RelayCommand<Window>(async obj =>
                {
                    foreach (string path in await Explorer.OpenExplorer(thisWindow, "csv")) { FilePath = path; }
                });
            }
        }
        #endregion
        public ICommand AddGroup
        {
            get
            {
                return new ActionCommand(() =>
                {
                    if(!MySqlHandler.CheckDupl("_groups", "name", GrpName)) MySqlHandler.WriteGroup(GrpName, selectSpec.Content.ToString(), (int)Count, selectType.Content.ToString());
                    bool spec;
                    if (selectType.Content.ToString().StartsWith("Специалист")) spec = true;
                    else spec = false;
                    MySqlHandler.WritePlan(GrpName, Read(spec, FilePath,1));
                    thisWindow.Close();
                });
            }
        }
    }
}
