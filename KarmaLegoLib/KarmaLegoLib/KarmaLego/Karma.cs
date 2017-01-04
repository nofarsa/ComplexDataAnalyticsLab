using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Threading.Tasks;

namespace KarmaLegoLib
{
    public class EntitiesVecOfstiListDic
    {
        
        Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] tiListDicsEntitiesVector; // = new Dictionary<TimeIntervalSymbol,List<TimeIntervalSymbol>>[];
        private List<int> verticalSupport = new List<int>();
        public void addEntity(int entityIdx) 
        {
            if (!verticalSupport.Contains(entityIdx))
                verticalSupport.Add(entityIdx); 
        } //public bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
        public int getVerticalSupport() { return verticalSupport.Count(); }
        public int horizontalSupport;
        
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] getEntitiesVec() { return tiListDicsEntitiesVector; }
        public EntitiesVecOfstiListDic(int entitieSize) 
        { 
            tiListDicsEntitiesVector = new Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[entitieSize];
            for (int i = 0; i < entitieSize; i++)
               tiListDicsEntitiesVector[i] = new Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>();
        }
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getEntityDic(int eIdx) { return tiListDicsEntitiesVector[eIdx]; }
        public List<TimeIntervalSymbol> getTisListbyEIdxTis(int eIdx, TimeIntervalSymbol tis) { if(tiListDicsEntitiesVector[eIdx].ContainsKey(tis)) return tiListDicsEntitiesVector[eIdx][tis]; else return null; }
        public void indexByEidxSTIs(int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            if(tiListDicsEntitiesVector[eIdx].ContainsKey(tisKey))
                tiListDicsEntitiesVector[eIdx][tisKey].Add(tisVal);
            else
            {
                List<TimeIntervalSymbol> stiList = new List<TimeIntervalSymbol>();
                stiList.Add(tisVal);
                tiListDicsEntitiesVector[eIdx].Add(tisKey, stiList);
                addEntity(eIdx);
                horizontalSupport++;
            }
        }
    }

    public class tindexRelEntry
    {
        int entitieSize;
        int relSize;

        public EntitiesVecOfstiListDic[] relsVecOfEntitiesDics;
        public tindexRelEntry(int seTentitieSize, int seTrelSize) 
        { 
            entitieSize = seTentitieSize;
            relSize = seTrelSize;
            relsVecOfEntitiesDics = new EntitiesVecOfstiListDic[relSize]; 
            for (int i = 0; i < relSize; i++) 
                relsVecOfEntitiesDics[i] = new EntitiesVecOfstiListDic(entitieSize);
        }
        public List<TimeIntervalSymbol> getRidxEidxTisList(int rIdx, int eIdx, TimeIntervalSymbol tis) { return relsVecOfEntitiesDics[rIdx].getTisListbyEIdxTis(eIdx, tis); }
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getRelEntityDic(int rel, int eIdx) 
        {
            return relsVecOfEntitiesDics[rel].getEntityDic(eIdx); 
        }
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] getRelEntitiesDics(int rel) { return relsVecOfEntitiesDics[rel].getEntitiesVec(); }
        public void indexByRelEidxSTIs(int rel, int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            ////if (relsVecOfEntitiesDics[rel] == null)
            ////    relsVecOfEntitiesDics[rel] = new EntitiesVecOfstiListDic(relSize);
            relsVecOfEntitiesDics[rel].indexByEidxSTIs(eIdx, tisKey, tisVal);
            //addEntity(eIdx);
            //horizontalSupport++;
        }

        //public List<int> verticalSupport = new List<int>(); 
        //public void addEntity(int entityIdx) { if (!verticalSupport.Contains(entityIdx)) verticalSupport.Add(entityIdx); } //public bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
        //public int horizontalSupport;
    }

    public class tindex
    {
        public tindexRelEntry[,] Kindex = new tindexRelEntry[KLC.NUM_OF_SYMBOLS, KLC.NUM_OF_SYMBOLS];
        public tindex(int seTentitieSize, int seTrelSize)
        {
            for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
                for (int j = 0; j < KLC.NUM_OF_SYMBOLS; j++)
                    Kindex[i, j] = new tindexRelEntry(seTentitieSize, seTrelSize);
        }
        public int getTindexRelVerticalSupport(int frstTndx, int scndTndx, int rel) 
        { 
            return Kindex[frstTndx, scndTndx].relsVecOfEntitiesDics[rel].getVerticalSupport(); 
        }
        public int getTindexRelHorizontalSupport(int frstTndx, int scndTndx, int rel) { return Kindex[frstTndx,scndTndx].relsVecOfEntitiesDics[rel].horizontalSupport; }
        public List<TimeIntervalSymbol> getTindexRelEidxTisList(int frstTndx, int scndTndx, int rel, int eIdx, TimeIntervalSymbol tis) { return Kindex[frstTndx, scndTndx].relsVecOfEntitiesDics[rel].getTisListbyEIdxTis(eIdx, tis); }
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getTindexRelEidxDic(int frstTndx, int scndTndx, int rel, int eIdx)
        {
            if (frstTndx > -1 && scndTndx > -1)
                return Kindex[frstTndx, scndTndx].getRelEntityDic(rel, eIdx);
            else
                return null;
        }
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] getTindexRelEntitiesDics(int frstTndx, int scndTndx, int rel) { return Kindex[frstTndx, scndTndx].getRelEntitiesDics(rel); }
        public void indexTTsRelEidxSTIs(int frstTncptIdx, int scndTncptIdx, int rel, int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            //if (Kindex[frstTncptIdx, scndTncptIdx] == null)
            //    Kindex[frstTncptIdx, scndTncptIdx] = new tindexRelEntry(7);
            Kindex[frstTncptIdx, scndTncptIdx].indexByRelEidxSTIs(rel, eIdx, tisKey, tisVal);
            //Kindex[frstTncptIdx, scndTncptIdx].relsVecOfEntitiesDics[rel].addEntity(eIdx);
        }
    }

    public class Karma
    {
        long mxAccsCounter;   public void   resetMxAccsCounter()    { mxAccsCounter = 0; } 
                              public long   getMxAccsCounter()      { return mxAccsCounter; }
        bool HS1;             public bool   getHS1()                { return HS1;   }
        int print;            public int    getPrint()              { return print; }
        bool trans;           public bool   getTrans()              { return trans; }    
                              public void   setTrans(bool seTrans)  { trans = seTrans; }
        int  relations_style; public int    getRelStyle()           { return relations_style; }  //number of reations (Allen7, KL3, KL2)
        int  epsilon;         public int    getEpsilon()            { return epsilon; }
        double min_ver_sup;   public double getMinVerSup()          { return min_ver_sup; }       //the exact value of the treshold in number of entities
        int  max_gap;         public int    getMaxGap()             { return max_gap; }
        int[][][] transition; public int    getFromRelation(int leftIdx, int topIdx) { return transition[leftIdx][topIdx][0]; }
                              public int    getToRelation(int leftIdx, int topIdx)   { return transition[leftIdx][topIdx][1]; }
        int entitieSize;      public int    getEntitieSize()        { return entitieSize; }
        int[] entitiesVec;    public int[]  getEntitiesVec()        { return entitiesVec; }
                              public int    getEntityByIdx(int idx) { return entitiesVec[idx]; }
        public Dictionary<int, List<TimeIntervalSymbol>> entityTISs;
        Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();
                              public Dictionary<int, TemporalConcept>.ValueCollection getToncepts() { return toncepts.Values; }
                              public int getTonceptByIDVerticalSupport(int t_id) { return toncepts[t_id].getTonceptVerticalSupport(); } //public TemporalConcept getTonceptByID(int t_id) { return toncepts[t_id]; }
                              public int getTonceptIndexByID(int t_id) { if (toncepts.ContainsKey(t_id)) return toncepts[t_id].tonceptINDEX; else return -1; }
                              public TemporalConcept getTonceptByOrder(int idx) { return toncepts.Values.ElementAt(idx); }
                              public TemporalConcept getTonceptByID(int t_id) { if (containsTonceptID(t_id)) return toncepts[t_id]; else return null; }
                                
                              public bool containsTonceptID(int t_id) { return toncepts.ContainsKey(t_id); }
                              //public int getTonceptByIdHorizontalSupport(int t_id) { if (containsTonceptID(t_id)) return toncepts[t_id].getTonceptHorizontalDic().Count(); else return 0; }
        int forBackWards; public int getForBackWardsMining() { return forBackWards; }
        // the trick here is that the order of the toncepts is opposite. the indices of the relations are the same, but they are ordered in a different way
        //forward mining |0|1|3|6|      backwards mininig |9|8|7|6|
        //                 |2|4|7|                          |5|4|3|
        //                   |5|8|                            |2|1|
        //                     |9|                              |0|
        // the trick is with the priniting, for which we have int[][]    logiRelsIndxs;
        int[][]    logiRelsIndxs; public int[][] getBackwardsRelsIndxs() { return logiRelsIndxs; }
        //twoSizedTIRPsMATRIXEntry[][] twoSizedTIRPsMatrix;
        //twoSizedTIRPsMATRIXEntry[,] twoSizedTIRPsMatrixWithPsik;
        public tindex karma;

        /*public int getMatrixVerticalSupport(int frstTncptIndx, int scndTncptIndx, int rel) 
        { 
            mxAccsCounter++; 
            if(MatrixFirstIndexNotNull(frstTncptIndx))
                if(MatrixEntryNotNull(frstTncptIndx, scndTncptIndx))
                    if (twoSizedTIRPsMatrix[frstTncptIndx][scndTncptIndx].prsMxRelVec[rel] != null) 
                        return twoSizedTIRPsMatrix[frstTncptIndx][scndTncptIndx].prsMxRelVec[rel].verticalSupport.Count; //else return 0;
            return 0;
        }*/
        
        /*public bool MatrixRelContainsKey(int frstIdx, int scndIdx, int rel, string containsKey) 
        { 
            mxAccsCounter++; 
            return twoSizedTIRPsMatrix[frstIdx][scndIdx].prsMxRelVec[rel].instancesDicList.ContainsKey(containsKey); 
        }*/
        
        /*public List<TimeIntervalSymbol> MatrixRelGetKey(int frstIdx, int scndIdx, int rel, string key) 
        { 
            return twoSizedTIRPsMatrix[frstIdx][scndIdx].prsMxRelVec[rel].instancesDicList[key]; 
        }*/
        
        /*public bool MatrixFirstIndexNotNull(int tonceptIndex) 
        {
            if (twoSizedTIRPsMatrix[tonceptIndex] == null)
            {
                mxAccsCounter++; // = mxAccsCounter + (toncepts.Count * relations_style);
                return false;
            }
            mxAccsCounter++;
            return true;
        }*/
        
        /*public bool MatrixEntryNotNull(int firstTonceptIndex, int secondTonceptIndex) 
        {
            if (twoSizedTIRPsMatrix[firstTonceptIndex][secondTonceptIndex] == null)
            {
                mxAccsCounter++; // = mxAccsCounter + relations_style;
                return false;
            }
            mxAccsCounter++;
            return true;
        }*/

        /*private bool MatrixEntryRelNotNull(int firstTonceptIndex, int secondTonceptIndex, int rel)
        {
          //  mxAccsCounter++;
            return twoSizedTIRPsMatrix[firstTonceptIndex][secondTonceptIndex].prsMxRelVec[rel] != null;
        }*/

        /*public Dictionary<string, List<TimeIntervalSymbol>> getMatrixEntryDic(int frstTncptIdx, int scndTncptIdx, int rel)
        {
            if (MatrixFirstIndexNotNull(frstTncptIdx) && MatrixEntryNotNull(frstTncptIdx, scndTncptIdx) && MatrixEntryRelNotNull(frstTncptIdx, scndTncptIdx, rel) )
                return twoSizedTIRPsMatrix[frstTncptIdx][scndTncptIdx].prsMxRelVec[rel].instancesDicList;
            else
                return null;            
        }*/

        public TIRP getMatrixDICasTIRP(int tTrgtID, int tErlyID, int rel)
        {
            TIRP twoSzdTIRP = new TIRP(tTrgtID, tErlyID, rel);
            //string trK = toncepts[tTrgtID].tonceptINDEX.ToString() + "-" + rel + "-" + toncepts[tErlyID].tonceptINDEX;
            //Dictionary<string, List<TimeIntervalSymbol>> tiListDic = twoSizedTIRPsMatrix[toncepts[tTrgtID].tonceptINDEX][toncepts[tErlyID].tonceptINDEX].prsMxRelVec[rel].instancesDicList; //entitiesKarmaVec[eIdx].instancesDic[trK];
            Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] tiListDicsVec = karma.Kindex[toncepts[tTrgtID].tonceptINDEX, toncepts[tErlyID].tonceptINDEX].getRelEntitiesDics(rel);
            for(int eIdx = 0; eIdx < tiListDicsVec.Count(); eIdx++)
            {
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTiDic = tiListDicsVec[eIdx];     
                foreach (KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListTiDic)
                    foreach (TimeIntervalSymbol tis in tiListTi.Value)
                    {
                        TIsInstance tisInsNew = new TIsInstance(new TimeIntervalSymbol(tiListTi.Key.startTime, tiListTi.Key.endTime, tiListTi.Key.symbol), tis, eIdx);
                        twoSzdTIRP.addEntity(eIdx); // int.Parse(tiListTi.Key.Split('-')[0]));
                        twoSzdTIRP.tinstancesList.Add(tisInsNew);
                        twoSzdTIRP.meanHorizontalSupport++;
                    }
            }
            return twoSzdTIRP;
        }
                
        public Karma(bool runKarma, int setForBackWards, int setRelationStyle, int setEpsilon, int setMaxGap, int setMinVerSup, string dsFilePath, bool seTrans, bool setHS1, int setPrint, ref string runTime, int entitieSizeLimit = KLC.NUM_OF_ENTITIES)
        {
            DateTime starTime = DateTime.Now;
            string timings = "";
            mxAccsCounter = 0;
            forBackWards = setForBackWards;
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            HS1 = setHS1;
            KarmE.read_tids_file(0, entitieSizeLimit, dsFilePath, ref entityTISs, ref toncepts);
            
            entitieSize = entityTISs.Count;
            min_ver_sup = ((double)setMinVerSup / 100) * entitieSize;
            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = entityTISs.Keys.ElementAt(eIdx);

            //twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][]; //initialize keys vector
            //for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
            //    twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS]; // initialize val vector
            
            //twoSizedTIRPsMatrixWithPsik = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS, KLC.NUM_OF_SYMBOLS];

            karma = new tindex(entitieSize, relations_style);

            print = setPrint;
            trans = seTrans;
            if (trans)
            {
                string tesTrans;
                if (relations_style == KLC.RELSTYLE_ALLEN7)
                    tesTrans = KLC.LoadTransitionTableALLEN7(ref transition);//load transition table
                else
                    tesTrans = KLC.LoadTransitionTableKL3(ref transition);
                if (tesTrans.Length > 0)
                    transition = null;
            }

            if (forBackWards == KLC.backwardsMining)
            {
                logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
                logiRelsIndxs[0/*3*/] = new int[1]  { 0 }; 
                logiRelsIndxs[1/*3*/] = new int[3]  { 2, 1, 0 };
                logiRelsIndxs[2/*4*/] = new int[6]  { 5, 4, 2, 3, 1, 0 };
                logiRelsIndxs[3/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
                logiRelsIndxs[4/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
                logiRelsIndxs[5/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[6/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[7/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[8/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
            }

            if( runKarma == true )
                RunKarmaRun(null, 0, entitieSizeLimit);
           
            DateTime endTime = DateTime.Now;
            runTime = endTime.Subtract(starTime).TotalMilliseconds.ToString(); // ((endTime - starTime).Minutes * 60 + (endTime - starTime).Seconds) + "," + (endTime - starTime).Minutes + ":" + (endTime - starTime).Seconds;
        }

        // for Single KarmaLego
        public Karma(int frstEntIdx, int lstEntIdx, int setForBackWards, int setRelationStyle, int setEpsilon, int setMaxGap, string dsFilePath, bool runKarma, List<int> tonceptIds)
        {
            mxAccsCounter = 0;
            forBackWards = setForBackWards;
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            HS1 = false;
            KarmE.read_tids_file(frstEntIdx, lstEntIdx, dsFilePath, ref entityTISs, ref toncepts);

            entitieSize = entityTISs.Count;
            min_ver_sup = 0; // ((double)setMinVerSup / 100) * entitieSize;
            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = entityTISs.Keys.ElementAt(eIdx);

            //twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][]; //initialize keys vector
            //for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
            //    twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS]; // initialize val vector
            karma = new tindex(entitieSize, relations_style);

            //print = setPrint;
            //trans = seTrans;
            if (trans)
            {
                string tesTrans;
                if (relations_style == KLC.RELSTYLE_ALLEN7)
                    tesTrans = KLC.LoadTransitionTableALLEN7(ref transition);//load transition table
                else
                    tesTrans = KLC.LoadTransitionTableKL3(ref transition);
                if (tesTrans.Length > 0)
                    transition = null;
            }

            if (forBackWards == KLC.backwardsMining)
            {
                logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
                logiRelsIndxs[0/*3*/] = new int[1] { 0 };
                logiRelsIndxs[1/*3*/] = new int[3] { 2, 1, 0 };
                logiRelsIndxs[2/*4*/] = new int[6] { 5, 4, 2, 3, 1, 0 };
                logiRelsIndxs[3/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
                logiRelsIndxs[4/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
                logiRelsIndxs[5/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[6/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[7/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[8/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
            }

            if (runKarma == true)
                RunKarmaRun(tonceptIds, frstEntIdx, lstEntIdx);

        }

        /*public void allocateTindex()
        {
            twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][]; //initialize keys vector
            for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
                twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS]; // initialize val vector
            //twoSizedTIRPsMatrixWithPsik = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS, KLC.NUM_OF_SYMBOLS];

        }*/

        public void RunKarmaRun(List<int> tonceptIds, int from_eIdx, int to_eIdx)
        {
            int index = 0;
            //try
            //{
                int[] relStat = new int[7];
                for (int eIdx = from_eIdx; eIdx < to_eIdx &&  eIdx < entityTISs.Count() && eIdx < entitieSize; eIdx++)
                {
                    if (eIdx == 7)
                        eIdx = eIdx;
                    indexEntitySTIs(eIdx, ref index, ref relStat, tonceptIds);
                    /*List<TimeIntervalSymbol> tisList = entityTISs[entitiesVec[eIdx]];
                    
                    for (int ti1Idx = 0; ti1Idx < tisList.Count; ti1Idx++)
                    {
                        TimeIntervalSymbol firsTis = tisList.ElementAt(ti1Idx);
                        addToncept(eIdx, firsTis, ref index); // update toncept entity support
                        //karma..
                        for (int ti2Idx = ti1Idx + 1; ti2Idx < tisList.Count; ti2Idx++)
                        {
                            TimeIntervalSymbol secondTis = tisList.ElementAt(ti2Idx);
                            if (firsTis.symbol != secondTis.symbol)
                            {
                                addToncept(eIdx, secondTis, ref index); // DONT update toncept entity support
                                int relation = KLC.WhichRelationEpsilon(firsTis, secondTis, relations_style, epsilon, max_gap);
                                if (relation > -1)
                                {
                                    relStat[relation]++;
                                    indexTISPair(firsTis, secondTis, relation, eIdx);
                                }
                                else
                                    break;
                            }
                        }
                    } */  
                }   //);
            //    pruneToncepts();      //pruneMatrixEntriesPairDics();
            //}
            /*catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }*/
        }

        public void indexEntitySTIs(int eIdx, ref int index, ref int[] relStat, List<int> tonceptIds)
        {
            List<TimeIntervalSymbol> tisList = entityTISs[entitiesVec[eIdx]];

            for (int ti1Idx = 0; ti1Idx < tisList.Count; ti1Idx++)
            {
                TimeIntervalSymbol firsTis = tisList.ElementAt(ti1Idx);
                if (tonceptIds == null || tonceptIds.Contains(firsTis.symbol))
                {
                    addToncept(eIdx, firsTis, ref index); // update toncept entity support
                    //karma..
                    for (int ti2Idx = ti1Idx + 1; ti2Idx < tisList.Count; ti2Idx++)
                    {
                        TimeIntervalSymbol secondTis = tisList.ElementAt(ti2Idx);
                        if (tonceptIds == null || tonceptIds.Contains(secondTis.symbol))
                        {
                            if (firsTis.symbol != secondTis.symbol)
                            {
                                addToncept(eIdx, secondTis, ref index); // DONT update toncept entity support
                                int relation = KLC.WhichRelationEpsilon(firsTis, secondTis, relations_style, epsilon, max_gap);
                                if (relation > -1)
                                {
                                    relStat[relation]++;
                                    indexTISPair(firsTis, secondTis, relation, eIdx);
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void indexTISPair(TimeIntervalSymbol firsTis, TimeIntervalSymbol secondTis, int relation, int eIdx)
        {
            if (forBackWards == KLC.forwardMining)
                indexTimeInetervalSymbolsPair(firsTis, secondTis, relation, eIdx);
            else
                indexTimeInetervalSymbolsPair(secondTis, firsTis, relation, eIdx);
        }

        private void addToncept(int entityIdx, TimeIntervalSymbol tis, ref int index)
        {
            TemporalConcept tc;
            if (!toncepts.ContainsKey(tis.symbol))
            {
                tc = new TemporalConcept(tis.symbol, toncepts.Count());// index);
                index++;
                toncepts.Add(tis.symbol, tc);
            }
            else
                tc = toncepts[tis.symbol];
            tc.addEntityTinstance(entityIdx, tis); // tc.addEntity(entityIdx);
        }

        private void indexTimeInetervalSymbolsPair(TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal, int relation, int entityIdx)
        {
            int tKeyIdx = toncepts[tisKey.symbol].tonceptINDEX;
            int tValIdx = toncepts[tisVal.symbol].tonceptINDEX;
            if (tisKey.symbol == 1125315 && tisVal.symbol == 941258 && tisKey.startTime == 2060 && tisVal.startTime == 2060)
                tKeyIdx = tKeyIdx;
            if (tisKey.symbol == 1307046 && tisVal.symbol == 941258 && tisKey.startTime == 2060 && tisVal.startTime == 2060)
                tKeyIdx = tKeyIdx;
            karma.indexTTsRelEidxSTIs(tKeyIdx, tValIdx, relation, entityIdx, tisKey, tisVal);
            /*if (twoSizedTIRPsMatrix[tKeyIdx][tValIdx] == null)
                twoSizedTIRPsMatrix[tKeyIdx][tValIdx] = new twoSizedTIRPsMATRIXEntry(relations_style);

            twoSizedTIRPsMATRIXEntry twoSizedTIRP_P = twoSizedTIRPsMatrix[tKeyIdx][tValIdx];
            pairsMATRIXrelEntry pMxRelE = twoSizedTIRP_P.prsMxRelVec[relation];
            if (pMxRelE == null)
            {
                twoSizedTIRPsMatrix[tKeyIdx][tValIdx].prsMxRelVec[relation] = new pairsMATRIXrelEntry();
                pMxRelE = twoSizedTIRPsMatrix[tKeyIdx][tValIdx].prsMxRelVec[relation];
            }
            string tiEntID = entityIdx + "-" + tisKey.symbol + "-" + tisKey.startTime + "-" + tisKey.endTime;
            if (pMxRelE.instancesDicList.ContainsKey(tiEntID))
                pMxRelE.instancesDicList[tiEntID].Add(tisVal);
            else
            {
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                pMxRelE.instancesDicList.Add(tiEntID, tisList);
            }
            pMxRelE.addEntity(entityIdx);
             */ 
            
        }

        /*private void pruneToncepts()
        {
            for (int t = 0; t < toncepts.Count; t++)
            {
                TemporalConcept tc = toncepts.Values.ElementAt(t);
                //for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                //    if (tc.entitiesSupport[eIdx] == true)
                //        tc.verticalSupport++;
                //tc.entitiesSupport = null;
                if (tc.getTonceptVerticalSupport() < min_ver_sup)
                {
                    twoSizedTIRPsMatrix[tc.tonceptINDEX] = null;
                    for (int idx = 0; idx < toncepts.Count; idx++)
                        if (tc.tonceptINDEX != idx && twoSizedTIRPsMatrix[idx] != null)
                            twoSizedTIRPsMatrix[idx][tc.tonceptINDEX] = null;
                    toncepts.Remove(tc.tonceptID);
                    t--;
                }
            }
        }*/

        /*public void printMatrixEntries(string filePath)
        {
            TextWriter tw = new StreamWriter(filePath);
            foreach(TemporalConcept toncept1 in toncepts.Values)
            {
                int t1Idx = toncept1.tonceptINDEX;
                if (twoSizedTIRPsMatrix[t1Idx] != null)
                {
                    foreach (TemporalConcept toncept2 in toncepts.Values)
                    {
                        int t2Idx = toncept2.tonceptINDEX;
                        twoSizedTIRPsMATRIXEntry tsTME = twoSizedTIRPsMatrix[t1Idx][t2Idx];
                        if (tsTME != null)
                        {
                            for (int r = 0; r < relations_style; r++)
                            {
                                pairsMATRIXrelEntry pMxRelE = tsTME.prsMxRelVec[r];
                                if (pMxRelE != null)
                                {
                                    if (pMxRelE.verticalSupport.Count > min_ver_sup)
                                    {
                                        pMxRelE.horizontalSupport = 0;
                                        for (int i = 0; i < pMxRelE.instancesDicList.Count; i++)
                                            if (pMxRelE.instancesDicList.ElementAt(i).Value != null)
                                                pMxRelE.horizontalSupport = pMxRelE.horizontalSupport + pMxRelE.instancesDicList.ElementAt(i).Value.Count;
                                        tw.WriteLine(toncept1.tonceptID + "," + toncept2.tonceptID + "," + r + "," + pMxRelE.verticalSupport.Count + "," + pMxRelE.horizontalSupport);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            tw.Close();
        }*/

        /*private void pruneMatrixEntriesPairDics()
        {
            for (int t1 = 0; t1 < toncepts.Count; t1++) //prune unfrequent pairs - check this?
            {
                int t1Idx = toncepts.Values.ElementAt(t1).tonceptINDEX;
                int nullMxEntryCount = 0;
                for (int t2 = 0; t2 < toncepts.Count; t2++)
                {
                    int t2Idx = toncepts.Values.ElementAt(t2).tonceptINDEX;
                    twoSizedTIRPsMATRIXEntry tsTME = twoSizedTIRPsMatrix[t1Idx][t2Idx];
                    if (tsTME != null)
                    {
                        int nullCount = 0;
                        for (int r = 0; r < relations_style; r++)
                        {
                            pairsMATRIXrelEntry pMxRelE = tsTME.prsMxRelVec[r];
                            if (pMxRelE != null)
                            {
                                if (pMxRelE.verticalSupport.Count < min_ver_sup)
                                {
                                    tsTME.prsMxRelVec[r] = null;
                                    nullCount++;
                                }
                            }
                            else
                                nullCount++;
                        }
                        if (nullCount == relations_style) // remove to check output without this removal
                        {
                            tsTME = null;
                            twoSizedTIRPsMatrix[t1Idx][t2Idx] = null;
                            nullMxEntryCount++;
                        }
                    }
                    else
                        nullMxEntryCount++;
                }
                if (nullMxEntryCount == toncepts.Count)
                    twoSizedTIRPsMatrix[t1Idx] = null;
            }
        }*/
    }
}
