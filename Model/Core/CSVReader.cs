using System.Collections.Generic;
using System.IO;

namespace MySQLConnect.Model.Core
{
    public static class CSVReader
    {
        public static List<Lesson> Read(bool speciality, string pathToFile, int semestr)
        {
            int modify;
            List<int> startPoints = new List<int>();
            if (speciality) modify = 15;
            else modify = 11;
            List<Lesson> lessons = new List<Lesson>();
            string[] allData = File.ReadAllLines(pathToFile);
            #region titleCheck
            string[] rowTitle1 = allData[5].Split(";");
            string[] rowTitle2 = allData[6].Split(";");
            for (int i = modify; i < rowTitle1.Length; i++) if (!rowTitle1[i].StartsWith("���") && rowTitle1[i].StartsWith("�") || rowTitle1[i].StartsWith("����")) startPoints.Add(i); // ��������� ����� ��������
            #endregion
            modify = startPoints[semestr - 1];
            foreach (string line in allData)
            {
                string[] row = line.Split(';');
                if (!row[0].Contains(".")) continue;
                for (int i = 0; i < row.Length; i++)
                    switch (row[i])
                    {
                        case "":
                            row[i] = "0"; // ������ ������ �������� �� 0
                            break;
                        case "���":
                            row[i] = "1";
                            break;
                        case "���":
                            row[i] = "36"; //6 ����� � ����, 6-�� ������
                            break;
                        case "��":
                            row[i] = "0";
                            break;
                    }
                Lesson lesson = new Lesson();
                lesson.index = row[0];
                lesson.name = row[1];
                lesson.semestr.allHours = row[modify];
                lesson.semestr.samostoyatelnie = row[modify + 1];
                if (speciality)
                {
                    lesson.semestr.konsultacii = row[modify + 2];
                    lesson.semestr.lekcii = row[modify + 4];
                    lesson.semestr.prackticheskie = row[modify + 5];
                    lesson.semestr.laboratornie = row[modify + 6];
                    if (rowTitle2[modify + 6].StartsWith("���"))
                    {
                        if (rowTitle2[modify + 7].StartsWith("����"))
                        {
                            lesson.semestr.kursProjectDo = row[modify + 7];
                            lesson.semestr.promezhAttestation = row[modify + 8];
                        }
                        else lesson.semestr.promezhAttestation = row[modify + 7];
                    }
                    else lesson.semestr.promezhAttestation = row[modify + 6];
                }
                else
                {
                    lesson.semestr.lekcii = row[modify + 3];
                    lesson.semestr.prackticheskie = row[modify + 4];
                    lesson.semestr.laboratornie = row[modify + 5];
                }
                lessons.Add(lesson);
            }
            return lessons;
        }

        public struct Lesson
        {
            public string index;
            public string name;
            //public AttestationForms attestationForm;
            public Semestr semestr;
        }
        public struct AttestationForms
        {
            public string ekzamen;
            public string zachet;
            public string difZachet;
            public string kursProject;
            public string kursRabot;
        }
        public struct Semestr
        {
            public string allHours;
            public string samostoyatelnie;
            public string konsultacii;
            public string lekcii;
            public string prackticheskie;
            public string laboratornie;
            public string kursProjectDo;
            public string promezhAttestation;
        }
    }

}