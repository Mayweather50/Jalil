using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLConnect
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    namespace MySQLConnect
    {
        public class MainWindowViewModel : INotifyPropertyChanged
        {
            private ObservableCollection<ScheduleItem> _scheduleItems;
            private ScheduleItem _selectedScheduleItem;

            public ObservableCollection<ScheduleItem> ScheduleItems
            {
                get { return _scheduleItems; }
                set { _scheduleItems = value; OnPropertyChanged(); }
            }

            public ScheduleItem SelectedScheduleItem
            {
                get { return _selectedScheduleItem; }
                set { _selectedScheduleItem = value; OnPropertyChanged(); }
            }

            public ICommand GenerateTabletimeCommand { get; private set; }

            public MainWindowViewModel()
            {
                ScheduleItems = new ObservableCollection<ScheduleItem>();
                GenerateTabletimeCommand = new RelayCommand(GenerateTabletime);
                GenerateTabletime();
            }

            private void GenerateTabletime()
            {
                ScheduleItems.Clear();
                ScheduleItems.Add(new ScheduleItem
                {
                    Monday = "Алгебра и геометрия",
                    MondayTime = "9:00 - 10:30",
                    MondayRoom = "Ауд. 101",
                    Tuesday = "Дискретная математика",
                    TuesdayTime = "10:45 - 12:15",
                    TuesdayRoom = "Ауд. 202",
                    Wednesday = "Программирование на Python",
                    WednesdayTime = "12:30 - 14:00",
                    WednesdayRoom = "Ауд. 303",
                    Thursday = "Теория информации",
                    ThursdayTime = "14:15 - 15:45",
                    ThursdayRoom = "Ауд. 404",
                    Friday = "Алгоритмы и структуры данных",
                    FridayTime = "16:00 - 17:30",
                    FridayRoom = "Ауд. 505",
                    Saturday = "Физика",
                    SaturdayTime = "9:00 - 10:30",
                    SaturdayRoom = "Ауд. 606"
                });
                ScheduleItems.Add(new ScheduleItem
                {
                    Monday = "Базы данных",
                    MondayTime = "10:45 - 12:15",
                    MondayRoom = "Ауд. 107",
                    Tuesday = "Физическая культура",
                    TuesdayTime = "12:30 - 14:00",
                    TuesdayRoom = "Спортзал",
                    Wednesday = "Сетевые технологии",
                    WednesdayTime = "14:15 - 15:45",
                    WednesdayRoom = "Ауд. 207",
                    Thursday = "Операционные системы",
                    ThursdayTime = "16:00 - 17:30",
                    ThursdayRoom = "Ауд. 307",
                    Friday = "Объектно-ориентированное программирование",
                    FridayTime = "9:00 - 10:30",
                    FridayRoom = "Ауд. 407",
                    Saturday = "История",
                    SaturdayTime = "10:45 - 12:15",
                    SaturdayRoom = "Ауд. 507"
                });
                ScheduleItems.Add(new ScheduleItem
                {
                    Monday = "Информатика",
                    MondayTime = "12:30 - 14:00",
                    MondayRoom = "Ауд. 108",
                    Tuesday = "Английский язык",
                    TuesdayTime = "14:15 - 15:45",
                    TuesdayRoom = "Ауд. 208",
                    Wednesday = "Архитектура вычислительных систем",
                    WednesdayTime = "16:00 - 17:30",
                    WednesdayRoom = "Ауд. 308",
                    Thursday = "Интернет-технологии",
                    ThursdayTime = "9:00 - 10:30",
                    ThursdayRoom = "Ауд. 408",
                    Friday = "Математический анализ",
                    FridayTime = "10:45 - 12:15",
                    FridayRoom = "Ауд. 508",
                    Saturday = "Экономика",
                    SaturdayTime = "12:30 - 14:00",
                    SaturdayRoom = "Ауд. 608"
                });
                ScheduleItems.Add(new ScheduleItem
                {
                    Monday = "Машинное обучение",
                    MondayTime = "14:15 - 15:45",
                    MondayRoom = "Ауд. 109",
                    Tuesday = "Анализ данных",
                    TuesdayTime = "16:00 - 17:30",
                    TuesdayRoom = "Ауд. 209",
                    Wednesday = "Проектирование программных систем",
                    WednesdayTime = "9:00 - 10:30",
                    WednesdayRoom = "Ауд. 309",
                    Thursday = "Информационная безопасность",
                    ThursdayTime = "10:45 - 12:15",
                    ThursdayRoom = "Ауд. 409",
                    Friday = "Микроэкономика",
                    FridayTime = "12:30 - 14:00",
                    FridayRoom = "Ауд. 509",
                    Saturday = "Социология",
                    SaturdayTime = "14:15 - 15:45",
                    SaturdayRoom = "Ауд. 609"
                });
                ScheduleItems.Add(new ScheduleItem
                {
                    Monday = "Теория автоматов и формальных языков",
                    MondayTime = "16:00 - 17:30",
                    MondayRoom = "Ауд. 110",
                    Tuesday = "Компьютерная графика",
                    TuesdayTime = "9:00 - 10:30",
                    TuesdayRoom = "Ауд. 210",
                    Wednesday = "Технологии разработки ПО",
                    WednesdayTime = "10:45 - 12:15",
                    WednesdayRoom = "Ауд. 310",
                    Thursday = "Человеко-компьютерное взаимодействие",
                    ThursdayTime = "12:30 - 14:00",
                    ThursdayRoom = "Ауд. 410",
                    Friday = "Электроника",
                    FridayTime = "14:15 - 15:45",
                    FridayRoom = "Ауд. 510",
                    Saturday = "Психология",
                    SaturdayTime = "16:00 - 17:30",
                    SaturdayRoom = "Ауд. 610"
                });
                ScheduleItems.Add(new ScheduleItem
                {
                    Monday = "Корпоративные информационные системы",
                    MondayTime = "9:00 - 10:30",
                    MondayRoom = "Ауд. 111",
                    Tuesday = "Управление проектами",
                    TuesdayTime = "10:45 - 12:15",
                    TuesdayRoom = "Ауд. 211",
                    Wednesday = "Веб-программирование",
                    WednesdayTime = "12:30 - 14:00",
                    WednesdayRoom = "Ауд. 311",
                    Thursday = "Программная инженерия",
                    ThursdayTime = "14:15 - 15:45",
                    ThursdayRoom = "Ауд. 411",
                    Friday = "Философия",
                    FridayTime = "16:00 - 17:30",
                    FridayRoom = "Ауд. 511",
                    Saturday = "Линейная алгебра",
                    SaturdayTime = "9:00 - 10:30",
                    SaturdayRoom = "Ауд. 611"
                });
            }


            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
