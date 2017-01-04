using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class TncptTncptSTIsDic
    {
        public Dictionary<int, Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>>[] ttSTIsDic = new Dictionary<int, Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>>[7];
    }

    public class entityKarma
    {
        public int entityID;
        //public Dictionary<string, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>> instancesDic = new Dictionary<string, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>();
        public TncptTncptSTIsDic pairsTindex = new TncptTncptSTIsDic();

        public entityKarma(int setEntityID) { entityID = setEntityID; }

        public List<TimeIntervalSymbol> hasInstances(int frsTncptIdx, int Rel, int ScndTncptIdx, TimeIntervalSymbol lstTIS)
        { //string trK = frsTncptIdx.ToString() + Rel + ScndTncptIdx; return instancesDic.ContainsKey(trK); }
            //string key = frsTncptIdx.ToString() + Rel + ScndTncptIdx;
            //if (instancesDic.ContainsKey(key) && instancesDic[key].ContainsKey(lstTIS))
            if (pairsTindex.ttSTIsDic[Rel].ContainsKey(frsTncptIdx) && pairsTindex.ttSTIsDic[Rel][frsTncptIdx].ContainsKey(ScndTncptIdx) && pairsTindex.ttSTIsDic[Rel][frsTncptIdx][ScndTncptIdx].ContainsKey(lstTIS))
                return pairsTindex.ttSTIsDic[Rel][frsTncptIdx][ScndTncptIdx][lstTIS]; // return instancesDic[key][lstTIS];
            else
                return null;
        }
        
        public bool instancesDicContainsKey(int frsTncptIdx, int Rel, int ScndTncptIdx) //{ string trK = frsTncptIdx.ToString() + "-" + Rel + "-" + ScndTncptIdx; return instancesDic.ContainsKey(trK); }
        {
            return (pairsTindex.ttSTIsDic[Rel] != null && pairsTindex.ttSTIsDic[Rel].ContainsKey(frsTncptIdx) && pairsTindex.ttSTIsDic[Rel][frsTncptIdx].ContainsKey(ScndTncptIdx));
        }
        
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getInstancesDicValuebyKey(int frsTncptIdx, int Rel, int ScndTncptIdx )
        { //{ string trK = frsTncptIdx.ToString() + "-" + Rel + "-" + ScndTncptIdx; return instancesDic[trK]; }
            if (pairsTindex.ttSTIsDic[Rel].ContainsKey(frsTncptIdx) && pairsTindex.ttSTIsDic[Rel][frsTncptIdx].ContainsKey(ScndTncptIdx))
                return pairsTindex.ttSTIsDic[Rel][frsTncptIdx][ScndTncptIdx]; // return instancesDic[key][lstTIS];
            else
                return null;
        }
        
        public void addtiListDicToTTDics(int frsTncptIdx, int Rel, int ScndTncptIdx, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic)
        {
            if (pairsTindex.ttSTIsDic[Rel].ContainsKey(frsTncptIdx))
                pairsTindex.ttSTIsDic[Rel][frsTncptIdx].Add(ScndTncptIdx, tiListDic);
            else
            {
                Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>> addDic = new Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>();
                addDic.Add(ScndTncptIdx, tiListDic);
                pairsTindex.ttSTIsDic[Rel].Add(frsTncptIdx, addDic);
            }
        }

        public void indexTimeInetervalSymbolsPair(int frstTncptIdx, int rel, int scndTncptidx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            if(pairsTindex.ttSTIsDic[rel] == null) // !pairsTindex.ttSTIsDic[rel].ContainsKey(frstTncptIdx))
            {
                pairsTindex.ttSTIsDic[rel] = new Dictionary<int,Dictionary<int,Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>>>();
                Dictionary<int, Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>> scndTncptIdxDic = new Dictionary<int,Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>>();
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = new Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>();
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                tiListDic.Add(tisKey, tisList);
                scndTncptIdxDic.Add(scndTncptidx, tiListDic);
                pairsTindex.ttSTIsDic[rel].Add(frstTncptIdx, scndTncptIdxDic);
            }
            else if (!pairsTindex.ttSTIsDic[rel].ContainsKey(frstTncptIdx))
            {
                Dictionary<int, Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>> scndTncptIdxDic = new Dictionary<int,Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>>();
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = new Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>();
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                tiListDic.Add(tisKey, tisList);
                scndTncptIdxDic.Add(scndTncptidx, tiListDic);
                pairsTindex.ttSTIsDic[rel].Add(frstTncptIdx, scndTncptIdxDic);
            }
            else if (!pairsTindex.ttSTIsDic[rel][frstTncptIdx].ContainsKey(scndTncptidx))
            {
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = new Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>();
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                tiListDic.Add(tisKey, tisList);
                pairsTindex.ttSTIsDic[rel][frstTncptIdx].Add(scndTncptidx, tiListDic);
            }
            else if (!pairsTindex.ttSTIsDic[rel][frstTncptIdx][scndTncptidx].ContainsKey(tisKey))
            {
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                pairsTindex.ttSTIsDic[rel][frstTncptIdx][scndTncptidx].Add(tisKey, tisList);
            }
            else
            {
                pairsTindex.ttSTIsDic[rel][frstTncptIdx][scndTncptidx][tisKey].Add(tisVal);
            }
        }
    }

    public class KarmE
    {
        bool parallel;           public bool getParallel() { return parallel; } public void setParallel(bool setParallel) { parallel = setParallel; }
        int  print;              public int getPrint() { return print; } public void setPrint(int setPrint) { print = setPrint; }
        bool trans;              public bool getTrans() { return trans; }
                                 public void setTrans(bool seTrans) { trans = seTrans; }
        
        int     relations_style; public int getRelStyle() { return relations_style; } //number of reations (Allen7, KL3, KL2)
        int     epsilon;         public int getEpsilon() { return epsilon; }
        double  min_ver_sup;     public double getMinVerSup() { return min_ver_sup; } //the exact value of the treshold in number of entities
        double  min_hrz_sup;     public double getMinHrzSup() { return min_hrz_sup; }
        int     max_gap;         public int getMaxGap() { return max_gap; }
        int maxTirpSize;         public int getMaxTirpSize() { return maxTirpSize; }

        int[][][] transition;    // transition table
        public int getFromRelation(int leftIdx, int topIdx) { return transition[leftIdx][topIdx][0]; }
        public int getToRelation(int leftIdx, int topIdx) { return transition[leftIdx][topIdx][1]; }

        int entitieSize;
        public int getEntitieSize() { return entitieSize; }

        entityKarma[] entitiesKarmaVec;
        public entityKarma[] getEntitiesKarmaVec() { return entitiesKarmaVec; }
        public entityKarma getEntityKarmaByIdx(int idx) { return entitiesKarmaVec[idx]; }
        public bool getEnttyKrmaByIdxInstncsCntnKey(int eIdx, int frsTncptIdx, int Rel, int ScndTncptIdx) { return entitiesKarmaVec[eIdx].instancesDicContainsKey(frsTncptIdx, Rel, ScndTncptIdx); }
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getEnttyKrmaByIdxInstncsDicValbyKey(int eIdx, int frsTncptIdx, int Rel, int ScndTncptIdx) { return entitiesKarmaVec[eIdx].getInstancesDicValuebyKey(frsTncptIdx, Rel, ScndTncptIdx); }
        public List<TimeIntervalSymbol> entityHasInstances(int entityIdx, int frsTncptIdx, int Rel, int ScndTncptIdx, TimeIntervalSymbol lstTIS) { return entitiesKarmaVec[entityIdx].hasInstances(frsTncptIdx, Rel, ScndTncptIdx, lstTIS); }

        //Dictionary<string, int> glblMxRelsVerSupport = new Dictionary<string, int>(); //int[][][] glblMxRelsVerSupport;
        Dictionary<string, List<int>> glblTindexVerticalSupport = new Dictionary<string, List<int>>();
        public int getglblTindexVerticalSupport(int frstTncptIdx, int scndTncptIdx, int relation) 
        {
            if ( glblTindexVerticalSupport.ContainsKey(frstTncptIdx + "-" + scndTncptIdx + "-" + relation))
                return glblTindexVerticalSupport[frstTncptIdx + "-" + scndTncptIdx + "-" + relation].Count; // glblMxRelsVerSupport[frstTncptIdx][scndTncptIdx][relation]; }
            else
                return 0;
        }      
        
        public void increaseGlblTindexVerticalSupport(int frstTncptIdx, int scndTncptIdx, int relation, int eIdx) 
        {
            if (!glblTindexVerticalSupport.ContainsKey(frstTncptIdx + "-" + scndTncptIdx + "-" + relation))
            {
                List<int> eIdxList = new List<int>();
                eIdxList.Add(eIdx);
                glblTindexVerticalSupport.Add(frstTncptIdx + "-" + scndTncptIdx + "-" + relation, eIdxList);
            }
            else if (!glblTindexVerticalSupport[frstTncptIdx + "-" + scndTncptIdx + "-" + relation].Contains(eIdx))
                glblTindexVerticalSupport[frstTncptIdx + "-" + scndTncptIdx + "-" + relation].Add(eIdx);
        }

        /*Dictionary<string, int> glblMxRelsHrzSupport = new Dictionary<string, int>(); //int[][][] glblMxRelsHrzSupport;
        public int getglblMxRelsHrzSupport(int frstTncptIdx, int scndTncptIdx, int relation) 
        {
            if (glblMxRelsHrzSupport.ContainsKey(frstTncptIdx + "-" + scndTncptIdx + "-" + relation))
                return glblMxRelsHrzSupport[frstTncptIdx + "-" + scndTncptIdx + "-" + relation];
            else
                return 0;
        }
        public void increaseGlblMxRelsHrzSupport(int frstTncptIdx, int scndTncptIdx, int relation) 
        {
            if (!glblMxRelsHrzSupport.ContainsKey(frstTncptIdx + "-" + scndTncptIdx + "-" + relation))
                glblMxRelsHrzSupport.Add(frstTncptIdx + "-" + scndTncptIdx + "-" + relation, 1);
            else
                glblMxRelsHrzSupport[frstTncptIdx + "-" + scndTncptIdx + "-" + relation]++;
        }*/

        Dictionary<int, List<TimeIntervalSymbol>> entityTISs;
        public List<TimeIntervalSymbol> getEntityTISsByIdx(int eIdx) { return entityTISs.ElementAt(eIdx).Value; }
        
        Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();
        public Dictionary<int, TemporalConcept>.ValueCollection getToncepts() { return toncepts.Values; }
        public void sorToncepts() { toncepts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value); }
        public int getTonceptByIDVerticalSupport(int t_id) { return toncepts[t_id].getTonceptVerticalSupport(); } //public TemporalConcept getTonceptByID(int t_id) { return toncepts[t_id]; }
        
        public int getTonceptIndexByID(int t_id) { return toncepts[t_id].tonceptINDEX; }
        public TemporalConcept getTonceptByOrder(int idx) { return toncepts.Values.ElementAt(idx); }

        //public Dictionary<int, TemporalConcept> getToncepts() { return toncepts; }
        public TemporalConcept getTonceptByID(int t_id) { if (containsTonceptID(t_id)) return toncepts[t_id]; else return null; }
        public bool containsTonceptID(int t_id) { return toncepts.ContainsKey(t_id); }

        int forBackWards; public int getForBackWardsMining() { return forBackWards; }
        // the trick here is that the order of the toncepts is opposite. the indices of the relations are the same, but they are ordered in a different way
        //forward mining |0|1|3|6|      backwards mininig |9|8|7|6|
        //                 |2|4|7|                          |5|4|3|
        //                   |5|8|                            |2|1|
        //                     |9|                              |0|
        // the trick is with the priniting, for which we have int[][]    logiRelsIndxs;
        int[][] logiRelsIndxs; public int[][] getBackwardsRelsIndxs() { return logiRelsIndxs; }
        
        /*
        public static void runKLD(int legologi, int rels, int epsilon, int maxGap, int minSup, string klFile, bool seTrans, string mXprntD, string outFolder, string outFile, ref string karmaDTime, ref string legoDTime)
        {
            DateTime starTime = DateTime.Now;
            KarmaLegoLib.KarmaD kD = new KarmaD(legologi, KLC.RELSTYLE_ALLEN7, epsilon, maxGap, minSup, klFile, seTrans); 
            kD.RunKarmaRun(); 
            if(mXprntD != "")
                kD.printMatrixEntries(mXprntD);
            karmaDTime = (DateTime.Now - starTime).Minutes + ":" + (DateTime.Now - starTime).Seconds;
            starTime = DateTime.Now;
            kD.RunLegoALL(outFolder, seTrans, KLC.KL_PRINT_YES, outFile);
            legoDTime = (DateTime.Now - starTime).Minutes + ":" + (DateTime.Now - starTime).Seconds;
            starTime = DateTime.Now;
            kD = null;
        }*/
        
        //public KarmaD(int setKarmaLegoLogi, int setRelationStyle, int setEpsilon, int setMaxGap, int setMinVerSup, string dsFilePath, bool seTrans)
        public KarmE(int setForBackWards, int setRelationStyle, int setEpsilon, int setMaxGap, int setMinVerSup, double setMinHrzSup, string dsFilePath, bool seTrans, bool setHS1, int setPrint, bool setParallel , ref string runTime, int setMaxTIRPSize, int eLast = KLC.NUM_OF_ENTITIES, int eFrst = 0)
        {
            parallel = setParallel;
            //karmalegologi = setKarmaLegoLogi;
            DateTime starTime = DateTime.Now;
            forBackWards = setForBackWards;
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            maxTirpSize = setMaxTIRPSize;
            //read_tids_file(0, KLC.NUM_OF_ENTITIES, dsFilePath, ref entityTISs, ref toncepts);
            read_tids_file(eFrst, eLast, dsFilePath, ref entityTISs, ref toncepts);
            entitieSize = entityTISs.Count;
            min_ver_sup = ((double)setMinVerSup / 100) * entitieSize;
            min_hrz_sup = setMinHrzSup;
            entitiesKarmaVec = new entityKarma[entitieSize];
            int eIdx = 0;
            foreach (int eKey in entityTISs.Keys)
                entitiesKarmaVec[eIdx++] = new entityKarma(eKey);

            //glblMxRelsVerSupport = new int[KLC.NUM_OF_SYMBOLS][][];
            //glblMxRelsHrzSupport = new int[KLC.NUM_OF_SYMBOLS][][];
            //for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
            //{
            //    glblMxRelsVerSupport[i] = new int[KLC.NUM_OF_SYMBOLS][];
            //    glblMxRelsHrzSupport[i] = new int[KLC.NUM_OF_SYMBOLS][];
            //    for (int j = 0; j < KLC.NUM_OF_SYMBOLS; j++)
            //    {
            //        glblMxRelsVerSupport[i][j] = new int[relations_style];
            //        glblMxRelsHrzSupport[i][j] = new int[relations_style];
            //    }
            //}

            print = setPrint;
            string tesTrans;
            if (seTrans)
            {
                if(relations_style == KLC.RELSTYLE_ALLEN7 )
                    tesTrans = KLC.LoadTransitionTableALLEN7(ref transition);//load transition table
                else
                    tesTrans = KLC.LoadTransitionTableKL3(ref transition);//load transition table
                if (tesTrans.Length > 0)
                    transition = null;
            }

            if (forBackWards == KLC.backwardsMining)
            {
                logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
                logiRelsIndxs[0/*2*/]  = new int[1]  { 0 };
                logiRelsIndxs[1/*3*/]  = new int[3]  { 2, 1, 0 };
                logiRelsIndxs[2/*4*/]  = new int[6]  { 5, 4, 2, 3, 1, 0 };
                logiRelsIndxs[3/*5*/]  = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
                logiRelsIndxs[4/*6*/]  = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
                logiRelsIndxs[5/*7*/]  = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[6/*8*/]  = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[7/*9*/]  = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[8/*10*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
                
            }

            if (runTime != "")
                RunKarmaRun();

            DateTime endTime = DateTime.Now;
            runTime = endTime.Subtract(starTime).TotalMilliseconds.ToString();

        }

        // single
        public KarmE( int setForBackWards, int setRelationStyle, int setEpsilon, int setMaxGap, string dsFilePath, bool runKarma, int setMaxTirpSize, int eFrst, int eLast)
        {
            parallel = false; // setParallel;
            //karmalegologi = setKarmaLegoLogi;
            //DateTime starTime = DateTime.Now;
            forBackWards = setForBackWards;
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            maxTirpSize = setMaxTirpSize;
            read_tids_file(eFrst, eLast, dsFilePath, ref entityTISs, ref toncepts); //KLC.NUM_OF_ENTITIES, dsFilePath, ref entityTISs, ref toncepts);
            entitieSize = entityTISs.Count;
            min_ver_sup = 0; // ((double)setMinVerSup / 100) * entitieSize;
            entitiesKarmaVec = new entityKarma[entitieSize];
            int eIdx = 0;
            foreach (int eKey in entityTISs.Keys)
                entitiesKarmaVec[eIdx++] = new entityKarma(eKey);

            print = 1; // setPrint;
            
            if (forBackWards == KLC.backwardsMining)
            {
                logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
                logiRelsIndxs[0/*2*/] = new int[1] { 0 };
                logiRelsIndxs[1/*3*/] = new int[3] { 2, 1, 0 };
                logiRelsIndxs[2/*4*/] = new int[6] { 5, 4, 2, 3, 1, 0 };
                logiRelsIndxs[3/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
                logiRelsIndxs[4/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
                logiRelsIndxs[5/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[6/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[7/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[8/*10*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };

            }

            if(runKarma == true)
                RunKarmaRun();

        }

        public static string read_tids_file(int frstEntIdx, int lstEntIdx, string filePath, ref Dictionary<int, List<TimeIntervalSymbol>> entityTISs, ref Dictionary<int, TemporalConcept> toncepts)
        {
            string time = "";
            DateTime starTime = DateTime.Now;
            try
            {
                //if (!File.Exists(filePath))
                //    return null;
                TemporalConcept tc;
                entityTISs = new Dictionary<int, List<TimeIntervalSymbol>>();
                TextReader tr = new StreamReader(filePath);
                string readLine = tr.ReadLine();
                //read the variables dictionary?
                readLine = tr.ReadLine();
                if (readLine == "startToncepts")
                {
                    while (tr.Peek() >= 0) //read the toncepts
                    {
                        readLine = tr.ReadLine();
                        if (readLine.StartsWith("numberOfEntities") )
                            break;
                        int binSize = int.Parse( readLine.Split(',')[3] ); // add the original variable ID
                        for (int i = 0; i < binSize; i++)
                        {
                            readLine = tr.ReadLine();
                            int tonceptID = int.Parse( readLine.Split(',')[2] );
                            tc = new TemporalConcept(tonceptID, toncepts.Count);
                            toncepts.Add(tonceptID, tc);
                        }
                    }
                }
                time = time + DateTime.Now.Subtract(starTime).TotalMilliseconds.ToString() + " ";
                int entitiesCounter = 0;
                while (tr.Peek() >= 0 ) // && entitiesCounter < lstEntIdx) //read the entities and their symbolic time intervals
                {
                    readLine = tr.ReadLine();
                    if (entitiesCounter >= frstEntIdx && entitiesCounter < lstEntIdx) //entitiesCounter >= frstEntIdx)
                    {
                        string[] mainDelimited = readLine.Split(';');
                        string entityID = mainDelimited[0].Split(',')[0];
                        readLine = tr.ReadLine();
                        mainDelimited = readLine.Split(';');
                        List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();

                        /*if (!mainDelimited[0].Contains(','))
                        {
                            int endOfObservation = int.Parse(mainDelimited[0]);
                            TimeIntervalSymbol tis = new TimeIntervalSymbol(0, 0, endOfObservation); // the time-point of the end of the observation-time, or prediction-time before the outcome
                            tisList.Add(tis);
                        }*/

                        for (int i = 0; i < mainDelimited.Length - 1; i++)
                        {
                            string[] tisDelimited = mainDelimited[i].Split(',');
                            int symbol = int.Parse(tisDelimited[2]);
                            TimeIntervalSymbol tis = new TimeIntervalSymbol(int.Parse(tisDelimited[0]), int.Parse(tisDelimited[1]), symbol); // int.Parse(tisDelimited[2]));
                            addToncept(entityTISs.Count, tis, ref toncepts);
                            tisList.Add(tis);
                        }
                        tisList.Sort(); ;
                        entityTISs.Add(int.Parse(entityID), tisList);
                    }
                    else
                        readLine = tr.ReadLine();
                    entitiesCounter++;
                }
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            time = time + DateTime.Now.Subtract(starTime).TotalMilliseconds.ToString() + " ";
            return time;
        }

        public void RunKarmaRun()
        {
            int allGapCnt = 0;
            try
            {
                int[] relStat = new int[7];
                /*if( parallel == KLC.parallelMining )
                    Parallel.For(0, entitieSize, eIdx =>
                    {
                        KarmaEntity(eIdx, ref relStat);
                    });
                else*/
                for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                        allGapCnt += KarmaEntity(eIdx, ref relStat);
                    //sumGlobalVerticalSupport();
                //pruneToncepts();
            //pruneMatrixEntriesPairDics();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private int KarmaEntity(int eIdx, ref int[] relStat)
        {
            int allGapCnt = 0, counter = 0;
            List<TimeIntervalSymbol> tisList = entityTISs[entitiesKarmaVec[eIdx].entityID];
            double allGap = tisList.ElementAt(tisList.Count - 1).startTime - tisList.ElementAt(0).endTime;
            if (allGap > 365)
                allGapCnt++;
            for (int ti1Idx = 0; ti1Idx < tisList.Count; ti1Idx++)
            {
                addToncept(eIdx, tisList.ElementAt(ti1Idx), ref toncepts);//, true); // update toncept entity support
                for (int ti2Idx = ti1Idx + 1; ti2Idx < tisList.Count; ti2Idx++)
                {
                    //if (tisList.ElementAt(ti1Idx).symbol != tisList.ElementAt(ti2Idx).symbol)
                    {
                        //addToncept(eIdx, tisList.ElementAt(ti2Idx), ref toncepts);//, false); // DONT update toncept entity support
                        int relation = KLC.WhichRelationEpsilon(tisList.ElementAt(ti1Idx), tisList.ElementAt(ti2Idx), relations_style, epsilon, max_gap);
                        if (relation > -1)
                        {
                            relStat[relation]++;
                            indexTISPair(tisList.ElementAt(ti1Idx), tisList.ElementAt(ti2Idx), relation, eIdx);
                            counter++;
                            if (relation == 0)
                            {
                                ti2Idx++;
                                while (ti2Idx < tisList.Count)
                                {
                                    relation = KLC.WhichRelationEpsilon(tisList.ElementAt(ti1Idx), tisList.ElementAt(ti2Idx), relations_style, epsilon, max_gap);
                                    double gap = tisList.ElementAt(ti2Idx).startTime - tisList.ElementAt(ti1Idx).endTime;
                                    if (gap >= max_gap || relation == -1)
                                    {
                                        ti2Idx = tisList.Count;
                                        break;
                                    }
                                    //if (tisList.ElementAt(ti1Idx).symbol != tisList.ElementAt(ti2Idx).symbol)
                                    {
                                        //addToncept(eIdx, tisList.ElementAt(ti2Idx), ref toncepts);//, false); // DONT update toncept entity support
                                        relStat[relation]++;
                                        indexTISPair(tisList.ElementAt(ti1Idx), tisList.ElementAt(ti2Idx), relation, eIdx);
                                        counter++;
                                    }
                                    ti2Idx++;
                                }
                            }
                        }
                        else
                            break;
                    }
                }
            }
            if (allGap > 365)
                return 1;
            else
                return 0;
            
        }

        /*
        public void RunLegoALL(string outFolder, bool seTrans, bool setPrint, string outFile)
        {
            string tempFolderPath = outFolder + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "\\";
            Directory.CreateDirectory(tempFolderPath);
            //Parallel.ForEach(toncepts, toncept =>
            foreach(KeyValuePair<int,TemporalConcept> toncept in toncepts)
            {
                if (getTonceptByID(toncept.Value.tonceptID).getTonceptVerticalSupport() > min_ver_sup)
                {
                    //LegoD logi = new LegoD(seTrans, setPrint, this);
                    LegoD_NCG logi = new LegoD_NCG(seTrans, setPrint, this); 
                    logi.runLego(toncept.Value.tonceptINDEX, toncept.Value.tonceptID, tempFolderPath);
                    logi = null;
                }
            } //  );
            WriteKLFileFromTonceptsFiles(outFile, tempFolderPath); // outFolder);
        }*/

        public static void WriteKLFileFromTonceptsFiles(string outFile, string tempFolderPath)
        {
            using (StreamWriter sw = new StreamWriter(outFile))
                foreach (string txtName in Directory.GetFiles(tempFolderPath))
                    using (StreamReader sr = new StreamReader(txtName))
                        sw.Write(sr.ReadToEnd());
            Directory.Delete(tempFolderPath, true);
        }

        /*
        public void RunLogiTONCEPT(int trgTncpt, string outFile, bool seTrans, bool setPrint)
        {
            LegoD logi = new LegoD(seTrans, setPrint, this);
            int tTrgtIdx = toncepts[trgTncpt].tonceptINDEX;
            int tTrgtID = trgTncpt;
            logi.runLego(tTrgtIdx, tTrgtID, outFile);
        }*/

        public TIRP getTwoSizedTIRP(int tTrgtID, int tErlyID, int rel)
        {
            TIRP twoSzdTIRP = new TIRP(tTrgtID, tErlyID, rel);
            string trK = toncepts[tTrgtID].tonceptINDEX + "-" + rel + "-" + toncepts[tErlyID].tonceptINDEX;
            for (int eIdx = 0; eIdx < entitieSize; eIdx++) //foreach(entityKarma eK in entitiesKarmaVec)
                if (entitiesKarmaVec[eIdx].instancesDicContainsKey(toncepts[tTrgtID].tonceptINDEX, rel, toncepts[tErlyID].tonceptINDEX)) // instancesDic.ContainsKey(trK))
                {
                    Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = entitiesKarmaVec[eIdx].getInstancesDicValuebyKey(toncepts[tTrgtID].tonceptINDEX, rel, toncepts[tErlyID].tonceptINDEX); // .instancesDic[trK];
                    foreach (KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListDic)
                        foreach (TimeIntervalSymbol tis in tiListTi.Value)
                        {
                            TIsInstance tisInsNew = new TIsInstance(tiListTi.Key, tis, eIdx);
                            twoSzdTIRP.tinstancesList.Add(tisInsNew);
                            twoSzdTIRP.meanHorizontalSupport++;
                        }
                    twoSzdTIRP.addEntity(entitiesKarmaVec[eIdx].entityID);
                }
            twoSzdTIRP.meanHorizontalSupport = twoSzdTIRP.meanHorizontalSupport / twoSzdTIRP.entitieVerticalSupport.Count();
            return twoSzdTIRP;
        }

        public static void addToncept(int entityIdx, TimeIntervalSymbol tis, ref Dictionary<int, TemporalConcept> toncepts)//, bool updateTonceptEntitiesSupport)
        {
            TemporalConcept tc;
            if (!toncepts.ContainsKey(tis.symbol))
            {
                tc = new TemporalConcept(tis.symbol, toncepts.Count);
                toncepts.Add(tis.symbol, tc);
            }
            else
                tc = toncepts[tis.symbol];
            //if (updateTonceptEntitiesSupport)
            //    tc.entitiesSupport[entityIdx] = true;
            tc.addEntityTinstance(entityIdx, tis);  // tc.addEntity(entityIdx);
        }

        private void indexTISPair(TimeIntervalSymbol firsTis, TimeIntervalSymbol secondTis, int relation, int eIdx)
        {
            if (forBackWards == KLC.forwardMining) // karmalegologi == KLC.forwardMining) // KarmaLego)
                indexTimeInetervalSymbolsPair(firsTis, secondTis, relation, eIdx);
            else
                indexTimeInetervalSymbolsPair(secondTis, firsTis, relation, eIdx);
        }

        private void indexTimeInetervalSymbolsPair(TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal, int relation, int entityIdx)
        {
            int tKeyIdx = toncepts[tisKey.symbol].tonceptINDEX;
            int tValIdx = toncepts[tisVal.symbol].tonceptINDEX;
            /*if (entitiesKarmaVec[entityIdx].instancesDic.ContainsKey(trkey))
            {
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = entitiesKarmaVec[entityIdx].getInstancesDicValuebyKey(tKeyIdx, relation, tValIdx); // .instancesDic[trkey];
                if (tiListDic.ContainsKey(tisKey))
                    tiListDic[tisKey].Add(tisVal);
                else
                {
                    List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                    tisList.Add(tisVal);
                    tiListDic.Add(tisKey, tisList);
                }
                
            }
            else
            {
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = new Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>(); 
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                tiListDic.Add(tisKey, tisList);
                entitiesKarmaVec[entityIdx].addtiListDicToTTDics(tKeyIdx, relation, tValIdx, tiListDic); // .instancesDic.Add(trkey, tiListDic);
                
                //toncepts[tisKey.symbol].entIdxKeyList.Add(entityIdx + ";" + trkey); //(entitiesKarmaVec[entityIdx].instancesDic.Count - 1));
                //toncepts[tisVal.symbol].entIdxKeyList.Add(entityIdx + ";" + trkey); //"-" + (entitiesKarmaVec[entityIdx].instancesDic.Count - 1));
                
                increaseGlblMxRelsVerSupport(tKeyIdx, tValIdx, relation); //glblMxRelsVerSupport[tKeyIdx][tValIdx][relation]++;
            }*/

            entitiesKarmaVec[entityIdx].indexTimeInetervalSymbolsPair(tKeyIdx, relation, tValIdx, tisKey, tisVal); //increaseGlblMxRelsHrzSupport(tKeyIdx,tValIdx,relation); //glbllsHrzSupport[tKeyIdx][tValIdx][relation]++;
            increaseGlblTindexVerticalSupport(tKeyIdx, tValIdx, relation, entityIdx);
            
            //check this
            /*string trkey = tKeyIdx + "-" + relation + "-" + tValIdx;
            if (!toncepts[tisKey.symbol].entIdxKeyList.Contains(entityIdx + ";" + trkey))
                toncepts[tisKey.symbol].entIdxKeyList.Add(entityIdx + ";" + trkey); //(entitiesKarmaVec[entityIdx].instancesDic.Count - 1));
            if (!toncepts[tisVal.symbol].entIdxKeyList.Contains(entityIdx + ";" + trkey)) 
                toncepts[tisVal.symbol].entIdxKeyList.Add(entityIdx + ";" + trkey); //"-" + (entitiesKarmaVec[entityIdx].instancesDic.Count - 1));
            */ 
            
        }

        private void sumGlobalVerticalSupport()
        {
            for (int eIdx = 0; eIdx < entitiesKarmaVec.Count(); eIdx++)
            {
                for (int relIdx = 0; relIdx < relations_style; relIdx++)
                {
                    if (entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx] != null)
                    {
                        for (int frstIdxKey = 0; frstIdxKey < entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].Count(); frstIdxKey++)
                        {
                            int tKeyIdx = entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Key;
                            for (int scndIdxKey = 0; scndIdxKey < entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Value.Count(); scndIdxKey++)
                            {
                                int tValIdx = entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Value.ElementAt(scndIdxKey).Key;
                                increaseGlblTindexVerticalSupport(tKeyIdx, tValIdx, relIdx, eIdx);
                                //string trkey = frstIdxKey + "-" + relIdx + "-" + scndIdxKey;
                                //if (!toncepts.ElementAt(tKeyIdx).Value.entIdxKeyList.Contains(eIdx + ";" + trkey)) // [entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Key].entIdxKeyList.Contains(eIdx + ";" + trkey))
                                //    toncepts.ElementAt(tKeyIdx).Value.entIdxKeyList.Add(eIdx + ";" + trkey); // [entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Key].entIdxKeyList.Add(eIdx + ";" + trkey); //(entitiesKarmaVec[entityIdx].instancesDic.Count - 1));
                                //if (!toncepts.ElementAt(tValIdx).Value.entIdxKeyList.Contains(eIdx + ";" + trkey)) // [entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Value.ElementAt(scndIdxKey).Key].entIdxKeyList.Contains(eIdx + ";" + trkey))
                                //    toncepts.ElementAt(tValIdx).Value.entIdxKeyList.Add(eIdx + ";" + trkey); // [entitiesKarmaVec[eIdx].pairsTindex.ttSTIsDic[relIdx].ElementAt(frstIdxKey).Value.ElementAt(scndIdxKey).Key].entIdxKeyList.Add(eIdx + ";" + trkey); //"-" + (entitiesKarmaVec[entityIdx].instancesDic.Count - 1));
                            }
                        }
                    }
                }
            }
        }

        public void printMatrixEntries(string filePath)
        {
            TextWriter tw = new StreamWriter(filePath);
            foreach (TemporalConcept toncept1 in toncepts.Values)
            {
                int t1Idx = toncept1.tonceptINDEX;
                foreach (TemporalConcept toncept2 in toncepts.Values)
                {
                    int t2Idx = toncept2.tonceptINDEX;
                    for (int r = 0; r < relations_style; r++)
                        if (getglblTindexVerticalSupport(t1Idx,t2Idx,r) > min_ver_sup) // glblMxRelsVerSupport[t1Idx][t2Idx][r] > min_ver_sup)
                            tw.WriteLine(toncept1.tonceptID + "," + toncept2.tonceptID + "," + r + "," + getglblTindexVerticalSupport(t1Idx,t2Idx,r) + ","); // + getglblMxRelsHrzSupport(t1Idx, t2Idx, r)); // glblMxRelsVerSupport[t1Idx][t2Idx][r] + "," + glblMxRelsHrzSupport[t1Idx][t2Idx][r]);
                }
            }
            tw.Close();

        }
        /*
        private void pruneToncepts()
        {
            for (int t = 0; t < toncepts.Count; t++)
            {
                TemporalConcept tc = toncepts.Values.ElementAt(t);
                if (tc.getTonceptVerticalSupport() < min_ver_sup)
                {
                    for(int i = 0; i < tc.entIdxKeyList.Count; i++)
                    {
                        string[] userPair = tc.entIdxKeyList.ElementAt(i).Split(';');
                        entityKarma eK = entitiesKarmaVec[int.Parse(userPair[0])];
                        //entitiesKarmaVec[int.Parse(userPair[0])].instancesDic.Remove(entitiesKarmaVec[int.Parse(userPair[0])].instancesDic.ElementAt(int.Parse(userPair[1])).Key);
                         //eK.instancesDic.Remove(userPair[1]); // eK.instancesDic.ElementAt(int.Parse(userPair[1])).Key);
                    }
                    toncepts.Remove(toncepts.Keys.ElementAt(t));
                    t--;
                }
                tc.entIdxKeyList.Clear();
            }
            for (int v = 0; v < glblTindexVerticalSupport.Count(); v++)
                if (glblTindexVerticalSupport.ElementAt(v).Value.Count() < min_ver_sup)
                {
                    glblTindexVerticalSupport.Remove(glblTindexVerticalSupport.ElementAt(v).Key);
                    v--;
                }
        }*/

        public TIRP getMatrixDICasTIRP(int tTrgtID, int tErlyID, int rel)
        {
            TIRP twoSzdTIRP = new TIRP(tTrgtID, tErlyID, rel);
            //string trK = toncepts[tTrgtID].tonceptINDEX + "-" + rel + "-" + toncepts[tErlyID].tonceptINDEX;
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
            {
                if (entitiesKarmaVec[eIdx].instancesDicContainsKey(toncepts[tTrgtID].tonceptINDEX, rel, toncepts[tErlyID].tonceptINDEX)) // .instancesDic.ContainsKey(trK))
                {
                    Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = entitiesKarmaVec[eIdx].getInstancesDicValuebyKey(toncepts[tTrgtID].tonceptINDEX, rel, toncepts[tErlyID].tonceptINDEX); // .instancesDic[trK];
                    foreach (KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListDic) // for(int i = 0; i < tiListDic.Count; i++)
                    {
                        foreach (TimeIntervalSymbol tis in tiListTi.Value)
                        {
                            TIsInstance tisInsNew = new TIsInstance(tiListTi.Key, tis, eIdx);
                            twoSzdTIRP.addEntity(eIdx); // int.Parse(tiListTi.Key.Split('-')[0]));
                            twoSzdTIRP.tinstancesList.Add(tisInsNew);
                        }
                        twoSzdTIRP.meanHorizontalSupport += tiListTi.Value.Count();
                    }
                }

            }
            twoSzdTIRP.meanHorizontalSupport = twoSzdTIRP.meanHorizontalSupport / twoSzdTIRP.entitieVerticalSupport.Count();
            return twoSzdTIRP;
        }

        /*private void pruneEntKrmMx()
        {
            foreach (TemporalConcept toncept1 in toncepts.Values)
            {
                int t1Idx = toncept1.tonceptINDEX;
                foreach (TemporalConcept toncept2 in toncepts.Values)
                {
                    int t2Idx = toncept2.tonceptINDEX;
                    for (int r = 0; r < relations_style; r++)
                        if (getglblMxRelsVerSupport(t1Idx,t2Idx,r) <= min_ver_sup) // glblMxRelsVerSupport[t1Idx][t2Idx][r] <= min_ver_sup)
                        {
                            //string trK = t1Idx.ToString() + r + t2Idx;
                            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                                if (entitiesKarmaVec[eIdx].instancesDicContainsKey(t1Idx, r, t2Idx))// .instancesDic.ContainsKey(trK))
                                    entitiesKarmaVec[eIdx].instancesDic.Remove(trK);
                        }
                }
            }
        }*/

    }
}
