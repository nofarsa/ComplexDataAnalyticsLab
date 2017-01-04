using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;



namespace KarmaLegoLib
{
    public class DharmaLego
    {
        public static void runTimeExperiment()
        {
            string dir = "D:\\M11\\new_cumc_sources\\procedures_cosinebased_ceBased_sourceFiles_oT365_pT30_mE0_minToE30_abstract\\KDDruntime"; //"C:\\new_cumc_sources\\procedures_cosinebased_ceBased_sourceFiles_oT365_pT30_mE0_minToE30_abstract\\KDDruntime";
            string mainDir = dir + "\\mainDir\\";
            string dataset = dir + "\\2002174_oT365_pT30_mE0_mTE30_1406_71.70982_72_OFILE.csv"; //"\\2002685_oT365_pT30_mE0_mTE30_511_75.78082_76_OFILE.csv"; //"\\2008285_oT365_pT30_mE0_mTE30_2147_61.64322_62_OFILE.csv"; // "\\2006933_oT365_pT30_mE0_mTE30_1009_77.42418_78_OFILE.csv"; //  // "\\  "\\ "\\2008285_oT365_pT30_mE0_mTE30_2147_61.64322_62_OFILE.csv"; // 
            int maxGap = 210;
            int maxTIRPSize = 6;
            int epsilon = 0;
            bool paralleling = KLC.nonParallelMining;
            int relStyle = KLC.RELSTYLE_ALLEN7;
            bool setFullDics = false;
            string runtimeReport = "runTime,RelVecSymVec,FullyHashed,RelVec,\n";

            for (int minSup = 20; minSup > 0; minSup = minSup - 5)
            {
                System.Threading.Thread.Sleep(10000);
                runtimeReport += runKarmalego(setFullDics, -1, 0, false, mainDir, dataset, 0, false, minSup, 0, maxGap, maxTIRPSize, epsilon, paralleling, relStyle, 10000);
            }

            int DharmaIndexType = KLC.dharma_relVecSymVecSymDic; // KLC.dharma_relSymSymDics; // KLC.dharma_relVecSymSymDics; ; //  
            for (int minSup = 20; minSup > 0; minSup = minSup - 5)
            {
                System.Threading.Thread.Sleep(10000);
                runtimeReport += runDharmalego(setFullDics, DharmaIndexType, 0, false, mainDir, dataset, 0, false, minSup, 0, maxGap, maxTIRPSize, epsilon, paralleling, relStyle, 10000);
            }
            //CUMC_Handler.writeToFile(mainDir + "runTime_1406_" + DharmaIndexType + "_" + setFullDics + ".csv", runtimeReport);

            DharmaIndexType = KLC.dharma_relSymSymDics; // KLC.dharma_relVecSymSymDics; ; //  
            for (int minSup = 20; minSup > 0; minSup = minSup - 5)
            {
                System.Threading.Thread.Sleep(10000);
                runtimeReport += runDharmalego(setFullDics, DharmaIndexType, 0, false, mainDir, dataset, 0, false, minSup, 0, maxGap, maxTIRPSize, epsilon, paralleling, relStyle, 10000);
            }
            //CUMC_Handler.writeToFile(mainDir + "runTime_1406_" + DharmaIndexType + "_" + setFullDics + ".csv", runtimeReport);

            DharmaIndexType = KLC.dharma_relVecSymSymDics;  
            for (int minSup = 20; minSup > 0; minSup = minSup - 5)
            {
                System.Threading.Thread.Sleep(10000);
                runtimeReport += runDharmalego(setFullDics, DharmaIndexType, 0, false, mainDir, dataset, 0, false, minSup, 0, maxGap, maxTIRPSize, epsilon, paralleling, relStyle, 10000);
            }

            CUMC_Handler.writeToFile(mainDir + "runTime_1009_" + DharmaIndexType + "_" + setFullDics + ".csv", runtimeReport);
            
        }

