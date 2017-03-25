using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tonception;

namespace DiscoStation
{
    internal class CSVHandler
    {
        Mutex entitiesM = new Mutex();
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
                    List<CSVRecord> records = csv.GetRecords<CSVRecord>().ToList();
                    EntityData ent = new EntityData();
                    EntityTempVariable tempv = new EntityTempVariable();//
                    tempv.varID = -1;//
                    int i = 0;
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
                        if (records.IndexOf(rec) == records.Count) // add the last entity
                        {
                            disco.entities.Add(ent);
                            updateGeneralVariableDict(ent, ref disco);

                        }
                        Console.WriteLine("added " + i + "out of " + records.Count);
                        i++;
                    }
                }
            }
            return disco;
        }

        public TonceptSet ParallelLoadTonceptSetFromCSV(string filePath, TonceptSet disco)
        {
            if(disco==null)
                disco = new TonceptSet();
            HashSet<int> idSet = new HashSet<int>();
            TextReader txtrdr = new StreamReader(filePath);
            txtrdr.ReadLine();
            char[] delim = new char[2] { '\n', '\r' };
            string[] allrec = txtrdr.ReadToEnd().Split(delim,StringSplitOptions.RemoveEmptyEntries);
            txtrdr.Close();
            TextWriter wr;
            Directory.CreateDirectory("temp");
            int tempid = int.Parse(allrec[0].Split(',')[0]);
            List<string> tempList = new List<string>();
            foreach (string s in allrec)
            {
                if (s == null || s == "")
                    continue;
                string[] rec = s.Split(',');
                if (int.Parse(rec[0]) != tempid)
                {
                    wr = new StreamWriter(File.Create(@"temp\" + tempid + ".txt"));
                    foreach (string tmpstr in tempList)
                    {
                        wr.WriteLine(tmpstr);
                    }
                    wr.Close();
                    tempList = new List<string>();
                    idSet.Add(tempid);
                    tempid = int.Parse(rec[0]);
                }
                tempList.Add(s);
            }
            wr = new StreamWriter(File.Create(@"temp\" + tempid + ".txt"));
            foreach (string tmpstr in tempList)
            {
                wr.WriteLine(tmpstr);
            }
            wr.Close();
            tempList = null;
            idSet.Add(tempid);
            Parallel.ForEach(idSet, id =>
            {
                workerThread(id,ref disco);
            });
            disco.entities.Sort(new EntityData.EntityComparer());
            Directory.Delete("temp",true);
            return disco;
        }

        private void workerThread(int id,ref TonceptSet disco)
        {
            TextReader rdr = new StreamReader(@"temp\" + id + ".txt");
            CsvReader csv = new CsvReader(rdr);
            csv.Configuration.HasHeaderRecord = false;
            var generatedMap = new MyClassMap();
            csv.Configuration.RegisterClassMap(generatedMap);
            List<CSVRecord> records = csv.GetRecords<CSVRecord>().ToList();
            csv.Dispose();
            rdr.Close();
            EntityData ent = new EntityData();
            ent.entityID = id;
            foreach (CSVRecord rec in records)
            {
                TimeStampValueSymbol tsv = new TimeStampValueSymbol(rec.TimeStamp, rec.TemporalPropertyValue);
                if (!ent.varDic.Keys.Contains(rec.TemporalPropertyID))
                {
                    ent.varDic.Add(rec.TemporalPropertyID, new EntityTempVariable(rec.TemporalPropertyID));
                }
                ent.varDic[rec.TemporalPropertyID].values.Add(tsv);
            }
            entitiesM.WaitOne();
            disco.entities.Add(ent);
            entitiesM.ReleaseMutex();
            //updateGeneralVariableDict(ent, ref disco);
            Console.WriteLine("done with entity "+id);
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
                // else
                //  disco.tariablesDic[etv.varID].allSize = disco.tariablesDic[etv.varID].allSize + etv.values.Count;
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