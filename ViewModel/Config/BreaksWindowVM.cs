using Avalonia.Controls;
using MySQLConnect.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace MySQLConnect.ViewModel.Config
{
    public class BreaksWindowVM : INotifyPropertyChanged
    {
        public Window thisWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private TimeSpan _startDay = Model.Config.timeBreaks.startDay;
        public TimeSpan startDay
        {
            get { return _startDay; }
            set
            {
                _startDay = value;
                OnPropertyChanged("startDay");
            }
        }
        private int _lunchBreak = Model.Config.timeBreaks.lunchBreak;
        public int lunchBreak
        {
            get { return _lunchBreak; }
            set
            {
                _lunchBreak = value;
                OnPropertyChanged("lunchBreak");
            }
        }
        private int _lessonsBreak = Model.Config.timeBreaks.lessonsBreak;
        public int lessonsBreak
        {
            get { return _lessonsBreak; }
            set
            {
                _lessonsBreak = value;
                OnPropertyChanged("lessonsBreak");
            }
        }
        private int _lectureBreak = Model.Config.timeBreaks.lectureBreak;
        public int lectureBreak
        {
            get { return _lectureBreak; }
            set
            {
                _lectureBreak = value;
                OnPropertyChanged("lectureBreak");
            }
        }
        public ICommand SaveTime
        {
            get {
                return new ActionCommand(async() =>{
                    using (FileStream fs = new FileStream("timebreaks.json", FileMode.OpenOrCreate))
                    {
                        Model.TimeBreaks time = new Model.TimeBreaks
                        {
                            startDay = startDay,
                            lunchBreak = lunchBreak,
                            lessonsBreak = lessonsBreak,
                            lectureBreak = lectureBreak
                        };
                        Model.Config.timeBreaks = time;
                        await JsonSerializer.SerializeAsync(fs, time);
                        Console.WriteLine("Данные о времени сохранены");
                        thisWindow.Close();
                    }
                });
            }
        }
    }
}
