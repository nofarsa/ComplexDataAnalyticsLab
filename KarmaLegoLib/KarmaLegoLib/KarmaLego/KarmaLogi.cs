using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

//using Tonception;

namespace KarmaLegoLib
{
    public class KarmaLogi
    {
        int karmalegologi;
        bool print;
        bool trans;
        int relations_style;   //number of reations (Allen7, KL3, KL2)
        int epsilon;
        int min_ver_sup;       //the exact value of the treshold in number of entities
        int max_gap;
        int entitieSize;
        int[][] logiRelsIndxs;
        int[] entitiesVec;
        Dictionary<int, List<TimeIntervalSymbol>> entityTISs; // = new Dictionary<int, List<TimeIntervalSymbol>>();
        Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();//key:t_id, val:toncept
        twoSizedTIRPsMATRIXEntry[][] twoSizedTIRPsMatrix; //frstIdx: key, scndIdx: vals
        int[][][] transition;  // transition table

        public KarmaLogi(bool seTrans, bool printSet, int relationStyleSet, int epsilonSet, int maxGap, int karmaLegoLogi, string dsFilePath) // TonceptSet ds) //DS)
        {
            karmalegologi = karmaLegoLogi;
            trans = seTrans;
            print = printSet;
            relations_style = relationStyleSet;
            epsilon = epsilonSet;
            max_gap = maxGap;
            //List<int> entitiesList = new List<int>();
            //for (int i = 0; i < ds.Count; i++)
            //    if (!entitiesList.Contains(ds.ElementAt(i).entityID))
            //        entitiesList.Add(ds.ElementAt(i).entityID);
            //read_KL_file(dsFilePath);
            entityTISs = KLC.read_KL_file(dsFilePath);
            entitieSize = entityTISs.Count; // entitiesList.Count; // ds.entities.Count;
            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = entityTISs.Keys.ElementAt(eIdx); // ds.entities[eIdx].entityID;

            twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][];//initialize keys vector
            for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
                twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS];// initialize val vector
            
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
            logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
            logiRelsIndxs[0/*3*/] = new int[3]  { 2, 1, 0 };
            logiRelsIndxs[1/*4*/] = new int[6]  { 5, 4, 2, 3, 1, 0 };
            logiRelsIndxs[2/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
            logiRelsIndxs[3/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
            logiRelsIndxs[4/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
            logiRelsIndxs[5/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
            logiRelsIndxs[6/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
            logiRelsIndxs[7/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };

        }
        
        public void KarmaAndLogi(bool seTrans, bool runKarma, /*TonceptSet ds,*/ string outputFile, int trgTncpt, double minVerSupport)
        { // LOGI HAS TO BE ENGINEERED BACKWARDS
            try
            {
                min_ver_sup = (int)((minVerSupport / 100) * (double)entitieSize);
                trans = seTrans;
                TextWriter tw = new StreamWriter(outputFile);
                if(runKarma)
                    Karma(); //ds);
                //LOGI starts here
                int tTrgt = 0;
                if (trgTncpt < KLC.NUM_OF_SYMBOLS)
                {
                    tTrgt = toncepts[trgTncpt].tonceptINDEX;
                    runLego(toncepts[trgTncpt].tonceptINDEX, toncepts[trgTncpt].tonceptID, tw);
                }
                else
                {
                    Parallel.ForEach(toncepts, toncept =>
                    //for (tTrgt = 0; tTrgt < toncepts.Count; tTrgt++)
                    {
                        //int tTrgtIdx = toncepts.Values.ElementAt(tTrgt).tonceptINDEX;
                        int tTrgtIdx = toncept.Value.tonceptINDEX; 
                        //int tTrgtID = toncepts.Values.ElementAt(tTrgt).tonceptID;
                        int tTrgtID = toncept.Value.tonceptID; 
                        
                        runLego(tTrgtIdx, tTrgtID, tw);
                    } );
                }
                tw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private void runLego(int tTrgtIdx, int tTrgtID, TextWriter tw)
        {
            if (twoSizedTIRPsMatrix[tTrgtIdx] != null)
            {
                tw.WriteLine("1 " + tTrgtID + "- " + toncepts[tTrgtID].verticalSupport + " " + toncepts[tTrgtID].verticalSupport);
                //for (int tErly = 0; tErly < toncepts.Count; tErly++)
                Parallel.ForEach(toncepts, toncept =>
                {
                    twoSizedTIRPsMATRIXEntry tsTME = twoSizedTIRPsMatrix[tTrgtIdx][toncept.Value.tonceptINDEX];// toncepts.Values.ElementAt(tErly).tonceptINDEX];
                    if (tsTME != null)
                        for (int rel = 0; rel < relations_style; rel++)
                        //Parallel.For(0, relations_style, rel =>
                        {
                            pairsMATRIXrelEntry pMxRelE = tsTME.prsMxRelVec[rel];
                            if (pMxRelE != null && pMxRelE.verticalSupport >= min_ver_sup)
                            {
                                int tErlyID = toncept.Value.tonceptID; // toncepts.Values.ElementAt(tErly).tonceptID;
                                TIRP twoSzdTIRP = new TIRP(tTrgtID, tErlyID, rel);
                                string writeLine = "";
                                for (int ins = 0; ins < pMxRelE.instancesDicList.Count; ins++)
                                {
                                    dicTIentID entTITrgt = pMxRelE.instancesDicList.Keys.ElementAt(ins);
                                    List<TimeIntervalSymbol> tiErlyList = pMxRelE.instancesDicList.Values.ElementAt(ins);
                                    for (int dicIdx = 0; dicIdx < tiErlyList.Count; dicIdx++)
                                    {
                                        TimeIntervalSymbol tisErly = tiErlyList.ElementAt(dicIdx);
                                        TIsInstance tisInsNew = new TIsInstance(entTITrgt.TIS, tisErly, entTITrgt.entityIndex);
                                        twoSzdTIRP.tinstancesList.Add(tisInsNew);
                                        if (karmalegologi == KLC.KarmaLogi)
                                            writeLine = writeLine + entitiesVec[entTITrgt.entityIndex] + " [" + tisErly.startTime + "-" + tisErly.endTime + "][" + entTITrgt.TIS.startTime + "-" + entTITrgt.TIS.endTime + "] ";
                                        else
                                            writeLine = writeLine + entitiesVec[entTITrgt.entityIndex] + " [" + entTITrgt.TIS.startTime + "-" + entTITrgt.TIS.endTime + "][" + tisErly.startTime + "-" + tisErly.endTime + "] ";
                                    }
                                }// consider to integrate this in the constructor of the TIRP, or copy it here.
                                if (print == true)
                                {
                                    if (karmalegologi == KLC.KarmaLogi)
                                        tw.WriteLine("2 " + tErlyID + "-" + tTrgtID + "- " + KLC.ALLEN7_RELCHARS[rel] + ". " + pMxRelE.instancesDicList.Count + " " + pMxRelE.verticalSupport);
                                    else
                                        tw.WriteLine("2 " + tTrgtID + "-" + tErlyID + "- " + KLC.ALLEN7_RELCHARS[rel] + ". " + pMxRelE.instancesDicList.Count + " " + pMxRelE.verticalSupport);
                                    tw.WriteLine(writeLine);
                                }
                                Lego(twoSzdTIRP, tw);
                            }
                        }  // );
                }   );
            }
        }

        private void Lego(TIRP tirp, TextWriter tw)
        {
            string writeLine = "";
            List<int[]> candidates = null;
            //for (int tIdx = 0; tIdx < toncepts.Count; tIdx++)
            Parallel.ForEach(toncepts, toncept =>
            {
                for (int seedRelation = 0; seedRelation < relations_style; seedRelation++)
                    //if (pairMxEntryRelEXIST(tirp.toncepts[tirp.size - 1], toncepts.Values.ElementAt(tIdx).tonceptID, seedRelation))
                    if (pairMxEntryRelEXIST(tirp.toncepts[tirp.size - 1], toncept.Value.tonceptID, seedRelation))
                    {
                        candidates = generateCandidates(tirp, seedRelation);
                        for (int cand = 0; cand < candidates.Count; cand++)
                        {   //consider creating tirpNew in searchSupportingInstances and to return it
                            //TIRP tirpNew = new TIRP(tirp, toncepts.Values.ElementAt(tIdx).tonceptID, seedRelation, candidates.ElementAt(cand));//, KLC.KarmaLogi);
                            TIRP tirpNew = new TIRP(tirp, toncept.Value.tonceptID, seedRelation, candidates.ElementAt(cand));//, KLC.KarmaLogi);
                            bool seedRelCnndtsEmpty = searchSupportingInstances(ref tirpNew, tirp.tinstancesList);
                            if (tirpNew.entitieSupport.Count >= min_ver_sup) // .verticalSupport >= min_ver_sup)
                            {
                                if (print == true)
                                    tirpNew.printTIRP(tw, entitiesVec, karmalegologi, logiRelsIndxs); //print
                                Lego(tirpNew, tw);
                            }
                        }
                    }
            }   );
        }

        public void Karma(/*TonceptSet ds*/)
        {
            int index = 0;
            try
            {
                int[] relStat = new int[7];
                for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                {
                    List<TimeIntervalSymbol> tisList = entityTISs[entitiesVec[eIdx]];
                    //for (int ti1Idx = 0; ti1Idx < ds.entities[eIdx].symbolic_intervals.Count; ti1Idx++)
                    for (int ti1Idx = 0; ti1Idx < tisList.Count; ti1Idx++) {
                    //Parallel.ForEach(tisList, tis => {
                    //    int ti1Idx = tisList.IndexOf(tis);
                    //    TimeIntervalSymbol firsTis = tis;
                        TimeIntervalSymbol firsTis = tisList[ti1Idx]; // ds.entities[eIdx].symbolic_intervals[ti1Idx];
                        addToncept(eIdx, firsTis, ref index, true); // update toncept entity support
                        //karma..
                        //for (int ti2Idx = ti1Idx + 1; ti2Idx < ds.entities[eIdx].symbolic_intervals.Count; ti2Idx++)
                        for (int ti2Idx = ti1Idx + 1; ti2Idx < tisList.Count; ti2Idx++)
                        {
                            TimeIntervalSymbol secondTis = tisList[ti2Idx]; // ds.entities[eIdx].symbolic_intervals[ti2Idx];
                            if (firsTis.symbol != secondTis.symbol)
                            {
                                addToncept(eIdx, secondTis, ref index, false); // DONT update toncept entity support
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
                    } //);
                }
                pruneToncepts();
                pruneMatrixEntriesPairDics();
            }
            catch (Exception e) 
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private void indexTISPair(TimeIntervalSymbol firsTis, TimeIntervalSymbol secondTis, int relation, int eIdx)
        {
            if (karmalegologi == KLC.KarmaLego)
                indexTimeInetervalSymbolsPair(firsTis, secondTis, relation, eIdx);
            else
                indexTimeInetervalSymbolsPair(secondTis, firsTis, relation, eIdx);
        }

        private bool pairMxEntryRelEXIST(int frstTncptID, int scndTncptID, int relation)
        {
            if (twoSizedTIRPsMatrix[toncepts[frstTncptID].tonceptINDEX] == null)
                return false;
            else
                if (twoSizedTIRPsMatrix[toncepts[frstTncptID].tonceptINDEX][toncepts[scndTncptID].tonceptINDEX] == null)
                    return false;
                else
                    if (twoSizedTIRPsMatrix[toncepts[frstTncptID].tonceptINDEX][toncepts[scndTncptID].tonceptINDEX].prsMxRelVec[relation] == null)
                        return false;
                    else
                        return true;
        }

        private bool searchSupportingInstances(ref TIRP tNew, List<TIsInstance> tinstances)
        {
            bool seedRelEmpty = true;
            //bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
            int topRelIdx = ((tNew.size - 1) * (tNew.size - 2) / 2), seedRelIdx = (tNew.size * (tNew.size - 1) / 2 - 1);
            int seedRelation = tNew.rels[seedRelIdx];
            int tncptLst = tNew.toncepts[tNew.size - 2], tncptNew = tNew.toncepts[tNew.size - 1];

            twoSizedTIRPsMATRIXEntry tsTME = twoSizedTIRPsMatrix[toncepts[tncptLst].tonceptINDEX][toncepts[tncptNew].tonceptINDEX];
            pairsMATRIXrelEntry pMxRelE = tsTME.prsMxRelVec[seedRelation];
            Dictionary<dicTIentID, List<TimeIntervalSymbol>> dicTIList = pMxRelE.instancesDicList;
            for (int tins = 0; tins < tinstances.Count; tins++)
            {
                dicTIentID TISearch = new dicTIentID(tinstances[tins].tis[tNew.size - 2], tinstances[tins].entityIdx);
                if (dicTIList.ContainsKey(TISearch))
                {
                    seedRelEmpty = false;
                    List<TimeIntervalSymbol> tisList = dicTIList[TISearch];
                    for (int i = 0; i < tisList.Count; i++)
                    {
                        int relIdx = 0;
                        for (relIdx = topRelIdx; relIdx < seedRelIdx; relIdx++)
                            if (!checkRelationAmongTwoTIs(tinstances[tins].tis[relIdx - topRelIdx], tisList.ElementAt(i), tNew.rels[relIdx]))//if (!KLC.checkRelationAmongTwoTIs(tinstances[tins].tis[relIdx - topRelIdx], tisList.ElementAt(i), tNew.rels[relIdx], epsilon, max_gap))
                                break;
                        if (relIdx == seedRelIdx)
                        {
                            TIsInstance newIns = new TIsInstance(tNew.size, tinstances[tins].tis, tisList.ElementAt(i), tinstances[tins].entityIdx); // D);
                            // entitieSupport[newIns.entityIdx] = true;
                            tNew.tinstancesList.Add(newIns);
                            tNew.addEntity(newIns.entityIdx);
                        }
                    }
                }
            }
            /*if(tNew.tinstancesList.Count > 0 )
                for (int tins = 0; tins < entitieSize; tins++)
                    if (entitieSupport[tins] == true)
                        tNew.verticalSupport++;*/
            return seedRelEmpty;
        }

        private bool checkRelationAmongTwoTIs(TimeIntervalSymbol tinstanceTIS, TimeIntervalSymbol listTIS, int relation)
        {
            if (karmalegologi == KLC.KarmaLego)
                return KLC.checkRelationAmongTwoTIs(tinstanceTIS, listTIS, relation, epsilon, max_gap);
            else
                return KLC.checkRelationAmongTwoTIs(listTIS, tinstanceTIS, relation, epsilon, max_gap);
        }

        private List<int[]> generateCandidates(TIRP tirp, int seedRel)
        {
            if (trans)
                return CandidatesGeneration_Trans(tirp, seedRel);
            else
                return CandidatesGeneration_Naive(tirp, seedRel);
        }

        private List<int[]> CandidatesGeneration_Trans(TIRP tirp, int seedRel)
        {
            int columSize = tirp.size; // this is the size of the extended TIRP (the generated TIRP is tirp.size+1)
            int topCndRelIdx = 0;
            int btmRelIdx = columSize - 2;
            List<int[]> candidatesList = new List<int[]>();
            int[] candidate = new int[columSize]; // Remember: the candidate relIdx is 0 -> 0, 1 -> 2, 3 - > 5,..
            candidate[columSize-1] = seedRel;         // and in each size of the tirp it is the last column
            candidatesList.Add(candidate);
            for (int relIdxToSet = btmRelIdx; relIdxToSet >= topCndRelIdx; relIdxToSet--)
            {
                int leftTirpIdx = ((relIdxToSet + 1) * relIdxToSet / 2) + relIdxToSet;
                int belowCndIdx = relIdxToSet + 1;
                int candListSize = candidatesList.Count;
                for (int candIdx = 0; candIdx < candListSize; candIdx++)
                {
                    candidate = candidatesList.ElementAt(candIdx);
                    int fromRel = transition[tirp.rels[leftTirpIdx]][candidate[belowCndIdx]][0];
                    int toRel = transition[tirp.rels[leftTirpIdx]][candidate[belowCndIdx]][1];
                    for (int rel = fromRel; rel <= toRel; rel++)
                    {
                        if (rel > fromRel)
                        {
                            int[] newCandidate = new int[columSize];
                            for (int rIdx = columSize - 1; rIdx > relIdxToSet; rIdx--)
                                newCandidate[rIdx] = candidate[rIdx];
                            newCandidate[relIdxToSet] = rel;
                            candidatesList.Add(newCandidate);
                        }
                        else
                            candidate[relIdxToSet] = rel;
                    }
                }
            }
            return candidatesList;
        }

        private List<int[]> CandidatesGeneration_Naive(TIRP tirp, int seedRel)
        {
            int columSize = tirp.size; // this is the size of the extended TIRP (the generated TIRP is tirp.size+1)
            int relCombs = (int)Math.Pow( relations_style, (columSize - 1));
            List<int[]> candidatesList = new List<int[]>();
            for (int comb = 0; comb < relCombs; comb++)
            {
                int[] candidate = new int[columSize];
                candidate[columSize - 1] = seedRel;  
                for (int relIdx = 0; relIdx < (columSize - 1); relIdx++)
                    candidate[relIdx] = (comb / (int)Math.Pow(relations_style, relIdx)) % relations_style;
                candidatesList.Add(candidate);
            }
            return candidatesList;
        }

        private void transCandidates(TIRP tirp, int relIdxToSet, ref List<int[]> candidatesList, int relsToGenerate)//, int topRelIdx, int btmRelIdx)
        {
            if (relIdxToSet < 0)
                return;
            int leftIdx  = ((relIdxToSet + 1) * relIdxToSet / 2) + relIdxToSet;
            int belowIdx = relIdxToSet + 1;
            for (int candIdx = 0; candIdx < candidatesList.Count; candIdx++)
            {
                int[] candidate = candidatesList.ElementAt(candIdx);
                int fromRel = transition[tirp.rels[leftIdx]][candidate[belowIdx]][0];
                int toRel   = transition[tirp.rels[leftIdx]][candidate[belowIdx]][1];
                for (int rel = fromRel; rel <= toRel; rel++)
                {
                    if (rel > fromRel)
                    {
                        int[] newCandidate = new int[relsToGenerate];
                        for (int rIdx = relsToGenerate - 1; rIdx > relIdxToSet; rIdx--)
                            newCandidate[rIdx] = candidate[rIdx];
                        newCandidate[relIdxToSet] = rel;
                        candidatesList.Add(newCandidate);
                        candIdx++;
                    }
                    candidate[relIdxToSet] = rel;
                }
            }
        }
                
        private void addToncept( int entityIdx, TimeIntervalSymbol tis, ref int index, bool updateTonceptEntitiesSupport)
        {
            TemporalConcept tc;
            if (!toncepts.ContainsKey(tis.symbol))
            {
                tc = new TemporalConcept(tis.symbol, index);
                index++;
                toncepts.Add(tis.symbol, tc);
            }
            else
                tc = toncepts[tis.symbol];
            if (updateTonceptEntitiesSupport)
                tc.entitiesSupport[entityIdx] = true;
        }

        private void indexTimeInetervalSymbolsPair(TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal, int relation, int entityIdx)
        {
            int tKeyIdx = toncepts[tisKey.symbol].tonceptINDEX;
            int tValIdx = toncepts[tisVal.symbol].tonceptINDEX;
            if (twoSizedTIRPsMatrix[tKeyIdx][tValIdx] == null)
                twoSizedTIRPsMatrix[tKeyIdx][tValIdx] = new twoSizedTIRPsMATRIXEntry(relations_style);

            twoSizedTIRPsMATRIXEntry twoSizedTIRP_P = twoSizedTIRPsMatrix[tKeyIdx][tValIdx];
            pairsMATRIXrelEntry pMxRelE = twoSizedTIRP_P.prsMxRelVec[relation];
            if (pMxRelE == null)
            {
                twoSizedTIRPsMatrix[tKeyIdx][tValIdx].prsMxRelVec[relation] = new pairsMATRIXrelEntry();
                pMxRelE = twoSizedTIRPsMatrix[tKeyIdx][tValIdx].prsMxRelVec[relation];
            }
            dicTIentID tiEntID = new dicTIentID(tisKey, entityIdx);
            if (pMxRelE.instancesDicList.ContainsKey(tiEntID))
                pMxRelE.instancesDicList[tiEntID].Add(tisVal);
            else
            {
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(tisVal);
                pMxRelE.instancesDicList.Add(tiEntID, tisList);
            }
            pMxRelE.entitieSupport[entityIdx] = true;
        }

        private void pruneToncepts()
        {
            for (int t = 0; t < toncepts.Count; t++)
            {
                TemporalConcept tc = toncepts.Values.ElementAt(t);
                for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                    if (tc.entitiesSupport[eIdx] == true)
                        tc.verticalSupport++;
                tc.entitiesSupport = null;
                if (tc.verticalSupport < min_ver_sup)
                {
                    twoSizedTIRPsMatrix[tc.tonceptINDEX] = null;
                    for (int idx = 0; idx < toncepts.Count; idx++)
                        if (tc.tonceptINDEX != idx && twoSizedTIRPsMatrix[idx] != null)
                            twoSizedTIRPsMatrix[idx][tc.tonceptINDEX] = null;
                    toncepts.Remove(tc.tonceptID);
                    t--;
                }
            }
        }

        private void pruneMatrixEntriesPairDics()
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
                                for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                                    if (pMxRelE.entitieSupport[eIdx] == true)
                                        pMxRelE.verticalSupport++;
                                if (pMxRelE.verticalSupport < min_ver_sup)
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
        }

        public void test()
        {
            TIRP tirp = new TIRP(34, 45, 3);
            List<int[]> candidates = CandidatesGeneration_Naive(tirp, 2);
            tirp.size = 3;
            candidates = CandidatesGeneration_Naive(tirp, 2);
            tirp.size = 4;
            candidates = CandidatesGeneration_Naive(tirp, 2);
            tirp.size = 5;
            candidates = CandidatesGeneration_Naive(tirp, 2);

        }
    }
}
