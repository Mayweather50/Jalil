using Avalonia.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MySQLConnect.Model.Core
{
    public static class Tabletime
    {
        static private Dictionary<int, Lesson[]> raspisanie = new Dictionary<int, Lesson[]>();
        static public List<int> idGroups = new List<int>();
        static public Dictionary<int, List<List<Lesson>>> finalRaspisanie = new Dictionary<int, List<List<Lesson>>>();
        static public Dictionary<int, List<Subject>> subjects = new Dictionary<int, List<Subject>>();
        static public List<string> teachers = new List<string>();
        static public List<Room> rooms = new List<Room>();
        static private int parInWeek = 3; // пар в день
        static private int semestrDays = 99; // Дней в семестре
        static private int semestrPar = semestrDays * parInWeek; // 17 недель
        static private Grid tableTime; // Таблица в любом случае определяется при запуске приложения
        static public int idChoosedGroup;

        static public void Start(Grid grid, int semestr)
        {
            tableTime = grid;
            MySqlHandler.GetData4TableTime(semestr);
            FirstGenerate();
        }
        static private void FirstGenerate() // Первичная генерация предмета с наибольшим кол-вом часов
        {
            raspisanie.Clear();
            foreach(int gr in idGroups)
            {
                if (subjects[gr].Count == 0) continue;
                raspisanie.Add(gr, new Lesson[semestrPar]); // добавление массива для группы
                int subjPar = subjects[gr][0].TotalHours / 2; // количество пар одного предмета
                int step = semestrPar / subjPar; // Шаг с которым необходимо расставить предмет
                for (int i = 0; i < subjPar; i++) // FOR по количеству пар
                {
                    for(int teachIndex = 0; teachIndex < teachers.Count; teachIndex++)
                    {
                        if (Check(i * step, teachers[teachIndex]))
                        {
                            raspisanie[gr][i * step] = new Lesson() // Добавление предмета в расписание
                            {
                                Subject = subjects[gr][0].Name,
                                Teacher = teachers[teachIndex],
                                Room = $"{rooms[0].Number} {rooms[0].Name}",

                            };
                        }
                        else
                        {
                            int pos = (i * step) + 1;
                            while (raspisanie[gr][pos].Subject != null && !Check(pos, teachers[teachIndex])) pos++; // Увеличение позиции на один
                            raspisanie[gr][pos] = new Lesson() // Добавление предмета в расписание
                            {
                                Subject = subjects[gr][0].Name,
                                Teacher = teachers[teachIndex],
                                Room = $"{rooms[0].Number} {rooms[0].Name}",

                            };
                        }
                    }
                }
            }
            if (raspisanie.Count == 0) return;
            Generate();
        }
        static private void Generate() // Основаня генерация предметов
        {
            foreach(int gr in idGroups)
            {
                if (subjects[gr].Count == 0) continue;
                for (int sub = 1; sub < subjects[gr].Count; sub++)
                {
                    int subjPar = subjects[gr][sub].TotalHours / 2; // количество пар одного предмета
                    int step = semestrPar / subjPar; // Шаг с которым необходимо расставить предмет
                    for (int i = 0; i < subjPar; i++) // FOR по количеству пар
                    {
                        for(int teachIndex = 0; teachIndex < teachers.Count; teachIndex++)
                        {
                            if (raspisanie[gr][i * step].Subject == null &&
                            Check(i * step, teachers[teachIndex])) // Проверка не занято ли значение
                            {
                                raspisanie[gr][i * step] = new Lesson() // Добавление предмета в расписание
                                {
                                    Subject = subjects[gr][sub].Name,
                                    Teacher = teachers[teachIndex],
                                    Room = $"{rooms[0].Number} {rooms[0].Name}",

                                };
                            }
                            else // Если пара занята другим предметом
                            {
                                int pos = (i * step) + 1; // Увеличение позиции на один
                                int counter = 0; // Счетчик для WHILE
                                while (raspisanie[gr][pos].Subject != null ||
                                    !Check(pos, teachers[teachIndex])) // Пока позиция занята другими предметами 
                                {
                                    if (counter > raspisanie[gr].Length) // Если счетчик превысит размер массива
                                    {
                                        break; // Выход из WHILE
                                    }
                                    pos++; // Увеличение позиции на один
                                    if (pos >= raspisanie[gr].Length) pos = 0; // Если позиция больше размера массива, то возвращает в начало
                                    counter++;
                                }
                                raspisanie[gr][pos] = new Lesson() // Добавление предмета в расписание
                                {
                                    Subject = subjects[gr][sub].Name,
                                    Teacher = teachers[teachIndex],
                                    Room = $"{rooms[0].Number} {rooms[0].Name}",

                                };
                            }
                        }
                    }
                }
            }
            CheckPari();
            PostGenerate();
        }
        static private bool Check(int pos, string teacher) // проверка // ЗАМЕНИТЬ НА ПРОВЕРКУ УЧИТЕЛЯ 
        {
            foreach (int gr in idGroups)
            {
                if (subjects[gr].Count == 0) continue;
                try { if (raspisanie[gr][pos].Teacher == teacher) return false; } 
                catch { }
            }
            return true;
        }
        static private void PostGenerate() // Генерация предметов с остатком часов
        {
            foreach (int gr in idGroups) // Группы
            {
                if (subjects[gr].Count == 0) continue;
                do
                {
                    for (int i = 0; i < bla[gr].Count; i++) // Остаточные предметы
                    {
                        int value = bla[gr][i].parLeft;
                        for (int j = 0; j < value; j++) // Пары предмета
                        {
                            for (int pos = 0; pos < semestrPar; pos++) // Позиция в расписании
                            {
                                for(int teachIndex = 0; teachIndex < teachers.Count; teachIndex++)
                                {
                                    if (bla[gr][i].parLeft > 0 && raspisanie[gr][pos].Subject == null && Check(pos, teachers[teachIndex]))
                                    {
                                        raspisanie[gr][pos] = new Lesson()
                                        {
                                            Subject = bla[gr][i].subject,
                                            Teacher = teachers[teachIndex],
                                            Room = $"{rooms[0].Number} {rooms[0].Name}",

                                        };
                                        TestStruct a = bla[gr][i];
                                        a.parLeft--;
                                        bla[gr][i] = a;
                                    }
                                }
                            }
                        }
                    }
                    bla[gr].RemoveAll(x => x.parLeft == 0);
                } while (bla[gr].Count > 0 && raspisanie[gr].Any(x => x.Subject == null));
            }
            FinalGeneration();
        }
        static private void FinalGeneration() // Расписание по дням
        {
            finalRaspisanie.Clear();
            foreach (int gr in idGroups)
            {
                if (subjects[gr].Count == 0) continue;
                finalRaspisanie.Add(gr, new List<List<Lesson>>());
                int day = 0;
                finalRaspisanie[gr].Add(new List<Lesson>());
                for (int i = 0; i < semestrPar; i++)
                {
                    raspisanie[gr][i].Day = day;
                    finalRaspisanie[gr][day].Add(raspisanie[gr][i]);
                    if ((i + 1) % parInWeek == 0 && i + 1 != semestrPar)
                    {
                        day++;
                        finalRaspisanie[gr].Add(new List<Lesson>());
                    }
                }
            }
            OutputWeek();
            JsonSave();
        }
        static public async void JsonSave() // Сохранение расписания в json
        {
            using (FileStream fs = new FileStream("tabletime.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(fs, finalRaspisanie, new JsonSerializerOptions { WriteIndented = true});
            }
        }
        static public async void JsonLoad(Window mainWindow) // Загрузка расписания из json если оно существует
        {
            try
            {
                using (FileStream fs = new FileStream("tabletime.json", FileMode.Open))
                {
                    finalRaspisanie = await JsonSerializer.DeserializeAsync<Dictionary<int, List<List<Lesson>>>>(fs);
                }
                tableTime = mainWindow.FindControl<Grid>("TableTime"); // Поиск таблицы для расписания                
                OutputWeek(idGroups[0]);
            }
            catch { }
        }
        static public void OutputWeek(int modify = 0) // генерация недели
        {
            tableTime.Children.Clear();
            EditorHandler.thisWeek.Clear();
            for (int day = 0; day < 6; day++)
            {
                EditorHandler.thisWeek.Add(new List<Lesson>());
                for (int para = 0; para < 3; para++)
                {
                    StackPanel subject = new StackPanel();
                    try
                    {
                        subject.Children.Add(new TextBlock()
                        {
                            Text = $"{finalRaspisanie[idChoosedGroup][day + modify][para].Subject}",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                        });
                        subject.Children.Add(new TextBlock()
                        {
                            Text = $"{finalRaspisanie[idChoosedGroup][day + modify][para].Teacher}",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                        });
                        subject.Children.Add(new TextBlock()
                        {
                            Text = $"{finalRaspisanie[idChoosedGroup][day + modify][para].Room}",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                        });
                    }
                    catch(Exception ex) { Console.WriteLine(ex.Message); continue; }
                    tableTime.Children.Add(subject);
                    Grid.SetColumn(subject, day);
                    Grid.SetRow(subject, para + 1);
                    EditorHandler.thisWeek[day].Add(finalRaspisanie[idChoosedGroup][day + modify][para]);
                }
            }
        }
        static public void OutputChangedWeek()
        {
            tableTime.Children.Clear();
            for (int day = 0; day < 6; day++)
            {
                for (int para = 0; para < EditorHandler.thisWeek[day].Count; para++)
                {
                    StackPanel subject = new StackPanel();
                    subject.Children.Add(new TextBlock()
                    {
                        Text = $"{EditorHandler.thisWeek[day][para].Subject}",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    });
                    subject.Children.Add(new TextBlock()
                    {
                        Text = $"{EditorHandler.thisWeek[day][para].Teacher}",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    });
                    subject.Children.Add(new TextBlock()
                    {
                        Text = $"{EditorHandler.thisWeek[day][para].Room}",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    });
                    tableTime.Children.Add(subject);
                    Grid.SetColumn(subject, day);
                    Grid.SetRow(subject, para + 1);
                    Console.WriteLine(EditorHandler.thisWeek[day][para].Day);
                }
            }
        }
        static public void ChangeTabletime()
        {
            for(int day = 0; day < 6; day++)
            {
                int dayRasp = EditorHandler.thisWeek[day][0].Day;
                for(int para = 0; para < EditorHandler.thisWeek[day].Count; para++)
                {
                    finalRaspisanie[idChoosedGroup][dayRasp][para] = EditorHandler.thisWeek[day][para];
                }
            }
        }
        static public void NextWeek()
        {
            int lastIndex;
            for (int day = 5, _day = 1; day >= 0; day--, _day++)
            {
                try { lastIndex = EditorHandler.thisWeek[day].Last().Day + _day; }
                catch { continue; }
                if (lastIndex < semestrDays) OutputWeek(lastIndex);
                return;
            }
        }
        static public void PreviousWeek()
        {
            int lastIndex;
            try { lastIndex = EditorHandler.thisWeek[0][0].Day - 1; }
            catch { return; }
            lastIndex -= 5;
            if (lastIndex < 0) return;
            OutputWeek(lastIndex);
        }
        static public void ExportWeek(string groupName)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter sw = new StreamWriter(docPath + @$"\Tabletime-{groupName}-Week.csv", false, System.Text.Encoding.UTF8))
            {
                int max = EditorHandler.thisWeek.Max(x => x.Count);
                int dayWeek = 1;
                for (int day = 0; day < 6; day++)
                {
                    sw.Write($"{DateAndTime.WeekdayName(dayWeek)}");
                    for (int para = 0; para < max; para++)
                    {
                        sw.Write($",{EditorHandler.thisWeek[day][para].Subject} {EditorHandler.thisWeek[day][para].Teacher} {EditorHandler.thisWeek[day][para].Room}");
                    }
                    sw.WriteLine();
                    dayWeek++;
                }
            }
        }
        static public void Export(string groupName)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter sw = new StreamWriter(docPath + @$"\Tabletime-{groupName}.csv", false, System.Text.Encoding.UTF8))
            {
                int max = finalRaspisanie[idChoosedGroup].Max(x => x.Count);
                for(int dayWeek = 1; dayWeek < 7; dayWeek++)
                {
                    sw.Write($"{DateAndTime.WeekdayName(dayWeek)}");
                    for (int para = 0; para < max; para++)
                    {
                        for (int day = 0; day < semestrDays; day++)
                        {
                            sw.Write($",{finalRaspisanie[idChoosedGroup][day][para].Subject} {finalRaspisanie[idChoosedGroup][day][para].Teacher} {finalRaspisanie[idChoosedGroup][day][para].Room}");
                        }
                        sw.WriteLine();
                    }
                    sw.WriteLine();
                }
            }
        }
        #region TestReg
        public struct TestStruct
        {
            public string subject;
            public int hour;
            public int par;
            public int parLeft;
        }
        static public Dictionary<int, List<TestStruct>> bla = new Dictionary<int, List<TestStruct>>();
        static public void CheckPari()
        {
            foreach (int gr in idGroups)
            {
                if (subjects[gr].Count == 0) continue;
                bla.Add(gr, new List<TestStruct>());
                for (int i = 0; i < semestrPar; i++)
                {
                    if (raspisanie[gr][i].Subject == null) continue;
                    if (bla[gr].Exists(x => x.subject == raspisanie[gr][i].Subject))
                    {
                        int index = bla[gr].FindIndex(x => x.subject == raspisanie[gr][i].Subject);
                        TestStruct a = bla[gr][index];
                        a.par++;
                        a.hour += 2;
                        bla[gr][index] = a;
                    }
                    else bla[gr].Add(new TestStruct
                    {
                        subject = raspisanie[gr][i].Subject,
                        hour = 0,
                        par = 0,
                        parLeft = 0
                    });
                }
            }
            foreach (int gr in idGroups)
            {
                if (subjects[gr].Count == 0) continue;
                for (int i = 0; i < bla[gr].Count; i++)
                {
                    TestStruct a = bla[gr][i];
                    a.parLeft = (subjects[gr].Find(x => x.Name == bla[gr][i].subject).TotalHours / 2) - bla[gr][i].par;
                    bla[gr][i] = a;
                }
            }
        }
        #endregion
    }
    public struct Subject
    {
        public string Name;
        public int TotalHours;
    }
    public struct Room
    {
        public int Number;
        public string Name;
        public string Type;
    }
    public struct Lesson
    {
        public string Subject { get; set; }
        public string Teacher { get; set; }
        public string Room { get; set; }
        public int Day { get; set; }
    }
}
