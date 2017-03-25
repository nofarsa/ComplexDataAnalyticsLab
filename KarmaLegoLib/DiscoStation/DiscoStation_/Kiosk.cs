using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;


namespace DiscoStation
{
    

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
                
        public static KarmaLegoLib.TonceptSet LoadTimeIntervalsTableToDiscoSet(string tableName, int entitieSize, DataBase dbase)
        {
            KarmaLegoLib.TonceptSet disco = new KarmaLegoLib.TonceptSet();
            List<int> idsList = dbase.getListOfDistinctByColumn(tableName, Const.STR_EXPORT_ENTITY_ID);
                
            for (int i = 0; i < entitieSize && i < idsList.Count; i++)
            {
                KarmaLegoLib.EntityData ent = new KarmaLegoLib.EntityData();
                ent.entityID = idsList[i];
                ent.entityINDEX = i;
                SqlDataReader reader = dbase.queryAndReturnSQLReader("SELECT * FROM " + tableName + " WHERE " + Const.STR_EXPORT_ENTITY_ID + " = '" + ent.entityID + "' ORDER BY " + Const.STR_EXPORT_STATE_ID + "," + Const.STR_EXPORT_START_TIME + "," + Const.STR_EXPORT_END_TIME);
                while (reader.Read())
                {
                    KarmaLegoLib.TimeIntervalSymbol tis = new KarmaLegoLib.TimeIntervalSymbol((int)reader[Const.STR_EXPORT_START_TIME], (int)reader[Const.STR_EXPORT_END_TIME], (int)reader[Const.STR_EXPORT_STATE_ID]);
                    ent.symbolic_intervals.Add(tis);
                }
                reader.Close();
                ent.symbolic_intervals.Sort(KarmaLegoLib.TimeIntervalSymbol.compareTIS);
                disco.entities.Add(ent);
            }
            return disco;
        }

        public static KarmaLegoLib.TonceptSet LoadTimestampsValuesTableToDiscoSet(string tableName, int entitieSize, DataBase dbase)
        {
            KarmaLegoLib.TonceptSet disco = new KarmaLegoLib.TonceptSet();
            List<int> idsList = dbase.getListOfDistinctByColumn(tableName, Const.STR_ENTITY_ID);
            List<int> variablesList = dbase.getListOfDistinctByColumn(tableName, Const.STR_TEMPORAL_PROPERTY_ID);
            for (int i = 0; i < entitieSize && i < idsList.Count; i++)
            {
                KarmaLegoLib.EntityData ent = new KarmaLegoLib.EntityData();
                ent.entityID = idsList[i];
                string sqlStr = "SELECT * FROM " + tableName + " WHERE " + Const.STR_ENTITY_ID + " = '" + idsList[i] + "' ORDER BY " + Const.STR_TEMPORAL_PROPERTY_ID + "," + Const.STR_TIME_STAMP;
                SqlDataReader reader = dbase.queryAndReturnSQLReader(sqlStr);
                KarmaLegoLib.TempVariable tv = new KarmaLegoLib.TempVariable();
                tv.varID = -1;
                while (reader.Read())
                {
                    int timestamp = (int)reader[Const.STR_TIME_STAMP];
                    double value = (double)reader[Const.STR_TEMPORAL_PROPERTY_VALUE];
                    KarmaLegoLib.TimeStampValueSymbol tsvs = new KarmaLegoLib.TimeStampValueSymbol(timestamp, value);
                    int varID = (int)reader[Const.STR_TEMPORAL_PROPERTY_ID];
                    if ( varID != tv.varID ) // if a new temporal variable
                    {
                        if (tv.varID > -1) // add the previous temporal variable to the entity
                        {
                            ent.varDic.Add(tv.varID, tv); // add the temporal variable the entity varDic
                            // accumulate the values to the general temporal variable
                            if (!disco.varSizes.ContainsKey(tv.varID))
                                disco.varSizes.Add(tv.varID, tv.values.Count);
                            else
                                disco.varSizes[tv.varID] = disco.varSizes[tv.varID] + tv.values.Count;
                        }
                        tv = new KarmaLegoLib.TempVariable();
                        tv.varID = varID; // (int)reader[Const.STR_TEMPORAL_PROPERTY_ID];
                    }
                    tv.values.Add(tsvs); // add value to temporal variable
                }
                reader.Close();
                disco.entities.Add(ent); // add entity to the entities
            }
            return disco;
        }
    }
}