        private static string runKarmalego(bool setFullDics, int setDharmaIndexType, int foldID, bool kl, string mainDir, string dataset, int tonceptID, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int maxTIRPSize, int epsilon, bool paralleling, int relStyle, int eLast, int Efrst = 0, bool setHS1 = false, int print = KLC.KL_PRINT_TONCEPTANDTIRPS)
        {

            var before = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;

            string dharmaTime = "";
            string tempDir = mainDir + "tempDir_" + tonceptID + "_" + foldID + "_" + DateTime.Now.Hour + DateTime.Now.Minute + "\\";
            Karma da = new Karma(true, KLC.backwardsMining, KLC.RELSTYLE_ALLEN7, epsilon, maxGap, minVerSup, dataset, KLC.KL_TRANS_YES, setHS1, print, ref dharmaTime, eLast); //entitieSizeLimit);
            //da.setDharmaIndexType(setDharmaIndexType);
            //entitieSizeLimit = ka.getEntitieSize();
            dataset = mainDir + dataset.Split('\\')[dataset.Split('\\').Length - 1].Replace(".csv", "") + foldID + "_ms_" + minVerSup + "_mhs" + minHrzSup + "_mg_" + maxGap + "_mxTrpSze_" + maxTIRPSize + "_e_" + epsilon + "_r_" + relStyle + "_dit_" + setDharmaIndexType + "_onlyTncpt_KarmEL.txt";
            //LegoForDharma legoDharma = new LegoForDharma(da);
            double legoTime = double.Parse(Lego.RunLegoALL(da, tempDir, dataset, da.getTrans()));
            var after = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            string runTime = minVerSup.ToString() + "," + setDharmaIndexType + "," + before + "," + after + "," + dharmaTime + "," + legoTime + "," + (double.Parse(dharmaTime) + legoTime).ToString() + ",\n";//, KLC.KL_TRANS_YES); // string karmelTime = ELIsrael.RunEL(kE, tempDir, dataset, onlyBEFORE, null);
            da = null;
            //legoDharma = null;
            return runTime;
            //ka = null;
        }

        private static string runDharmalego(bool setFullDics, int setDharmaIndexType, int foldID, bool kl, string mainDir, string dataset, int tonceptID, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int maxTIRPSize, int epsilon, bool paralleling, int relStyle, int eLast, int Efrst = 0, bool setHS1 = false, int print = KLC.KL_PRINT_TONCEPTANDTIRPS)
        {
            
            var before = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            
            string dharmaTime = "";
            string tempDir = mainDir + "tempDir_" + tonceptID + "_" + foldID + "_" + DateTime.Now.Hour + DateTime.Now.Minute + "\\";
            Dharma da = new Dharma(maxTIRPSize, setFullDics, setDharmaIndexType, true, KLC.backwardsMining, KLC.RELSTYLE_ALLEN7, epsilon, maxGap, minVerSup, dataset, KLC.KL_TRANS_YES, setHS1, print, ref dharmaTime, eLast); //entitieSizeLimit);
            da.setDharmaIndexType(setDharmaIndexType);
            //entitieSizeLimit = ka.getEntitieSize();
            dataset = mainDir + dataset.Split('\\')[dataset.Split('\\').Length - 1].Replace(".csv", "") + foldID + "_ms_" + minVerSup + "_mhs" + minHrzSup + "_mg_" + maxGap + "_mxTrpSze_" + maxTIRPSize + "_e_" + epsilon + "_r_" + relStyle + "_dit_" + setDharmaIndexType + "_onlyTncpt_KarmEL.txt";
            LegoForDharma legoDharma = new LegoForDharma(da);
            double legoTime = double.Parse(legoDharma.RunLegoALL(tempDir, dataset));
            var after = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            string runTime = minVerSup.ToString() + "," + setDharmaIndexType + "," + before + "," + after + "," + dharmaTime + "," + legoTime + "," + (double.Parse(dharmaTime) + legoTime).ToString() + ",\n";//, KLC.KL_TRANS_YES); // string karmelTime = ELIsrael.RunEL(kE, tempDir, dataset, onlyBEFORE, null);
            da = null;
            legoDharma = null;
            return runTime;
            //ka = null;
        }
    }

    public class LegoForDharma
    {
        Dharma dharma;
        
        public LegoForDharma(Dharma setDharma)
        {
            dharma = setDharma;
        }

