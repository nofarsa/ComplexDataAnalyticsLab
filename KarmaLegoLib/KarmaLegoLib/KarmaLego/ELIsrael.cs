using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class ELIsrael
    {
        KarmE karmaD;
        
        public ELIsrael(KarmE setKarmaD)
        {
            karmaD = setKarmaD;
        }

        public static string RunEL(KarmE karmE, string outFolder, string outFile, bool onlyBEFORE = false, TemporalConcept toncept = null)
        {
            Directory.CreateDirectory(outFolder);
            DateTime starTime = DateTime.Now;
            if (toncept == null)
            {
                if (karmE.getParallel() == KLC.parallelMining)// && toncept == null)
                    Parallel.ForEach(karmE.getToncepts(), toncept1 =>
                    {
/*remember this!*/      if (karmE.getTonceptByIDVerticalSupport(toncept1.tonceptID) > karmE.getMinVerSup())
                        {
                            ELIsrael el = new ELIsrael(karmE);
                            el.EL(karmE, toncept1, /*toncept,*/ outFolder, onlyBEFORE);
                        }
                    });
                else
                    foreach (TemporalConcept toncept1 in karmE.getToncepts())
                    {
/*remember this!*/      if (karmE.getTonceptByIDVerticalSupport(toncept1.tonceptID) > karmE.getMinVerSup())
                        {
                            ELIsrael el = new ELIsrael(karmE);
                            el.EL(karmE, toncept1, /*toncept,*/ outFolder, onlyBEFORE);
                        }
                    }
            }
            else
            {
                if (karmE.getTonceptByIDVerticalSupport(toncept.tonceptID) > karmE.getMinVerSup())
                {
                    ELIsrael el = new ELIsrael(karmE);
                    el.EL(karmE, toncept, /*toncept,*/ outFolder, onlyBEFORE);
                }
            }
            
            if (karmE.getPrint() > KLC.KL_PRINT_NO)
                KarmE.WriteKLFileFromTonceptsFiles(outFile, outFolder);
            DateTime endTime = DateTime.Now;
            string runTime = /*karmE.getMxAccsCounter() + "," + */(endTime - starTime).TotalMilliseconds.ToString();
            return runTime;
        }

        private void EL(KarmE karmE, TemporalConcept toncept1, /*TemporalConcept toncept,*/ string outFolder, bool onlyBEFORE)
        {
            int tTrgtID = toncept1.tonceptID;
            int tTrgtIdx = toncept1.tonceptINDEX;
            /*if (toncept != null)
            {
                tTrgtID = toncept.tonceptID;
                tTrgtIdx = toncept.tonceptINDEX;
            }*/
            int freqNum = 0;
            string filePath = outFolder + tTrgtID + ".txt";
            TextWriter tw = null;
            if (karmE.getPrint() > KLC.KL_PRINT_NO)
            {
                tw = new StreamWriter(filePath);
                //tw.WriteLine("1 " + tTrgtID + "- " + karmE.getTonceptByIDVerticalSupport(tTrgtID) + " " + karmE.getTonceptByIDVerticalSupport(tTrgtID));
                if (karmE.getPrint() >= KLC.KL_PRINT_TONCEPTANDTIRPS)
                {
                    string instances = " ";
                    for (int i = 0; i < karmE.getTonceptByIDVerticalSupport(tTrgtID); i++)
                    {
                        KeyValuePair<int, List<TimeIntervalSymbol>> userInstances = karmE.getTonceptByID(tTrgtID).getTonceptHorizontalDic().ElementAt(i);
                        for (int j = 0; j < userInstances.Value.Count; j++)
                            instances += karmE.getEntityKarmaByIdx(userInstances.Key).entityID + " [" + userInstances.Value.ElementAt(j).startTime + "-" + userInstances.Value.ElementAt(j).endTime + "] ";
                    }
                    //tw.WriteLine("1 " + tTrgtID + "- -. " + karmE.getTonceptByIDVerticalSupport(tTrgtID) + " " + karmE.getTonceptByIDVerticalSupport(tTrgtID) + instances);
                    tw.WriteLine("1 " + tTrgtID + "- -. " + karmE.getTonceptByIDVerticalSupport(tTrgtID) + " " +  toncept1.getMeanHorizontalSupport().ToString() + instances);
                }
                else
                    //tw.WriteLine("1 " + tTrgtID + "- -. " + karmE.getTonceptByIDVerticalSupport(tTrgtID) + " " + karmE.getTonceptByIDVerticalSupport(tTrgtID));
                    tw.WriteLine("1 " + tTrgtID + "- -. " + karmE.getTonceptByIDVerticalSupport(tTrgtID) + " " + toncept1.getMeanHorizontalSupport().ToString());
            }
            double mhs = toncept1.getMeanHorizontalSupport();
/*new*/     if (toncept1.getMeanHorizontalSupport() >= karmaD.getMinHrzSup())
/*new*/     {
                foreach (TemporalConcept toncept2 in karmE.getToncepts())
                {
                    int tErlyID = toncept2.tonceptID;
                    for (int rel = 0; rel < karmE.getRelStyle(); rel++)
                    {
                        int verSup = 0;
                        //if( karmaD.getForBackWardsMining() == KLC.forwardMining)
                        //    verSup = karmE.getglblMxRelsVerSupport(toncept2.tonceptINDEX, tTrgtIdx, rel);
                        verSup = karmE.getglblTindexVerticalSupport(tTrgtIdx, toncept2.tonceptINDEX, rel);
                        if (verSup > karmE.getMinVerSup())
                        {
                            freqNum++;
                            TIRP twoSzdTIRP = karmE.getTwoSizedTIRP(tTrgtID, tErlyID, rel);
                            if (karmE.getPrint() > KLC.KL_PRINT_NO)
                                twoSzdTIRP.printTIRP(tw, karmE.getEntitiesKarmaVec(), karmE.getForBackWardsMining(), karmE.getBackwardsRelsIndxs(), karmE.getPrint(), karmE.getRelStyle());
 /*new*/                     if (twoSzdTIRP.meanHorizontalSupport >= karmaD.getMinHrzSup() && twoSzdTIRP.size < karmaD.getMaxTirpSize() )
                                ELleven(twoSzdTIRP, tw);
                        }
                        if (onlyBEFORE == true)
                            break;
                    }
                }
    /*new*/ }
            if (karmE.getPrint() > KLC.KL_PRINT_NO)
                tw.Close();
       //     if (freqNum == 0)
       //         File.Delete(filePath);
        }

        private void ELleven(TIRP tirp, TextWriter tw)
        {
            Dictionary<string, TIRP> cnddTirpsDic = null;
            foreach (TemporalConcept toncept in karmaD.getToncepts())
            {
                for (int seedRelation = 0; seedRelation < karmaD.getRelStyle(); seedRelation++)
                    if (karmaD.getglblTindexVerticalSupport(karmaD.getTonceptByID(tirp.toncepts[tirp.size - 1]).tonceptINDEX, toncept.tonceptINDEX, seedRelation) > karmaD.getMinVerSup())
                    {
                        cnddTirpsDic = new Dictionary<string, TIRP>();
                        //string trK = karmaD.getTonceptByID(tirp.toncepts[tirp.size - 1]).tonceptINDEX.ToString() + "-" + seedRelation + "-" + toncept.tonceptINDEX;
                        for (int tins = 0; tins < tirp.tinstancesList.Count; tins++)
                        {
                            if ( karmaD.getEntityKarmaByIdx(tirp.tinstancesList[tins].entityIdx).instancesDicContainsKey(karmaD.getTonceptByID(tirp.toncepts[tirp.size - 1]).tonceptINDEX, seedRelation, toncept.tonceptINDEX)) // .instancesDic.ContainsKey(trK) )
                            {
                                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListDic = karmaD.getEntityKarmaByIdx(tirp.tinstancesList[tins].entityIdx).getInstancesDicValuebyKey(karmaD.getTonceptByID(tirp.toncepts[tirp.size - 1]).tonceptINDEX, seedRelation, toncept.tonceptINDEX); // .instancesDic[trK];
                                if (tiListDic.ContainsKey(tirp.tinstancesList[tins].tis[tirp.size - 1]))
                                {
                                    List<TimeIntervalSymbol> tisList = tiListDic[tirp.tinstancesList[tins].tis[tirp.size - 1]];
                                    for (int tiIdx = 0; tiIdx < tisList.Count; tiIdx++)
                                    {
                                        string rels = "";
                                        int[] relsVec = new int[tirp.size];
                                        for (int relIdx = 0; relIdx < tirp.size; relIdx++)
                                        {
                                            relsVec[relIdx] = KLC.WhichRelationEpsilon(tirp.tinstancesList[tins].tis[relIdx], tisList[tiIdx], karmaD.getRelStyle(), karmaD.getEpsilon(), karmaD.getMaxGap());
                                            rels = rels + relsVec[relIdx];
                                        }
                                        if (!rels.Contains('-'))
                                        {
                                            TIsInstance newIns = new TIsInstance(tirp.size + 1, tirp.tinstancesList[tins].tis, tisList[tiIdx], tirp.tinstancesList[tins].entityIdx); // D);
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
                        }
                        foreach (TIRP cnddTirp in cnddTirpsDic.Values)
                        {
                            if (cnddTirp.entitieVerticalSupport.Count >= karmaD.getMinVerSup() && cnddTirp.meanHorizontalSupport >= karmaD.getMinHrzSup() && cnddTirp.size < KLC.MAX_TIRP_TONCEPTS_SIZE )
                            {
                                cnddTirp.meanHorizontalSupport = cnddTirp.meanHorizontalSupport / cnddTirp.entitieVerticalSupport.Count; // HS compute
                                if (karmaD.getPrint() > KLC.KL_PRINT_NO)
                                   cnddTirp.printTIRP(tw, karmaD.getEntitiesKarmaVec(), karmaD.getForBackWardsMining(), karmaD.getBackwardsRelsIndxs(), karmaD.getPrint(), karmaD.getRelStyle());
                                if (cnddTirp.meanHorizontalSupport >= karmaD.getMinHrzSup() && cnddTirp.size < karmaD.getMaxTirpSize() ) //< KLC.MAX_TIRP_TONCEPTS_SIZE)
                                    ELleven(cnddTirp, tw); 
                            }
                        }
                    }
            }
        }

    }
}
