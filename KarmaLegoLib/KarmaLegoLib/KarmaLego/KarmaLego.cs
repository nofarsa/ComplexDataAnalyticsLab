using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Tonception;

namespace KarmaLegoLib
{

    public class KarmaLego
    {
        bool print;
        bool trans;
        int relations_style;   //number of reations (Allen7, KL3, KL2)
        int epsilon;
        int min_ver_sup;       //the exact value of the treshold in number of entities
        int max_gap;
        int entitieSize;
        int[] entitiesVec;
        Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();
        twoSizedTIRPsMATRIXEntry[][] twoSizedTIRPsMatrix;
        int[][][] transition;  // transition table

        public KarmaLego(bool seTrans, bool printSet, int relationStyleSet, int epsilonSet, double minVerSupport, int maxGap, TonceptSet ds) //DS)
        {
            trans = seTrans;
            print = printSet;
            relations_style = relationStyleSet;
            epsilon = epsilonSet;
            max_gap = maxGap;
            entitieSize = ds.entities.Count;
            min_ver_sup = (int)((minVerSupport / 100) * (double)entitieSize);
            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = ds.entities[eIdx].entityID;

            twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][];
            for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
                twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS];
            if (trans)
            {
                string tesTrans = KLC.LoadTransitionTable(ref transition);
                if (tesTrans.Length > 0)
                    transition = null;
            }

        }

        public void KarmaAndLego(bool seTrans, bool runKarma, TonceptSet ds, string outputFile)
        {
            try
            {
                trans = seTrans;
                TextWriter tw = new StreamWriter(outputFile);
                int time = 0;
                if(runKarma)
                    Karma(ds);
                for (int t1 = 0; t1 < toncepts.Count; t1++)
                {
                    if (twoSizedTIRPsMatrix[toncepts.Values.ElementAt(t1).tonceptINDEX] != null)
                    {
                        int t1ID = toncepts.Values.ElementAt(t1).tonceptID;
                        tw.WriteLine("1 " + t1ID + "- " +  toncepts.Values.ElementAt(t1).verticalSupport + " " + toncepts.Values.ElementAt(t1).verticalSupport);
                        for (int t2 = 0; t2 < toncepts.Count; t2++)
                        {
                            twoSizedTIRPsMATRIXEntry tsTME = twoSizedTIRPsMatrix[toncepts.Values.ElementAt(t1).tonceptINDEX][toncepts.Values.ElementAt(t2).tonceptINDEX];
                            if (tsTME != null)
                                for (int r = 0; r < relations_style; r++)
                                {
                                    pairsMATRIXrelEntry pMxRelE = tsTME.prsMxRelVec[r];
                                    if (pMxRelE != null && pMxRelE.verticalSupport >= min_ver_sup)
                                    {
                                        int t2ID = toncepts.Values.ElementAt(t2).tonceptID;
                                        TIRP twoSzdTIRP = new TIRP(t1ID, t2ID, r);
                                        string writeLine = "";
                                        for (int ins = 0; ins < pMxRelE.instancesDicList.Count; ins++)
                                        {
                                            dicTIentID entTI = pMxRelE.instancesDicList.Keys.ElementAt(ins);
                                            List<TimeIntervalSymbol> dicList = pMxRelE.instancesDicList.Values.ElementAt(ins);
                                            for(int dicIdx = 0; dicIdx < dicList.Count; dicIdx++)
                                            {
                                                TimeIntervalSymbol TI2 = dicList.ElementAt(dicIdx);
                                                TIsInstance tisInsNew = new TIsInstance(entTI.TIS, TI2, entTI.entityIndex);
                                                twoSzdTIRP.tinstancesList.Add(tisInsNew);
                                                writeLine = writeLine + entitiesVec[entTI.entityIndex] + " [" + entTI.TIS.startTime + "-" + entTI.TIS.endTime + "][" + TI2.startTime + "-" + TI2.endTime + "] ";
                                            }
                                        }// consider to integrate this in the constructor of the TIRP, or copy it here.
                                        if (print == true)
                                        {
                                            tw.WriteLine("2 " + t1ID + "-" + t2ID + "- " + KLC.ALLEN7_RELCHARS[r] + ". " + pMxRelE.instancesDicList.Count + " " + pMxRelE.verticalSupport);
                                            tw.WriteLine(writeLine);
                                        }
                                        Lego(twoSzdTIRP, tw);
                                    }
                                }
                        }
                    }
                }
                tw.Close();
                time = 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private void Lego(TIRP tirp, TextWriter tw)
        {
            string writeLine = "";
            List<int[]> candidates = null;
            for (int toncept = 0; toncept < toncepts.Count; toncept++)
            {
                for (int seedRelation = 0; seedRelation < relations_style; seedRelation++)
                    if (pairMxEntryRelEXIST(tirp.toncepts[tirp.size - 1], toncepts.Values.ElementAt(toncept).tonceptID, seedRelation))
                    {
                        if(trans)
                            candidates = CandidatesGeneration_Trans(tirp, seedRelation);
                        else
                            candidates = CandidatesGeneration_Naive(tirp, seedRelation);
                        for (int cand = 0; cand < candidates.Count; cand++)
                        {
                            TIRP tirpNew = new TIRP(tirp, toncepts.Values.ElementAt(toncept).tonceptID, seedRelation, candidates.ElementAt(cand));//, KLC.KarmaLego);
                            bool seedRelCnndtsEmpty = searchSupportingInstances(ref tirpNew, tirp.tinstancesList);
                            //if (seedRelCnndtsEmpty)
                            //    break;
                            if (tirpNew.verticalSupport >= min_ver_sup)
                            {
                                int[][] relDummi = null;
                                if (print == true)
                                    tirpNew.printTIRP(tw, entitiesVec, KLC.KarmaLego, relDummi); //print
                                Lego(tirpNew, tw);
                            }
                        }
                    }
            }
        }

        public void Karma(TonceptSet ds)
        {
            int index = 0;
            try
            {
                int[] relStat = new int[7];
                for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                {
                    for (int ti1Idx = 0; ti1Idx < ds.entities[eIdx].symbolic_intervals.Count; ti1Idx++)
                    {
/*                        TimeIntervalSymbol firsTis = ds.entities[eIdx].symbolic_intervals[ti1Idx];
                        addToncept( eIdx, firsTis, ref index, true); // update toncept entity support
                        //karma..
                        for (int ti2Idx = ti1Idx + 1; ti2Idx < ds.entities[eIdx].symbolic_intervals.Count; ti2Idx++)
                        {
                            TimeIntervalSymbol secondTis = ds.entities[eIdx].symbolic_intervals[ti2Idx];
                            if (firsTis.symbol != secondTis.symbol)
                            {
                                addToncept( eIdx, secondTis, ref index, false); // DONT update toncept entity support
                                int relation = KLC.WhichRelationEpsilon(firsTis, secondTis, relations_style, epsilon, max_gap);
                                if (relation > -1)
                                {
                                    relStat[relation]++;
                                    indexTimeInetervalSymbolsPair(firsTis, secondTis, relation, eIdx);
                                }
                                else
                                    break;
                            }
                        }
 */ 
                    }
                }
                pruneToncepts();
                pruneMatrixEntriesPairDics();
            }
            catch (Exception e) 
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
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
            bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
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
                            if (!KLC.checkRelationAmongTwoTIs(tinstances[tins].tis[relIdx - topRelIdx], tisList.ElementAt(i), tNew.rels[relIdx], epsilon, max_gap))
                                break;
                        if (relIdx == seedRelIdx)
                        {
                            TIsInstance newIns = new TIsInstance(tNew.size, tinstances[tins].tis, tisList.ElementAt(i), tinstances[tins].entityIdx); // D);
                            entitieSupport[newIns.entityIdx] = true;
                            tNew.tinstancesList.Add(newIns);
                        }
                    }
                }
            }
            if(tNew.tinstancesList.Count > 0 )
                for (int tins = 0; tins < entitieSize; tins++)
                    if (entitieSupport[tins] == true)
                        tNew.verticalSupport++;
            return seedRelEmpty;
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

        private void indexTimeInetervalSymbolsPair(TimeIntervalSymbol frstTIS, TimeIntervalSymbol scndTIS, int relation, int entityIdx)
        {
            int t1Idx = toncepts[frstTIS.symbol].tonceptINDEX;
            int t2Idx = toncepts[scndTIS.symbol].tonceptINDEX;
            if (twoSizedTIRPsMatrix[t1Idx][t2Idx] == null)
                twoSizedTIRPsMatrix[t1Idx][t2Idx] = new twoSizedTIRPsMATRIXEntry(relations_style);

            twoSizedTIRPsMATRIXEntry twoSizedTIRP_P = twoSizedTIRPsMatrix[t1Idx][t2Idx];
            pairsMATRIXrelEntry pMxRelE = twoSizedTIRP_P.prsMxRelVec[relation];
            if (pMxRelE == null)
                twoSizedTIRPsMatrix[t1Idx][t2Idx].prsMxRelVec[relation] = new pairsMATRIXrelEntry();
            dicTIentID tiEntID = new dicTIentID(frstTIS, entityIdx);
            if (twoSizedTIRPsMatrix[t1Idx][t2Idx].prsMxRelVec[relation].instancesDicList.ContainsKey(tiEntID))
                twoSizedTIRPsMatrix[t1Idx][t2Idx].prsMxRelVec[relation].instancesDicList[tiEntID].Add(scndTIS);
            else
            {
                List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                tisList.Add(scndTIS);
                twoSizedTIRPsMatrix[t1Idx][t2Idx].prsMxRelVec[relation].instancesDicList.Add(tiEntID, tisList);
            }
            twoSizedTIRPsMatrix[t1Idx][t2Idx].prsMxRelVec[relation].entitieSupport[entityIdx] = true; // ds.entities[e_idx].entityINDEX] = true;
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
                int nullMxEntryCount = 0;
                for (int t2 = 0; t2 < toncepts.Count; t2++)
                {
                    twoSizedTIRPsMATRIXEntry tsTME = twoSizedTIRPsMatrix[toncepts.Values.ElementAt(t1).tonceptINDEX][toncepts.Values.ElementAt(t2).tonceptINDEX];
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
                            twoSizedTIRPsMatrix[toncepts.Values.ElementAt(t1).tonceptINDEX][toncepts.Values.ElementAt(t2).tonceptINDEX] = null;
                            nullMxEntryCount++;
                        }
                    }
                    else
                        nullMxEntryCount++;
                }
                if (nullMxEntryCount == toncepts.Count)
                    twoSizedTIRPsMatrix[toncepts.Values.ElementAt(t1).tonceptINDEX] = null;
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
