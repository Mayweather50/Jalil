namespace MySQLConnect.Model
{
    public class TeacherModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public float Timejob { get; set; }
        public string Roomnumber { get; set; }
        public string Subject { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Name = reader.GetString(0);
            State = reader.GetString(1);
            Timejob = reader.GetFloat(2);
            //Roomnumber = reader.GetString(3);
            //Subject = reader.GetString(4);
        }
    }
}
