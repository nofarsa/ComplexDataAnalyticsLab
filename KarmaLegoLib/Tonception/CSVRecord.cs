namespace DiscoStation
{
    public class CSVRecord
    {
        public int EntityID { get; set; }
        public int TemporalPropertyID { get; set; }
        public int TimeStamp { get; set; }
        public double TemporalPropertyValue { get; set; }

        public CSVRecord(int id, int propid, int tstamp, double tempval)
        {
            EntityID = id;
            TemporalPropertyID = propid;
            TimeStamp = tstamp;
            TemporalPropertyValue = tempval;
        }

        public CSVRecord()
        {
        }
    }
}