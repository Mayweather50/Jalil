namespace MySQLConnect.Model
{
    public class TarificationModel
    {
        public string Teacher { get; set; }
        public string Subject { get; set; }
        public string Group { get; set; }
        public ushort Theory { get; set; }
        public ushort Division { get; set; }
        public ushort Consultation { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Teacher = reader.GetString(0);
            Subject = reader.GetString(1);
            Group = reader.GetString(2);
            Theory = reader.GetUInt16(3);
            Division = reader.GetUInt16(4);
            Consultation = reader.GetUInt16(5);
        }
    }
}
