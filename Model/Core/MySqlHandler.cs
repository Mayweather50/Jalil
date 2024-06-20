using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MySQLConnect.Model.Core
{
    public static class MySqlHandler
    {
        // Консоль
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static MySqlConnection conn = new MySqlConnection();

        public static bool Connect(string connstr)// Подключение к БД
        {
            AllocConsole(); // Запуск консоли
            conn = new MySqlConnection(connstr);
            try { conn.Open(); }
            catch (MySqlException ex)
            {
                Config.errorText = ex.Message;
                return false;
            }
            CheckTables();
            GetAllGroupsId();
            try { Tabletime.idChoosedGroup = Tabletime.idGroups[0]; } catch (Exception ex) { Console.WriteLine(ex.Message); }
            return true;
        }
        private static void CheckTables()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'tabletimedb';", conn);
            int.TryParse(cmd.ExecuteScalar().ToString(), out int count);
            if (count == 0) CreateTables();
        }
        private static void CreateTables()// Создание таблиц при начальном запуске приложения
        {
            string states = "create table if not exists `states`(id int primary key auto_increment,`name` varchar(15) not null);";
            string roomTypes = "create table if not exists `roomtypes`(id int primary key auto_increment,`name` varchar(20) not null);";
            //string specialtyTypes = "create table if not exists `specialitytypes`(id int primary key auto_increment,`name` varchar(20) not null); " +
            //    "insert into `specialitytypes`(`name`) values('Профессиональный'), ('Специалиальный'), ('Коррекционый');";
            string subjects = "create table if not exists `subjects`(id int primary key auto_increment,`name` varchar(100) not null unique,allhourbudjet smallint not null default 0," +
                "allhourcorrection smallint not null default 0,allHournotbudjet smallint not null default 0,alltheoryhour smallint not null default 0,allconsultationhour smallint not null default 0);";
            string teachers = "create table if not exists `teachers`(id int primary key auto_increment,fullname varchar(50) not null unique,state int not null default 1,stake float(2) not null," +
                "burden smallint not null default 0,allhour smallint not null default 0,allhourcorrection smallint not null default 0,foreign key(state) references `states`(id) on delete cascade on update cascade);";
            string specialities = "create table if not exists `specialities`(id int primary key auto_increment,`name` varchar(100) not null,`code` varchar(15) not null unique);";
            string rooms = "create table if not exists `rooms`(id int primary key auto_increment,`name` varchar(50) not null,`number` smallint not null unique,`type` int not null," +
                "foreign key(`type`) references `roomtypes`(id) on delete cascade on update cascade);";
            string groups = "create table if not exists `groups`(id int primary key auto_increment,`name` varchar(10) not null unique,speciality int not null,count tinyint not null," +
                "foreign key(speciality) references `specialities`(id) on delete cascade on update cascade);";
            string teacherSubject = "create table if not exists `teachersubject`(id int primary key auto_increment,teacher_id int not null,subject_id int not null," +
                "foreign key(teacher_id) references `teachers`(id) on delete cascade on update cascade,foreign key(subject_id) references `subjects`(id) on delete cascade on update cascade);";
            string groupSubject = "create table if not exists `groupsubject`(id int primary key auto_increment,subject_id int not null,group_id int not null," +
                "foreign key(subject_id) references `subjects`(id) on delete cascade on update cascade,foreign key(group_id) references `groups`(id) on delete cascade on update cascade);";
            string tarification = "create table if not exists `tarification`(id int primary key auto_increment,groupsubject_id int not null unique,theory smallint not null," +
                "division smallint not null default 0,consultation smallint not null,foreign key(groupsubject_id) references `groupsubject`(id) on delete cascade on update cascade);";
            string plans = "create table if not exists `plans`(id int primary key auto_increment,groupsubject_id int not null,semestr tinyint not null,totalhour smallint not null," +
                "independentwork smallint not null,consultation smallint not null,lesson smallint not null,practicalwork smallint not null,labwork smallint not null," +
                "kursproject smallint not null,attestation smallint not null,foreign key(groupsubject_id) references `groupsubject`(id) on delete cascade on update cascade);";

            List<string> commands = new List<string>() { states, roomTypes, subjects, teachers, specialities, rooms, groups, teacherSubject, groupSubject, tarification, plans };

            MySqlCommand cmd;
            for (int i = 0; i < commands.Count; i++)
            {
                cmd = new MySqlCommand(commands[i], conn);
                cmd.ExecuteNonQuery();
            }

            List<string> stateNames = new List<string>() { "Работает", "На больничном", "В командировке" };
            foreach (string stateName in stateNames)
            {
                AddState(stateName);
            }

            List<string> roomTypeNames = new List<string>() { "Лекционный", "Компьютерный", "Спортзал" };
            foreach (string roomTypeName in roomTypeNames)
            {
                AddRoomType(roomTypeName);
            }


        }

        public static void AddState(string stateName)
        {
            string query = $"SELECT COUNT(*) FROM states WHERE name = '{stateName}'";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
            {
                // Запись с таким именем еще не существует, добавляем новую запись
                string insertQuery = $"INSERT INTO states (name) VALUES ('{stateName}')";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.ExecuteNonQuery();
            }
        }

        public static void AddRoomType(string typeName)
        {
            string query = $"SELECT COUNT(*) FROM roomtypes WHERE name = '{typeName}'";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
            {
                // Запись с таким именем еще не существует, добавляем новую запись
                string insertQuery = $"INSERT INTO roomtypes (name) VALUES ('{typeName}')";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.ExecuteNonQuery();
            }
        }




        public static void GetAllGroupsId() // Получение всех id'шников групп
        {
            MySqlCommand cmd = new MySqlCommand() { Connection = conn };
            cmd.CommandText = "select `id` from `groups`;";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Tabletime.idGroups.Add(reader.GetInt32(0));
                    }
            }
        }
        public static bool CheckTeachers()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT count(*) FROM teachers;", conn);
            int.TryParse(cmd.ExecuteScalar().ToString(), out int count);
            if (count == 0) return true;
            else return false;
        }
        public static void WriteSpeciality()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            foreach (Tarification.Speciality spec in Tarification.allSpecialities)
            {
                string command = $"insert ignore into `specialities`(`name`, `code`) values ('{spec.Name}', '{spec.Code}');";
                cmd.CommandText = command;
                try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
            }
        }
        public static void WriteSubjectsTeachersGroups()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            foreach (Tarification.Teacher teacher in Tarification.teachers)
            {
                string command = $"INSERT ignore INTO `teachers` (`fullname`, `stake`, `burden`, `allhour`, `allhourcorrection`) " +
                    $"VALUES ('{teacher.Name}', {teacher.Stavka.ToString().Replace(",", ".")}, {teacher.Burden}, {teacher.AllHour}, {teacher.AllHourCorrection});";
                cmd.CommandText = command;
                try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
                foreach (Tarification.Subject subject in teacher.Subjects)
                {
                    command = $"INSERT ignore INTO `subjects` (`name`, `allhourbudjet`, `allhourcorrection`, `allHournotbudjet`, `alltheoryhour`, `allconsultationhour`) " +
                    $"VALUES ('{subject.Name}', {subject.AllHour}, {subject.AllHourCorrection}, 0, {subject.TheoryHour}, {subject.ConsultationHour});";
                    cmd.CommandText = command;
                    try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }

                    cmd.CommandText = $"select `id` from `subjects` where `name` = '{subject.Name}';";
                    int subjectId = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = $"select `id` from `teachers` where `fullname` = '{teacher.Name}';";
                    int teacherId = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = $"select exists(select `id` from `teachersubject` where `subject_id` = {subjectId} and `teacher_id` = {teacherId});";
                    int recordExists = Convert.ToInt32(cmd.ExecuteScalar());
                    if(recordExists == 0)
                    {
                        cmd.CommandText = $"insert ignore into `teachersubject`(`teacher_id`, `subject_id`) values({teacherId}, {subjectId});";
                        try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
                    }
                    foreach (Tarification.Group group in subject.Groups)
                    {
                        string newCommand = $"select `id` from `specialities` where concat(`code`, ' ', `name`) = '{group.Speciality}';";
                        cmd.CommandText = newCommand;
                        int specId = Convert.ToInt32(cmd.ExecuteScalar());
                        command = $"INSERT ignore INTO `groups` (`name`, `speciality`, `count`) " +
                            $"VALUES ('{group.Name}', {specId}, {group.Count});";
                        cmd.CommandText = command;
                        try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }

                        command = $"select `id` from `subjects` where `name` = '{subject.Name}';";
                        cmd.CommandText = command;
                        subjectId = Convert.ToInt32(cmd.ExecuteScalar());
                        command = $"select `id` from `groups` where `name` = '{group.Name}';";
                        cmd.CommandText = command;
                        int groupId = Convert.ToInt32(cmd.ExecuteScalar());

                        command = $"select exists(select `id` from `groupsubject` where `subject_id` = {subjectId} and `group_id` = {groupId});";
                        cmd.CommandText = command;
                        recordExists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (recordExists == 0)
                        {
                            command = $"insert ignore into `groupsubject`(`subject_id`, `group_id`) values({subjectId}, {groupId});";
                            cmd.CommandText = command;
                            try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
                        }
                    }
                }
            }
            GetAllGroupsId();
            try { Tabletime.idChoosedGroup = Tabletime.idGroups[0]; } catch (Exception ex) { Console.WriteLine(ex.Message); }
            LoadTarification();
        }
        private static void LoadTarification()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            foreach (Tarification.Teacher teacher in Tarification.teachers)
            {
                foreach (Tarification.Subject subject in teacher.Subjects)
                {
                    foreach (Tarification.Group group in subject.Groups)
                    {
                        if (group.Hours[0] == 0 && group.Hours[1] == 0 && group.Hours[2] == 0) continue;
                        string command = $"select `id` from `groups` where `name` = '{group.Name}';";
                        cmd.CommandText = command;
                        int groupId = Convert.ToInt32(cmd.ExecuteScalar());
                        command = $"select `id` from `subjects` where `name` = '{subject.Name}';";
                        cmd.CommandText = command;
                        int subjectId = Convert.ToInt32(cmd.ExecuteScalar());
                        command = $"select `id` from `groupsubject` where `subject_id` = {subjectId} and `group_id` = {groupId};";
                        cmd.CommandText = command;
                        int id = Convert.ToInt32(cmd.ExecuteScalar());

                        command = $"insert ignore into `tarification`(`groupsubject_id`, `theory`, `division`, `consultation`)" +
                            $"values ({id}, {group.Hours[0]}, {group.Hours[1]}, {group.Hours[2]});";
                        cmd.CommandText = command;
                        try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
                    }
                }
            }
        }
        public static void WritePlan(string group) // запись дисциплин в БД
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            int groupId = GetGroupId(group);
            foreach(int semestr in CSVReader.plan.Keys)
            {
                foreach(CSVReader.Subject subject in CSVReader.plan[semestr])
                {
                    int subjectId = 0;
                    cmd.CommandText = $"SELECT `id` from `subjects` where `name` = '{subject.Name}';";
                    var a = cmd.ExecuteScalar();
                    if(a != null) subjectId = Convert.ToInt32(a);
                    else
                    {
                        cmd.CommandText = $"INSERT ignore INTO `subjects` (`name`, `allhourbudjet`, `allhourcorrection`, `allHournotbudjet`, `alltheoryhour`, `allconsultationhour`) " +
                            $"VALUES ('{subject.Name}', default, default, default, default, default);";
                        try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
                        cmd.CommandText = $"SELECT `id` from `subjects` where `name` = '{subject.Name}';";
                        subjectId = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.CommandText = $"insert ignore into `groupsubject`(`subject_id`, `group_id`) values({subjectId}, {groupId});";
                        try { cmd.ExecuteNonQuery(); }catch(MySqlException ex) { Console.WriteLine(ex.Message); }
                    }
                    cmd.CommandText = $"select `id` from `groupsubject` where `subject_id` = {subjectId} and `group_id` = {groupId};";
                    int groupSubjectId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = $"insert ignore into `plans`(`groupsubject_id`, `semestr`, `totalhour`, `independentwork`, `consultation`, `lesson`, `practicalwork`, `labwork`, `kursproject`, `attestation`) " +
                        $"values({groupSubjectId}, {semestr}, {subject.TotalHour}, {subject.IndependentHour}, {subject.ConsultationHour}, {subject.Lesson}," +
                        $"{subject.PracWork}, {subject.LabWork}, {subject.Kurs}, {subject.Attestation});";
                    try { cmd.ExecuteNonQuery(); } catch (MySqlException ex) { Console.WriteLine(ex.Message); }
                }
            }
        }
        public static List<object> TakeData(ushort index)
        {
            string teachers = "select `fullname` as 'ФИО', (select `name` from `states` where `id` = `state`) as 'Состояние', `stake` as 'Ставка', `burden` as 'Нагрузка', " +
                "`allhour` as 'Всего часов', `allhourcorrection` as 'Всего часов коррекции' from `teachers`;";
            string plans = "select (select (select `name` from `groups` where `id` = `group_id`) from `groupsubject` where `id` = `groupsubject_id`) as 'Группа'," +
                "(select (select `name` from `subjects` where `id` = `subject_id`) from `groupsubject` where `id` = `groupsubject_id`) as 'Предмет',`semestr` as 'Семестр', " +
                "`totalhour` as 'Всего часов', `independentwork` as 'Самостоятельная работа', `consultation` as 'Консультация', `lesson` as 'Лекции', `practicalwork` as 'Практическая работа'," +
                "`labwork` as 'Лабораторная работа', `kursproject` as 'Курсовой проект', `attestation` as 'Аттестация' from `plans`;";
            string tarification = "select (select (select (select `fullname` from `teachers` where `id` = `teacher_id`) from `teachersubject` where `subject_id` = `groupsubject`.`subject_id`) " +
                "from `groupsubject` where `id` = `groupsubject_id`) as 'Преподаватель',(select (select `name` from `subjects` where `id` = `subject_id`) " +
                "from `groupsubject` where `id` = `groupsubject_id`) as 'Предмет',(select (select `name` from `groups` where `id` = `group_id`) from `groupsubject` where `id` = `groupsubject_id`) as 'Группа'," +
                "`theory` as 'Теория', `division` as 'Деление по группам', `consultation` as 'Консультации' from `tarification`;";
            string groups = "select `name` as 'Наименование',(select `name` from `specialities` where `id` = `speciality`) as 'Специальность',`count` as 'Количество человек' from `groups`;";

            List<string> commands = new List<string>() { teachers, plans, tarification, groups };
            List<object> data = new List<object>();
            MySqlCommand cmd = new MySqlCommand(commands[index], conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            try { if (!reader.HasRows) throw new Exception("READER не имеет строк!"); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Dispose();
                return new List<object>();
            }
            while (reader.Read())
            {
                try
                {
                    switch (index)
                    {
                        case 0:
                            TeacherModel tm = new TeacherModel();
                            tm.FillValue(reader);
                            data.Add(tm);
                            break;
                        case 1:
                            PlanModel sm = new PlanModel();
                            sm.FillValue(reader);
                            data.Add(sm);
                            break;
                        case 2:
                            TarificationModel rm = new TarificationModel();
                            rm.FillValue(reader);
                            data.Add(rm);
                            break;
                        case 3:
                            GroupsModel gm = new GroupsModel();
                            gm.FillValue(reader);
                            data.Add(gm);
                            break;
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            reader.Dispose();
            return data;
        }
        #region Dropdown'ы
        public static List<string> SpecDropdwn() // вывод в DropDown специальностей групп
        {
            List<string> data = new List<string>();
            string request = "SELECT `code`, `name` FROM `specialities`;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add($"{reader[0]} {reader[1]}");
            reader.Close();
            return data;
        }
        public static List<string> TypeDropdwn(string table) // вывод в DropDown типы групп
        {
            List<string> data = new List<string>();
            string request = $"SELECT `name` FROM `{table}`;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add(reader[0].ToString());
            reader.Close();
            return data;
        }
        public static List<string> GroupsDropdwn() // вывод в DropDown группы
        {
            List<string> data = new List<string>();
            string request = "SELECT `name` FROM `groups`;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add(reader[0].ToString());
            reader.Close();
            return data;
        }
        public static List<string> TchrStateDropdwn()
        {
            List<string> data = new List<string>();
            string request = "SELECT `name` FROM `states`;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add(reader.GetString(0));
            reader.Close();
            return data;
        }
        public static List<string> SubjDropdwn()
        {
            List<string> data = new List<string>();
            string request = "SELECT `name` FROM `subjects`;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add($"{reader.GetString(0)}");
            reader.Close();
            return data;
        }
        #endregion
        public static bool CheckDupl(string table, string column, object value)
        {
            string chck = $"select exists(select * from `{table}` where `{column}` = '{value}');";
            MySqlCommand cmd = new MySqlCommand(chck, conn);
            string a = cmd.ExecuteScalar().ToString();
            if (int.TryParse(a, out int arr) && arr == 0) return false;
            else return true;
        }
        public static void WriteGroup(string group, string spec, int count) // Запись группы в БД
        {
            MySqlCommand cmd = new MySqlCommand() { Connection = conn };
            cmd.CommandText = $"insert into `groups`(`name`, `speciality`, `count`) values" +
                    $"('{group}', (select `id` from `specialities` where concat(`code`, ' ', `name`) = '{spec}'), {count});";
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        public static void WriteRoom(string name, ushort number, string type)
        {
            MySqlCommand cmd = new MySqlCommand() { Connection = conn };
            string insert = $"insert into rooms(`name`, `number`, `type`) values" +
                    $"('{name}'," +
                    $"{number}, " +
                    $"(select `id` from `roomtypes` where `name` = '{type}'));";
            cmd.CommandText = insert;
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        public static void WriteSpec(string code, string name)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"insert ignore into `specialities` (`name`, `code`) values ('{name}', '{code}');";
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        public static void WriteTeacher(string name, string state, double timeJob)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"insert into `teachers`(`fullName`, `state`, `stake`, `burden`, `allhour`, `allhourcorrection`) values (" +
                $"'{name}'," +
                $"(select `id` from `states` where `name` = '{state}')," +
                $"{timeJob}, default, default, default);";
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        //-------------------------------------
        public static void GetData4TableTime(int semestr)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            Tabletime.subjects.Clear();
            Tabletime.teachers.Clear();
            Tabletime.rooms.Clear();
            foreach (int gr in Tabletime.idGroups)
            {
                Tabletime.subjects.Add(gr, new List<Subject>());
                cmd.CommandText = "select(select (select `name` from `subjects` where `id` = `subject_id`) from `groupsubject` where `id` = `groupsubject_id`)," +
                    $"`totalhour` from `plans` where (select `group_id` from `groupsubject` where `id` = `groupsubject_id`) = {gr} and `semestr` = {semestr} order by `totalhour` desc;";
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            if (reader.GetInt32(1) <= 1) continue;
                            Subject sub = new Subject()
                            {
                                Name = reader.GetString(0),
                                TotalHours = reader.GetInt32(1)
                            };
                            Tabletime.subjects[gr].Add(sub);
                        }
                }
            }
            cmd.CommandText = "select `fullName` from `teachers`;";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                    while (reader.Read())
                        Tabletime.teachers.Add(reader.GetString(0));
            }
            cmd.CommandText = "SELECT `number`, `name`, (select `name` from `roomtypes` where `id` = `type`) FROM `rooms`;";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                    while (reader.Read())
                        Tabletime.rooms.Add(new Room
                        {
                            Number = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Type = reader.GetString(2)
                        });
            }
        }
        public static int GetGroupId(string group)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"(SELECT `id` from `groups` where `name` = '{group}')";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}