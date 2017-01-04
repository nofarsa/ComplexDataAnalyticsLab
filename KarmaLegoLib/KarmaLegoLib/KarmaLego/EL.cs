using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class EL
    {
        KarmE karmE;
        
        public EL(KarmE setKarmE)
        {
            karmE = setKarmE;
        }

        public static string RunEl(KarmE k, string outFolder, string outFile, TemporalConcept toncept = null)
        {
            Directory.CreateDirectory(outFolder);
            DateTime starTime = DateTime.Now;
            //Parallel.ForEach(toncepts, toncept =>
            foreach (TemporalConcept toncept1 in k.getToncepts())
            {
                if (k.getTonceptByIDVerticalSupport(toncept1.tonceptID) > k.getMinVerSup() )
                {
                    int tTrgtID  = toncept1.tonceptID;
                    int tTrgtIdx = toncept1.tonceptINDEX;
                    if (toncept != null)
                    {
                        tTrgtID  = toncept.tonceptID;
                        tTrgtIdx = toncept.tonceptINDEX;
                    }
                    int freqNum = 0;
                    string filePath = outFolder + tTrgtID + ".txt";
                    TextWriter tw = new StreamWriter(filePath);
                    tw.WriteLine("1 " + tTrgtID + "- " + k.getTonceptByIDVerticalSupport(tTrgtID) + " " + k.getTonceptByIDVerticalSupport(tTrgtID));
                    //if (k.MatrixFirstIndexNotNull(tTrgtIdx))
                    foreach (TemporalConcept toncept2 in k.getToncepts())
                    //Parallel.ForEach(karma.getToncepts(), toncept => // toncepts, toncept => 
                    {
                        int tErlyID = toncept2.tonceptID;
                        if (k.getTonceptByIDVerticalSupport(toncept2.tonceptID) > k.getMinVerSup()) //if (k.MatrixEntryNotNull(tTrgtIdx, toncept2.tonceptINDEX))
                        {
                            for (int rel = 0; rel < k.getRelStyle(); rel++)
                            //Parallel.For(0, relations_style, rel =>
                            {
                                int verSup = k.getglblTindexVerticalSupport(tTrgtIdx, toncept2.tonceptINDEX, rel); 
                                //int verSup = k.getMatrixVerticalSupport(tTrgtIdx, toncept2.tonceptINDEX, rel);
                                if (verSup > k.getMinVerSup())
                                {
                                    freqNum++;
                                    TIRP twoSzdTIRP = k.getMatrixDICasTIRP(tTrgtID, tErlyID, rel);
                                    EL el = new EL(k);
                                    if (k.getPrint() > KLC.KL_PRINT_NO)
                                        twoSzdTIRP.printTIRP(tw, k.getEntitiesKarmaVec(), k.getForBackWardsMining(), k.getBackwardsRelsIndxs(), k.getPrint(),k.getRelStyle());
                                    el.Leven(twoSzdTIRP, tw);
                                }
                            }  // );
                        }
                    } // );
                    tw.Close();
                    if (freqNum == 0)
                        File.Delete(filePath);
                    if (toncept != null)
                        break;
                }
            } //);
            if (k.getPrint() > KLC.KL_PRINT_NO)
                KarmE.WriteKLFileFromTonceptsFiles(outFile, outFolder);
            DateTime endTime = DateTime.Now;
            string runTime = "111" + "," + ((endTime - starTime).TotalMilliseconds);
            return runTime;
        }

        private void Leven(TIRP tirp, TextWriter tw)
        {
            if (karmE.getTonceptByIDVerticalSupport(tirp.toncepts[tirp.size - 1]) > karmE.getMinVerSup()) // .MatrixFirstIndexNotNull(karmE.getTonceptIndexByID(tirp.toncepts[tirp.size - 1])))
            {
                int lsTncptIdx = karmE.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]);
                Dictionary<string, TIRP> cnddTirpsDic = null;
                foreach (TemporalConcept toncept in karmE.getToncepts())
                //Parallel.ForEach(karma.getToncepts(), toncept => // toncepts, toncept =>
                {
                    //if (karmE.MatrixEntryNotNull(lsTncptIdx, toncept.tonceptINDEX))
                        for (int seedRelation = 0; seedRelation < karmE.getRelStyle(); seedRelation++)
                            if (karmE.getglblTindexVerticalSupport(lsTncptIdx, toncept.tonceptINDEX, seedRelation) > karmE.getMinVerSup()) // .getMatrixVerticalSupport(lsTncptIdx, toncept.tonceptINDEX, seedRelation) > karma.getMinVerSup())
                            {
                                cnddTirpsDic = new Dictionary<string, TIRP>();
                                for (int tins = 0; tins < tirp.tinstancesList.Count; tins++)
                                {   // string trkey = tKeyIdx + "-" + relation + "-" + tValIdx;
                                    //string trK = tirp.tinstancesList[tins].entityIdx + "-" + tirp.tinstancesList[tins].tis[tirp.size - 1].symbol + "-" + tirp.tinstancesList[tins].tis[tirp.size - 1].startTime + "-" + tirp.tinstancesList[tins].tis[tirp.size - 1].endTime;
                                    //if (karmE.getEnttyKrmaByIdxInstncsCntnKey(tirp.tinstancesList[tins].entityIdx, lsTncptIdx, seedRelation, toncept.tonceptINDEX)) // getEntitiesKarmaVec()[tirp.tinstancesList[tins].entityIdx].instancesDicContainsKey( .MatrixRelContainsKey(lsTncptIdx, toncept.tonceptINDEX, seedRelation, trK))
                                    List<TimeIntervalSymbol> tisList = karmE.entityHasInstances(tirp.tinstancesList[tins].entityIdx, lsTncptIdx, seedRelation, toncept.tonceptINDEX, tirp.tinstancesList[tins].tis[tirp.size - 1]);
                                    {
                                        //List<TimeIntervalSymbol> tisList = karmE.getEnttyKrmaByIdxInstncsDicValbyKey(tirp.tinstancesList[tins].entityIdx, lsTncptIdx, seedRelation, toncept.tonceptINDEX); // .MatrixRelGetKey(lsTncptIdx, toncept.tonceptINDEX, seedRelation, trK);
                                        for (int tiIdx = 0; tiIdx < tisList.Count; tiIdx++)
                                        {
                                            string rels = "";
                                            int[] relsVec = new int[tirp.size];
                                            for (int relIdx = 0; relIdx < tirp.size; relIdx++)
                                            {
                                                if(karmE.getForBackWardsMining() == KLC.forwardMining)
                                                    relsVec[relIdx] = KLC.WhichRelationEpsilon(tirp.tinstancesList[tins].tis[relIdx], tisList[tiIdx], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                                                else
                                                    relsVec[relIdx] = KLC.WhichRelationEpsilon(tisList[tiIdx], tirp.tinstancesList[tins].tis[relIdx], karmE.getRelStyle(), karmE.getEpsilon(), karmE.getMaxGap());
                                                rels = rels + relsVec[relIdx];
                                            }
                                            if (!rels.Contains('-'))
                                            {
                                                TIsInstance newIns = new TIsInstance(tirp.size + 1, tirp.tinstancesList[tins].tis, tisList[tiIdx], tirp.tinstancesList[tins].entityIdx);
                                                if (cnddTirpsDic.ContainsKey(rels))
                                                {
                                                    cnddTirpsDic[rels].tinstancesList.Add(newIns);
                                                    cnddTirpsDic[rels].addEntity(newIns.entityIdx);
                                                    cnddTirpsDic[rels].meanHorizontalSupport++;
                                                }
                                                else
                                                {
                                                    TIRP newTIRP = new TIRP(tirp, toncept.tonceptID, seedRelation, relsVec);
                                                    newTIRP.tinstancesList.Add(newIns);
                                                    newTIRP.addEntity(newIns.entityIdx);
                                                    newTIRP.meanHorizontalSupport = 1; 
                                                    cnddTirpsDic.Add(rels, newTIRP);
                                                }
                                            }
                                        }
                                    }
                                }
                                foreach (TIRP cnddTirp in cnddTirpsDic.Values)
                                    if (cnddTirp.entitieVerticalSupport.Count >= karmE.getMinVerSup())
                                    {
                                        cnddTirp.meanHorizontalSupport = cnddTirp.meanHorizontalSupport / cnddTirp.entitieVerticalSupport.Count; // HS compute
                                        if (karmE.getPrint() > KLC.KL_PRINT_NO)
                                            cnddTirp.printTIRP(tw, karmE.getEntitiesKarmaVec(), karmE.getForBackWardsMining(), karmE.getBackwardsRelsIndxs(), karmE.getPrint(), karmE.getRelStyle()); //print
                                        if( cnddTirp.meanHorizontalSupport >= karmE.getMinHrzSup() )
                                            Leven(cnddTirp, tw);
                                    }
                            }
                } //);
            }
        }

    }
}
