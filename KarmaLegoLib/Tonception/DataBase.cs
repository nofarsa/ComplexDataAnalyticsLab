using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;

namespace Tonception
{
    /// <summary>
    /// Holds data from SQL database using defined classes
    /// This is the primary Database object, all database actions and events are managed 
    /// in this class.
    /// </summary>
    public class DataBase
    {
        /// <summary>
        /// The Connection stores the Discretization Database connection details.
        /// </summary>
        public SqlConnection Connection;


        /// <summary>
        /// Class contructor which connects to a certain databse using given parameters
        /// </summary>
        /// <param name="server">Server name</param>
        /// <param name="user">User name</param>
        /// <param name="password">User's Password</param>
        /// <param name="database">Database Name</param>
        /// <param name="trustedconn">Trusted Connection : yes or no</param>
        /// <param name="conntimeout">Connection timeout</param>
        public DataBase(string server, string user, string password, string database, bool trustedconn, int conntimeout)
        {
            string trusted = trustedconn ? "yes" : "no";
            Connection = new SqlConnection("user id=" + user + ";password=" + password + ";server=" + server +
                        ";Trusted_Connection=" + trusted + ";database=" + database +
                        ";connection timeout=" + conntimeout.ToString());
            Connection.Open();
        } // basic constructor

        /// <summary>
        /// checks server for given database existence and connectivity
        /// </summary>
        /// <param name="server">server name</param>
        /// <param name="user">user name</param>
        /// <param name="pass">password</param>
        /// <param name="db">database name</param>
        public static void checkServerDatabase(string server, string user, string pass, string db)
        {
            dbMethods.UpdateDBServer(server, user, pass);
            try
            {
                dbMethods.Connect();
            }
            catch (Exception ex)
            {
                Error.hardError(Const.INT_ERR_SERVER);
            }

            if (!dbMethods.databaseExists(db))
                Error.hardError(Const.INT_ERR_DATABASE);

        } // checkServerDatabase

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            try
            {
                Connection.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error : unable to close SQL connection");
            }
        } // Close


        public bool isTableExists(string tableName)
        {
            bool ans = false;
            string commandline = "SELECT * FROM sys.objects WHERE name='" + tableName + "' AND type_desc='USER_TABLE'";
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(commandline, Connection);
            reader = command.ExecuteReader();
            ans = reader.HasRows;
            reader.Close();
            return ans;
        }

        public void CreateTable(string tablename, string[] colnames, string[] coltypes, string[] coldefs)
        {
            int i;
            string strCommand = "CREATE TABLE [dbo].[" + tablename + "](";
            for (i = 0; i < colnames.Length - 1; i++)
                strCommand += " [" + colnames[i] + "] [" + coltypes[i] + "] " + coldefs[i] + ",";
            strCommand += " [" + colnames[i] + "] [" + coltypes[i] + "] " + coldefs[i] + ") ON [PRIMARY]";
            SqlCommand command = new SqlCommand(strCommand, Connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        } // CreateTable

        /// <summary>
        /// Execute SQL Command string
        /// </summary>
        /// <param name="command">command string</param>
        public void ExecuteSQL(string command)
        {
            SqlCommand command1 = new SqlCommand(command, Connection);
            command1.CommandTimeout = 0;
            command1.ExecuteNonQuery();
        } // ExecuteSQL


        /// <summary>
        /// Returns the index, by a column name, of a record which agree swith given conditions
        /// </summary>
        /// <param name="table">Table's name</param>
        /// <param name="indexcolumn">Name of Index column</param>
        /// <param name="columns">Conditions columns names array of strings</param>
        /// <param name="values">Conditions columns values array of strings</param>
        /// <returns>index of requested record (row), -1 if doesn't exist</returns>
        public long getIndexOf(string table, string indexcolumn, string[] columns, string[] values)
        {
            long ans = -1;
            int i;
            SqlDataReader reader;
            string commandline = "SELECT " + indexcolumn + " FROM " + table + " WHERE ";
            for (i = 0; i < columns.Length - 1; i++)
                commandline += columns[i] + "='" + values[i] + "' AND ";
            commandline += columns[i] + "='" + values[i] + "'";

            SqlCommand command = new SqlCommand(commandline, Connection);

            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                ans = (int)reader[indexcolumn];
            }
            reader.Close();
            return (long)ans;

        } // getIndexOf


