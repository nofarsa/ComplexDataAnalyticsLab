using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarmaLegoLib
{
    public class ELEVEN
    {
        Karma karma;
        
        public ELEVEN(Karma setKarma)
        {
            karma = setKarma;
        }

        public static string RunEleven(Karma k, string outFolder, string outFile, TemporalConcept toncept = null)
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
                    //if (k.MatrixFirstIndexNotNull(tTrgtIdx))
                        foreach (TemporalConcept toncept2 in k.getToncepts())
                        //Parallel.ForEach(karma.getToncepts(), toncept => // toncepts, toncept => 
                        {
                            int tErlyID = toncept2.tonceptID;
                            if (k.getTonceptByIDVerticalSupport(tErlyID) > k.getMinVerSup())
                            //if (k.MatrixEntryNotNull(tTrgtIdx, toncept2.tonceptINDEX))
                                for (int rel = 0; rel < k.getRelStyle(); rel++)
                                //Parallel.For(0, relations_style, rel =>
                                {
                                    int verSup = k.karma.getTindexRelVerticalSupport(tTrgtIdx, toncept2.tonceptINDEX, rel); //k.getMatrixVerticalSupport(tTrgtIdx, toncept2.tonceptINDEX, rel);
                                    if (verSup > k.getMinVerSup())
                                    {
                                        freqNum++;
                                        TIRP twoSzdTIRP = k.getMatrixDICasTIRP(tTrgtID, tErlyID, rel);
                                        ELEVEN Karma11 = new ELEVEN(k);
                                        if (k.getPrint() > KLC.KL_PRINT_NO)
                                            twoSzdTIRP.printTIRP(tw, k.getEntitiesVec(), k.getForBackWardsMining(), k.getBackwardsRelsIndxs(), k.getRelStyle());
                                        Karma11.Leven(twoSzdTIRP, tw);
                                    }
                                }  // );
                        }  // );
                    if (k.getPrint() > KLC.KL_PRINT_NO)
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
            string runTime = k.getMxAccsCounter() + "," + ((endTime - starTime).TotalMilliseconds);
            return runTime;
        }

        private void Leven(TIRP tirp, TextWriter tw)
        {
            if (tirp.size == 3 && tirp.toncepts[0] == 53 && tirp.toncepts[1] == 191 && tirp.toncepts[2] == 253)
                tirp.toncepts[0] = tirp.toncepts[0];
            //if (karma.MatrixFirstIndexNotNull(karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1])))
            {
                int lsTncptIdx = karma.getTonceptIndexByID(tirp.toncepts[tirp.size - 1]);
                Dictionary<string, TIRP> cnddTirpsDic = null;
                foreach (TemporalConcept toncept in karma.getToncepts())
                //Parallel.ForEach(karma.getToncepts(), toncept => // toncepts, toncept =>
                {
                    //if (karma.MatrixEntryNotNull(lsTncptIdx, toncept.tonceptINDEX))
                        for (int seedRelation = 0; seedRelation < karma.getRelStyle(); seedRelation++)
                            //if (karma.getMatrixVerticalSupport(lsTncptIdx, toncept.tonceptINDEX, seedRelation) > karma.getMinVerSup())
                            if (karma.karma.getTindexRelVerticalSupport(lsTncptIdx, toncept.tonceptINDEX, seedRelation) > karma.getMinVerSup())
                            {
                                cnddTirpsDic = new Dictionary<string, TIRP>();
                                for (int tins = 0; tins < tirp.tinstancesList.Count; tins++)
                                {   
                                    string trK = tirp.tinstancesList[tins].entityIdx + "-" + tirp.tinstancesList[tins].tis[tirp.size - 1].symbol + "-" + tirp.tinstancesList[tins].tis[tirp.size - 1].startTime + "-" + tirp.tinstancesList[tins].tis[tirp.size - 1].endTime;
                                    //if (karma.MatrixRelContainsKey(lsTncptIdx, toncept.tonceptINDEX, seedRelation, trK))
                                    //if(karma.karma.getTindexRelEidxDic(lsTncptIdx, toncept.tonceptINDEX, seedRelation, tirp.tinstancesList[tins].entityIdx).ContainsKey(tirp.tinstancesList[tins].tis[tirp.size - 1])
                                    {
                                        List<TimeIntervalSymbol> tisList = karma.karma.getTindexRelEidxTisList(lsTncptIdx, toncept.tonceptINDEX, seedRelation, tirp.tinstancesList[tins].entityIdx, tirp.tinstancesList[tins].tis[tirp.size - 1]); // MatrixRelGetKey(lsTncptIdx, toncept.tonceptINDEX, seedRelation, trK);
                                        if (tisList != null)
                                        {
                                            for (int tiIdx = 0; tiIdx < tisList.Count; tiIdx++)
                                            {
                                                string rels = "";
                                                int[] relsVec = new int[tirp.size];
                                                for (int relIdx = 0; relIdx < tirp.size; relIdx++)
                                                {
                                                    if (karma.getForBackWardsMining() == KLC.forwardMining)
                                                        relsVec[relIdx] = KLC.WhichRelationEpsilon(tirp.tinstancesList[tins].tis[relIdx], tisList[tiIdx], karma.getRelStyle(), karma.getEpsilon(), karma.getMaxGap());
                                                    else
                                                        relsVec[relIdx] = KLC.WhichRelationEpsilon(tisList[tiIdx], tirp.tinstancesList[tins].tis[relIdx], karma.getRelStyle(), karma.getEpsilon(), karma.getMaxGap());
                                                    rels = rels + relsVec[relIdx];
                                                }
                                                if (!rels.Contains('-'))
                                                {
                                                    TIsInstance newIns = new TIsInstance(tirp.size + 1, tirp.tinstancesList[tins].tis, tisList[tiIdx], tirp.tinstancesList[tins].entityIdx);
                                                    if (cnddTirpsDic.ContainsKey(rels))
                                                    {
                                                        cnddTirpsDic[rels].tinstancesList.Add(newIns);
                                                        cnddTirpsDic[rels].addEntity(newIns.entityIdx);
                                                    }
                                                    else
                                                    {
                                                        TIRP newTIRP = new TIRP(tirp, toncept.tonceptID, seedRelation, relsVec);
                                                        newTIRP.tinstancesList.Add(newIns);
                                                        newTIRP.addEntity(newIns.entityIdx);
                                                        cnddTirpsDic.Add(rels, newTIRP);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                foreach (TIRP cnddTirp in cnddTirpsDic.Values)
                                    if (cnddTirp.entitieVerticalSupport.Count >= karma.getMinVerSup())
                                    {
                                        if (karma.getPrint() > KLC.KL_PRINT_NO)
                                            cnddTirp.printTIRP(tw, karma.getEntitiesVec(), karma.getForBackWardsMining(), karma.getBackwardsRelsIndxs(), karma.getRelStyle()); //print
                                        Leven(cnddTirp, tw);
                                    }
                            }
                } //);
            }
        }
    }
}
