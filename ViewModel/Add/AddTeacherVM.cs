using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model;
using MySQLConnect.Model.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel.Add
{
    public class AddTeacherVM : INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #region Необходимые даные
        public string FullName { get; set; }
        public ComboBoxItem selectType { get; set; }
        public ComboBoxItem selectState { get; set; }
        public double TimeJob { get; set; } = 0.1;
        public ComboBoxItem selectGroup { get; set; }
        public ComboBoxItem selectSubj { get; set; }
        #endregion

        public List<ComboBoxItem> stateList
        {
            get
            {
                List<ComboBoxItem> stateData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.TchrStateDropdwn()) stateData.Add(new ComboBoxItem() { Content = row });
                return stateData;
            }
        }
        public List<ComboBoxItem> grpList
        {
            get
            {
                List<ComboBoxItem> grpData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.GroupsDropdwn()) grpData.Add(new ComboBoxItem() { Content = row });
                return grpData;
            }
        }
        public List<ComboBoxItem> subjList
        {
            get
            {
                List<ComboBoxItem> subjData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.SubjDropdwn()) subjData.Add(new ComboBoxItem() { Content = row });
                return subjData;
            }
        }
        private int _Buttons = 7;
        public int Buttons
        {
            get { return _Buttons; }
            set
            {
                _Buttons = value;
                OnPropertyChanged("Buttons");
            }
        }
        private int _AddTchrRow = 8;
        public int AddTchrRow
        {
            get { return _AddTchrRow; }
            set
            {
                _AddTchrRow = value;
                OnPropertyChanged("AddTchrRow");
            }
        }
        public Grid rootGrid { get; set; }

        public ICommand AddTchr
        {
            get
            {
                return new ActionCommand(() =>
                {
                    if (!MySqlHandler.CheckDupl("teachers", "fullname", FullName))
                        MySqlHandler.WriteTeacher(FullName, (string)selectState.Content, TimeJob);
                });
            }
        }
        public ICommand AddRow // Добавляет новый Dropdown предметов
        {
            get
            {
                return new RelayCommand<Grid>(grid =>
                {
                    var a = CRUDRows.AddRow(thisWindow, grid, subjList);
                    if(a != null)
                    {
                        Buttons = a[0];
                        AddTchrRow = a[1];
                    }
                });
            }
        }
        public ICommand RemoveRow // Удаляет Dropdown предметов
        {
            get
            {
                return new RelayCommand<Grid>(grid =>
                {
                    var a = CRUDRows.RemoveRow(thisWindow, grid);
                    if (a != null)
                    {
                        Buttons = a[0];
                        AddTchrRow = a[1];
                    }
                });
            }
        }
    }
}
