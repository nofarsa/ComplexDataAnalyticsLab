using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tonception;

namespace DiscoStation
{
    internal class CSVHandler
    {
        public static TonceptSet LoadTonceptSetFromCSV(string filePath)
        {
            TonceptSet disco = new TonceptSet();
            using (TextReader txtrdr = new StreamReader(filePath))
            {
                using (CsvReader csv = new CsvReader(txtrdr))
                {
                    List<int> idList = new List<int>();
                    var generatedMap = new MyClassMap();
                    csv.Configuration.RegisterClassMap(generatedMap);
                    List<CSVRecord> records = csv.GetRecords<CSVRecord>().ToList<CSVRecord>();
                    EntityData ent = new EntityData();
                    EntityTempVariable tempv = new EntityTempVariable();//
                    tempv.varID = -1;//
                    foreach (CSVRecord rec in records)
                    {
                        if (!idList.Contains(rec.EntityID))// new entity
                        {
                            idList.Add(rec.EntityID);
                            if (ent.entityID != 0)
                            {
                                disco.entities.Add(ent);
                                updateGeneralVariableDict(ent, ref disco);
                            }
                            ent = new EntityData();
                            ent.entityID = rec.EntityID;
                        }
                        TimeStampValueSymbol tsv = new TimeStampValueSymbol(rec.TimeStamp, rec.TemporalPropertyValue);
                        if (!ent.varDic.Keys.Contains(rec.TemporalPropertyID))
                        {
                            ent.varDic.Add(rec.TemporalPropertyID, new EntityTempVariable(rec.TemporalPropertyID));
                        }
                        ent.varDic[rec.TemporalPropertyID].values.Add(tsv);
                    }
                }
            }
            return disco;
        }

        private static void updateGeneralVariableDict(EntityData ent, ref TonceptSet disco)
        {
            foreach (EntityTempVariable etv in ent.varDic.Values)
            {
                if (!disco.tariablesDic.ContainsKey(etv.varID)) // varSizes.ContainsKey(tv.varID))
                {
                    TempVarInfo tvi = new TempVarInfo(etv.varID, etv.varName);
                    disco.tariablesDic.Add(etv.varID, tvi); // varSizes.Add(tv.varID, tv.values.Count);
                }
                //else
                    //disco.tariablesDic[etv.varID].allSize = disco.tariablesDic[etv.varID].allSize + etv.values.Count;
            }
        }
    }

    public sealed class MyClassMap : CsvClassMap<CSVRecord>
    {
        public MyClassMap()
        {
            Map(m => m.EntityID).Index(0);
            Map(m => m.TemporalPropertyID).Index(1);
            Map(m => m.TimeStamp).Index(2);
            Map(m => m.TemporalPropertyValue).Index(3);
        }
    }
}