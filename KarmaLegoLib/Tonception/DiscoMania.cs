using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using KarmaLegoLib;

namespace Tonception
{
    public class DiscoMania
    {
        public static void testTD4C()
        {
            double[][] databyclass = new double[2][];
            double[][][] databyclassuser = new double[2][][];
            databyclass[0] = new double[] { 1, 2, 3, 1, 2, 2, 43, 4, 965, 07, 9, 887, 7, 8564, 36, 5323, 4243, 786, 86, 789, 76, 2, 4, 65, 56, 57, 2, 54, 4235, 345, 23, 5, 43, 5132, 32 };
            databyclass[1] = new double[] { 68, 6, 7, 62, 5, 87, 97, 162, 5, 7, 868, 60, 31573, 2, 432, 5, 345, 432, 54, 235, 345, 5, 2345, 75, 564, 56, 90, 5, 786, 4, 54, 525687, 98, 97, 45365 };
            databyclassuser[0] = new double[2][];
            databyclassuser[0][0] = new double[] { 1, 2, 3, 1, 2, 2, 43, 4, 965, 07, 9, 887, 7, 8564, 36, 5323, 4243, 786 };
            databyclassuser[0][1] = new double[] { 86, 789, 76, 2, 4, 65, 56, 57, 2, 54, 4235, 345, 23, 5, 43, 5132, 32 };
            databyclassuser[1] = new double[2][];
            databyclassuser[1][0] = new double[] { 68, 6, 7, 62, 5, 87, 97, 162, 5, 7, 868, 60, 31573, 2, 432, 5, 345, 432 };
            databyclassuser[1][1] = new double[] { 54, 235, 345, 5, 2345, 75, 564, 56, 90, 5, 786, 4, 54, 525687, 98, 97, 45365 };
            double[] data = new double[] { 1, 2, 3, 1, 2, 2, 43, 4, 965, 07, 9, 887, 7, 8564, 36, 5323, 4243, 786, 86, 789, 76, 2, 4, 65, 56, 57, 2, 54, 4235, 345, 23, 5, 43, 5132, 32, 68, 6, 7, 62, 5, 87, 97, 162, 5, 7, 868, 60, 31573, 2, 432, 5, 345, 432, 54, 235, 345, 5, 2345, 75, 564, 56, 90, 5, 786, 4, 54, 525687, 98, 97, 45365 };
            Tonception.TD4CLA_Ent d = new Tonception.TD4CLA_Ent();
            d.parameters.Add("TopK", 10);
            d.parameters.Add(Tonception.Const.STR_NUM_OF_BINS, 5);
            d.parameters.Add(Tonception.Const.STR_DATA_BY_CLASS, databyclass);
            d.parameters.Add(Tonception.Const.STR_DATA_BY_CLASS_USER, databyclassuser);
            List<Tonception.Bin> bins = d.CalculateBins(data);

            Tonception.TD4C_Ent d1 = new Tonception.TD4C_Ent();
            d1.parameters.Add(Tonception.Const.STR_NUM_OF_BINS, 5);
            d1.parameters.Add(Tonception.Const.STR_DATA_BY_CLASS, databyclass);
            bins = d1.CalculateBins(data);
            bins.Add(new Tonception.Bin(0, 4));
        }

        public static void saveTemporalPropertiesTableToTSsFile(string server, string user, string password, string dbName, string tableName, int entitieSize, string TSsFile)
        {
            DataBase dbase = new DataBase(server, user, password, dbName, true, 30);
            TonceptSet disco = new TonceptSet();
            disco = Tonception.Kiosk.LoadTimestampsValuesTableToDiscoSet(tableName, entitieSize, dbase);
            disco.saveTonceptSetTSsToFile(TSsFile);
        }

        public static void tonceptlizeTimeSeriesByClass(string timeSeriesFile, int binsNum, string absMethod, string timeIntervalsFile, int classID)
        {
            TonceptSet disco = new TonceptSet();
            disco.classSeparator = classID;
            disco.readTonceptSetTSsFile(timeSeriesFile);
            disco.tonceptulize(absMethod, binsNum);
            disco.multValueClassTI(timeIntervalsFile);
        }

