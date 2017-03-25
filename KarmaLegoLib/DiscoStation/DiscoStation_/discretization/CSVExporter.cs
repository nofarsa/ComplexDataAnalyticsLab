using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;

namespace DiscoStation //Discretizer
{
    /// <summary>
    /// used to export calculated data to CSV files and import raw data to datasets
    /// </summary>
    public static class CSVExporter
    {

        /// <summary>
        /// loads a CSV file to a dataset
        /// </summary>
        /// <param name="fileName">CSV file name</param>
        /// <returns>dataset including the data from the file</returns>
        public static DataSet loadCSVtoDataSet(string fileName)
        {
            string[] values;
            string line;

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());


            using (StreamReader sr = new StreamReader(fileName))
            {
                values = sr.ReadLine().Split(',');
                for (int i = 0; i < values.Length; i++)
                    ds.Tables[0].Columns.Add(values[i]);

                while ((line = sr.ReadLine()) != null)
                {
                    values = line.Split(',');
                    ds.Tables[0].Rows.Add(values);
                }

            } // using

            return ds;

        } // loadCSVtoDataSet

        /// <summary>
        /// saves the given DataTable using CSV format to the given file path.
        /// </summary>
        /// <param name="fileName">file path (no extension)</param>
        /// <param name="table">DataTable object</param>
        public static void saveDataTableToCSV(string fileName, DataTable table)
        {
            int i, j;
            string strLine = "";

            using (StreamWriter sw = new StreamWriter(fileName + ".csv"))
            {
                strLine = table.Columns[0].ColumnName;
                for (i = 1; i < table.Columns.Count; i++)
                    strLine += "," + table.Columns[i].ColumnName;
                sw.WriteLine(strLine);
                strLine = "";
                for (i = 0; i < table.Rows.Count; i++)
                {
                    strLine = table.Rows[i][table.Columns[0].ColumnName].ToString();
                    for (j = 1; j < table.Columns.Count; j++)
                        strLine += "," + table.Rows[i][table.Columns[j].ColumnName].ToString();
                    sw.WriteLine(strLine);
                } // for rows
            } // using
        } // saveDataTableToCSV
    } // class CVSExporter
}
