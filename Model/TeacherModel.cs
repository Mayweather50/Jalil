namespace MySQLConnect.Model
{
    public class TeacherModel
    {
        public string Fullname { get; set; }
        public string State { get; set; }
        public float Stake { get; set; }
        public ushort Burden { get; set; }
        public ushort AllHour { get; set; }
        public ushort AllHourCorrection { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Fullname = reader.GetString(0);
            State = reader.GetString(1);
            Stake = reader.GetFloat(2);
            Burden = reader.GetUInt16(3);
            AllHour = reader.GetUInt16(4);
            AllHourCorrection = reader.GetUInt16(5);
        }
    }
}
