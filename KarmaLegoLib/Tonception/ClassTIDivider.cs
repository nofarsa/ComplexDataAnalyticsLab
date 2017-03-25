using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tonception
{
    public class ClassTIDivider
    {
        //List<int> classA = new List<int>();
        // List<int> classB = new List<int>();
        ConcurrentDictionary<int, EntityData> classA = new ConcurrentDictionary<int, EntityData>();
        ConcurrentDictionary<int, EntityData> classB = new ConcurrentDictionary<int, EntityData>();
        TextWriter writer1;
        TextWriter writer2;
        public void start(string id1path, string id2path, string tipath, string outputName)
        {
            checkExistence(id1path, id2path, tipath);
            Thread t1= new Thread(s=> fillID(id1path,ref classA));
            Thread t2= new Thread(s => fillID(id2path, ref classB));
            t1.Name = "t1";
            t2.Name = "t2";
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            //fillID(id1path,ref classA);
            //fillID(id2path, ref classB);
            string dir = @"C:\\Users\\nahmiasd\\Documents\\KL\\";
            string tiA = dir+outputName + "ClassA.csv";
            string tiB = dir+outputName + "ClassB.csv";
            FileStream f1= File.Create(tiA);
            FileStream f2 = File.Create(tiB);
            f1.Close();
            f2.Close();
            writer1 = new StreamWriter(tiA);
            writer2 = new StreamWriter(tiB);
            WriteBoth("numberOfEntities," + (classA.Count + classB.Count));
            TextReader rdr = new StreamReader(tipath);
            string all = rdr.ReadToEnd();
            string[] delim = new string[] { "\r\n" };
            string[] records = all.Split(delim,StringSplitOptions.None);
            Parallel.For(1, records.Length, index =>
             {
                 if (records[index] == "")
                     return;
                 string[] fields = records[index].Split(',');
                 int entID = int.Parse(fields[0]);
                 int varID = int.Parse(fields[1]);
                 int symbol = int.Parse(fields[2]);
                 int sTime = int.Parse(fields[3]);
                 int eTime = int.Parse(fields[4]);
                 TimeIntervalSymbol tis = new TimeIntervalSymbol(sTime, eTime, symbol);
                 if (!(classA.ContainsKey(entID)) && !(classB.ContainsKey(entID)))
                     Console.WriteLine(entID+ " could not be found");
                 if (classA.ContainsKey(entID))
                     classA[entID].ParallelAddTIS(tis);
                 else
                     classB[entID].ParallelAddTIS(tis);
             });
            Parallel.ForEach(classA.Values, ent =>
            {
                ent.symbolic_intervals.Sort(TimeIntervalSymbol.compareTIS);
            });
            Parallel.ForEach(classB.Values, ent =>
            {
                ent.symbolic_intervals.Sort(TimeIntervalSymbol.compareTIS);
            });
            t1 = new Thread(s => writeTIs(0));
            t2 = new Thread(s => writeTIs(1));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            //rdr.ReadLine();
            //string line = rdr.ReadLine();
            //while (line != "" && line != null)
            //{
            //    string[] fields = line.Split(',');
            //    int entID = int.Parse(fields[0]);
            //    int varID = int.Parse(fields[1]);
            //    int symbol = int.Parse(fields[2]);
            //    int sTime = int.Parse(fields[3]);
            //    int eTime = int.Parse(fields[4]);
            //    TimeIntervalSymbol tis = new TimeIntervalSymbol(sTime, eTime, symbol);
            //    if (classA.Contains(entID))
            //    {
            //        write
            //    }
        }

        private void checkExistence(string id1path, string id2path, string tipath)
        {
            if (!File.Exists(id1path))
                throw new Exception("id1 invalid");
            if (!File.Exists(id2path))
                throw new Exception("id2 invalid");
            if (!File.Exists(tipath))
                throw new Exception("ti file invalid");
        }

        private void fillID(string idpath, ref ConcurrentDictionary<int, EntityData> list)
        {
            if (!File.Exists(idpath))
            {
                throw new Exception("invalid id file");
            }
            TextReader rdr = new StreamReader(idpath);
            string readline = rdr.ReadLine();
            while ((readline=rdr.ReadLine())!= null)
            {
                int id;
                if (int.TryParse(readline, out id))
                {
                    if (list.TryAdd(id, new EntityData()))
                        Console.WriteLine("Thread " + Thread.CurrentThread.Name + " wrote id " + id);
                    else
                    {
                        Console.WriteLine(id + " faild");
                    }
                }
                else
                {
                    Console.WriteLine("parsing faild");
                }
            }
            rdr.Close();
        }
        private void WriteBoth(string line)
        {
            if (writer1 != null && writer2 != null)
            {
                writer1.WriteLine(line);
                writer2.WriteLine(line);
            }
        }
        private void writeTIs(int classval)
        {
            TextWriter writer;
            ConcurrentDictionary<int, EntityData> dict;
            if (classval == 0)
            {
                writer = writer1;
                dict = classA;
            }
            else
            {
                writer = writer2;
                dict = classB;
            }
            foreach (int entID in dict.Keys)
            {
                writer.WriteLine(entID + ";");
                string writeline = "";
                for(int i = 0; i < dict[entID].symbolic_intervals.Count; i++)
                {
                    TimeIntervalSymbol tis = dict[entID].symbolic_intervals[i];
                    writeline=writeline+tis.startTime + "," + tis.endTime + "," + tis.symbol + ";";
                }
                    writer.WriteLine(writeline);
            }
                
        }
    }
}