        /// <summary>
        /// returns table's number of records (rows) unconiditionally
        /// </summary>
        /// <param name="table">table's name</param>
        /// <returns>table's number of records</returns>
        public long getTableSize(string table)
        {
            long ans;
            SqlCommand command = new SqlCommand("SELECT COUNT(*) AS Total FROM " + table, Connection);
            ans = (int)command.ExecuteScalar();
            return ans;
        } // getTableSize


        /// <summary>
        /// returns the next available index (size of table) for the given table conditionally (by selected records only)
        /// </summary>
        /// <param name="table">given table</param>
        /// <returns>number of rows in table + 1 </returns>
        public long getNextAvailableIndex(string table, string[] condcolumns, string[] condvalues)
        {
            long ans;
            string str = "SELECT COUNT(*) AS Total FROM " + table;
            if (condcolumns.Length > 0) str += " WHERE " + condcolumns[0] + "=" + condvalues[0];
            for (int i = 1; i < condcolumns.Length; i++)
                str += "AND " + condcolumns[i] + "=" + condvalues[i];
            //command.Connection.Open();
            SqlCommand command = new SqlCommand(str, Connection);
            ans = (int)command.ExecuteScalar();
            //command.Connection.Close();
            return (long)ans + 1;
        } // getNextAvailableIndex

        /// <summary>
        /// returns the next available index (size of table) for the current table unconditionally (all records)
        /// </summary>
        /// <param name="table">given table</param>
        /// <returns>number of rows in table + 1 </returns>
        public long getNextAvailableIndex(string table)
        {
            long ans;
            SqlCommand command = new SqlCommand("SELECT COUNT(*) AS Total FROM " + table, Connection);
            //command.Connection.Open();
            ans = (int)command.ExecuteScalar();
            //command.Connection.Close();
            return (long)ans + 1;
        } // getNextAvailableIndex


  
        //#############################################################################################################

        /// <summary>
        /// adds a new temporal property to the TemporalProperties table
        /// </summary>
        /// <param name="ID">new property's ID</param>
        /// <param name="name">new property's name</param>
        /// <returns>new Property's ID</returns>
        public bool addTemporalProperty(long ID, string name)
        {
            string[] columns = { Const.STR_TEMPORAL_PROPERTY_ID, Const.STR_TEMPORAL_PROPERTY_NAME };
            string[] values = new string[2];
            values[0] = ID.ToString();
            values[1] = name;
            return addConditionally(Const.STR_TEMPORAL_PROPERTIES_VALUES_TABLE, columns, values);
        } // addTemporalProperty

 
        
        public void loadDataTableToTable(string table, DataSet data)
        {

            //ExecuteSQL("DELETE " + table);
            // Create an instance of a DataAdapter.
            SqlDataAdapter dataAdapter
                = new SqlDataAdapter("SELECT * FROM " + table, Connection);
            SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Update(data, table);

        } // loadDataSetToTable


        /// <summary>
        /// loads a whole table into a data set
        /// </summary>
        /// <param name="entity">Given entity</param>
        /// <param name="ID">Temporal property ID</param>
        /// <param name="time">time stamp</param>
        /// <param name="value">new value to write</param>
        public DataSet loadTableToDataSet(string table, string[] ordercol, string[] condcolumns, string[] condvalues)
        {
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();
            string str;
            if (condcolumns.Length > 0)
            {
                str = "SELECT * FROM " + table + " WHERE " + condcolumns[0] + "=" + condvalues[0];
                for (int i = 1; i < condcolumns.Length; i++)
                    str += " AND " + condcolumns[i] + "=" + condvalues[i];

            }
            else
                str = "SELECT * FROM " + table;

            if (ordercol.Length > 0)
            {
                int i;
                str += " ORDER BY ";
                for (i = 0; i < ordercol.Length - 1; i++)
                    str += ordercol[i] + ",";
                str += ordercol[i];

            } // if order

            adapter = new SqlDataAdapter(str, Connection);
            //builder = new SqlCommandBuilder(adapter);
            adapter.Fill(ds, table);
            return ds;

        } // updateTemporalPropertyValue



