namespace MySQLConnect.Model
{
    public class RoomModel
    {
        public string Name { get; set; }
        public ushort Number { get; set; }
        public string Type { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Name = reader.GetString(0);
            Number = reader.GetUInt16(1);
            Type = reader.GetString(2);
        }
    }
}
