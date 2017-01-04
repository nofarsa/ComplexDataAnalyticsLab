using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KarmaLegoLib
{
    public class CUMC_entity : IComparable
    {
        public int entityID;
        public string entityLine;
        public List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();

        //public TemporalConcept cohrTncpts = new TemporalConcept(0,0);
        public TemporalConcept condTncpts = new TemporalConcept(1,1);
        public TemporalConcept drugTncpts = new TemporalConcept(2,2);
        public TemporalConcept procTncpts = new TemporalConcept(3,3);

        //public int cohrTncptNum;
        public int condTncptNum;
        public int drugTncptNum;
        public int procTncptNum;
        public double simScore;

        public CUMC_entity(int eID)
        {
            entityID = eID;
        }

        public int CompareTo(object obj)
        {
            if (obj is CUMC_entity)
            {
                CUMC_entity other = obj as CUMC_entity;
                int result = simScore.CompareTo(other.simScore);
                return result;
            }
            else
                throw new ArgumentException("Object is not a CUMC_entity.");
        }

        public void addTIS(TimeIntervalSymbol tis, string tncptOrig)
        {
            tisList.Add(tis);
            //if (tncptOrig.StartsWith("CHRT"))
            //    addTncptID(ref cohrTncpts, ref cohrTncptNum, tis.symbol);
            //else 
            if(tncptOrig.StartsWith("COND"))
                addTncptID(ref condTncpts, tis); // ref condTncptNum, tis.symbol);
            else if(tncptOrig.StartsWith("DRUG"))
                addTncptID(ref drugTncpts, tis); // ref drugTncptNum, tis.symbol);
            else if(tncptOrig.StartsWith("PROC"))
                addTncptID(ref procTncpts, tis); // ref procTncptNum, tis.symbol);
        }

        private void addTncptID(ref TemporalConcept tc, TimeIntervalSymbol tis) // ref int tcNum, int tncptID)
        {
            tc.addEntityTinstance(tis.symbol, tis); // tncptID, null); //tc.addEntity(tncptID);
            //tcNum++;
        }

        public double hasPROCtoncepts(Dictionary<int, float> tncptCounter)
        {
            double count = 0;
            for(int t = 0; t < tncptCounter.Count(); t++)
                if( procTncpts.getTonceptHorizontalDic().ContainsKey(tncptCounter.ElementAt(t).Key) )
                    count++;
            return count / tncptCounter.Count();
        }

        public double hasDRUGtoncepts(Dictionary<int, float> tncptCounter)
        {
            double count = 0;
            for(int t = 0; t < tncptCounter.Count(); t++)
                if( drugTncpts.getTonceptHorizontalDic().ContainsKey(tncptCounter.ElementAt(t).Key) )
                    count++;
            return count / tncptCounter.Count();
        }

        public double hasCONDtoncepts(Dictionary<int, float> tncptCounter)
        {
            double count = 0;
            for(int t = 0; t < tncptCounter.Count(); t++)
                if( !condTncpts.getTonceptHorizontalDic().ContainsKey(tncptCounter.ElementAt(t).Key) )
                    count++;
            return count / tncptCounter.Count();
        }
    }

    public class CUMC_Handler
    {
        private static Dictionary<int, string> read_file_toIntStringDic(string filePath)
        {
            Dictionary<int, string> intStringDic = new Dictionary<int,string>();
            TextReader tr = new StreamReader(filePath);
            int count = 0;
            string readLine = tr.ReadLine();
            while (tr.Peek() >= 0)
            {
                readLine = tr.ReadLine();
                if (readLine.Contains(','))
                {
                    string[] mainDelimited = readLine.Split(',');
                    int id = int.Parse(mainDelimited[0]);
                    if (intStringDic.ContainsKey(id))
                        intStringDic[id] += readLine + ";";
                    else
                        intStringDic.Add(int.Parse(mainDelimited[0]), readLine + ";");
                    count++;
                }
                else
                    count = count;
            }
            tr.Close();
            return intStringDic;    
        }

        private static void report_toncepts(Dictionary<int, TemporalConcept> tonceptsDic, int top, Dictionary<int, string> conceptsInfo , Dictionary<int, string> conceptsParents, string filePath)
        {
            //report procedures
            string report = "toncrpt ID, toncept NAME, vertical support, mean horizontal support, parents.., grand parents..,\n";
            for (int t = 0; t < top; t++)
            {
                int parentID = 0, grandParentID = 0;
                string conceptParentRecord = "", parents = "", grandParents = "";
                TemporalConcept tc = tonceptsDic.ElementAt(t).Value;
                if (conceptsParents.ContainsKey(tc.tonceptID))
                {
                    conceptParentRecord = conceptsParents[tc.tonceptID];
                    string[] conceptParentRecVec = conceptParentRecord.Split(';');
                    //string[] parentIDVec = conceptParentRecVec[3].Split(';');
                    for (int i = 0; i < conceptParentRecVec.Length - 1; i++)
                    {
                        string[] strVec = conceptParentRecVec[i].Split(','); //[3];
                        parentID = int.Parse(strVec[strVec.Length-1]); // 3]); // conceptParentRecVec[i].Split(',')[3]); //parentIDVec[3]);
                        parents += parentID + ",";
                        if (conceptsInfo.ContainsKey(parentID) && conceptsInfo[parentID].Split(',')[1].Length > 0)
                            parents += conceptsInfo[parentID].Split(',')[1].Replace(";", "") + ",";
                        if (conceptsParents.ContainsKey(parentID))
                        {
                            string[] grandParentsVec = conceptsParents[parentID].Split(';');
                            for (int j = 0; j < grandParentsVec.Length - 1; j++)
                            {
                                strVec = grandParentsVec[j].Split(',');
                                grandParentID = int.Parse(strVec[strVec.Length-1]);// 3]);
                                grandParents += grandParentID + ",";
                                if (conceptsInfo.ContainsKey(grandParentID))
                                    grandParents += conceptsInfo[grandParentID].Split(',')[1].Replace(";", "") + ",";
                            }
                        }
                        else
                            grandParents += "0,";
                        //grandParentNames = conceptsInfo[grandParentID].Split(',')[1];
                    }
                }
                else
                    parents += "0,";
                double mean_hor_sup = tc.getTotalHorizontalSupport();
                mean_hor_sup = mean_hor_sup / tc.getTonceptVerticalSupport();
                report += tc.tonceptID + "," + tc.tonceptName.Replace(",", "_") + "," + tc.getTonceptVerticalSupport() + "," + mean_hor_sup + "," + parents + "\n"; // ",," + grandParents + ",\n";
            }
            writeToFile(filePath, report);
        }

        public static void print_new_cumc_data_analytics( string conditionsFile, string drugsFile, string proceduresFile, string writeTo, string patientsFile, string conceptsFile, string hierarchyFile )
        {
            Dictionary<int, CUMC_entity> entityTISs = new Dictionary<int, CUMC_entity>();
//            if (tonceptParentingFile != null)
//                tonceptParentingDic = read_allenParenToncepts_file(tonceptParentingFile);
            
            Dictionary<int, TemporalConcept> conditionsToncepts = read_cumc_file(conditionsFile, ref entityTISs, '\t', "COND"); //, tonceptParentingDic); // "C:\\robs\\KarmaLegoBook\\DataAndDBSets\\CUMC_DB\\sources\\NEWcolinsConditionsTonceptParenting.csv");
            Dictionary<int, TemporalConcept> drugsToncepts = read_cumc_file(drugsFile, ref entityTISs, '\t', "DRUG");
            Dictionary<int, TemporalConcept> proceduresToncepts = read_cumc_file(proceduresFile, ref entityTISs, '\t', "PROC"); //, tonceptParentingDic); // "C:\\robs\\KarmaLegoBook\\DataAndDBSets\\CUMC_DB\\sources\\NEWcolinsProceduresTonceptParenting.csv");
            
            Dictionary<int, string> patientsInfo = read_file_toIntStringDic(patientsFile);
            Dictionary<int, string> conceptsInfo = read_file_toIntStringDic(conceptsFile);
            Dictionary<int, string> conceptsParents = read_file_toIntStringDic(hierarchyFile);
            
            report_toncepts(proceduresToncepts, 200, conceptsInfo, conceptsParents, writeTo + "\\procReport.csv");//report procedures
            report_toncepts(conditionsToncepts, 200, conceptsInfo, conceptsParents, writeTo + "\\condReport.csv");//report conditions
            report_toncepts(drugsToncepts, 200, conceptsInfo, conceptsParents, writeTo + "\\drugsReport.csv");//report drugs
            
            //patients report
            Dictionary<string, int> gndrDistDic = new Dictionary<string, int>();
            Dictionary<string, int> raceDistDic = new Dictionary<string, int>();
            Dictionary<string, int> ethnDistDic = new Dictionary<string, int>();
            Dictionary<int, int>    ageDistDic = new Dictionary<int, int>();
            ageDistDic.Add(0, 0); ageDistDic.Add(1920, 0); ageDistDic.Add(1930, 0); ageDistDic.Add(1940, 0); ageDistDic.Add(1950, 0); ageDistDic.Add(1960, 0); ageDistDic.Add(1970, 0); ageDistDic.Add(1980, 0); ageDistDic.Add(1990, 0); ageDistDic.Add(2000, 0); ageDistDic.Add(2010, 0);

            string patientStr = "";
            for (int p = 0; p < patientsInfo.Count; p++)
            {
                string[] patientVec = patientsInfo.ElementAt(p).Value.Split(',');
                int patientID = int.Parse(patientVec[0]);
                string gender = patientVec[2];
                if (!gndrDistDic.ContainsKey(gender))
                    gndrDistDic.Add(gender, 1);
                else
                    gndrDistDic[gender]++;
                int birthYear = int.Parse(patientVec[4]);
                if (birthYear == 0)
                    ageDistDic[0]++;
                else
                {
                    for (int i = 1; i < ageDistDic.Count; i++)
                        if (birthYear > ageDistDic.ElementAt(i).Key && birthYear < ageDistDic.ElementAt(i + 1).Key)
                        {
                            ageDistDic[ageDistDic.ElementAt(i).Key]++;
                            break;
                        }
                }
                int race = int.Parse(patientVec[6]);
                if (!raceDistDic.ContainsKey(gender))
                    raceDistDic.Add(gender, 1);
                else
                    raceDistDic[gender]++;
                int ethnicity = int.Parse(patientVec[8]);
                if (!ethnDistDic.ContainsKey(gender))
                    ethnDistDic.Add(gender, 1);
                else
                    ethnDistDic[gender]++;
                patientStr += patientID + "," + gender + "," + birthYear + "," + race + "," + ethnicity + "," + entityTISs[patientID].condTncptNum + "," + entityTISs[patientID].drugTncptNum + "," + entityTISs[patientID].procTncptNum + "," + entityTISs[patientID].tisList.Count + ",\n";
            }
            writeToFile(writeTo + "\\patientsInfo.csv", patientStr);

        }

        private static void print_toncepts(Dictionary<int, TemporalConcept> inTonceptsDic, string printFilePath)
        {
            string stats = "conditionParentID,parentDescription,conditionID,description,cost,verticalSupport,horizontalSupport,\n";
            inTonceptsDic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            for (int i = 0; i < inTonceptsDic.Count; i++)
                stats += inTonceptsDic.ElementAt(i).Value.tonceptID + "," + inTonceptsDic.ElementAt(i).Value.tonceptName.Replace(",", "-") + "," + inTonceptsDic.ElementAt(i).Value.getTonceptVerticalSupport() + "," + inTonceptsDic.ElementAt(i).Value.getTotalHorizontalSupport() + ",\n";
            writeToFile(printFilePath, stats);
        }

        private static ConcurrentDictionary<int, CUMC_entity> read_new_cumc_data_files(int BIGSize, ref Dictionary<int, TemporalConcept> conditionsToncepts, ref Dictionary<int, TemporalConcept> drugsToncepts, ref Dictionary<int, TemporalConcept> proceduresToncepts, string conditionsFile, string drugsFile, string proceduresFile, string tonceptParentingFile, string printFileDirPath, int minEvents)
        {
            Dictionary<int, CUMC_entity> fullEntityTISs = new Dictionary<int, CUMC_entity>(); 
            ConcurrentDictionary<int, CUMC_entity> entityTISs = new ConcurrentDictionary<int,CUMC_entity>();
            //Dictionary<int, CUMC_entity> entityTISs = new Dictionary<int, CUMC_entity>(); 
            Dictionary<int, int> tonceptParentingDic = tonceptParentingDic = read_allenParenToncepts_file(tonceptParentingFile);
            conditionsToncepts = read_cumc_file(conditionsFile, ref fullEntityTISs, '\t', "COND");//, tonceptParentingDic); 
            drugsToncepts = read_cumc_file(drugsFile, ref fullEntityTISs, '\t', "DRUG"); 
            proceduresToncepts = read_cumc_file(proceduresFile, ref fullEntityTISs, '\t', "PROC");//, tonceptParentingDic);

            print_toncepts(proceduresToncepts, printFileDirPath + "\\before_procStats.csv");
            print_toncepts(conditionsToncepts, printFileDirPath + "\\before_condStats.csv");
            
            double SumOfSTIsNum = 0;
            //for (int eIdx = 0; eIdx < fullEntityTISs.Count(); eIdx++)
            Parallel.For(0, fullEntityTISs.Count(), eIdx =>
            {
                CUMC_entity ce = fullEntityTISs.ElementAt(eIdx).Value;
                int tncptsCntr = 0;
                if (ce.condTncpts.getTonceptVerticalSupport() > 0) tncptsCntr++;
                if (ce.drugTncpts.getTonceptVerticalSupport() > 0) tncptsCntr++;
                if (ce.procTncpts.getTonceptVerticalSupport() > 0) tncptsCntr++;

                if (tncptsCntr > BIGSize && ce.tisList.Count > minEvents ) //ce.condTncpts.getTonceptVerticalSupport() > 0 && ce.drugTncpts.getTonceptVerticalSupport() > 0 && ce.procTncpts.getTonceptVerticalSupport() > 0) // && 
                //if(ce.tisList.Count() > minEvents)
                {
                    ce.tisList.Sort();
                    entityTISs.TryAdd(ce.entityID, ce ); // .Add(ce.entityID, ce);
                    SumOfSTIsNum = SumOfSTIsNum + ce.tisList.Count();
                }
                /*else
                {
                    for (int i = 0; i < ce.condTncpts.getTonceptHorizontalDic().Count; i++)
                        conditionsToncepts[ce.condTncpts.getTonceptHorizontalDic().ElementAt(i).Key].getTonceptHorizontalDic().Remove(ce.entityID);
                    for (int i = 0; i < ce.procTncpts.getTonceptHorizontalDic().Count; i++)
                        proceduresToncepts[ce.procTncpts.getTonceptHorizontalDic().ElementAt(i).Key].getTonceptHorizontalDic().Remove(ce.entityID);
                    for (int i = 0; i < ce.drugTncpts.getTonceptHorizontalDic().Count; i++)
                        drugsToncepts[ce.drugTncpts.getTonceptHorizontalDic().ElementAt(i).Key].getTonceptHorizontalDic().Remove(ce.entityID);
                    fullEntityTISs.Remove(ce.entityID);
                    eIdx--;
                  
                }*/
                //if (entityTISs.Count() >= 51657)
                //    break;
            });
            SumOfSTIsNum = SumOfSTIsNum / entityTISs.Count();

            print_toncepts(proceduresToncepts, printFileDirPath + "\\after_procStats.csv");
            print_toncepts(conditionsToncepts, printFileDirPath + "\\after_condStats.csv");

            return entityTISs;
            
        }

        public static void create_abstracted_datasets_from_folder(string filesFolder, string conditionsMap, string proceduresMap, int abstractLevel)
        {
            
            Dictionary<int, string> conditionsMapDic = read_file_toIntStringDic(conditionsMap);
            Dictionary<int, string> proceduresMapDic = read_file_toIntStringDic(proceduresMap);

            Directory.CreateDirectory(filesFolder.Replace("sourceData", "abstractSourceData"));
      
            //Parallel.ForEach(Directory.GetDirectories(filesFolder), matrixFilePath =>
            foreach(string matrixFilePath in Directory.GetFiles(filesFolder))
            {
                if (matrixFilePath.Contains("OFILE") && matrixFilePath.Contains(".csv"))
                {
                    create_abstracted_dataset(matrixFilePath, conditionsMapDic, proceduresMapDic);
                    create_abstracted_dataset(matrixFilePath.Replace("OFILE", "OtherFILE"), conditionsMapDic, proceduresMapDic);
                }
            }//);
        }

        public static void create_abstracted_dataset(string filePath, Dictionary<int, string> conditionsMapDic, Dictionary<int, string> proceduresMapDic) //, int abstractLevel, string rplcStrategy /* all OR first*/)
        {
            string abstractFilePath = filePath.Replace("sourceData", "abstractSourceData");
            TimeIntervalSymbol sti = new TimeIntervalSymbol(0,0,0);
            Dictionary<int, TemporalConcept> toncepts = new Dictionary<int,TemporalConcept>();
            string abstractRecord = "";
            Dictionary<int, List<TimeIntervalSymbol>> entityTISs = new Dictionary<int,List<TimeIntervalSymbol>>();
            KarmE.read_tids_file(0, KLC.NUM_OF_ENTITIES, filePath, ref entityTISs, ref toncepts);
            string stiFile = "";
            for (int e = 0; e < entityTISs.Count(); e++)
            {
                int absCnt = 0, rmvCnt = 0;
                List<TimeIntervalSymbol> stiList = entityTISs.ElementAt(e).Value;
                int stiListSize = stiList.Count;
                List<TimeIntervalSymbol> addedStiList = new List<TimeIntervalSymbol>();
                for (int stIdx = 0; stIdx < stiList.Count(); stIdx++)
                {
                    sti = stiList[stIdx];
                    List<string[]> parentsVectorsList = new List<string[]>();
                    if (conditionsMapDic.ContainsKey(sti.symbol))
                        abstractRecord = conditionsMapDic[sti.symbol];//.Split(',')[1];
                    else if (proceduresMapDic.ContainsKey(sti.symbol))
                        abstractRecord = proceduresMapDic[sti.symbol];//.Split(',')[1];
                    string[] abstractsVec = abstractRecord.Split(';');
                    List<int> abstractIds = new List<int>();
                    for (int a = 0; a < abstractsVec.Count()-1; a++)
                    {
                        string abs = abstractsVec[a].Split(',')[1];
                        if (abs.Contains('.'))
                            abs = abs.Split('.')[0];
                        if (abs.Contains('D'))
                            abs = abs.Replace("D", "400");
                        if (abs.Contains('E'))
                            abs = abs.Replace("E", "500");
                        if (abs.Contains('V'))
                            abs = abs.Replace("V", "2200");
                        if (abs.Contains('G'))
                            abs = abs.Replace("G", "700");
                        if (!abstractIds.Contains(int.Parse(abs)))
                            abstractIds.Add(int.Parse(abs));
                    }
                    if (abstractIds.Count == 1)
                        sti.symbol = abstractIds[0];
                    else if(abstractIds.Count > 1)
                    {
                        for (int l = 0; l < abstractIds.Count(); l++)
                        {
                            TimeIntervalSymbol new_sti = new TimeIntervalSymbol(sti.startTime, sti.endTime, sti.symbol);
                            addedStiList.Add(new_sti);
                            absCnt++;
                        }
                        stiList.Remove(sti);
                        stIdx--;
                        rmvCnt++;
                    }
                    abstractRecord = "";
                }
                for (int ad = 0; ad < addedStiList.Count; ad++)
                    stiList.Add(addedStiList[ad]);
                if (stiList.Count < stiListSize)
                    stiListSize = stiListSize;
                stiList.Sort();
                string entitySTIstring = "";
                for(int ti = 0; ti < stiList.Count(); ti++)
                    entitySTIstring += stiList.ElementAt(ti).startTime + "," + stiList.ElementAt(ti).endTime + "," + stiList.ElementAt(ti).symbol + ";";
                entitySTIstring += "\n";
                stiFile += entityTISs.ElementAt(e).Key + "," + e + ";\n" + entitySTIstring;
            }
            writeToFile(abstractFilePath, "\n" + "startToncepts\n" + "numberOfEntities," + entityTISs.Count() + "\n" + stiFile);
        }

        public static double calculateMeanStDev(ref double std, List<int> items)
        {
            double mean = 0; //, std = 0;
            for (int x = 0; x < items.Count; x++)
                mean = mean + items[x];
            mean = mean / items.Count;

            for (int x = 0; x < items.Count; x++)
                std = std + Math.Pow((items[x] - mean), 2);
            std = std / items.Count;
            std = Math.Sqrt(std);

            return mean;
        }

        public static double calculateMeanStDev(ref double std, List<double> items)
        {
            double mean = 0; //, std = 0;
            for (int x = 0; x < items.Count; x++)
                mean = mean + items[x];
            mean = mean / items.Count;

            for (int x = 0; x < items.Count; x++)
                std = std + Math.Pow((items[x] - mean), 2);
            std = std / items.Count;
            std = Math.Sqrt(std);

            return mean;
        }

        public static void create_FAST_TTI_Files(int BIGSize, int minObservationTime, int observationTime, int predictionTime, int minEvents, int minToEvent, string printFileDirPAth, string conditionsFile, string drugsFile, string proceduresFile, string patientDemographicFile, string tonceptParentingFile, string procCostsFile, string condCostsFile, string procOrCond, bool timeAndEvents, string abstract_conditions_, string abstract_procedures_)
        {
            // REMEMBER TO CHECK THE MIN_TO_EVENT WHATS THE RIGHT VALUE

            Directory.CreateDirectory(printFileDirPAth);
            Dictionary<int, TemporalConcept> conditionsToncepts = new Dictionary<int, TemporalConcept>(), drugsToncepts = new Dictionary<int, TemporalConcept>(), proceduresToncepts = new Dictionary<int, TemporalConcept>();
            Dictionary<int, string> tonceptParentingDic = read_dbParenToncepts_file(tonceptParentingFile); // read_allenParenToncepts_file(tonceptParentingFile);
            Dictionary<int, string> tonceptsDic = read_file_toIntStringDic(tonceptParentingFile);
            Dictionary<int, string> tonceptProcCosts = read_file_toIntStringDic(procCostsFile);
            Dictionary<int, string> tonceptCondCosts = read_file_toIntStringDic(condCostsFile);
            Dictionary<int, string> patientDemographics = read_file_toIntStringDic(patientDemographicFile);
            //Dictionary<int, CUMC_entity> entityTISs = read_new_cumc_data_files(ref conditionsToncepts, ref drugsToncepts, ref proceduresToncepts, conditionsFile, drugsFile, proceduresFile, tonceptParentingFile, printFileDirPAth, minEvents);
            ConcurrentDictionary<int, CUMC_entity> entityTISs = read_new_cumc_data_files(BIGSize, ref conditionsToncepts, ref drugsToncepts, ref proceduresToncepts, conditionsFile, drugsFile, proceduresFile, tonceptParentingFile, printFileDirPAth, minToEvent);
            List<int> eventSizeMetrics = new List<int>();
            List<int> overallEventSizeMetrics = new List<int>();
            List<int> observationPeriods = new List<int>();
            //int[] outcomeProcs = { 2514435, 2514433, 2001544, 2008237, 2002703, 42738972, 2008267, 2006918, 2002688, 2314026, 2006634, 2007901, 2007079, 2006993, 2000173, 2001505, 2008337, 2008009, 2008271, 2008264, 2008264 };

            ConcurrentBag<string> concurrentList = new ConcurrentBag<string>();

            string report = "tonceptID,tonceptName,verticalSupport,allInstances,cohortSize,meanToEvent,cost,Female,Male,Unknown,Other,White,Other,Unknown,Asian,Black,Hispanic or Latino,Not Hispanic or Latino,Unknown,chf,diabetes,highCholesterol,parents,\n";
            //float meanToEvent = 0, meanOverallEvents = 0, meanObservations = 0;
            Dictionary<int, TemporalConcept> predictedToncepts;
            if (procOrCond == "procedures")
                predictedToncepts = proceduresToncepts; // conditionsToncepts; // 
            else
                predictedToncepts = conditionsToncepts;
            Dictionary<int, string> tonceptCosts;
            if (procOrCond == "procedures")
                tonceptCosts = tonceptProcCosts;
            else
                tonceptCosts = tonceptCondCosts;

            Directory.CreateDirectory(printFileDirPAth + "\\sourceData");
            predictedToncepts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            for (int i = 0; i < predictedToncepts.Count(); i++) // run on all concepts
            //Parallel.For(0, predictedToncepts.Count(), i =>
            {
                string tonceptReport = createTonceptDataset(predictedToncepts.ElementAt(i).Value, predictedToncepts, tonceptCosts, entityTISs, procOrCond, minObservationTime, observationTime, predictionTime, minToEvent, tonceptParentingDic, patientDemographics, tonceptsDic, printFileDirPAth + "\\sourceData", timeAndEvents);
                concurrentList.Add(tonceptReport);
            } //);

            int totalHighChol = 0, totalChf = 0, totalDiabetes = 0;
            Dictionary<string, int> totalGenderDis = new Dictionary<string, int>(), totalRaceDis = new Dictionary<string, int>(), totalEthDis = new Dictionary<string, int>();
            totalGenderDis.Add("F", 0); totalGenderDis.Add("M", 0); totalGenderDis.Add("U", 0); totalGenderDis.Add("O", 0);//genderDis.Add("X",0);genderDis.Add("I",0);
            totalRaceDis.Add("W", 0); totalRaceDis.Add("O", 0); totalRaceDis.Add("U", 0); totalRaceDis.Add("A", 0); totalRaceDis.Add("B", 0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
            totalEthDis.Add("N", 0); totalEthDis.Add("U", 0); totalEthDis.Add("H", 0); //ethDis.Add("D",0); 
            for (int eIdx = 0; eIdx < entityTISs.Count; eIdx++)
            {
                CUMC_entity ce = entityTISs.ElementAt(eIdx).Value;
                string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                if (totalGenderDis.ContainsKey(patientDemoVec[11]))
                    totalGenderDis[patientDemoVec[11]]++;//gender
                if (totalRaceDis.ContainsKey(patientDemoVec[12]))
                    totalRaceDis[patientDemoVec[12]]++; //race
                if (totalEthDis.ContainsKey(patientDemoVec[13]))
                    totalEthDis[patientDemoVec[13]]++; //ethnicity

                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                    totalHighChol++;
                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                    totalChf++;
                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                    totalDiabetes++;

            }
            string totalDistributions = "";
            for (int d = 0; d < totalGenderDis.Count(); d++)
                totalDistributions += (double)totalGenderDis.ElementAt(d).Value / (double)entityTISs.Count + ",";
            for (int d = 0; d < totalRaceDis.Count(); d++)
                totalDistributions += (double)totalRaceDis.ElementAt(d).Value / (double)entityTISs.Count + ",";
            for (int d = 0; d < totalEthDis.Count(); d++)
                totalDistributions += (double)totalEthDis.ElementAt(d).Value / (double)entityTISs.Count + ",";

            for (int i = 0; i < concurrentList.Count; i++)
                report += concurrentList.ElementAt(i);// +"\n";
            report += "total,,,,,,," + totalDistributions + "," + (double)totalChf / (double)entityTISs.Count + "," + (double)totalDiabetes / (double)entityTISs.Count + "," + (double)totalHighChol / (double)entityTISs.Count + ",,\n";
            writeToFile(printFileDirPAth + "\\" +  procOrCond + "Table_.csv", report);

            create_abstracted_datasets_from_folder(printFileDirPAth + "\\sourceData", abstract_conditions_, abstract_procedures_, 1);
            
        
        }

        public static string checkEntityRecords(CUMC_entity ce, TemporalConcept tcPtr, int tonceptId, int minObservationTime, int observationTime, int predictionTime, int minToEvent )
        {
            string returnString = "";
            //CUMC_entity ce = entityTISs[toncept.getTonceptHorizontalDic().ElementAt(e).Key]; // eId];
            int latestObservTiIdx = 0, earlyObservTiIdx = 0, timeToPredict = 0; ce.entityLine = "";
             
            // double observation = 0, observationPlusOne = 0;
            if (tcPtr.getTonceptHorizontalDic().ContainsKey(tonceptId)) //.tonceptID))// if the patient has the outcome event
            {
                for (int trgTncptIdx = 0; trgTncptIdx < ce.tisList.Count; trgTncptIdx++)
                {
                    if (ce.tisList[trgTncptIdx].symbol == tonceptId) //.tonceptID) // if the outcome event was found
                    {
                        //int trgTncptIdx = tiIdx;
                        timeToPredict = ce.tisList[trgTncptIdx].startTime - predictionTime;
                        for (latestObservTiIdx = trgTncptIdx; latestObservTiIdx > 0; latestObservTiIdx--)
                            if (ce.tisList[trgTncptIdx].startTime - ce.tisList[latestObservTiIdx].endTime > predictionTime) // if prediction time exceeded
                                break;
                        int earliesTime = ce.tisList[trgTncptIdx].startTime - (observationTime + predictionTime);
                        for (earlyObservTiIdx = latestObservTiIdx; earlyObservTiIdx > -1; earlyObservTiIdx--)
                            if (ce.tisList[earlyObservTiIdx].endTime < earliesTime)
                                break;
                        earlyObservTiIdx++;
                        break;
                    }
                }

                int observation = ce.tisList[latestObservTiIdx].startTime - ce.tisList[earlyObservTiIdx].startTime;
                if ((latestObservTiIdx - earlyObservTiIdx) > minToEvent && observation > minObservationTime && observation <= observationTime) // minE
                {
                    //ce.entityLine = "";
                    returnString = ce.entityID + "," + timeToPredict + "," + latestObservTiIdx + "," + earlyObservTiIdx + "-";
                    //returnString += timeToPredict.ToString() + ";"; 
                    for (int tiIdx = earlyObservTiIdx + 1; tiIdx <= latestObservTiIdx; tiIdx++)
                        returnString += ce.tisList.ElementAt(tiIdx).startTime + "," + ce.tisList.ElementAt(tiIdx).endTime + "," + ce.tisList.ElementAt(tiIdx).symbol + ";";
                    returnString += timeToPredict.ToString() + "," + (timeToPredict+1).ToString() + "," + "11331133;"; 
                    returnString += "\n";
                }
            }
            return returnString;
        }

        public static string createTonceptDataset(TemporalConcept toncept, Dictionary<int, TemporalConcept> predictedToncepts, Dictionary<int, string> tonceptCosts, ConcurrentDictionary<int, CUMC_entity> entityTISs, string procOrCond, int minObservationTime, int observationTime, int predictionTime, int minToEvent, Dictionary<int, string> tonceptParentingDic, Dictionary<int, string> patientDemographics, Dictionary<int, string> tonceptsDic, string printFileDirPAth, bool timeAndEvents )
        {
            string report = "";
            List<int> eventSizeMetrics = new List<int>(); List<int> overallEventSizeMetrics = new List<int>(); List<int> observationPeriods = new List<int>();
            //eventSizeMetrics.Clear(); overallEventSizeMetrics.Clear(); observationPeriods.Clear();
            Dictionary<int, float> procCentroidDic = new Dictionary<int, float>(); Dictionary<int, float> drugCentroidDic = new Dictionary<int, float>(); Dictionary<int, float> condCentroidDic = new Dictionary<int, float>();
            double meanToEvent = 0, meanOverallEvents = 0, meanObservations = 0;
            //TemporalConcept toncept = predictedToncepts.ElementAt(i).Value;
            
            //string cosTest = "noCost";
            //if (tonceptCosts.ContainsKey(toncept.tonceptID))
            //        cosTest = tonceptCosts[toncept.tonceptID].Split(',')[2];

            //print examples
            string tisStr = "";
            int cohortCount = 0; float chf = 0, diabetes = 0, highChol = 0;
            Dictionary<string, int> genderDis = new Dictionary<string, int>(), raceDis = new Dictionary<string, int>(), ethDis = new Dictionary<string, int>();
            genderDis.Add("F", 0); genderDis.Add("M", 0); genderDis.Add("U", 0); genderDis.Add("O", 0);//genderDis.Add("X",0);genderDis.Add("I",0);
            raceDis.Add("W", 0); raceDis.Add("O", 0); raceDis.Add("U", 0); raceDis.Add("A", 0); raceDis.Add("B", 0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
            ethDis.Add("N", 0); ethDis.Add("U", 0); ethDis.Add("H", 0); //ethDis.Add("D",0); 

            ConcurrentBag<string> concurrentBagStrings = new ConcurrentBag<string>();

            //for (int e = 0; e < toncept.getTonceptHorizontalDic().Count(); e++) // all the patients having the outcome
            Parallel.For(0, toncept.getTonceptHorizontalDic().Count(), e =>
            {
                //int eId = toncept.getTonceptHorizontalDic().ElementAt(e).Key;
                if (entityTISs.ContainsKey(toncept.getTonceptHorizontalDic().ElementAt(e).Key)) // eId))
                {
                    CUMC_entity ce = entityTISs[toncept.getTonceptHorizontalDic().ElementAt(e).Key]; // eId];

                    //int latestObservTiIdx = 0, earlyObservTiIdx = 0; ce.entityLine = "";

                    TemporalConcept tcPtr;
                    if (procOrCond == "procedures")
                        tcPtr = ce.procTncpts;
                    else
                        tcPtr = ce.condTncpts;
                    string entString = checkEntityRecords(ce, tcPtr, toncept.tonceptID, minObservationTime, observationTime, predictionTime, minToEvent);
                    if( entString != "" )
                        concurrentBagStrings.Add(entString);
                }
            });

            for(int i = 0; i < concurrentBagStrings.Count; i++)
            {
                string conStrings   = concurrentBagStrings.ElementAt(i);
                if (conStrings != "")
                {
                    string[] stringVec = conStrings.Split('-');
                    string[] items = stringVec[0].Split(',');
                    CUMC_entity ce = entityTISs[int.Parse(items[0])]; // eId];

                    addTonceptsToTonceptsHSCounter(ref procCentroidDic, ce.procTncpts);
                    addTonceptsToTonceptsHSCounter(ref drugCentroidDic, ce.drugTncpts);
                    addTonceptsToTonceptsHSCounter(ref condCentroidDic, ce.condTncpts);

                    string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                    if (genderDis.ContainsKey(patientDemoVec[11])) genderDis[patientDemoVec[11]]++; //gender
                    if (raceDis.ContainsKey(patientDemoVec[12])) raceDis[patientDemoVec[12]]++;     //race
                    if (ethDis.ContainsKey(patientDemoVec[13])) ethDis[patientDemoVec[13]]++;       //ethnicity

                    if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                        highChol++;
                    if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                        chf++;
                    if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                        diabetes++;
                    int latestObservTiIdx = int.Parse(items[2/*1*/]), earlyObservTiIdx = int.Parse(items[3/*2*/]);
                    meanToEvent = meanToEvent + (latestObservTiIdx - earlyObservTiIdx);
                    meanOverallEvents = meanOverallEvents + ce.tisList.Count;
                    meanObservations = meanObservations + (ce.tisList[latestObservTiIdx].endTime - ce.tisList[earlyObservTiIdx].startTime);

                    eventSizeMetrics.Add((latestObservTiIdx - earlyObservTiIdx));
                    overallEventSizeMetrics.Add(ce.tisList.Count);
                    observationPeriods.Add((ce.tisList[latestObservTiIdx].endTime - ce.tisList[earlyObservTiIdx].startTime));

                    tisStr += ce.entityID + "," + cohortCount++ + ";\n" + stringVec[1]; //ce.entityLine;
                }
            }
            
            string distributions = "";
            for (int d = 0; d < genderDis.Count(); d++)
                distributions += (double)genderDis.ElementAt(d).Value / (double)cohortCount + ",";
            for (int d = 0; d < raceDis.Count(); d++)
                distributions += (double)raceDis.ElementAt(d).Value / (double)cohortCount + ",";
            for (int d = 0; d < ethDis.Count(); d++)
                distributions += (double)ethDis.ElementAt(d).Value / (double)cohortCount + ",";

            if (cohortCount > 500)
            {
                // calculate mean/std metrics 
                meanToEvent = meanToEvent / cohortCount; 
                double mean = 0, std = 0; 
                //for (int x = 0; x < eventSizeMetrics.Count; x++) mean = mean + eventSizeMetrics[x]; mean = mean / eventSizeMetrics.Count; for (int x = 0; x < eventSizeMetrics.Count; x++) std = std + Math.Pow((eventSizeMetrics[x] - mean), 2); std = std / eventSizeMetrics.Count; std = Math.Sqrt(std);

                mean = calculateMeanStDev(ref std, eventSizeMetrics);

                double minMeanToEven = mean - (0.5 * std);
                
                Random _r = new Random();

                meanOverallEvents = meanOverallEvents / cohortCount;
                double overallMean = 0, overallStd = 0;
                //for (int x = 0; x < overallEventSizeMetrics.Count; x++) overallMean = overallMean + overallEventSizeMetrics[x]; overallMean = overallMean / overallEventSizeMetrics.Count; for (int x = 0; x < overallEventSizeMetrics.Count; x++) overallStd = overallStd + Math.Pow((overallEventSizeMetrics[x] - overallMean), 2); overallStd = overallStd / overallEventSizeMetrics.Count; overallStd = Math.Sqrt(overallStd); overallMean = calculateMeanStDev(ref overallStd, overallEventSizeMetrics); double minOverallMeanToEven = overallMean - (0.75 * overallStd);

                meanObservations = meanObservations / cohortCount;
                double observMean = 0, observStd = 0;
                //for (int x = 0; x < observationPeriods.Count; x++) observMean = observMean + observationPeriods[x]; observMean = observMean / observationPeriods.Count; for (int x = 0; x < observationPeriods.Count; x++) observStd = observStd + Math.Pow((observationPeriods[x] - observMean), 2); observStd = observStd / observationPeriods.Count; observStd = Math.Sqrt(observStd);

                observMean = calculateMeanStDev(ref observStd, observationPeriods);

                //double minMeanObserv = observMean - (0.75 * observStd);
                    
                string cost = "noCost";
                if (tonceptCosts.ContainsKey(toncept.tonceptID))
                    cost = tonceptCosts[toncept.tonceptID].Split(',')[2];
                string parents = "noParents";
                if (tonceptParentingDic.ContainsKey(toncept.tonceptID))
                {
                    parents = tonceptParentingDic[toncept.tonceptID];
                    string[] parentsVec = parents.Split(',');
                    for (int p = 0; p < parentsVec.Count() - 1; p++)
                    {
                        int parentId = int.Parse(parentsVec[p]);
                        if (tonceptsDic.ContainsKey(parentId)) //if (predictedToncepts.ContainsKey(parentId))
                        {
                            string[] parentsPropVec = tonceptsDic[parentId].Split(',');
                            parents += "(" + parentsPropVec[2] + ")" + parentsPropVec[1] + ",";  //    parents += predictedToncepts[parentId].tonceptName + ",";
                        }
                    }
                }
                //calculate cohort HS centroid
                /*    for (int c = 0; c < procCentroidDic.Count(); c++)   procCentroidDic[procCentroidDic.ElementAt(c).Key] = (float)(procCentroidDic.ElementAt(c).Value / (float)cohortCount);
                    for (int c = 0; c < drugCentroidDic.Count(); c++)   drugCentroidDic[drugCentroidDic.ElementAt(c).Key] = (float)(drugCentroidDic.ElementAt(c).Value / (float)cohortCount);
                    for (int c = 0; c < condCentroidDic.Count(); c++)   condCentroidDic[condCentroidDic.ElementAt(c).Key] = (float)(condCentroidDic.ElementAt(c).Value / (float)cohortCount);
                */

                List<CUMC_entity> controls = new List<CUMC_entity>();
                List<int> control_ids = new List<int>();
                double controlMeanToEvent = 0;

                while (controls.Count < cohortCount)
                {
                    int random_idx = _r.Next(0, entityTISs.Count);
                    CUMC_entity r_ce = entityTISs.ElementAt(random_idx).Value;
                    if (r_ce.tisList.Count > minToEvent && !control_ids.Contains(r_ce.entityID) && !toncept.getTonceptHorizontalDic().ContainsKey(r_ce.entityID))
                    {
                        for (int tIdx = minToEvent; tIdx < r_ce.tisList.Count; tIdx++)
                        {
                            if (predictedToncepts.ContainsKey(r_ce.tisList[tIdx].symbol))
                            {
                                int latestObservTiIdx = 0, trgTncptIdx = tIdx;
                                for (latestObservTiIdx = trgTncptIdx; latestObservTiIdx > 0; latestObservTiIdx--)
                                    if (r_ce.tisList[trgTncptIdx].startTime - r_ce.tisList[latestObservTiIdx].endTime > predictionTime) // if prediction time exceeded
                                        break;
                                int timeToPredict = r_ce.tisList[trgTncptIdx].startTime - predictionTime; // time to Predict

                                int earliesTime = r_ce.tisList[trgTncptIdx].startTime - (observationTime + predictionTime);
                                for (int earlyObservTiIdx = latestObservTiIdx; earlyObservTiIdx > -1; earlyObservTiIdx--)
                                {
                                    int observation = r_ce.tisList[latestObservTiIdx].startTime - r_ce.tisList[earlyObservTiIdx].startTime;
                                    if (r_ce.tisList[earlyObservTiIdx].endTime < earliesTime && (latestObservTiIdx - earlyObservTiIdx) > minToEvent && observation > minObservationTime && observation <= observationTime)
                                    {
                                        //if (latestObservTiIdx - earlyObservTiIdx > minToEvent)
                                        {
                                            r_ce.entityLine = "";
                                            earlyObservTiIdx++;
                                            for (int ti = earlyObservTiIdx; ti < latestObservTiIdx; ti++)
                                                r_ce.entityLine += r_ce.tisList.ElementAt(ti).startTime + "," + r_ce.tisList.ElementAt(ti).endTime + "," + r_ce.tisList.ElementAt(ti).symbol + ";";
                                            controlMeanToEvent = controlMeanToEvent + (latestObservTiIdx - earlyObservTiIdx);
                                            r_ce.entityLine += timeToPredict.ToString() + "," + (timeToPredict+1).ToString() + ",11331133;"; //
                                            r_ce.entityLine += "\n";
                                            controls.Add(r_ce);
                                            control_ids.Add(r_ce.entityID);
                                        }
                                        tIdx = r_ce.tisList.Count;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                                
                    Dictionary<string, int> cGenderDis = new Dictionary<string, int>(), cRaceDis = new Dictionary<string, int>(), cEthDis = new Dictionary<string, int>();
                    cGenderDis.Add("F", 0); cGenderDis.Add("M", 0); cGenderDis.Add("U", 0); cGenderDis.Add("O", 0);
                    cRaceDis.Add("W", 0); cRaceDis.Add("O", 0); cRaceDis.Add("U", 0); cRaceDis.Add("A", 0); cRaceDis.Add("B", 0);
                    cEthDis.Add("N", 0); cEthDis.Add("U", 0); cEthDis.Add("H", 0);
                    int controlCount = 0;
                    //double controlMeanToEvent = 0;
                    string controlTisStr = "";
                    float controlChf = 0, controlDiabetes = 0, controlHighChol = 0;
                    
                    //for ( int eIdx = controls.Count-1; eIdx > -1; eIdx--)  //
                    for( int eIdx = 0; eIdx < controls.Count; eIdx++)
                    {
                        CUMC_entity ce = controls.ElementAt(eIdx);
                        controlTisStr += ce.entityID + "," + controlCount++ + ";\n" + ce.entityLine;
                        
                        string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                        if (cGenderDis.ContainsKey(patientDemoVec[11]))
                            cGenderDis[patientDemoVec[11]]++;//gender
                        if (cRaceDis.ContainsKey(patientDemoVec[12]))
                            cRaceDis[patientDemoVec[12]]++; //race
                        if (cEthDis.ContainsKey(patientDemoVec[13]))
                            cEthDis[patientDemoVec[13]]++; //ethnicity

                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                            controlHighChol++;
                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                            controlChf++;
                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                            controlDiabetes++;
                    }

                    if (controlCount == cohortCount)
                    {
                        controlMeanToEvent = controlMeanToEvent / cohortCount; // otherCount;
                        string controlDistributions = "";
                        for (int d = 0; d < cGenderDis.Count(); d++)
                            controlDistributions += (double)cGenderDis.ElementAt(d).Value / (double)cohortCount + ",";
                        for (int d = 0; d < cRaceDis.Count(); d++)
                            controlDistributions += (double)cRaceDis.ElementAt(d).Value / (double)cohortCount + ",";
                        for (int d = 0; d < cEthDis.Count(); d++)
                            controlDistributions += (double)cEthDis.ElementAt(d).Value / (double)cohortCount + ",";

                        report = toncept.tonceptID + "," + toncept.tonceptName.Replace(",", "_") + "," + toncept.getTonceptVerticalSupport() + "," + toncept.getTotalHorizontalSupport() + "," + cohortCount + "," + meanToEvent + "," + cost + "," + distributions + "," + (double)chf / (double)cohortCount + "," + (double)diabetes / (double)cohortCount + "," + (double)highChol / (double)cohortCount + ",,\n";
                        report += "control,,,," + cohortCount + "," + controlMeanToEvent + ",," + controlDistributions + "," + (double)controlChf / (double)cohortCount + "," + (double)controlDiabetes / (double)cohortCount + "," + (double)controlHighChol / (double)cohortCount + "," + parents + ",\n";
                        //writeToFile(printFileDirPAth + "\\proceduresTable_.csv", report);
                        if (tisStr.Count() > 0)
                        {
                            string fileName = toncept.tonceptID + "_oT" + observationTime + "_pT" + predictionTime + /*"_mE" + minEvents +*/ "_mTE" + minToEvent + "_" + cohortCount + "_" + (int)mean/*ToEvent*/ + "_" + (int)controlMeanToEvent + "_OFILE.csv";
                            if (fileName.Length > 250) fileName = fileName.Remove(250);
                            writeToFile(printFileDirPAth + "\\" + fileName, "\n" + "startToncepts\n" + "numberOfEntities," + cohortCount.ToString() + "\n" + tisStr);

                            fileName = fileName.Replace("OFILE", "OtherFILE");
                            writeToFile(printFileDirPAth + "\\" + fileName, "\n" + "startToncepts\n" + "numberOfEntities," + controlCount.ToString() + "\n" + controlTisStr);
                            //break;
                        }
                    }
                }
                return report;
        }//);

        public static void cosineSimilarity(TemporalConcept eTypeTncpts, Dictionary<int, float> typeCentroidDic, ref double cosineDenominator, ref double SumA2, ref double SumB2)
        {
            for (int t = 0; t < eTypeTncpts.getTonceptVerticalSupport(); t++)
            {
                if (typeCentroidDic.ContainsKey(eTypeTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                {// intersection++;
                    float centVal = typeCentroidDic[eTypeTncpts.getTonceptHorizontalDic().ElementAt(t).Key];
                    SumA2 += Math.Pow(centVal, 2);
                    int ceTnptCOunt = eTypeTncpts.getTonceptHorizontalDic().ElementAt(t).Value.Count();
                    SumB2 += Math.Pow(ceTnptCOunt, 2);
                    cosineDenominator += centVal * ceTnptCOunt;
                }
            }
        }
        
/*        public static void create_CosineBased_TTI_Files(int observationTime, int predictionTime, int minEvents, int minToEvent, string printFileDirPAth, string conditionsFile, string drugsFile, string proceduresFile, string patientDemographicFile, string tonceptParentingFile, string procCostsFile, string condCostsFile, string procOrCond)
        {
            // REMEMBER TO CHECK THE MIN_TO_EVENT WHATS THE RIGHT VALUE

            Directory.CreateDirectory(printFileDirPAth);
            Dictionary<int, TemporalConcept> conditionsToncepts = new Dictionary<int, TemporalConcept>(), drugsToncepts = new Dictionary<int, TemporalConcept>(), proceduresToncepts = new Dictionary<int, TemporalConcept>();
            Dictionary<int, string> tonceptParentingDic = read_dbParenToncepts_file(tonceptParentingFile); // read_allenParenToncepts_file(tonceptParentingFile);
            Dictionary<int, string> tonceptsDic = read_file_toIntStringDic(tonceptParentingFile);
            Dictionary<int, string> tonceptProcCosts = read_file_toIntStringDic(procCostsFile);
            Dictionary<int, string> tonceptCondCosts = read_file_toIntStringDic(condCostsFile);
            Dictionary<int, string> patientDemographics = read_file_toIntStringDic(patientDemographicFile);
            Dictionary<int, CUMC_entity> entityTISs = read_new_cumc_data_files(ref conditionsToncepts, ref drugsToncepts, ref proceduresToncepts, conditionsFile, drugsFile, proceduresFile, tonceptParentingFile, printFileDirPAth, minEvents);
            List<int> eventSizeMetrics = new List<int>();
            //int[] outcomeProcs = { 2514435, 2514433, 2001544, 2008237, 2002703, 42738972, 2008267, 2006918, 2002688, 2314026, 2006634, 2007901, 2007079, 2006993, 2000173, 2001505, 2008337, 2008009, 2008271, 2008264, 2008264 };

            string report = "tonceptID,tonceptName,verticalSupport,allInstances,cohortSize,meanToEvent,cost,Female,Male,Unknown,Other,White,Other,Unknown,Asian,Black,Hispanic or Latino,Not Hispanic or Latino,Unknown,chf,diabetes,highCholesterol,parents,\n";
            float meanToEvent = 0;
            Dictionary<int, TemporalConcept> predictedToncepts;
            if (procOrCond == "procedures")
                predictedToncepts = proceduresToncepts; // conditionsToncepts; // 
            else
                predictedToncepts = conditionsToncepts;
            Dictionary<int, string> tonceptCosts;
            if (procOrCond == "procedures")
                tonceptCosts = tonceptProcCosts;
            else
                tonceptCosts = tonceptCondCosts;

            predictedToncepts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            for (int i = 0; i < predictedToncepts.Count(); i++)
            //Parallel.For(0, predictedToncepts.Count(), i =>
            {
                eventSizeMetrics.Clear();
                Dictionary<int, float> procCentroidDic = new Dictionary<int, float>(); Dictionary<int, float> drugCentroidDic = new Dictionary<int, float>(); Dictionary<int, float> condCentroidDic = new Dictionary<int, float>();
                meanToEvent = 0;
                TemporalConcept toncept = predictedToncepts.ElementAt(i).Value;

                string cosTest = "noCost";
                if (tonceptCosts.ContainsKey(toncept.tonceptID))
                    cosTest = tonceptCosts[toncept.tonceptID].Split(',')[2];

                //print examples
                string tisStr = "";
                int count = 0; float chf = 0, diabetes = 0, highChol = 0;
                Dictionary<string, int> genderDis = new Dictionary<string, int>(), raceDis = new Dictionary<string, int>(), ethDis = new Dictionary<string, int>();
                genderDis.Add("F", 0); genderDis.Add("M", 0); genderDis.Add("U", 0); genderDis.Add("O", 0);//genderDis.Add("X",0);genderDis.Add("I",0);
                raceDis.Add("W", 0); raceDis.Add("O", 0); raceDis.Add("U", 0); raceDis.Add("A", 0); raceDis.Add("B", 0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
                ethDis.Add("N", 0); ethDis.Add("U", 0); ethDis.Add("H", 0); //ethDis.Add("D",0); 

                for (int e = 0; e < toncept.getTonceptHorizontalDic().Count(); e++)
                {
                    int eId = toncept.getTonceptHorizontalDic().ElementAt(e).Key;
                    if (entityTISs.ContainsKey(eId))
                    {
                        CUMC_entity ce = entityTISs[eId];
                        int tiIdx = 0, predTiIdx = 0, strtObservTiIdx = 0; ce.entityLine = "";

                        TemporalConcept tcPtr;
                        if (procOrCond == "procedures")
                            tcPtr = ce.procTncpts;
                        else
                            tcPtr = ce.condTncpts;

                        double observation = 0, observationPlusOne = 0;
                        //if (ce.procTncpts.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID))
                        //if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID)) // if the patient has the outcome event
                        if (tcPtr.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID))
                        {
                            for (tiIdx = 0; tiIdx < ce.tisList.Count; tiIdx++)
                            {
                                if (ce.tisList[tiIdx].symbol == toncept.tonceptID) // if the outcome event was found
                                {
                                    int trgTncptIdx = tiIdx;
                                    for (predTiIdx = tiIdx; predTiIdx > -1; predTiIdx--)
                                        if (ce.tisList[tiIdx].startTime - ce.tisList[predTiIdx].endTime > predictionTime) // if prediction time exceeded
                                            break;
                                    int earlyTime = ce.tisList[tiIdx].startTime - (observationTime + predictionTime);
                                    for (strtObservTiIdx = predTiIdx; strtObservTiIdx > -1; strtObservTiIdx--)
                                    {
                                        observation = ce.tisList[tiIdx].startTime - ce.tisList[strtObservTiIdx].endTime;
                                        observationPlusOne = ce.tisList[tiIdx].startTime - ce.tisList[strtObservTiIdx+1].endTime;
                                        //if ((ce.tisList[tiIdx].endTime - ce.tisList[strtObservTiIdx].startTime) > (observationTime + predictionTime))
                                        //if (observation > (observationTime + predictionTime))
                                        if(ce.tisList[strtObservTiIdx].endTime < earlyTime)
                                            break;
                                    }
                                    if (strtObservTiIdx == -1)
                                        strtObservTiIdx = 0;
                                    break;
                                }
                            }
                            if ((predTiIdx - strtObservTiIdx) > minToEvent) // minE
                            {
                                ce.entityLine = "";
                                for (tiIdx = strtObservTiIdx+1; tiIdx <= predTiIdx; tiIdx++)
                                    ce.entityLine += ce.tisList.ElementAt(tiIdx).startTime + "," + ce.tisList.ElementAt(tiIdx).endTime + "," + ce.tisList.ElementAt(tiIdx).symbol + ";";
                                ce.entityLine += "\n";

                                addTonceptsToTonceptsHSCounter(ref procCentroidDic, ce.procTncpts);
                                addTonceptsToTonceptsHSCounter(ref drugCentroidDic, ce.drugTncpts);
                                addTonceptsToTonceptsHSCounter(ref condCentroidDic, ce.condTncpts);

                                string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                                if (genderDis.ContainsKey(patientDemoVec[11]))
                                    genderDis[patientDemoVec[11]]++;//gender
                                if (raceDis.ContainsKey(patientDemoVec[12]))
                                    raceDis[patientDemoVec[12]]++; //race
                                if (ethDis.ContainsKey(patientDemoVec[13]))
                                    ethDis[patientDemoVec[13]]++; //ethnicity

                                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                                    highChol++;
                                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                                    chf++;
                                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                                    diabetes++;
                                meanToEvent = meanToEvent + predTiIdx - (strtObservTiIdx+1);
                                eventSizeMetrics.Add(predTiIdx - (strtObservTiIdx + 1));
                                tisStr += ce.entityID + "," + count++ + ";\n" + ce.entityLine;
                            }
                        }
                    }
                }

                string distributions = "";
                for (int d = 0; d < genderDis.Count(); d++)
                    distributions += (double)genderDis.ElementAt(d).Value / (double)count + ",";
                for (int d = 0; d < raceDis.Count(); d++)
                    distributions += (double)raceDis.ElementAt(d).Value / (double)count + ",";
                for (int d = 0; d < ethDis.Count(); d++)
                    distributions += (double)ethDis.ElementAt(d).Value / (double)count + ",";

                if (count > 500)
                {
                    // calculate mean/std metrics 
                    meanToEvent = meanToEvent / count;
                    double mean = 0, std = 0;
                    for (int x = 0; x < eventSizeMetrics.Count; x++)
                        mean = mean + eventSizeMetrics[x];
                    mean = mean / eventSizeMetrics.Count;
                    
                    for (int x = 0; x < eventSizeMetrics.Count; x++)
                        std = std + Math.Pow((eventSizeMetrics[x] - mean), 2);
                    std = std / eventSizeMetrics.Count;
                    std = Math.Sqrt(std);
                    Random _r = new Random();
                    
                    string cost = "noCost";
                    if (tonceptCosts.ContainsKey(toncept.tonceptID))
                        cost = tonceptCosts[toncept.tonceptID].Split(',')[2];
                    string parents = "noParents";
                    if (tonceptParentingDic.ContainsKey(toncept.tonceptID))
                    {
                        parents = tonceptParentingDic[toncept.tonceptID];
                        string[] parentsVec = parents.Split(',');
                        for (int p = 0; p < parentsVec.Count() - 1; p++)
                        {
                            int parentId = int.Parse(parentsVec[p]);
                            if (tonceptsDic.ContainsKey(parentId)) //if (predictedToncepts.ContainsKey(parentId))
                            {
                                string[] parentsPropVec = tonceptsDic[parentId].Split(',');
                                parents += "(" + parentsPropVec[2] + ")" + parentsPropVec[1] + ",";  //    parents += predictedToncepts[parentId].tonceptName + ",";
                            }
                        }
                    }
                    //calculate cohort HS centroid
                    for (int c = 0; c < procCentroidDic.Count(); c++)
                        procCentroidDic[procCentroidDic.ElementAt(c).Key] = (float)(procCentroidDic.ElementAt(c).Value / (float)count);
                    for (int c = 0; c < drugCentroidDic.Count(); c++)
                        drugCentroidDic[drugCentroidDic.ElementAt(c).Key] = (float)(drugCentroidDic.ElementAt(c).Value / (float)count);
                    for (int c = 0; c < condCentroidDic.Count(); c++)
                        condCentroidDic[condCentroidDic.ElementAt(c).Key] = (float)(condCentroidDic.ElementAt(c).Value / (float)count);

                    List<CUMC_entity> controls = new List<CUMC_entity>();
                    for (int eIdx = 0; eIdx < entityTISs.Count(); eIdx++)
                    {
                        CUMC_entity ce = entityTISs.ElementAt(eIdx).Value;

                        if (!ce.procTncpts.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID) && ce.condTncpts.getTonceptVerticalSupport() > 0 && ce.drugTncpts.getTonceptVerticalSupport() > 0 && ce.procTncpts.getTonceptVerticalSupport() > 0 && ce.tisList.Count() > meanToEvent) // !hasTncpt && hasALL > 0.5 )
                        {
                            //int intersection = 0, unification = condCentroidDic.Count() + drugCentroidDic.Count() + procCentroidDic.Count();
                            double cosineDenominator = 0, SumA2 = 0, SumA2i = 0, SumB2 = 0;
                            //for (int t = 0; t < condCentroidDic.Count(); t++) 
                            for (int t = 0; t < ce.condTncpts.getTonceptVerticalSupport(); t++)
                            {
                                //if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(condCentroidDic.ElementAt(t).Key)) //
                                if (condCentroidDic.ContainsKey(ce.condTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                                {// intersection++;
                                    float centVal = condCentroidDic[ce.condTncpts.getTonceptHorizontalDic().ElementAt(t).Key]; // condCentroidDic.ElementAt(t).Value;
                                    SumA2 += Math.Pow(centVal, 2);
                                    int ceTnptCOunt = ce.condTncpts.getTonceptHorizontalDic().ElementAt(t).Value.Count(); // ce.condTncpts.getTonceptHorizontalDic()[condCentroidDic.ElementAt(t).Key].Count();
                                    SumB2 += Math.Pow(ceTnptCOunt, 2);
                                    cosineDenominator += centVal * ceTnptCOunt;
                                }
                            }
                            //for (int t = 0; t < drugCentroidDic.Count(); t++) 
                            for (int t = 0; t < ce.drugTncpts.getTonceptVerticalSupport(); t++)
                            {
                                //if (ce.drugTncpts.getTonceptHorizontalDic().ContainsKey(drugCentroidDic.ElementAt(t).Key)) //
                                if (drugCentroidDic.ContainsKey(ce.drugTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                                {// intersection++;
                                    float centVal = drugCentroidDic[ce.drugTncpts.getTonceptHorizontalDic().ElementAt(t).Key]; // condCentroidDic.ElementAt(t).Value;
                                    SumA2 += Math.Pow(centVal, 2);
                                    int ceTnptCOunt = ce.drugTncpts.getTonceptHorizontalDic().ElementAt(t).Value.Count(); // ce.condTncpts.getTonceptHorizontalDic()[condCentroidDic.ElementAt(t).Key].Count();
                                    SumB2 += Math.Pow(ceTnptCOunt, 2);
                                    cosineDenominator += centVal * ceTnptCOunt;
                                }
                            }
                            //for (int t = 0; t < procCentroidDic.Count(); t++) 
                            for (int t = 0; t < ce.procTncpts.getTonceptVerticalSupport(); t++)
                            {
                                //if (ce.procTncpts.getTonceptHorizontalDic().ContainsKey(procCentroidDic.ElementAt(t).Key)) //
                                if (procCentroidDic.ContainsKey(ce.procTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                                {// intersection++;
                                    float centVal = procCentroidDic[ce.procTncpts.getTonceptHorizontalDic().ElementAt(t).Key]; // condCentroidDic.ElementAt(t).Value;
                                    SumA2 += Math.Pow(centVal, 2);
                                    int ceTnptCOunt = ce.procTncpts.getTonceptHorizontalDic().ElementAt(t).Value.Count(); // ce.condTncpts.getTonceptHorizontalDic()[condCentroidDic.ElementAt(t).Key].Count();
                                    SumB2 += Math.Pow(ceTnptCOunt, 2);
                                    cosineDenominator += centVal * ceTnptCOunt;
                                }
                            } 
                            double simScorei = cosineDenominator / (Math.Sqrt(SumA2i) * Math.Sqrt(SumB2));
                            ce.simScore = cosineDenominator / (Math.Sqrt(SumA2) * Math.Sqrt(SumB2)); // (double)intersection / (double)unification;
                            if (controls.Count < count)
                                controls.Add(ce);
                            else
                            {
                                controls.Sort();
                                if (controls.ElementAt(0).simScore < ce.simScore)
                                {
                                    controls.Remove(controls.ElementAt(0));
                                    controls.Add(ce);
                                }
                            }
                        }
                        else
                            ce.simScore = 0;
                    }

                    Dictionary<string, int> cGenderDis = new Dictionary<string, int>(), cRaceDis = new Dictionary<string, int>(), cEthDis = new Dictionary<string, int>();
                    cGenderDis.Add("F", 0); cGenderDis.Add("M", 0); cGenderDis.Add("U", 0); cGenderDis.Add("O", 0);//genderDis.Add("X",0);genderDis.Add("I",0);
                    cRaceDis.Add("W", 0); cRaceDis.Add("O", 0); cRaceDis.Add("U", 0); cRaceDis.Add("A", 0); cRaceDis.Add("B", 0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
                    cEthDis.Add("N", 0); cEthDis.Add("U", 0); cEthDis.Add("H", 0); //ethDis.Add("D",0); 
                    int controlCount = 0;
                    double controlMeanToEvent = 0;
                    string controlTisStr = "";
                    float controlChf = 0, controlDiabetes = 0, controlHighChol = 0;

                    for (int eIdx = 0; eIdx < controls.Count; eIdx++)
                    {
                        CUMC_entity ce = controls.ElementAt(eIdx);
                        ce.entityLine = "";
                        int ti = 0;
                        //randomly generate a sequence of the events
                        int meanTo = (int)mean + 2;
                        int eventsTo = _r.Next((int)(meanTo - std), (int)(meanTo + std));
                        
                        for (ti = 0; ti < ce.tisList.Count && ti < eventsTo; ti++)
                            ce.entityLine += ce.tisList.ElementAt(ti).startTime + "," + ce.tisList.ElementAt(ti).endTime + "," + ce.tisList.ElementAt(ti).symbol + ";";
                        ce.entityLine += "\n";
                        controlMeanToEvent = controlMeanToEvent + ti;
                        controlTisStr += ce.entityID + "," + controlCount++ + ";\n" + ce.entityLine;

                        string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                        if (cGenderDis.ContainsKey(patientDemoVec[11]))
                            cGenderDis[patientDemoVec[11]]++;//gender
                        if (cRaceDis.ContainsKey(patientDemoVec[12]))
                            cRaceDis[patientDemoVec[12]]++; //race
                        if (cEthDis.ContainsKey(patientDemoVec[13]))
                            cEthDis[patientDemoVec[13]]++; //ethnicity

                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                            controlHighChol++;
                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                            controlChf++;
                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                            controlDiabetes++;
                    }
                    controlMeanToEvent = controlMeanToEvent / count; // otherCount;
                    string controlDistributions = "";
                    for (int d = 0; d < cGenderDis.Count(); d++)
                        controlDistributions += (double)cGenderDis.ElementAt(d).Value / (double)count + ",";
                    for (int d = 0; d < cRaceDis.Count(); d++)
                        controlDistributions += (double)cRaceDis.ElementAt(d).Value / (double)count + ",";
                    for (int d = 0; d < cEthDis.Count(); d++)
                        controlDistributions += (double)cEthDis.ElementAt(d).Value / (double)count + ",";

                    report += toncept.tonceptID + "," + toncept.tonceptName.Replace(",", "_") + "," + toncept.getTonceptVerticalSupport() + "," + toncept.getTotalHorizontalSupport() + "," + count + "," + meanToEvent + "," + cost + "," + distributions + "," + (double)chf / (double)count + "," + (double)diabetes / (double)count + "," + (double)highChol / (double)count + ",,\n";
                    report += "control,,,," + count + "," + controlMeanToEvent + ",," + controlDistributions + "," + (double)controlChf / (double)count + "," + (double)controlDiabetes / (double)count + "," + (double)controlHighChol / (double)count + "," + parents + ",\n";
                    writeToFile(printFileDirPAth + "\\proceduresTable_.csv", report);
                    if (tisStr.Count() > 0)
                    {
                        string fileName = toncept.tonceptID + "_oT" + observationTime + "_pT" + predictionTime + "_mE" + minEvents + "_mTE" + minToEvent + "_" + count + "_" + (int)mean + "_" + (int)controlMeanToEvent + "_OFILE.csv";
                        if (fileName.Length > 250) fileName = fileName.Remove(250);
                        writeToFile(printFileDirPAth + "\\" + fileName, "\n" + "startToncepts\n" + "numberOfEntities," + count.ToString() + "\n" + tisStr);

                        fileName = fileName.Replace("OFILE", "OtherFILE");
                        writeToFile(printFileDirPAth + "\\" + fileName, "\n" + "startToncepts\n" + "numberOfEntities," + controlCount.ToString() + "\n" + controlTisStr);
                        //break;
                    }
                }
            }//);

            int totalHighChol = 0, totalChf = 0, totalDiabetes = 0;
            Dictionary<string, int> totalGenderDis = new Dictionary<string, int>(), totalRaceDis = new Dictionary<string, int>(), totalEthDis = new Dictionary<string, int>();
            totalGenderDis.Add("F", 0); totalGenderDis.Add("M", 0); totalGenderDis.Add("U", 0); totalGenderDis.Add("O", 0);//genderDis.Add("X",0);genderDis.Add("I",0);
            totalRaceDis.Add("W", 0); totalRaceDis.Add("O", 0); totalRaceDis.Add("U", 0); totalRaceDis.Add("A", 0); totalRaceDis.Add("B", 0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
            totalEthDis.Add("N", 0); totalEthDis.Add("U", 0); totalEthDis.Add("H", 0); //ethDis.Add("D",0); 
            for (int eIdx = 0; eIdx < entityTISs.Count; eIdx++)
            {
                CUMC_entity ce = entityTISs.ElementAt(eIdx).Value;
                string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                if (totalGenderDis.ContainsKey(patientDemoVec[11]))
                    totalGenderDis[patientDemoVec[11]]++;//gender
                if (totalRaceDis.ContainsKey(patientDemoVec[12]))
                    totalRaceDis[patientDemoVec[12]]++; //race
                if (totalEthDis.ContainsKey(patientDemoVec[13]))
                    totalEthDis[patientDemoVec[13]]++; //ethnicity

                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                    totalHighChol++;
                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                    totalChf++;
                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                    totalDiabetes++;

            }
            string totalDistributions = "";
            for (int d = 0; d < totalGenderDis.Count(); d++)
                totalDistributions += (double)totalGenderDis.ElementAt(d).Value / (double)entityTISs.Count + ",";
            for (int d = 0; d < totalRaceDis.Count(); d++)
                totalDistributions += (double)totalRaceDis.ElementAt(d).Value / (double)entityTISs.Count + ",";
            for (int d = 0; d < totalEthDis.Count(); d++)
                totalDistributions += (double)totalEthDis.ElementAt(d).Value / (double)entityTISs.Count + ",";

            report += "total,,,,,,," + totalDistributions + "," + (double)totalChf / (double)entityTISs.Count + "," + (double)totalDiabetes / (double)entityTISs.Count + "," + (double)totalHighChol / (double)entityTISs.Count + ",,\n";
            writeToFile(printFileDirPAth + "\\proceduresTable_.csv", report);

        }
 */ 
        /*
        public static void create_TTI_Files(int observationTime, int predictionTime, int minEvents, int minToEvent, string printFileDirPAth, string conditionsFile, string drugsFile, string proceduresFile, string patientDemographicFile, string tonceptParentingFile, string procCostsFile, string condCostsFile, string procOrCond)
        {
            // REMEMBER TO CHECK THE MIN_TO_EVENT WHATS THE RIGHT VALUE

            Directory.CreateDirectory(printFileDirPAth);
            Dictionary<int, TemporalConcept> conditionsToncepts = new Dictionary<int,TemporalConcept>(), drugsToncepts = new Dictionary<int,TemporalConcept>(), proceduresToncepts = new Dictionary<int,TemporalConcept>();
            Dictionary<int, string> tonceptParentingDic = read_dbParenToncepts_file(tonceptParentingFile); // read_allenParenToncepts_file(tonceptParentingFile);
            Dictionary<int, string> tonceptsDic = read_file_toIntStringDic(tonceptParentingFile);
            Dictionary<int, string> tonceptProcCosts = read_file_toIntStringDic(procCostsFile);
            Dictionary<int, string> tonceptCondCosts = read_file_toIntStringDic(condCostsFile);
            Dictionary<int, string> patientDemographics = read_file_toIntStringDic(patientDemographicFile);
            Dictionary<int, CUMC_entity> entityTISs = read_new_cumc_data_files(ref conditionsToncepts, ref drugsToncepts, ref proceduresToncepts, conditionsFile, drugsFile, proceduresFile, tonceptParentingFile, printFileDirPAth, minEvents);
                        
            //int[] outcomeProcs = { 2514435, 2514433, 2001544, 2008237, 2002703, 42738972, 2008267, 2006918, 2002688, 2314026, 2006634, 2007901, 2007079, 2006993, 2000173, 2001505, 2008337, 2008009, 2008271, 2008264, 2008264 };

            string report = "tonceptID,tonceptName,verticalSupport,allInstances,cohortSize,meanToEvent,cost,Female,Male,Unknown,Other,White,Other,Unknown,Asian,Black,Hispanic or Latino,Not Hispanic or Latino,Unknown,chf,diabetes,highCholesterol,parents,\n";
            float meanToEvent = 0;
            Dictionary<int, TemporalConcept> predictedToncepts;
            if (procOrCond == "procedures")
                predictedToncepts = proceduresToncepts; // conditionsToncepts; // 
            else
                predictedToncepts = conditionsToncepts;
            Dictionary<int, string> tonceptCosts;
            if (procOrCond == "procedures")
                tonceptCosts = tonceptProcCosts; 
            else
                tonceptCosts = tonceptCondCosts;

            predictedToncepts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            for (int i = 0; i < predictedToncepts.Count(); i++)
            {
                Dictionary<int, float> procCentroidDic = new Dictionary<int, float>();   Dictionary<int, float> drugCentroidDic = new Dictionary<int, float>();   Dictionary<int, float> condCentroidDic = new Dictionary<int, float>();
                meanToEvent = 0;
                TemporalConcept toncept = predictedToncepts.ElementAt(i).Value;

                string cosTest = "noCost";
                if (tonceptCosts.ContainsKey(toncept.tonceptID))
                    cosTest = tonceptCosts[toncept.tonceptID].Split(',')[2];
                                
                //print examples
                string tisStr = "";
                int count = 0; float chf = 0, diabetes = 0, highChol = 0;
                Dictionary<string, int> genderDis = new Dictionary<string,int>(), raceDis = new Dictionary<string,int>(), ethDis = new Dictionary<string,int>();
                genderDis.Add("F",0);genderDis.Add("M",0);genderDis.Add("U",0);genderDis.Add("O",0);//genderDis.Add("X",0);genderDis.Add("I",0);
                raceDis.Add("W",0); raceDis.Add("O",0); raceDis.Add("U",0); raceDis.Add("A",0); raceDis.Add("B",0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
                ethDis.Add("N", 0); ethDis.Add("U", 0); ethDis.Add("H", 0); //ethDis.Add("D",0); 
                for (int e = 0; e < toncept.getTonceptHorizontalDic().Count(); e++)
                {
                    int eId = toncept.getTonceptHorizontalDic().ElementAt(e).Key;
                    if (entityTISs.ContainsKey(eId))
                    {
                        CUMC_entity ce = entityTISs[eId];
                        int tiIdx = 0, predTiIdx = 0, strtObservTiIdx = 0; ce.entityLine = "";

                        TemporalConcept tcPtr;
                        if (procOrCond == "procedures")
                            tcPtr = ce.procTncpts;
                        else
                            tcPtr = ce.condTncpts;
                        //if (ce.procTncpts.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID))
                        //if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID)) // if the patient has the outcome event
                        if (tcPtr.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID))
                        {
                            for (tiIdx = 0; tiIdx < ce.tisList.Count; tiIdx++)
                            {
                                if (ce.tisList[tiIdx].symbol == toncept.tonceptID) // if the outcome event was found
                                {
                                    for (predTiIdx = tiIdx; predTiIdx > -1; predTiIdx--)
                                        if (ce.tisList[tiIdx].startTime - ce.tisList[predTiIdx].endTime > predictionTime) // if prediction time exceeded
                                            break;
                                    for (strtObservTiIdx = predTiIdx; strtObservTiIdx > -1; strtObservTiIdx--)
                                        if ((ce.tisList[tiIdx].endTime - ce.tisList[strtObservTiIdx].startTime) > (observationTime + predictionTime))
                                            break;
                                    if (strtObservTiIdx == -1)
                                        strtObservTiIdx = 0;
                                    break;
                                }
                            }
                            if ((predTiIdx - strtObservTiIdx) > minToEvent) // minE
                            {
                                ce.entityLine = "";
                                for (tiIdx = strtObservTiIdx; tiIdx <= predTiIdx; tiIdx++)
                                    ce.entityLine += ce.tisList.ElementAt(tiIdx).startTime + "," + ce.tisList.ElementAt(tiIdx).endTime + "," + ce.tisList.ElementAt(tiIdx).symbol + ";";
                                ce.entityLine += "\n";

                                addTonceptsToTonceptsCounter(ref procCentroidDic, ce.procTncpts);
                                addTonceptsToTonceptsCounter(ref drugCentroidDic, ce.drugTncpts);
                                addTonceptsToTonceptsCounter(ref condCentroidDic, ce.condTncpts);

                                string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                                if(genderDis.ContainsKey(patientDemoVec[11]))
                                    genderDis[patientDemoVec[11]]++;//gender
                                if(raceDis.ContainsKey(patientDemoVec[12]))
                                    raceDis[patientDemoVec[12]]++; //race
                                if(ethDis.ContainsKey(patientDemoVec[13]))
                                    ethDis[patientDemoVec[13]]++; //ethnicity

                                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                                    highChol++;
                                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                                    chf++;
                                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                                    diabetes++;
                                meanToEvent = meanToEvent + predTiIdx - strtObservTiIdx;
                                tisStr += ce.entityID + "," + count++ + ";\n" + ce.entityLine;
                            }
                        }
                    }
                }

                string distributions = "";
                for (int d = 0; d < genderDis.Count(); d++)
                    distributions += (double)genderDis.ElementAt(d).Value/(double)count + ",";
                for (int d = 0; d < raceDis.Count(); d++)
                    distributions += (double)raceDis.ElementAt(d).Value / (double)count + ",";
                for (int d = 0; d < ethDis.Count(); d++)
                    distributions += (double)ethDis.ElementAt(d).Value / (double)count + ",";

                if (count > 500)
                {
                    meanToEvent = meanToEvent / count;
                    string cost = "noCost";
                    if (tonceptCosts.ContainsKey(toncept.tonceptID))
                        cost = tonceptCosts[toncept.tonceptID].Split(',')[2];
                    string parents = "noParents";
                    if (tonceptParentingDic.ContainsKey(toncept.tonceptID))
                    {
                        parents = tonceptParentingDic[toncept.tonceptID];
                        string[] parentsVec = parents.Split(',');
                        for (int p = 0; p < parentsVec.Count()-1; p++)
                        {
                            int parentId = int.Parse(parentsVec[p]);
                            if (tonceptsDic.ContainsKey(parentId)) //if (predictedToncepts.ContainsKey(parentId))
                            {
                                string[] parentsPropVec = tonceptsDic[parentId].Split(',');
                                parents += "("+parentsPropVec[2]+")"+ parentsPropVec[1] + ",";  //    parents += predictedToncepts[parentId].tonceptName + ",";
                            }
                        }
                    }
                    List<CUMC_entity> controls = new List<CUMC_entity>();
                    for (int eIdx = 0; eIdx < entityTISs.Count(); eIdx++)
                    {
                        CUMC_entity ce = entityTISs.ElementAt(eIdx).Value;

                        if (!ce.procTncpts.getTonceptHorizontalDic().ContainsKey(toncept.tonceptID)) // !hasTncpt && hasALL > 0.5 )
                        {
                            int intersection = 0, unification = condCentroidDic.Count() + drugCentroidDic.Count() + procCentroidDic.Count();
                            for (int t = 0; t < ce.condTncpts.getTonceptVerticalSupport(); t++)
                                if (condCentroidDic.ContainsKey(ce.condTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                                    intersection++;
                            for (int t = 0; t < ce.condTncpts.getTonceptVerticalSupport(); t++)
                                if (condCentroidDic.ContainsKey(ce.condTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                                    intersection++;
                            for (int t = 0; t < ce.condTncpts.getTonceptVerticalSupport(); t++)
                                if (condCentroidDic.ContainsKey(ce.condTncpts.getTonceptHorizontalDic().ElementAt(t).Key))
                                    intersection++;
                            ce.simScore = (double)intersection / (double)unification;
                            if (controls.Count < count)
                                controls.Add(ce);
                            else
                            {
                                controls.Sort();
                                if (controls.ElementAt(0).simScore < ce.simScore)
                                {
                                    controls.Remove(controls.ElementAt(0));
                                    controls.Add(ce);
                                }
                            }
                        }
                        else
                            ce.simScore = 0;
                        
                    }
                    
                    Dictionary<string, int> cGenderDis = new Dictionary<string,int>(), cRaceDis = new Dictionary<string,int>(), cEthDis = new Dictionary<string,int>();
                    cGenderDis.Add("F",0);cGenderDis.Add("M",0);cGenderDis.Add("U",0);cGenderDis.Add("O",0);//genderDis.Add("X",0);genderDis.Add("I",0);
                    cRaceDis.Add("W",0); cRaceDis.Add("O",0); cRaceDis.Add("U",0); cRaceDis.Add("A",0); cRaceDis.Add("B",0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
                    cEthDis.Add("N", 0); cEthDis.Add("U", 0); cEthDis.Add("H", 0); //ethDis.Add("D",0); 
                    int controlCount = 0;
                    double controlMeanToEvent = 0;
                    string controlTisStr = "";
                    float controlChf = 0, controlDiabetes = 0, controlHighChol = 0;
                    for (int eIdx = 0; eIdx < controls.Count; eIdx++)
                    {
                        CUMC_entity ce = controls.ElementAt(eIdx);
                        ce.entityLine = "";
                        int ti = 0;
                        for (ti = 0; ti < ce.tisList.Count && ti <= meanToEvent; ti++)
                            ce.entityLine += ce.tisList.ElementAt(ti).startTime + "," + ce.tisList.ElementAt(ti).endTime + "," + ce.tisList.ElementAt(ti).symbol + ";";
                        ce.entityLine += "\n";
                        controlMeanToEvent = controlMeanToEvent + ti;
                        controlTisStr += ce.entityID + "," + controlCount++ + ";\n" + ce.entityLine;

                        string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                        if(cGenderDis.ContainsKey(patientDemoVec[11]))
                            cGenderDis[patientDemoVec[11]]++;//gender
                        if(cRaceDis.ContainsKey(patientDemoVec[12]))
                            cRaceDis[patientDemoVec[12]]++; //race
                        if(cEthDis.ContainsKey(patientDemoVec[13]))
                            cEthDis[patientDemoVec[13]]++; //ethnicity

                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                            controlHighChol++;
                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                            controlChf++;
                        if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                            controlDiabetes++;
                    }
                    controlMeanToEvent = controlMeanToEvent / count; // otherCount;
                    string controlDistributions = "";
                    for (int d = 0; d < cGenderDis.Count(); d++)
                        controlDistributions += (double)cGenderDis.ElementAt(d).Value / (double)count + ",";
                    for (int d = 0; d < cRaceDis.Count(); d++)
                        controlDistributions += (double)cRaceDis.ElementAt(d).Value / (double)count + ",";
                    for (int d = 0; d < cEthDis.Count(); d++)
                        controlDistributions += (double)cEthDis.ElementAt(d).Value / (double)count + ",";
   
                    report += toncept.tonceptID + "," + toncept.tonceptName.Replace(",", "_") + "," + toncept.getTonceptVerticalSupport() + "," + toncept.getTotalHorizontalSupport() + "," + count + "," + meanToEvent + "," + cost + "," + distributions + "," + (double)chf / (double)count + "," + (double)diabetes / (double)count + "," + (double) highChol / (double)count + ",,\n";
                    report += "control,,,," + count + "," + controlMeanToEvent + ",," + controlDistributions + "," + (double)controlChf / (double)count + "," + (double)controlDiabetes / (double)count + "," + (double)controlHighChol / (double)count + "," + parents + ",\n";
                    writeToFile(printFileDirPAth + "\\proceduresTable_.csv", report);
                    if (tisStr.Count() > 0)
                    {
                        string fileName = toncept.tonceptID + "_oT" + observationTime + "_pT" + predictionTime + "_mE" + minEvents + "_mTE" + minToEvent + "_" + count + "_" + meanToEvent + "_" + controlMeanToEvent + "_OFILE.csv";
                        if (fileName.Length > 250) fileName = fileName.Remove(250);
                            writeToFile(printFileDirPAth + "\\" + fileName, "\n" + "startToncepts\n" + "numberOfEntities," + count.ToString() + "\n" + tisStr);

                        fileName = fileName.Replace("OFILE", "OtherFILE");
                        writeToFile(printFileDirPAth + "\\" + fileName, "\n" + "startToncepts\n" + "numberOfEntities," +controlCount.ToString() + "\n" + controlTisStr);
                        //break;
                    }
                }
           }

            int totalHighChol = 0, totalChf = 0, totalDiabetes = 0;
            Dictionary<string, int> totalGenderDis = new Dictionary<string, int>(), totalRaceDis = new Dictionary<string, int>(), totalEthDis = new Dictionary<string, int>();
            totalGenderDis.Add("F", 0); totalGenderDis.Add("M", 0); totalGenderDis.Add("U", 0); totalGenderDis.Add("O", 0);//genderDis.Add("X",0);genderDis.Add("I",0);
            totalRaceDis.Add("W", 0); totalRaceDis.Add("O", 0); totalRaceDis.Add("U", 0); totalRaceDis.Add("A", 0); totalRaceDis.Add("B", 0); //raceDis.Add("D",0); raceDis.Add("I",0); raceDis.Add("H",0); raceDis.Add("P",0); raceDis.Add("X",0);
            totalEthDis.Add("N", 0); totalEthDis.Add("U", 0); totalEthDis.Add("H", 0); //ethDis.Add("D",0); 
            for (int eIdx = 0; eIdx < entityTISs.Count; eIdx++)
            {
                CUMC_entity ce = entityTISs.ElementAt(eIdx).Value;
                string[] patientDemoVec = patientDemographics[ce.entityID].Split(',');
                if (totalGenderDis.ContainsKey(patientDemoVec[11]))
                    totalGenderDis[patientDemoVec[11]]++;//gender
                if (totalRaceDis.ContainsKey(patientDemoVec[12]))
                    totalRaceDis[patientDemoVec[12]]++; //race
                if (totalEthDis.ContainsKey(patientDemoVec[13]))
                    totalEthDis[patientDemoVec[13]]++; //ethnicity

                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(4096215) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(432867))
                    totalHighChol++;
                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442310) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443580) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(319835))
                    totalChf++;
                if (ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201820) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(201826) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(443767) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(192279) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(376065) || ce.condTncpts.getTonceptHorizontalDic().ContainsKey(442793))
                    totalDiabetes++;

            }
            string totalDistributions = "";
            for (int d = 0; d < totalGenderDis.Count(); d++)
                totalDistributions += (double)totalGenderDis.ElementAt(d).Value / (double)entityTISs.Count + ",";
            for (int d = 0; d < totalRaceDis.Count(); d++)
                totalDistributions += (double)totalRaceDis.ElementAt(d).Value / (double)entityTISs.Count + ",";
            for (int d = 0; d < totalEthDis.Count(); d++)
                totalDistributions += (double)totalEthDis.ElementAt(d).Value / (double)entityTISs.Count + ",";

            report += "total,,,,,,," + totalDistributions + "," + (double)totalChf / (double)entityTISs.Count + "," + (double)totalDiabetes / (double)entityTISs.Count + "," + (double)totalHighChol / (double)entityTISs.Count + ",,\n";
            writeToFile(printFileDirPAth + "\\proceduresTable_.csv", report);
                    
        }*/

        public static Dictionary<int, TemporalConcept> read_cumc_file(string filePath, ref Dictionary<int, CUMC_entity> entityTISsRef, char delim, string tonceptGroup)//, Dictionary<int, int> tonceptParentingDic = null)//, string tonceptParentingFile = null)
        {
            Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();
            try
            {
                TemporalConcept tc;
                //Dictionary<int, int> tonceptParentingDic = null;
                //if (tonceptParentingFile != null)
                //    tonceptParentingDic = read_allenParenToncepts_file(tonceptParentingFile);
                TextReader tr = new StreamReader(filePath);
                string readLine;
                int prevEntityID = -1;//, prevTonceptID = -1;
                CUMC_entity cumcEntityPtr = null;
                while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
                {
                    readLine = tr.ReadLine();
                    string[] mainDelimited = readLine.Split(delim);
                    int entityID = int.Parse(mainDelimited[0]);
                    int tonceptID = int.Parse(mainDelimited[1]);
                    //if (tonceptParentingDic != null && tonceptParentingDic.ContainsKey(tonceptID))
                    //    tonceptID = tonceptParentingDic[tonceptID];
                    if (!toncepts.ContainsKey(tonceptID))
                    {
                        tc = new TemporalConcept(tonceptID, 0);
                        tc.tonceptName = mainDelimited[4];//5];
                        tc.tonceptOrig = tonceptGroup;
                        toncepts.Add(tonceptID, tc);
                    }
                    int startTime = int.Parse(mainDelimited[2]);
                    int endTime = int.Parse(mainDelimited[3]);
                    TimeIntervalSymbol tis = new TimeIntervalSymbol(startTime, endTime, tonceptID);
                    toncepts[tonceptID].addEntityTinstance(entityID, tis); //null);
                    //if (prevEntityID != entityID)
                    {
                        if (!entityTISsRef.ContainsKey(entityID))
                        {
                            cumcEntityPtr = new CUMC_entity(entityID);
                            entityTISsRef.Add(entityID, cumcEntityPtr);
                        }
                        else
                            cumcEntityPtr = entityTISsRef[entityID];
                    }
                    //else
                    //    entityID = entityID;
                    cumcEntityPtr.addTIS(tis, tonceptGroup);

                    //prevTonceptID = tonceptID;
                    prevEntityID = entityID;
                }
                tr.Close();
                toncepts = toncepts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return toncepts;
        }

        private static void removeToncepts(ref Dictionary<int, float> tonceptCounterDic, int count, double threshold)
        {
            for (int t = 0; t < tonceptCounterDic.Count(); t++)
            {
                double ratio = tonceptCounterDic.ElementAt(t).Value / count;
                if (ratio < threshold)
                {
                    tonceptCounterDic.Remove(tonceptCounterDic.ElementAt(t).Key);
                    t--;
                }
            }
        }
        
        private static void addTonceptsToTonceptsCounter(ref Dictionary<int, float> conceptsCounter, TemporalConcept ceToncept)
        {
            for (int p = 0; p < ceToncept.getTonceptVerticalSupport(); p++)
                if (conceptsCounter.ContainsKey(ceToncept.getTonceptHorizontalDic().ElementAt(p).Key))
                    conceptsCounter[ceToncept.getTonceptHorizontalDic().ElementAt(p).Key] += ceToncept.getTonceptHorizontalDic().ElementAt(p).Value.Count; //conceptsCounter[ceToncept.getTonceptHorizontalDic().ElementAt(p).Key]++;
                else
                    conceptsCounter.Add(ceToncept.getTonceptHorizontalDic().ElementAt(p).Key, ceToncept.getTonceptHorizontalDic().ElementAt(p).Value.Count); //conceptsCounter.Add(ceToncept.getTonceptHorizontalDic().ElementAt(p).Key, 1);
        }

        private static void addTonceptsToTonceptsHSCounter(ref Dictionary<int, float> conceptsCounter, TemporalConcept ceToncept)
        {
            for (int p = 0; p < ceToncept.getTonceptVerticalSupport(); p++)
                if (conceptsCounter.ContainsKey(ceToncept.getTonceptHorizontalDic().ElementAt(p).Key))
                    conceptsCounter[ceToncept.getTonceptHorizontalDic().ElementAt(p).Key] += ceToncept.getTonceptHorizontalDic().ElementAt(p).Value.Count; //conceptsCounter[ceToncept.getTonceptHorizontalDic().ElementAt(p).Key]++;
                else
                    conceptsCounter.Add(ceToncept.getTonceptHorizontalDic().ElementAt(p).Key, ceToncept.getTonceptHorizontalDic().ElementAt(p).Value.Count); //conceptsCounter.Add(ceToncept.getTonceptHorizontalDic().ElementAt(p).Key, 1);
        }

        public static void writeToFile(string filePath, string fileContent)
        {
            TextWriter tw = new StreamWriter(filePath); // matrixFileName.Replace(".csv", "_HS_MATRIX.arff"));
            tw.Write(fileContent); // fileHeader + features);
            tw.Close();
        }

        private static string cumc_data_stats(TemporalConcept trgTncpt, ref List<int> trgTncptMin, ref Dictionary<int, CUMC_entity> entityTISs, ref Dictionary<int, TemporalConcept> toncepts, int topTonceptsNum)
        {
            string trgtStats = "";
            int targetVerticalSupport = trgTncpt.getTonceptVerticalSupport();
            for (int t = 0; t < topTonceptsNum && t < toncepts.Count; t++)
            {
                TemporalConcept checkToncept = toncepts.ElementAt(t).Value;
                if (checkToncept.tonceptID != trgTncpt.tonceptID)
                {
                    /*intrsctCount = 0;
                    for (int e = 0; e < targetVerticalSupport; e++)
                    {
                        int eId = trgTncpt.getTonceptEntitiesList().ElementAt(e);
                        if (checkToncept.getTonceptEntitiesList().Contains(eId))
                            intrsctCount++;
                    }
                     trgtStats = trgtStats + intrsctCount + ","; //cout intrsctCoun
                    */
                    for (int e = 0; e < trgTncptMin.Count; e++)
                    {
                        int eId = trgTncptMin.ElementAt(e);
                        if (!checkToncept.getTonceptHorizontalDic().ContainsKey(eId))
                        {
                            trgTncptMin.Remove(eId);
                            e--;
                        }
                    }
                    trgtStats = trgtStats + trgTncptMin.Count + ","; //cout intrsctCount
                }
                else
                    trgtStats = trgtStats + ","; //cout intrsctCount

            }
            return trgtStats;
        }

        public static Dictionary<int, int> read_allenParenToncepts_file(string fileName)
        {
            Dictionary<int, int> tonceptsParents = new Dictionary<int, int>();
            try
            {
                int res = 0;
                TextReader tr = new StreamReader(fileName);
                string readLine = tr.ReadLine();
                while (tr.Peek() >= 0)
                {
                    readLine = tr.ReadLine();
                    string[] mainDelimited = readLine.Split(',');
                    if (mainDelimited[0].Length > 0 && mainDelimited[0] != mainDelimited[1])
                        if (int.TryParse(mainDelimited[1], out res) && int.TryParse(mainDelimited[0], out res))
                            tonceptsParents.Add(int.Parse(mainDelimited[1]), int.Parse(mainDelimited[0]));
                }
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return tonceptsParents;
        }

        public static Dictionary<int, float /*List<int>*/> read_proCosts_file(string fileName)
        {
            int res = 0;
            Dictionary<int, float /*List<int>*/> tonceptsCosts = new Dictionary<int, float /*List<int>*/>();
            try
            {
                //int counter = 0;
                /*List<int>*/
                string intList;
                TextReader tr = new StreamReader(fileName);
                string readLine = tr.ReadLine();
                while (tr.Peek() >= 0)
                {
                    readLine = tr.ReadLine();
                    string[] mainDelimited = readLine.Split(',');
                    int tonceptID = int.Parse(mainDelimited[0]);
                    float cost = float.Parse(mainDelimited[3]);
                    if (!tonceptsCosts.ContainsKey(tonceptID))
                    {
                        //intList = new List<int>();
                        //intList = parentID.ToString(); //intList.Add(parentID);
                        tonceptsCosts.Add(tonceptID, cost); //intList);
                    }
                    else
                    {
                        //intList = tonceptsParents[tonceptID];
                        //intList.Add(parentID);
                        //tonceptsCosts[tonceptID] += ("," + parentID.ToString());
                        cost = cost;
                    }
                    //counter++;
                }
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return tonceptsCosts;
        }


        public static Dictionary<int, string /*List<int>*/> read_dbParenToncepts_file(string fileName)
        {
            int res = 0;
            int counter = 0;
            Dictionary<int, string /*List<int>*/> tonceptsParents = new Dictionary<int, string /*List<int>*/>();
            try
            {
                /*List<int>*/ string intList;
                TextReader tr = new StreamReader(fileName);
                string readLine = tr.ReadLine();
                while (tr.Peek() >= 0)
                {
                    readLine = tr.ReadLine();
                    string[] mainDelimited = readLine.Split(',');
                    int tonceptID = int.Parse(mainDelimited[0]);
                    int parentID = int.Parse(mainDelimited[3]);
                    if (!tonceptsParents.ContainsKey(tonceptID))
                    {
                        //intList = new List<int>();
                        intList = parentID + ","; //intList.Add(parentID);
                        tonceptsParents.Add(tonceptID, intList);
                    }
                    else
                    {
                        //intList = tonceptsParents[tonceptID];
                        //intList.Add(parentID);
                        tonceptsParents[tonceptID] += (parentID + ",");
                    }
                    counter++;
                    if (counter == 1515)
                        counter = counter;
                }
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return tonceptsParents;
        }

        /*public static void summarizeWekaResults(string wekaResults)
        {
            TextReader tr = new StreamReader(wekaResults);
            string readLine = tr.ReadLine();
            string[] readVec = readLine.Split(',');
            string summarizedFile = "toncept_ID,minEvents,cohort,number,minSup,maxGap,epsilon,R,Rep,TIRPs,TPR,FPR,TNR,FNR,AUC\n";
            int counter = 0;
            double tpr29 = 0, fpr31 = 0, tnr33 = 0, fnr35 = 0, aucMeasure40 = 0;
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                readLine = tr.ReadLine();
                readVec = readLine.Split(',');
                string[] keyDataset = readVec[0].Split('_');
                tpr29 +=      double.Parse(readVec[29]);
                fpr31 +=      double.Parse(readVec[31]);
                tnr33 +=      double.Parse(readVec[33]);
                fnr35 +=      double.Parse(readVec[35]);
                aucMeasure40 += double.Parse(readVec[40]);
                counter++;
                if (counter == 10)
                {
                    summarizedFile += keyDataset[0] + "," + keyDataset[1] + "," + keyDataset[2] + "," + keyDataset[3] + "," + keyDataset[4] + "," + keyDataset[5] + "," + keyDataset[6] + "," + keyDataset[7] + "," + keyDataset[8] + "," + keyDataset[9] + "," + (tpr29 / 10) + "," + (fpr31 / 10) + "," + (tnr33 / 10) + "," + (fnr35 / 10) + "," + (aucMeasure40 / 10) + "\n";
                    counter = 0;
                    tpr29 = 0; fpr31 = 0; tnr33 = 0; fnr35 = 0; aucMeasure40 = 0; 
                }
            }
            tr.Close();
            TextWriter tw = new StreamWriter(wekaResults.Replace(".csv", "_Summerized.csv"));
            tw.Write(summarizedFile);
            tw.Close();

        }*/

        /*public static string mineOutcomeFilesDirectoryInTwoSteps(string mainDir, int BinHSmeanD, bool onlyBEFORE, int minSup, int maxGap, int epsilon, int relStyle)
        {
            string outDir = mainDir.Replace("\\sourceData", "") + "\\_ms" + minSup + "_mg" + maxGap + "_e" + epsilon + "_r" + relStyle;
            List<string> tonceptTirpsFileList = new List<string>();
            Directory.CreateDirectory(outDir);
            outDir += "\\";
            foreach (string filePath in Directory.GetFiles(mainDir))
            {
                if (filePath.Contains("OFILE.csv"))
                {
                    string fileName = filePath.Split('\\')[filePath.Split('\\').Length - 1];
                    string[] fileNameParts = fileName.Split('_');
                    int tonceptID = int.Parse(fileNameParts[0]);
                    tonceptTirpsFileList.Add(filePath + ";" + tonceptID);
                }
                break;
            }

            foreach (string toncepTirpsFile in tonceptTirpsFileList)
            //Parallel.ForEach(tonceptTirpsFileList, toncepTirpsFile =>
            {
                string filePath = toncepTirpsFile.Split(';')[0];
                //string tirpsFile = toncepTirpsFile.Split(';')[1];
                int tonceptID = int.Parse(toncepTirpsFile.Split(';')[1]); 
                string backTIRPsFile = mineBack(outDir, filePath, tonceptID, onlyBEFORE, minSup, maxGap, epsilon, relStyle);
                
                List<TIRP> tirpsList = Single11EVEN.readTIRPsFile(backTIRPsFile, true, tonceptID, false);
                string firstLine = "paitentID,";
                for (int t = 0; t < tirpsList.Count; t++)
                {
                    for (int tncpt = 0; tncpt < tirpsList.ElementAt(t).toncepts.Length; tncpt++)
                        firstLine += (tirpsList.ElementAt(t).toncepts[tncpt] + "-");
                    firstLine += ("," + tirpsList.ElementAt(t).size + ",");
                }
                firstLine += "class,\n";
                /*firstLine += "paitentID,";
                for (int t = 0; t < tirpsList.Count; t++)
                    firstLine += (tirpsList.ElementAt(t).size + ",");
                firstLine += "class,\n";*/

                //singlemine outcome with outcomeTIRPs
          //      string outcomeClassEntities = Single11EVEN.mineClassEntitiesByTIRPs(filePath, tirpsList, 1, relStyle, epsilon, maxGap);//  BinHSmeanD, 1, KLC.RELSTYLE_ALLEN7, 300, 1000);
                //singlemine other with outcomeTIRPs
          //      string otherClassEntities = Single11EVEN.mineClassEntitiesByTIRPs(filePath.Replace("OFILE.csv", "OtherFILE"), tirpsList, 0, relStyle, epsilon, maxGap); // BinHSmeanD, 0, KLC.RELSTYLE_ALLEN7, 300, 1000);
                //createMatrix
                //string matrixFile = outcomeFile.Replace("OFILE.csv", ("_MATRIX_"+"ms_"+minSup+"mg_"+maxGap+"e_"+epsilon+"r_"+relStyle));
          //      string matrixFile = backTIRPsFile.Replace("OFILE", "MATRIX");
          //      matrixFile = matrixFile.Replace("_onlyTncpt_KarmEL.txt", ".csv");
          //      TextWriter tw = new StreamWriter(matrixFile);
          //      tw.Write(firstLine + outcomeClassEntities + otherClassEntities);
          //      tw.Close();
          //  }//);

          //  return outDir;
     //   }

        /*public static string mineOutcomeFilesDirectory(string mainDir, int BinHSmeanD, bool onlyBEFORE, int minSup, int maxGap, int epsilon, int relStyle)
        {
            string outDir = mainDir.Replace("\\sourceData", "") +"\\_ms" + minSup + "_mg" + maxGap + "_e" + epsilon + "_r" + relStyle;
            Directory.CreateDirectory(outDir);
            outDir += "\\";
            foreach (string filePath in Directory.GetFiles(mainDir))
            {
                if (filePath.Contains("OFILE.csv"))
                {
                    string fileName = filePath.Split('\\')[filePath.Split('\\').Length - 1];
                    string[] fileNameParts = fileName.Split('_');
                    int tonceptID = int.Parse(fileNameParts[0]);
                    //if(tonceptID > 2002703)
                        matrixOutcomeAndOther(outDir, filePath, filePath.Replace("OFILE.csv", "OtherFILE"), BinHSmeanD, tonceptID, onlyBEFORE, minSup, maxGap, epsilon, relStyle);
                }
            }
            return outDir;
        }*/

        /*private static void matrixOutcomeAndOther(string mainDir, string outcomeFile, string otherFile, int BinHSmeanD, int tonceptID, bool onlyBEFORE, int minSup, int maxGap, int epsilon, int relStyle)
        {
            //mine outcome
            string backTIRPsFile = mineBack(mainDir, outcomeFile, tonceptID, onlyBEFORE, minSup, maxGap, epsilon, relStyle);
            List<TIRP> tirpsList = Single11EVEN.readTIRPsFile(backTIRPsFile, true, tonceptID, false);
            string firstLine = "paitentID,";
            for (int t = 0; t < tirpsList.Count; t++)
            {
                for (int tncpt = 0; tncpt < tirpsList.ElementAt(t).toncepts.Length; tncpt++)
                    firstLine += (tirpsList.ElementAt(t).toncepts[tncpt] + "-");
                firstLine += ",";
            }
            firstLine += "class,\n";
            firstLine += "paitentID,";
            for (int t = 0; t < tirpsList.Count; t++)
                firstLine += (tirpsList.ElementAt(t).size + ",");
            firstLine += "class,\n";
            //singlemine outcome with outcomeTIRPs
            string outcomeClassEntities = Single11EVEN.mineClassEntitiesByTIRPs(outcomeFile, tirpsList, 1, relStyle, epsilon, maxGap); // BinHSmeanD, 1, KLC.RELSTYLE_ALLEN7, 300, 1000);
            //singlemine other with outcomeTIRPs
            string otherClassEntities = Single11EVEN.mineClassEntitiesByTIRPs(otherFile, tirpsList, 0, relStyle, epsilon, maxGap); // BinHSmeanD, 0, KLC.RELSTYLE_ALLEN7, 300, 1000);
            //createMatrix
            //string matrixFile = outcomeFile.Replace("OFILE.csv", ("_MATRIX_"+"ms_"+minSup+"mg_"+maxGap+"e_"+epsilon+"r_"+relStyle));
            string matrixFile = backTIRPsFile.Replace("OFILE", "MATRIX");
            matrixFile = matrixFile.Replace("_onlyTncpt_KarmEL.txt", ".csv");
            TextWriter tw = new StreamWriter(matrixFile);
            tw.Write(firstLine + outcomeClassEntities + otherClassEntities);
            tw.Close();
        }*/

        /*public static string mineBack(string mainDir, string dataset, int tonceptID, bool onlyBEFORE, int minSup = 50, int maxGap = 1000, int epsilon = 300, int relStyle = KLC.RELSTYLE_ALLEN7, bool setHS1 = false, int print = KLC.KL_PRINT_TONCEPTANDTIRPS) // NO_INSTANCES) // KLC.KL_PRINT_TIRPS)
        {
            string klFile = dataset;
            string tempDir = mainDir + "tempDir_" + tonceptID + "_" + DateTime.Now.Hour + DateTime.Now.Minute + "\\";
            Directory.CreateDirectory(tempDir);

            string karmETime = "karmETime";
            KarmE kE = new KarmE(KLC.backwardsMining, relStyle, epsilon, maxGap, minSup, klFile, KLC.KL_TRANS_YES, setHS1, print, KLC.nonParallelMining, ref karmETime);
            dataset = mainDir + dataset.Split('\\')[dataset.Split('\\').Length-1].Replace(".csv", "") + "_ms_" + minSup + "_mg_" + maxGap + "_e_" + epsilon + "_r_" + relStyle + "_onlyTncpt_KarmEL.txt";
            string karmelTime = ELIsrael.RunEL(kE, tempDir, dataset, onlyBEFORE, null); //kE.getTonceptByID(tonceptID));
            kE = null;
            
            return dataset;
        }*/

        /*public static void createARFFilesDirectory(string mainDir)
        {
            foreach (string fileName in Directory.GetFiles(mainDir))
            {
                if(fileName.Contains("MATRIX") && fileName.Contains(".csv"))
                    createCSVorARFFfiles(fileName); 
            }
        }

        public static void createCSVorARFFfiles(string matrixFileName)
        {
            List<int> shortTIRPs = new List<int>();
            TextReader tr = new StreamReader(matrixFileName);
            string[] matrixFileProp = matrixFileName.Replace(".csv", "").Split('\\')[matrixFileName.Split('\\').Length-1].Split('_');
            string fileDescription      = "@RELATION " + matrixFileProp[0] + "_" + matrixFileProp[1] + "_" + matrixFileProp[2] + "_" + matrixFileProp[3] + "_" + matrixFileProp[6] + "_" + matrixFileProp[8] + "_" + matrixFileProp[10] + "_" + matrixFileProp[12];
            string[] shortFileHeaderVec = { fileDescription + "_BIN_short\n", fileDescription + "_HS_short\n", fileDescription + "_MEAND_short\n", fileDescription + "_HSNRM_short\n", fileDescription + "_HSMND_short\n" };
            string[] fullFileHeaderVec  = { fileDescription + "_BIN_full\n", fileDescription + "_HS_full\n", fileDescription + "_MEAND_full\n", fileDescription + "_HSNRM_full\n", fileDescription + "_HSMND_full\n" };
            string[] tirpsFileHeaderVec = { fileDescription + "_BIN_tirps\n", fileDescription + "_HS_tirps\n", fileDescription + "_MEAND_tirps\n", fileDescription + "_HSNRM_tirps\n", fileDescription + "_HSMND_tirps\n" };
            
            string[] delimLine = tr.ReadLine().Split(',');
            for(int i = 2; i < delimLine.Length-2; i = i+2)
            {
                if(int.Parse(delimLine[i]) == 1)
                {
                    shortTIRPs.Add(i);
                    addStringToeachStringVec(ref shortFileHeaderVec, "@ATTRIBUTE attr" + shortTIRPs.Count + " NUMERIC\n");
                }
                else
                    addStringToeachStringVec(ref tirpsFileHeaderVec, "@ATTRIBUTE attr" + (i - shortTIRPs.Count) + " NUMERIC\n");
                addStringToeachStringVec(ref fullFileHeaderVec, "@ATTRIBUTE attr" + i + " NUMERIC\n");
            }
            addStringToeachStringVec(ref shortFileHeaderVec, "@ATTRIBUTE class {0,1}\n");
            addStringToeachStringVec(ref tirpsFileHeaderVec, "@ATTRIBUTE class {0,1}\n");
            addStringToeachStringVec(ref fullFileHeaderVec, "@ATTRIBUTE class {0,1}\n");

            addStringToeachStringVec(ref shortFileHeaderVec, "@DATA\n");
            addStringToeachStringVec(ref tirpsFileHeaderVec, "@DATA\n");
            addStringToeachStringVec(ref fullFileHeaderVec,  "@DATA\n");

            string[] shortFeaturesVec = { "", "", "", "", "" };
            string[] fullFeaturesVec  = { "", "", "", "", "" };
            string[] tirpsFeaturesVec = { "", "", "", "", "" };
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                delimLine = tr.ReadLine().Split(',');
                string hsLine = "", binLine = "", hsNormLine = "", meanDLine = "", hsmnDLine = "";
                double maxVal = 0;
                for (int i = 2; i < delimLine.Length - 1; i = i+2)
                    addToLines(i - 1, i, ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref fullFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");

                maxVal = 0; hsLine = ""; binLine = ""; hsNormLine = ""; meanDLine = ""; hsmnDLine = "";
                for (int i = 0; i < shortTIRPs.Count; i++)
                    addToLines(shortTIRPs[i] - 1, shortTIRPs[i], ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref shortFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");
                hsLine = ""; binLine = ""; hsNormLine = ""; meanDLine = ""; hsmnDLine = "";
                maxVal = 0;
                for (int i = 2; i < delimLine.Length - 1; i = i + 2)
                {
                    if (!shortTIRPs.Contains(i))
                        addToLines(i - 1, i, ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                }
                addToFeatures(ref tirpsFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");

            }
            tr.Close();
            
            string[] tirpMetrics = { "BIN", "HS", "MND", "NRM", "HSMND" };
            for (int i = 0; i < fullFileHeaderVec.Length; i++)
                writeToFile(matrixFileName.Replace(".csv", "_FULL_" + tirpMetrics[i] + "_MATRIX.arff"), fullFileHeaderVec[i] + fullFeaturesVec[i]);
            if (shortTIRPs.Count > 0)
            {
                for (int i = 0; i < fullFileHeaderVec.Length; i++)
                    writeToFile(matrixFileName.Replace(".csv", "_SHORT_" + tirpMetrics[i] + "_MATRIX.arff"), shortFileHeaderVec[i] + shortFeaturesVec[i]);

            }
            for (int i = 0; i < fullFileHeaderVec.Length; i++)
                writeToFile(matrixFileName.Replace(".csv", "_TIRPS_" + tirpMetrics[i] + "_MATRIX.arff"), tirpsFileHeaderVec[i] + tirpsFeaturesVec[i]);

        }

        public static void addStringToeachStringVec(ref string[] stringVec, string stringToAdd)
        {
            for (int i = 0; i < stringVec.Length; i++)
                stringVec[i] += stringToAdd;
        }

        public static void addToFeatures(ref string[] Features, string binLine, string hsLine, string meanDLine, string normLine, string hsmnDLine, double maxVal, string[] delimLine, string str)
        {
            Features[0]  += (binLine + str); // delimLine[delimLine.Length - 1] + "\n"); //binLine += delimLine[delimLine.Length - 1] + "\n";
            Features[1]  += (hsLine + str); // delimLine[delimLine.Length - 1] + "\n"); //hsLine += delimLine[delimLine.Length - 1] + "\n";
            Features[2]  += (meanDLine + str); // delimLine[delimLine.Length - 1] + "\n"); //meanDLine += delimLine[delimLine.Length - 1] + "\n";
            string[] hsVals = hsLine.Split(',');
            if (maxVal > 0)
            {
                for (int i = 0; i < hsVals.Length-1; i++)
                    normLine += ((double.Parse(hsVals[i]) / maxVal) + ",");
                Features[3]  += (normLine + str); // delimLine[delimLine.Length - 1] + "\n"); //normLine += delimLine[delimLine.Length - 1] + "\n";
            }
            else
                Features[3]  += (hsLine + str); // delimLine[delimLine.Length - 1] + "\n"); //normLine += delimLine[delimLine.Length - 1] + "\n";
            Features[4] += (hsmnDLine + str); // delimLine[delimLine.Length - 1] + "\n"); //hsmnDLine += delimLine[delimLine.Length - 1] + "\n";
        }

        public static void addToLines(int hsIdx, int mndIdx, ref double maxVal, string[] delimLine, ref string binLine, ref string hsLine, ref string meanDLine, ref string hsmnDLine)
        {
            hsLine += (delimLine[hsIdx] + ",");
            if(double.Parse(delimLine[hsIdx]) == 0)
                binLine += "0,";
            else
                binLine += "1,";
            if (maxVal < double.Parse(delimLine[hsIdx]))
                maxVal = double.Parse(delimLine[hsIdx]);
            meanDLine += (delimLine[mndIdx] + ",");
            hsmnDLine += ((double.Parse(delimLine[hsIdx]) * double.Parse(delimLine[mndIdx])) + ",");
        }*/

        /*public static void mineDirectory(string mainDir)
        {
            int counter = 0;
            foreach (string fileName in Directory.GetFiles(mainDir))
            {
                string[] fileNameParts = fileName.Split('_');
                int tonceptID = int.Parse(fileNameParts[5]);
                string outFileName = fileName;
                mineBack(mainDir, fileName, tonceptID, );
                if (counter++ > 10)
                    break;
            }
        }*/


    }
}
