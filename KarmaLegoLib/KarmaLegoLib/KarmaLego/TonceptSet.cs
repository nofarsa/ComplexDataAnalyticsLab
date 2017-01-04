using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KarmaLegoLib
{
    public class TonceptSet
    {
        public string datasetName;
        public Dictionary<int, int> varSizes = new Dictionary<int, int>(); // number of entities having the variable
        public List<KarmaLegoLib.EntityData> entities = new List<KarmaLegoLib.EntityData>();

        public void saveTonceptSetTIsToFile(string filePath)
        {
            try
            {
                string writeLine = "";
                TextWriter tw = new StreamWriter(filePath);
                tw.WriteLine(datasetName);
                //print the variables dictionary?
                for (int eIdx = 0; eIdx < entities.Count; eIdx++)
                {
                    EntityData entity = entities.ElementAt(eIdx);
                    writeLine = entity.entityID + "," + entity.entityINDEX + ";";
                    for (int tiIdx = 0; tiIdx < entity.symbolic_intervals.Count; tiIdx++)
                        writeLine = writeLine + entity.symbolic_intervals.ElementAt(tiIdx).startTime + "," + entity.symbolic_intervals.ElementAt(tiIdx).endTime + "," + entity.symbolic_intervals.ElementAt(tiIdx).symbol + ";";
                    tw.WriteLine(writeLine);
                }
                tw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public void readTonceptSetTIsFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;
                string readLine = "";
                TextReader tr = new StreamReader(filePath);
                datasetName = tr.ReadLine();
                //read the variables dictionary?
                while( tr.Peek() >= 0 ) //read the entities and their symbolic time intervals
                {
                    readLine = tr.ReadLine();
                    EntityData entity = new EntityData();
                    string[] mainDelimited = readLine.Split(';');
                    entity.entityID = int.Parse( mainDelimited[0].Split(',')[0] );
                    entity.entityINDEX = int.Parse( mainDelimited[0].Split(',')[1] );
                    for (int i = 1; i < mainDelimited.Length-1; i++)
                    {
                        string[] tisDelimited = mainDelimited[i].Split(',');
                        TimeIntervalSymbol tis = new TimeIntervalSymbol(int.Parse(tisDelimited[0]), int.Parse(tisDelimited[1]), int.Parse(tisDelimited[2]));
                        entity.symbolic_intervals.Add(tis);
                    }
                    entities.Add(entity);
                }
                tr.Close();
            }
            catch (Exception e) 
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public void saveTonceptSetTSsToFile(string filePath)
        {
            try
            {
                string writeLine = "";
                TextWriter tw = new StreamWriter(filePath);
                tw.WriteLine(datasetName);
                //print the variables dictionary?
                for (int vIdx = 0; vIdx < varSizes.Count; vIdx++)
                    tw.WriteLine( varSizes.Keys.ElementAt(vIdx) + "," + varSizes.Values.ElementAt(vIdx) );
                tw.WriteLine("entitiesVariablesData");
                for (int eIdx = 0; eIdx < entities.Count; eIdx++)
                {
                    EntityData entity = entities.ElementAt(eIdx);
                    tw.WriteLine(entity.entityID + "," + entity.entityINDEX);
                    for (int tvIdx = 0; tvIdx < entity.varDic.Count; tvIdx++)
                    {
                        tw.WriteLine(entity.varDic.ElementAt(tvIdx).Key);
                        EntityTempVariable tv = entity.varDic.ElementAt(tvIdx).Value;
                        writeLine = "";
                        for (int tsIdx = 0; tsIdx < tv.values.Count; tsIdx++)
                            writeLine = writeLine + tv.values[tsIdx].symbol + "," + tv.values[tsIdx].timestamp + "," + tv.values[tsIdx].value + ";";
                        tw.WriteLine(writeLine);
                    }
                    tw.WriteLine("enDity");
                }
                tw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public void readTonceptSetTSsFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;
                string readLine = "";
                TextReader tr = new StreamReader(filePath);
                datasetName = tr.ReadLine();
                while (tr.Peek() >= 0) //read dictinary varSizes
                {
                    readLine = tr.ReadLine();
                    if (readLine == "entitiesVariablesData")
                        break;
                    string[] delimVec = readLine.Split(',');
                    varSizes.Add(int.Parse(delimVec[0]), int.Parse(delimVec[1]));
                }
                
                while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
                {
                    readLine = tr.ReadLine();
                    EntityData entity = new EntityData();
                    string[] delimVec = readLine.Split(',');
                    entity.entityID = int.Parse(delimVec[0]);
                    entity.entityINDEX = int.Parse(delimVec[1]);
                    while(tr.Peek() >= 0)
                    {
                        readLine = tr.ReadLine();
                        if (readLine.EndsWith("enDity"))
                            break;
                        EntityTempVariable tv = new EntityTempVariable();
                        tv.varID = int.Parse(readLine);
                        readLine = tr.ReadLine();
                        string[] delimitedValsVec = readLine.Split(';');
                        for(int i = 0; i < delimitedValsVec.Length-1; i++)
                        {
                            TimeStampValueSymbol tsvs = new TimeStampValueSymbol(int.Parse(delimitedValsVec[i].Split(',')[1]), double.Parse(delimitedValsVec[i].Split(',')[2]));
                            tsvs.symbol = int.Parse(delimitedValsVec[i].Split(',')[0]);
                            tv.values.Add(tsvs);
                        }
                        entity.varDic.Add(tv.varID, tv);
                    }
                    entities.Add(entity);
                }
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public double[] getValuesAsDoubleVectorOfVariable(int varId)
        {
            double[] dVec = new double[varSizes[varId]];
            int idx = 0;
            for (int i = 0; i < entities.Count; i++)
            {
                if( entities[i].varDic.ContainsKey(varId) )
                    for (int j = 0; j < entities[i].varDic[varId].values.Count; j++)
                        dVec[idx++] = entities[i].varDic[varId].values[j].value;
            }
            return dVec;
        }

        public TimeStampValueSymbol[] getTimeStampValuesVectorOfVariable(int varId)
        {
            //double[] dVec = new double[varSizes[varId]];
            TimeStampValueSymbol[] tsVec = new TimeStampValueSymbol[varSizes[varId]];
            int idx = 0;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].varDic.ContainsKey(varId))
                    for (int j = 0; j < entities[i].varDic[varId].values.Count; j++)
                    {
                        tsVec[idx++] = entities[i].varDic[varId].values[j];//.value;
                    }
            }
            return tsVec;
        }

    }

    public class TempVarInfo
    {
        int varID;
        int verSupport;
        //List<
    }

    public class TimeStampValueSymbol
    {
        public int timestamp;
        public double value;
        public int symbol;
        // parameter name?

        public TimeStampValueSymbol(int setTimeStamp, double setValue)
        {
            timestamp = setTimeStamp;
            value = setValue;
        }
    }

    public class EntityTempVariable
    {
        public int varID;
        public List<TimeStampValueSymbol> values = new List<TimeStampValueSymbol>();
        public List<KarmaLegoLib.TimeIntervalSymbol> intervals = new List<KarmaLegoLib.TimeIntervalSymbol>();

        /// <summary>
        /// Discretizes a single value by given cuts list
        /// </summary>
        /// <param name="value">value to be discretized</param>
        /// <param name="cuts">cuts list (levels definitions)</param>
        /// <returns>discretized value (level number)</returns>
        public static int getDiscretizedValue(double value, double[] cuts)
        {
            int i = 0;
            while (i < cuts.Length && value >= cuts[i])
                i++;
            return i + 1;
        }

        public void symbolizeValues(double[] cuts)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i].symbol = getDiscretizedValue(values[i].value, cuts);
            } // for values
        }

        public void DisConcatenateData(int time_gap)
        {
            for (int i = 0; i < values.Count; i++)
            {
                int j = i + 1;
                while (values[j].symbol == values[i].symbol && values[j].timestamp - values[i].timestamp < 1 + time_gap)
                    j++;
                KarmaLegoLib.TimeIntervalSymbol tis = new KarmaLegoLib.TimeIntervalSymbol(values[i].timestamp, values[j].timestamp, values[i].symbol);
                intervals.Add(tis);
                i = j;
            }
        }
    }

    public class EntityData
    {
        public int entityID;
        public int entityINDEX;
        public Dictionary<int, EntityTempVariable> varDic = new Dictionary<int, EntityTempVariable>();
        public List<TimeIntervalSymbol> symbolic_intervals = new List<TimeIntervalSymbol>();
    }

    public class TimeIntervalSymbol
    {
        public int startTime;
        public int endTime;
        public int symbol;

        public TimeIntervalSymbol(int setStartTime, int setEndTime, int setSymbol)
        {
            startTime = setStartTime;
            endTime   = setEndTime;
            symbol    = setSymbol;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + symbol.GetHashCode();
            hash = (hash * 7) + startTime.GetHashCode();
            hash = (hash * 7) + endTime.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            TimeIntervalSymbol other = obj as TimeIntervalSymbol;
            return (other != null && startTime == other.startTime && endTime == other.endTime);
        }

        public static bool operator ==(TimeIntervalSymbol a, TimeIntervalSymbol b)
        {
            if (a.startTime == b.startTime && a.endTime == b.endTime && a.symbol == b.symbol)
                return true;
            else
                return false;
        }

        public static bool operator !=(TimeIntervalSymbol a, TimeIntervalSymbol b)
        {
            if (a.startTime != b.startTime || a.endTime != b.endTime || a.symbol != b.symbol)
                return true;
            else
                return false;
        }

        public static int compareTIS(TimeIntervalSymbol A, TimeIntervalSymbol B) // A<B?
        {
            if (A.startTime < B.startTime)
                return -1;
            else if (A.startTime == B.startTime && A.endTime < B.endTime)
                return -1;
            else if (A.startTime == B.startTime && A.endTime == B.endTime && A.symbol < B.symbol)
                return -1;
            else
                return 1;
        }
    }
}
