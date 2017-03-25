using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;

using System.Data;
//using System.IO;
//using System.Diagnostics;


namespace DiscoStation
{
    class Error
    {
        /// <summary>
        /// soft error - display error, update log file and continue with process
        /// </summary>
        /// <param name="code">error code</param>
        public static void softError(int code)
        {
            string[] vals = Const.ARR_ERROR_MSG[code].Split('-');
            System.Console.WriteLine("ERROR : " + vals[0] + " : " + vals[1]);
            //log.WriteLine("ERROR : " + vals[0] + " : " + vals[1]);

        } // softError

        /// <summary>
        /// hard error - display error, update log file and exit process
        /// </summary>
        /// <param name="code">error code</param>
        public static void hardError(int code)
        {
            string[] vals = Const.ARR_ERROR_MSG[code].Split('-');
            System.Console.WriteLine("ERROR : " + vals[0] + " : " + vals[1]);
            //log.WriteLine("ERROR : " + vals[0] + " : " + vals[1]);
            System.Console.WriteLine("Aborting...");
            //log.WriteLine("Aborting...");
            //log.WriteHR();
            Environment.Exit(0);
        } // hardError

    } // class Error

    class dbMethods
    {
        private static SqlConnection Connection = new SqlConnection();

        public static string ServerName;
        public static string UserName;
        public static string Password;

        /// <summary>
        /// basic constructor, connect to server parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void UpdateDBServer(string name, string user, string password)
        {
            ServerName = name;
            UserName = user;
            Password = password;
        } // UpdateDBServer


        public static void Connect()
        {
            Connection = new SqlConnection("server = " + ServerName +
                                 "; user id='" + UserName + "'; password='" + Password + "'; Integrated Security=no;");
            Connection.Open();
            Connection.Close();
        }

        public static bool CreateDatabase(string dbName)
        {
            bool ans = false;
            SqlConnection tmpConn = new SqlConnection();
            tmpConn.ConnectionString = "Data Source=(local); DATABASE = master;Integrated Security=True; user instance=false";
            string sqlCreateDBQuery = " CREATE DATABASE " + dbName + " ON PRIMARY " +
                        " (NAME = '" + dbName + "', "
                        + @" FILENAME = N'C:\Guy\Databases\AbVTemporal\" + dbName + ".mdf', "
                        + " SIZE = 10240KB,"
                        + " FILEGROWTH = 1024KB) "
                        + " LOG ON (NAME =" + dbName + "LOG, "
                        + @" FILENAME = N'C:\Guy\Databases\AbVTemporal\" +
                        dbName + "_log.ldf', "
                        + " SIZE = 5MB, "
                        + " FILEGROWTH =10%) ";

            SqlCommand myCommand = new SqlCommand(sqlCreateDBQuery, tmpConn);

            try
            {
                tmpConn.Open();
                myCommand.ExecuteNonQuery();
                ans = true;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error creating database : " + ex.Message);
            }
            finally
            {
                tmpConn.Close();
            }
            return ans;

        } //CreateDatabase


        public static bool databaseExists(string dbName)
        {
            if (Connection.State == ConnectionState.Open) Connection.Close();
            Connection = new SqlConnection("server = " + ServerName +
                                 "; user id ='" + UserName + "'; password='" + Password + "'; Integrated Security=no;");
            Connection.Open();
            SqlCommand command = new SqlCommand("SELECT name FROM sys.databases WHERE name='" + dbName + "'", Connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                Connection.Close();
                return true;
            }
            else
            {
                Connection.Close();
                return false;
            }
        } // databaseExists

    }
}
