using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace KarmaLegoLib
{
    public class Lego
    {
        //int[][] logiRelsIndxs;
        Karma karma;
        
        public Lego(Karma setKarma)
        {
            karma = setKarma;
            
            /*logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
            //logiRelsIndxs[0/*3*///] = new int[3]  { 2, 1, 0 };
            //logiRelsIndxs[1/*4*/] = new int[6]  { 5, 4, 2, 3, 1, 0 };
            //logiRelsIndxs[2/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
            //logiRelsIndxs[3/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[4/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[5/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[6/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
            //logiRelsIndxs[7/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
            //*/
        }

        public static string RunLegoALL(Karma k, string outFolder, string outFile, bool seTrans) //, string folderName)
        {
            Directory.CreateDirectory(outFolder);
            k.setTrans(seTrans);
            DateTime starTime = DateTime.Now;
            foreach (TemporalConcept toncept1 in k.getToncepts())
            {
                if (k.getTonceptByIDVerticalSupport(toncept1.tonceptID) > k.getMinVerSup())
                {
                    int tTrgtID  = toncept1.tonceptID;
                    int t1Idx = toncept1.tonceptINDEX;
                    string filePath = outFolder + tTrgtID + ".txt";
                    int freqNum = 0;
                    TextWriter tw = null;
                    if (k.getPrint() > KLC.KL_PRINT_NO)
                    {
                        tw = new StreamWriter(filePath);
                        string instances = " ";
                        for (int i = 0; i < k.getTonceptByIDVerticalSupport(tTrgtID); i++)
                        {
                            KeyValuePair<int, List<TimeIntervalSymbol>> userInstances = k.getTonceptByID(tTrgtID).getTonceptHorizontalDic().ElementAt(i);
                            for (int j = 0; j < userInstances.Value.Count; j++)
                                instances += k.getEntityByIdx(userInstances.Key) + " [" + userInstances.Value.ElementAt(j).startTime + "-" + userInstances.Value.ElementAt(j).endTime + "] ";
                        }
                        tw.WriteLine("1 " + tTrgtID + "- -. " + k.getTonceptByIDVerticalSupport(tTrgtID) + " " + k.getTonceptByIDVerticalSupport(tTrgtID) + instances);
                    }
                    //if (k.MatrixFirstIndexNotNull(t1Idx))
                    //{
                        foreach (TemporalConcept toncept2 in k.getToncepts())
                        {
                      //      if (k.MatrixEntryNotNull(t1Idx, toncept2.tonceptINDEX))
                                for (int rel = 0; rel < k.getRelStyle(); rel++)
                                {
                                    if(k.karma.getTindexRelVerticalSupport(t1Idx, toncept2.tonceptINDEX, rel) > k.getMinVerSup()) // if (k.getMatrixVerticalSupport(t1Idx, toncept2.tonceptINDEX, rel) > k.getMinVerSup())
                                    {
                                        freqNum++;
                                        TIRP twoSzdTIRP = k.getMatrixDICasTIRP(tTrgtID, toncept2.tonceptID, rel);
                                        Lego lego = new Lego(k);
                                        if (k.getPrint() > KLC.KL_PRINT_NO)
                                            twoSzdTIRP.printTIRP(tw, k.getEntitiesVec(), k.getForBackWardsMining(), k.getBackwardsRelsIndxs(), k.getRelStyle()); // lego.logiRelsIndxs);
                                        lego.doLego(twoSzdTIRP, tw);
                                        lego = null;
                                    }
                                }
                        }
                    //}
                    if (k.getPrint() > KLC.KL_PRINT_NO)
                        tw.Close();
                    //if (freqNum == 0)
                    //    File.Delete(filePath);
                }
            }
            if (k.getPrint() > KLC.KL_PRINT_NO)
                KarmE.WriteKLFileFromTonceptsFiles(outFile, outFolder);
            //    using (StreamWriter sw = new StreamWriter(outFile))
            //        foreach (string txtName in Directory.GetFiles(outFolder))
            //            using (StreamReader sr = new StreamReader(txtName))
            //                sw.Write(sr.ReadToEnd());
            DateTime endTime = DateTime.Now;
            string runTime = k.getMxAccsCounter() + "," + ((endTime - starTime).TotalMilliseconds);
            return runTime;
        }

        private void doLego(TIRP tirp, TextWriter tw)
        {
            int tr = 0;
            //if (karma.MatrixFirstIndexNotNull(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1])))
            //{
                List<int[]> candidates = null;
                foreach (TemporalConcept toncept in karma.getToncepts())
                    //if (karma.MatrixEntryNotNull(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX))
                        for (int seedRelation = 0; seedRelation < karma.getRelStyle(); seedRelation++)
                            //if (karma.getMatrixVerticalSupport(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX, seedRelation) > karma.getMinVerSup())
                            if (karma.karma.getTindexRelVerticalSupport(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX, seedRelation) > karma.getMinVerSup())
                            {
                                candidates = generateCandidates(tirp, seedRelation);
                                for (int cand = 0; cand < candidates.Count; cand++)
                                {   //consider creating tirpNew in searchSupportingInstances and to return it
                                    TIRP tirpNew = new TIRP(tirp, toncept.tonceptID, seedRelation, candidates.ElementAt(cand));
                                    bool seedRelCnndtsEmpty = searchSupportingInstances(ref tirpNew, tirp.tinstancesList);
                                    if (tirpNew.entitieVerticalSupport.Count >= karma.getMinVerSup())
                                    {
                                        if (karma.getPrint() > KLC.KL_PRINT_NO)
                                            tirpNew.printTIRP(tw, karma.getEntitiesVec(), karma.getForBackWardsMining(), karma.getBackwardsRelsIndxs(), karma.getRelStyle()); // logiRelsIndxs); //print
                                        doLego(tirpNew, tw);
                                    }
                                }
                            }
            //}
        }

        private bool searchSupportingInstances(ref TIRP tNew, List<TIsInstance> tinstances)
        {
            bool seedRelEmpty = true;
            bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
            int topRelIdx = ((tNew.size - 1) * (tNew.size - 2) / 2), seedRelIdx = (tNew.size * (tNew.size - 1) / 2 - 1);
            int seedRelation = tNew.rels[seedRelIdx];
            int tncptLst = tNew.toncepts[tNew.size - 2], tncptNew = tNew.toncepts[tNew.size - 1];

            int lsTncptIdx = karma.getTonceptIndexByID(tncptLst);
            int nxTncptIdx = karma.getTonceptIndexByID(tncptNew);
            for (int tins = 0; tins < tinstances.Count; tins++)
            {
                //if (karma.getHS1() == false || (karma.getHS1() == true && !tNew.entitieSupport.Contains(tinstances[tins].entityIdx)))
                //{
                    //string TISearch = tinstances[tins].entityIdx + "-" + tinstances[tins].tis[tNew.size - 2].symbol + "-" + tinstances[tins].tis[tNew.size - 2].startTime + "-" + tinstances[tins].tis[tNew.size - 2].endTime;
                    //if (karma.MatrixRelContainsKey(lsTncptIdx, nxTncptIdx, seedRelation, TISearch))
                    {
                        seedRelEmpty = false;
                        List<TimeIntervalSymbol> tisList = karma.karma.getTindexRelEidxTisList(lsTncptIdx, nxTncptIdx, seedRelation, tinstances[tins].entityIdx, tinstances[tins].tis[tNew.size - 2]);  //karma.MatrixRelGetKey(lsTncptIdx, nxTncptIdx, seedRelation, TISearch);
                        if (tisList != null)
                        {
                            for (int i = 0; (karma.getHS1() == false && i < tisList.Count) || (karma.getHS1() == true && i < 1); i++)
                            {
                                int relIdx = 0;
                                for (relIdx = topRelIdx; relIdx < seedRelIdx; relIdx++)
                                    if (!checkRelationAmongTwoTIs(tinstances[tins].tis[relIdx - topRelIdx], tisList.ElementAt(i), tNew.rels[relIdx]))
                                        break;
                                if (relIdx == seedRelIdx)
                                {
                                    TIsInstance newIns = new TIsInstance(tNew.size, tinstances[tins].tis, tisList.ElementAt(i), tinstances[tins].entityIdx);
                                    tNew.addEntity(newIns.entityIdx);
                                    tNew.tinstancesList.Add(newIns);
                                }
                            }
                        }
                    }
                //}
            }
            return seedRelEmpty;
        }

        private bool checkRelationAmongTwoTIs(TimeIntervalSymbol tinstanceTIS, TimeIntervalSymbol listTIS, int relation)
        {
            if (karma.getForBackWardsMining() == KLC.forwardMining)
                return KLC.checkRelationAmongTwoTIs(tinstanceTIS, listTIS, relation, karma.getEpsilon(), karma.getMaxGap());
            else
                return KLC.checkRelationAmongTwoTIs(listTIS, tinstanceTIS, relation, karma.getEpsilon(), karma.getMaxGap());
        }

        private List<int[]> generateCandidates(TIRP tirp, int seedRel)
        {
            if (karma.getTrans())
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
                    int fromRel = karma.getFromRelation(tirp.rels[leftTirpIdx], candidate[belowCndIdx]);
                    int toRel =   karma.getToRelation( tirp.rels[leftTirpIdx], candidate[belowCndIdx]);
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
            int relCombs = (int)Math.Pow(karma.getRelStyle(), (columSize - 1));
            List<int[]> candidatesList = new List<int[]>();
            for (int comb = 0; comb < relCombs; comb++)
            {
                int[] candidate = new int[columSize];
                candidate[columSize - 1] = seedRel;
                for (int relIdx = 0; relIdx < (columSize - 1); relIdx++)
                    candidate[relIdx] = (comb / (int)Math.Pow(karma.getRelStyle(), relIdx)) % karma.getRelStyle();
                candidatesList.Add(candidate);
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
                int fromRel = karma.getToRelation(tirp.rels[leftIdx], candidate[belowIdx]);
                int toRel   = karma.getToRelation(tirp.rels[leftIdx], candidate[belowIdx]);
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