        public /*static*/ string RunLegoALL(/*Dharma k,*/ string outFolder, string outFile)//, bool seTrans) //, string folderName)
        {
            Directory.CreateDirectory(outFolder);
            //dharma.setTrans(seTrans);
            DateTime starTime = DateTime.Now;
            foreach (TemporalConcept toncept1 in dharma.getToncepts())
            {
                if (dharma.getTonceptByIDVerticalSupport(toncept1.tonceptID) > dharma.getMinVerSup())
                {
                    int tTrgtID  = toncept1.tonceptID;
                    int t1Idx = toncept1.tonceptINDEX;
                    string filePath = outFolder + tTrgtID + ".txt";
                    int freqNum = 0;
                    TextWriter tw = null;
                    if (dharma.getPrint() > KLC.KL_PRINT_NO)
                    {
                        tw = new StreamWriter(filePath);
                        string instances = " ";
                        for (int i = 0; i < dharma.getTonceptByIDVerticalSupport(tTrgtID); i++)
                        {
                            KeyValuePair<int, List<TimeIntervalSymbol>> userInstances = dharma.getTonceptByID(tTrgtID).getTonceptHorizontalDic().ElementAt(i);
                            for (int j = 0; j < userInstances.Value.Count; j++)
                                instances += dharma.getEntityByIdx(userInstances.Key) + " [" + userInstances.Value.ElementAt(j).startTime + "-" + userInstances.Value.ElementAt(j).endTime + "] ";
                        }
                        tw.WriteLine("1 " + tTrgtID + "- -. " + dharma.getTonceptByIDVerticalSupport(tTrgtID) + " " + dharma.getTonceptByIDVerticalSupport(tTrgtID) + instances);
                    }
                    ////if (k.MatrixFirstIndexNotNull(t1Idx))
                    ////{
                    foreach (TemporalConcept toncept2 in dharma.getToncepts())
                    {
                      ////      if (k.MatrixEntryNotNull(t1Idx, toncept2.tonceptINDEX))
                        for (int rel = 0; rel < dharma.getRelStyle(); rel++)
                        {
                            //if(karma.getTindexRelVerticalSupport(t1Idx, toncept2.tonceptINDEX, rel) > dharma.getMinVerSup()) // if (k.getMatrixVerticalSupport(t1Idx, toncept2.tonceptINDEX, rel) > k.getMinVerSup())
                            if( dharma.get2SizedVerticalSupport(t1Idx, toncept2.tonceptINDEX, rel) > dharma.getMinVerSup())
                            {
                                freqNum++;
                                TIRP twoSzdTIRP = dharma.get2SizedAsTIRP(tTrgtID, toncept2.tonceptID, rel); //karma.getMatrixDICasTIRP(tTrgtID, toncept2.tonceptID, rel);
                                LegoForDharma lego = new LegoForDharma(dharma); //Lego lego = new Lego(k);
                                if (dharma.getPrint() > KLC.KL_PRINT_NO)
                                    twoSzdTIRP.printTIRP(tw, dharma.getEntitiesVec(), dharma.getForBackWardsMining(), dharma.getBackwardsRelsIndxs(), dharma.getRelStyle()); // lego.logiRelsIndxs);
                                lego.doLego(twoSzdTIRP, tw);
                                lego = null;
                                }
                            }
                        }
                    //}
                    if (dharma.getPrint() > KLC.KL_PRINT_NO)
                        tw.Close();
                    //if (freqNum == 0)
                    //    File.Delete(filePath);
                }
            }
            if (dharma.getPrint() > KLC.KL_PRINT_NO)
                KarmE.WriteKLFileFromTonceptsFiles(outFile, outFolder);
            DateTime endTime = DateTime.Now;
            string runTime = dharma.getMxAccsCounter() + "," + ((endTime - starTime).TotalMilliseconds);
            return runTime;
        }

        private void doLego(TIRP tirp, TextWriter tw)
        {
            int tr = 0;
            //if (karma.MatrixFirstIndexNotNull(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1])))
            //{
                List<int[]> candidates = null;
                foreach (TemporalConcept toncept in dharma.getToncepts())
                    //if (karma.MatrixEntryNotNull(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX))
                        for (int seedRelation = 0; seedRelation < dharma.getRelStyle(); seedRelation++)
                            ////if (karma.getMatrixVerticalSupport(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX, seedRelation) > karma.getMinVerSup())
                            //if (karma.getTindexRelVerticalSupport(dharma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX, seedRelation) > dharma.getMinVerSup())
                            
                            // CHECK THIS - WHAT IS PASSED HERE, is it INDEX or ID ???
                            if (dharma.get2SizedVerticalSupport(dharma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]), toncept.tonceptINDEX, seedRelation) > dharma.getMinVerSup())
                            // CHECK THIS - WHAT IS PASSED HERE, is it INDEX or ID ???
                            
