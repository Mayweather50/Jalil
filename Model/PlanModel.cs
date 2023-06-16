namespace MySQLConnect.Model
{
    public class PlanModel
    {
        public string Group { get; set; }
        public string Subject { get; set; }
        public ushort Semestr { get; set; }
        public ushort TotalHour { get; set; }
        public ushort IndependentWork { get; set; }
        public ushort Consultation { get; set; }
        public ushort Lesson { get; set; }
        public ushort PracWork { get; set; }
        public ushort LabWork { get; set; }
        public ushort Kurs { get; set; }
        public ushort Attestation { get; set; }

        public void FillValue(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Group = reader.GetString(0);
            Subject = reader.GetString(1);
            Semestr = (ushort)(reader.GetUInt16(2) + 1);
            TotalHour = reader.GetUInt16(3);
            IndependentWork = reader.GetUInt16(4);
            Consultation = reader.GetUInt16(5);
            Lesson = reader.GetUInt16(6);
            PracWork = reader.GetUInt16(7);
            LabWork = reader.GetUInt16(8);
            Kurs = reader.GetUInt16(9);
            Attestation = reader.GetUInt16(10);
        }
    }
}
