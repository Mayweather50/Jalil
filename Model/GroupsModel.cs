namespace MySQLConnect.Model
{
    public class GroupsModel
    {
        public string Name { get; set; }
        public string Speciality { get; set; }
        public ushort Count { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Name = reader.GetString(0);
            Speciality = reader.GetString(1);
            Count = reader.GetUInt16(2);
        }
    }
}
