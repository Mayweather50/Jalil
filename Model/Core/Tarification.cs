using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MySQLConnect.Model.Core
{
    public static class Tarification
    {
        static private string[] allData;
        static private List<int> groupsIndex = new List<int>();
        static public List<Speciality> allSpecialities = new List<Speciality>();
        static public List<Teacher> teachers = new List<Teacher>(); // Список учителей с уже прочтенной тарификацией
        static private Teacher teacher; // Переменная для учителей
        static private string spec;
        static public void Read(string pathToTarification, bool budjet)
        {
            allData = File.ReadAllLines(pathToTarification);
            if (budjet) Budjet();
            else NotBudjet();
            MySqlHandler.WriteSpeciality();
            MySqlHandler.WriteSubjectsTeachersGroups();
        }
        static private void Budjet()
        {
            List<int> allHoursIndex = new List<int>();
            #region Чтение групп
            string[] _groupsCount = allData[12].Split(',');
            string[] _groups = allData[14].Split(',');
            for (int i = 0; i < _groups.Length; i++)
            {
                if (_groups[i] != "") groupsIndex.Add(i);
            }
            #endregion
            #region Чтение специальностей
            List<string> speciality = allData[10].Split(',').ToList();
            for (int i = 0; i < speciality.Count; i++)
            {

                if (speciality[i] != "" && !speciality[i].EndsWith("\""))
                {
                    Speciality spec = new Speciality();
                    if (speciality[i].StartsWith("\""))
                    {
                        speciality[i] += "," + speciality[i + 1];
                        speciality.RemoveAt(i + 1);
                        speciality[i] = speciality[i].Trim('\"');
                    }
                    string[] specLine = speciality[i].Split(" ");
                    spec.Code = specLine[0];
                    for (int j = 1; j < specLine.Length; j++) spec.Name += $"{specLine[j]} ";
                    spec.Name = spec.Name.Trim();
                    allSpecialities.Add(spec);
                }
            }
            #endregion
            #region Поиск необходимых столбцов
            string[] titles = allData[9].Split(',');
            for (int i = 0; i < titles.Length; i++)
            {
                if (titles[i].StartsWith("Всего")) allHoursIndex.Add(i);
            }
            #endregion
            #region Чтение предметов
            for (int row = 16; row < allData.Length; row++)
            {
                string[] line = allData[row].Split(",");
                if (!int.TryParse(line[0], out _) && line[3] == "") // Если последняя строчка не содержит индекс преподавателя
                {
                    teachers.Add(teacher);
                    break;
                }
                if (line[1] != "") // Если ячейка преподавателя не пустая
                {
                    if (teacher.Name != null) // Если это не первый преподаватель
                    {
                        teachers.Add(teacher);
                        teacher = new Teacher();
                    }
                    teacher.Name = line[1];
                    teacher.Stavka = Convert.ToDouble(line[2].Replace('.', ','));
                    teacher.AllHour = ToInt32(line[allHoursIndex[0]]);
                    teacher.AllHourCorrection = ToInt32(line[allHoursIndex[1]]);
                    teacher.Burden = ToInt32(line[allHoursIndex[1] + 1]);
                    teacher.Subjects = new List<Subject>();
                }
                Subject subject = new Subject();
                if (line[3].Contains('.'))
                {
                    string[] nameLine = line[3].Split(' ');
                    for (int i = 2; i < nameLine.Length; i++) subject.Name += nameLine[i] + " ";
                }
                else subject.Name = line[3];
                subject.Name = subject.Name.Trim();
                subject.AllHour = ToInt32(line[allHoursIndex[0] - 1]);
                subject.AllHourCorrection = ToInt32(line[allHoursIndex[1] - 1]);
                subject.TheoryHour = ToInt32(line[allHoursIndex[1] + 2]);
                subject.ConsultationHour = ToInt32(line[allHoursIndex[1] + 3]);
                subject.Groups = new List<Group>();
                for (int i = 0; i < groupsIndex.Count; i++)
                {
                    int grpIndex = groupsIndex[i];
                    if (speciality[grpIndex] != "") spec = speciality[grpIndex];
                    Group group = new Group();
                    int diff;
                    // Если следующей группы нет, то за границу берется индекс "Сумма". Находится разница между группами для выявления столбцов
                    if (i + 1 >= groupsIndex.Count) diff = (allHoursIndex[1] - 1) - grpIndex;
                    else if (groupsIndex[i + 1] - grpIndex > 3) diff = (allHoursIndex[0] - 1) - grpIndex; // Если после группы идет количество часов предмета
                    else diff = groupsIndex[i + 1] - grpIndex;
                    group.Name = _groups[grpIndex];
                    group.Speciality = spec.Trim();
                    group.Count = ToInt32(_groupsCount[grpIndex]);
                    group.Hours = new int[3];
                    group.Hours[0] = ToInt32(line[grpIndex]);
                    if (diff == 2) group.Hours[2] = ToInt32(line[grpIndex + 1]);
                    else if (diff == 3)
                    {
                        group.Hours[1] = ToInt32(line[grpIndex + 1]);
                        group.Hours[2] = ToInt32(line[grpIndex + 2]);
                    }
                    //if (group.Hours[0] == 0 && group.Hours[1] == 0 && group.Hours[2] == 0) continue; // Если у группы нет часов, то она не добавляется в список
                    subject.Groups.Add(group);
                }
                teacher.Subjects.Add(subject);
            }
            #endregion
        }
        static private void NotBudjet()
        {
            int allHourIndex = 0;
            #region Чтение групп
            string[] _groupsCount = allData[12].Split(',');
            string[] _groups = allData[14].Split(',');
            for (int i = 0; i < _groups.Length; i++)
            {
                if (_groups[i] != "") groupsIndex.Add(i);
            }
            #endregion
            #region Чтение специальностей
            List<string> speciality = allData[10].Split(',').ToList();
            for (int i = 0; i < speciality.Count; i++)
            {

                if (speciality[i] != "" && !speciality[i].EndsWith("\""))
                {
                    Speciality spec = new Speciality();
                    if (speciality[i].StartsWith("\""))
                    {
                        speciality[i] += "," + speciality[i + 1];
                        speciality.RemoveAt(i + 1);
                        speciality[i] = speciality[i].Trim('\"');
                    }
                    string[] specLine = speciality[i].Split(" ");
                    spec.Code = specLine[0];
                    for (int j = 1; j < specLine.Length; j++) spec.Name += $"{specLine[j]} ";
                    spec.Name = spec.Name.Trim();
                    allSpecialities.Add(spec);
                }
            }
            #endregion
            #region Поиск необходимых столбцов
            string[] titles = allData[9].Split(',');
            for (int i = 0; i < titles.Length; i++)
            {
                if (titles[i].StartsWith("Всего"))
                {
                    allHourIndex = i;
                    break;
                }
            }
            #endregion
            #region Чтение предметов
            for (int row = 16; row < allData.Length; row++)
            {
                string[] line = allData[row].Split(",");
                if (line[1].Contains("МДК")) // Если последняя строчка содержит "МДК", значит это конец документа
                {
                    teachers.Add(teacher);
                    break;
                }
                if (line[1] != "") // Если ячейка преподавателя не пустая
                {
                    if (teacher.Name != null)
                    {
                        teachers.Add(teacher);
                        teacher = new Teacher();
                    }
                    teacher.Name = line[1];
                    teacher.Stavka = Convert.ToDouble(line[2].Replace('.', ','));
                    teacher.AllHour = ToInt32(line[allHourIndex]);
                    teacher.Subjects = new List<Subject>();
                }
                Subject subject = new Subject();
                if (line[3].Contains('.'))
                {
                    string[] nameLine = line[3].Split(' ');
                    for (int i = 2; i < nameLine.Length; i++) subject.Name += nameLine[i] + " ";
                }
                else subject.Name = line[3];
                subject.Name = subject.Name.Trim();
                subject.AllHour = ToInt32(line[allHourIndex - 1]);
                subject.Groups = new List<Group>();
                for (int i = 0; i < groupsIndex.Count; i++)
                {
                    int grpIndex = groupsIndex[i];
                    if (speciality[grpIndex] != "") spec = speciality[grpIndex];
                    Group group = new Group();
                    int diff;
                    // Если следующей группы нет, то за границу берется индекс "Сумма". Находится разница между группами для выявления столбцов
                    if (i + 1 >= groupsIndex.Count) diff = (allHourIndex - 1) - groupsIndex[i];
                    else diff = groupsIndex[i + 1] - groupsIndex[i];
                    group.Name = _groups[grpIndex];
                    group.Speciality = spec.Trim();
                    group.Count = ToInt32(_groupsCount[grpIndex]);
                    group.Hours = new int[3];
                    group.Hours[0] = ToInt32(line[grpIndex]);
                    subject.TheoryHour += ToInt32(line[grpIndex]);
                    if (diff == 2)
                    {
                        subject.ConsultationHour += ToInt32(line[grpIndex + 1]);
                        group.Hours[2] = ToInt32(line[grpIndex + 1]);
                    }
                    else if (diff == 3)
                    {
                        group.Hours[1] = ToInt32(line[grpIndex + 1]);
                        group.Hours[2] = ToInt32(line[grpIndex + 2]);
                    }
                    //if (group.Hours[0] == 0 && group.Hours[1] == 0 && group.Hours[2] == 0) continue; // Если у группы нет часов, то она не добавляется в список
                    subject.Groups.Add(group);
                }
                teacher.Subjects.Add(subject);
            }
            #endregion
        }
        static private int ToInt32(string var)
        {
            if (int.TryParse(var, out int a)) return a;
            else return 0;
        }
        public struct Speciality
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public struct Teacher
        {
            public string Name { get; set; }
            public double Stavka { get; set; }
            public int AllHour { get; set; }
            public int AllHourCorrection { get; set; }
            public int Burden { get; set; }
            public List<Subject> Subjects { get; set; }
            public Teacher(string name, double stavka, int allHour, int allHourCorrection, int burden)
            {
                Name = name;
                Stavka = stavka;
                AllHour = allHour;
                AllHourCorrection = allHourCorrection;
                Burden = burden;
                Subjects = new List<Subject>();
            }
        }
        public struct Subject
        {
            public string Name { get; set; }
            public int AllHour { get; set; }
            public int AllHourCorrection { get; set; }
            public int TheoryHour { get; set; }
            public int ConsultationHour { get; set; }
            public List<Group> Groups { get; set; }
            public Subject(string name, int allHour, int allHourCorrection, int theoryHour, int consultationHour)
            {
                Name = name;
                AllHour = allHour;
                AllHourCorrection = allHourCorrection;
                TheoryHour = theoryHour;
                ConsultationHour = consultationHour;
                Groups = new List<Group>();
            }
        }
        public struct Group
        {
            public string Name { get; set; }
            public string Speciality { get; set; }
            public int Count { get; set; }
            public int[] Hours { get; set; }
        }
    }
}
