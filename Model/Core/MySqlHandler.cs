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
            Tabletime.idChoosedGroup = Tabletime.idGroups[0];
            return true;
        }
        public static void GetAllGroupsId() // Получение всех id'шников групп
        {
            MySqlCommand cmd = new MySqlCommand() { Connection = conn };
            cmd.CommandText = "select `id` from _groups;";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Tabletime.idGroups.Add(reader.GetInt32(0));
                    }
            }
        }
        private static void CheckTables()// Определяет, хватает ли таблиц(доработать, пусть стучится до каждой таблицы)
        {
            int tablesCount = 0;
            string sql = $"show tables from {conn.Database} like '_%';";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) tablesCount++;
            reader.Close();
            if (tablesCount < 12) CreateTables();
        }
        private static void CreateTables()// Создание таблиц при их нехватке
        {
            string speciality = "CREATE TABLE IF NOT EXISTS `_Speciality` ( " +
                                    "`id` int NOT NULL AUTO_INCREMENT ," +
                                    "`code` VARCHAR(255) NOT NULL COMMENT 'Код специальности' ," +
                                    "`name` VARCHAR(255) NOT NULL COMMENT 'Наименование специальности' ," +
                                    "PRIMARY KEY (`id`)) ENGINE = InnoDB; ";

            string groups = "CREATE TABLE IF NOT EXISTS `_groups` ( " +
                            "`id` int NOT NULL AUTO_INCREMENT ," +
                            "`name` VARCHAR(45) NOT NULL COMMENT 'Индекс группы' ," +
                            "`speciality` int NOT NULL COMMENT 'Наименование специальности' ," +
                            "`count` int NOT NULL COMMENT 'Кол-во студентов' ," +
                            "`type` int NOT NULL COMMENT 'Тип группы' ," +
                            "FOREIGN KEY (speciality)  REFERENCES _Speciality (id) on update cascade on delete restrict ," +
                            "FOREIGN KEY (type)  REFERENCES _groupTypes (id) on update cascade on delete restrict," +
                            "PRIMARY KEY (`id`))  ENGINE = InnoDB; ";

            string teachers = "CREATE TABLE IF NOT EXISTS `_Teachers` ( " +
                                    "`id` int NOT NULL AUTO_INCREMENT ," +
                                    "`fullName` VARCHAR(255) NOT NULL COMMENT 'ФИО' ," +
                                    "`state` int NOT NULL DEFAULT '1' COMMENT 'Состояние' ," +
                                    "`timeJob` double NOT NULL DEFAULT '1' COMMENT 'Коэфициент ставки' ," +
                                    "FOREIGN KEY (state)  REFERENCES _teacherstates (id) on update cascade on delete restrict," +
                                    "PRIMARY KEY (`id`))  ENGINE = InnoDB; ";

            string teachersRoom = "CREATE TABLE IF NOT EXISTS `_TeachersRoom` ( " +
                                    "`teacher` int NOT NULL COMMENT 'Преподаватель' ," +
                                    "`room` int NOT NULL COMMENT 'Аудитория' ," +
                                    "FOREIGN KEY (room)  REFERENCES _Rooms (id) on update cascade on delete restrict," +
                                    "FOREIGN KEY (teacher)  REFERENCES _Teachers (id) on update cascade on delete restrict) ENGINE = InnoDB; ";

            string teachersSubject = "CREATE TABLE IF NOT EXISTS `_TeachersSubject` ( " +
                                    "`subject` int NOT NULL COMMENT 'Дисциплина' ," +
                                    "`teacher` int NOT NULL COMMENT 'Преподаватель' ," +
                                    "FOREIGN KEY (subject)  REFERENCES _Subjects (`id`) on update cascade on delete restrict," +
                                    "FOREIGN KEY (teacher)  REFERENCES _Teachers (id) on update cascade on delete restrict) ENGINE = InnoDB; ";

            string subjects = "CREATE TABLE IF NOT EXISTS `_Subjects` ( " +
                                    "`id` int not null auto_increment," +
                                    "`index` VARCHAR(45) NOT NULL COMMENT 'Индекс дисциплины'," +
                                    "`name` VARCHAR(255) NOT NULL COMMENT 'Наименование дисциплины' ," +
                                    "`group` int NULL COMMENT 'Группа' ," +
                                    "`roomType` int NULL COMMENT 'Тип аудитории' ," +
                                    "FOREIGN KEY (group)  REFERENCES _groups (id) on update cascade on delete cascade," +
                                    "FOREIGN KEY (roomType)  REFERENCES _roomTypes (id) on update cascade on delete restrict," +
                                    "PRIMARY KEY (`id`))  ENGINE = InnoDB; ";

            string roomTypes = "CREATE TABLE IF NOT EXISTS `_roomTypes` ( " +
                                    "`id` int NOT NULL AUTO_INCREMENT ," +
                                    "`type` varchar(255) NOT NULL COMMENT 'Наименование типа' ," +
                                    "PRIMARY KEY (`id`))  ENGINE = InnoDB;" +
                               "insert into _roomtypes(`type`) values" +
                                       "('Лекционный')," +
                                       "('Компьютерный')," +
                                       "('Спортзал');";

            string rooms = "CREATE TABLE IF NOT EXISTS `_Rooms` ( " +
                                    "`id` int NOT NULL AUTO_INCREMENT ," +
                                    "`name` varchar(255) NOT NULL DEFAULT 'Кабинет' COMMENT 'Наименование кабинета' ," +
                                    "`number` int NOT NULL COMMENT 'Номер кабинета' ," +
                                    "`type` int NOT NULL COMMENT 'Тип' ," +
                                    "FOREIGN KEY(`type`)  REFERENCES _roomTypes(id) on update cascade on delete restrict," +
                                    "PRIMARY KEY (`id`))  ENGINE = InnoDB; ";

            string plans = $"CREATE TABLE IF NOT EXISTS `_plans` (" +
                                        "`index` varchar(255) not null, " +
                                        "`group` varchar(45) not null, " +
                                        "`totalHour` varchar(255) not null, " +
                                        "`independentWork` varchar(255) not null, " +
                                        "`consultations` varchar(255) not null, " +
                                        "`lessons` varchar(255) not null, " +
                                        "`practicalWork` varchar(255) not null, " +
                                        "`laboratoryWork` varchar(255) not null," +
                                        "`kursPojectDo` varchar(255) null, " +
                                        "`attestation` varchar(255) not null, " +
                                        "PRIMARY KEY(`index`))ENGINE = InnoDB;";

            string groupTypes = "CREATE TABLE IF NOT EXISTS `_groupTypes` (" +
                                        "`id` int not null AUTO_INCREMENT, " +
                                        "`type` varchar(45) not null, " +
                                        "PRIMARY KEY(`id`))ENGINE = InnoDB;" +
                                 "insert into _grouptypes(`type`) values" +
                                        "('Специалисты среднего звена')," +
                                        "('Квалифицированные рабочие служащие')," +
                                        "('Лица с ограниченными возможностями здоровья');";

            string teacherStates = "CREATE TABLE IF NOT EXISTS `_teacherStates` (" +
                                            "`id` int NOT NULL AUTO_INCREMENT ," +
                                            "`name` varchar(255) NOT NULL COMMENT 'Наименование Состояния' ," +
                                            "PRIMARY KEY (`id`))  ENGINE = InnoDB;" +
                                   "insert into _teacherStates(`name`) values" +
                                        "('Работает')," +
                                        "('На больничном')," +
                                        "('В командировке')," +
                                        "('В отпуске');";

            List<string> commands = new List<string>() { teacherStates, speciality, teachers, roomTypes, groupTypes, groups, subjects, rooms, teachersSubject, teachersRoom, plans };

            MySqlCommand cmd;
            for (int i = 0; i < commands.Count; i++)
            {
                cmd = new MySqlCommand(commands[i], conn);
                cmd.ExecuteNonQuery();
            }
        }
        public static void WritePlan(string group, List<CSVReader.Lesson> lessons) // запись дисциплин в БД
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            int grpId = GetGroupId(group);
            if (grpId == null)
            {
                Console.WriteLine("Ошибка с получением ID группы!");
                return;
            }
            cmd.CommandText = $"DELETE from _plans where _plans.group = '{grpId}';"; // Очистка старого учебного плана этой группы            
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            foreach (CSVReader.Lesson lesson in lessons)
            {
                string insert = $"INSERT INTO `_plans` (`index`, `group`, `totalHour`, `independentWork`, `consultations`, `lessons`, " +
                    $"`practicalWork`, `laboratoryWork`,`kursPojectDo`, `attestation`)" +
                $"VALUES ('{lesson.index}'," +
                $"{grpId}," +
                $"'{lesson.semestr.allHours}', '{lesson.semestr.samostoyatelnie}', '{lesson.semestr.konsultacii}'," +
                $"'{lesson.semestr.lekcii}', '{lesson.semestr.prackticheskie}', '{lesson.semestr.laboratornie}', '{lesson.semestr.kursProjectDo}'," +
                $"'{lesson.semestr.promezhAttestation}');";
                cmd.CommandText = insert;
                try { cmd.ExecuteNonQuery(); }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"WritePlan {ex.Message}");
                    continue;
                }
                WriteSubjects(lesson.index, lesson.name);
            }
            Console.WriteLine("План успешно загружен!");
        }
        private static void WriteSubjects(string index, string name)
        {
            string insert = $"insert ignore into _subjects(`index`, `name`) values ('{index}', '{name}');";
            MySqlCommand cmd = new MySqlCommand(insert, conn);
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine($"WriteSubjects {ex.Message}");
                return;
            }
        }
        public static List<object> TakeData(ushort index)
        {
            string selTeachers = "SELECT `fullName`," +
                "(select `name` from _teacherstates where _teacherstates.id = _teachers.state), " +
                "`timeJob`, " +
                "(select `number` from _Rooms where _Rooms.id = (select `room` from _TeachersRoom where _TeachersRoom.teacher = _teachers.id)), " +
                "(select `subject` from _TeachersSubject where _TeachersSubject.teacher = _teachers.id) from _teachers;";
            string selRooms = "SELECT `name`, `number`, (select `type` from _roomtypes where _roomtypes.id = _Rooms.`type`) from _Rooms;";
            string selSubjects = "SELECT `index`, `name`, (select `type` from _roomtypes where _roomtypes.id = _Subjects.`roomtype`) from _Subjects;";
            string selGroups = "SELECT `name`, (select `name` from _speciality where _speciality.`id` = _groups.`speciality`), `count`," +
                "(select `type` from _grouptypes where _grouptypes.`id` = _groups.`type`) from _groups;";

            List<string> commands = new List<string>() { selTeachers, selRooms, selSubjects, selGroups };
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
                        case 3:
                            GroupsModel gm = new GroupsModel();
                            gm.FillValue(reader);
                            data.Add(gm);
                            break;
                        case 2:
                            SubjectModel sm = new SubjectModel();
                            sm.FillValue(reader);
                            data.Add(sm);
                            break;
                        default:
                            RoomModel rm = new RoomModel();
                            rm.FillValue(reader);
                            data.Add(rm);
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
            string request = "SELECT `code`, `name` FROM _speciality;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add($"{reader[0]} {reader[1]}");
            reader.Close();
            return data;
        }
        public static List<string> TypeDropdwn(string table) // вывод в DropDown типы групп
        {
            List<string> data = new List<string>();
            string request = $"SELECT `type` FROM {table};";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add(reader[0].ToString());
            reader.Close();
            return data;
        }
        public static List<string> GroupsDropdwn() // вывод в DropDown группы
        {
            List<string> data = new List<string>();
            string request = "SELECT `name` FROM _groups;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add(reader[0].ToString());
            reader.Close();
            return data;
        }
        public static List<string> TchrStateDropdwn()
        {
            List<string> data = new List<string>();
            string request = "SELECT `name` FROM _teacherstates;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                    data.Add(reader.GetString(0));
                }
            reader.Close();
            return data;
        }
        public static List<string> GroupDropdwn()
        {
            List<string> data = new List<string>();
            string request = "SELECT `name` FROM _groups;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                    data.Add(reader.GetString(0));
                }
            reader.Close();
            return data;
        }
        public static List<string> SubjDropdwn()
        {
            List<string> data = new List<string>();
            string request = "SELECT `index`, `name` FROM _subjects;";
            MySqlCommand cmd = new MySqlCommand(request, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) while (reader.Read()) data.Add($"{reader.GetString(0)} {reader.GetString(1)}"); //{reader.GetString(2)} - группа
            reader.Close();
            return data;
        }
        #endregion
        public static bool CheckDupl(string table, string column, object value)
        {
            string chck = $"select exists(select * from {table} where `{column}` = '{value}');";
            MySqlCommand cmd = new MySqlCommand(chck, conn);
            string a = cmd.ExecuteScalar().ToString();
            if (int.TryParse(a, out int arr) && arr == 0) return false;
            else return true;
        }
        public static void WriteGroup(string group, string spec, int count, string type) // Запись группы в БД
        {
            MySqlCommand cmd = new MySqlCommand() { Connection = conn};
            cmd.CommandText = $"insert into _groups(`name`, `speciality`, `count`, `type`) values" +
                    $"('{group}', (select `id` from _speciality where concat(`code`, ' ', `name`) = '{spec}')," +
                    $"{count}, (select `id` from _grouptypes where `type` = '{type}'));";
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Группа успешно записана!");
        }
        public static void WriteRoom(string name, ushort number, string type)
        {
            MySqlCommand cmd = new MySqlCommand() { Connection = conn };
            string insert = $"insert into _rooms(`name`, `number`, `type`) values" +
                    $"('{name}'," +
                    $"{number}, " +
                    $"(select `id` from _roomtypes where `type` = '{type}'));";
            cmd.CommandText = insert;
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Аудитория успешно добавлена!");
        }
        public static void WriteSpec(string code, string name)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"insert into _speciality(`code`, `name`) values ('{code}', '{name}');";
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Специальность успешно добавлена!");
        }
        public static void WriteTeacher(string name, string type, string state, double timeJob, string group, string subject)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"insert into _teachers(`fullName`, `type`, `state`, `timeJob`, `group`) values (" +
                $"'{name}'," +
                $"(select `id` from _teachertypes where `type` = '{type}')," +
                $"(select `id` from _teacherstates where _teacherstates.name = '{state}')," +
                $"{timeJob}," +
                $"(select `id` from _groups where `name` = '{group}'));" +
                $"insert into _teacherssubject(`subject`, `teacher`) values (" +
                $"(select `index` from _subjects where concat(`index`, ' ', `name`) = '{subject}')," +
                $"(select `id` from _teachers where `fullName` = '{name}'));";
            try { cmd.ExecuteNonQuery(); }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Преподаватель успешно добавлен!");
        }
        public static void GetData4TableTime()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            foreach (int gr in Tabletime.idGroups)
            {
                Tabletime.subjects.Add(gr, new List<Subject>());
                cmd.CommandText = "select `index`, (select `name` from _subjects where _subjects.`index` = _plans.`index`), `totalHour`," +
                "(select `type` from _roomtypes where _roomtypes.`id` = (select `roomType` from _subjects where _subjects.`index` = _plans.`index`))" +
                $"from _plans where `group` = {gr} order by `totalHour` desc;";
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            if (reader.GetInt32(2) <= 1) continue;
                            Subject sub = new Subject()
                            {
                                Name = reader.GetString(1),
                                TotalHours = reader.GetInt32(2)
                            };
                            Tabletime.subjects[gr].Add(sub);
                        }
                }
            }
            cmd.CommandText = "select `fullName` from _teachers;";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                    while (reader.Read())
                        Tabletime.teachers.Add(reader.GetString(0));
            }
            cmd.CommandText = "SELECT `number`, `name`, (select `type` from _roomtypes where _roomtypes.`id` = _rooms.`type`) FROM _rooms;";
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
            cmd.CommandText = $"(SELECT `id` from _groups where `name` = '{group}')";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}