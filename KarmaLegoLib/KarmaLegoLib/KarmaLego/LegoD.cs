using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class LegoD
    {
        //int karmalegologi;
        //bool print;
        //bool trans;
        //int relations_style;
        //int epsilon;
        //int max_gap;
        //double min_ver_sup;
        //int[][] logiRelsIndxs;
        KarmE karmaD;
        
        public LegoD(bool seTrans, bool setPrint, KarmE setKarmaD)
        {
            karmaD = setKarmaD;
            //karmalegologi = karmaD.getKarmaLegoLogi();
            //trans = seTrans;
            //print = setPrint;
            //relations_style = karmaD.getRelStyle();
            //epsilon = karmaD.getEpsilon();
            //max_gap = karmaD.getMaxGap();
            //min_ver_sup = karmaD.getMinVerSup();
            
            //logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
            //logiRelsIndxs[0/*3*/] = new int[3]  { 2, 1, 0 };
            //logiRelsIndxs[1/*4*/] = new int[6]  { 5, 4, 2, 3, 1, 0 };
            //logiRelsIndxs[2/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
            //logiRelsIndxs[3/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[4/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[5/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[6/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[7/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
        }

        public void runLego(int tTrgtIdx, int tTrgtID, string folderPath)
        {
            bool twoSizedTIRP = false;
            string filePath = folderPath + karmaD.getTonceptByID(tTrgtID).tonceptID + ".txt";
            TextWriter tw = new StreamWriter(filePath);
            tw.WriteLine("1 " + tTrgtID + "- " + karmaD.getTonceptByID(tTrgtID).getTonceptVerticalSupport() + " " + karmaD.getTonceptByID(tTrgtID).getTonceptVerticalSupport());
            foreach (TemporalConcept toncept in karmaD.getToncepts()) //.Values)
            //Parallel.ForEach(karma.getToncepts(), toncept => // toncepts, toncept =>
            {
                int tErlyID = toncept.tonceptID;
                for (int rel = 0; rel < karmaD.getRelStyle(); rel++) // relations_style; rel++)
                //Parallel.For(0, relations_style, rel =>
                {
                    int verSup = karmaD.getglblTindexVerticalSupport(karmaD.getTonceptByID(tTrgtID).tonceptINDEX, toncept.tonceptINDEX, rel);
                    if (verSup >= karmaD.getMinVerSup()) // min_ver_sup)
                    {
                        twoSizedTIRP = true;
                        TIRP twoSzdTIRP = karmaD.getTwoSizedTIRP(tTrgtID, tErlyID, rel);
                        string writeLine = "";
                        
                        if (karmaD.getPrint() > KLC.KL_PRINT_NO) // print == true)
                        {
                            string tonceptList = "";
                            if (karmaD.getForBackWardsMining() == KLC.backwardsMining) // karmalegologi == KLC.backwardsMining) // .KarmaLogi)
                                tonceptList = tErlyID + "-" + tTrgtID; // + "- " + KLC.ALLEN7_RELCHARS[rel] + ". " + twoSzdTIRP.tinstancesList.Count + " " + verSup;
                            else
                                tonceptList = tTrgtID + "-" + tErlyID; // + "- " + KLC.ALLEN7_RELCHARS[rel] + ". " + twoSzdTIRP.tinstancesList.Count + " " + verSup;
                            
                            //string filePath2 = folderPath + tonceptList + "-0-0-0-0-0-0-0-0-r" + rel + ".txt";
                            //TextWriter tw2 = new StreamWriter(filePath2);
                            tw.WriteLine("2 " + tonceptList + "- " + KLC.ALLEN7_RELCHARS[rel] + ". " + twoSzdTIRP.tinstancesList.Count + " " + verSup);
                            foreach (TIsInstance tIns in twoSzdTIRP.tinstancesList)
                            {
                                if (karmaD.getForBackWardsMining() == KLC.backwardsMining) // karmalegologi == KLC.backwardsMining) // .KarmaLogi)
                                    writeLine = writeLine + karmaD.getEntityKarmaByIdx(tIns.entityIdx).entityID + " [" + tIns.tis[1].startTime + "-" + tIns.tis[1].endTime + "][" + tIns.tis[0].startTime + "-" + tIns.tis[0].endTime + "] ";
                                else
                                    writeLine = writeLine + karmaD.getEntityKarmaByIdx(tIns.entityIdx).entityID + " [" + tIns.tis[0].startTime + "-" + tIns.tis[0].endTime + "][" + tIns.tis[1].startTime + "-" + tIns.tis[1].endTime + "] ";
                            }// consider to integrate this in the constructor of the TIRP, or copy it here.
                            tw.WriteLine(writeLine);
                            //tw2.Close();
                        }
                        LogiD(twoSzdTIRP, tw); // folderPath); // 
                    }
                }  // );
            }  // );
            tw.Close();
            if (twoSizedTIRP == false)
                File.Delete(filePath);
        }

        private void LogiD(TIRP tirp, TextWriter tw) // string folderPath)
        {
            List<int[]> candidates = null;
            foreach (TemporalConcept toncept in karmaD.getToncepts()) //.Values)
            //Parallel.ForEach(karma.getToncepts(), toncept => // toncepts, toncept =>
            {
                for (int seedRelation = 0; seedRelation < karmaD.getRelStyle(); seedRelation++) // relations_style; seedRelation++)
                    if (karmaD.getglblTindexVerticalSupport(karmaD.getTonceptByID(tirp.toncepts[tirp.size - 1]).tonceptINDEX, toncept.tonceptINDEX, seedRelation) > karmaD.getMinVerSup()) // min_ver_sup ) //pairMxEntryRelEXIST(tirp.toncepts[tirp.size - 1], toncept.tonceptID, seedRelation))
                    {
                        candidates = generateCandidates(tirp, seedRelation);
                        for (int cand = 0; cand < candidates.Count; cand++)
                        {   //consider creating tirpNew in searchSupportingInstances and to return it
                            TIRP tirpNew = new TIRP(tirp, toncept.tonceptID, seedRelation, candidates.ElementAt(cand));
                            bool seedRelCnndtsEmpty = searchSupportingInstances(ref tirpNew, tirp.tinstancesList);
                            if (tirpNew.entitieVerticalSupport.Count >= karmaD.getMinVerSup()) // min_ver_sup) // .verticalSupport >= min_ver_sup)
                            {
                                tirpNew.printTIRP(/*folderPath*/ tw, karmaD.getEntitiesKarmaVec(), karmaD.getForBackWardsMining(), karmaD.getBackwardsRelsIndxs(), karmaD.getPrint(), karmaD.getRelStyle()); // karmalegologi, logiRelsIndxs); //print
                                LogiD(tirpNew, tw); //folderPath); // 
                            }
                        }
                    }
            } //);
        }

        private bool searchSupportingInstances(ref TIRP tNew, List<TIsInstance> tinstances)
        {
            bool seedRelEmpty = true;
            //bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
            int topRelIdx = ((tNew.size - 1) * (tNew.size - 2) / 2), seedRelIdx = (tNew.size * (tNew.size - 1) / 2 - 1);
            int seedRelation = tNew.rels[seedRelIdx];
            int tncptLst = tNew.toncepts[tNew.size - 2], tncptNew = tNew.toncepts[tNew.size - 1];
            Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic;
            //tnctpsRelKey trK = new tnctpsRelKey(karmaD.getTonceptByID(tncptLst).tonceptINDEX, karmaD.getTonceptByID(tncptNew).tonceptINDEX, seedRelation);
            //string trK = karmaD.getTonceptByID(tncptLst).tonceptINDEX.ToString() + seedRelation + karmaD.getTonceptByID(tncptNew).tonceptINDEX;
            for (int tins = 0; tins < tinstances.Count; tins++)
            {
                if (karmaD.getEntityKarmaByIdx(tinstances[tins].entityIdx).instancesDicContainsKey(karmaD.getTonceptByID(tncptLst).tonceptINDEX, seedRelation, karmaD.getTonceptByID(tncptNew).tonceptINDEX)) // .instancesDic.ContainsKey(trK))
                {
                    tiListDic = karmaD.getEntityKarmaByIdx(tinstances[tins].entityIdx).getInstancesDicValuebyKey(karmaD.getTonceptByID(tncptLst).tonceptINDEX, seedRelation, karmaD.getTonceptByID(tncptNew).tonceptINDEX); // .instancesDic[trK];
                    if (tiListDic.ContainsKey(tinstances[tins].tis[tNew.size - 2]))
                    {
                        seedRelEmpty = false;
                        List<TimeIntervalSymbol> tisList = tiListDic[tinstances[tins].tis[tNew.size - 2]];
                        for (int i = 0; i < tisList.Count; i++)
                        {
                            int relIdx = 0;
                            for (relIdx = topRelIdx; relIdx < seedRelIdx; relIdx++)
                                if (!checkRelationAmongTwoTIs(tinstances[tins].tis[relIdx - topRelIdx], tisList.ElementAt(i), tNew.rels[relIdx]))//if (!KLC.checkRelationAmongTwoTIs(tinstances[tins].tis[relIdx - topRelIdx], tisList.ElementAt(i), tNew.rels[relIdx], epsilon, max_gap))
                                    break;
                            if (relIdx == seedRelIdx)
                            {
                                TIsInstance newIns = new TIsInstance(tNew.size, tinstances[tins].tis, tisList.ElementAt(i), tinstances[tins].entityIdx); // D);
                                tNew.addEntity(newIns.entityIdx); // entitieSupport[newIns.entityIdx] = true;
                                tNew.tinstancesList.Add(newIns);
                            }
                        }
                    }
                }
            }
            /*if (tNew.tinstancesList.Count > 0)
                for (int tins = 0; tins < karmaD.getEntitieSize(); tins++)
                    if (entitieSupport[tins] == true)
                        tNew.verticalSupport++;*/
            return seedRelEmpty;
        }

        private bool checkRelationAmongTwoTIs(TimeIntervalSymbol tinstanceTIS, TimeIntervalSymbol listTIS, int relation)
        {
            if (karmaD.getForBackWardsMining() == KLC.forwardMining) // karmalegologi == KLC .forwardMining) //.KarmaLego)
                return KLC.checkRelationAmongTwoTIs(tinstanceTIS, listTIS, relation, karmaD.getEpsilon(), karmaD.getMaxGap()); // epsilon, max_gap);
            else
                return KLC.checkRelationAmongTwoTIs(listTIS, tinstanceTIS, relation, karmaD.getEpsilon(), karmaD.getMaxGap()); // epsilon, max_gap);
        }

        private List<int[]> generateCandidates(TIRP tirp, int seedRel)
        {
            if (karmaD.getTrans()) // trans)
                return CandidatesGeneration_Trans(tirp, seedRel);
            else
                return CandidatesGeneration_Naive(tirp, seedRel);
        }

        private List<int[]> CandidatesGeneration_Naive(TIRP tirp, int seedRel)
        {
            int columSize = tirp.size; // this is the size of the extended TIRP (the generated TIRP is tirp.size+1)
            int relCombs = (int)Math.Pow(karmaD.getRelStyle(), (columSize - 1)); // relations_style, (columSize - 1));
            List<int[]> candidatesList = new List<int[]>();
            for (int comb = 0; comb < relCombs; comb++)
            {
                int[] candidate = new int[columSize];
                candidate[columSize - 1] = seedRel;
                for (int relIdx = 0; relIdx < (columSize - 1); relIdx++)
                    candidate[relIdx] = (comb / (int)Math.Pow(karmaD.getRelStyle(), relIdx)) % karmaD.getRelStyle(); // relations_style, relIdx)) % relations_style;
                candidatesList.Add(candidate);
            }
            return candidatesList;
        }

        private List<int[]> CandidatesGeneration_Trans(TIRP tirp, int seedRel)
        {
            int columSize = tirp.size; // this is the size of the extended TIRP (the generated TIRP is tirp.size+1)
            int topCndRelIdx = 0;
            int btmRelIdx = columSize - 2;
            List<int[]> candidatesList = new List<int[]>();
            int[] candidate = new int[columSize]; // Remember: the candidate relIdx is 0 -> 0, 1 -> 2, 3 - > 5,..
            candidate[columSize - 1] = seedRel;         // and in each size of the tirp it is the last column
            candidatesList.Add(candidate);
            for (int relIdxToSet = btmRelIdx; relIdxToSet >= topCndRelIdx; relIdxToSet--)
            {
                int leftTirpIdx = ((relIdxToSet + 1) * relIdxToSet / 2) + relIdxToSet;
                int belowCndIdx = relIdxToSet + 1;
                int candListSize = candidatesList.Count;
                for (int candIdx = 0; candIdx < candListSize; candIdx++)
                {
                    candidate = candidatesList.ElementAt(candIdx);
                    int fromRel = karmaD.getFromRelation(tirp.rels[leftTirpIdx], candidate[belowCndIdx]);
                    int toRel = karmaD.getToRelation(tirp.rels[leftTirpIdx], candidate[belowCndIdx]);
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

        private void transCandidates(TIRP tirp, int relIdxToSet, ref List<int[]> candidatesList, int relsToGenerate)
        {
            if (relIdxToSet < 0)
                return;
            int leftIdx = ((relIdxToSet + 1) * relIdxToSet / 2) + relIdxToSet;
            int belowIdx = relIdxToSet + 1;
            for (int candIdx = 0; candIdx < candidatesList.Count; candIdx++)
            {
                int[] candidate = candidatesList.ElementAt(candIdx);
                int fromRel = karmaD.getToRelation(tirp.rels[leftIdx], candidate[belowIdx]);
                int toRel = karmaD.getToRelation(tirp.rels[leftIdx], candidate[belowIdx]);
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
    }
}
