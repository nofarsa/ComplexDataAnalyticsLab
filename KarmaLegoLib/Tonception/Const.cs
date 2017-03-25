using System;
using System.Collections.Generic;
using System.Text;

namespace Tonception
{
    /// <summary>
    /// Contains strings used in the program, including fields names, tables names, settings strings etc.
    /// </summary>
    public class Const
    {

        // error codes
        public static string[] ARR_ERROR_MSG = new string[] 
                { "Method Name-Specified method name is undefined !",
                  "Options-Unknown option, ignoring...",
                  "Server-Unable to connect to server !",
                  "Database-Database does not exist !",
                  "Database-At least one of the tables does not exist !",
                  "Options-Temporal property 'Class' does not exist! Using no class...",
                  "Class-User's class is undefined ! Removing user...",
                  "Parameters-Missing parameters !\n Usage :\n Discretizer.exe ServerName UserName Password DatabaseName EntitiesTableName PropertiesTableName PropertiesValuesTableName MethodName NumberOfBins Options", 
                  "DataBase-Unable to create results tables !",
                  "Options-Bins filename doesn't exist !",
                  "Options-Nothing to discretize : Select States and/or Tendency !"
                };
        public static int INT_ERR_METHOD_NAME = 0;
        public static int INT_ERR_UNKNOWN_OPTION = 1;
        public static int INT_ERR_SERVER = 2;
        public static int INT_ERR_DATABASE = 3;
        public static int INT_ERR_TABLES = 4;
        public static int INT_ERR_CLASS_PROPERTY = 5;
        public static int INT_ERR_USER_CLASS = 6;
        public static int INT_ERR_PARAMS_NUM = 7;
        public static int INT_ERR_CREATE_TABLE = 8;
        public static int INT_ERR_BINS_FILE = 9;
        public static int INT_ERR_OPTIONS_ERROR = 10;

        //**********************************************************************

        // program settings
        public static string STR_SERVER_NAME = "ServerName";
        public static string STR_DATABASE_NAME = "DatabaseName";
        public static string STR_USER_NAME = "UserName";
        public static string STR_PASSWORD = "Password";
        public static string STR_DATASET_NAME = "DatasetName";
        public static string STR_FULL_DATASET_NAME = "FullDatasetName";
        public static string STR_NO_USE_CLASS = "NoUseClass";
        public static string STR_USE_CLASS = "UseClass";
        public static string STR_NO_USE_TENDENCY = "NoUseTendency";
        public static string STR_USE_TENDENCY = "UseTendency";
        public static string STR_NO_USE_STATES = "NoUseStates";
        public static string STR_INTERPOLATION = "Interpolation";

        public static string STR_USERS_CLASSES = "UsersClasses";
        public static string STR_CLASS_PROPERTY_ID = "ClassID";
        public static string STR_CLASS_PROPERTY_NAME = "class";
        public static string STR_USERS_BY_CLASSES = "UsersByClassesList";
        public static string STR_CLASSES_LIST = "ClassesList";
        public static string STR_PROPERTIES_NAMES = "PropertiesNames";
        public static string STR_BINS_FILENAME = "BinsFilename";

        //*********************************************************************
        // Mathematical
        public static double EPS = 0.000000001;
        public static double EQF_DEVIATION_FIX = 0.01;


        //*********************************************************************

        // Entitites 
        public static string STR_ENTITIES_SAMPLES_TABLE = "EntitiesSamples";
        public static string STR_SAMPLE_ID = "SampleID";
        public static string STR_ENTITIES_TABLE = "Entities";
        public static string STR_ENTITY_ID = "EntityID";
        public static string STR_ENTITY_NAME = "EntityName";

        //  Properties table Constants
        public static string STR_STATIC_PROPERTIES_TABLE = "StaticProperties";
        public static string STR_STATIC_PROPERTY_ID = "StaticPropertyID";
        public static string STR_STATIC_PROPERTY_NAME = "StaticPropertyName";

        //  Properties values ants
        public static string STR_STATIC_PROPERTIES_VALUES_TABLE = "StaticPropertiesValues";
        public static string STR_STATIC_PROPERTY_VALUE = "StaticPropertyValue";


        // Temporal Properties table ants
        public static string STR_TEMPORAL_PROPERTIES_TABLE = "TemporalProperties";
        public static string STR_TEMPORAL_PROPERTY_ID = "TemporalPropertyID";
        public static string STR_TEMPORAL_PROPERTY_NAME = "TemporalPropertyName";
        public static string STR_TEMPORAL_PROPERTY_SETTINGS = "Settings";
        public static string STR_TEMPORAL_PROPERTY_CLASS = "CLASS";

        // Temporal Properties values table ants
        public static string STR_TEMPORAL_PROPERTIES_VALUES_TABLE = "TemporalPropertiesValues";
        public static string STR_TEMPORAL_PROPERTY_VALUE = "TemporalPropertyValue";
        public static string STR_TIME_STAMP = "TimeStamp";

        // Bins definitions table
        public static string STR_BINS_DEFINITIONS_TABLE = "BinsDefinitions";
        public static string STR_METHOD_NAME = "MethodName";
        public static string STR_METHOD_ID = "MethodID";
        public static string STR_RESULTS_ERROR1 = "Error1";
        public static string STR_RESULTS_ENTROPY = "Entropy";
        public static string STR_STATE_ID = "StateID";
        public static string STR_BIN_ID = "BinID";
        public static string[] STR_ARR_BIN_BASE_LABELS = { "Low", "Medium", "High" };
        public static string STR_BIN_LABEL = "BinLabel";
        public static string STR_BIN_FROM = "BinFrom";
        public static string STR_BIN_TO = "BinTo";