                            {
                                candidates = generateCandidates(tirp, seedRelation);
                                for (int cand = 0; cand < candidates.Count; cand++)
                                {   //consider creating tirpNew in searchSupportingInstances and to return it
                                    TIRP tirpNew = new TIRP(tirp, toncept.tonceptID, seedRelation, candidates.ElementAt(cand));
                                    bool seedRelCnndtsEmpty = searchSupportingInstances(ref tirpNew, tirp.tinstancesList);
                                    if (tirpNew.entitieVerticalSupport.Count >= dharma.getMinVerSup() && tirpNew.size < dharma.getMaxTirpSize())
                                    {
                                        if (dharma.getPrint() > KLC.KL_PRINT_NO)
                                            tirpNew.printTIRP(tw, dharma.getEntitiesVec(), dharma.getForBackWardsMining(), dharma.getBackwardsRelsIndxs(), dharma.getRelStyle()); // logiRelsIndxs); //print
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

            int lsTncptIdx = dharma.getTonceptIndexByID(tncptLst);
            int nxTncptIdx = dharma.getTonceptIndexByID(tncptNew);
            for (int tins = 0; tins < tinstances.Count; tins++)
            {
                ////if (karma.getHS1() == false || (karma.getHS1() == true && !tNew.entitieSupport.Contains(tinstances[tins].entityIdx)))
                ////{
                    ////string TISearch = tinstances[tins].entityIdx + "-" + tinstances[tins].tis[tNew.size - 2].symbol + "-" + tinstances[tins].tis[tNew.size - 2].startTime + "-" + tinstances[tins].tis[tNew.size - 2].endTime;
                    ////if (karma.MatrixRelContainsKey(lsTncptIdx, nxTncptIdx, seedRelation, TISearch))
                    {
                        seedRelEmpty = false;
                        //List<TimeIntervalSymbol> tisList = karma.getTindexRelEidxTisList(lsTncptIdx, nxTncptIdx, seedRelation, tinstances[tins].entityIdx, tinstances[tins].tis[tNew.size - 2]);  //karma.MatrixRelGetKey(lsTncptIdx, nxTncptIdx, seedRelation, TISearch);
                        List<TimeIntervalSymbol> tisList = dharma.get2SizedEntSTIListOfSTIs(lsTncptIdx, nxTncptIdx, seedRelation, tinstances[tins].entityIdx, tinstances[tins].tis[tNew.size - 2]);
                        if (tisList != null)
                        {
                            for (int i = 0; (dharma.getHS1() == false && i < tisList.Count) || (dharma.getHS1() == true && i < 1); i++)
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
            if (dharma.getForBackWardsMining() == KLC.forwardMining)
                return KLC.checkRelationAmongTwoTIs(tinstanceTIS, listTIS, relation, dharma.getEpsilon(), dharma.getMaxGap());
            else
                return KLC.checkRelationAmongTwoTIs(listTIS, tinstanceTIS, relation, dharma.getEpsilon(), dharma.getMaxGap());
        }

        private List<int[]> generateCandidates(TIRP tirp, int seedRel)
        {
            if (dharma.getTrans())
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
                    int fromRel = dharma.getFromRelation(tirp.rels[leftTirpIdx], candidate[belowCndIdx]);
                    int toRel =   dharma.getToRelation( tirp.rels[leftTirpIdx], candidate[belowCndIdx]);
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
            int relCombs = (int)Math.Pow(dharma.getRelStyle(), (columSize - 1));
            List<int[]> candidatesList = new List<int[]>();
            for (int comb = 0; comb < relCombs; comb++)
            {
                int[] candidate = new int[columSize];
                candidate[columSize - 1] = seedRel;
                for (int relIdx = 0; relIdx < (columSize - 1); relIdx++)
                    candidate[relIdx] = (comb / (int)Math.Pow(dharma.getRelStyle(), relIdx)) % dharma.getRelStyle();
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
                int fromRel = dharma.getToRelation(tirp.rels[leftIdx], candidate[belowIdx]);
                int toRel   = dharma.getToRelation(tirp.rels[leftIdx], candidate[belowIdx]);
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
