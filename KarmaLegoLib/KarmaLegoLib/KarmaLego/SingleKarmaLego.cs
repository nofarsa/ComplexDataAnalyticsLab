using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class SingleKarmaLego
    {
        Karma karma;
        
        public SingleKarmaLego(Karma setKarma)
        {
            karma = setKarma;
        }

        public static List<TIRP> readTIRPsFile(string tirpsFilePath, int relationStyle, bool stripOutcome = true, int outcomeToncept = -1, bool readInstances = false)
        {
            // if (stripOutcome == true)
            //     readInstances = false;
            Dictionary<string, int> relsDic = new Dictionary<string, int>();
            if (relationStyle == KLC.RELSTYLE_ALLEN7)
            {
                relsDic.Add("<", KLC.ALLEN_BEFORE /*0*/); relsDic.Add("m", KLC.ALLEN_MEET /*1*/); relsDic.Add("o", KLC.ALLEN_OVERLAP /*2*/); 
                relsDic.Add("f", KLC.ALLEN_FINISHBY /*3*/); relsDic.Add("c", KLC.ALLEN_CONTAIN /*4*/); relsDic.Add("=", KLC.ALLEN_EQUAL /*5*/); 
                relsDic.Add("s", KLC.ALLEN_STARTS /*6*/); //relsDic.Add("-", 7);
            }
            else
            {
                relsDic.Add("<", KLC.KL_BEFORE /*0*/); relsDic.Add("O", KLC.KL_OVERLAP /*1*/); relsDic.Add("C", KLC.KL_CONTAIN /*2*/);
            }
            List<TIRP> tirpsList = new List<TIRP>();
            TextReader tr = new StreamReader(tirpsFilePath);
            string readLine = "";
            while (tr.Peek() >= 0) //read the toncepts
            {
                readLine = tr.ReadLine();
                string[] lineVec = readLine.Split(' ');
                int tirpSize = int.Parse(lineVec[0]);
                {
                    TIRP tirp = new TIRP(tirpSize);
                    string[] toncepts = lineVec[1].Split('-');
                    for (int t = 0; t < tirpSize; t++)
                        tirp.toncepts[t] = int.Parse(toncepts[t]);
                    string[] rels = lineVec[2].Split('.');
                    for (int r = 0; r < tirp.relSize; r++)
                        tirp.rels[r] = relsDic[rels[r]];
                    if (stripOutcome == true && tirp.toncepts[tirp.size - 1] == outcomeToncept)
                    {
                        tirp.size = tirp.size - 1;
                        int[] new_toncepts = new int[tirp.size];
                        for (int t = 0; t < tirp.size; t++)
                            new_toncepts[t] = tirp.toncepts[t];
                        tirp.toncepts = new_toncepts;
                        tirp.relSize = tirp.relSize - tirp.size;
                        int[] new_rels = new int[tirp.relSize];
                        for (int r = 0; r < tirp.relSize; r++)
                            new_rels[r] = tirp.rels[r];
                        tirp.rels = new_rels;
                    }
                    if (readInstances == true)
                    {
                        int horSup = int.Parse(lineVec[3]);
                        int verSup = int.Parse(lineVec[4]);
                        for (int i = 5; i < lineVec.Length - 1; i = i + 2)
                        {
                            int entID = int.Parse(lineVec[i]);
                            TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[tirpSize];
                            string[] tis = lineVec[i + 1].Split(']');
                            for (int t = 0; t < tis.Length - 1; t++)
                            {
                                string[] times = tis[t].Split('-');
                                int strTime = int.Parse(times[0].Replace("[", ""));
                                int endTime = int.Parse(times[1]);
                                tisVec[t] = new TimeIntervalSymbol(strTime, endTime, tirp.toncepts[t]);
                            }
                            TIsInstance tIns = new TIsInstance(tisVec, entID);
                            if (!tirp.entInstancesDic.ContainsKey(entID))
                            {
                                List<TIsInstance> tistanceList = new List<TIsInstance>();
                                tistanceList.Add(tIns);
                                tirp.entInstancesDic.Add(entID, tistanceList);
                            }
                            else
                                tirp.entInstancesDic[entID].Add(tIns);
                        }
                    }
                    if (tirp.size > 0 && tirp.toncepts[tirp.size-1] != outcomeToncept)
                        tirpsList.Add(tirp);
                }
            }
            tr.Close();

            return tirpsList;
        }

        /*
        public static double after_mineClassEntitiesByTIRPs(int tirpSize, int max_entSize, int entitieSize, int singleMethod, string entitiesFilePath, List<TIRP> tirpsList, int classLabel, int setRelStyle, int setEpsilon, int setMaxGap, List<int> tonceptIds)//, int setMinVerSup, bool seTrans, bool setHS1, int setPrint, bool setParallel)
        {
            int eIdx_maxSize = 0;
            double time = 0;
            DateTime startTime = DateTime.Now, endTime = DateTime.Now;
            string classEntitiesFeatures = "";
            Karma kma = null; SingleKarmaLego skl = null;
        //    KarmE karmE = null; 
            Single11EVEN sE11 = null;
            if (singleMethod == 0) // Sequnetial
            {
                time = 0;
                startTime = DateTime.Now;
                KarmE karmE = new KarmE(KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, false, );
                sE11 = new Single11EVEN(karmE);
                time += (DateTime.Now - startTime).TotalSeconds;
                while (eIdx_maxSize < max_entSize)
                {
                    startTime = DateTime.Now;
                    for (int e = eIdx_maxSize; e < eIdx_maxSize + entitieSize; e++)
                        classEntitiesFeatures += karmE.getEntityKarmaByIdx(e).entityID + "," + sE11.mineLinearSingleEntity(e, tirpsList, tirpSize) + classLabel + "\n";
                    eIdx_maxSize = eIdx_maxSize + entitieSize;
                    time += (DateTime.Now - startTime).TotalSeconds;
                }
            }
            else if( singleMethod == 1) // SKL
            {
                time = 0;
                startTime = DateTime.Now;
                kma = new Karma(0, max_entSize, KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, false, null);
                skl = new SingleKarmaLego(kma);
                time += (DateTime.Now - startTime).TotalSeconds;
                while (eIdx_maxSize < max_entSize)
                {
                    startTime = DateTime.Now;
                    for (int e = eIdx_maxSize; e < eIdx_maxSize + entitieSize; e++)
                    {
                        kma.RunKarmaRun(null, e, e + 1);
                        classEntitiesFeatures += kma.getEntityByIdx(e) + "," + skl.mineSKLEntity(e, tirpsList, tirpSize) + classLabel + "\n";
                    }
                    eIdx_maxSize = eIdx_maxSize + entitieSize;
                    time += (DateTime.Now - startTime).TotalSeconds;
                }
            }
            else if( singleMethod == 11) // S11, or SKL++
            {
                time = 0;
                startTime = DateTime.Now;
                kma = new Karma(0, max_entSize, KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, false, tonceptIds);
                skl = new SingleKarmaLego(kma);
                time += (DateTime.Now - startTime).TotalSeconds;
                while (eIdx_maxSize < max_entSize)
                {
                    startTime = DateTime.Now;
                    kma.RunKarmaRun(tonceptIds, eIdx_maxSize, eIdx_maxSize + entitieSize); 
                    for (int e = eIdx_maxSize; e < eIdx_maxSize + entitieSize; e++)
                        classEntitiesFeatures += kma.getEntityByIdx(e) + "," + skl.mineSKLEntity(e, tirpsList, tirpSize) + classLabel + "\n";
                    eIdx_maxSize = eIdx_maxSize + entitieSize;
                    time += (DateTime.Now - startTime).TotalSeconds;
                }
            }
            double divideBy = (max_entSize / entitieSize);
            time = time / divideBy;
            return time; // return classEntitiesFeatures;
        }*/

        public static double detectEntitiesByTIRPs(ref string entitiesValues, int tirpSizeLimit, int eStrt, int eLast, int singleMethod /*0-Seq;1-SKL;11-S11(SKL++);*/, string entitiesFilePath, List<TIRP> tirpsList, /*int BinHSmeanD,*/ int classLabel, int setRelStyle, int setEpsilon, int setMaxGap, List<int> tonceptIds)//, int setMinVerSup, bool seTrans, bool setHS1, int setPrint, bool setParallel)
        {
            double time = 0;
            DateTime startTime = DateTime.Now;//, endTime = DateTime.Now;
            string classEntitiesFeatures = "";
            Karma kma = null; SingleKarmaLego skl = null;
            //    KarmE karmE = null; 
            Single11EVEN sE11 = null;
            if (singleMethod == 0) // Sequnetial
            {
                KarmE karmE = new KarmE(KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, false, 100, eStrt, eLast);
                sE11 = new Single11EVEN(karmE);
                //for (int e = eStrt; e < eLast && e < karmE.getEntitieSize(); e++)
                for (int e = 0; e < karmE.getEntitieSize(); e++)
                    classEntitiesFeatures += karmE.getEntityKarmaByIdx(e).entityID + "," + sE11.mineLinearSingleEntity(e, tirpsList, tirpSizeLimit) + classLabel + "\n";
            }
            else if (singleMethod == 1) // SKL
            {
                kma = new Karma(eStrt, eLast, KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, true, tonceptIds); //kma = new Karma(0, entitieSize, KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, true, tonceptIds); // REMEMBER TO TAKE IT OFF null);
                skl = new SingleKarmaLego(kma);
                //for (int e = eStrt; e < eLast && e < kma.getEntitieSize(); e++) 
                for (int e = 0; e < kma.getEntitieSize(); e++)
                {
                    //kma = new Karma(e, e + 1, KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, true, null);
                    //skl = new SingleKarmaLego(kma);
                    classEntitiesFeatures += kma.getEntityByIdx(/*0*/e) + "," + skl.mineSKLEntity(/*0*/e, tirpsList)/*, tirpSize)*/ + classLabel + "\n";
                }
            }
            else if (singleMethod == 11) // S11
            {
                KarmE kmE = new KarmE(KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, false, 1000, eStrt, eLast);
                sE11 = new Single11EVEN(kmE);
                kmE.RunKarmaRun(); 
                for (int e = 0; e < kmE.getEntitieSize(); e++)
                {
                    classEntitiesFeatures += kmE.getEntityKarmaByIdx(e).entityID + "," + sE11.mineS11Entity(e, tirpsList, tirpSizeLimit) + classLabel + "\n";
                }
            }
            //return classEntitiesFeatures;
            entitiesValues = classEntitiesFeatures;
            time = (DateTime.Now - startTime).TotalSeconds;
            return time;
        }


        
        private string mineSKLEntity(int eIdx, List<TIRP> tirpsList)//, int tirpSize) //, int BinHSmeanD)
        {
            int tirpIdx = 0;
            int HS = 0;
            double timeDuration = 0;
            int[] relStat = new int[7];

            int tonceptIndex = 0;

            //karma.allocateTindex();
            //karma.indexEntitySTIs(eIdx, ref tonceptIndex, ref relStat);

            //entityKarma ek = karmE.getEntityKarmaByIdx(eIdx);
            TIRP tirp = null, prevTirp = null;
            //Dictionary<string, List<TimeIntervalSymbol>> tiListDic;
            Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic;
            string entityFeaturesLine = "";
            while (tirpIdx < tirpsList.Count) // && tirpIdx < tirpSize)
            {
                HS = 0; timeDuration = 0;
                tirp = tirpsList.ElementAt(tirpIdx);
                if(tirpIdx > 0)
                    prevTirp = tirpsList.ElementAt(tirpIdx-1);
                if (tirp.size == 1)
                {
                    if (tirp.toncepts[0] == 16)
                        tirp.toncepts[0] = tirp.toncepts[0];
                    if(karma.getTonceptByID(tirp.toncepts[0]) != null && karma.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic().ContainsKey(eIdx))
                        HS = karma.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx].Count();  // ByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx].Count();
                    for (int i = 0; i < HS; i++)
                    {
                        TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[1];
                        tisVec[0] = karma.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx][i]; //.ElementAt(i); // [eIdx][i];

                        timeDuration += (tisVec[0].endTime - tisVec[0].startTime);

                        TIsInstance tIns = new TIsInstance(tisVec, eIdx); // ek.entityID);
                        addTInstanceToTIRP(ref tirp, tIns, eIdx); // ek);

                    }
                    if(HS > 0)
                        timeDuration = (timeDuration / HS);
                    entityFeaturesLine += HS + "," + timeDuration + ",";
                    //entityFeaturesLine += ",";
                }
                else if (tirp.size == 2)
                {
                    //if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.containsTonceptID(tirp.toncepts[1]))
                    //if(karma.MatrixEntryNotNull(karma.getTonceptIndexByID(tirp.toncepts[0]), karma.getTonceptIndexByID(tirp.toncepts[1])))
                    //{
                        //int frsTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[0]), scndTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[1]);
                        //int frsTncptIdx = karma.getTonceptIndexByID(tirp.toncepts[0]), scndTncptIdx = karma.getTonceptIndexByID(tirp.toncepts[1]);
                        //if (ek.instancesDicContainsKey(scndTncptIdx, tirp.rels[0], frsTncptIdx))
                        //if(karma.MatrixFirstIndexNotNull(karma.getTonceptIndexByID(tirp.toncepts[0])) && karma.MatrixEntryNotNull(karma.getTonceptIndexByID(tirp.toncepts[0]), karma.getTonceptIndexByID(tirp.toncepts[1]))
                       
                        //tiListDic = karma.getMatrixEntryDic(karma.getTonceptIndexByID(tirp.toncepts[0]), karma.getTonceptIndexByID(tirp.toncepts[1]), tirp.rels[0]);
                        //tiListDic = karma.getMatrixEntryDic(karma.getTonceptIndexByID(tirp.toncepts[1]), karma.getTonceptIndexByID(tirp.toncepts[0]), tirp.rels[0]);
                        //int frsTIdx = karma.getTonceptIndexByID(tirp.toncepts[1]), scnDIdx = karma.getTonceptIndexByID(tirp.toncepts[0]), rel = tirp.rels[0];
                        tiListDic = karma.karma.getTindexRelEidxDic(karma.getTonceptIndexByID(tirp.toncepts[1]), karma.getTonceptIndexByID(tirp.toncepts[0]), tirp.rels[0], eIdx);
                        if(tiListDic != null)
                        {
                            //tiListDic = ek.getInstancesDicValuebyKey(scndTncptIdx, tirp.rels[0], frsTncptIdx);
                            
                            //HS = tiListDic.Count();
                            foreach(KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListDic) //foreach (KeyValuePair<string, List<TimeIntervalSymbol>> tiListTi in tiListDic)
                            {
                                TimeIntervalSymbol tisKey = tiListTi.Key; // new TimeIntervalSymbol(int.Parse(tiListTi.Key.Split('-')[2]), int.Parse(tiListTi.Key.Split('-')[3]), int.Parse(tiListTi.Key.Split('-')[1])); 
                                foreach (TimeIntervalSymbol tis in tiListTi.Value)
                                {
                                    timeDuration += (tiListTi.Key.endTime - tis.startTime); //(int.Parse(tiListTi.Key.Split('-')[3]) - tis.startTime); // .endTime - tis.startTime);

                                    TIsInstance tIns = new TIsInstance(tisKey, tis, eIdx);
                                    ////addTInstanceToTIRP(ref tirp, tIns, tisKey.);
                                    HS++;
                                }
                            }
                            if (HS > 0) 
                                timeDuration = (timeDuration / HS);
                            entityFeaturesLine += HS + "," + timeDuration;
                        }
                        else
                            entityFeaturesLine += "0,0";
                    //}
                    //else
                    //    entityFeaturesLine += "0,0";
                    entityFeaturesLine += ",";
                }
                else
                {
                    int idx = tirpIdx;
                    while (tirp.size - prevTirp.size < 1)
                    {
                        idx--;
                        prevTirp = tirpsList.ElementAt(idx);
                    }

                    int relIdx = tirp.relSize - 1;
                    //if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.containsTonceptID(tirp.toncepts[1]))
                    //{
                        //int frsTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[0]), scndTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[1]);
                        //if (ek.instancesDicContainsKey(scndTncptIdx, tirp.rels[relIdx], frsTncptIdx))
                        //{
                        //    tiListDic = ek.getInstancesDicValuebyKey(scndTncptIdx, tirp.rels[relIdx], frsTncptIdx);
                        //tiListDic = karma.getMatrixEntryDic(karma.getTonceptIndexByID(tirp.toncepts[0]), karma.getTonceptIndexByID(tirp.toncepts[1]), tirp.rels[relIdx]);
                        tiListDic = karma.karma.getTindexRelEidxDic(karma.getTonceptIndexByID(tirp.toncepts[0]), karma.getTonceptIndexByID(tirp.toncepts[1]), tirp.rels[relIdx], eIdx);
                        if(tiListDic != null)
                        {
                            for (int tins = 0; tins < prevTirp.tinstancesList.Count; tins++)
                            {
                                //if (prevTirp.tinstancesList[tins].entityIdx == ek.entityID && tiListDic.ContainsKey(prevTirp.tinstancesList[tins].tis[prevTirp.size - 1]))
                                //string tisKey = eIdx + "-" + prevTirp.tinstancesList[tins].tis[prevTirp.size - 1].symbol + "-" + prevTirp.tinstancesList[tins].tis[prevTirp.size - 1].startTime + "-" + prevTirp.tinstancesList[tins].tis[prevTirp.size - 1].endTime;
                                TimeIntervalSymbol tisKey = prevTirp.tinstancesList[tins].tis[prevTirp.size - 1];
                                if(tiListDic.ContainsKey(tisKey))
                                {
                                    List<TimeIntervalSymbol> tisList = tiListDic[tisKey]; //prevTirp.tinstancesList[tins].tis[prevTirp.size - 1]];
                                    for (int tiIdx = 0; tiIdx < tisList.Count; tiIdx++)
                                    {
                                        int rIdx = 0, relStart = (((tirp.size - 2) * (tirp.size - 1)) / 2);
                                        for (rIdx = 0; rIdx < tirp.size - 2; rIdx++)
                                        {
                                            int rel = KLC.WhichRelationEpsilon(prevTirp.tinstancesList[tins].tis[rIdx], tisList[tiIdx], karma.getRelStyle(), karma.getEpsilon(), karma.getMaxGap());
                                            if (rel != tirp.rels[relStart + rIdx])
                                                break;
                                        }
                                        if (rIdx == tirp.size - 2)
                                        {
                                            HS++;
                                            TIsInstance tIns = new TIsInstance(tirp.size, prevTirp.tinstancesList[tins].tis, tisList[tiIdx], eIdx); // ek.entityID);
                                            ////addTInstanceToTIRP(ref tirp, tIns, ek);

                                            int minTime = tIns.tis[0].startTime, maxTime = tIns.tis[0].endTime;
                                            for (int i = 1; i < tirp.size; i++)
                                            {
                                                if (tIns.tis[i].endTime > maxTime)
                                                    maxTime = tIns.tis[i].endTime;
                                            }
                                            timeDuration = timeDuration + (maxTime - minTime);
                                        }
                                    }
                                }
                            }
                            if (HS == 0)
                                entityFeaturesLine += "0,0";
                            else
                            {
                                timeDuration = (timeDuration / HS);
                                entityFeaturesLine += HS + "," + timeDuration;
                            }
                        }
                        else
                            entityFeaturesLine += "0,0";
                    //}
                    //else
                    //    entityFeaturesLine += "0,0";
                    entityFeaturesLine += ",";
                }
                tirpIdx++;
            }
            return entityFeaturesLine;
        }

        private static void addTInstanceToTIRP(ref TIRP tirp, TIsInstance tIns, int eIdx)
        {
            tirp.tinstancesList.Add(tIns);
            if (!tirp.entInstancesDic.ContainsKey(eIdx))
            {
                List<TIsInstance> tistanceList = new List<TIsInstance>();
                tistanceList.Add(tIns);
                tirp.entInstancesDic.Add(eIdx, tistanceList); // ek.entityID, tistanceList);
            }
            else
                tirp.entInstancesDic[eIdx].Add(tIns);
        }

    }
}
