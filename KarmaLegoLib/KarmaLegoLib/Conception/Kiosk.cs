using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;


namespace KarmaLegoLib
{
    class DiscoSet
    {
        public string datasetName;
        public Dictionary<int, int> varSizes = new Dictionary<int, int>();
        public List<EntityData> entities = new List<EntityData>();

        public double[] getValuesAsDoubleVectorOfVariable(int varId)
        {
            double[] dVec = new double[varSizes[varId]];
            int idx = 0;
            for (int i = 0; i < entities.Count; i++)
                for(int j=0; j<entities[i].varDic[varId].values.Count; j++)
                    dVec[idx++] = entities[i].varDic[varId].values[j].value;
            return dVec;

        }

    }

    class TempVariable
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
        public int EntID;
        public Dictionary<int, TempVariable> varDic = new Dictionary<int, TempVariable>();//varID, tempVar
        public List<TimeIntervalSymbol> symbolic_intervals = new List<TimeIntervalSymbol>();
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

    /*
    class TimeStampSymbol
    {
        public int timestamp;
        public int symbol;
        // parameter name?

        public TimeStampSymbol(int setTimeStamp, int setSymbol)
        {
            timestamp = setTimeStamp;
            symbol = setSymbol;
        }
    }*/


    class Kiosk
    {
        /* Test for the SORT - the solution should be: <5,8,2><5,8,3><11,13,1><11,15,3>
            TimeIntervalSymbol tis = new TimeIntervalSymbol(11, 15, 3); ent.symbolic_intervals.Add(tis);
            tis = new TimeIntervalSymbol(11, 13, 1); ent.symbolic_intervals.Add(tis);
            tis = new TimeIntervalSymbol(5, 8, 3); ent.symbolic_intervals.Add(tis);
            tis = new TimeIntervalSymbol(5, 8, 2); ent.symbolic_intervals.Add(tis);
        */ 
                
        public static DiscoSet LoadTimesIntervalsTableToDiscoSet(string tableName, int entitieSize, DataBase dbase)
        {
            DiscoSet disco = new DiscoSet();
            List<int> idsList = dbase.getListOfDistinctByColumn(tableName, Const.STR_EXPORT_ENTITY_ID);

            for (int i = 0; i < entitieSize && i < idsList.Count; i++)
            {
                EntityData ent = new EntityData();
                ent.EntID = idsList[i];
                SqlDataReader reader = dbase.queryAndReturnSQLReader("SELECT * FROM " + tableName + " WHERE " + Const.STR_EXPORT_ENTITY_ID + " = '" + ent.EntID + "' ORDER BY " + Const.STR_EXPORT_STATE_ID + "," + Const.STR_EXPORT_START_TIME + "," + Const.STR_EXPORT_END_TIME);
                while (reader.Read())
                {
                    TimeIntervalSymbol tis = new TimeIntervalSymbol((int)reader[Const.STR_EXPORT_START_TIME], (int)reader[Const.STR_EXPORT_END_TIME], (int)reader[Const.STR_EXPORT_STATE_ID]);
                    ent.symbolic_intervals.Add(tis);
                }
                reader.Close();
                ent.symbolic_intervals.Sort(KarmaLegoLib.TimeIntervalSymbol.compareTIS);
                disco.entities.Add(ent);
            }
            return disco;
        }

        public static DiscoSet LoadTimestampsValuesTableToDiscoSet(string tableName, int entitieSize, DataBase dbase)
        {
            DiscoSet disco = new DiscoSet();
            List<int> idsList = dbase.getListOfDistinctByColumn(tableName, Const.STR_ENTITY_ID);
            List<int> variablesList = dbase.getListOfDistinctByColumn(tableName, Const.STR_TEMPORAL_PROPERTY_ID);
            for (int i = 0; i < entitieSize && i < idsList.Count; i++)
            {
                EntityData ent = new EntityData();
                ent.EntID = idsList[i];
                SqlDataReader reader = dbase.queryAndReturnSQLReader("SELECT * FROM " + tableName + " WHERE " + Const.STR_ENTITY_ID + " = '" + idsList[i] + "' ORDER BY " + Const.STR_TEMPORAL_PROPERTY_ID + "," + Const.STR_TIME_STAMP);
                TempVariable tv = new TempVariable();
                tv.varID = -1;
                while (reader.Read())
                {
                    TimeStampValueSymbol tsvs = new TimeStampValueSymbol((int)reader[Const.STR_TIME_STAMP], (double)reader[Const.STR_TEMPORAL_PROPERTY_VALUE]);
                    if ((int)reader[Const.STR_TEMPORAL_PROPERTY_ID] != tv.varID)
                    {
                        if (tv.varID > -1)
                        {
                            ent.varDic.Add(tv.varID, tv); // variables.Add(tv);
                            if (!disco.varSizes.ContainsKey(tv.varID))
                                disco.varSizes.Add(tv.varID, tv.values.Count);
                            else
                                disco.varSizes[tv.varID] = disco.varSizes[tv.varID] + tv.values.Count;
                        }
                        tv = new TempVariable();
                        tv.varID = (int)reader[Const.STR_TEMPORAL_PROPERTY_ID];
                    }
                    tv.values.Add(tsvs);
                }
                reader.Close();
                disco.entities.Add(ent);
            }
            return disco;
        }
    }
}
