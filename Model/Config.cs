using Avalonia.Controls;
using MySQLConnect.View.Config;
using System;
using System.IO;
using System.Text.Json;

namespace MySQLConnect.Model
{
    static class Config
    {
        public static string errorText;
        public static Window rootWindow;
        public static TimeBreaks timeBreaks;
        public static double width;
        public static double height;
        async public static void LoadData()
        {
            using (FileStream fs = new FileStream("timebreaks.json", FileMode.OpenOrCreate))
            {
                try
                {
                    timeBreaks = await JsonSerializer.DeserializeAsync<TimeBreaks>(fs);
                }catch(JsonException ex) { Console.WriteLine(ex.Message); }
            }
        }
        public static void CallNotification(Window parent, string? error = null)
        {
            Notification notif = new Notification();
            if (error == null) notif.data.NotifText = errorText;
            else notif.data.NotifText = error;
            notif.Title = "ERROR";
            try
            {
                WindowIcon ara = new WindowIcon(@"error.png");
                notif.Icon = ara;
            }
            catch { }
            notif.ShowDialog(parent);
        }
    }
    public struct TimeBreaks
    {
        public TimeSpan startDay { get; set; }
        public int lunchBreak { get; set; }
        public int lessonsBreak { get; set; }
        public int lectureBreak { get; set; }
    }
}
