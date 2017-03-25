using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace DiscoStation //Discretizer
{

    /// <summary>
    /// defines a cut, having a value and a label
    /// </summary>
    public class Cut
    {
        public double cutValue;
        public string cutLabel;

        /// <summary>
        /// basic constructor defines value and label
        /// </summary>
        /// <param name="value">cut's value</param>
        /// <param name="label">cut's label</param>
        public Cut(double value, string label)
        {
            cutValue = value;
            cutLabel = label;
        } // basic constructor

    } // struct Cut


    /// <summary>
    /// defines a list of cuts ( as opposed to list of bins, cuts don't include a range but a value of <=
    /// </summary>
    public class CutsList
    {
        List<Cut> cuts;

        /// <summary>
        /// basic constructor
        /// </summary>
        public CutsList()
        {
            cuts = new List<Cut>();
        } // Basic constructor


        /// <summary>
        /// returns the cut at given position
        /// </summary>
        /// <param name="index">position of cut to be returned</param>
        /// <returns>requested cut, if not found returns a maximal cut</returns>
        public Cut CutAt(int index)
        {
            if (index < cuts.Count && index > -1)
                return cuts[index];
            else
                return new Cut(Bin.MaxValue, "");
        } // CutAt

        /// <summary>
        /// /gets number of cuts in list
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return cuts.Count;
        } // Count

        /// <summary>
        /// add new cut to list
        /// </summary>
        /// <param name="cutValue"></param>
        /// <param name="cutLabel"></param>
        public void AddCut(double cutValue, string cutLabel)
        {
            cuts.Add(new Cut(cutValue, cutLabel));
        } //AddCut

        /// <summary>
        /// remove cut at given position
        /// </summary>
        /// <param name="index"></param>
        public void removeCutAt(int index)
        {
            if (index > -1 && index < cuts.Count)
                cuts.RemoveAt(index);
        } // RemoveCutAt

        /// <summary>
        /// removes a given cut from the list
        /// </summary>
        /// <param name="cut"></param>
        public void removeCut(Cut cut)
        {
            cuts.Remove(cut);
        }

        /// <summary>
        /// returns a deep copy of cuts
        /// </summary>
        /// <returns></returns>
        public CutsList getCopy()
        {
            CutsList ans = new CutsList();
            for (int i = 0; i < cuts.Count; i++)
                ans.AddCut(cuts[i].cutValue, cuts[i].cutLabel);
            return ans;
        } // getCopy

        /// <summary>
        /// returns minimal cut in list
        /// </summary>
        /// <returns></returns>
        public Cut getMinimalCut()
        {
            //if (cuts.Count == 0) return null;
            Cut cut = cuts[0];
            int i;
            for (i = 1; i < cuts.Count; i++)
                if (cuts[i].cutValue < cut.cutValue) cut = cuts[i];

            return cut;
        } // getMinInCutsList

        /// <summary>
        /// returns a sorted list
        /// </summary>
        /// <returns></returns>
        public CutsList getSortedCutsList()
        {
            CutsList copy = getCopy();
            CutsList ans = new CutsList();
            int i = 0;
            Cut temp;
            int count = copy.Count();
            while (i++ < count)
            {
                temp = copy.getMinimalCut();
                ans.AddCut(temp.cutValue, temp.cutLabel);
                copy.removeCut(temp);
            } // while
            return ans;
        } // getSortedCutsList

        /// <summary>
        /// create a list of bins from current cuts (last bins is Maximal)
        /// </summary>
        /// <returns></returns>
        public List<Bin> getBinsFromCuts()
        {
            int i;
            List<Bin> ans = new List<Bin>();

            if (cuts.Count == 0) return null;

            // add first bin
            ans.Add(new Bin(1, cuts[0].cutLabel, Bin.MinValue, cuts[0].cutValue));
            // add rest of bins but the last one.
            for (i = 1; i < cuts.Count; i++)
                ans.Add(new Bin(i + 1, cuts[i].cutLabel, cuts[i - 1].cutValue, cuts[i].cutValue));
            // add last bin
            ans.Add(new Bin(cuts.Count + 1, "High", cuts[cuts.Count - 1].cutValue, Bin.MaxValue));

            return ans;
        } // getBinsFromCuts


    } // class CutsList


    /// <summary>
    /// used to load the calculated data into a database
    /// </summary>
    public static class DBExporter
    {

        public static bool CreateBinsDefinitionsTable(string datasetName, bool cluster, DataBase db)
        {
            string strTableName = "BinsDefinitions" + datasetName;
            int numOfColumns = 9;
            if (cluster)
                numOfColumns = 13;

            string[] colnames = new string[numOfColumns];
            string[] coldefs = new string[numOfColumns];
            string[] coltypes = new string[numOfColumns];

            //StateID	IntervalClusterID	TemporalPropertyID	TemporalPropertyName	MethodName	StateID	BinLabel	BinFrom	BinTo	IntervalClusterLabel	IntervalClusterCentroid	IntervalClusterVariance	IntervalClusterSize
            /*
            // create Methods definitions table
            colnames[0] = Const.STR_STATE_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_CLUSTER_ID; coltypes[1] = "int"; coldefs[1] = " NOT NULL";
            colnames[2] = Const.STR_TEMPORAL_PROPERTY_ID; coltypes[2] = "int"; coldefs[2] = " NOT NULL";
            colnames[3] = Const.STR_TEMPORAL_PROPERTY_NAME; coltypes[3] = "nvarchar"; coldefs[3] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[4] = Const.STR_METHOD_NAME; coltypes[4] = "nvarchar"; coldefs[4] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[5] = Const.STR_RESULTS_ERROR1; coltypes[5] = "float"; coldefs[5] = " NOT NULL";
            colnames[6] = Const.STR_RESULTS_ENTROPY; coltypes[6] = "float"; coldefs[6] = " NULL";            
            colnames[7] = Const.STR_BIN_ID; coltypes[7] = "int"; coldefs[7] = " NOT NULL";
            colnames[8] = Const.STR_BIN_LABEL; coltypes[8] = "nvarchar"; coldefs[8] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[9] = Const.STR_BIN_FROM; coltypes[9] = "float"; coldefs[9] = " NOT NULL";
            colnames[10] = Const.STR_BIN_TO; coltypes[10] = "float"; coldefs[10] = " NOT NULL";
            colnames[11] = Const.STR_CLUSTER_LABEL; coltypes[11] = "nvarchar"; coldefs[11] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[12] = Const.STR_CLUSTER_MEAN; coltypes[12] = "float"; coldefs[12] = " NOT NULL";
            colnames[13] = Const.STR_CLUSTER_VARIANCE; coltypes[13] = "float"; coldefs[13] = " NOT NULL";
            colnames[14] = Const.STR_CLUSTER_SIZE; coltypes[14] = "int"; coldefs[14] = " NOT NULL, " +
                "CONSTRAINT [PK_" + strTableName + "] PRIMARY KEY CLUSTERED" +
                "([" + Const.STR_STATE_ID + "] ASC, [" +
                       Const.STR_CLUSTER_ID + "] ASC, [" +
                       Const.STR_TEMPORAL_PROPERTY_ID + "] ASC, [" +
                       Const.STR_BIN_ID + "] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] ";
            */
            // create Methods definitions table
            colnames[0] = Const.STR_STATE_ID;               coltypes[0] = "int";        coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_TEMPORAL_PROPERTY_ID;   coltypes[1] = "int";        coldefs[1] = " NOT NULL";
            colnames[2] = Const.STR_TEMPORAL_PROPERTY_NAME; coltypes[2] = "nvarchar";   coldefs[2] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[3] = Const.STR_METHOD_NAME;            coltypes[3] = "nvarchar";   coldefs[3] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[4] = "Score";                          coltypes[4] = "float";      coldefs[4] = " NOT NULL";
            colnames[5] = Const.STR_BIN_ID;                 coltypes[5] = "int";        coldefs[5] = " NOT NULL";
            colnames[6] = Const.STR_BIN_LABEL;              coltypes[6] = "nvarchar";   coldefs[6] = " (50) COLLATE Latin1_General_CI_AS";
            colnames[7] = Const.STR_BIN_FROM;               coltypes[7] = "float";      coldefs[7] = " NOT NULL";
            if (!cluster)
            {
                colnames[8] = Const.STR_BIN_TO;             coltypes[8] = "float";      coldefs[8] = " NOT NULL, " +
                    "CONSTRAINT [PK_" + strTableName + "] PRIMARY KEY CLUSTERED" +
                    "([" + Const.STR_STATE_ID + "] ASC, [" +
                           Const.STR_TEMPORAL_PROPERTY_ID + "] ASC, [" +
                           Const.STR_BIN_ID + "] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] ";

            } // if not clusters
            else
            {
                colnames[8] = Const.STR_BIN_TO;             coltypes[8] = "float";      coldefs[8] = " NOT NULL";
                colnames[9] = Const.STR_CLUSTER_LABEL;      coltypes[9] = "nvarchar";   coldefs[9] = " (50) COLLATE Latin1_General_CI_AS";
                colnames[10] = Const.STR_CLUSTER_MEAN;      coltypes[10] = "float";     coldefs[10] = " NOT NULL";
                colnames[11] = Const.STR_CLUSTER_VARIANCE;  coltypes[11] = "float";     coldefs[11] = " NOT NULL";
                colnames[12] = Const.STR_CLUSTER_SIZE;      coltypes[12] = "int";       coldefs[12] = " NOT NULL, " +
                    "CONSTRAINT [PK_" + strTableName + "] PRIMARY KEY CLUSTERED" +
                    "([" + Const.STR_STATE_ID + "] ASC, [" +
                           Const.STR_CLUSTER_ID + "] ASC, [" +
                           Const.STR_TEMPORAL_PROPERTY_ID + "] ASC, [" +
                           Const.STR_BIN_ID + "] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] ";

            } // if cluster


            try
            {
                db.CreateTable(strTableName, colnames, coltypes, coldefs);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        } // CreateBinsDefinitionsTable

        public static bool CreateTemporalPropertiesIntervalsTable(string datasetName, DataBase db)
        {
            string strTableName = "TemporalPropertiesIntervals" + datasetName;
            string[] colnames = new string[5];
            string[] coldefs = new string[5];
            string[] coltypes = new string[5];

            //StateID	IntervalClusterID	TemporalPropertyID	TemporalPropertyName	MethodName	StateID	BinLabel	BinFrom	BinTo	IntervalClusterLabel	IntervalClusterCentroid	IntervalClusterVariance	IntervalClusterSize

            // create Methods definitions table
            colnames[0] = Const.STR_EXPORT_ENTITY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_EXPORT_TEMPORAL_PROPERTY_ID; coltypes[1] = "int"; coldefs[1] = " NOT NULL";
            colnames[2] = Const.STR_EXPORT_STATE_ID; coltypes[2] = "int"; coldefs[2] = " NOT NULL";
            colnames[3] = Const.STR_EXPORT_START_TIME; coltypes[3] = "int"; coldefs[3] = " NOT NULL";
            colnames[4] = Const.STR_EXPORT_END_TIME; coltypes[4] = "int"; coldefs[4] = " NOT NULL , " +
                "CONSTRAINT [PK_" + strTableName + "] PRIMARY KEY CLUSTERED" +
                "([" + Const.STR_EXPORT_ENTITY_ID + "] ASC, [" +
                       Const.STR_EXPORT_TEMPORAL_PROPERTY_ID + "] ASC, [" +
                       Const.STR_EXPORT_STATE_ID + "] ASC, [" +
                       Const.STR_EXPORT_START_TIME + "] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] ";
            try
            {
                db.CreateTable(strTableName, colnames, coltypes, coldefs);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        } // CreateTemporalPropertiesIntervalsTable

    }//DBExporter class



    /* Directory structure :
 * - ROOT: 
 * 1) Entities.csv - Entities Table (Entity's name)
 * 2) Statics.csv - Static properties list (Property name)
 * 3) StaticsValues.csv - Static Properties values (EntityName, Property1 Value, Property2 Value... PropertyNValue)
 * 4) Temporals.csv - Temporal Properties list (Property name)
 * - Temporals:
 * 1) PropertyName.csv - Temporal property's values (time series) for each property.
 * (Entity Name, Time Stamp , Value)
 * 
 */
    /// <summary>
    /// Holds the base datasets loaded from files.
    /// </summary>
    public static class BaseDataSet
    {
        /// <summary>
        /// expected files names
        /// </summary>
        public static string entitiesfilepath = "entities.csv";
        public static string staticsfilepath = "statics.csv";
        public static string staticsvaluesfilepath = "staticsvalues.csv";
        public static string temporalsfilepath = "temporals.csv";
        public static string temporalsvaluesfolderpath = "Temporals";

        public static DataSet Entities;
        public static DataSet StaticProperties;
        public static DataSet StaticPropertiesValues;
        public static int StaticPropertiesEntities;
        public static DataSet TemporalProperties;
        public static DataSet TemporalPropertiesValues;
        public static int TemporalPropertiesCounter;

        /// <summary>
        /// load data from a given folder (including all files)
        /// </summary>
        /// <param name="basepath"></param>
        public static void LoadDataFromFolder(string basepath)
        {
            DataLoader.path = basepath;
            Entities = DataLoader.loadEntitiesListFromFile(entitiesfilepath);
            StaticProperties = DataLoader.loadStaticPropertiesListFromFile(staticsfilepath);
            StaticPropertiesValues = DataLoader.loadStaticPropertiesValuesListFromFile(staticsvaluesfilepath);
            TemporalProperties = DataLoader.loadTemporalPropertiesListFromFile(temporalsfilepath);
            TemporalPropertiesValues = DataLoader.loadTemporalPropertiesValuesListFromFile(temporalsvaluesfolderpath, TemporalProperties.Tables[0]);

        } // LoadDataFromFolder

        public static int getEntityID(string entityname)
        {
            int i = 0;
            while (i < Entities.Tables[0].Rows.Count && entityname.ToUpper() != ((string)Entities.Tables[0].Rows[i][Const.STR_ENTITY_NAME]).ToUpper()) i++;
            if (i == Entities.Tables[0].Rows.Count) return -1;
            else return int.Parse((string)Entities.Tables[0].Rows[i][Const.STR_ENTITY_ID]);
        }


        public static int getStaticPropertyID(string propertyname)
        {
            int i = 0;
            while (i < StaticProperties.Tables[0].Rows.Count && propertyname.ToUpper() != ((string)StaticProperties.Tables[0].Rows[i][Const.STR_STATIC_PROPERTY_NAME]).ToUpper()) i++;
            if (i == StaticProperties.Tables[0].Rows.Count) return -1;
            else return int.Parse((string)StaticProperties.Tables[0].Rows[i][Const.STR_STATIC_PROPERTY_ID]);
        }

        public static int getTemporalPropertyID(string propertyname)
        {
            int i = 0;
            while (i < TemporalProperties.Tables[0].Rows.Count && propertyname.ToUpper() != ((string)TemporalProperties.Tables[0].Rows[i][Const.STR_TEMPORAL_PROPERTY_NAME]).ToUpper()) i++;
            if (i == TemporalProperties.Tables[0].Rows.Count) return -1;
            else return int.Parse((string)TemporalProperties.Tables[0].Rows[i][Const.STR_TEMPORAL_PROPERTY_ID]);
        }


        public static void FillTables(DataBase db)
        {
            try
            {

                db.loadDataTableToTable(Const.STR_ENTITIES_TABLE, Entities);
                db.loadDataTableToTable(Const.STR_STATIC_PROPERTIES_TABLE, StaticProperties);
                db.loadDataTableToTable(Const.STR_TEMPORAL_PROPERTIES_TABLE, TemporalProperties);
                db.loadDataTableToTable(Const.STR_STATIC_PROPERTIES_VALUES_TABLE, StaticPropertiesValues);
/* */ //                CSVExporter.saveDataTableToCSV("D:\\t1.csv", TemporalPropertiesValues.Tables[0]);
                db.loadDataTableToTable(Const.STR_TEMPORAL_PROPERTIES_VALUES_TABLE, TemporalPropertiesValues);

                System.Console.WriteLine("All tables were filled successfully ");
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Not All tables were filled successfully");
            }
        }


        public static void CreateSequentialsTable(DataBase db)
        {
            string[] colnames = new string[2];
            string[] coldefs = new string[2];
            string[] coltypes = new string[2];
            // create Sequentials Table
            colnames[0] = Const.STR_SEQUENTIALS_PROPERTY_FROM; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_SEQUENTIALS_PROPERTY_TO; coltypes[1] = "int"; coldefs[1] = "NOT NULL," +
                "CONSTRAINT [PK_" + Const.STR_SEQUENTIALS_TABLE + "] PRIMARY KEY CLUSTERED" +
                "([" + Const.STR_SEQUENTIALS_PROPERTY_FROM + "] ASC,[" + Const.STR_SEQUENTIALS_PROPERTY_TO + "] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            db.CreateTable(Const.STR_SEQUENTIALS_TABLE, colnames, coltypes, coldefs);
        }

        public static void CreateMethodsDefinitionsTable(DataBase db)
        {
            string[] colnames = new string[5];
            string[] coldefs = new string[5];
            string[] coltypes = new string[5];
            // create Methods definitions table
            colnames[0] = Const.STR_TEMPORAL_PROPERTY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_METHODS_DEFINITIONS_ID; coltypes[1] = "int"; coldefs[1] = " NOT NULL";
            colnames[2] = Const.STR_METHOD_ID; coltypes[2] = "int"; coldefs[2] = " NOT NULL";
            colnames[3] = Const.STR_NUM_OF_BINS; coltypes[3] = "int"; coldefs[3] = " NOT NULL";
            colnames[4] = Const.STR_METHODS_DEFINITIONS_COMMENTS; coltypes[4] = "nvarchar"; coldefs[4] = "(MAX) COLLATE Latin1_General_CI_AS NULL, " +
                "CONSTRAINT [PK_" + Const.STR_METHODS_DEFINITIONS_TABLE + "] PRIMARY KEY CLUSTERED" +
                "([" + Const.STR_TEMPORAL_PROPERTY_ID + "] ASC, [" + Const.STR_METHODS_DEFINITIONS_ID + "] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] ";
            db.CreateTable(Const.STR_METHODS_DEFINITIONS_TABLE, colnames, coltypes, coldefs);
        }



        public static void CreateTables(DataBase db)
        {

            // create Entities Table
            string[] colnames = new string[2];
            string[] coldefs = new string[2];
            string[] coltypes = new string[2];

            colnames[0] = Const.STR_ENTITY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_ENTITY_NAME; coltypes[1] = "nvarchar"; coldefs[1] = "(50) COLLATE Latin1_General_CI_AS NULL," +
                "CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED" +
                "([EntityID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            db.CreateTable(Const.STR_ENTITIES_TABLE, colnames, coltypes, coldefs);


            // create Static properties Table
            colnames[0] = Const.STR_STATIC_PROPERTY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_STATIC_PROPERTY_NAME; coltypes[1] = "nvarchar"; coldefs[1] = "(50) COLLATE Latin1_General_CI_AS NULL," +
                "CONSTRAINT [PK_StaticProperties] PRIMARY KEY CLUSTERED" +
                "([StaticPropertyID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            db.CreateTable(Const.STR_STATIC_PROPERTIES_TABLE, colnames, coltypes, coldefs);


            CreateSequentialsTable(db);


            colnames = new string[3];
            coldefs = new string[3];
            coltypes = new string[3];

            // create temporal properties Table
            colnames[0] = Const.STR_TEMPORAL_PROPERTY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_TEMPORAL_PROPERTY_NAME; coltypes[1] = "nvarchar"; coldefs[1] = "(50) COLLATE Latin1_General_CI_AS NULL";
            colnames[2] = Const.STR_TEMPORAL_PROPERTY_SETTINGS; coltypes[2] = "nvarchar"; coldefs[2] = "(MAX) COLLATE Latin1_General_CI_AS NULL," +
                "CONSTRAINT [PK_TemporalProperties] PRIMARY KEY CLUSTERED" +
                "([TemporalPropertyID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            db.CreateTable(Const.STR_TEMPORAL_PROPERTIES_TABLE, colnames, coltypes, coldefs);

            // create static properties values table
            colnames[0] = Const.STR_ENTITY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_STATIC_PROPERTY_ID; coltypes[1] = "int"; coldefs[1] = " NOT NULL";
            colnames[2] = Const.STR_STATIC_PROPERTY_VALUE; coltypes[2] = "nvarchar"; coldefs[2] = "(50) COLLATE Latin1_General_CI_AS NULL, " +
                " CONSTRAINT [PK_StaticPropertiesValues] PRIMARY KEY CLUSTERED " +
                "([EntityID] ASC,[StaticPropertyID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            db.CreateTable(Const.STR_STATIC_PROPERTIES_VALUES_TABLE, colnames, coltypes, coldefs);


            colnames = new string[4];
            coldefs = new string[4];
            coltypes = new string[4];

            // create temporal properties values table
            colnames[0] = Const.STR_ENTITY_ID; coltypes[0] = "int"; coldefs[0] = " NOT NULL";
            colnames[1] = Const.STR_TEMPORAL_PROPERTY_ID; coltypes[1] = "int"; coldefs[1] = " NOT NULL";
            colnames[2] = Const.STR_TIME_STAMP; coltypes[2] = "int"; coldefs[2] = " NOT NULL";
            colnames[3] = Const.STR_TEMPORAL_PROPERTY_VALUE; coltypes[3] = "float"; coldefs[3] = " NOT NULL, " +
                "CONSTRAINT [PK_TemporalPropertiesValues] PRIMARY KEY CLUSTERED" +
                "([EntityID] ASC, [TemporalPropertyID] ASC, [TimeStamp] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] ";
            db.CreateTable(Const.STR_TEMPORAL_PROPERTIES_VALUES_TABLE, colnames, coltypes, coldefs);


            CreateMethodsDefinitionsTable(db);

        } // CreateTables

    } // class BaseDataSet


    /// <summary>
    /// Includes methods to load the data from csv files
    /// </summary>
    public static class DataLoader
    {

        public static string path;

        /// <summary>/// loads the entities list from a data file (.csv),
        /// Format: each entity's name in a row
        /// </summary>
        /// <returns>Dataset containing the entities dataset </returns>
        public static DataSet loadEntitiesListFromFile(string filepath)
        {


            // entity's ID
            int id = 1;

            string str;
            string[] vals;
            object[] row = new object[2];

            // build new DataTable
            DataSet entities = new DataSet(Const.STR_ENTITIES_TABLE);
            entities.Tables.Add(new DataTable(Const.STR_ENTITIES_TABLE));
            entities.Tables[0].Columns.Add(Const.STR_ENTITY_ID);
            entities.Tables[0].Columns.Add(Const.STR_ENTITY_NAME);

            using (StreamReader sr = new StreamReader(path + "\\" + filepath))
            {
                while ((str = sr.ReadLine()) != null) // while reading
                {
                    // add new entity
                    vals = str.Split(',');
                    row[0] = id++;
                    row[1] = vals[0];
                    entities.Tables[0].Rows.Add(row);
                } // while

            } // using

            return entities;
        } // loadEntitiesListFromFile


        /// <summary>
        /// loads the static properties list from a data file (.csv),
        /// Format: each property's name in a row
        /// </summary>
        /// <returns>Dataset containing the static properties dataset </returns>
        public static DataSet loadStaticPropertiesListFromFile(string filepath)
        {

            // property's ID
            int id = 1;

            string str;
            string[] vals;
            object[] row = new object[2];

            // build new DataTable
            DataSet statics = new DataSet(Const.STR_STATIC_PROPERTIES_TABLE);
            statics.Tables.Add(new DataTable(Const.STR_STATIC_PROPERTIES_TABLE));
            statics.Tables[0].Columns.Add(Const.STR_STATIC_PROPERTY_ID);
            statics.Tables[0].Columns.Add(Const.STR_STATIC_PROPERTY_NAME);

            using (StreamReader sr = new StreamReader(path + "\\" + filepath))
            {
                while ((str = sr.ReadLine()) != null) // while reading
                {
                    // add new property
                    vals = str.Split(',');
                    row[0] = id++;
                    row[1] = vals[0];
                    statics.Tables[0].Rows.Add(row);
                } // while

            } // using

            return statics;
        } // loadStaticPropertiesListFromFile

        /// <summary>
        /// loads the temporal properties list from a data file (.csv),
        /// Format: each property's name in a row
        /// </summary>
        /// <returns>Dataset containing the temporal properties dataset </returns>
        public static DataSet loadTemporalPropertiesListFromFile(string filepath)
        {

            // property's ID
            int id = 1;

            string str;
            string[] vals;
            object[] row = new object[3];

            // build new DataTable
            DataSet temporals = new DataSet(Const.STR_TEMPORAL_PROPERTIES_TABLE);
            temporals.Tables.Add(new DataTable(Const.STR_TEMPORAL_PROPERTIES_TABLE));
            temporals.Tables[0].Columns.Add(Const.STR_TEMPORAL_PROPERTY_ID);
            temporals.Tables[0].Columns.Add(Const.STR_TEMPORAL_PROPERTY_NAME);
            temporals.Tables[0].Columns.Add(Const.STR_TEMPORAL_PROPERTY_SETTINGS);

            using (StreamReader sr = new StreamReader(path + "\\" + filepath))
            {
                // read columns names
                sr.ReadLine();
                while ((str = sr.ReadLine()) != null) // while reading
                {
                    // add new property
                    vals = str.Split(',');
                    row[0] = id++;
                    row[1] = vals[0];
                    row[2] = "";
                    temporals.Tables[0].Rows.Add(row);
                } // while

            } // using

            return temporals;
        } // loadTemporalPropertiesListFromFile


        /// <summary>
        /// loads the temporal properties values list from data files (.csv), each property in its own file
        /// Format: each property's time stamp in a row: Entity's name, Time, Property Value
        /// </summary>
        /// <returns>Dataset containing the temporal properties values dataset </returns>
        public static DataSet loadTemporalPropertiesValuesListFromFile(string folderpath, DataTable properties)
        {

            int i;
            string str;
            string[] vals;
            object[] row = new object[4];
            int count = 0;

            // build new DataTable
            DataSet temporalsvalues = new DataSet(Const.STR_TEMPORAL_PROPERTIES_VALUES_TABLE);
            temporalsvalues.Tables.Add(new DataTable(Const.STR_TEMPORAL_PROPERTIES_VALUES_TABLE));
            temporalsvalues.Tables[0].Columns.Add(Const.STR_ENTITY_ID);
            temporalsvalues.Tables[0].Columns.Add(Const.STR_TEMPORAL_PROPERTY_ID);
            temporalsvalues.Tables[0].Columns.Add(Const.STR_TIME_STAMP);
            temporalsvalues.Tables[0].Columns.Add(Const.STR_TEMPORAL_PROPERTY_VALUE);

            for (i = 0; i < properties.Rows.Count; i++)
            {
                // add check wether the file exists or not
                if (System.IO.File.Exists(path + "\\" + folderpath + "\\" + properties.Rows[i][Const.STR_TEMPORAL_PROPERTY_NAME] + ".csv"))
                {
                    count++;
                    row[1] = properties.Rows[i][Const.STR_TEMPORAL_PROPERTY_ID];

                    using (StreamReader sr = new StreamReader(path + "\\" + folderpath + "\\" + properties.Rows[i][Const.STR_TEMPORAL_PROPERTY_NAME] + ".csv"))
                    {
                        // read columns names
                        sr.ReadLine();


                        while ((str = sr.ReadLine()) != null && str.Trim() != "") // while reading
                        {
                            // add new property value
                            vals = str.Split(',');
                            row[0] = BaseDataSet.getEntityID(vals[0]);
                            row[2] = int.Parse(vals[1]);
                            row[3] = double.Parse(vals[2]);
                            temporalsvalues.Tables[0].Rows.Add(row);
                        } // while
                    } // using
                } // if exists
            } // for

            BaseDataSet.TemporalPropertiesCounter = count;

            return temporalsvalues;
        } // loadEntitiesListFromFile



        /// <summary>
        /// loads the static properties values list from a data file (.csv),
        /// Format: entity's name , static proeprty no. 1 value, static property no. 2 value...
        /// properties order by the first row - columns' names
        /// </summary>
        /// <returns>Dataset containing the static properties values  dataset </returns>
        public static DataSet loadStaticPropertiesValuesListFromFile(string filepath)
        {
            int i;
            int[] indexes;
            string str;
            string[] vals;
            // row format: EntityID, StaticPropertyID, StaticPropertyValue
            object[] row = new object[3];
            int count = 0;

            DataSet staticsvalues;

            using (StreamReader sr = new StreamReader(path + "\\" + filepath))
            {
                // first set the order of properties in list by first row - columns' names
                vals = sr.ReadLine().Split(',');
                indexes = new int[vals.Length - 1];

                for (i = 1; i < vals.Length; i++)
                    indexes[i - 1] = BaseDataSet.getStaticPropertyID(vals[i]);

                // build new DataTable
                staticsvalues = new DataSet(Const.STR_STATIC_PROPERTIES_VALUES_TABLE);
                staticsvalues.Tables.Add(new DataTable(Const.STR_STATIC_PROPERTIES_VALUES_TABLE));
                staticsvalues.Tables[0].Columns.Add(Const.STR_ENTITY_ID);
                staticsvalues.Tables[0].Columns.Add(Const.STR_STATIC_PROPERTY_ID);
                staticsvalues.Tables[0].Columns.Add(Const.STR_STATIC_PROPERTY_VALUE);


                while ((str = sr.ReadLine()) != null) // while reading
                {
                    count++;
                    // add entity's static properties
                    vals = str.Split(',');
                    // get entity's id by name
                    row[0] = BaseDataSet.getEntityID(vals[0]);
                    for (i = 1; i < vals.Length; i++)
                    {
                        row[1] = indexes[i - 1];
                        row[2] = vals[i];
                        staticsvalues.Tables[0].Rows.Add(row);
                    } // for
                } // while

            } // using

            BaseDataSet.StaticPropertiesEntities = count;
            return staticsvalues;
        } // loadStaticPropertiesValuesListFromFile

    } // DataLoader



    public static class DBServer
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

    } // class DBServer


    /// <summary>
    /// merge sort a double list
    /// </summary>
    static public class MergeSort
    {

        public static void Sort(double[] data, int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;
                Sort(data, left, middle);
                Sort(data, middle + 1, right);
                Merge(data, left, middle, middle + 1, right);
            }
        }

        public static void Merge(double[] data, int left, int middle, int middle1, int right)
        {
            int oldPosition = left;
            int size = right - left + 1;
            double[] temp = new double[size];
            int i = 0;

            while (left <= middle && middle1 <= right)
            {
                if (data[left] <= data[middle1])
                    temp[i++] = data[left++];
                else
                    temp[i++] = data[middle1++];
            }
            if (left > middle)
                for (int j = middle1; j <= right; j++)
                    temp[i++] = data[middle1++];
            else
                for (int j = left; j <= middle; j++)
                    temp[i++] = data[left++];
            Array.Copy(temp, 0, data, oldPosition, size);
        }

    } // class MergeSort


    // Several functions used in different discretization methods
    static public class Methods
    {


        public class Subsets
        {

            public static List<double[]> chooseOrdered(List<double> list)
            {
                return chooseOrdered(new List<double>(), list);
            }


            public static List<double[]> chooseOrdered(List<double> current, List<double> left)
            {

                List<double[]> ans = new List<double[]>();
                if (left.Count == 0)
                {
                    ans.Add(current.ToArray());
                }
                else
                {
                    List<double> newcur;
                    List<double> newleft;
                    for (int i = 0; i < left.Count; i++)
                    {
                        newcur = current.GetRange(0, current.Count);
                        newcur.Add(left[i]);
                        newleft = left.GetRange(0, left.Count);
                        newleft.RemoveAt(i);
                        ans.AddRange(chooseOrdered(newcur, newleft));
                    }
                }
                return ans;
            } // chooseOrdered


            public static List<int[]> choose(int count, int low, int high)
            {
                int[] currentInstance = new int[count];
                List<int[]> instances = new List<int[]>();
                choose(count, low, high, currentInstance, 0, instances);
                return instances;
            }


            public static void choose(int count, int low, int high,
                        int[] currentInstance, int index, List<int[]> instances)
            {
                int lastValue;
                if (index == 0)
                    lastValue = low - 1;
                else
                    lastValue = currentInstance[index - 1];
                for (int i = lastValue + 1; i <= high; i++)
                {
                    currentInstance[index] = i;
                    if (index + 1 < count)
                        choose(count, low, high, currentInstance, index + 1, instances);
                    else
                    {
                        int[] clone = (int[])currentInstance.Clone();
                        instances.Add(clone);
                    }
                }
            }


            public static int[] invert(int[] currentVector, int low, int high)
            {
                Dictionary<int, bool> currentValues = new Dictionary<int, bool>();
                for (int i = 0; i < currentVector.Length; i++)
                {
                    currentValues.Add(currentVector[i], true);
                }
                int[] inverted = new int[high - low - currentVector.Length + 1];
                int j = 0;
                for (int i = low; i <= high; i++)
                    if (!currentValues.ContainsKey(i))
                    {
                        inverted[j] = i;
                        j++;
                    }
                return inverted;
            } // invert
        } // class Subsets



        public static string getArrayString<T>(List<T> list, string seperator)
        {
            if (list.Count ==  0) return "";
            string str = list[0].ToString();
            for (int i = 1; i < list.Count; i++)
                str += seperator + list[i].ToString();
            return str;
        }

        public static string getArrayString<T>(T[] list, string seperator)
        {
            if (list.Length == 0) return "";
            string str = list[0].ToString();
            for (int i = 1; i < list.Length; i++)
                str += seperator + list[i].ToString();
            return str;
        }

        public static double[] flattenList(double[][] lists)
        {
            double[] list;
            int len = 0;
            int i, j, ind = 0;
            for (i = 0; i < lists.Length; i++)
                len += lists[i].Length;

            list = new double[len];
            for (i = 0; i < lists.Length; i++)
                for (j = 0; j < lists[i].Length; j++)
                    list[ind++] = lists[i][j];

            return list;

        }

        /// <summary>
        /// determines wether a file exists or not
        /// </summary>
        /// <param name="path">path to file</param>
        /// <param name="filename">file's name</param>
        /// <returns></returns>
        public static bool isFileExists(string path, string filename)
        {
            string slash = "\\";
            if (path[path.Length - 1] == '\\') slash = "";
            return System.IO.File.Exists(path + slash + filename);
        } // isFileExists


        public static bool isLegalFilename(string str)
        {
            int intLastSlashPos = str.LastIndexOf('\\');
            if (intLastSlashPos < 1) // only file name
                return true;
            else
            {
                if (!System.IO.Directory.Exists(str.Substring(0, intLastSlashPos)))
                    return true;
                else return false;
            }
        }


        public static string replaceCharsInString(string str, char chOrg, char chNew)
        {
            char[] strNew = new char[str.Length];
            int i;
            for (i = 0; i < str.Length; i++)
            {
                strNew[i] = str[i];
                if (str[i] == chOrg) strNew[i] = chNew;
            }

            return new string(strNew);
        } // replaceCharsInString

        /// <summary>
        /// return the defined row if exists int he given DataTable
        /// </summary>
        /// <param name="table">given DataTable</param>
        /// <param name="selectString">selest expression (sql format)</param>
        /// <returns>the defined row</returns>
        public static DataRow getRowInDataTable(DataTable table, string selectString)
        {
            DataRow[] rows = table.Select(selectString);
            if (rows.Length > 0)
                return rows[0];
            else
                return null;

        } // getRowInDataTable


        public static double[] getDistinctList(double[] lst)
        {
            int i, j;
            List<double> ans = new List<double>();
            Dictionary<double, int> dictDistinct = new Dictionary<double, int>();
            for (i = 0; i < lst.Length ; i++)
                if (!dictDistinct.TryGetValue(lst[i], out j))
                {
                    ans.Add(lst[i]);
                    dictDistinct.Add(lst[i], i);
                }
            return ans.ToArray ();
        }



        /// <summary>
        /// Gets a whole double column froma DataTable
        /// </summary>
        /// <param name="table">DataTable to load column from</param>
        /// <param name="columnName">column's name</param>
        /// <returns>a double array containing the column's values</returns>
        public static double[] getDoubleColumnFromDataTable(DataTable table, string columnName)
        {
            double[] ans = new double[table.Rows.Count];
            int i;
            for (i = 0; i < ans.Length; i++)
                ans[i] = (double)table.Rows[i][columnName];
            return ans;
        } // getDoubleColumnFromDataTable

        /// <summary>
        /// Gets a whole double column froma DataTable
        /// </summary>
        /// <param name="table">DataTable to load column from</param>
        /// <param name="columnName">column's name</param>
        /// <returns>a double array containing the column's values</returns>
        public static int[] getIntColumnFromDataTable(DataTable table, string columnName)
        {
            int[] ans = new int[table.Rows.Count];
            int i;
            for (i = 0; i < ans.Length; i++)
                ans[i] = (int)table.Rows[i][columnName];
            return ans;
        } // getDoubleColumnFromDataTable

        /// <summary>
        /// Discretizes a single value by given cuts list
        /// </summary>
        /// <param name="value">value to be discretized</param>
        /// <param name="cuts">cuts list (levels definitions)</param>
        /// <returns>discretized value (level number)</returns>
        public static int getDiscretizedValue(double value, double[] cuts)
        {
            int i = 0;
            while (i < cuts.Length && value >= cuts[i]) i++;
            return i + 1;
        } // getDiscretizedValue


        /// <summary>
        /// Discretizes a list of values, by given list of cuts
        /// </summary>
        /// <param name="data">data to be discretized</param>
        /// <param name="cuts">cuts list (levels definitions)</param>
        /// <returns>list of discretized data, by levels numbers</returns>
        public static int[] DiscretizeData(double[] data, double[] cuts)
        {
            int[] DiscretizedData = new int[data.Length];
            int i;
            for (i = 0; i < data.Length; i++)
                DiscretizedData[i] = getDiscretizedValue(data[i], cuts);
            return DiscretizedData;
        } // DiscretizeData



        /// <summary>
        /// returns a list containing all of the values of all the series given as parameters
        /// used to dicretize data of different entities
        /// </summary>
        /// <param name="series">array of time series</param>
        /// <returns>an array containing all of the values, used for bins calculation</returns>
        static public double[] getTimeSeriesJoinedValues(TimeSeries[] series)
        {
            int i, j;
            long size = 0, ind = 0;
            for (i = 0; i < series.Length; i++)
                size += series[i].timeseriesvalues.Count;
            double[] total = new double[size];
            for (i = 0; i < series.Length; i++)
                for (j = 0; j < series[i].timeseriesvalues.Count; j++, ind++)
                    total[ind] = series[i].getValueAt(j);
            return total;

        } //getTimeSeriesJoinedValues

        /// <summary>
        /// Markov chain maximal likelihood - returns the diagonal only, stating
        /// the probability to stay in a certain state for all states.
        /// </summary>
        /// <param name="data">time serie</param>
        /// <param name="states">number of states</param>
        /// <param name="forbidden">transitions to exclude from count (by indexes)</param>
        /// <returns>diagonal of markov matrix </returns>
        public static double[] MCML(int[] data, int states, int[] forbidden)
        {
            int i;

            int n = data.Length - forbidden.Length;
            int k = states;
            double[] countArr = new double[k];
            double[] diagonal = new double[k];
            for (i = 0; i < countArr.Length; i++)
                countArr[i] = 0;
            for (i = 0; i < diagonal.Length; i++)
                diagonal[i] = 0;

            // calculate self transitions
            for (i = 0; i < n - 1; i++)
            {
                // count only if transition is legal (not in forbidden)
                if (inList(forbidden, i + 1) == -1)
                {
                    // count self transitions
                    if (data[i] == data[i + 1]) diagonal[data[i] - 1]++;
                    // count transitions
                    countArr[data[i] - 1]++;
                } // if legal
            } // for
            //countArr [data[i]-1]++;

            for (i = 0; i < k; i++)
                if (countArr[i] != 0)
                    diagonal[i] /= countArr[i];

            return diagonal;

        } // MCML

        /// <summary>
        /// calculates the kullback liebler divergence of two equal sized
        /// probability groups
        /// </summary>
        /// <param name="p">group p</param>
        /// <param name="q">group q</param>
        /// <returns>The divergence value (a double)</returns>
        static public double getKLDiv(double[] p, double[] q)
        {
            double kldiv = 0;
            int i;
            double d;
            for (i = 0; i < p.Length; i++)
            {
                d = (q[i] != 0) ? (p[i] / q[i]) : Const.EPS;
                kldiv += p[i] * Math.Log(d, Math.E);
            }
            return (p.Length > 0) ?  (kldiv / ((double)p.Length)) : 0;
        } // getKLDiv

        /// <summary>
        /// The symetric version of kullback liebler diversion,
        /// which is the mean of both directions
        /// </summary>
        /// <param name="p">group p</param>
        /// <param name="q">group q</param>
        /// <returns>symetric kullback liebler diversion</returns>
        static public double getSKLDiv(double[] p, double[] q)
        {
            return 0.5 * (getKLDiv(p, q) + getKLDiv(q, p));
        } // getSKLDiv


        /* returns the array replicated n times as a matrix, 
         * where all the rows are equal to the given array
         *
        public static double[][] ReplicateRow(double[] data, int n)
        {
            double[][] Result = new double[n][];
            for (int i = 0; i < n; i++)
                Result[i] = data;
            return Result;
        } // ReplicateRow
        */
        /* returns the array replicated n times as a matrix, 
         * where all the Columns are equal to the given array
         *
        public static double[][] ReplicateColumn(double[] data, int n)
        {
            double[][] Result = new double[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                Result[i] = new double[n];
                for (int j = 0; j < n; j++)
                    Result[i][j] = data[i];
            }
            return Result;
        } // ReplicateColumn
        */

        public static int[] getLevelsByCuts(double[] cuts)
        {
            int[] ans = new int[cuts.Length + 1];
            for (int i = 1; i <= ans.Length; i++)
                ans[i - 1] = i;
            return ans;
        }


        /// <summary>
        /// Compare between the Items of two arrays, by order
        /// </summary>
        /// <param name="arr1">first array</param>
        /// <param name="arr2">second array</param>
        /// <returns>true if same Items, false if not</returns>
        static public bool compareArrays(double[] arr1, double[] arr2)
        {
            int i = 0;
            bool ans = arr1.Length == arr2.Length;
            while (ans && i < arr1.Length)
                if (arr1[i] != arr2[i++]) ans = false;
            return ans;
        } // compareArrays


        /// <summary>
        /// tells wether a value is in a given array, and returns its index
        /// </summary>
        /// <param name="list">list of values</param>
        /// <param name="what">value to look for</param>
        /// <returns>index of value if found, otherwise -1</returns>
        public static int inList(int[] list, int what)
        {
            int ans = -1;
            for (int i = 0; i < list.Length && ans == -1; i++)
                if (list[i] == what) ans = i;
            return ans;
        } // inList


        /// <summary>
        /// sorts a given list, from smallest to largest
        /// </summary>
        /// <param name="data">given list</param>
        /// <returns></returns>
        public static double[] sortList(double[] data)
        {
            double[] dataCopy = new double[data.Length];
            data.CopyTo(dataCopy, 0);

            MergeSort.Sort(dataCopy, 0, data.Length - 1);

            return dataCopy;

        } // sortList

        /// <summary>
        /// returns a list of values complying to given list of requested percentiles of the original data list.
        /// </summary>
        /// <param name="list">data to get values from</param>
        /// <param name="percList">list of requested percentiles (0 to 1)</param>
        /// <returns>list of values for each requested percentile</returns>
        static public double[] getListPrctile(double[] list, double[] percList)
        {
            int i;
            double[] result = new double[percList.Length];
            double temp;

            /*if (list.Length < 2)
            {
                result[i] = list[;
                return result;
            } // if not enough data
*/
            for (i = 0; i < percList.Length; i++)
            {
                if (percList[i] > 0)
                {
                    temp = percList[i] * list.Length + 0.5;
                    if (temp >= list.Length) temp = list.Length - 1;
                    if (temp < 1) temp = 1;
                    temp = list[(int)Math.Floor(temp) - 1] + (temp - Math.Floor(temp)) * (list[(int)Math.Floor(temp)] - list[(int)Math.Floor(temp) - 1]);
                    result[i] = temp;
                } // if legal
                else
                    result[i] = 0;
            } // for
            return result;
        } // getTimeSeriesPrctile

        /* returns the sum of values of as given list from index ind1 to index ind2
         */
        /// <summary>
        /// returns the sum of a part of a list (for a whole list use 0, list.Length)
        /// </summary>
        /// <param name="list">given list</param>
        /// <param name="ind1">starting index</param>
        /// <param name="ind2">ending index (not included)</param>
        /// <returns></returns>
        static public double getListSum(double[] list, int ind1, int ind2)
        {
            double sum = 0;
            for (int i = ind1; i < ind2; i++)
                sum += list[i];
            return sum;
        } // getSumOfSubList


        /// <summary>
        /// gets a part of a list's standard deviation by given average
        /// </summary>
        /// <param name="list">given list</param>
        /// <param name="ind1">starting index</param>
        /// <param name="ind2">ending index (not included)</param>
        /// <param name="mean">average of list's items</param>
        /// <returns>the standard deviation of the list by given average</returns>
        static public double getListStandardDeviation(double[] list, int ind1, int ind2, double mean)
        {
            double stddev = 0;
            for (int i = ind1; i < ind2; i++)
                stddev += Math.Pow(list[i] - mean, 2);
            stddev /= (ind2 - ind1);
            stddev = Math.Sqrt(stddev);
            return stddev;
        } // getTimeSeriesStandardDeviation


        /// <summary>
        /// returns the variance of a given list and its mean
        /// the variance is defined to be the average of squared distances of the list items from the mean.
        /// </summary>
        /// <param name="list">list to find variance of</param>
        /// <param name="mean">list's mean</param>
        /// <returns></returns>
        static public double getListVariance(double[] list, double mean)
        {
            int i;
            double var = 0;
            for (i = 0; i < list.Length; i++)
                var += Math.Pow(list[i] - mean, 2);
            return (i != 0) ? (var / i) : 0;
        } // getListVariance


        /* Creates list of Bin Objects using a list of cuts.
         */
        /// <summary>
        /// creates a bins list by given cuts. infinty is defined as 10^6.
        /// </summary>
        /// <param name="cuts"></param>
        /// <returns></returns>
        static public List<Bin> CreateBins(double[] cuts, string[] baseLabels)
        {
            List<Bin> Bins = new List<Bin>();
            string[] labels = Bin.getBinsLabels(cuts.Length + 1, baseLabels);
            int i;
            Bins.Add(new Bin(1, labels[0], Bin.MinValue, cuts[0]));
            for (i = 2; i <= cuts.Length; i++)
                Bins.Add(new Bin(i, labels[i - 1], cuts[i - 2], cuts[i - 1]));
            Bins.Add(new Bin(i, labels[i - 1], cuts[cuts.Length - 1], Bin.MaxValue));
            return Bins;
        } // CreateBins

        /* Creates list of Bin Objects using a list of cuts.
 */
        /// <summary>
        /// creates a bins list by given cuts. infinty is defined as 10^6.
        /// </summary>
        /// <param name="cuts"></param>
        /// <returns></returns>
        static public List<Bin> CreateBins(double[] cuts, List<string> names)
        {
            List<Bin> Bins = new List<Bin>();
            // Add first Bin
            int i;
            Bins.Add(new Bin(1, names[0], Bin.MinValue, cuts[0]));
            for (i = 2; i <= cuts.Length; i++)
                Bins.Add(new Bin(i, names[i - 1], cuts[i - 2], cuts[i - 1]));
            Bins.Add(new Bin(i, "Highest", cuts[cuts.Length - 1], Bin.MaxValue));
            return Bins;
        } // CreateBins

        /// <summary>
        /// convert list of bins to array of cuts
        /// </summary>
        /// <param name="bins"> list of bins</param>
        /// <returns>array of cuts</returns>
        static public double[] Bins2Cuts(List<Bin> bins)
        {
            int i;
            double[] cuts = new double[bins.Count - 1];
            for (i = 0; i < bins.Count - 1; i++)
                cuts[i] = bins[i]._highlimit;

            return cuts;
        } // Bins2Cuts

    } // Methods

    /// <summary>
    /// Holds a list of parameters and their values
    /// used for the different methods of discretization, requiring different parameters
    /// </summary>
    public class Parameters
    {
        /// <summary>
        ///  holds the value for an unfound parameter requested by the user
        /// </summary>
        public static object NotFound = new int();
        // parameters names list
        public List<string> paramsnames;
        // parameters values list
        public List<object> paramsvalues;

        /// <summary>
        /// basic constructor 
        /// </summary>
        public Parameters()
        {
            paramsnames = new List<string>();
            paramsvalues = new List<object>();
        } // constructor;

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">object to copy</param>
        public Parameters(Parameters copy)
        {
            string s;
            paramsnames = new List<string>();
            paramsvalues = new List<object>();
            for (int i = 0; i < copy.paramsnames.Count; i++)
            {
                paramsnames.Add(copy.paramsnames[i]);

                s = copy.paramsvalues[i].GetType().Name;
                if (copy.paramsvalues[i].GetType().Name == "Int32")
                    paramsvalues.Add(((int)copy.paramsvalues[i]));
                else if (copy.paramsvalues[i].GetType().Name == "Int64")
                    paramsvalues.Add(((long)copy.paramsvalues[i]));
                else if (copy.paramsvalues[i].GetType().Name == "String")
                    paramsvalues.Add(((string)copy.paramsvalues[i]));
                else if (copy.paramsvalues[i].GetType().Name == "Boolean")
                    paramsvalues.Add(((bool)copy.paramsvalues[i]));
                else if (copy.paramsvalues[i].GetType().Name == "Long")
                    paramsvalues.Add(((long)copy.paramsvalues[i]));
                else if (copy.paramsvalues[i].GetType().Name == "Decimal")
                    paramsvalues.Add(((decimal)copy.paramsvalues[i]));
                else if (copy.paramsvalues[i].GetType().Name == "Double")
                    paramsvalues.Add(((double)copy.paramsvalues[i]));
            } // for
        } // copy constructor


        /// <summary>
        /// Adds a new parameter to the parameters list
        /// </summary>
        /// <param name="name">name of parameter to add</param>
        /// <param name="value">parameter's value</param>
        public void Add(string name, object value)
        {
            if (paramsnames.IndexOf(name) >= 0)
                setParam(name, value);
            else
            {
                paramsnames.Add(name);
                paramsvalues.Add(value);
            }

        } // Add

        /// <summary>
        /// Remove a parameter from the object
        /// </summary>
        /// <param name="name">name of parameter to remove</param>
        public void Remove(string name)
        {
            int ind = paramsnames.IndexOf(name);
            if (ind >= 0)
            {
                paramsnames.RemoveAt(ind);
                paramsvalues.RemoveAt(ind);
            } // if
        } // Remove

        /// <summary>
        /// gets a parameter's value by its name
        /// </summary>
        /// <param name="name">parameter's name</param>
        /// <returns>value if found, NotFound object if not</returns>
        public object getParam(string name)
        {
            int ind = paramsnames.IndexOf(name);
            if (ind != -1) return paramsvalues[ind];
            else return NotFound;
        } // getParam

        /// <summary>
        /// changes a parameter's value
        /// </summary>
        /// <param name="name">parameter's name</param>
        /// <param name="value">new value to update to</param>
        /// <returns>null if found and updated, NotFound if not</returns>
        public object setParam(string name, object value)
        {
            int ind = paramsnames.IndexOf(name);
            if (ind != -1)
            {
                paramsvalues[ind] = value;
                return null;
            }
            else return NotFound;
        } // setParam

        /// <summary>
        /// return number of parameters
        /// </summary>
        /// <returns>number of parameters</returns>
        public int Count()
        {
            return paramsnames.Count;
        } // Count

        /// <summary>
        /// gets the parameters string of current object
        /// </summary>
        /// <param name="seperate">seperator character</param>
        /// <param name="exclude">exclude list (dont include in string)</param>
        /// <returns>parameters string</returns>
        public string getParametersString(char seperate, List<string> exclude)
        {
            int i;
            string ans = "";
            for (i = 0; i < paramsnames.Count - 1; i++)
                if (exclude.IndexOf(paramsnames[i]) == -1)
                    ans += paramsnames[i] + seperate.ToString() + paramsvalues[i] + seperate.ToString();
            if (paramsnames.Count > 0 && exclude.IndexOf(paramsnames[i]) == -1)
                ans += paramsnames[i] + seperate.ToString() + paramsvalues[i].ToString();
            return ans;
        }

    } // Class Parameters


    /// <summary>
    /// Every Discretization method has to implement this interface!
    /// </summary>
    public interface DiscretizationMethod
    {
        long getID();
        void setID(long ID);
        Parameters getParameters();
        void setParameters(Parameters _parameters);

        // calculate bins using the current discretization method
        List<Bin> CalculateBins(double[] list);
        // returns a list of discretized data using cuts
        int[] DiscretizeData(double[] data, double[] cuts);
        // returns a list of discretized values (1,2,3..) for the given list of values
        int[] getDiscretizedValues(double[] data, double[] cuts);
    } // interface DiscretizationMethod


    /// <summary>
    /// holds data for the list of available discretization modules names
    /// usually : SAX, Persist, EQF, EQW, Tendency, K-Means
    /// </summary>
    public class DiscretizationMethodName
    {
        public int ID;
        public string name;

        /// <summary>
        /// basic constructor, creates an object by given parameters
        /// </summary>
        /// <param name="ID">Method module ID number</param>
        /// <param name="name">Method module name</param>
        public DiscretizationMethodName(int ID, string name)
        {
            this.ID = ID;
            this.name = name;
        } // constructor

    } // class DiscretizationMethodName

    /// <summary>
    /// Holds data for temporal property
    /// </summary>
    public class TemporalProperty
    {
        // Property ID
        public long _ID;
        // Property Name;
        public string _label;
        // value
        public double _value;

        /// <summary>
        /// basic constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="label"></param>
        /// <param name="value"></param>
        public TemporalProperty(long ID, string label, double value)
        {
            _ID = ID;
            _label = label;
            _value = value;
        } // Constructor

    } // Class TemporalProperty


    /// <summary>
    /// holds data for a static property
    /// </summary>
    public class StaticProperty
    {
        // Property ID
        public long _ID;
        // Property Name
        public string _name;
        // Property Value
        public object _value;

        public StaticProperty(long ID, string name, object value)
        {
            _ID = ID;
            _name = name;
            _value = value;
        } // constructor
    } // Class StaticProperty


    /* defines an entity (whose properties we get as a time series and static).
     * an entity has a list of static properties, and timeseries of temporal
     * properties.
     */
    public class Entity
    {
        public long _ID;
        public string _name;
        public int _class = -1;
        public List<StaticProperty> staticProperties;

        public Entity(long ID, string name)
        {
            _ID = ID;
            _name = name;
            staticProperties = new List<StaticProperty>();
        } // basic constructor

        public int getStaticPropertyIndexInList(StaticProperty property)
        {
            int i = 0;
            while (i < staticProperties.Count && staticProperties[i]._ID != property._ID) i++;
            if (i == staticProperties.Count) return -1;

            else return i;
        }
    } // Class Entity


    /* Describes a time stamp, a point in time holding a Temporal property 
     * value.
     */
    public class TimeStamp
    {
        public long _ID;
        public long _time;
        public TemporalProperty temporalproperty;

        public TimeStamp(long time, TemporalProperty prop)
        {
            _time = time;
            temporalproperty = prop;
        } // basic constructor

    } // Class TimeStamp

    /// <summary>
    /// Describes a time interval (time window with a certain (discretized) value)
    /// </summary>
    public class TimeInterval
    {
        public long ID;
        public TemporalProperty value;
        public TimeStamp start;
        public TimeStamp end;

        public TimeInterval(long id, TemporalProperty property, TimeStamp starttime, TimeStamp endtime)
        {
            ID = id;
            value = property;
            start = starttime;
            end = endtime;
        } // basic constructor

    } // TimeInterval


    /* Describes a Time Series of a certain temporal property, built 
     * of time stamps.
     */
    public class TimeSeries
    {
        public long _ID;
        // holds the temporal property described by this series,
        // this object has no value.
        public TemporalProperty property;
        public List<TimeStamp> timeseriesvalues;
        public List<TimeInterval> timeintervals;

        public TimeSeries(long id, TemporalProperty temporalproperty)
        {
            _ID = id;
            property = temporalproperty;
            timeseriesvalues = new List<TimeStamp>();
            timeintervals = new List<TimeInterval>();
        } // basic Constructor

        /* returns the value at a specific index / timepoint
         */
        public double getValueAt(int index)
        {
            return timeseriesvalues[index].temporalproperty._value;
        } // getValueAt

        /* Returns a list of timeseries values, used for discretization bins definition procedure
         */
        public double[] getTimeSeriesValuesArray()
        {
            double[] ans = new double[timeseriesvalues.Count];
            for (int i = 0; i < ans.Length; i++)
                ans[i] = getValueAt(i);

            return ans;
        } // getTimeSeriesValuesArray


    } // Class TimeSeries

    // Describes a bin (discretization parameter)
    public class Bin
    {
        public long _ID;
        // description
        public string _label;
        // lower limit ( >= )
        public double _lowlimit;
        // higher limit  ( < )
        public double _highlimit;
        // Error1
        //public double _error1;


        public double mean;
        public double variance;
        public int size;

        public static double MinValue = -1000000;
        public static double MaxValue = 1000000;

        public Bin(long ID, double lowlimit)
        {
            _ID = ID;
            _label = ID.ToString();
            _lowlimit = lowlimit;
            mean = 0;
        } // Bin  constructor


        /// <summary>
        /// Bin constructor
        /// </summary>
        /// <param name="ID">bin's id</param>
        /// <param name="label">bin's label</param>
        /// <param name="lowlimit">lower limit of bin</param>
        /// <param name="highlimit">higher limit of bin</param>
        public Bin(long ID, string label, double lowlimit, double highlimit)
        {
            _ID = ID;
            _label = label;
            _lowlimit = lowlimit;
            _highlimit = highlimit;
        } // Bin  constructor




        /// <summary>
        /// returns a list of bins labels by given number of bins
        /// for example: NumOfBins = 3 -> returns {"Low", "Medium" , "High"}
        /// </summary>
        /// <param name="NumOfBins"></param>
        /// <returns></returns>
        public static string[] getBinsLabels(int NumOfBins, string[] BaseLabels)
        {
            int i = 2;
            string[] ans = new string[NumOfBins];
            if (NumOfBins < 2) return new string[] { "All" };
            ans[0] = BaseLabels[0];
            if (NumOfBins == 3)
                ans[1] = BaseLabels[1];
            else
                for (i = 1; i < NumOfBins - 1; i++)
                    ans[i] = BaseLabels[1] + i.ToString();
            ans[i] = BaseLabels[2];
            return ans;
        } // getBinsLabels

    } // Class Bin

    class None : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;

        #region DiscretizationMethod Members

        public long getID()
        {
            return _ID;
        }

        public void setID(long ID)
        {
            _ID = ID;
        }

        public Parameters getParameters()
        {
            return parameters;
        }

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        }

        public List<Bin> CalculateBins(double[] list)
        {
            List<Bin> bins = new List<Bin>();
            bins.Add(new Bin(1, "Original Values", Bin.MinValue, Bin.MaxValue));
            return bins;
        } // CalculateBins


        /* this method should'nt be used !!!!
         * the intervaling method should get the original values as discretized values
         */
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return new int[0];
        }

        /* this method should'nt be used !!!!
         * the intervaling method should get the original values as discretized values
         */
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            return new int[0];
        }

        #endregion
    }


    class BinsFile : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;

        DataTable tblBins;
        public int curProperty;
        #region DiscretizationMethod Members

        public BinsFile(string filename)
        {
            parameters = new Parameters();
/* *///            tblBins = CSVExporter.loadCSVtoDataSet(filename).Tables[0];
        }


        public long getID()
        {
            return _ID;
        }

        public void setID(long ID)
        {
            _ID = ID;
        }

        public Parameters getParameters()
        {
            return parameters;
        }

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        }

        public List<Bin> CalculateBins(double[] list)
        {

            curProperty = (int)parameters.getParam(Const.STR_CURRENT_PROPERTY);

            // get bins definitions for current property - only for discretization - without tendency
            string select = Const.STR_TEMPORAL_PROPERTY_ID + "='" + curProperty.ToString() + "'";
            DataRow[] rows = tblBins.Select(select, Const.STR_STATE_ID);

            List<Bin> bins = new List<Bin>();
            for (int j = 0; j < rows.Length; j++)
            {
                bins.Add(new Bin(int.Parse(rows[j][Const.STR_BIN_ID].ToString()),
                                 rows[j][Const.STR_BIN_LABEL].ToString(),
                                 double.Parse(rows[j][Const.STR_BIN_FROM].ToString()),
                                 double.Parse(rows[j][Const.STR_BIN_TO].ToString())));
            }

            return bins;
        } // CalculateBins


        /* this method should'nt be used !!!!
         * the intervaling method should get the original values as discretized values
         */
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return new int[0];
        }

        /* this method should'nt be used !!!!
         * the intervaling method should get the original values as discretized values
         */
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            return new int[0];
        }

        #endregion
    }


    class UserDefined : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        private List<Bin> bins;

        private List<Bin> getBinsFromParameters()
        {
            List<Bin> ans = new List<Bin>();
            int numofbins = 0;
            int i;

            if (parameters.getParam(Const.STR_NUM_OF_BINS) != Parameters.NotFound)
                numofbins = (int)parameters.getParam(Const.STR_NUM_OF_BINS);

            for (i = 1; i <= numofbins; i++)
            {
                ans.Add(new Bin(long.Parse(parameters.getParam("Bin" + i.ToString() + "ID").ToString()),
                                parameters.getParam("Bin" + i.ToString() + "Label").ToString(),
                                double.Parse(parameters.getParam("Bin" + i.ToString() + "From").ToString()),
                                double.Parse(parameters.getParam("Bin" + i.ToString() + "To").ToString())));
            } // for

            return ans;

        } // getBinsFromParameters

        #region DiscretizationMethod Members

        public long getID()
        {
            return _ID;
        }

        public void setID(long ID)
        {
            _ID = ID;
        }

        public Parameters getParameters()
        {
            return parameters;
        }

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        }


        public List<Bin> CalculateBins(double[] list)
        {
            if (bins == null) // new object, get bins from parameters
                bins = getBinsFromParameters();
            return bins;
        }

        /// <summary>
        /// Discretize data by user defined bins
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData

        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        }

        #endregion
    } // class UserDefined



    /// <summary>
    /// Tendency discretization : states the tendency of data (steady, go up, go down)
    /// </summary>
    class Tendency : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;

        public Tendency()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters


        /// <summary>
        /// discretize a time point value using the next time point value (tendency)
        /// </summary>
        /// <param name="curvalue">value to discretize</param>
        /// <param name="nextvalue">next value in series to define tendency</param>
        /// <returns>1 for descent, 2 for steady, 3 for ascent</returns>
        public static int discretizeData(double curvalue, double nextvalue)
        {
            // going up
            if (nextvalue > curvalue) return 3;
            // going down
            else if (nextvalue < curvalue) return 1;
            // steady (unchanged)
            else return 2;
        } // discretizeData

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            double avg;
            int pl, pr, i;
            double[] sdata = new double[data.Length];
            int[] res = new int[data.Length];
            // set default parameters
            int k = 4;

            // get parameters
            if (parameters.getParam(Const.STR_TENDENCY_K) != Parameters.NotFound)
                k = int.Parse(parameters.getParam(Const.STR_TENDENCY_K).ToString());

            // smooth list
            for (i = 0; i < data.Length; i++)
            {
                pl = i - k;
                if (pl < 0) pl = 0;
                pr = i + k + 1;
                if (pr > data.Length - 1) pr = data.Length;
                // localize values
                avg = (pr - pl != 0) ? Methods.getListSum(data, pl, pr) / (pr - pl) : 0;
                sdata[i] = avg;
            } // for smooth


            // to get velocity of change:
            double[] velocities = new double[data.Length - 1];
            // velocity is defined as the difference between two adjcent time points' values
            //for (i = 0; i < velocities.Length; i++)
            //    velocities[i] = sdata[i + 1] - sdata[i];

            // to define intervals average velocity
            // return should be velocities, and this is the discretization.
            // positive value is ascent, negative is descent and 0 is unchanged.
            // each interval of ascent, descent or unchanged values can be defined 
            // by its average velocity.

            // discretize
            for (i = 0; i < sdata.Length - 1; i++)
            {
                res[i] = discretizeData(sdata[i], sdata[i + 1]);
            } // for


            // Add last value
            res[res.Length - 1] = 2;
            return res;

        } // DiscretizeData


        public List<Bin> CalculateBins(double[] list)
        {
            List<Bin> bins = new List<Bin>();
            bins.Add(new Bin(1, "descent", Bin.MinValue, 0));
            bins.Add(new Bin(2, "unchanged", 0, 0));
            bins.Add(new Bin(3, "ascent", 0, Bin.MaxValue));
            return bins;
        } // CalculateBins

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            if (data.Length < 2) return null;
            int[] res = new int[data.Length - 1];
            for (int i = 0; i < data.Length - 1; i++)
                res[i] = discretizeData(data[i], data[i + 1]);
            return res;
        } // getDiscretizedValue


    } // class Tendency

    /* Persist Discretization Method
     */
    class Persist : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public Persist()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ

        /// <summary>
        /// Calculates persistence of data by given cuts
        /// </summary>
        /// <param name="data">data to be examined</param>
        /// <param name="cuts">cuts used for discretization</param>
        /// <returns>The persistence score of the given data discretized</returns>
        public double getPersistence(double[] data, double[] cuts, int[] forbiddenList)
        {
            int i;
            // number of states
            int k = cuts.Length + 1;
            // number of time points
            int n = data.Length;
            // result
            int sCount;
            double pscore;
            // discretize data by cuts
            int[] DiscretizedData = Methods.DiscretizeData(data, cuts);
            // get markov chain diagonal ( the probability to remain in the same
            // state for each state
            double[] diagonal = Methods.MCML(DiscretizedData, k, forbiddenList);
            double[] p = new double[2];
            double[] q = new double[2];

            /*
            // make sure there are no zeros or ones
            // eps is given in class
            for (i = 0; i < k; i++)
                if (diagonal[i] == 1) diagonal[i] = 1.0 - eps;
            for (i = 0; i < k; i++)
                if (diagonal[i] == 0) diagonal[i] = eps;
            */

            // calculate start probabilities
            double[] prob = new double[k];
            for (i = 0; i < k; i++)
                prob[i] = 0;
            for (i = 0; i < n; i++)
                prob[DiscretizedData[i] - 1]++;
            for (i = 0; i < k; i++)
                prob[i] = (n != 0) ? prob[i] / n : 0;

            // make sure there are no zeros or ones
            // eps is given in class
            /*            for (i = 0; i < k; i++)
                            if (prob[i] == 1) prob[i] = 1.0 - Const.EPS;
                        for (i = 0; i < k; i++)
                            if (prob[i] == 0) prob[i] = Const.EPS;
              */
            sCount = 0;
            pscore = 0;
            for (i = 0; i < k; i++)
            {
                if (prob[i] != 0)
                {
                    // prepare transition probabilities for current state
                    // self vs. non self
                    p[0] = diagonal[i] > 0 ? diagonal[i] : Const.EPS;
                    p[1] = diagonal[i] > 0 ? 1 - diagonal[i] : 1 - Const.EPS;
                    // prepare start probabilities for current state
                    q[0] = prob[i];// > 0 ? prob[i] : Const.EPS;
                    q[1] = 1 - prob[i]; // > 0 ? 1 - prob[i] : 1 - Const.EPS;

                    // add to or substract from score 
                    if (checkPQ(p, q))
                    {
                        sCount++;
                        pscore += Math.Sign(diagonal[i] - prob[i]) * Methods.getSKLDiv(p, q);
                    } // if legal
                } // if state is used
            } // for calculate score

            // return mean
            if (sCount != 0)
                return pscore / ((double)(sCount));
            else
                return 0;
        } // getPersistence


        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] list)
        {
            //input 
            double[] Serie = list;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> PersistenceScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 4;
            int SkipHigh = 4;
            int Percentiles = 99;
            int[] ForbiddenList = new int[0];

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam("ForbiddenTransitions") != Parameters.NotFound)
                ForbiddenList = ((int[])parameters.getParam("ForbiddenTransitions"));

            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for            
            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;

            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            CandidateCuts = Methods.getListPrctile(Methods.sortList(Serie), range);


            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    PersistenceScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        PersistenceScores.Add(getPersistence(Serie, cuts, ForbiddenList));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = PersistenceScores[0];
                    for (i = 0; i < PersistenceScores.Count; i++)
                    {
                        if (PersistenceScores[i] > BestScore)
                        {
                            BestScore = PersistenceScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;

        } // CalculateBins -  Persist

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue
    } // Class Persist

    public class CandidateInfo
    {

        public List<double> CandidateCuts;
        // used for current tested cut (joined with found cuts
        public List<double> CandidateTemp;

        // used when finding new cuts with filtered Candidates list
        public List<double> cuts;
        public List<CandidateInfo> NextLevelCandidates;
        public List<CandidateCell> cellinfo;


        public CandidateInfo()
        {
        }

    }

    public class CandidateCell : IComparable
    {
        public double cut;
        public double score;
        public double[] var;
        public int index;

        public CandidateCell(double c, double s, int i, double[] v)
        {
            cut = c;
            score = s;
            index = i;
            var = v;
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (this.score == ((CandidateCell)obj).score)
                return 0;
            return this.score - ((CandidateCell)obj).score < 0 ? 1 : -1;
        }

        #endregion
    }



    public class TD4CLA_Ent : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);



        bool ScanRange;
        int TopK;
        int NumOfBins;
        int MinBins;
        int MaxBins;
        int SkipLow;
        int SkipHigh;
        int Percentiles;
        Dictionary<string, string> dicCutoffs;


        public TD4CLA_Ent()
        {
            parameters = new Parameters();
            parameters.Add (Const.STR_CURRENT_PROPERTY, 0);
            parameters.Add ("ResultsFolder", Path.GetDirectoryName(Application.ExecutablePath));

        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static object[] getEntropyMeasure(double[][] databyclass, double[][][] databyclassuser, double[] cuts)
        {

            int i, j, k;
            object[] results = new object[2];
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[] entropybyclass = new double[databyclass.Length];
            double[] cosimvar = new double[databyclass.Length];
            int[] discretizeddatauser;
            double[] counts;
            double[] totalcounts = new double[levels.Length];
            double[][] userprob;
            double sumuser, sumclass;


            for (i = 0; i < databyclass.Length; i++)
            {
                sumclass = 0;
                for (k = 0; k < levels.Length; k++)
                    totalcounts[k] = 0;
                
                //discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                userprob = new double[databyclassuser[i].Length][];

                for (j = 0; j < databyclassuser[i].Length; j++)
                {
                    sumuser = 0;
                    discretizeddatauser = Methods.DiscretizeData(databyclassuser[i][j], cuts);
                    counts = Statistics.Measures.getLevelsCounts(levels, discretizeddatauser);
                    sumuser = Methods.getListSum(counts, 0, counts.Length);
                    for (k = 0; k < levels.Length; k++)
                    {
                        totalcounts[k] += counts[k];
                        counts[k] = (sumuser != 0) ? counts[k] / sumuser : 0;
                    }
                    userprob[j] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddatauser);
                }
                
                sumclass = Methods.getListSum(totalcounts, 0, totalcounts.Length);
                for (k = 0; k < totalcounts.Length; k++) totalcounts[k] /= sumclass;
 
                entropybyclass[i] = Statistics.Measures.getEntropy(totalcounts, databyclass.Length);
                cosimvar[i] = Statistics.Measures.getSumOfCosineSimilarities(userprob);
            }

            results[0] = Statistics.Measures.getSumOfDistances(entropybyclass);
            results[1] = cosimvar;
            return results;
        }




        public CandidateInfo recCalculateCuts(CandidateInfo info, StreamWriter sw, double[][] values, double[][][] valuesbyuser, int cuts, int k)
        {

            int i, j;

            // end
            if (cuts == 0)
            {
                if (info.cuts.Count > 0)
                {
                    sw.Write(info.cuts.Count.ToString() + ",");
                    for (i = 0; i < info.cuts.Count; i++)
                    {
                        sw.Write(info.cellinfo[i].cut.ToString() + ",");
                        sw.Write(info.cellinfo[i].score.ToString() + ",");
                        for (j = 0; j < info.cellinfo[i].var.Length; j++)
                            sw.Write(info.cellinfo[i].var[j].ToString() + ",");

                    }
                    sw.WriteLine("TOTAL");
                }
                return info;
                
            }
            else
            {
                List<CandidateCell> lstCells = new List<CandidateCell>();
                List<double> tempcuts;
                object[] results;
                string res, temp;
                string[] vals;
                double[] vars;
                // check all candidates, select top k
                for (i = 0; i < info.CandidateCuts.Count; i++)
                {
                    tempcuts = info.cuts.GetRange(0, info.cuts.Count);
                    tempcuts.Add(info.CandidateCuts[i]);
                    tempcuts.Sort();
                    
                    temp = Methods.getArrayString<double>(tempcuts, "-");

                    if (dicCutoffs.TryGetValue(temp, out res))
                    {
                        vals = res.Split('-');
                        vars = new double[vals.Length - 1];
                        for (j = 1; j < vals.Length; j++)
                            vars[j - 1] = double.Parse(vals[j]);
                        lstCells.Add(new CandidateCell(info.CandidateCuts[i], double.Parse(vals[0]), i, vars));

                    }
                    else
                    {
                        results = getEntropyMeasure(values, valuesbyuser, tempcuts.ToArray());
                        res = results[0].ToString() + "-" + Methods.getArrayString<double>((double[])results[1], "-");
                        dicCutoffs.Add(temp, res);
                        lstCells.Add(new CandidateCell(info.CandidateCuts[i], (double)results[0], i, (double[])results[1]));
                    }
                } // for candidates

                // sort results
                lstCells.Sort();


                // Take Top K
                int count = k;
                info.NextLevelCandidates = new List<CandidateInfo>();
                CandidateInfo infotemp;
                int ind1, ind2;
                List<double> tmpCuts  = new List<double>();
                // now run for each and get the result
                for (i = 0; count > 0 && i < lstCells.Count; i++)
                {

                    if (tmpCuts.IndexOf(lstCells[i].cut) == -1)
                    {
                        count--;
                        tmpCuts.Add(lstCells[i].cut);

                        List<double> newcuts = info.CandidateCuts.GetRange(0, info.CandidateCuts.Count);
                        // create new candidates list, without current cut (and surrounding radius 4 cuts)
                        ind1 = Math.Max(0, lstCells[i].index - SkipLow);
                        ind2 = Math.Min(newcuts.Count, lstCells[i].index + SkipHigh + 1);
                        for (j = ind1; j < ind2; j++)
                            newcuts.RemoveAt(ind1);

                        // create new candidate info
                        infotemp = new CandidateInfo();
                        infotemp.cuts = info.cuts.GetRange(0, info.cuts.Count);
                        infotemp.cuts.Add(lstCells[i].cut);
                        infotemp.cellinfo = info.cellinfo.GetRange(0, info.cellinfo.Count);
                        infotemp.cellinfo.Add(lstCells[i]);
                        infotemp.CandidateCuts = newcuts;

                        info.NextLevelCandidates.Add(infotemp);
                    }
                } // for top k cuts

                //****** update results file *********
                // write current cuts:
                /*
                if (info.cuts.Count > 0)
                {
                    sw.Write(info.cuts.Count.ToString() + ",");
                    for (i = 0; i < info.cuts.Count; i++)
                        sw.Write(info.cuts[i].ToString() + ",");
                    sw.WriteLine( info.cellinfo.score.ToString());

                }
                */
                List<CandidateInfo> lstResults = new List<CandidateInfo>();

                for (i = 0; i < info.NextLevelCandidates.Count; i++)
                {
                    lstResults.Add(recCalculateCuts(info.NextLevelCandidates[i], sw, values,valuesbyuser , cuts - 1, k));
                }

                // now select best cuts
                int maxindex = 0;
                double maxscore = lstResults[0].cellinfo[lstResults[0].cellinfo.Count - 1].score;
                for (i = 1; i< lstResults.Count; i++)
                    if (maxscore < lstResults[i].cellinfo[lstResults[i].cellinfo.Count - 1].score)
                    {
                        maxindex = i;
                        maxscore = lstResults[i].cellinfo[lstResults[i].cellinfo.Count - 1].score;
                    }

                return lstResults[maxindex];

            } // if more cuts to be selected


        } // recCalculateCuts



        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            // used for percentiles
            double[] range;
            //double prod;
            List<Bin> Bins;

            // loop variables
            int i, j;

            // set default parameters values
            ScanRange = false;
            TopK = 10;
            NumOfBins = 3;
            MinBins = 3;
            MaxBins = 7;
            SkipLow = 0;
            SkipHigh = 0;
            Percentiles = 99;
            double[][] databyclass = new double[1][];
            double[][][] databyclassuser = new double[1][][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam("TopK") != Parameters.NotFound)
                TopK = int.Parse(parameters.getParam("TopK").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));
            if (parameters.getParam(Const.STR_DATA_BY_CLASS_USER) != Parameters.NotFound)
                databyclassuser = (double[][][])(parameters.getParam(Const.STR_DATA_BY_CLASS_USER));

            CandidateInfo Candidates = new CandidateInfo();
            Candidates.cuts = new List<double>();
            Candidates.cellinfo = new List<CandidateCell>();

            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            Candidates.CandidateCuts = new List<double>();
            // Available candidate cuts (marked as 1)
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            range = new double[Percentiles];
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = (Percentiles + 1 != 0) ? ((double)i) / (double)(Percentiles + 1) : 0;
                // cut list's ends - List of indexes of available cuts
            } // for            

            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            
            for (i = SkipLow; i < candidates.Length - SkipHigh ; i++)
                Candidates.CandidateCuts.Add(candidates[i]);
            
            CandidateInfo bestCandidate;

            string folder = (string)parameters.getParam("ResultsFolder");

            int curProperty = (int)parameters.getParam(Const.STR_CURRENT_PROPERTY);

            string resfilename = folder + "\\ResultsTD4CLA_Ent-" + curProperty.ToString() + ".csv";


            dicCutoffs = new Dictionary<string, string>();

            using (StreamWriter sw = new StreamWriter(resfilename));

            using (StreamWriter sw = new StreamWriter(resfilename, true))
            {
                sw.Write("CutsNumber,");
                for (i = 0 ; i < NumOfBins - 1; i++)
                {
                    sw.Write("Cut" + i.ToString() + ",Cut" + i.ToString() + "Score");
                    for (j = 0; j < databyclass.Length; j++)
                        sw.Write(",Cut" + i.ToString() + "Class" + j.ToString() + "CosSim");
                    if (i != NumOfBins - 2)
                        sw.Write(",");
                }
                sw.WriteLine("");
                bestCandidate = recCalculateCuts(Candidates, sw, databyclass,databyclassuser, NumOfBins - 1, TopK);
            }

            bestCandidate.cuts.Sort();
            Bins = Methods.CreateBins( bestCandidate.cuts.ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4CLA_Ent



    public class TD4CLA_Cos : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);



        bool ScanRange;
        int TopK;
        int NumOfBins;
        int MinBins;
        int MaxBins;
        int SkipLow;
        int SkipHigh;
        int Percentiles;
        Dictionary<string, string> dicCutoffs;

        public TD4CLA_Cos()
        {
            parameters = new Parameters();
            parameters.Add (Const.STR_CURRENT_PROPERTY, 0);
            parameters.Add ("ResultsFolder", Path.GetDirectoryName(Application.ExecutablePath));

        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        
        public static object[] getCosineMeasure(double[][] databyclass, double[][][] databyclassuser, double[] cuts)
        {

            int i, j, k;
            object[] results = new object[2];
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[] entropybyclass = new double[databyclass.Length];
            double[] cosimvar = new double[databyclass.Length];
            int[] discretizeddatauser;
            double[] counts;
            double[] totalcounts = new double[levels.Length];
            double[][] userprob;
            double[][] classprob = new double[databyclass.Length][];
            double sumuser, sumclass;


            for (i = 0; i < databyclass.Length; i++)
            {
                sumclass = 0;
                for (k = 0; k < levels.Length; k++)
                    totalcounts[k] = 0;

                //discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                userprob = new double[databyclassuser[i].Length][];
                classprob[i] = new double[levels.Length];

                for (j = 0; j < databyclassuser[i].Length; j++)
                {
                    sumuser = 0;
                    discretizeddatauser = Methods.DiscretizeData(databyclassuser[i][j], cuts);
                    counts = Statistics.Measures.getLevelsCounts(levels, discretizeddatauser);
                    sumuser = Methods.getListSum(counts, 0, counts.Length);
                    for (k = 0; k < levels.Length; k++)
                    {
                        totalcounts[k] += counts[k];
                        counts[k] = (sumuser != 0) ? counts[k] / sumuser : 0;
                    }
                    userprob[j] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddatauser);
                }

                sumclass = Methods.getListSum(totalcounts, 0, totalcounts.Length);
                for (k = 0; k < totalcounts.Length; k++) totalcounts[k] /= sumclass;

                totalcounts.CopyTo(classprob[i], 0);
                cosimvar[i] = Statistics.Measures.getSumOfCosineSimilarities(userprob);
            }

            results[0] = Statistics.Measures.getSumOfCosineSimilarities(classprob);
            results[1] = cosimvar;
            return results;
        }

        
        public CandidateInfo recCalculateCuts(CandidateInfo info, StreamWriter sw, double[][] values, double[][][] valuesbyuser, int cuts, int k)
        {

            int i, j;

            // end
            if (cuts == 0)
            {
                if (info.cuts.Count > 0)
                {
                    sw.Write(info.cuts.Count.ToString() + ",");
                    for (i = 0; i < info.cuts.Count; i++)
                    {
                        sw.Write(info.cellinfo[i].cut.ToString() + ",");
                        sw.Write(info.cellinfo[i].score.ToString() + ",");
                        for (j = 0; j < info.cellinfo[i].var.Length; j++)
                            sw.Write(info.cellinfo[i].var[j].ToString() + ",");

                    }
                    sw.WriteLine("TOTAL");
                }
                return info;

            }
            else
            {
                List<CandidateCell> lstCells = new List<CandidateCell>();
                List<double> tempcuts;
                object[] results;


                string res, temp;
                string[] vals;
                double[] vars;
                // check all candidates, select top k
                for (i = 0; i < info.CandidateCuts.Count; i++)
                {
                    tempcuts = info.cuts.GetRange(0, info.cuts.Count);
                    tempcuts.Add(info.CandidateCuts[i]);
                    tempcuts.Sort();

                    temp = Methods.getArrayString<double>(tempcuts, "-");

                    if (dicCutoffs.TryGetValue(temp, out res))
                    {
                        vals = res.Split('-');
                        vars = new double[vals.Length - 1];
                        for (j = 1; j < vals.Length; j++)
                            vars[j - 1] = double.Parse(vals[j]);
                        lstCells.Add(new CandidateCell(info.CandidateCuts[i], double.Parse(vals[0]), i, vars));

                    }
                    else
                    {
                        results = getCosineMeasure(values, valuesbyuser, tempcuts.ToArray());
                        res = results[0].ToString() + "-" + Methods.getArrayString<double>((double[])results[1], "-");
                        dicCutoffs.Add(temp, res);
                        lstCells.Add(new CandidateCell(info.CandidateCuts[i], (double)results[0], i, (double[])results[1]));
                    }
                } // for candidates

                // sort results
                lstCells.Sort();


                // Take Top K
                // Take Top K
                int count = k;
                info.NextLevelCandidates = new List<CandidateInfo>();
                CandidateInfo infotemp;
                int ind1, ind2;
                List<double> tmpCuts  = new List<double>();
                // now run for each and get the result
                for (i = 0; count > 0 && i < lstCells.Count; i++)
                {

                    if (tmpCuts.IndexOf(lstCells[i].cut) == -1)
                    {
                        count--;
                        tmpCuts.Add(lstCells[i].cut);
                        List<double> newcuts = info.CandidateCuts.GetRange(0, info.CandidateCuts.Count);
                        // create new candidates list, without current cut (and surrounding radius 4 cuts)
                        ind1 = Math.Max(0, lstCells[i].index - SkipLow);
                        ind2 = Math.Min(newcuts.Count, lstCells[i].index + SkipHigh + 1);
                        for (j = ind1; j < ind2; j++)
                            newcuts.RemoveAt(ind1);

                        // create new candidate info
                        infotemp = new CandidateInfo();
                        infotemp.cuts = info.cuts.GetRange(0, info.cuts.Count);
                        infotemp.cuts.Add(lstCells[i].cut);
                        infotemp.cellinfo = info.cellinfo.GetRange(0, info.cellinfo.Count);
                        infotemp.cellinfo.Add(lstCells[i]);
                        infotemp.CandidateCuts = newcuts;

                        info.NextLevelCandidates.Add(infotemp);
                    } // if distinct
                } // for top k cuts

                //****** update results file *********
                // write current cuts:
                /*
                if (info.cuts.Count > 0)
                {
                    sw.Write(info.cuts.Count.ToString() + ",");
                    for (i = 0; i < info.cuts.Count; i++)
                        sw.Write(info.cuts[i].ToString() + ",");
                    sw.WriteLine( info.cellinfo.score.ToString());

                }
                */
                List<CandidateInfo> lstResults = new List<CandidateInfo>();

                for (i = 0; i < info.NextLevelCandidates.Count; i++)
                {
                    lstResults.Add(recCalculateCuts(info.NextLevelCandidates[i], sw, values, valuesbyuser, cuts - 1, k));
                }

                // now select best cuts
                int maxindex = 0;
                double maxscore = lstResults[0].cellinfo[lstResults[0].cellinfo.Count - 1].score;
                for (i = 1; i < lstResults.Count; i++)
                    if (maxscore < lstResults[i].cellinfo[lstResults[i].cellinfo.Count - 1].score)
                    {
                        maxindex = i;
                        maxscore = lstResults[i].cellinfo[lstResults[i].cellinfo.Count - 1].score;
                    }

                return lstResults[maxindex];

            } // if more cuts to be selected


        } // recCalculateCuts



        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            // used for percentiles
            double[] range;
            //double prod;
            List<Bin> Bins;

            // loop variables
            int i, j;

            // set default parameters values
            ScanRange = false;
            TopK = 10;
            NumOfBins = 3;
            MinBins = 3;
            MaxBins = 7;
            SkipLow = 0;
            SkipHigh = 0;
            Percentiles = 99;
            double[][] databyclass = new double[1][];
            double[][][] databyclassuser = new double[1][][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam("TopK") != Parameters.NotFound)
                TopK = int.Parse(parameters.getParam("TopK").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));
            if (parameters.getParam(Const.STR_DATA_BY_CLASS_USER) != Parameters.NotFound)
                databyclassuser = (double[][][])(parameters.getParam(Const.STR_DATA_BY_CLASS_USER));

            CandidateInfo Candidates = new CandidateInfo();
            Candidates.cuts = new List<double>();
            Candidates.cellinfo = new List<CandidateCell>();

            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            Candidates.CandidateCuts = new List<double>();
            // Available candidate cuts (marked as 1)
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            range = new double[Percentiles];
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = (Percentiles + 1 != 0) ? ((double)i) / (double)(Percentiles + 1) : 0;
                // cut list's ends - List of indexes of available cuts
            } // for            

            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            for (i = SkipLow; i < candidates.Length - SkipHigh; i++)
                Candidates.CandidateCuts.Add(candidates[i]);

            CandidateInfo bestCandidate;

            string folder = (string)parameters.getParam("ResultsFolder");

            int curProperty = (int)parameters.getParam(Const.STR_CURRENT_PROPERTY);

            string resfilename = folder + "\\ResultsTD4CLA_Cos-" + curProperty.ToString() + ".csv";

            dicCutoffs = new Dictionary<string, string>();

            using (StreamWriter sw = new StreamWriter(resfilename));

            using (StreamWriter sw = new StreamWriter(resfilename, true))
            {
                sw.Write("CutsNumber,");
                for (i = 0; i < NumOfBins - 1; i++)
                {
                    sw.Write("Cut" + i.ToString() + ",Cut" + i.ToString() + "Score");
                    for (j = 0; j < databyclass.Length; j++)
                        sw.Write(",Cut" + i.ToString() + "Class" + j.ToString() + "CosSim");
                    if (i != NumOfBins - 2)
                        sw.Write(",");
                }
                sw.WriteLine("");
                bestCandidate = recCalculateCuts(Candidates, sw, databyclass, databyclassuser, NumOfBins - 1, TopK);
            }
            
            bestCandidate.cuts.Sort();
            Bins = Methods.CreateBins(bestCandidate.cuts.ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4CLA_Cos

    public class TD4CLA_KL : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);



        bool ScanRange;
        int TopK;
        int NumOfBins;
        int MinBins;
        int MaxBins;
        int SkipLow;
        int SkipHigh;
        int Percentiles;
        Dictionary<string, string> dicCutoffs;

        public TD4CLA_KL()
        {
            parameters = new Parameters();
            parameters.Add (Const.STR_CURRENT_PROPERTY, 0);
            parameters.Add ("ResultsFolder", Path.GetDirectoryName(Application.ExecutablePath));

        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ



        public static object[] getKLMeasure(double[][] databyclass, double[][][] databyclassuser, double[] cuts)
        {

            int i, j, k;
            object[] results = new object[2];
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[] entropybyclass = new double[databyclass.Length];
            double[] cosimvar = new double[databyclass.Length];
            int[] discretizeddatauser;
            double[] counts;
            double[] totalcounts = new double[levels.Length];
            double[][] userprob;
            double[][] classprob = new double[databyclass.Length][];
            double sumuser, sumclass;


            for (i = 0; i < databyclass.Length; i++)
            {
                sumclass = 0;
                for (k = 0; k < levels.Length; k++)
                    totalcounts[k] = 0;

                //discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                userprob = new double[databyclassuser[i].Length][];

                classprob[i] = new double[levels.Length];

                for (j = 0; j < databyclassuser[i].Length; j++)
                {
                    sumuser = 0;
                    discretizeddatauser = Methods.DiscretizeData(databyclassuser[i][j], cuts);
                    counts = Statistics.Measures.getLevelsCounts(levels, discretizeddatauser);
                    sumuser = Methods.getListSum(counts, 0, counts.Length);
                    for (k = 0; k < levels.Length; k++)
                    {
                        totalcounts[k] += counts[k];
                        counts[k] = (sumuser != 0 ) ? (counts[k] / sumuser) : 0;
                    }
                    userprob[j] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddatauser);
                }

                sumclass = Methods.getListSum(totalcounts, 0, totalcounts.Length);
                for (k = 0; k < totalcounts.Length; k++) totalcounts[k] /= sumclass;

                totalcounts.CopyTo(classprob[i], 0);
                cosimvar[i] = Statistics.Measures.getSumOfCosineSimilarities(userprob);
            }

            results[0] = Statistics.Measures.getSumOfKLDivergences(classprob);
            results[1] = cosimvar;
            return results;
        }


        public CandidateInfo recCalculateCuts(CandidateInfo info, StreamWriter sw, double[][] values, double[][][] valuesbyuser, int cuts, int k)
        {

            int i, j;

            // end
            if (cuts == 0)
            {
                if (info.cuts.Count > 0)
                {
                    sw.Write(info.cuts.Count.ToString() + ",");
                    for (i = 0; i < info.cuts.Count; i++)
                    {
                        sw.Write(info.cellinfo[i].cut.ToString() + ",");
                        sw.Write(info.cellinfo[i].score.ToString() + ",");
                        for (j = 0; j < info.cellinfo[i].var.Length; j++)
                            sw.Write(info.cellinfo[i].var[j].ToString() + ",");

                    }
                    sw.WriteLine("TOTAL");
                }
                return info;

            }
            else
            {
                List<CandidateCell> lstCells = new List<CandidateCell>();
                List<double> tempcuts;
                object[] results;

                string res, temp;
                string[] vals;
                double[] vars;
                // check all candidates, select top k
                for (i = 0; i < info.CandidateCuts.Count; i++)
                {
                    tempcuts = info.cuts.GetRange(0, info.cuts.Count);
                    tempcuts.Add(info.CandidateCuts[i]);
                    tempcuts.Sort();

                    temp = Methods.getArrayString<double>(tempcuts, "-");

                    if (dicCutoffs.TryGetValue(temp, out res))
                    {
                        vals = res.Split('-');
                        vars = new double[vals.Length - 1];
                        for (j = 1; j < vals.Length; j++)
                            vars[j - 1] = double.Parse(vals[j]);
                        lstCells.Add(new CandidateCell(info.CandidateCuts[i], double.Parse(vals[0]), i, vars));

                    }
                    else
                    {
                        results = getKLMeasure(values, valuesbyuser, tempcuts.ToArray());
                        res = results[0].ToString() + "-" + Methods.getArrayString<double>((double[])results[1], "-");
                        dicCutoffs.Add(temp, res);
                        lstCells.Add(new CandidateCell(info.CandidateCuts[i], (double)results[0], i, (double[])results[1]));
                    }
                    
                } // for candidates
                

                // sort results
                lstCells.Sort();


                // Take Top K
                // Take Top K
                int count = k;
                info.NextLevelCandidates = new List<CandidateInfo>();
                CandidateInfo infotemp;
                int ind1, ind2;
                List<double> tmpCuts  = new List<double>();
                // now run for each and get the result
                for (i = 0; count > 0 && i < lstCells.Count; i++)
                {

                    if (tmpCuts.IndexOf(lstCells[i].cut) == -1)
                    {
                        count--;
                        tmpCuts.Add(lstCells[i].cut);
                        List<double> newcuts = info.CandidateCuts.GetRange(0, info.CandidateCuts.Count);
                        // create new candidates list, without current cut (and surrounding radius 4 cuts)
                        ind1 = Math.Max(0, lstCells[i].index - SkipLow);
                        ind2 = Math.Min(newcuts.Count, lstCells[i].index + SkipHigh + 1);
                        for (j = ind1; j < ind2; j++)
                            newcuts.RemoveAt(ind1);

                        // create new candidate info
                        infotemp = new CandidateInfo();
                        infotemp.cuts = info.cuts.GetRange(0, info.cuts.Count);
                        infotemp.cuts.Add(lstCells[i].cut);
                        infotemp.cellinfo = info.cellinfo.GetRange(0, info.cellinfo.Count);
                        infotemp.cellinfo.Add(lstCells[i]);
                        infotemp.CandidateCuts = newcuts;

                        info.NextLevelCandidates.Add(infotemp);
                    } // if distinct
                } // for top k cuts

                //****** update results file *********
                // write current cuts:
                /*
                if (info.cuts.Count > 0)
                {
                    sw.Write(info.cuts.Count.ToString() + ",");
                    for (i = 0; i < info.cuts.Count; i++)
                        sw.Write(info.cuts[i].ToString() + ",");
                    sw.WriteLine( info.cellinfo.score.ToString());

                }
                */
                List<CandidateInfo> lstResults = new List<CandidateInfo>();

                for (i = 0; i < info.NextLevelCandidates.Count; i++)
                {
                    lstResults.Add(recCalculateCuts(info.NextLevelCandidates[i], sw, values, valuesbyuser, cuts - 1, k));
                }

                // now select best cuts
                int maxindex = 0;
                double maxscore = lstResults[0].cellinfo[lstResults[0].cellinfo.Count - 1].score;
                for (i = 1; i < lstResults.Count; i++)
                    if (maxscore < lstResults[i].cellinfo[lstResults[i].cellinfo.Count - 1].score)
                    {
                        maxindex = i;
                        maxscore = lstResults[i].cellinfo[lstResults[i].cellinfo.Count - 1].score;
                    }

                return lstResults[maxindex];

            } // if more cuts to be selected


        } // recCalculateCuts



        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            // used for percentiles
            double[] range;
            //double prod;
            List<Bin> Bins;

            // loop variables
            int i, j;

            // set default parameters values
            ScanRange = false;
            TopK = 10;
            NumOfBins = 3;
            MinBins = 3;
            MaxBins = 7;
            SkipLow = 0;
            SkipHigh = 0;
            Percentiles = 99;
            double[][] databyclass = new double[1][];
            double[][][] databyclassuser = new double[1][][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam("TopK") != Parameters.NotFound)
                TopK = int.Parse(parameters.getParam("TopK").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));
            if (parameters.getParam(Const.STR_DATA_BY_CLASS_USER) != Parameters.NotFound)
                databyclassuser = (double[][][])(parameters.getParam(Const.STR_DATA_BY_CLASS_USER));

            CandidateInfo Candidates = new CandidateInfo();
            Candidates.cuts = new List<double>();
            Candidates.cellinfo = new List<CandidateCell>();

            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            Candidates.CandidateCuts = new List<double>();
            // Available candidate cuts (marked as 1)
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            range = new double[Percentiles];
            for (i = 1; i <= range.Length; i++)
            {
                //range[i - 1] = ((double)i) / (double)(Percentiles + 1);
                range[i - 1] = (Percentiles + 1 != 0) ? ((double)i) / (double)(Percentiles + 1) : 0;
                // cut list's ends - List of indexes of available cuts
            } // for            

            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            for (i = SkipLow; i < candidates.Length - SkipHigh; i++)
                Candidates.CandidateCuts.Add(candidates[i]);

            CandidateInfo bestCandidate;

            string folder = (string)parameters.getParam("ResultsFolder");

            int curProperty = (int)parameters.getParam(Const.STR_CURRENT_PROPERTY);

            string resfilename = folder + "\\ResultsTD4CLA_KL-" + curProperty.ToString() + ".csv";

            dicCutoffs = new Dictionary<string, string>();


            using (StreamWriter sw = new StreamWriter(resfilename));

            using (StreamWriter sw = new StreamWriter(resfilename, true))
            {
                sw.Write("CutsNumber,");
                for (i = 0; i < NumOfBins - 1; i++)
                {
                    sw.Write("Cut" + i.ToString() + ",Cut" + i.ToString() + "Score");
                    for (j = 0; j < databyclass.Length; j++)
                        sw.Write(",Cut" + i.ToString() + "Class" + j.ToString() + "CosSim");
                    if (i != NumOfBins - 2)
                        sw.Write(",");
                }
                sw.WriteLine("");
                bestCandidate = recCalculateCuts(Candidates, sw, databyclass, databyclassuser, NumOfBins - 1, TopK);
            }
            
            bestCandidate.cuts.Sort();
            Bins = Methods.CreateBins(bestCandidate.cuts.ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4CLA_KL




    public class TD4C_Ent : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public TD4C_Ent()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static double getEntropyMeasure(double[][] databyclass, double[] cuts)
        {

            int i;
            int[] discretizeddata;
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[] entropybyclass = new double[databyclass.Length];

            for (i = 0; i < databyclass.Length; i++)
            {
                discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                entropybyclass[i] = Statistics.Measures.getEntropy(Statistics.Measures.getLevelsProbabilities(levels, discretizeddata), databyclass.Length);
            }

            return Statistics.Measures.getSumOfDistances(entropybyclass);

        }

        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> EntropyScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 0;
            int SkipHigh = 0;
            int Percentiles = 99;
            double[][] databyclass = new double[1][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));


            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for         


            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] v = Methods.sortList(Serie);
            double[] v1 = Methods.getDistinctList(v);
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(v, range));

            range = candidates;
            CandidateCuts = candidates;

            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;


            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    EntropyScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        EntropyScores.Add(getEntropyMeasure(databyclass, cuts));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = EntropyScores[0];
                    for (i = 0; i < EntropyScores.Count; i++)
                    {
                        if (EntropyScores[i] > BestScore)
                        {
                            BestScore = EntropyScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            Bins[0].mean = BestScore;
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4C_Ent



    public class TD4C_DiffSumMax : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public TD4C_DiffSumMax()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static double getDiffSumMaxMeasure(double[][] databyclass, double[] cuts)
        {

            int i, j;
            int[] discretizeddata;
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[][] probs = new double[databyclass.Length][];

            for (i = 0; i < databyclass.Length; i++)
            {
                discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                probs[i] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddata);
            }


            double[] diffs = new double[probs[0].Length];
            for (i = 0; i < probs[0].Length; i++)
                diffs[i] = probs[1][i] - probs[0][i];

            double max = 0;

            for (i = 0; i < diffs.Length; i++)
                for (j = i + 1; j < diffs.Length ; j++)
                    if (Math.Abs (diffs[i] - diffs[j]) > max)
                        max = Math.Abs (diffs[i] - diffs[j]);

            return max;

        }

        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> EntropyScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 0;
            int SkipHigh = 0;
            int Percentiles = 99;
            double[][] databyclass = new double[1][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));


            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for         


            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            range = candidates;
            CandidateCuts = candidates;

            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;


            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    EntropyScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        EntropyScores.Add(getDiffSumMaxMeasure(databyclass, cuts));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = EntropyScores[0];
                    for (i = 0; i < EntropyScores.Count; i++)
                    {
                        if (EntropyScores[i] > BestScore)
                        {
                            BestScore = EntropyScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            Bins[0].mean = BestScore;
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4C_DiffSumMax





    public class TD4C_DEnt : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public TD4C_DEnt()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static double getDentropyMeasure(double[][] databyclass, double[] cuts)
        {

            int i;
            int[] discretizeddata;
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[] entropybyclass = new double[databyclass.Length];
            double[][] probs = new double[databyclass.Length][];

            for (i = 0; i < databyclass.Length; i++)
            {
                discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                probs[i] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddata);
            }


            double[] diffs = new double [probs[0].Length];
            for (i = 0; i < probs[0].Length; i++)
                diffs[i] = Math.Abs(probs[1][i] - probs[0][i]);

            return Statistics.Measures.getEntropy(diffs, Math.E);
            
        }

        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> EntropyScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 0;
            int SkipHigh = 0;
            int Percentiles = 99;
            double[][] databyclass = new double[1][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));


            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for         


            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            range = candidates;
            CandidateCuts = candidates;

            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;


            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    EntropyScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        EntropyScores.Add(getDentropyMeasure(databyclass, cuts));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = EntropyScores[0];
                    for (i = 0; i < EntropyScores.Count; i++)
                    {
                        if (EntropyScores[i] > BestScore)
                        {
                            BestScore = EntropyScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            Bins[0].mean = BestScore;
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4C_DEnt



    public class TD4C_DiffSum : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public TD4C_DiffSum()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static double getDiffSum(double[][] databyclass, double[] cuts)
        {

            int i;
            int[] discretizeddata;
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[][] probs = new double[databyclass.Length][];

            for (i = 0; i < databyclass.Length; i++)
            {
                discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                probs[i] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddata);
            }

            double diffs = 0;
            for (i = 0; i < probs[0].Length; i++)
                diffs += Math.Abs(probs[1][i] - probs[0][i]);

            return diffs;

        }

        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> EntropyScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 0;
            int SkipHigh = 0;
            int Percentiles = 99;
            double[][] databyclass = new double[1][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));


            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for         


            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            range = candidates;
            CandidateCuts = candidates;

            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;


            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    EntropyScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        EntropyScores.Add(getDiffSum(databyclass, cuts));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = EntropyScores[0];
                    for (i = 0; i < EntropyScores.Count; i++)
                    {
                        if (EntropyScores[i] > BestScore)
                        {
                            BestScore = EntropyScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            Bins[0].mean = BestScore;

            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4C_DiffSum








    public class TD4C_KL : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public TD4C_KL()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static double getKLDMeasure(double[][] databyclass, double[] cuts)
        {

            int i;
            int[] discretizeddata;
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[][] pdfbyclass = new double[databyclass.Length][];

            for (i = 0; i < databyclass.Length; i++)
            {
                discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                pdfbyclass[i] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddata);
            }

            return Statistics.Measures.getSumOfKLDivergences(pdfbyclass);

        }

        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> EntropyScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 0;
            int SkipHigh = 0;
            int Percentiles = 99;
            double[][] databyclass = new double[1][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));


            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for            


            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            range = candidates;
            CandidateCuts = candidates;

            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;

            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    EntropyScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        EntropyScores.Add(getKLDMeasure(databyclass, cuts));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = EntropyScores[0];
                    for (i = 0; i < EntropyScores.Count; i++)
                    {
                        if (EntropyScores[i] > BestScore)
                        {
                            BestScore = EntropyScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            Bins[0].mean = BestScore;
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4C_KL

    public class TD4C_Cos : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public TD4C_Cos()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ


        public static double getDistanceMeasure(double[][] databyclass, double[] cuts)
        {

            int i;
            int[] discretizeddata;
            int[] levels = Methods.getLevelsByCuts(cuts);
            double[][] vectorsbyclass = new double[databyclass.Length][];

            for (i = 0; i < databyclass.Length; i++)
            {
                discretizeddata = Methods.DiscretizeData(databyclass[i], cuts);
                vectorsbyclass[i] = Statistics.Measures.getLevelsProbabilities(levels, discretizeddata);
            }

            return Statistics.Measures.getSumOfCosineSimilarities(vectorsbyclass);

        }

        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] allvalues)
        {
            //input 
            double[] Serie = allvalues;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> EntropyScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 0;
            int SkipHigh = 0;
            int Percentiles = 99;
            double[][] databyclass = new double[1][];
            databyclass[0] = Serie;

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam(Const.STR_DATA_BY_CLASS) != Parameters.NotFound)
                databyclass = (double[][])(parameters.getParam(Const.STR_DATA_BY_CLASS));


            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for            


            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            double[] candidates = Methods.getDistinctList(Methods.getListPrctile(Methods.sortList(Serie), range));

            range = candidates;
            CandidateCuts = candidates;

            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;

            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    EntropyScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        EntropyScores.Add(getDistanceMeasure(databyclass, cuts));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = EntropyScores[0];
                    for (i = 0; i < EntropyScores.Count; i++)
                    {
                        if (EntropyScores[i] > BestScore)
                        {
                            BestScore = EntropyScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            Bins[0].mean = BestScore;
            return Bins;

        } // CalculateBins - TD4C_Ent

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // Class TD4C_Cos



    public class KMeans : DiscretizationMethod
    {

        long _ID;
        public Parameters parameters;

        // arrays for holding the resulted groups' stats
        public double[] SumOfItems;
        public int[] NumOfItems;
        public double[] variances;


        public KMeans()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters



        private void sortKMeansResults(double[] cuts, double[] sumofItems, int[] numofItems, double[] variances)
        {
            //double[] result = new double[data.Length];
            int j, i;
            double cutval, sumval, varval;
            int numval;
            for (i = 1; i < cuts.Length; i++)
            {
                j = i;
                cutval = cuts[i]; sumval = sumofItems[i]; numval = numofItems[i]; varval = variances[i];
                while (j > 0 && cuts[j - 1] > cutval)
                {
                    cuts[j] = cuts[j - 1];
                    sumofItems[j] = sumofItems[j - 1];
                    numofItems[j] = numofItems[j - 1];
                    variances[j] = variances[j - 1];
                    j--;
                }
                cuts[j] = cutval; sumofItems[j] = sumval; numofItems[j] = numval; variances[j] = varval;

            } // for


        }


        /// <summary>
        /// returns the discretized value for the given value, using the given centroids
        /// </summary>
        /// <param name="value">given value</param>
        /// <param name="centroids">list of centroids</param>
        /// <returns></returns>
        public int DiscretizeValue(double value, double[] centroids)
        {
            int ans = 1;
            int i = 0;

            while (i < centroids.Length)
            {
                if (Math.Abs(value - centroids[i]) < Math.Abs(value - centroids[ans - 1]))
                    ans = i + 1;
                i++;
            }
            return ans;

        } // DiscretizeValue

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            int i;
            int[] ans = new int[data.Length];

            for (i = 0; i < data.Length; i++)
                ans[i] = DiscretizeValue(data[i], cuts);

            return ans;
        } // DiscretizeData


        private double[] getMeansCuts(double[] means, double[] variances)
        {
            int i;
            double[] ans = new double[means.Length - 1];
            double[] stds = new double[variances.Length];
            for (i = 0; i < variances.Length; i++)
                stds[i] = Math.Sqrt(variances[i]);
            for (i = 0; i < ans.Length; i++)
                ans[i] = (stds[i] + stds[i + 1] != 0) ? (means[i] + ((means[i + 1] - means[i]) * stds[i] / (stds[i] + stds[i + 1]))) : 0;
            return ans;
        }


        /// <summary>
        /// Calculates bins using K-Means Discretization, with a one dimensional data.
        /// </summary>
        /// <param name="list">data to be discretized</param>
        /// <returns></returns>
        public List<Bin> CalculateBins(double[] list)
        {
            int i, j;
            int group;

            int numofbins = 3;
            if (parameters.getParam(Const.STR_NUM_OF_BINS) != Parameters.NotFound)
                numofbins = ((int)parameters.getParam(Const.STR_NUM_OF_BINS));

            if (list.Length == 1)
            {
                double[] fcuts = new double[numofbins];
                for (i = 0; i < fcuts.Length; i++)
                    fcuts[i] = list[0];
                List<Bin> fbins = Methods.CreateBins(fcuts, Const.STR_ARR_BIN_BASE_LABELS);
                for (i = 0; i < fbins.Count; i++)
                {
                    fbins[i].mean = list[0];
                    fbins[i].variance = 0;
                    fbins[i].size = 1;
                }

                return fbins;
            }

            // create primal cuts
            double[] percentiles = new double[numofbins];
            for (i = 0; i < numofbins; i++)
                percentiles[i] = (numofbins + 1 != 0) ? (double)(i + 1) / (double)(numofbins + 1) : 0;

            // cuts is the list of centroids, starting with percentiles by num of bins
            double[] cuts = Methods.getListPrctile(Methods.sortList(list), percentiles);
            //double[] cuts = { list[0], list[1], list[2] };
            //double[] cuts = { list[list.Length - 3], list[list.Length - 2], list[list.Length - 1] };
            double[] prevcuts = new double[cuts.Length];
            double[] sumofItems = new double[cuts.Length];
            int[] numofItems = new int[cuts.Length];
            double[] variances = new double[cuts.Length];

            do
            {
                cuts.CopyTo(prevcuts, 0);
                for (i = 0; i < sumofItems.Length; i++) sumofItems[i] = 0;
                for (i = 0; i < numofItems.Length; i++) numofItems[i] = 0;
                for (i = 0; i < variances.Length; i++) variances[i] = 0;

                // first gather Items by groups
                for (i = 0; i < list.Length; i++)
                {
                    group = 0;
                    // get the closest cut to the current item
                    for (j = 1; j < prevcuts.Length; j++)
                        if (Math.Abs(list[i] - prevcuts[j]) < Math.Abs(list[i] - prevcuts[group]))
                            // update Item's group (closest cut)
                            group = j;

                    sumofItems[group] += list[i];
                    numofItems[group]++;
                    variances[group] += Math.Pow(Math.Abs(list[i] - prevcuts[group]), 2);
                } // for

                // Calculate new centroids
                for (i = 0; i < cuts.Length; i++)
                {
                    if (numofItems[i] == 0)
                    {
                        cuts[i] = 0;
                        variances[i] = 0;
                    }
                    else
                    {
                        cuts[i] = (numofItems[i] != 0) ? (sumofItems[i] / numofItems[i]) : 0;
                        variances[i] /= numofItems[i];
                    }
                }
            }
            // once there's no difference, cuts are found
            while (!Methods.compareArrays(cuts, prevcuts));

            sortKMeansResults(cuts, sumofItems, numofItems, variances);
            SumOfItems = sumofItems;
            NumOfItems = numofItems;
            this.variances = variances;

            double[] kcuts = getMeansCuts(cuts, variances);
            List<Bin> bins = Methods.CreateBins(kcuts, Const.STR_ARR_BIN_BASE_LABELS);
            for (i = 0; i < bins.Count; i++)
            {
                bins[i].mean = cuts[i];
                bins[i].variance = variances[i];
                bins[i].size = numofItems[i];
            }

            return bins;

        } // CalculateBins

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts - means (centroids)</param>
        /// <returns>list of clustered values</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = DiscretizeValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // class KMeans


    /* SAX Discretization method
     */
    public class SAX : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;

        public SAX()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// returns the appropriate cuts by using normal deviation
        /// loads values from given file name
        /// </summary>
        /// <param name="filename">name of file containing the deviation cuts</param>
        /// <param name=Const.STR_NUM_OF_BINS>number of bins used</param>
        /// <returns>a list of deviation cuts, used in SAX discretization</returns>
        private double[] getSAXCutsByBins(int numofbins)
        {
            double[] cuts = new double[numofbins - 1];
            string[] arrCuts ={"0",
                              "-0.4307 0.4307",
                              "-0.6745 0 0.6745",
                              "-0.8416 -0.2533 0.2533 0.8416",
                              "-0.9774 -0.4307 0 0.4307 0.9774",
                              "-1.0676 -0.5659 -0.18 0.18 0.5659 1.0676",
                              "-1.1503 -0.6745 -0.3186 0 0.3186 0.6745 1.1503",
                              "-1.2206 -0.7647 -0.4307 -0.1397 0.1397 0.4307 0.7647 1.2206",
                              "-1.2816 -0.8416 -0.5244 -0.2522 0. 0.2522 0.5244 0.8416 1.2816"};
            int i;
            string[] vals;


            vals = arrCuts[numofbins - 2].Split(' ');
            for (i = 0; i < vals.Length; i++)
                cuts[i] = double.Parse(vals[i]);
            return cuts;
        }//getSaxCutsFromFile

        /// <summary>
        /// Calculates Bins using SAX Discretization
        /// </summary>
        /// <param name="timeseries">the time series</param>
        /// <param name="parameters">given parameters used inth process (NumOfBins) </param>
        /// <returns>a list of bins ready to use</returns>
        public List<Bin> CalculateBins(double[] list)
        {

            int numofbins = 3;
            if (parameters.getParam(Const.STR_NUM_OF_BINS) != Parameters.NotFound)
                numofbins = ((int)parameters.getParam(Const.STR_NUM_OF_BINS));
            //string filename = ((string)parameters.getParam("CutsFileName"));
            int j;
            int n = list.Length;
            double mean, stddev, sum;
            double[] cuts;
            List<Bin> Bins;

            cuts = getSAXCutsByBins(numofbins);

            // calculate sum of values
            sum = Methods.getListSum(list, 0, n);
            // calculate avarage
            mean = (n != 0) ? sum / ((double)(n)) : 0;
            // calculate standard deviation
            stddev = Methods.getListStandardDeviation(list, 0, n, mean);
            // Z normalization
            for (j = 0; j < numofbins - 1; j++)
                cuts[j] = cuts[j] * stddev + mean;
            Bins = Methods.CreateBins(cuts, Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;
        } // CalculateBins SAX

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // class SAX 

    /// <summary>
    /// Equal frequencies discretization
    /// </summary>
    public class EQF : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;

        public EQF()
        {
            parameters = new Parameters();
        } // basic constructor;

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        public List<Bin> CalculateBins(double[] list)
        {
            int i;
            list = Methods.sortList(list);
            int numofbins = ((int)parameters.getParam(Const.STR_NUM_OF_BINS));
            double[] cuts;
            double[] p = new double[numofbins - 1];
            List<Bin> Bins;
            for (i = 1; i < numofbins; i++)
                p[i - 1] = (numofbins != 0) ? ((double)i) / ((double)numofbins) : 0;
            // get percentiles as the cuts
            cuts = Methods.getListPrctile(list, p);
            for (i = 0; i < cuts.Length; i++)
                cuts[i] += Const.EQF_DEVIATION_FIX;
            Bins = Methods.CreateBins(cuts, Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;
        } // CalculateBins EQF

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue


    } // class EQF

    /// <summary>
    /// Equal width discretization
    /// </summary>
    public class EQW : DiscretizationMethod
    {
        long _ID;
        public Parameters parameters;

        public EQW()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters


        /// <summary>
        /// Discretize data by EQW
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData



        /// <summary>
        /// EQW Discretization method, generate discretization in a simple
        /// inefficient way by using equal width bins.
        /// </summary>
        /// <param name="list">data to discretize</param>
        /// <param name="parameters">parameters used in the process (NumOfBins)</param>
        /// <returns></returns>
        public List<Bin> CalculateBins(double[] list)
        {
            int i;
            int numofbins = ((int)parameters.getParam(Const.STR_NUM_OF_BINS));
            // no parameter
            if (numofbins == -1) numofbins = 3;
            List<Bin> Bins = new List<Bin>();
            double[] cuts = new double[numofbins - 1];
            double min = list[0];
            double max = list[0];

            for (i = 0; i < list.Length; i++)
            {
                if (list[i] > max) max = list[i];
                if (list[i] < min) min = list[i];
            } // for find min and max

            double IntervalWidth = (numofbins != 0) ? (max - min) / numofbins : 0;

            for (i = 1; i < numofbins; i++)
                cuts[i - 1] = min + i * IntervalWidth;

            Bins = Methods.CreateBins(cuts, Const.STR_ARR_BIN_BASE_LABELS);

            // update min and max;
            Bins[0]._lowlimit = min;
            Bins[Bins.Count - 1]._highlimit = max;


            return Bins;
        } // CalculateBins EQW

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue

    } // class EQW


}