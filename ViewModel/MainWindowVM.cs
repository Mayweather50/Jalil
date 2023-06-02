using Avalonia.Controls;
using MySQLConnect.Core;
using MySQLConnect.Model.Core;
using MySQLConnect.View;
using MySQLConnect.View.Add;
using MySQLConnect.View.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MySQLConnect.ViewModel
{
    public class MainWindowVM : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
#region Вывод инфы на таблицу
        private List<object> _dataGrid = new List<object>();
        public List<object> dataGrid
        {
            get { return _dataGrid; }
            set
            {
                _dataGrid = value;
                OnPropertyChanged("dataGrid");
            }
        }
        public ICommand ShowGrid
        {
            get
            {
                return new RelayCommand<ushort>(index =>
                {
                    dataGrid = MySqlHandler.TakeData(index);
                });
            }
        }
#endregion
#region Открытие диалоговых окон
        public ICommand AddRoom
        {
            get
            {
                return new ActionCommand(() =>
                {
                    AddRoom addRoom = new AddRoom();
                    addRoom.ShowDialog(Model.Config.rootWindow);
                });
            }
        }
        public ICommand AddGroup
        {
            get
            {
                return new ActionCommand(() =>
                {
                    AddGroup addGrp = new AddGroup();
                    addGrp.ShowDialog(Model.Config.rootWindow); // Отображение окна и ожидание его закрытия
                });
            }
        }
        public ICommand AddSpec
        {
            get
            {
                return new ActionCommand(() =>
                {
                    AddSpeciality addSpec = new AddSpeciality();
                    addSpec.ShowDialog(Model.Config.rootWindow);
                });
            }
        }
        public ICommand AddTchr
        {
            get
            {
                return new ActionCommand(() =>
                {
                    AddTeacher addTchr = new AddTeacher();
                    addTchr.ShowDialog(Model.Config.rootWindow);
                });
            }
        }
        public ICommand EditTime
        {
            get
            {
                return new ActionCommand(() =>
                {
                    BreaksWindow timeWin = new BreaksWindow();
                    timeWin.ShowDialog(Model.Config.rootWindow);
                });
            }
        }
#endregion
#region Чтение рабочей программы
        private string _testText;
        public string testText
        {
            get { return _testText; }
            set
            {
                _testText = value;
                OnPropertyChanged("testText");
            }
        }
        public ICommand LoadProgram
        {
            get
            {
                return new ActionCommand(async () =>
                {
                    testText = "";
                    foreach (string path in await Explorer.OpenExplorer(Model.Config.rootWindow, "docx"))
                    {
                        foreach(string line in WordReader.Read(path))
                        {
                            testText += line + "\n";
                        }
                    }
                });
            }
        }
        public ComboBoxItem selectGroup_Program { get; set; }
        public List<ComboBoxItem> grpList_Program
        {
            get
            {
                List<ComboBoxItem> grpData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.GroupsDropdwn()) grpData.Add(new ComboBoxItem() { Content = row });
                return grpData;
            }
        }
        #endregion
#region Распиание
        public ComboBoxItem selectGroup_TableTime { get; set; }
        public List<ComboBoxItem> grpList_TableTime // Выпадающий список групп
        {
            get
            {
                List<ComboBoxItem> grpData = new List<ComboBoxItem>();
                foreach (string row in MySqlHandler.GroupsDropdwn()) grpData.Add(new ComboBoxItem() { Content = row });
                return grpData;
            }
        }
        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                Tabletime.idChoosedGroup = MySqlHandler.GetGroupId(grpList_TableTime[value].Content.ToString());
                Tabletime.OutputWeek();
                OnPropertyChanged("SelectedIndex");
            }
        }
        public ICommand generateTabletime // Генерация расписания
        {
            get
            {
                return new RelayCommand<Grid>((grid) =>
                {
                    Tabletime.Start(grid);
                });
            }
        }
        public ICommand ExportTabletime
        {
            get
            {
                return new ActionCommand(() =>
                {
                    try
                    {
                        Tabletime.Export(selectGroup_TableTime.Content.ToString());
                    }
                    catch
                    {
                        Console.WriteLine("Export Failed!");
                        return;
                    }
                    Console.WriteLine("Export Successful!");
                });
            }
        }
        public ICommand ExportWeek
        {
            get
            {
                return new ActionCommand(() =>
                {
                    try
                    {
                        Tabletime.ExportWeek(selectGroup_TableTime.Content.ToString());
                    }
                    catch
                    {
                        Console.WriteLine("Week Export Failed!");
                        return;
                    }
                    Console.WriteLine("Week Export Successful!");
                });
            }
        }
        public ICommand startEditor // Запуск редактирования расписания
        {
            get
            {
                return new ActionCommand(() =>
                {
                    Editor editor = new Editor();
                    editor.ShowDialog(Model.Config.rootWindow);
                });
            }
        }
        public ICommand NextWeek
        {
            get
            {
                return new RelayCommand<Grid>((grid) =>
                {
                    Tabletime.NextWeek();
                });
            }
        }
        public ICommand PreviousWeek
        {
            get
            {
                return new RelayCommand<Grid>((grid) =>
                {
                    Tabletime.PreviousWeek();
                });
            }
        }
        #endregion
    }
}
