using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySQLConnect.Model.Core
{
    public static class CSVReader
    {
        //static private string[] allData;
        public static List<string> allData = new List<string>();
        static private List<int> semestrIndex = new List<int>();
        static private List<int> countTypes = new List<int>();
        static public Dictionary<int, List<Subject>> plan = new Dictionary<int, List<Subject>>();
        static private int startRow;
        public static void Read(string path, string group)
        {
           


            allData.Add("\"Семестр\", \"Предмет\", \"Преподаватель\", \"Время\", \"Период\"");
            allData.Add("\"1 семестр\", \"Математика\", \"Иванов И.И.\", \"Пн 9:00\", \"12 нед\"");
            allData.Add("\"1 семестр\", \"Физика\", \"Петров П.П.\", \"Ср 11:00\", \"14 нед\"");
            allData.Add("\"2 семестр\", \"Программирование\", \"Сидоров С.С.\", \"Пт 14:00\", \"10 нед\"");




            //using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding(1251)))
            //{
            //    string line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        allData.Add(line);
            //    }
            //}

            FindSemestrIndex();
            for (int row = startRow; row < allData.ToArray().Length; row++)
            {
                string[] line = allData[row].Split(',');
                if (line[0].Contains('.'))
                {
                    for (int i = 0; i < semestrIndex.Count; i++)
                    {
                        int modify = 0;
                        if (!plan.ContainsKey(i)) plan.Add(i, new List<Subject>());
                        Subject subject = new Subject();
                        subject.Index = line[0];
                        if (line[1].StartsWith("\""))
                        {
                            line[1] += $",{line[2]}";
                            line[2] = "";
                            modify = 1;
                            line[1] = line[1].Trim('"');
                        }
                        subject.Name = line[1];
                        if (line[0].StartsWith("УП.") || line[0].StartsWith("ПП."))
                        {
                            int practicSemestr = 0, allHourIndex = 1;
                            do
                            {
                                allHourIndex++;
                                if (ToInt32(line[allHourIndex]) != 0 && ToInt32(line[allHourIndex]) < 10) practicSemestr = allHourIndex;
                            } while (ToInt32(line[allHourIndex]) < 10);
                            subject.TotalHour = ToInt32(line[allHourIndex]);
                            if (!plan.ContainsKey(ToInt32(line[practicSemestr]) - 1)) plan.Add(i, new List<Subject>());
                            plan[ToInt32(line[practicSemestr]) - 1].Add(subject);
                            break;
                        }
                        subject.TotalHour = ToInt32(line[semestrIndex[i] + modify]);
                        if (subject.TotalHour == 0) continue;
                        subject.IndependentHour = ToInt32(line[semestrIndex[i] + modify + 1]);
                        subject.ConsultationHour = ToInt32(line[semestrIndex[i] + modify + 2]);
                        subject.Lesson = ToInt32(line[semestrIndex[i] + modify + 4]);
                        subject.PracWork = ToInt32(line[semestrIndex[i] + modify + 5]);
                        int attesttationIndex = 0;
                        switch (countTypes[i])
                        {
                            case 4:
                                subject.LabWork = ToInt32(line[semestrIndex[i] + modify + 6]);
                                subject.Kurs = ToInt32(line[semestrIndex[i] + modify + 7]);
                                attesttationIndex = 8;
                                break;
                            case 3:
                                subject.LabWork = ToInt32(line[semestrIndex[i] + modify + 6]);
                                attesttationIndex = 7;
                                break;
                            case 2:
                                attesttationIndex = 6;
                                break;
                        }
                        subject.Attestation = ToInt32(line[semestrIndex[i] + modify + attesttationIndex]);
                        plan[i].Add(subject);
                    }
                }
            }
            MySqlHandler.WritePlan(group);
        }
        private static int ToInt32(string var)
        {
            if (int.TryParse(var, out int a)) return a;
            else return 0;
        }
        private static void FindSemestrIndex()
        {
            startRow = 8; // Начальная строка
            if (allData.Count < startRow)
            {
                startRow = allData.Count; // Обработка случая, когда данных меньше, чем startRow
            }

            for (int row = 0; row < startRow; row++)
            {
                string[] line = allData[row].Split(',');
                for (int column = 0; column < line.Length; column++)
                {
                    if (line[column].Trim() == "") continue;
                    Console.WriteLine("Вывод: " + line[column]);
                    if (line[column].Trim().EndsWith("нед"))
                    {
                        startRow = row;
                        semestrIndex.Add(column);
                    }
                }
            }
            if (semestrIndex.Count > 0)
            {
                FindLessonType();
            }
            else
            {
                Console.WriteLine("Не найдено ни одного столбца, заканчивающегося на 'нед'.");
            }
        }

        private static void FindLessonType()
        {
            startRow += 2; // Переход к строке после "стартовой"
            if (startRow >= allData.Count)
            {
                Console.WriteLine("Значение startRow превышает количество строк в данных.");
                return;
            }
            string[] line = allData[startRow].Split(",");
            int counter = 0;
            int column = semestrIndex[0];
            do
            {
                string value = line[column].Trim();
                if (value != "")
                {
                    if (value.StartsWith("\""))
                    {
                        value += $",{line[column + 1].Trim()}";
                        line[column + 1] = "";
                        if (counter != 0)
                            countTypes.Add(counter);
                        counter = 0;
                    }
                    counter++;
                }
                column++;
            } while (column < line.Length);
            countTypes.Add(counter);
        }
        public struct Subject
        {
            public string Index { get; set; }
            public string Name { get; set; }
            public int TotalHour { get; set; }
            public int IndependentHour { get; set; }
            public int ConsultationHour { get; set; }
            public int Lesson { get; set; }
            public int PracWork { get; set; }
            public int LabWork { get; set; }
            public int Kurs { get; set; }
            public int Attestation { get; set; }
        }
    }
}