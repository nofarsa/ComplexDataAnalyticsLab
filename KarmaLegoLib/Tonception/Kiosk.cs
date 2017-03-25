using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;

namespace Tonception
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


    public class Kiosk
    {
        /* Test for the SORT - the solution should be: <5,8,2><5,8,3><11,13,1><11,15,3>
            TimeIntervalSymbol tis = new TimeIntervalSymbol(11, 15, 3); ent.symbolic_intervals.Add(tis);
            tis = new TimeIntervalSymbol(11, 13, 1); ent.symbolic_intervals.Add(tis);
            tis = new TimeIntervalSymbol(5, 8, 3); ent.symbolic_intervals.Add(tis);
            tis = new TimeIntervalSymbol(5, 8, 2); ent.symbolic_intervals.Add(tis);
        */

        public static void /*TonceptSet*/ LoadBinsDefinitionsTableToDiscoSet(ref TonceptSet disco, string tableName, DataBase dbase)
        {
            TempVarInfo tvi = new TempVarInfo(0,"");
            if(disco == null)
                disco = new TonceptSet();
            string sqlStr = "SELECT * FROM " + tableName;
            SqlDataReader reader = dbase.queryAndReturnSQLReader(sqlStr);
            int lastTemporalPropertyID = 0;

            while (reader.Read())
            {
                //TimeIntervalSymbol tis = new TimeIntervalSymbol((int)reader[Const.STR_EXPORT_START_TIME], (int)reader[Const.STR_EXPORT_END_TIME], (int)reader[Const.STR_EXPORT_STATE_ID]);
                int tempPropID = (int)reader["TemporalPropertyID"];
                if (tempPropID != lastTemporalPropertyID)
                {
                    if (lastTemporalPropertyID > 0)
                        disco.tariablesDic.Add(tvi.varID, tvi);
                    tvi = new TempVarInfo(tempPropID, "");
                    tvi.absMethod = (string)reader["MethodName"];
                    lastTemporalPropertyID = tempPropID;
                }
                Bin b = new Bin((int)reader["StateID"], (double)reader["BinFrom"]);
                b._highlimit = (double)reader["BinTo"];
                b._label = (string)reader["BinLabel"];
                tvi.binList.Add(b);
            }
            reader.Close();
            
        }

        public static void LoadTimeIntervalsTableToDiscoSet(ref TonceptSet disco, string tableName, int entitieSize, DataBase dbase)
        {
            if(disco == null)
                disco = new TonceptSet();
            List<int> idsList = dbase.getListOfDistinctByColumn(tableName, Const.STR_EXPORT_ENTITY_ID);
                
            for (int i = 0; i < entitieSize && i < idsList.Count; i++)
            {
                EntityData ent = new EntityData();
                ent.entityID = idsList[i];
                ent.entityINDEX = i;
                SqlDataReader reader = dbase.queryAndReturnSQLReader("SELECT * FROM " + tableName + " WHERE " + Const.STR_EXPORT_ENTITY_ID + " = '" + ent.entityID + "' ORDER BY " + Const.STR_EXPORT_STATE_ID + "," + Const.STR_EXPORT_START_TIME + "," + Const.STR_EXPORT_END_TIME);
                while (reader.Read())
                {
                    TimeIntervalSymbol tis = new TimeIntervalSymbol((int)reader[Const.STR_EXPORT_START_TIME], (int)reader[Const.STR_EXPORT_END_TIME], (int)reader[Const.STR_EXPORT_STATE_ID]);
                    ent.symbolic_intervals.Add(tis);
                }
                reader.Close();
                ent.symbolic_intervals.Sort(TimeIntervalSymbol.compareTIS);
                disco.entities.Add(ent);
            }
        }

        public static TonceptSet LoadTimestampsValuesTableToDiscoSet(string tableName, int entitieSize, DataBase dbase)
        {
            TonceptSet disco = new TonceptSet();
            List<int> idsList = dbase.getListOfDistinctByColumn(tableName, Const.STR_ENTITY_ID);
            List<int> variablesList = dbase.getListOfDistinctByColumn(tableName, Const.STR_TEMPORAL_PROPERTY_ID);
            for (int i = 0; i < entitieSize && i < idsList.Count; i++)
            {
                EntityData ent = new EntityData();
                ent.entityID = idsList[i];
                string sqlStr = "SELECT * FROM " + tableName + " WHERE " + Const.STR_ENTITY_ID + " = '" + idsList[i] + "' ORDER BY " + Const.STR_TEMPORAL_PROPERTY_ID + "," + Const.STR_TIME_STAMP;
                SqlDataReader reader = dbase.queryAndReturnSQLReader(sqlStr);
                EntityTempVariable etv = new EntityTempVariable();
                etv.varID = -1;
                while (reader.Read())
                {
                    int timestamp = (int)reader[Const.STR_TIME_STAMP];
                    double value = (double)reader[Const.STR_TEMPORAL_PROPERTY_VALUE];
                    TimeStampValueSymbol tsvs = new TimeStampValueSymbol(timestamp, value);
                    int varID = (int)reader[Const.STR_TEMPORAL_PROPERTY_ID];
                    if (varID == 55)
                        varID = varID;
                    if ( varID != etv.varID ) // if a new temporal variable
                    {
                        if (etv.varID > -1) // add the previous temporal variable to the entity
                        {
                            ent.varDic.Add(etv.varID, etv); // add the temporal variable the entity varDic
                            // accumulate the values to the general temporal variable
                            if (!disco.tariablesDic.ContainsKey(etv.varID)) // varSizes.ContainsKey(tv.varID))
                            {
                                TempVarInfo tvi = new TempVarInfo(etv.varID,"");
                                disco.tariablesDic.Add(etv.varID, tvi); // varSizes.Add(tv.varID, tv.values.Count);
                            }
                           // else
                               // disco.tariablesDic[etv.varID].allSize = disco.tariablesDic[etv.varID].allSize + etv.values.Count; // disco.varSizes[tv.varID] = disco.varSizes[tv.varID] + tv.values.Count;
                        }
                        etv = new EntityTempVariable();
                        etv.varID = varID; // (int)reader[Const.STR_TEMPORAL_PROPERTY_ID];
                    }
                    etv.values.Add(tsvs); // add value to temporal variable
                }
                if (!disco.tariablesDic.ContainsKey(etv.varID)) // varSizes.ContainsKey(tv.varID))
                {
                    TempVarInfo tvi = new TempVarInfo(etv.varID,"");
                    disco.tariablesDic.Add(etv.varID, tvi); // varSizes.Add(tv.varID, tv.values.Count);
                }
                //else
                    //disco.tariablesDic[etv.varID].allSize = disco.tariablesDic[etv.varID].allSize + etv.values.Count; // disco.varSizes[tv.varID] = disco.varSizes[tv.varID] + tv.values.Count;
                if (!ent.varDic.ContainsKey(etv.varID))
                    ent.varDic.Add(etv.varID, etv);
                reader.Close();
                disco.entities.Add(ent); // add entity to the entities
            }
            return disco;
        }
    }
}
