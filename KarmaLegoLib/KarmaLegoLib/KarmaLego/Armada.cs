using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class Armada
    {
        bool HS1;
        int relations_style; public int getRelStyle() { return relations_style; } //number of reations (Allen7, KL3, KL2)
        int epsilon; public int getEpsilon() { return epsilon; }
        double min_ver_sup; public double getMinVerSup() { return min_ver_sup; } //the exact value of the treshold in number of entities
        int max_gap; public int getMaxGap() { return max_gap; }

        int entitieSize;
        public int getEntitieSize() { return entitieSize; }

        int[] entitiesVec;
        public int[] getEntitiesVec() { return entitiesVec; }
        public int getEntityByIdx(int idx) { return entitiesVec[idx]; }

        Dictionary<int, List<TimeIntervalSymbol>> entityTISs;
        Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();
        public Dictionary<int, TemporalConcept> getToncepts() { return toncepts; }
        public TemporalConcept getTonceptByID(int t_id) { return toncepts[t_id]; }

        public static void runRMDA(int legologi, int rels, int epsilon, int maxGap, int minSup, string klFile, bool seTrans, string mXprntD, string outFolder, string outFile, ref string karmaDTime, ref string legoDTime, bool setHS1 = false)
        {
            DateTime starTime = DateTime.Now;
            Armada rmDa = new Armada(legologi, KLC.RELSTYLE_ALLEN7, epsilon, maxGap, minSup, klFile, seTrans, setHS1);
            karmaDTime = (DateTime.Now - starTime).Minutes + ":" + (DateTime.Now - starTime).Seconds; 
            rmDa.RunArmadaRun(outFolder, outFile);
            rmDa = null;
        }

        public Armada(int setKarmaLegoLogi, int setRelationStyle, int setEpsilon, int setMaxGap, int setMinVerSup, string dsFilePath, bool seTrans, bool setHS1)
        {
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            HS1 = setHS1;
            KarmE.read_tids_file(0, KLC.NUM_OF_ENTITIES, dsFilePath, ref entityTISs, ref toncepts);
            entitieSize = entityTISs.Count;
            min_ver_sup = ((double)setMinVerSup / 100) * entitieSize;

            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = entityTISs.Keys.ElementAt(eIdx);

        }

        private void RunArmadaRun(string outFolder, string outFile)
        {
            //scan all the data to discover symbols frequnecy
            for (int eIdx = 0; eIdx < entityTISs.Count; eIdx++)
                for (int tiIdx = 0; tiIdx < entityTISs.ElementAt(eIdx).Value.Count; tiIdx++)
                    KarmE.addToncept(eIdx, entityTISs.ElementAt(eIdx).Value.ElementAt(tiIdx), ref toncepts);
            
            string tempFolderPath = outFolder + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "\\";
            Directory.CreateDirectory(tempFolderPath);

            for (int tcIdx = 0; tcIdx < toncepts.Count; tcIdx++)
                if (toncepts.ElementAt(tcIdx).Value.getTonceptVerticalSupport() <= min_ver_sup)
                {
                    toncepts.Remove(toncepts.ElementAt(tcIdx).Key);
                    tcIdx--;
                }

            foreach (TemporalConcept tc1 in toncepts.Values)
                if (tc1.getTonceptVerticalSupport() > min_ver_sup)
                    runRmada(tc1, tempFolderPath);
            KarmE.WriteKLFileFromTonceptsFiles(outFile, tempFolderPath); // outFolder);
            
        }

        private void runRmada(TemporalConcept tc1, string folderPath)
        {
            int freqNum = 0;
            string filePath = folderPath + tc1.tonceptID + ".txt";
            List<string> enTIdxList = new List<string>();
            //bool[] tonceptEntitiesFirstInstance = new bool[entitieSize];

            TextWriter tw = new StreamWriter(filePath);
            tw.WriteLine("1 " + tc1.tonceptID + "- " + tc1.getTonceptVerticalSupport() + " " + tc1.getTonceptVerticalSupport());

            // search all the symbol instances
            for (int eIdx = 0; eIdx < entityTISs.Count; eIdx++)
                for (int i = 0; i < entityTISs.ElementAt(eIdx).Value.Count; i++)
                    if (entityTISs.ElementAt(eIdx).Value.ElementAt(i).symbol == tc1.tonceptID)
                    {
                        enTIdxList.Add(eIdx + "," + i);
                        //if( HS1 == true )
                        //    break;
                    }

            foreach (TemporalConcept tc2 in toncepts.Values)
            {
                if (tc2.tonceptID != tc1.tonceptID)
                {
                    Dictionary<string, TIRP> cnddTirpsDic = new Dictionary<string, TIRP>();
                    foreach (string enTIdx in enTIdxList)
                    {
                        int entIdx = int.Parse(enTIdx.Split(',')[0]);
                        int tIdx = int.Parse(enTIdx.Split(',')[1]);

                        if (tc1.tonceptID == 10 && tc2.tonceptID == 17 && entIdx == 4)
                            freqNum = freqNum;

                        for (int i = tIdx; i < entityTISs.ElementAt(entIdx).Value.Count; i++)
                        {
                            TimeIntervalSymbol tis = entityTISs.ElementAt(entIdx).Value.ElementAt(i);
                            if (entityTISs.ElementAt(entIdx).Value.ElementAt(i).symbol == tc2.tonceptID)
                            {
                                int rel = KLC.WhichRelationEpsilon(entityTISs.ElementAt(entIdx).Value.ElementAt(tIdx), entityTISs.ElementAt(entIdx).Value.ElementAt(i), relations_style, epsilon, max_gap);
                                string cnddkey = rel + "," + tc2.tonceptID;
                                if (!cnddkey.Contains('-'))
                                {
                                    TIsInstance newIns = new TIsInstance(entityTISs.ElementAt(entIdx).Value.ElementAt(tIdx), entityTISs.ElementAt(entIdx).Value.ElementAt(i), entIdx, i);
                                    if (cnddTirpsDic.ContainsKey(cnddkey))
                                    {
                                        cnddTirpsDic[cnddkey].tinstancesList.Add(newIns);
                                        cnddTirpsDic[cnddkey].addEntity(newIns.entityIdx);
                                    }
                                    else
                                    {
                                        TIRP newTIRP = new TIRP(tc1.tonceptID, tc2.tonceptID, rel);
                                        newTIRP.tinstancesList.Add(newIns);
                                        newTIRP.addEntity(newIns.entityIdx);
                                        cnddTirpsDic.Add(cnddkey, newTIRP);
                                    }
                                    if (HS1 == true)
                                        break;
                                }
                                //else
                                //    break;
                            }
                        }
                    }
                    foreach (TIRP t in cnddTirpsDic.Values)
                    {
                        if (t.entitieVerticalSupport.Count > min_ver_sup)
                        {
                            freqNum++;
                            string writeLine = "2 " + tc1.tonceptID + "-" + tc2.tonceptID + "- " + KLC.ALLEN7_RELCHARS[t.rels[0]] + ". " + t.tinstancesList.Count + " " + t.entitieVerticalSupport.Count + " ";
                            foreach (TIsInstance tIns in t.tinstancesList)
                                writeLine = writeLine + entitiesVec[tIns.entityIdx] + " [" + tIns.tis[0].startTime + "-" + tIns.tis[0].endTime + "][" + tIns.tis[1].startTime + "-" + tIns.tis[1].endTime + "] ";
                            tw.WriteLine(writeLine);
                            CreateAndMine(t, tw);
                        }
                    }
                }
            }
            tw.Close();
            if( freqNum == 0 )
                File.Delete(filePath);
        }

        private void CreateAndMine(TIRP tirp, TextWriter tw)
        {
            int tinsCount = tirp.tinstancesList.Count;
            int relSize = tirp.size;
            //Dictionary<string, TIRP> cnddTirpsDic = new Dictionary<string, TIRP>();

            //Dictionary<string, TIRP>[] cnddTirpsDicVector = new Dictionary<string, TIRP>[toncepts.Count];
            Dictionary<int, Dictionary<string, TIRP>> cnddTirpsDicDic = new Dictionary<int, Dictionary<string, TIRP>>(); //<tonceptIdx,<rels,tirp>>

   //         NOW YOU HAVE TO COORDINATE ALL ACCORDING TO THIS DATA STRUCTURE

            for (int tinsIdx = 0; tinsIdx < tinsCount; tinsIdx++)
            {
                TIsInstance tins = tirp.tinstancesList[tinsIdx];
                for (int i = tins.armadaTisIdx + 1; i < entityTISs.ElementAt(tins.entityIdx).Value.Count; i++)
                {
                    string rels = "";
                    int[] relsVec = new int[relSize];
                    int relIdx = relSize - 1;
                    if ( toncepts.ContainsKey(entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol) && tirp.toncepts[tirp.size - 1] != entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol)
                    {
                        relsVec[relIdx] = KLC.WhichRelationEpsilon(tins.tis[relIdx], entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), relations_style, epsilon, max_gap);
                        if (relsVec[relIdx] == -1)
                            break;
                        else
                        {
                            for (relIdx = 0; relIdx < relSize; relIdx++)
                            {
                                relsVec[relIdx] = KLC.WhichRelationEpsilon(tins.tis[relIdx], entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), relations_style, epsilon, max_gap);
                                rels = rels + relsVec[relIdx];
                            }
                            if (!rels.Contains('-'))
                            { 
                                TIsInstance newIns = new TIsInstance(tirp.size + 1, tins.tis, entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), tins.entityIdx, i); // D);
                                int tonceptIdx = toncepts[entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol].tonceptINDEX;
                                if (cnddTirpsDicDic.ContainsKey(tonceptIdx)) // cnddTirpsDicVector[tonceptIdx] == null)
                                { //cnddTirpsDicVector[tonceptIdx] = new Dictionary<string, TIRP>();
                                    if(cnddTirpsDicDic[tonceptIdx].ContainsKey(rels))
                                    {
                                        cnddTirpsDicDic[tonceptIdx][rels].tinstancesList.Add(newIns);
                                        cnddTirpsDicDic[tonceptIdx][rels].addEntity(newIns.entityIdx);
                                    }
                                    else
                                    {
                                        int symbol = entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol;
                                        TIRP newTIRP = new TIRP(tirp, symbol, relsVec[relSize - 1], relsVec);
                                        newTIRP.tinstancesList.Add(newIns);
                                        newTIRP.addEntity(newIns.entityIdx);
                                        cnddTirpsDicDic[tonceptIdx].Add(rels, newTIRP);
                                    }
                                }
                                else
                                {
                                    int symbol = entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol;
                                    TIRP newTIRP = new TIRP(tirp, symbol, relsVec[relSize - 1], relsVec);
                                    newTIRP.tinstancesList.Add(newIns);
                                    newTIRP.addEntity(newIns.entityIdx);
                                    Dictionary<string, TIRP> strTirpDic = new Dictionary<string,TIRP>();
                                    strTirpDic.Add(rels, newTIRP);
                                    cnddTirpsDicDic.Add(tonceptIdx,strTirpDic);
                                }
                                /*}
                                if (cnddTirpsDicVector[tonceptIdx].ContainsKey(rels))
                                {
                                    cnddTirpsDicVector[tonceptIdx][rels].tinstancesList.Add(newIns);
                                    cnddTirpsDicVector[tonceptIdx][rels].addEntity(newIns.entityIdx);
                                }
                                else
                                {
                                    int symbol = entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol;
                                    TIRP newTIRP = new TIRP(tirp, symbol, relsVec[relSize - 1], relsVec);
                                    newTIRP.tinstancesList.Add(newIns);
                                    newTIRP.addEntity(newIns.entityIdx);
                                    cnddTirpsDicVector[tonceptIdx].Add(rels, newTIRP);
                                }*/
                                if (HS1 == true)
                                    break;
                            }
                        }
                    }
                }
            }
            for (int tIdx = 0; tIdx < cnddTirpsDicDic.Count; tIdx++) // toncepts.Count; tIdx++)
                //if (cnddTirpsDicVector[tIdx] != null)
                    foreach (TIRP t in cnddTirpsDicDic.ElementAt(tIdx).Value.Values)
                    {
                        int[][] rels = null;
                        if (t.entitieVerticalSupport.Count > min_ver_sup)
                        {
                            t.printTIRP(tw, entitiesVec, 1, rels, getRelStyle());
                            CreateAndMine(t, tw);
                        }
                    }
        }

        private void CreateAndMineIndexSet(TIRP tirp, int symbol, TextWriter tw)
        {
            int tinsCount = tirp.tinstancesList.Count;
            int relSize = tirp.size;
            Dictionary<string, TIRP> cnddTirpsDic = new Dictionary<string, TIRP>();

            for (int tinsIdx = 0; tinsIdx < tinsCount; tinsIdx++)
            {
                TIsInstance tins = tirp.tinstancesList[tinsIdx];
                for (int i = tins.armadaTisIdx + 1; i < entityTISs.ElementAt(tins.entityIdx).Value.Count; i++)
                {
                    string rels = "";
                    int[] relsVec = new int[relSize];
                    int relIdx = relSize - 1;
                    relsVec[relIdx] = KLC.WhichRelationEpsilon(tins.tis[relIdx], entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), relations_style, epsilon, max_gap);
                    if (relsVec[relIdx] == -1)
                        break;
                    else
                    {
                    if (entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol == symbol)
                    {
//                      string rels = "";
//                      int[] relsVec = new int[relSize];
//                      int relIdx = relSize - 1;
//                      relsVec[relIdx] = KLC.WhichRelationEpsilon(tins.tis[relIdx], entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), relations_style, epsilon, max_gap);
//                      if (relsVec[relIdx] == -1)
//                            break;
                     // else
                     //   {
                            for (relIdx = 0; relIdx < relSize; relIdx++)
                            {
                                relsVec[relIdx] = KLC.WhichRelationEpsilon(tins.tis[relIdx], entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), relations_style, epsilon, max_gap);
                                rels = rels + relsVec[relIdx];
                            }
                            if (!rels.Contains('-'))
                            {
                                TIsInstance newIns = new TIsInstance(tirp.size + 1, tins.tis, entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), tins.entityIdx, i); // D);
                                if (cnddTirpsDic.ContainsKey(rels))
                                {
                                    cnddTirpsDic[rels].tinstancesList.Add(newIns);
                                    cnddTirpsDic[rels].addEntity(newIns.entityIdx);
                                }
                                else
                                {
                                    TIRP newTIRP = new TIRP(tirp, symbol, relsVec[relSize - 1], relsVec);

                                    newTIRP.tinstancesList.Add(newIns);
                                    newTIRP.addEntity(newIns.entityIdx);
                                    cnddTirpsDic.Add(rels, newTIRP);
                                }
                            }
                        }
                    }
                }
            }
            foreach (TIRP t in cnddTirpsDic.Values)
            {
                int[][] rels = null;
                if (t.entitieVerticalSupport.Count > min_ver_sup)
                {
                    t.printTIRP(tw, entitiesVec, 1, rels, getRelStyle());
                    foreach (TemporalConcept toncept in toncepts.Values)
                    {
                        if(tirp.toncepts[tirp.size-1] != toncept.tonceptID)
                            CreateAndMineIndexSet(t, toncept.tonceptID, tw);
                    }
                }
            }
        }

        private void CreateIndexSet(TIRP tirp, int symbol, TextWriter tw)
        {
            int tinsCount = tirp.tinstancesList.Count;
            for(int tinsIdx = 0; tinsIdx < tinsCount; tinsIdx++ )
            {
                TIsInstance tins = tirp.tinstancesList[tinsIdx];
                int matchCount = 0;
                for (int i = tins.armadaTisIdx + 1; i < entityTISs.ElementAt(tins.entityIdx).Value.Count; i++)
                {
                    if (entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i).symbol == symbol)
                    {

                        if (matchCount == 0)
                        {
                            tins.armadaTisIdx = i;
                            matchCount++;
                        }
                        else
                        {
                            TIsInstance newTins = new TIsInstance(tirp.size, tins.tis,  entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(i), tins.entityIdx, -1);
                            tirp.tinstancesList.Add(newTins);
                        }
                    }
                }
            }
        }

        private void MineIndexSet(TIRP tirp, TextWriter tw) //int symbol, TextWriter tw)
        {
            int relSize = tirp.size;
            Dictionary<string, TIRP> cnddTirpsDic = new Dictionary<string, TIRP>();

            for (int tinsIdx = 0; tinsIdx < tirp.tinstancesList.Count; tinsIdx++)
            {
                TIsInstance tins = tirp.tinstancesList[tinsIdx];

                if (tins.armadaTisIdx != -1)
                {
                    string rels = "";
                    int[] relsVec = new int[relSize];
                    for (int relIdx = 0; relIdx < relSize; relIdx++)
                    {
                        relsVec[relIdx] = KLC.WhichRelationEpsilon(tins.tis[relIdx], entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(tins.armadaTisIdx), relations_style, epsilon, max_gap);
                        rels = rels + relsVec[relIdx];
                    }
                    if (!rels.Contains('-'))
                    {
                        TIsInstance newIns = new TIsInstance(tirp.size+1, tins.tis, entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(tins.armadaTisIdx), tins.entityIdx); // D);
                        if (cnddTirpsDic.ContainsKey(rels))
                        {
                            cnddTirpsDic[rels].tinstancesList.Add(newIns);
                            cnddTirpsDic[rels].addEntity(newIns.entityIdx);
                        }
                        else
                        {
                            TIRP newTIRP = new TIRP(tirp, entityTISs.ElementAt(tins.entityIdx).Value.ElementAt(tins.armadaTisIdx).symbol, relsVec[relSize - 1], relsVec);
                            
                            newTIRP.tinstancesList.Add(newIns);
                            newTIRP.addEntity(newIns.entityIdx);
                            cnddTirpsDic.Add(rels, newTIRP);
                        }
                    }
                }
            }
            foreach (TIRP t in cnddTirpsDic.Values)
            {
                int[][] rels = null;
                if (t.entitieVerticalSupport.Count > min_ver_sup)
                {
                    t.printTIRP(tw, entitiesVec, 1, rels, getRelStyle() );
                    foreach(TemporalConcept toncept in toncepts.Values)
                    {
                        CreateIndexSet(t, toncept.tonceptID, tw);
                        MineIndexSet(t, tw);
                    }
                }
            }
        }
    }

}
