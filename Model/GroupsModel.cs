namespace MySQLConnect.Model
{
    public class GroupsModel
    {
        public string Name { get; set; }
        public string Speciality { get; set; }
        public uint Count { get; set; }
        public string Type { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Name = reader.GetString(0);
            Speciality = reader.GetString(1);
            Count = reader.GetUInt16(2);
            Type = reader.GetString(3);
        }
    }
}
