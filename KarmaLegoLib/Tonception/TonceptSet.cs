using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

//using KarmaLegoLib;

namespace Tonception
{
    public class TonceptSet
    {
        public string datasetName;

        //public Dictionary<int, int> varSizes = new Dictionary<int, int>(); // number of entities having the variable
        public Dictionary<int, TempVarInfo> tariablesDic = new Dictionary<int, TempVarInfo>();
        public HashSet<int> classKeys = new HashSet<int>();
        public List<EntityData> entities = new List<EntityData>();
        public int classSeparator = -1;
        public bool binsOnly = false;
        public Mutex entM = new Mutex();
        public Dictionary<int, List<TimeIntervalSymbol>> getEntitiesTISs(string filePath)
        {
            try
            {
                Dictionary<int, List<TimeIntervalSymbol>> entityTISs = new Dictionary<int, List<TimeIntervalSymbol>>();
                for (int eIdx = 0; eIdx < entities.Count; eIdx++)
                {
                    EntityData entity = entities.ElementAt(eIdx);
                    List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                    for (int tiIdx = 0; tiIdx < entity.symbolic_intervals.Count; tiIdx++)
                    {
                        TimeIntervalSymbol tis = new TimeIntervalSymbol(entity.symbolic_intervals.ElementAt(tiIdx).startTime, entity.symbolic_intervals.ElementAt(tiIdx).endTime, entity.symbolic_intervals.ElementAt(tiIdx).symbol);
                        tisList.Add(tis);
                    }
                    entityTISs.Add(entity.entityID, tisList);
                }
                return entityTISs;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return null;
            }
        }
        public void updateVariableDicFromFile(string path)
        {
            char[] delim = new char[2] { '\n', '\r' };
            using (TextReader rdr = new StreamReader(path))
            {
                string[] records = rdr.ReadToEnd().Split(delim, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in records)
                {
                    int varid;
                    string[] rec = s.Split(',');
                    if (!int.TryParse(rec[0], out varid))
                        continue;
                    tariablesDic.Add(varid, new TempVarInfo(varid, rec[1]));
                }
            }
        }
        public void saveKLfile(string filePath)
        {
            try
            {
                string writeLine = "";
                TextWriter tw = new StreamWriter(filePath);
                tw.WriteLine(datasetName);
                //print the variables dictionary?
                for (int tvIdx = 0; tvIdx < tariablesDic.Count; tvIdx++)
                {
                    TempVarInfo tvi = tariablesDic.Values.ElementAt(tvIdx);
                    tw.WriteLine(tvi.varID + "," + tvi.varName + "," + tvi.absMethod + "," + tvi.binList.Count);
                    for (int i = 0; i < tvi.binList.Count; i++)
                    {
                        Bin b = tvi.binList.ElementAt(i);
                        tw.WriteLine(b._highlimit + "," + b._lowlimit + "," + b._ID + "," + b._label);
                    }
                }
                tw.WriteLine("NumberOfEntities " + entities.Count);
                for (int eIdx = 0; eIdx < entities.Count; eIdx++)
                {
                    EntityData entity = entities.ElementAt(eIdx);
                    tw.WriteLine(entity.entityID + ";"); // was with entity index, removed it.
                    writeLine = "";
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

        public void updateVarDic()
        {
            foreach (EntityData ent in entities)
            {
                foreach (EntityTempVariable etv in ent.varDic.Values)
                {
                    if (!tariablesDic.ContainsKey(etv.varID)) // varSizes.ContainsKey(tv.varID))
                    {
                        TempVarInfo tvi = new TempVarInfo(etv.varID, etv.varName);
                        tariablesDic.Add(etv.varID, tvi); // varSizes.Add(tv.varID, tv.values.Count);
                    }
                    // else
                    //  disco.tariablesDic[etv.varID].allSize = disco.tariablesDic[etv.varID].allSize + etv.values.Count;
                }
            }
        }

        public void tonceptulize(string absMethod, int binsNumber)
        {
            int bIndex = 1;
            for (int trIdx = 0; trIdx < tariablesDic.Count; trIdx++)
            {
                TempVarInfo tvi = tariablesDic.Values.ElementAt(trIdx);
                if (classKeys.Contains(tvi.varID))// CHECK!!!!
                    continue;
                tvi.absMethod = absMethod;
                double[] vals = getValuesAsDoubleVectorOfVariable(tvi.varID);
                if (vals.Length == 0)
                    continue;
                tvi.binList = DiscoMania.getBinsPerMethodAndData(absMethod, binsNumber, vals, "");
                for (int bIdx = 0; bIdx < tvi.binList.Count; bIdx++)
                {
                    tvi.binList[bIdx]._ID = bIndex;
                    bIndex++;
                }

                for (int eIdx = 0; eIdx < entities.Count; eIdx++)
                {
                    if (entities.ElementAt(eIdx).varDic.ContainsKey(tvi.varID))
                    {
                        EntityTempVariable etv = entities.ElementAt(eIdx).varDic[tvi.varID];
                        DiscoMania.DiscretizeTSVSs(etv.values, tvi.binList); // cuts);
                        if (!binsOnly)
                        {
                            List<TimeIntervalSymbol> tisList = DiscoMania.intervalize(etv.values);
                            entities.ElementAt(eIdx).varDic[tvi.varID].intervals = tisList;
                            addTIsToEntityFromTraiable(entities.ElementAt(eIdx).symbolic_intervals, etv.intervals);
                        }
                    }
                }
            }
            if (!binsOnly)
            {
                for (int eIdx = 0; eIdx < entities.Count; eIdx++)
                    entities.ElementAt(eIdx).symbolic_intervals.Sort(TimeIntervalSymbol.compareTIS);
            }
        }


        private void addTIsToEntityFromTraiable(List<TimeIntervalSymbol> entityList, List<TimeIntervalSymbol> traibleList)
        {
            for (int i = 0; i < traibleList.Count; i++)
                entityList.Add(traibleList.ElementAt(i));
        }
        public void multValueClassTI(string filePath)
        {
            Dictionary<double, TextWriter> writers = new Dictionary<double, TextWriter>();
            string writeLine = "";
            for (int eIdx = 0; eIdx < entities.Count; eIdx++)
            {
                EntityData entity = entities.ElementAt(eIdx);
                if (!entity.varDic.ContainsKey(classSeparator)) // if that entity doesn't contain class info.
                    continue;
                double classValue = entity.varDic[classSeparator].values[0].value;
                if (!writers.ContainsKey(classValue))
                {
                    string[] pathsplit = filePath.Split('.');
                    TextWriter wr = new StreamWriter(pathsplit[0] + "_Class" + classValue + ".csv");
                    writers.Add(classValue, wr);
                    writeHeader(ref wr);
                }
                TextWriter tw = writers[classValue];
                tw.WriteLine(entity.entityID + ";");
                writeLine = "";
                for (int tiIdx = 0; tiIdx < entity.symbolic_intervals.Count; tiIdx++)
                    writeLine = writeLine + entity.symbolic_intervals.ElementAt(tiIdx).startTime + "," + entity.symbolic_intervals.ElementAt(tiIdx).endTime + "," + entity.symbolic_intervals.ElementAt(tiIdx).symbol + ";";
                tw.WriteLine(writeLine);
            Console.WriteLine("added entity index " + eIdx + " out of " + entities.Count);
            }
            foreach (TextWriter writer in writers.Values)
            {
                writer.Close();
            }
        }



        private void writeHeader(ref TextWriter tw)
        {
            tw.WriteLine(datasetName);
            tw.WriteLine("startToncepts");
            for (int tvIdx = 0; tvIdx < tariablesDic.Count; tvIdx++)
            {
                TempVarInfo tvi = tariablesDic.Values.ElementAt(tvIdx);
                tw.WriteLine(tvi.varID + "," + tvi.varName + "," + tvi.absMethod + "," + tvi.binList.Count);
                for (int i = 0; i < tvi.binList.Count; i++)
                {
                    Bin b = tvi.binList.ElementAt(i);
                    tw.WriteLine(b._lowlimit + "," + b._highlimit + "," + b._ID + "," + b._label);
                }
            }
            tw.WriteLine("numberOfEntities," + entities.Count);
        }


    public void saveTonceptSetTIsToFile(string filePath)
    {
        if (classSeparator != -1)
        {
                multValueClassTI(filePath);
                return;
        }
        try
        {
            string writeLine = "";
            TextWriter tw = new StreamWriter(filePath);
            tw.WriteLine(datasetName);
            tw.WriteLine("startToncepts");
            for (int tvIdx = 0; tvIdx < tariablesDic.Count; tvIdx++)
            {
                TempVarInfo tvi = tariablesDic.Values.ElementAt(tvIdx);
                tw.WriteLine(tvi.varID + "," + tvi.varName + "," + tvi.absMethod + "," + tvi.binList.Count);
                for (int i = 0; i < tvi.binList.Count; i++)
                {
                    Bin b = tvi.binList.ElementAt(i);
                    tw.WriteLine(b._lowlimit + "," + b._highlimit + "," + b._ID + "," + b._label);
                }
            }
            tw.WriteLine("numberOfEntities," + entities.Count);
            for (int eIdx = 0; eIdx < entities.Count; eIdx++)
            {
                EntityData entity = entities.ElementAt(eIdx);
                tw.WriteLine(entity.entityID + ";");
                writeLine = "";
                if (binsOnly)
                {
                    for (int tsIdx = 0; tsIdx < entity.varDic.Count; tsIdx++)
                    {
                        EntityTempVariable tvar = entity.varDic.ElementAt(tsIdx).Value;
                        foreach (var tsvr in tvar.values)
                        {
                            writeLine = writeLine +tvar.varID+","+ tsvr.timestamp + "," + tsvr.symbol + ";";
                        }
                    }
                }
                else
                {
                    for (int tiIdx = 0; tiIdx < entity.symbolic_intervals.Count; tiIdx++)
                        writeLine = writeLine + entity.symbolic_intervals.ElementAt(tiIdx).startTime + "," + entity.symbolic_intervals.ElementAt(tiIdx).endTime + "," + entity.symbolic_intervals.ElementAt(tiIdx).symbol + ";";
                }
                tw.WriteLine(writeLine);
                Console.WriteLine("wrote to TI " + eIdx + " out of " + entities.Count);
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
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                readLine = tr.ReadLine();
                EntityData entity = new EntityData();
                string[] mainDelimited = readLine.Split(';');
                entity.entityID = int.Parse(mainDelimited[0].Split(',')[0]);
                entity.entityINDEX = int.Parse(mainDelimited[0].Split(',')[1]);
                for (int i = 1; i < mainDelimited.Length - 1; i++)
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
            string variablesLine = "vID,", variableSumsLine = "vName,";
            for (int vIdx = 0; vIdx < tariablesDic.Count; vIdx++)
            {
                //tw.WriteLine(tariablesDic.Values.ElementAt(vIdx).varID + "," + tariablesDic.Values.ElementAt(vIdx).allSize); // varSizes.Keys.ElementAt(vIdx) + "," + varSizes.Values.ElementAt(vIdx) );
                variablesLine = variablesLine + tariablesDic.Values.ElementAt(vIdx).varID + ",";
                variableSumsLine = variableSumsLine + tariablesDic.Values.ElementAt(vIdx).varName + ",";
            }
            tw.WriteLine(variablesLine);
            tw.WriteLine(variableSumsLine);
            tw.WriteLine("entitiesVariablesData");
            for (int eIdx = 0; eIdx < entities.Count; eIdx++)
            {
                EntityData entity = entities.ElementAt(eIdx);
                tw.WriteLine(entity.entityID); //removed redundant entity index
                for (int tvIdx = 0; tvIdx < entity.varDic.Count; tvIdx++)
                {
                    tw.WriteLine(entity.varDic.ElementAt(tvIdx).Key);
                    EntityTempVariable tv = entity.varDic.ElementAt(tvIdx).Value;
                    writeLine = "";
                    for (int tsIdx = 0; tsIdx < tv.values.Count; tsIdx++)
                        writeLine = writeLine + tv.values[tsIdx].timestamp + "," + tv.values[tsIdx].value + ";"; //removed redundant symbol
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
            string[] varsKeysVec = tr.ReadLine().Split(',');
            string[] varsValsVec = tr.ReadLine().Split(',');
            for (int v = 1; v < varsKeysVec.Length - 1; v++)
            {
                TempVarInfo tvi = new TempVarInfo(int.Parse(varsKeysVec[v]), varsValsVec[v]);
                tariablesDic.Add(tvi.varID, tvi);
                if (varsValsVec[v].ToLower() == "class")
                    classKeys.Add(int.Parse(varsKeysVec[v]));
            }
            while (tr.Peek() >= 0) //read dictinary varSizes
            {
                readLine = tr.ReadLine();
                if (readLine == "entitiesVariablesData")
                    break;
                //string[] delimVec = readLine.Split(',');
                //TempVarInfo tvi = new TempVarInfo(int.Parse(delimVec[0]), int.Parse(delimVec[1]));
                //tariablesDic.Add(tvi.varID, tvi);
            }

            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                readLine = tr.ReadLine();
                EntityData entity = new EntityData();
                string[] delimVec = readLine.Split(',');
                entity.entityID = int.Parse(delimVec[0]);
                //entity.entityINDEX = int.Parse(delimVec[1]);
                entity.entityINDEX = 0; // in the meantime
                while (tr.Peek() >= 0)
                {
                    readLine = tr.ReadLine();
                    if (readLine.EndsWith("enDity"))
                        break;
                    EntityTempVariable tv = new EntityTempVariable();
                    tv.varID = int.Parse(readLine);
                    readLine = tr.ReadLine();
                    string[] delimitedValsVec = readLine.Split(';');
                    for (int i = 0; i < delimitedValsVec.Length - 1; i++)
                    {
                        TimeStampValueSymbol tsvs = new TimeStampValueSymbol(int.Parse(delimitedValsVec[i].Split(',')[0]), double.Parse(delimitedValsVec[i].Split(',')[1]));
                        tsvs.symbol = 0; // in the meantime
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

    public double[] getValuesAsDoubleVectorOfVariable(int varId) // can be easily paralleled
    {
        //double[] dVec = new double[tariablesDic[varId].allSize];
        List<double> dVec = new List<double>();
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].varDic.ContainsKey(varId))
                for (int j = 0; j < entities[i].varDic[varId].values.Count; j++)
                    dVec.Add(entities[i].varDic[varId].values[j].value);
        }
        return dVec.ToArray();
    }

    public TimeStampValueSymbol[] getTimeStampValuesVectorOfVariable(int varId)
    {
        //TimeStampValueSymbol[] tsVec = new TimeStampValueSymbol[tariablesDic[varId].allSize];
        List<TimeStampValueSymbol> tsVec = new List<TimeStampValueSymbol>();
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].varDic.ContainsKey(varId))
                for (int j = 0; j < entities[i].varDic[varId].values.Count; j++)
                    tsVec.Add(entities[i].varDic[varId].values[j]);
        }
        return tsVec.ToArray();
    }
}

public class TempVarInfo
{
    public int varID;
    public string varName;
    //public int allSize; //verSupport;
    public string absMethod;
    public List<Bin> binList = new List<Bin>();
    public HashSet<double> seenValues = new HashSet<double>(); //for separating by value

    public TempVarInfo(int setVarID, string setName)
    {
        varID = setVarID;
        //allSize = setAllSize;
        varName = setName;
    }
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
    public string varName;
    public List<TimeStampValueSymbol> values = new List<TimeStampValueSymbol>();
    public List<TimeIntervalSymbol> intervals = new List<TimeIntervalSymbol>();

    public EntityTempVariable(int id, string name)
    {
        varID = id;
        varName = name;
        values = new List<TimeStampValueSymbol>();
        intervals = new List<TimeIntervalSymbol>();
    }

    public EntityTempVariable(int id)
    {
        varID = id;
        values = new List<TimeStampValueSymbol>();
        intervals = new List<TimeIntervalSymbol>();
    }

    public EntityTempVariable()
    {
        varID = 0;
        values = new List<TimeStampValueSymbol>();
        intervals = new List<TimeIntervalSymbol>();
    }

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
            TimeIntervalSymbol tis = new TimeIntervalSymbol(values[i].timestamp, values[j].timestamp, values[i].symbol);
            intervals.Add(tis);
            i = j;
        }
    }
}

public class EntityData
{
    public int entityID;
    public int entityINDEX;
    private Mutex m = new Mutex();
    public Dictionary<int, EntityTempVariable> varDic = new Dictionary<int, EntityTempVariable>();
    public List<TimeIntervalSymbol> symbolic_intervals = new List<TimeIntervalSymbol>();
    public void ParallelAddTIS(TimeIntervalSymbol tis)
    {
        m.WaitOne();
        symbolic_intervals.Add(tis);
        m.ReleaseMutex();
    }
    public class EntityComparer : IComparer<EntityData>
    {
        public int Compare(EntityData x, EntityData y)
        {
            if (x.entityID > y.entityID)
                return 1;
            return -1;
        }
    }
}

public class TimeIntervalSymbol
{
    public int startTime;
    public int endTime;
    public int symbol;

    public TimeIntervalSymbol(int setStartTime, int setEndTime, int setSymbol)
    {
        startTime = setStartTime;
        endTime = setEndTime;
        symbol = setSymbol;
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