        public static void saveTmprlPrprtsIntrvlsTblToTIsFile(string server, string user, string password, string dbName, string tiTableName, string bdTableName, int entitieSize, string TIsFile)
        {
            DataBase dbase = new DataBase(server, user, password, dbName, true, 30);
            TonceptSet disco = new TonceptSet();
            Kiosk.LoadBinsDefinitionsTableToDiscoSet(ref disco, bdTableName, dbase);
            Kiosk.LoadTimeIntervalsTableToDiscoSet(ref disco, tiTableName, entitieSize, dbase);
            disco.saveTonceptSetTIsToFile(TIsFile);
        }

        public static void tonceptulizeTimeSeries(string timeSeriesFile, int binsNum, string absMethod, string timeIntervalsFile)
        {
            TonceptSet disco = new TonceptSet();
            disco.readTonceptSetTSsFile(timeSeriesFile); // "D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\" + daTable + "TimeSeriesSet.txt");
            disco.tonceptulize(absMethod, binsNum); 
            disco.saveTonceptSetTIsToFile(timeIntervalsFile);
        }

        public static void testTonceptions()
        {
            Tonception.DataBase dbase = new Tonception.DataBase("ROBERT-X220", "QiSecUser", "AbV01", "Diabetes_TDW", true, 30);
            TonceptSet disco = new TonceptSet();
            string daTable = "TemporalPropertiesValuesFA"; //"TemporalPropertiesValuesFAGender";
            disco = Tonception.Kiosk.LoadTimestampsValuesTableToDiscoSet(daTable, 3000, dbase);
            disco.saveTonceptSetTSsToFile("D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\" + daTable + "TimeSeriesSet.txt");
            TonceptSet discoFile = new TonceptSet();
            discoFile.readTonceptSetTSsFile("D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\" + daTable + "TimeSeriesSet.txt");
            int binsNum = 3;
            string absMethod = Const.STR_MNAME_SAX;
            disco.tonceptulize(absMethod, binsNum);
            disco.saveTonceptSetTIsToFile("D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\" + daTable + "TimeIntrvlsSet" + absMethod + binsNum + ".txt");
            disco.saveKLfile("D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\" + daTable + "_KL_" + absMethod + binsNum + ".txt");

            /*
            double[] vals = disco.getValuesAsDoubleVectorOfVariable(3);
            //TimeStampValueSymbol[] tsVec = disco.getTimeStampValuesVectorOfVariable(3);

            //DiscretizationMethod method = getMethodObject(progparameters.getParam(Const.STR_METHOD_NAME).ToString(), intBins, intTopK, progparameters);
            List<Tonception.Bin> binsListPRSST = Tonception.DiscoMania.getBinsPerMethodAndData(Tonception.Const.STR_MNAME_PERSIST, 3, vals, "");
            //List<Tonception.Bin> binsListPRSST = Tonception.DiscoMania.getBinsPerMethodFromTSVSs(Tonception.Const.STR_MNAME_PERSIST, 3, vals, "");
            List<Tonception.Bin> binsListSAX = Tonception.DiscoMania.getBinsPerMethodAndData(Tonception.Const.STR_MNAME_SAX, 3, vals, "");
            double[] cutsPRSST = DiscoMania.BinsToCuts(binsListPRSST);
            double[] cutsSAX = DiscoMania.BinsToCuts(binsListSAX);
            int[] discValuesPRSST = Methods.DiscretizeData(vals, cutsPRSST);
            int[] discValuesSAX = Methods.DiscretizeData(vals, cutsSAX);
            DiscretizeTSVSs(tsVec, binsListSAX); // cutsSAX);
            TimeStampValueSymbol tsvs;
            TimeStampValueSymbol[] tisVec = new TimeStampValueSymbol[discValuesSAX.Length];
            //List<TimeIntervalSymbol> tisList = intervalize(tsVec); // check the settings, make TIs at least 1 timestamp
            */
        }

        public static double[] BinsToCuts(List<Bin> bins)
        {
            return Methods.Bins2Cuts(bins);
        }

        /// <summary>
        /// Discretizes a list of values, by given list of cuts
        /// </summary>
        /// <param name="data">data to be discretized</param>
        /// <param name="cuts">cuts list (levels definitions)</param>
        /// <returns>list of discretized data, by levels numbers</returns>
        public static void DiscretizeTSVSs(List<TimeStampValueSymbol> tsvsList, List<Bin> binList) // double[] cuts)
        {
            for (int i = 0; i < tsvsList.Count; i++)
                tsvsList.ElementAt(i).symbol = (int)getDiscretizedValue(tsvsList.ElementAt(i).value, binList); // cuts);
        }

        /// <summary>
        /// Discretizes a single value by given bins list
        /// </summary>
        /// <param name="value">value to be discretized</param>
        /// <param name="cuts">cuts list (levels definitions)</param>
        /// <returns>discretized value (level number)</returns>
        public static long getDiscretizedValue(double value, List<Bin> binList)
        {
            int i = 0;
            while (i < binList.Count && value >= binList.ElementAt(i)._highlimit)
            {
                if (i == binList.Count - 1)
                    break;
                i++;
            }
            return binList.ElementAt(i)._ID;
        } // getDiscretizedValue

        // *************************** Intervals definitions **************************

        /// <summary>
        /// Define intervals for this entity using the given discretized data, and intervals definition settings
        /// Available options:
        /// ExtendIntervalsPlus - each
        /// NoFlatIntervals - a one time point interval will be set to start at time and end at time + 1
        /// TimeRegardless - time gaps will be ignored and won't define the end of an interval.
        /// </summary>
        /// <param name="data">discretized data table, including timestamp and binid</param>
        /// <param name="parameters">Parameters object containing options for intervals definition</param>
        public static List<TimeIntervalSymbol> intervalize(List<TimeStampValueSymbol> tsvsList /*[]  serie*/) //, string strTableName, int entityID, int propertyID, Parameters parameters)
        {
            List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
            TimeIntervalSymbol tis;
            // *********************  set default settings
            bool blnNoFlatIntervals = true; // Intervals of single time point will be defined to start on time and end on time + 1
            bool blnTimeRegardless = false; // Intervals won't be cut by a time gap
            int intPostTime = 1; // intervals will be cut only if time gap is longer than this value. used only // in NoTimeRegardless mode

            int idx = 0;
            if (tsvsList.Count == 0) return tisList; // if no data, exit
            int curTime = 0, starTime = tsvsList.ElementAt(idx).timestamp, endTime = 0;
            int prevTime = starTime, curBin = 0, prevBin = tsvsList.ElementAt(idx).symbol;

            while (idx < tsvsList.Count)
            {
                curBin = tsvsList.ElementAt(idx).symbol;
                curTime = tsvsList.ElementAt(idx).timestamp;
                if ((curBin != prevBin) || (!blnTimeRegardless && curTime > prevTime + intPostTime))
                {
                    endTime = blnNoFlatIntervals && starTime == prevTime ? prevTime + 1 : prevTime;
                    tis = new TimeIntervalSymbol(starTime, endTime, prevBin);
                    tisList.Add(tis);

                    starTime = curTime;
                }// if interval was over
                prevBin = curBin;
                prevTime = curTime;
                idx++;
            }// while data is read

            // add last interval
            endTime = blnNoFlatIntervals && starTime == prevTime ? prevTime + 1 : prevTime;
            tis = new TimeIntervalSymbol(starTime, endTime, prevBin);
            tisList.Add(tis);
            return tisList;
        } // defineIntervals

        public static int[] TonceptByBins(double[] values, double[] cuts)
        {
            return Methods.DiscretizeData(values, cuts);
        }

        public static List<Bin> getBinsPerMethodFromTSVSs(string methodName, int binsNumber, TimeStampValueSymbol[] tsvss, string kbFileName)
        {
            double[] vals = new double[tsvss.Length];
            for (int i = 0; i < tsvss.Length; i++)
                vals[i] = tsvss[i].value;
            DiscretizationMethod method = getMethodObject(methodName, binsNumber, kbFileName); //, intTopK, progparameters);
            List<Bin> binsList = method.CalculateBins(vals);

            return binsList;
        }

        public static List<Bin> getBinsPerMethodAndData(string methodName, int binsNumber, double[] vals, string kbFileName)
        {
            DiscretizationMethod method = getMethodObject(methodName, binsNumber, kbFileName); //, intTopK, progparameters);
            List<Bin> binsList = method.CalculateBins(vals);

            return binsList;
        }

        /// <summary>
        /// analyzes given method name and creates the respected object
        /// </summary>
        /// <param name="strMethodName"></param>
        /// <param name="intBins"></param>
        /// <param name="intTopK"></param>
        /// <param name="progparameters"></param>
        /// <returns></returns>
        private static DiscretizationMethod getMethodObject(string strMethodName, int intBins, string kb_bins_filename) //, int intTopK, Parameters progparameters)
        {
            DiscretizationMethod method = null;
            if (strMethodName.ToUpper() == Const.STR_MNAME_EQW.ToUpper())
            {
                EQW eqw = new EQW();
                eqw.parameters = new Parameters();
                eqw.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                method = eqw;
            } // if eqw
            else if (strMethodName.ToUpper() == Const.STR_MNAME_SAX.ToUpper())
            {
                SAX sax = new SAX();
                sax.parameters = new Parameters();
                sax.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                method = sax;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_EQF.ToUpper())
            {
                EQF eqf = new EQF();
                eqf.parameters = new Parameters();
                eqf.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                method = eqf;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_PERSIST.ToUpper())
            {
                Persist persist = new Persist();
                persist.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                persist.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = persist;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_FILE.ToUpper())
            {
                BinsFile binsfile = new BinsFile(kb_bins_filename); // progparameters.getParam(Const.STR_BINS_FILENAME).ToString());
                binsfile.parameters.Add(Const.STR_CURRENT_PROPERTY, 0);
                method = binsfile;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C.ToUpper())
            {
                TD4C_Ent d4c = new TD4C_Ent();
                d4c.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C2.ToUpper())
            {
                TD4C_Cos d4c2 = new TD4C_Cos();
                d4c2.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c2.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c2;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C3.ToUpper())
            {
                TD4C_KL d4c3 = new TD4C_KL();
                d4c3.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c3.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c3;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C4.ToUpper())
            {
                TD4C_DiffSum d4c = new TD4C_DiffSum();
                d4c.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C5.ToUpper())
            {
                TD4C_DEnt d4c2 = new TD4C_DEnt();
                d4c2.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c2.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c2;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C6.ToUpper())
            {
                TD4C_DiffSumMax d4c3 = new TD4C_DiffSumMax();
                d4c3.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c3.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c3;
            }
            /*else if (strMethodName.ToUpper() == Const.STR_MNAME_D4CLA.ToUpper())
            {
                TD4CLA_Ent d4cla = new TD4CLA_Ent();
                d4cla.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4cla.parameters.Add(Const.STR_SCAN_RANGE, "false");
                d4cla.parameters.Add(Const.STR_TOP_K, intTopK);
                method = d4cla;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C2LA.ToUpper())
            {
                TD4CLA_Cos d4cla2 = new TD4CLA_Cos();
                d4cla2.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4cla2.parameters.Add(Const.STR_SCAN_RANGE, "false");
                d4cla2.parameters.Add(Const.STR_TOP_K, intTopK);
                method = d4cla2;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C3LA.ToUpper())
            {
                TD4CLA_KL d4cla3 = new TD4CLA_KL();
                d4cla3.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4cla3.parameters.Add(Const.STR_SCAN_RANGE, "false");
                d4cla3.parameters.Add(Const.STR_TOP_K, intTopK);
                method = d4cla3;
            }*/
            else
                Error.hardError(Const.INT_ERR_METHOD_NAME);
            return method;
        } // getMethodObject
    }
}