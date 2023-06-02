namespace MySQLConnect.Model
{
    public class SubjectModel
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Index = reader.GetString(0);
            Name = reader.GetString("Name");
            Type = reader.GetString(2);
        }
    }
}