        /// <summary>
        /// Adds a time interval by IntervalID(comboination  of methodID, StateID, PropertyID)
        /// </summary>
        /// <param name="entity">given entity</param>
        /// <param name="IntervalID">Interval ID</param>
        /// <param name="start">start time</param>
        /// <param name="end">end time</param>
        public bool addTemporalPropertyTimeInterval(long entityID, int methodID, long StateID, long start, long end)
        {
            string[] columns = { Const.STR_ENTITY_ID, Const.STR_METHOD_ID, Const.STR_STATE_ID, Const.STR_START_TIME, Const.STR_END_TIME };
            string[] values = new string[5];
            values[0] = entityID.ToString();
            values[1] = methodID.ToString();
            values[2] = StateID.ToString();
            values[3] = start.ToString();
            values[4] = end.ToString();
            return addConditionally("TemporalPropertiesDiscretizedTimeIntervals", columns, values);
        } // addTemporalProperty

        /// <summary>
        /// loads a whole columns as an array, used for discretization
        /// </summary>
        /// <param name="table">table's name</param>
        /// <param name="column">columns name</param>
        /// <param name="column">conditions columns names </param>
        /// <param name="values">conditions columns values</param>
        /// <returns>an array containing the columns values</returns>
        public double[] loadDoubleColumnToArray(string table, string column, string[] columns, string[] values)
        {
            List<double> res = new List<double>();
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            string str;
            int i;
            if (columns.Length > 1) str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];
            else str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];

            for (i = 1; i < columns.Length; i++)
                str += "' AND " + columns[i] + "='" + values[i];
            str += "'";
            command = new SqlCommand(str, Connection);
            reader = command.ExecuteReader();
            while (reader.Read())
                res.Add(((double)reader[column]));
            reader.Close();
            return res.ToArray();

        } // loadColumnToArray

        /*
        public List<TimeStampValue> returnTimeStampValueList()
        {
            List<TimeStampValue> tsvList = new List<TimeStampValue>();
            return tsvList;
        }*/

        /// <summary>
        /// loads a whole columns as an array, used for discretization
        /// </summary>
        /// <param name="table">table's name</param>
        /// <param name="column">columns name</param>
        /// <param name="column">conditions columns names </param>
        /// <param name="values">conditions columns values</param>
        /// <returns>an array containing the columns values</returns>
        public List<TimeStampValueSymbol> loadTableTimestampsValues(string table, /*string column,*/ string[] columns, string[] values)
        {
            List<TimeStampValueSymbol> timeStampValueList = new List<TimeStampValueSymbol>();
            //SqlDataReader reader;
            //SqlCommand command = new SqlCommand();
            string str;
            int i;
            //if (columns.Length > 1) str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];
            //                   else str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];
            str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];
            for (i = 1; i < columns.Length; i++)
                str += "' AND " + columns[i] + "='" + values[i];
            str += "'";
            //command = new SqlCommand(str, Connection);
            //reader = command.ExecuteReader();
            SqlDataReader reader = queryAndReturnSQLReader(str);
            while (reader.Read())
            {
                TimeStampValueSymbol tivs = new TimeStampValueSymbol((int)reader["TimeStamp"], (double)reader["TemporalPropertyValue"]);
                timeStampValueList.Add(tivs);
            }
            reader.Close();
            return timeStampValueList;

        } // loadColumnToArray

        public List<int> getListOfDistinctByColumn(string table, string columName)//, string[] columns, string[] values)
        {
            List<int> idsList = new List<int>();
            string sqlStr = "SELECT DISTINCT " + columName + " FROM " + table + " ORDER BY " + columName;
            SqlDataReader reader = queryAndReturnSQLReader(sqlStr);
            while (reader.Read())
            {
                idsList.Add((int)reader[columName]);
            }
            reader.Close();
            return idsList;

        }


        public SqlDataReader queryAndReturnSQLReader(string sql)
        {
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            command = new SqlCommand(sql, Connection);
            reader = command.ExecuteReader();
            return reader;
        }

        

        /// <summary>
        /// loads a whole columns as an array, used for discretization
        /// </summary>
        /// <param name="table">table's name</param>
        /// <param name="column">column's name</param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<double> loadDoubleColumnToList(string table, string column, string[] columns, string[] values)
        {
            List<double> res = new List<double>();
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            string str;
            int i;
            if (columns.Length > 1) str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];
            else str = "SELECT * FROM " + table + " WHERE " + columns[0] + "='" + values[0];

            for (i = 1; i < columns.Length; i++)
                str += "' AND " + columns[i] + "='" + values[i];
            str += "'";
            command = new SqlCommand(str, Connection);
            reader = command.ExecuteReader();
            while (reader.Read())
                res.Add(((double)reader[column]));
            reader.Close();
            return res;

        } // loadColumnToArray


        /// <summary>
        /// loads a whole columns as an array, used for discretization
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int[] loadIntColumnToArray(string table, string selcolumns, string column, string order, string[] columns, string[] values)
        {
            List<int> res = new List<int>();
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            string str;
            int i;
            if (columns.Length > 0)
            {
                str = "SELECT " + selcolumns +  " FROM " + table + " WHERE " + columns[0] + "=" + values[0];
                for (i = 1; i < columns.Length; i++)
                    str += " AND " + columns[i] + "=" + values[i];
            }
            else
                str = "SELECT * FROM " + table;


            str += " ORDER BY " + order;

            command = new SqlCommand(str, Connection);
            reader = command.ExecuteReader();
            while (reader.Read())
                res.Add((int.Parse(reader[column].ToString ())));
            reader.Close();
            return res.ToArray();

        } // loadColumnToArray

        /// <summary>
        /// loads a whole column as an array, used for discretization
        /// </summary>
        /// <param name="table">name of table</param>
        /// <param name="column">name of column to load</param>
        /// <param name="order">order by column</param>
        /// <param name="columns">list of condition columns names</param>
        /// <param name="values">list of condition columns values</param>
        /// <returns>array of string values</returns>
        public string[] loadStringColumnToArray(string table, string column, string order, string[] columns, string[] values)
        {
            List<string> res = new List<string>();
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            string str;
            int i;
            if (columns.Length > 0)
            {
                str = "SELECT * FROM " + table + " WHERE " + columns[0] + "=" + values[0];
                for (i = 1; i < columns.Length; i++)
                    str += " AND " + columns[i] + "=" + values[i];
            }
            else
                str = "SELECT * FROM " + table;


            str += " ORDER BY " + order;

            command = new SqlCommand(str, Connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader[column] != DBNull.Value)
                    res.Add(((string)reader[column]));
                else
                    res.Add("");
            }
            reader.Close();
            return res.ToArray();

        } // loadColumnToArray


        /// <summary>
        /// returns wether a certain record is in the given table or not
        /// </summary>
        /// <param name="table">given table</param>
        /// <param name="columns">columns names</param>
        /// <param name="values">values (corresponds to columns</param>
        /// <returns>true if in table, false if not..</returns>
        public bool inTable(string table, string[] columns, string[] values)
        {
            int i;
            bool ans = false;
            string commandline = "SELECT " + columns[0] + " FROM " + table + " WHERE ";
            for (i = 0; i < columns.Length - 1; i++)
                commandline += columns[i] + "='" + values[i] + "' AND ";
            commandline += columns[i] + "='" + values[i] + "'";

            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(commandline, Connection);
            reader = command.ExecuteReader();
            ans = reader.HasRows;
            reader.Close();
            return ans;
        }  // inTable


        /// <summary>
        /// adds a record if does'nt exist in table
        /// </summary>
        /// <param name="table">givan table</param>
        /// <param name="columns">record's columns</param>
        /// <param name="values">record's values</param>
        /// <returns>true if added, false if not</returns>
        public bool addConditionally(string table, string[] columns, string[] values)
        {
            bool ans = inTable(table, columns, values);
            if (!ans) inputData(table, columns, values);
            return !ans;
        } // addConditionally


        /// <summary>
        /// Inserts row to a given table in current database
        /// </summary>
        /// <param name="table">given table</param>
        /// <param name="culomns">list of culomns names</param>
        /// <param name="values">values to be insert</param>
        public void inputData(string table, string[] culomns, string[] values)
        {
            string str;
            int i;
            SqlCommand command;

            str = "INSERT INTO " + table + " (";
            for (i = 0; i < culomns.Length - 1; i++)
                str += culomns[i] + ",";
            str += culomns[i] + ") Values('";
            for (i = 0; i < values.Length - 1; i++)
                str += values[i] + "','";
            str += values[i] + "')";

            command = new SqlCommand(str, Connection);
            command.ExecuteNonQuery();
        } // inputData

    } // class DataBase
}