        // Sequentials table constants
        public static string STR_SEQUENTIALS_TABLE = "Sequentials";
        public static string STR_SEQUENTIALS_PROPERTY_FROM = "FromTemporalProperty";
        public static string STR_SEQUENTIALS_PROPERTY_TO = "ToTemporalProperty";

        // Discretization methods definitions table constants
        public static string STR_METHODS_DEFINITIONS_TABLE = "DiscretizationMethodsDefinitions";
        public static string STR_METHODS_DEFINITIONS_ID = "DefinitionsID";
        public static string STR_METHODS_DEFINITIONS_COMMENTS = "Comments";

        //****************************************************************************

        public static string STR_MNAME_EQW = "EQW";
        public static string STR_MNAME_EQF = "EQF";
        public static string STR_MNAME_SAX = "SAX";
        public static string STR_MNAME_PERSIST = "Persist";
        public static string STR_MNAME_KMEANS = "KMeans";
        public static string STR_MNAME_NONE = "None";
        public static string STR_MNAME_D4C = "TD4C_Ent";
        public static string STR_MNAME_D4C2 = "TD4C_Cos";
        public static string STR_MNAME_D4C3 = "TD4C_KL";
        public static string STR_MNAME_D4C4 = "TD4C_DiffSum";
        public static string STR_MNAME_D4C5 = "TD4C_DEnt";
        public static string STR_MNAME_D4C6 = "TD4C_DiffSumMax";
        public static string STR_MNAME_D4CLA = "TD4CLA_Ent";
        public static string STR_MNAME_D4C2LA = "TD4CLA_Cos";
        public static string STR_MNAME_D4C3LA = "TD4CLA_KL";

        public static string STR_MNAME_FILE = "File";


        // Discretization method settings
        public static string STR_NUM_OF_BINS = "NumOfBins";

        // persist
        public static string STR_TRIM_LEFT = "TrimLeft";
        public static string STR_TRIM_RIGHT = "TrimRight";
        public static string STR_PERCENTILES = "Percentiles";
        public static string STR_SCAN_RANGE = "ScanRange";

        // TD4C_Ent
        public static string STR_DATA_BY_CLASS = "DataByClass";
        public static string STR_DATA_BY_CLASS_USER = "DataByClassUser";
        public static string STR_TOP_K = "TopK";

        public static string STR_CURRENT_PROPERTY = Const.STR_CURRENT_PROPERTY;



        // Tendency
        public static string STR_CALCULATE_TENDENCY = "CalculateTendency";
        public static string STR_TENDENCY_K = "TendencyK";

        // K-Means Clustering
        public static string[] STR_ARR_CLUSTER_BINS_BASE_LABELS = { "Short", "Medium", "Long" };
        public static string STR_CLUSTER_ID = "IntervalClusterID";
        public static string STR_CLUSTER_LABEL = "IntervalClusterLabel";
        public static string STR_CLUSTER_MEAN = "IntervalClusterCentroid";
        public static string STR_CLUSTER_VARIANCE = "IntervalClusterVariance";
        public static string STR_CLUSTER_SIZE = "IntervalClusterSize";

        // GMM Clustering
        public static string STR_SIGMA = "Sigma";

        // intervals defintion settings
        public static string STR_EXTEND_INTERVALS_MINUS = "ExtendIntervalsMinus";
        public static string STR_EXTEND_INTERVALS_PLUS = "ExtendIntervalsPlus";
        public static string STR_NO_FLAT_INTERVALS = "NoFlatIntervals";
        public static string STR_FLAT_INTERVALS = "FlatIntervals";
        public static string STR_POST_TIME = "PostTime";
        public static string STR_TIME_REGARDLESS = "TimeRegardless";
        public static string STR_NO_TIME_REGARDLESS = "NoTimeRegardless";
        public static string STR_CLUSTER_INTERVALS = "ClusterIntervals";
        public static string STR_NO_CLUSTER_INTERVALS = "NoClusterIntervals";
        public static string STR_CLUSTER_K = "ClusterK";
        // intervals table settings
        public static string STR_INTERVALS_TABLE = "TemporalPropertiesIntervals";
        public static string STR_START_TIME = "StartTime";
        public static string STR_END_TIME = "EndTime";


        // Export settings
        public static string STR_EXPORT_ENTITY_ID = "object_id";
        public static string STR_EXPORT_TEMPORAL_PROPERTY_ID = "op_id";
        public static string STR_EXPORT_STATE_ID = "p_id";
        public static string STR_EXPORT_START_TIME = "s_time";
        public static string STR_EXPORT_END_TIME = "e_time";

        // Temporal features names
        public static string STR_TF_KEYDOWN_PER_SECOND = "KeyboardKeyDownPerSecond";
        public static string STR_TF_KEYPRESS_PER_SECOND = "KeyboardKeyPressPerSecond";
        public static string STR_TF_KEYUP_PER_SECOND = "KeyboardKeyUpPerSecond";
        public static string STR_TF_AVG_DWELL_PER_SECOND = "KeyboardAvgDwellTimePerSecond";
        public static string STR_TF_AVG_FLIGHT_PER_SECOND = "KeyboardAvgFlightTimePerSecond";
        public static string STR_TF_KB_PRESSED = "KeyboardPressedBoolean";

        // Erorr Texts
        public static string STR_ERR_NO_DATA = "No data to load!";
    }

}
