using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class Single11EVEN
    {
        KarmE karmE;
        
        public Single11EVEN(KarmE setKarmE)
        {
            karmE = setKarmE;
        }

        /*
        public static string mineClassEntitiesByTIRPs(bool SKL, string entitiesFilePath, List<TIRP> tirpsList, int classLabel, int setRelStyle, int setEpsilon, int setMaxGap)//, int setMinVerSup, bool seTrans, bool setHS1, int setPrint, bool setParallel)
        {
            string classEntitiesFeatures = "";
            string runTime = "";
            KarmE karmE = new KarmE(KLC.backwardsMining, setRelStyle, setEpsilon, setMaxGap, entitiesFilePath, SKL);
            //List<TIRP> tirpsList = readTIRPsFile(tirpsFilePath, true, false);
            Single11EVEN sE11 = new Single11EVEN(karmE);
            for (int e = 0; e < karmE.getEntitieSize(); e++)
            {
                if(SKL == true)
                    classEntitiesFeatures += karmE.getEntityKarmaByIdx(e).entityID + "," + sE11.mineS11Entity(e, tirpsList) + classLabel + "\n";
                else
                    classEntitiesFeatures += karmE.getEntityKarmaByIdx(e).entityID + "," + sE11.mineLinearSingleEntity(e, tirpsList, 1000) + classLabel + "\n";
            }
            return classEntitiesFeatures;
        }*/

        public string mineLinearSingleEntity(int eIdx, List<TIRP> tirpsList, int tirpSizeLimit)
        {
            int tirpIdx = 0;
            double timeDuration = 0;
            string entityFeaturesLine = "";
            TIRP tirp = null;
            //Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic;
            
            while (tirpIdx < tirpsList.Count && tirpIdx < tirpSizeLimit)
            {
                List<List<int>> intList = new List<List<int>>();
                timeDuration = 0;
                tirp = tirpsList.ElementAt(tirpIdx);
                if (tirp.tinstancesList.Count() > 0)
                    tirp.tinstancesList.Clear();
                int n = karmE.getEntityTISsByIdx(eIdx).Count(); // k.entityTISs[eIdx].Count();
                for (int i = 0; i < n; i++)
                {
                    if (karmE.getEntityTISsByIdx(eIdx)[i].symbol == tirp.toncepts[0]) // k.entityTISs[eIdx][i].symbol == tirp.toncepts[0])
                    {
                        if (tirp.size == 1)
                        {
                            TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[1];
                            tisVec[0] = karmE.getEntityTISsByIdx(eIdx)[i]; // k.entityTISs[eIdx][tisList[x]];
                            //TIsInstance tInstance = new TIsInstance(tisVec, eIdx);
                            tirp.tinstancesList.Add(new TIsInstance(tisVec, eIdx)); //tInstance);
                        }
                        else
                            tirp.size = tirp.size;
                        List<int> tisList = new List<int>();
                        tisList.Add(i);
                        int j = i, tIdx = 1;
                        while (j < n && tIdx < tirp.size)
                        {
                            if (karmE.getEntityTISsByIdx(eIdx)[j].symbol == tirp.toncepts[tIdx]) //  k.entityTISs[eIdx][j].symbol == tirp.toncepts[tIdx])
                            {
                                int rFrom = ((tIdx * tIdx - tIdx) / 2);
                                int rTo = (((tIdx + 1) * (tIdx + 1) - (tIdx + 1)) / 2);
                                int r = 0;
                                for (r = rFrom; r < rTo; r++)
                                {
                                    //if (KLC.WhichRelationEpsilon(k.entityTISs[eIdx][tisList[r - rFrom]], k.entityTISs[eIdx][j], k.getRelStyle(), k.getEpsilon(), k.getMaxGap()) != tirp.rels[r])
                                    int rel = KLC.WhichRelationEpsilon(karmE.getEntityTISsByIdx(eIdx)[tisList[r - rFrom]], karmE.getEntityTISsByIdx(eIdx)[j], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                                    if ( rel != tirp.rels[r])
                                        break;
                                }
                                if (r == rTo)
                                {
                                    tisList.Add(j);
                                    tIdx++;
                                    if (tIdx == tirp.size)
                                    {
                                        TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[tisList.Count()];
                                        for (int x = 0; x < tisList.Count(); x++)
                                            tisVec[x] = karmE.getEntityTISsByIdx(eIdx)[tisList[x]]; // k.entityTISs[eIdx][tisList[x]];
                                        TIsInstance tInstance = new TIsInstance(tisVec, eIdx);
                                        tirp.tinstancesList.Add(tInstance);
                                        tIdx = 1; //break;
                                        tisList.Clear();
                                        tisList.Add(i);
                                    }
                                }
                            }
                            int relation = KLC.WhichRelationEpsilon(karmE.getEntityTISsByIdx(eIdx)[i], karmE.getEntityTISsByIdx(eIdx)[j], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                            if (relation < 0)
                                break;
                            j++;
                        }
                    }
                }

                if (tirp.tinstancesList.Count() > 0)
                {
                    timeDuration = 0;
                    for (int x = 0; x < tirp.tinstancesList.Count; x++)
                        timeDuration += tirp.tinstancesList[x].tis[tirp.size - 1].endTime - tirp.tinstancesList[x].tis[0].startTime;
                    timeDuration = timeDuration / tirp.tinstancesList.Count;
                    //entityFeaturesLine += tirp.tinstancesList.Count + "," + timeDuration + ",";
                    //tirp.tinstancesList.Clear();
                }
                //else
                entityFeaturesLine += tirp.tinstancesList.Count + "," + timeDuration + ","; // entityFeaturesLine += "0,0,";
                tirp.tinstancesList.Clear();
                timeDuration = 0;
         
                tirpIdx++;
            }
            return entityFeaturesLine;
        }

        /*
        public string mineSequentialSingleEntity(int eIdx, List<TIRP> tirpsList)
        {
            int tirpIdx = 0;
            int HS = 0;
            double timeDuration = 0;

            entityKarma ek = karmE.getEntityKarmaByIdx(eIdx);
            TIRP tirp = null;
            string entityFeaturesLine = "";

            while (tirpIdx < tirpsList.Count )
            {
                HS = 0; timeDuration = 0;
                tirp = tirpsList.ElementAt(tirpIdx);
                List<TimeIntervalSymbol> entityStis = karmE.getEntityTISsByIdx(eIdx);
                for (int i = 0; i < entityStis.Count(); i++)
                {
                    if (tirp.toncepts[0] == entityStis[i].symbol)
                    {
                        //HS++;
                        //timeDuration = timeDuration + entityStis[i].endTime - entityStis[i].startTime;

                        TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[1];
                        tisVec[0] = entityStis[i];
                        TIsInstance tIns = new TIsInstance(tisVec, ek.entityID);
                        addTInstanceToTIRP(ref tirp, tIns, ek);
                    }
                }

                int tonceptIdx = 1;
                while(tonceptIdx < tirp.size)
                {
                    for(int ins = 0; ins < tirp.tinstancesList.Count; ins++)
                    {





                    timeDuration = (timeDuration / HS);
                    entityFeaturesLine += HS + "," + timeDuration;
                }
                else
                    entityFeaturesLine += "0,0";
                entityFeaturesLine += ",";
            }
        }*/

        /*
        public string detectTIRPsSKL(int eIdx, List<TIRP> tirpsList)
        {
            int tirpIdx = 0;
            int HS = 0;
            double timeDuration = 0;
            entityKarma ek = karmE.getEntityKarmaByIdx(eIdx);
            TIRP tirp = null, prevTirp = null;
            Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic;
            string entityFeaturesLine = "";
            
            while (tirpIdx < tirpsList.Count())
            {
                //create new instances vector
                HS = 0; timeDuration = 0;
                tirp = tirpsList.ElementAt(tirpIdx);
                if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic().ContainsKey(eIdx)) // check this - basically they all should appear..
                {
                    HS = karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx].Count();
                    for (int i = 0; i < HS; i++)
                    {
                        TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[1];
                        tisVec[0] = karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx][i];

                        timeDuration += (tisVec[0].endTime - tisVec[0].startTime);

                        TIsInstance tIns = new TIsInstance(tisVec, ek.entityID);
                        addTInstanceToTIRP(ref tirp, tIns, ek);

                    }
                    timeDuration = (timeDuration / HS);
                    entityFeaturesLine += HS + "," + timeDuration;
                    //call recursion and add to the string
                    entityFeaturesLine += detectTIRPsSKLrecurse(ek, tirpIdx++, tirpsList, tirp);
                }
                else
                    entityFeaturesLine += "0,0";
                    
            }
        }

        /*
        private string detectTIRPsSKLrecurse(entityKarma ek, int tirpIdx, List<TIRP> tirpsList, TIRP parenTirp)
        {
            int HS = 0;
            double meanDuration = 0;
            TIRP tirp = tirpsList.ElementAt(tirpIdx);
            int relIdx = tirp.relSize - 1;
            if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.containsTonceptID(tirp.toncepts[1]))
            {
                int frsTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[0]), scndTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[1]);
                if (ek.instancesDicContainsKey(scndTncptIdx, tirp.rels[relIdx], frsTncptIdx))
                {
                    tiListDic = ek.getInstancesDicValuebyKey(scndTncptIdx, tirp.rels[relIdx], frsTncptIdx);
                    for (int tins = 0; tins < prevTirp.tinstancesList.Count; tins++)
                    {
                        if (prevTirp.tinstancesList[tins].entityIdx == ek.entityID && tiListDic.ContainsKey(prevTirp.tinstancesList[tins].tis[prevTirp.size - 1]))
                        {
                            List<TimeIntervalSymbol> tisList = tiListDic[prevTirp.tinstancesList[tins].tis[prevTirp.size - 1]];
                            for (int tiIdx = 0; tiIdx < tisList.Count; tiIdx++)
                            {
                                int rIdx = 0, relStart = (((tirp.size - 2) * (tirp.size - 1)) / 2);
                                for (rIdx = 0; rIdx < tirp.size - 2; rIdx++)
                                {
                                    int rel = KLC.WhichRelationEpsilon(prevTirp.tinstancesList[tins].tis[rIdx], tisList[tiIdx], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                                    if (rel != tirp.rels[relStart + rIdx])
                                        break;
                                }
                                if (rIdx == tirp.size - 2)
                                {
                                    HS++;
                                    TIsInstance tIns = new TIsInstance(tirp.size, prevTirp.tinstancesList[tins].tis, tisList[tiIdx], ek.entityID);
                                    addTInstanceToTIRP(ref tirp, tIns, ek);

                                    int minTime = tIns.tis[0].startTime, maxTime = tIns.tis[0].endTime;
                                    for (int i = 1; i < tirp.size; i++)
                                    {

                                        if (tIns.tis[i].endTime > maxTime)
                                            maxTime = tIns.tis[i].endTime;
                                    }
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
            
        }*/

        public string mineS11Entity(int eIdx, List<TIRP> tirpsList, int tirpSizeLimit, int forBackwards = KLC.backwardsMining) //, int BinHSmeanD)
        {
            int tirpIdx = 0;
            int HS = 0, minOffset = 0;
            double timeDuration = 0, meanOffset = 0;

            entityKarma ek = karmE.getEntityKarmaByIdx(eIdx);
            int endObservTime = karmE.getEntityTISsByIdx(eIdx)[karmE.getEntityTISsByIdx(eIdx).Count - 1].startTime; // the end time of the observation period
            TIRP tirp = null, prevTirp = null;
            Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic;
            string entityFeaturesLine = "";
            while (tirpIdx < tirpsList.Count && tirpIdx < tirpSizeLimit)
            {
                HS = 0; timeDuration = 0; minOffset = 0; meanOffset = 0;
                tirp = tirpsList.ElementAt(tirpIdx);
                if(tirpIdx > 0)
                    prevTirp = tirpsList.ElementAt(tirpIdx-1);
                if (tirp.size == 1)
                {
                    if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic().ContainsKey(eIdx)) // check this - basically they all should appear..
                    {
                        HS = karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx].Count();
                        minOffset = Math.Max(0, endObservTime - karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx][karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx].Count - 1].endTime); 
                        for (int i = 0; i < HS; i++)
                        {
                            TimeIntervalSymbol[] tisVec = new TimeIntervalSymbol[1];
                            tisVec[0] = karmE.getTonceptByID(tirp.toncepts[0]).getTonceptHorizontalDic()[eIdx][i];

                            timeDuration += (tisVec[0].endTime - tisVec[0].startTime);
                            meanOffset += Math.Max(0, endObservTime - tisVec[0].endTime);

                            TIsInstance tIns = new TIsInstance(tisVec, ek.entityID);
                            addTInstanceToTIRP(ref tirp, tIns, ek);

                        }
                        timeDuration = (timeDuration / HS);
                        meanOffset = (meanOffset / HS);
                        entityFeaturesLine += HS + "," + timeDuration + "," + minOffset + "," + meanOffset;
                    }
                    else
                        entityFeaturesLine += "0,0,0,0";
                    entityFeaturesLine += ",";
                }
                else if (tirp.size == 2)
                {
                    if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.containsTonceptID(tirp.toncepts[1]))
                    {
                        int frsTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[0]), scndTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[1]);
                        if (ek.instancesDicContainsKey(scndTncptIdx, tirp.rels[0], frsTncptIdx))
                        {
                            tiListDic = ek.getInstancesDicValuebyKey(scndTncptIdx, tirp.rels[0], frsTncptIdx);
                            minOffset = Math.Max(0, endObservTime - tiListDic.ElementAt(0).Key.endTime);
                            //HS = tiListDic.Count();
                            foreach (KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListDic)
                                foreach (TimeIntervalSymbol tis in tiListTi.Value)
                                {
                                    timeDuration += (tiListTi.Key.endTime - tis.startTime);
                                    int offset = Math.Max(0, endObservTime - tiListTi.Key.endTime);
                                    if(minOffset > offset)
                                        minOffset = offset;
                                    meanOffset += offset;
                                    TIsInstance tIns = new TIsInstance(tiListTi.Key, tis, ek.entityID);
                                    addTInstanceToTIRP(ref tirp, tIns, ek);
                                    HS++;
                                }
                            timeDuration = (timeDuration / HS);
                            meanOffset = (meanOffset / HS);
                            entityFeaturesLine += HS + "," + timeDuration + "," + minOffset + "," + meanOffset;
                        }
                        else
                            entityFeaturesLine += "0,0,0,0";
                    }
                    else
                        entityFeaturesLine += "0,0,0,0";
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
                    if (karmE.containsTonceptID(tirp.toncepts[0]) && karmE.containsTonceptID(tirp.toncepts[1]))
                    {
                        int frsTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[0]), scndTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[1]);
                        if (ek.instancesDicContainsKey(scndTncptIdx, tirp.rels[relIdx], frsTncptIdx))
                        {
                            tiListDic = ek.getInstancesDicValuebyKey(scndTncptIdx, tirp.rels[relIdx], frsTncptIdx);
                            minOffset = Math.Max(0, endObservTime - tiListDic.ElementAt(0).Value.ElementAt(0).endTime);
                            for (int tins = 0; tins < prevTirp.tinstancesList.Count; tins++)
                            {
                                if (prevTirp.tinstancesList[tins].entityIdx == ek.entityID && tiListDic.ContainsKey(prevTirp.tinstancesList[tins].tis[prevTirp.size - 1]))
                                {
                                    List<TimeIntervalSymbol> tisList = tiListDic[prevTirp.tinstancesList[tins].tis[prevTirp.size - 1]];
                                    for (int tiIdx = 0; tiIdx < tisList.Count; tiIdx++)
                                    {
                                        int rIdx = 0, relStart = (((tirp.size - 2) * (tirp.size - 1)) / 2);
                                        for (rIdx = 0; rIdx < tirp.size - 2; rIdx++)
                                        {
                                            int rel = 0;
                                            if(forBackwards == KLC.forwardMining)
                                                rel = KLC.WhichRelationEpsilon(prevTirp.tinstancesList[tins].tis[rIdx], tisList[tiIdx], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                                            else
                                                rel = KLC.WhichRelationEpsilon(tisList[tiIdx], prevTirp.tinstancesList[tins].tis[rIdx], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                                            if (rel != tirp.rels[relStart + rIdx])
                                                break;
                                        }
                                        if (rIdx == tirp.size - 2)
                                        {
                                            HS++;
                                            TIsInstance tIns = new TIsInstance(tirp.size, prevTirp.tinstancesList[tins].tis, tisList[tiIdx], ek.entityID);
                                            addTInstanceToTIRP(ref tirp, tIns, ek);

                                            int minTime = 0, maxTime = 0;
                                            minTime = tIns.tis[0].startTime;
                                            maxTime = tIns.tis[0].endTime;
                                            for (int i = 1; i < tirp.size; i++)
                                            {
                                                if (forBackwards == KLC.forwardMining)
                                                {
                                                    if (tIns.tis[i].endTime > maxTime)
                                                        maxTime = tIns.tis[i].endTime;
                                                }
                                                else
                                                {
                                                    if (tIns.tis[i].startTime < minTime)
                                                        minTime = tIns.tis[i].startTime;
                                                }
                                            }
                                            int offset = Math.Max(0, endObservTime - maxTime);
                                            timeDuration = timeDuration + (maxTime - minTime); // CHECK THIS, it looks located WRONG!
                                            if (minOffset > offset)
                                                minOffset = offset;
                                            meanOffset += offset;
                                        }
                                    }
                                }

                            }
                            if (HS == 0)
                                entityFeaturesLine += "0,0,0,0";
                            else
                            {
                                timeDuration = (timeDuration / HS);
                                meanOffset = (meanOffset / HS);
                                entityFeaturesLine += HS + "," + timeDuration + "," + minOffset + "," + meanOffset;
                            }
                        }
                        else
                            entityFeaturesLine += "0,0,0,0";
                    }
                    else
                        entityFeaturesLine += "0,0,0,0";
                    entityFeaturesLine += ",";
                }
                tirpIdx++;
            }
            return entityFeaturesLine;
        }
        
        private static void addTInstanceToTIRP(ref TIRP tirp, TIsInstance tIns, entityKarma ek)
        {
            tirp.tinstancesList.Add(tIns);
            if (!tirp.entInstancesDic.ContainsKey(ek.entityID))
            {
                List<TIsInstance> tistanceList = new List<TIsInstance>();
                tistanceList.Add(tIns);
                tirp.entInstancesDic.Add(ek.entityID, tistanceList);
            }
            else
                tirp.entInstancesDic[ek.entityID].Add(tIns);
        }
                
        public static List<TIRP> readTIRPsFile(string tirpsFilePath, ref List<int> tonceptIds, int rel_style, bool stripOutcome = true, int outcomeToncept = -1, bool readInstances = false)
        {
         // if (stripOutcome == true)
         //     readInstances = false;
            Dictionary<string, int> relsDic = new Dictionary<string, int>();
            //tonceptIds = new List<int>();
            if (rel_style == KLC.RELSTYLE_ALLEN7)
            {
                relsDic.Add("<", 0); relsDic.Add("m", 1); relsDic.Add("o", 2); relsDic.Add("f", 3); relsDic.Add("c", 4); relsDic.Add("=", 5); relsDic.Add("s", 6); relsDic.Add("-", 7);
            }
            else if (rel_style == KLC.RELSTYLE_KL3)
            {
                relsDic.Add("<", KLC.KL_BEFORE); relsDic.Add("O", KLC.KL_OVERLAP); relsDic.Add("C", KLC.KL_CONTAIN); relsDic.Add("-", 7);
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
                    tirp.tncptsRels = lineVec[1] + "-" + lineVec[2];
                    string[] toncepts = lineVec[1].Split('-');
                    for (int t = 0; t < tirpSize; t++)
                    {
                        tirp.toncepts[t] = int.Parse(toncepts[t]);
                        if (!tonceptIds.Contains(tirp.toncepts[t]))
                            tonceptIds.Add(tirp.toncepts[t]);
                    }
                    string[] rels = lineVec[2].Split('.');
                    for (int r = 0; r < tirp.relSize; r++)
                        tirp.rels[r] = relsDic[rels[r]];
                    if (stripOutcome == true && tirp.toncepts[tirp.size-1] == outcomeToncept)
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
                        int verSup = int.Parse(lineVec[3]);
                        double meanhorSup = double.Parse(lineVec[4]);
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
                    if(tirp.size > 0)
                        tirpsList.Add(tirp);
                }
            }
            tr.Close();

            /*TextWriter tw = new StreamWriter(tirpsFilePath.Replace(".txt", "_output.txt"));
            for(int t = 0; t < tirpsList.Count; t++)
            {
                int counter = 0;
                string tirpDef = "", tirpIns = "";
                TIRP tirp = tirpsList.ElementAt(t);
                tirpDef += tirp.size + " ";
                for (int ts = 0; ts < tirp.size; ts++)
                    tirpDef += (tirp.toncepts[ts] + "-");
                tirpDef += " ";
                for (int r = 0; r < tirp.relSize; r++)
                    tirpDef += (KLC.ALLEN7_RELCHARS[tirp.rels[r]] + ".");
                tirpDef += " ";
                for (int e = 0; e < tirp.entInstancesDic.Count; e++)
                {
                    for (int ei = 0; ei < tirp.entInstancesDic.ElementAt(e).Value.Count; ei++)
                    {
                        TIsInstance tis = tirp.entInstancesDic.ElementAt(e).Value.ElementAt(ei);
                        tirpIns += (tis.entityIdx + " ");
                        for (int i = 0; i < tis.tis.Length; i++)
                            tirpIns += ("[" + tis.tis[i].startTime + "-" + tis.tis[i].endTime + "]");
                        tirpIns += " ";
                        
                    }
                    counter = counter + tirp.entInstancesDic.ElementAt(e).Value.Count;
                 }
                 tw.WriteLine(tirpDef + counter + " " + tirp.entInstancesDic.Count + " " +  tirpIns);
            }
            tw.Close();*/

            return tirpsList;
        }

        public static void prinTirpsList(List<TIRP> tirpsList, string printFile, KarmE karmE)
        {
            TextWriter tw = new StreamWriter(printFile);
            for (int t = 0; t < tirpsList.Count; t++)
            {
                int counter = 0;
                string tirpDef = "", tirpIns = "";
                TIRP tirp = tirpsList.ElementAt(t);
                tirpDef += tirp.size + " ";
                for (int ts = 0; ts < tirp.size; ts++)
                    tirpDef += (tirp.toncepts[ts] + "-");
                tirpDef += " ";
                for (int r = 0; r < tirp.relSize; r++)
                    tirpDef += (KLC.ALLEN7_RELCHARS[tirp.rels[r]] + ".");
                tirpDef += " ";
                for (int e = 0; e < tirp.entInstancesDic.Count; e++)
                {
                    for (int ei = 0; ei < tirp.entInstancesDic.ElementAt(e).Value.Count; ei++)
                    {
                        TIsInstance tis = tirp.entInstancesDic.ElementAt(e).Value.ElementAt(ei);
                        tirpIns += (tis.entityIdx + " ");
                        for (int i = tis.tis.Length-1; i >= 0; i--) // for (int i = 0; i < tis.tis.Length; i++)
                            tirpIns += ("[" + tis.tis[i].startTime + "-" + tis.tis[i].endTime + "]");
                        tirpIns += " ";

                    }
                    counter = counter + tirp.entInstancesDic.ElementAt(e).Value.Count;
                }
                tw.WriteLine(tirpDef + counter + " " + tirp.entInstancesDic.Count + " " + tirpIns);
            }
            tw.Close();
        }
    }
}
