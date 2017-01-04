using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace KarmaLegoLib
{
    public class Result
    {
        public string    tonceptId;
        public string featureName;

//        public string TIRPs; // SHORT, TIRPs
//        public string Metrics; // Bool, HS, HSNRMH, HSNRMV, 

        public int    featureSize;
        public double cohortSize;
        public List<double> ShortBIN           = new List<double>();
        public List<double> TirpsBIN           = new List<double>();
        public List<double> TirpsHS            = new List<double>();
        public List<double> TirpsHSHNRM        = new List<double>();
        public List<double> TirpsHSIDF         = new List<double>();
        public List<double> TirpsHSVNRM        = new List<double>();
        public List<double> TirpsMND           = new List<double>();
        public List<double> TirpsMINOFST       = new List<double>();
        public List<double> TirpsMEANOFST      = new List<double>();
        public List<double> TirpsHSHSVNRMHSIEF = new List<double>();
        /*public override int GetHashCode()
        {
            int hash = 13, TIRPsSum = 0, MetricsSum = 0;
            for(int i = 0; i < TIRPs.Length; i++)
                TIRPsSum += TIRPs[i];
            for (int i = 0; i < Metrics.Length; i++)
                MetricsSum += Metrics[i];
            hash = (hash * 7) + int.Parse(tonceptId) + TIRPsSum + MetricsSum;
            return hash;
        }

        public override bool Equals(object obj)
        {
            Result other = obj as Result;
            if (tonceptId == other.tonceptId && TIRPs == other.TIRPs && Metrics == other.Metrics)
                return true;
            else
                return false;
        }*/


    }

    public class clsFeature
    {
        public int    tonceptId;
        public string featureName;
        public int    featureSize;
        public double verticalSupport;
        public double InverseTIRPFrequency;
        public double meanHS;
        public double maxHS;
        public double maxMeanD;

        public double featureCosineE;
        public double PearsonCorrelation;
        public double featureStDev;

        public List<double> entitiesMeanDValuesVector = new List<double>();
        public List<int>    entitiesHSValuesVector    = new List<int>();
        public List<int> entitiesMinOffsetValuesVector = new List<int>();
        public List<double> entitiesMeanOffsetValuesVector = new List<double>();
        //clsFeature(string name, int size) { featureName = name; featureSize = size; }

        public static int compareVSAndNames(clsFeature Y, clsFeature X) // A<B? descending
        {
            if (X == null)
                if (Y == null)
                    return 0;
                else
                    return -1;
            else
                if (Y == null)
                    return 1;
                else
                {
                    if (X.verticalSupport == Y.verticalSupport)
                        return X.featureName.CompareTo(Y.featureName); // return 0;
                    else if (X.verticalSupport < Y.verticalSupport)
                        return -1;
                    else
                        return 1;
                }
        }

        public static int compareCosinesE(clsFeature Y, clsFeature X) // A<B? descending
        {
            if (X == null)
                if (Y == null)
                    return 0;
                else
                    return -1;
            else
                if (Y == null)
                    return 1;
                else
                {
                    if (X.featureCosineE == Y.featureCosineE)
                        return 0;
                    else if (X.featureCosineE < Y.featureCosineE)
                        return -1;
                    else
                        return 1;
                }

/*            if (A.featureCosineE < B.featureCosineE)
                return 1;
            else
                return -1;
 */
        }

        public static int compareMinStDevE(clsFeature X, clsFeature Y) // A<B? descending
        {
            if (X == null)
                if (Y == null)
                    return 0;
                else
                    return -1;
            else
                if (Y == null)
                    return 1;
                else
                {
                    if (X.featureStDev == Y.featureStDev)
                        return 0;
                    else if (X.featureStDev < Y.featureStDev)
                        return -1;
                    else
                        return 1;
                }

            /*            if (A.featureCosineE < B.featureCosineE)
                            return 1;
                        else
                            return -1;
             */
        }

        public static int comparePearsonE(clsFeature Y, clsFeature X) // A<B?
        {
            if (X == null)
                if (Y == null)
                    return 0;
                else
                    return -1;
            else
                if (Y == null)
                    return 1;
                else
                {
                    if (X.PearsonCorrelation == Y.PearsonCorrelation)
                        return 0;
                    else if (X.PearsonCorrelation < Y.PearsonCorrelation)
                        return -1;
                    else
                        return 1;
                }

/*            if (B.PearsonCorrelation < A.PearsonCorrelation)
                return 1;
            else
                return -1;
*/ 
        }
    }

    public class Maitreya
    {
        public static ConcurrentBag<string> runMultiFoldMiningAndMaitreyaEval(string sourceDataDir, bool paralleling, int minVerSup, int[] maxGapVec, int[] epsilonVec, int[] relsVec, int maxTirpSize)
        {
            //sourceDataDir = "D:\\M11\\new_cumc_sources\\procedures_sourceFiles_oT365_pT30_mE50_minE40\\Maitreya_JAMIA\\251_\\sourceData";// conditions_sourceFiles_oT365_pT30_mE50_minE30\\sourceData"; //Maitreya_JAMIA\\sourceData"; // 
            bool onlyBEFORE = false;
            //bool paralleling = KLC.parallelMining; // KLC.nonParallelMining; // remember to change and investigate this
            //int minVerSup = 0, maxGap = 210, 
            //int maxTirpSize = 3;
            //int minVerSup = 20/*, maxGap = 210 /*210*/, maxTirpSize = 3;
            //int[] maxGapVec = {730}; //180, 270};
            double minHrzSup = 0; //double[] minHrzSupVec = { 0 }; // 1, 1.25, 1.5 };
            //int[] epsilonVec = { 0 }; //, 90 }; //  0, 30, 60, 90, 120, 150, 180, 210 }; //,
            //int[] relStyleVec = { KLC.RELSTYLE_ALLEN7 }; //KLC.RELSTYLE_KL3, 
            string filesFolder = "";
            //string tonceptFilePath = "D:\\M11\\new_cumc_sources\\procedures_cosinebased_ceBased_sourceFiles_oT365_pT30_mE0_minToE30\\sourceData\\2002174_oT365_pT30_mE0_mTE30_1406_71.70982_72_OFILE.csv"; //2002174_oT365_pT30_mE50_mTE40_1710_82.8848_83_OFILE.csv";
            //runPredictionOnFile(tonceptFilePath, onlyBEFORE, minVerSup, minHrzSupVec[0], maxGap, maxTirpSize, epsilonVec[0], paralleling, relStyleVec[0], "D:\\M11\\new_cumc_sources\\procedures_cosinebased_ceBased_sourceFiles_oT365_pT30_mE0_minToE30\\_ms20_mhs0_mg210_mxTrpSz3_e0_r3_2002174" + "\\");

            ConcurrentBag<string> filesFoldersBag = new ConcurrentBag<string>();
            int maxGap = maxGapVec[0];
            if (paralleling == KLC.parallelMining)
            {
                //Parallel.ForEach(maxGapVec, maxGap =>
                //{
                    Parallel.For(0, epsilonVec.Length, epsilonIdx => // Parallel.ForEach(epsilonVec, epsilon => //
                    //for (int r = 0; r < relStyleVec.Length; r++)
                    {
                        Parallel.For(0, relsVec.Length, relsIdx => // Parallel.ForEach(relsVec, relStyle => //
                        //for (int e = 0; e < epsilonVec.Length; e++)
                        {
                            filesFolder = "allT1_ms" + minVerSup + "_mhs" + minHrzSup + "_mg" + maxGap + "_mxTrpSz" + maxTirpSize + "_e" + epsilonVec[epsilonIdx] + "_r" + relsVec[relsIdx];
                            filesFoldersBag.Add(filesFolder);
                            string outDir = sourceDataDir.Replace("\\sourceData", "") + "\\" + filesFolder;
                            List<string> tonceptTirpsFileList = new List<string>();
                            Directory.CreateDirectory(outDir);
                            outDir += "\\";
                            for (int i = 0; i < Directory.GetFiles(sourceDataDir).Length; i++)
                            {
                                string filePath = Directory.GetFiles(sourceDataDir)[i];
                                if (filePath.Contains("OFILE.csv"))
                                    tonceptTirpsFileList.Add(filePath);
                            }

                            //foreach (string toncepTirpsFilePath in tonceptTirpsFileList)
                            Parallel.ForEach(tonceptTirpsFileList, toncepTirpsFilePath =>
                            {//869MAXBN - robert2014nyc
                                string tonceptId = toncepTirpsFilePath.Split('\\')[toncepTirpsFilePath.Split('\\').Length - 1].Split('_')[0]; // toncepTirpsFile.Split(';')[1];
                                Directory.CreateDirectory(outDir + tonceptId);
                                runPredictionOnFile(toncepTirpsFilePath, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilonVec[epsilonIdx], paralleling, relsVec[relsIdx], outDir + tonceptId + "\\");
                            });
                        });
                    });
                //});
                    
            }
            else // not parallel
            {
                for (int r = 0; r < relsVec.Length; r++)
                {
                    for (int e = 0; e < epsilonVec.Length; e++)
                    {
                        //Parallel.ForEach(minHrzSupVec, minHrzSup =>
                        //double minHrzSup = 0;
                        for(int mg = 0; mg < maxGapVec.Length; mg++)
                        {
                            filesFolder = "tst_ms" + minVerSup + "_mhs" + minHrzSup + "_mg" + maxGapVec[mg] + "_mxTrpSz" + maxTirpSize + "_e" + epsilonVec[e] + "_r" + relsVec[r];
                            filesFoldersBag.Add(filesFolder);
                            string outDir = sourceDataDir.Replace("\\sourceData", "") + "\\" + filesFolder;
                            List<string> tonceptTirpsFileList = new List<string>();
                            Directory.CreateDirectory(outDir);
                            outDir += "\\";
                            for (int i = 0; i < Directory.GetFiles(sourceDataDir).Length; i++)
                            {
                                string filePath = Directory.GetFiles(sourceDataDir)[i];
                                if (filePath.Contains("OFILE.csv"))
                                    tonceptTirpsFileList.Add(filePath);
                            }

                            foreach (string toncepTirpsFilePath in tonceptTirpsFileList)
                            {//869MAXBN - robert2014nyc
                                string tonceptId = toncepTirpsFilePath.Split('\\')[toncepTirpsFilePath.Split('\\').Length - 1].Split('_')[0]; // toncepTirpsFile.Split(';')[1];
                                Directory.CreateDirectory(outDir + tonceptId);
                                runPredictionOnFile(toncepTirpsFilePath, onlyBEFORE, minVerSup, minHrzSup, maxGapVec[mg], maxTirpSize, epsilonVec[e], paralleling, relsVec[r], outDir + tonceptId + "\\");
                            }
                        }//);
                    }
                }
            }
            /*filesFolder = "";
            foreach (string ff in filesFoldersBag)
                filesFolder = filesFolder + ff + ";";
            return filesFolder;
             */
            return filesFoldersBag;

        }

        
        public static void runPredictionOnFile(string filePath, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int maxTirpSize, int epsilon, bool paralleling, int relStyle, string filesFolder)
        {
            //filePath = "D:\\M11\\new_cumc_sources\\procedures_oT365_pT30_mE50_minE30_experiment\\sourceData\\2514434_oT365_pT30_mE50_mTE30_1525_65.76_64.3154098360656_OFILE.csv";
            string fileName = filePath.Split('\\')[filePath.Split('\\').Count()-1];
            //string outDir = filePath.Replace( "sourceData\\" + fileName, "");
            //outDir = outDir + filesFolder + "\\";
            //Directory.CreateDirectory(outDir + "arffiles");
            int tonceptId = int.Parse(fileName.Split('_')[0]);
            int oSize = int.Parse(fileName.Split('_')[5]);
            int miningFolds = 3;
            int foldSize = oSize / miningFolds;

            //foldSize = 100; /// REMOVE THIS AT THE END OF THE EXPERIMENT~~~!!!
            if (paralleling == KLC.parallelMining)
            {
                Parallel.For(0, miningFolds, f =>
                {
                    //mineAndEvaluateFold(f, foldSize, filesFolder, filePath, tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, miningFolds);
                    tirpSelectAndEvaluateFold(f, foldSize, filesFolder, filePath, tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, miningFolds);
                });
            }
            else
                for (int f = 0; f < miningFolds; f++)
                    //mineAndEvaluateFold(f, foldSize, filesFolder, filePath, tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, miningFolds);
                    tirpSelectAndEvaluateFold(f, foldSize, filesFolder, filePath, tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, miningFolds);
            
        }

        public static void tirpSelectAndEvaluateFold(int f, int foldSize, string filesFolder, string filePath, int tonceptId, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int maxTirpSize, int epsilon, bool paralleling, int relStyle, int miningFolds)
        {   
            int eFrst = f * foldSize;
            int eLast = eFrst + foldSize; // 150;

            string cohortBackTIRPsFile = mineBack(f, false, filesFolder, filePath, tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, eLast, eFrst);
            string contrlBackTIRPsFile = mineBack(f, false, filesFolder, filePath.Replace("OFILE", "OtherFILE") , tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, eLast, eFrst);

            //string backTIRPsFile = "D:\\M11\\new_cumc_sources\\procedures_sourceFiles_oT365_pT30_mE50_minE40\\BMC_Medicine\\_ms20_mhs0_mg210_mxTrpSz4_e0_r3_2002178\\2002178_oT365_pT30_mE50_mTE40_2547_76.03966_77_OFILE0_ms_20_mhs0_mg_210_mxTrpSze_4_e_0_r_3_onlyTncpt_KarmEL.txt";
            int tirpSLimit = 20000;

            List<int> cohortTonceptsIds = new List<int>(), contrlTonceptsIds = new List<int>();
            List<TIRP> cohortTirpsList = Single11EVEN.readTIRPsFile(cohortBackTIRPsFile, ref cohortTonceptsIds, relStyle, true, tonceptId, true);
            List<TIRP> contrlTirpsList = Single11EVEN.readTIRPsFile(contrlBackTIRPsFile, ref contrlTonceptsIds, relStyle, true, tonceptId, true);

            //select
            for (int t = 0; t < cohortTirpsList.Count; t++)
            {
                TIRP chrtTirp = cohortTirpsList.ElementAt(t);
                TIRP ctrlTirp = contrlTirpsList.Find(x => x.tncptsRels == chrtTirp.tncptsRels);
                if (ctrlTirp != null)
                {
                    
                    chrtTirp.selScore_VS = (chrtTirp.getVerticalSupport() - ctrlTirp.getVerticalSupport()) / foldSize;
                    double chrtMnHS = chrtTirp.getSumHorizontalSupport(), ctrlMnHS = ctrlTirp.getSumHorizontalSupport();
                    
                    int chrtHsSize = 0, chrtMndSize = 0, ctrlHsSize = 0, ctrlMndSize = 0;
                    double chrtStDevHS = 0, chrtMeanHS = 0, chrtStDevMND = 0, chrtMeanMND = 0, ctrlStDevHS = 0, ctrlMeanHS = 0, ctrlStDevMND = 0, ctrlMeanMND = 0;
                    chrtMeanMND = chrtTirp.returnMeanHSAndMND(ref chrtMeanHS, ref chrtStDevHS, ref chrtStDevMND, ref chrtHsSize, ref chrtMndSize);
                    ctrlMeanMND = ctrlTirp.returnMeanHSAndMND(ref ctrlMeanHS, ref ctrlStDevHS, ref ctrlStDevMND, ref ctrlHsSize, ref ctrlMndSize);
                    
                    chrtTirp.selScore_MnHS = Math.Abs(chrtMeanHS - ctrlMeanHS);
                    double sqrtHsNs = Math.Sqrt(Math.Pow(chrtTirp.getMeanHorizontalSupport(), -1) + Math.Pow(ctrlTirp.getMeanHorizontalSupport(), -1));
                    double sqrtHsNsTEST = Math.Pow(chrtHsSize, -1);
                    sqrtHsNsTEST = sqrtHsNsTEST + Math.Pow(ctrlHsSize, -1);
                    sqrtHsNsTEST = Math.Sqrt(sqrtHsNsTEST);
                    double sX1X2 = Math.Sqrt((((chrtHsSize - 1) * Math.Pow(chrtStDevHS, 2) + ((ctrlHsSize - 1) * Math.Pow(ctrlStDevHS, 2))) / (chrtHsSize + ctrlHsSize - 2)));
                    chrtTirp.selScore_MnHS = chrtTirp.selScore_MnHS / (sX1X2 * sqrtHsNs);
                    
                    chrtTirp.selScore_MnDr = Math.Abs(chrtMeanMND - ctrlMeanMND);
                    double sqrtMndNs = Math.Sqrt(Math.Pow(chrtMndSize, -1) + Math.Pow(ctrlMndSize, -1));
                    double sMndX1X2 = Math.Sqrt((((chrtMndSize - 1) * Math.Pow(chrtStDevMND, 2) + ((chrtMndSize - 1) * Math.Pow(ctrlStDevMND, 2))) / (chrtMndSize + ctrlMndSize - 2)));
                    chrtTirp.selScore_MnDr = chrtTirp.selScore_MnDr / (sMndX1X2 * sqrtMndNs);
 
                }
            }

            int eval_eFrst = (f + 1) * foldSize;
            int eval_eLast = (f + 2) * foldSize;
            if (f == miningFolds - 1)
            {
                eval_eFrst = 0;
                eval_eLast = foldSize;
            }

            List<TIRP> tirpsList = cohortTirpsList;
            string firstLine = "paitentID,";
            for (int t = 0; t < tirpsList.Count && t < tirpSLimit; t++)
            {
                for (int tncpt = 0; tncpt < tirpsList.ElementAt(t).toncepts.Length; tncpt++)
                    firstLine += (tirpsList.ElementAt(t).toncepts[tncpt] + "-");
                //
                for (int rel = 0; rel < tirpsList.ElementAt(t).rels.Length; rel++)
                    firstLine += (tirpsList.ElementAt(t).rels[rel] + "r");
                // ADD THE VS_DIFF, the HS_MEANS_DIFF, and the MND_MEANS_DIFF
                firstLine += ("," + tirpsList.ElementAt(t).size + "," + tirpsList.ElementAt(t).selScore_VS + "/" + tirpsList.ElementAt(t).selScore_MnHS + "/" + tirpsList.ElementAt(t).selScore_MnDr + ",," ); 
                
            }
            firstLine += "class,\n";

            List<int> tonceptsIds = cohortTonceptsIds;
            string outcome_bySTD_ClassEntities = "";
            SingleKarmaLego.detectEntitiesByTIRPs(ref outcome_bySTD_ClassEntities, tirpSLimit, eval_eFrst, eval_eLast, 11, filePath, tirpsList, 1, relStyle, epsilon, maxGap, tonceptsIds);
            string other_bySTD_ClassEntities = "";
            SingleKarmaLego.detectEntitiesByTIRPs(ref other_bySTD_ClassEntities, tirpSLimit, eval_eFrst, eval_eLast, 11, filePath.Replace("OFILE", "OtherFILE"), tirpsList, 0, relStyle, epsilon, maxGap, tonceptsIds);

            string backTIRPsFile = cohortBackTIRPsFile;
            string matrixFile = backTIRPsFile.Replace("OFILE", "MATRIX");
            matrixFile = matrixFile.Replace("_onlyTncpt_KarmEL.txt", ".csv");
            CUMC_Handler.writeToFile(matrixFile, (firstLine + outcome_bySTD_ClassEntities + other_bySTD_ClassEntities));
            
        }

        public static void mineAndEvaluateFold(int f, int foldSize, string filesFolder, string filePath, int tonceptId, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int maxTirpSize, int epsilon, bool paralleling, int relStyle, int miningFolds)
        {
            int eFrst = f * foldSize;
            int eLast = eFrst + foldSize; // 150;

            string backTIRPsFile = mineBack(f, false, filesFolder /*outDir*/, filePath, tonceptId, onlyBEFORE, minVerSup, minHrzSup, maxGap, maxTirpSize, epsilon, paralleling, relStyle, eLast, eFrst);

            //string backTIRPsFile = "D:\\M11\\new_cumc_sources\\procedures_sourceFiles_oT365_pT30_mE50_minE40\\BMC_Medicine\\_ms20_mhs0_mg210_mxTrpSz4_e0_r3_2002178\\2002178_oT365_pT30_mE50_mTE40_2547_76.03966_77_OFILE0_ms_20_mhs0_mg_210_mxTrpSze_4_e_0_r_3_onlyTncpt_KarmEL.txt";
            int tirpSLimit = 20000;

            List<int> tonceptsIds = new List<int>();
            List<TIRP> tirpsList = Single11EVEN.readTIRPsFile(backTIRPsFile, ref tonceptsIds, relStyle, true, tonceptId, true);

            int eval_eFrst = (f + 1) * foldSize;
            int eval_eLast = (f + 2) * foldSize;
            if (f == miningFolds - 1)
            {
                eval_eFrst = 0;
                eval_eLast = foldSize;
            }

            string firstLine = "paitentID,";
            for (int t = 0; t < tirpsList.Count && t < tirpSLimit; t++)
            {
                for (int tncpt = 0; tncpt < tirpsList.ElementAt(t).toncepts.Length; tncpt++)
                    firstLine += (tirpsList.ElementAt(t).toncepts[tncpt] + "-");
                //
                for (int rel = 0; rel < tirpsList.ElementAt(t).rels.Length; rel++)
                    firstLine += (tirpsList.ElementAt(t).rels[rel] + "r");
                //
                firstLine += ("," + tirpsList.ElementAt(t).size + ",,,");
            }
            firstLine += "class,\n";

            //experiment DELETE at the END
            //eval_eFrst = eFrst; eval_eLast = eLast;
            /*string tirpsFeatures = CreateStringEntitiesMatrix(tirpsList, (eLast-eFrst), 1, tirpSLimit);
            CUMC_Handler.writeToFile(backTIRPsFile.Replace("KarmEL.txt", "MATRIX_frmtrps.csv"), (firstLine + tirpsFeatures));
            string std_bySTD_ClassEntities = "";
            double sklTime = SingleKarmaLego.detectEntitiesByTIRPs(ref std_bySTD_ClassEntities, tirpSLimit, eFrst, eLast, 0, filePath, tirpsList, 1, relStyle, epsilon, maxGap, tonceptsIds);
            CUMC_Handler.writeToFile(backTIRPsFile.Replace("KarmEL.txt", "MATRIX_std.csv"), (firstLine + std_bySTD_ClassEntities));
            string skl_bySTD_ClassEntities = "";
            double s11Time = SingleKarmaLego.detectEntitiesByTIRPs(ref skl_bySTD_ClassEntities, tirpSLimit, eFrst, eLast, 11, filePath, tirpsList, 1, relStyle, epsilon, maxGap, tonceptsIds);
            CUMC_Handler.writeToFile(backTIRPsFile.Replace("KarmEL.txt", "MATRIX_skl.csv"), (firstLine + skl_bySTD_ClassEntities));
            */
            //experiment

            string outcome_bySTD_ClassEntities = "";
            SingleKarmaLego.detectEntitiesByTIRPs(ref outcome_bySTD_ClassEntities, tirpSLimit, eval_eFrst, eval_eLast, 11, filePath, tirpsList, 1, relStyle, epsilon, maxGap, tonceptsIds);
            string other_bySTD_ClassEntities = "";
            SingleKarmaLego.detectEntitiesByTIRPs(ref other_bySTD_ClassEntities, tirpSLimit, eval_eFrst, eval_eLast, 11/*0-probablyWasAMistake*/, filePath.Replace("OFILE", "OtherFILE"), tirpsList, 0, relStyle, epsilon, maxGap, tonceptsIds);

            /*
            string other_bySTD_ClassEntities_0 = "";
            SingleKarmaLego.detectEntitiesByTIRPs(ref other_bySTD_ClassEntities_0, tirpSLimit, eval_eFrst, eval_eLast, 0, filePath.Replace("OFILE", "OtherFILE"), tirpsList, 0, relStyle, epsilon, maxGap, tonceptsIds);
            if (other_bySTD_ClassEntities.CompareTo(other_bySTD_ClassEntities_0) != 0)
            {
                string[] otherVec = other_bySTD_ClassEntities.Split('\n');
                string[] otherVec_0 = other_bySTD_ClassEntities_0.Split('\n');
                for (int i = 0; i < otherVec.Length; i++)
                    if (otherVec[i] != otherVec_0[i])
                        i = i;
            }*/

            string matrixFile = backTIRPsFile.Replace("OFILE", "MATRIX");
            matrixFile = matrixFile.Replace("_onlyTncpt_KarmEL.txt", ".csv");
            CUMC_Handler.writeToFile(matrixFile, (firstLine + outcome_bySTD_ClassEntities + other_bySTD_ClassEntities));

            //createCSVorARFFfiles(matrixFile);

        }

        
        public static string evalSingle_TD_KL(string mainDir, string filePath, int tonceptID, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int epsilon, int relStyle, bool paralleling)
        {
            int eSizeLimit = 200; // 300; // ICUSAX3-200;
            minVerSup = 70;
            bool kl = true;
            string outDir = mainDir.Replace("\\sourceData", "") + "\\_ms" + minVerSup + "_mhs" + minHrzSup + "_mg" + maxGap + "_e" + epsilon + "_r" + relStyle;
            List<string> tonceptTirpsFileList = new List<string>();
            Directory.CreateDirectory(outDir);
            outDir += "\\";
            
            List<TIRP> tirpsList = null;
            int[] tirpSizeVec = {20, 30, 40, 50, 60, 80, 100, 120 };
            int[] entitieSizeVec = { 1, 5, 10, 50, 100, 200 }; //, 150, 200 }; //, 200, 1800 };
            int maxEntSize = 200;
            int[] maxGapVec = { 5, 10 }; //, 15 }; // 200 };
            string runtime = "maxGap, |entieties| , |tirps| , linear, skl , skl11 ,\n";
            List<int> tonceptIds = new List<int>();
            foreach (int maxG in maxGapVec)
            {
                string backTIRPsFile = mineBack(0, kl, outDir, filePath, tonceptID, onlyBEFORE, minVerSup, minHrzSup, maxG, 100, epsilon, paralleling, relStyle, maxEntSize);
                //tirpsList = Single11EVEN.readTIRPsFile(backTIRPsFile, ref tonceptIds, true, tonceptID, true);
                string tirpsFeatures = ""; // CreateStringEntitiesMatrix(tirpsList, entSize, 1);
                tonceptIds.Clear();
                tirpsList = Single11EVEN.readTIRPsFile(backTIRPsFile, ref tonceptIds, relStyle, true, tonceptID, false);
    
                foreach (int entSize in entitieSizeVec)
                {
                    foreach (int tirpS in tirpSizeVec)
                    {
                        string str = "";
                        //double seqTime = SingleKarmaLego.mineClassEntitiesByTIRPs(ref str, tirpS, /*maxEntSize,*/ entSize, 0, filePath, tirpsList, 1, relStyle, epsilon, maxG, null);
                        double seqTime = SingleKarmaLego.detectEntitiesByTIRPs(ref str, tirpS, /*maxEntSize,*/0, entSize, 0, filePath, tirpsList, 1, relStyle, epsilon, maxG, null);

                        //double sklTime = SingleKarmaLego.mineClassEntitiesByTIRPs(ref str, tirpS, /*maxEntSize,*/ entSize, 1, filePath, tirpsList, 1, relStyle, epsilon, maxG, null);
                        double sklTime = SingleKarmaLego.detectEntitiesByTIRPs(ref str, tirpS, /*maxEntSize,*/0, entSize, 1, filePath, tirpsList, 1, relStyle, epsilon, maxG, null);

                        //double skl11Time = SingleKarmaLego.mineClassEntitiesByTIRPs(ref str, tirpS, /*maxEntSize,*/ entSize, 11, filePath, tirpsList, 1, relStyle, epsilon, maxG, tonceptIds);
                        double skl11Time = SingleKarmaLego.detectEntitiesByTIRPs(ref str, tirpS, /*maxEntSize,*/0, entSize, 11, filePath, tirpsList, 1, relStyle, epsilon, maxG, tonceptIds);

                        runtime += maxG + "," + entSize + "," + tirpS + "," + seqTime + "," + sklTime + "," + skl11Time + ",\n";
                        
                    }
                }
            }
            CUMC_Handler.writeToFile(mainDir + "\\runTime_" + tirpsList.Count() + filePath.Split('\\')[filePath.Split('\\').Count()-1], runtime);
                      
            //CUMC_Handler.writeToFile(mainDir + "\\tirpsFeatures.csv", tirpsFeatures);
            //CUMC_Handler.writeToFile(mainDir + "\\singleKarmaLegoFeatures.csv", singleKarmaLegoFeatures);
            //CUMC_Handler.writeToFile(mainDir + "\\singleLinearFeatures.csv", singleLinearFeatures);

            //if (tirpsFeatures == singleKarmaLegoFeatures && tirpsFeatures == singleLinearFeatures)
            //    return resulTotalMillisecs + "\n"; //entitieSize = entitieSize;
            return ",,,\n";

        }

        private static string CreateStringEntitiesMatrix(List<TIRP> tirpList, int entitieSize, int classLabel, int tirpSizeLimit)
        {
            Dictionary<string, int> idsDic = new Dictionary<string, int>();
            int[,] entitiesHSs       = new int[entitieSize,tirpList.Count()];
            double[,] entitiesMeanDs = new double[entitieSize,tirpList.Count()];
            
            for (int t = 0; t < tirpList.Count() && t < tirpSizeLimit; t++)
            {
                TIRP tirp = tirpList.ElementAt(t);
                for (int e = 0; e < tirp.entInstancesDic.Count() && e < entitieSize; e++)
                {
                    string eId = tirp.entInstancesDic.ElementAt(e).Key.ToString();
                    if (eId.Length == 5)
                        eId = "00" + eId;
                    else if (eId.Length == 6)
                        eId = "0" + eId;
                        
                    if (!idsDic.ContainsKey(eId))
                        idsDic.Add(eId, idsDic.Count());
                    int eIdx = idsDic[eId];

                    double timeDuration = 0;
                    for(int i = 0; i < tirp.entInstancesDic.ElementAt(e).Value.Count(); i++)
                    {
                        TIsInstance tinstance = tirp.entInstancesDic.ElementAt(e).Value.ElementAt(i);
                        timeDuration += tinstance.tis[tinstance.tis.Length-1].endTime - tinstance.tis[0].startTime;
                    }
                    timeDuration = timeDuration / tirp.entInstancesDic.ElementAt(e).Value.Count();
                    entitiesHSs[eIdx, t] = tirp.entInstancesDic.ElementAt(e).Value.Count();
                    entitiesMeanDs[eIdx, t] = timeDuration;
                }
            }
            string Features = "";
            idsDic = idsDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            for (int e = 0; e < idsDic.Count(); e++)
            {
                string entityFeatures = "";
                for (int t = 0; t < tirpList.Count() && t < tirpSizeLimit; t++)
                    entityFeatures += entitiesHSs[idsDic.ElementAt(e).Value, t] + "," + entitiesMeanDs[idsDic.ElementAt(e).Value, t] + ",";
                string eId = idsDic.ElementAt(e).Key;
                if (eId.StartsWith("0")) 
                    eId = eId.Substring(1);
                if (eId.StartsWith("0")) eId = eId.Substring(1);
                Features += eId + "," + entityFeatures + classLabel + "\n";
            }
            return Features;
        }
        
        private static string mineBack(int foldID, bool kl, string mainDir, string dataset, int tonceptID, bool onlyBEFORE, int minVerSup, double minHrzSup, int maxGap, int maxTIRPSize, int epsilon, bool paralleling, int relStyle, int eLast, int Efrst = 0, bool setHS1 = false, int print = KLC.KL_PRINT_TONCEPTANDTIRPS) // NO_INSTANCES) // KLC.KL_PRINT_TIRPS)
        {
            string klFile = dataset;
            string tempDir = mainDir + "tempDir_" + tonceptID + "_" + foldID + "_" + DateTime.Now.Hour + DateTime.Now.Minute +"\\";
            Directory.CreateDirectory(tempDir);

            string karmETime = "karmETime";
            if (kl == true)
            {
                Karma ka = new Karma(true, KLC.backwardsMining, relStyle, epsilon, maxGap, minVerSup, klFile, KLC.KL_TRANS_YES, setHS1, print, ref karmETime, eLast); //entitieSizeLimit);
                //entitieSizeLimit = ka.getEntitieSize();
                dataset = mainDir + dataset.Split('\\')[dataset.Split('\\').Length - 1].Replace(".csv", "") + foldID + "_ms_" + minVerSup + "_mhs" + minHrzSup + "_mg_" + maxGap + "_mxTrpSze_" + maxTIRPSize + "_e_" + epsilon + "_r_" + relStyle + "_onlyTncpt_KarmEL.txt";
                string karmaTime = Lego.RunLegoALL(ka, tempDir, dataset, KLC.KL_TRANS_YES); // string karmelTime = ELIsrael.RunEL(kE, tempDir, dataset, onlyBEFORE, null);
                ka = null;
            }
            else
            {
                KarmE kE = new KarmE(KLC.backwardsMining, relStyle, epsilon, maxGap, minVerSup, minHrzSup, klFile, KLC.KL_TRANS_YES, setHS1, print, paralleling, ref karmETime, maxTIRPSize, eLast, Efrst);
                //entitieSize = kE.getEntitieSize();
                dataset = mainDir + dataset.Split('\\')[dataset.Split('\\').Length - 1].Replace(".csv", "") + foldID + "_ms_" + minVerSup + "_mhs" + minHrzSup + "_mg_" + maxGap + "_mxTrpSze_" + maxTIRPSize + "_e_" + epsilon + "_r_" + relStyle + "_onlyTncpt_KarmEL.txt";
                string karmelTime = ELIsrael.RunEL(kE, tempDir, dataset, onlyBEFORE, null);
                kE = null;
            }
            return dataset;
        }

        public static void summarizeWekaResults(string wekaResults, int classfoldsSize, string tonceptsFile) // 0_12_134736_730_60_mTE50_527_101_70_MATRIX0_ms_mhs0_mg_mxTrpSze_e_r_SHORT_VrSpO_BIN
        {
            TextReader tr = new StreamReader(wekaResults);
            string readLine = tr.ReadLine();
            string[] readVec = readLine.Split(',');
            //string summarizedFile = "toncept_ID,minEvents,cohort,number,minSup,maxGap,epsilon,R,Rep,TIRPs,TPR,FPR,TNR,FNR,AUC\n";
            //string summarizedFile = "tirpsNum,toncept_ID,obserTime,predTime,minEvents,minE,cohort,meanToEvent,meanToEvent,miningFolds,minSup,?,maxGap,epsilon,R,Rep,TIRPs,trainInst,testInst,TPR,FPR,TNR,FNR,AUC,precision,recall,Fmeasure\n";
            string summarizedFile = "entsWithoutTIRPs,tirpsNum,toncept_ID,obserTime,predTime,minEvents,minE,cohort,meanToEventCohort,meanToEventControl,miningFolds,minSup,mhs,maxGap,mxTrpSze,epsilon,R,TIRPs,OCFS,Rep,trainInst,testInst,TPR,FPR,TNR,FNR,AUC,precision,recall,Fmeasure\n";
            List<double> doubList = new List<double>();
            Dictionary<string, Result> resultsDic = new Dictionary<string, Result>();

            int counter = 0, rowsCounter = 0;
            int summarySize = classfoldsSize * 3;
            double trn07 = 0, tst08 = 0, tpr29 = 0, fpr31 = 0, tnr33 = 0, fnr35 = 0, prec37 = 0, recl38 = 0, Fmsr39 = 0, aucMeasure40 = 0;
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                readLine = tr.ReadLine();
                readVec = readLine.Split(',');

                Result res = new Result();
                string[] attributes = readVec[0].Split('_');
                res.tonceptId = attributes[2];
                string TIRPs = attributes[16];
                string Metrics = attributes[18];

                if( resultsDic.ContainsKey(res.tonceptId) )
                    res = resultsDic[res.tonceptId]; //.Add(double.Parse(readVec[40]));
                else
                    resultsDic.Add(res.tonceptId, res);
                if (TIRPs == "SHORT") // && res.Metrics == "BIN")
                    res.ShortBIN.Add(double.Parse(readVec[40]));
                else
                {
                    if (Metrics == "BIN")
                        res.TirpsBIN.Add(double.Parse(readVec[40]));
                    else if (Metrics == "HS")
                        res.TirpsHS.Add(double.Parse(readVec[40]));
                    else if (Metrics == "HSHNRM")
                        res.TirpsHSHNRM.Add(double.Parse(readVec[40]));
                    else if (Metrics == "HSVNRM")
                        res.TirpsHSVNRM.Add(double.Parse(readVec[40]));
                    else if (Metrics == "HSIDF")
                        res.TirpsHSIDF.Add(double.Parse(readVec[40]));
                    else if (Metrics == "MND")
                        res.TirpsMND.Add(double.Parse(readVec[40]));
                    else if (Metrics == "MINOFST")
                        res.TirpsMINOFST.Add(double.Parse(readVec[40]));
                    else if (Metrics == "MEANOFST")
                        res.TirpsMEANOFST.Add(double.Parse(readVec[40]));
                    else if (Metrics == "HSHSVNRMHSIEF")
                        res.TirpsHSHSVNRMHSIEF.Add(double.Parse(readVec[40]));
                }

                trn07 += double.Parse(readVec[07]);
                tst08 += double.Parse(readVec[08]);

                tpr29 += double.Parse(readVec[29]);
                fpr31 += double.Parse(readVec[31]);
                tnr33 += double.Parse(readVec[33]);
                fnr35 += double.Parse(readVec[35]);

                prec37 += double.Parse(readVec[37]);
                recl38 += double.Parse(readVec[38]);
                Fmsr39 += double.Parse(readVec[39]);

                doubList.Add(double.Parse(readVec[40]));
                aucMeasure40 += double.Parse(readVec[40]);
                counter++;

                //summarizedFile += readVec[0].Replace("_", ",") + "," + (readVec[07] + "00000").Substring(0, 5) + "," + (readVec[08] + "00000").Substring(0, 5) + "," + (readVec[29] + "00000").Substring(0, 5) + "," + (readVec[31] + "00000").Substring(0, 5) + "," + (readVec[33] + "00000").Substring(0, 5) + "," + (readVec[35] + "00000").Substring(0, 5) + "," + (readVec[40] + "00000").Substring(0, 5) + "," + (readVec[37] + "00000").Substring(0, 5) + "," + (readVec[38] + "00000").Substring(0, 5) + "," + (readVec[39] + "00000").Substring(0, 5) + "\n";
                summarizedFile += readVec[0].Replace("_", ",") + "," + (readVec[07] + "00000").Substring(0, 5) + "," + (readVec[08] + "00000").Substring(0, 5) + "," + (readVec[29] + "00000").Substring(0, 5) + "," + (readVec[31] + "00000").Substring(0, 5) + "," + (readVec[33] + "00000").Substring(0, 5) + "," + (readVec[35] + "00000").Substring(0, 5) + "," + (readVec[40] + "00000").Substring(0, 5) + "," + (readVec[37] + "00000").Substring(0, 5) + "," + (readVec[38] + "00000").Substring(0, 5) + "," + (readVec[39] + "00000").Substring(0, 5) + "\n";

                /*if (counter == summarySize) // 10)
                {
                    double stDevAUC = 0, meanAUC = aucMeasure40 / summarySize;
                    for (int i = 0; i < doubList.Count; i++)
                        stDevAUC += Math.Pow(doubList[i] - meanAUC, 2);
                    stDevAUC = stDevAUC / doubList.Count;
                    stDevAUC = Math.Sqrt(stDevAUC);
                    double ci = 1.96*stDevAUC/Math.Sqrt(summarySize);
                    double meanMci = meanAUC - ci, meanPci = meanAUC + ci;
                    
                    if (rowsCounter == 128)
                        rowsCounter = rowsCounter;
                    summarizedFile += readVec[0].Replace("_", ",") + "," + ((trn07 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((tst08 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((tpr29 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((fpr31 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((tnr33 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((fnr35 / summarySize).ToString().Substring(0, 5)+"00000").Substring(0, 5) + "," + (meanAUC.ToString()+"00000").Substring(0, 5) + "," + (stDevAUC.ToString()+"00000").Substring(0, 5) + "," + (ci.ToString()+"00000").Substring(0, 5) + "," + (meanMci+"00000").Substring(0, 5) + "," + (meanPci+"00000").Substring(0, 5) + "," + ((prec37 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((recl38 / summarySize).ToString()+"00000").Substring(0, 5) + "," + ((Fmsr39 / summarySize).ToString().Substring(0, 5)+"00000").Substring(0, 5) + "\n";
                    rowsCounter++;
                    counter = 0;
                    tpr29 = 0; fpr31 = 0; tnr33 = 0; fnr35 = 0; aucMeasure40 = 0;
                    prec37 = 0; recl38 = 0; Fmsr39 = 0;
                    doubList.Clear();
                }*/
            }
            tr.Close();
            CUMC_Handler.writeToFile(wekaResults.Replace(".csv", "_Summerized.csv"), summarizedFile); //TextWriter tw = new StreamWriter(wekaResults.Replace(".csv", "_Summerized.csv")); //tw.Write(summarizedFile); //tw.Close();
            
            tr = new StreamReader(tonceptsFile);
            readLine = tr.ReadLine();
            Dictionary<string, string> tonceptsTableDic = new Dictionary<string, string>();
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                readLine = tr.ReadLine();
                readVec = readLine.Split(',');
                if (readVec[0] == "total")
                    break;
                tr.ReadLine();  //readLine = readLine + ";" + tr.ReadLine();
                tonceptsTableDic.Add(readVec[0], readLine);
            }

            //string ortoResultsReport = "tonceptId,description,cohortSize,observTimeMin,observTimeMax,predTime,tirpSize, ShortBIN_mean,ShortBIN_stdv,TirpsBIN_mean,TirpsBIN_stdv,TirpsHS_mean,TirpsHSHNRM_stdv,TirpsHS_mean,TirpsHSHNRM_stdv,TirpsHSVNRM_mean,TirpsHSVNRM_stdv,TirpsHSIDF_mean,TirpsHSIDF_stdv,TirpsMND_mean,TirpsMND_stdv\n";
            string ortoResultsReport = "tonceptId,clinicRel,description,S_BIN_mn,S_BIN_std,T_BIN_mn,T_BIN_std,T_HS_mn,T_HSHNRM_std,T_HS_mn,T_HSHNRM_std,T_HSVNRM_mn,T_HSVNRM_std,T_HSIDF_mn,T_HSIDF_std,T_MND_mn,T_MND_stdv,T_MINOFST_mn,T_MINOFST_std,T_MEANOFST_mn,T_MEANOFST_std,,BIN,HS,HSHNRM,HSVNRM,HSIDF,MND,MINOFST,MEANOFST,maxDelta,maxDelRep\n";

            for (int i = 0; i < resultsDic.Count; i++)
            {
                string resultLine = resultsDic.ElementAt(i).Key + ","; // description, tripSize, Cohort .. 
                if (tonceptsTableDic.ContainsKey(resultsDic.ElementAt(i).Key))
                {
                    string[] strVec = tonceptsTableDic[resultsDic.ElementAt(i).Key].Split(',');
                    resultLine = resultLine + strVec[1] + "," + strVec[2] + ",";
                }
                else
                    resultLine = resultLine + ",,";
                double shrtBINmn = 0, mean = 0, stDev = 0, maxDiff = 0;
                string maxDiffName = "";
                shrtBINmn = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.ShortBIN); // SHORT BIN
                resultLine += (shrtBINmn.ToString()+"000").Substring(0,5) + "," + (stDev.ToString()+"000").Substring(0,5) + ","; 

                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsBIN); // TIRPS BIN
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ",";
                maxDiff = (mean - shrtBINmn); maxDiffName = "BIN";
                string compare_means = (((mean - shrtBINmn)*100).ToString() + "00000").Substring(0, 5) + ",";
                
                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsHS); // TIRPS HS
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ",";
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "HS"; }
                compare_means = compare_means + (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";

                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsHSHNRM); // TIRPS HSHNRM
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ","; 
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "HSHNRM"; }
                compare_means = compare_means + (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";

                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsHSVNRM); // TIRPS HSVNRM
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ",";
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "HSVNRM"; }
                compare_means = compare_means + (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";

                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsHSIDF); // TIRPS HSIDF
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ",";
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "HSIEF"; }
                compare_means = compare_means + (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";
                
                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsMND);
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ","; // TIRPS MND
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "MND"; }
                compare_means = compare_means + trimDoubleToString(((mean - shrtBINmn) * 100).ToString(), 5) + ","; // (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";

                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsMINOFST);
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ","; // TIRPS MND
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "MINOFST"; }
                compare_means = compare_means + trimDoubleToString(((mean - shrtBINmn) * 100).ToString(), 5) + ","; // (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";

                mean = CUMC_Handler.calculateMeanStDev(ref stDev, resultsDic.ElementAt(i).Value.TirpsMEANOFST);
                resultLine += (mean.ToString() + "000").Substring(0, 5) + "," + (stDev.ToString() + "000").Substring(0, 5) + ","; // TIRPS MND
                if ((mean - shrtBINmn) > maxDiff)
                { maxDiff = (mean - shrtBINmn); maxDiffName = "MEANOFST"; }
                compare_means = compare_means + trimDoubleToString(((mean - shrtBINmn) * 100).ToString(), 5) + ","; // (((mean - shrtBINmn) * 100).ToString() + "00000").Substring(0, 5) + ",";

                ortoResultsReport += resultLine + "," + compare_means + "," + trimDoubleToString((maxDiff * 100).ToString(), 5) + "," + maxDiffName + "\n"; // ((maxDiff * 100).ToString() + "000").Substring(0, 5) + "," + maxDiffName + "\n";
            }
            CUMC_Handler.writeToFile(wekaResults.Replace(".csv", "_ortoSummary.csv"), ortoResultsReport); //TextWriter tw = new StreamWriter(wekaResults.Replace(".csv", "_Summerized.csv")); //tw.Write(summarizedFile); //tw.Close();
            
        }

        public static string trimDoubleToString(string number, int trimSize)
        {
            string zeros = "";
            for (int i = 0; i < trimSize; i++)
                zeros += "0";
            return (number + zeros).Substring(0, trimSize);
        }

        public static void runMultiFoldersCreateARFFfilesToARFFolder(string sourceDataDir, string startsWith, string endsWith, int tirpSize, int multiplyFeatures, string arfFilesOrSparsity)
        {
            sourceDataDir = sourceDataDir.Replace("sourceData", "");
            ConcurrentBag<string> concurrentList = new ConcurrentBag<string>();
             
            
            Directory.CreateDirectory(sourceDataDir + startsWith + endsWith);
            string[] directs = Directory.GetDirectories(sourceDataDir);
            //foreach (string dirPath in directs)
            Parallel.ForEach(Directory.GetDirectories(sourceDataDir), dirPath =>
            {
                if (dirPath.Contains(startsWith) && !dirPath.Contains("sourceData") && !dirPath.Contains(endsWith) && dirPath.EndsWith(startsWith))
                {
                    string[] tonceptDirs = Directory.GetDirectories(dirPath);
                    //foreach (string tonceptDirPath in tonceptDirs)
                    Parallel.ForEach(Directory.GetDirectories(dirPath), tonceptDirPath =>
                    {
                        if (!tonceptDirPath.Contains(endsWith)) // "arfFiles"))
                        {
                            string[] files = Directory.GetFiles(tonceptDirPath);
                            //foreach (string matrixFilePath in files)
                            Parallel.ForEach(Directory.GetFiles(tonceptDirPath), matrixFilePath =>
                            {
                                if (matrixFilePath.Contains("MATRIX") && matrixFilePath.Contains(".csv"))
                                    concurrentList.Add( oneClassFeatureSelectAndCreaterARFFfiles(matrixFilePath, sourceDataDir + startsWith + endsWith, tirpSize, multiplyFeatures, arfFilesOrSparsity) );
                                //selectTopFeaturesAndCreaterARFFfiles(matrixFilePath, sourceDataDir + startsWith + endsWith); //createrARFFfiles(matrixFilePath, sourceDataDir + startsWith + "_arfFiles");
                            });
                        }
                    });
                }
            });

            if (arfFilesOrSparsity == "sparsity")
            {
                Dictionary<string, string> tonceptSparsityReportsDic = new Dictionary<string, string>();
                for (int i = 0; i < concurrentList.Count; i++)
                {
                    string[] sparsityReports = concurrentList.ElementAt(i).Split('-');
                    if (tonceptSparsityReportsDic.ContainsKey(sparsityReports[0]))
                        tonceptSparsityReportsDic[sparsityReports[0]] = tonceptSparsityReportsDic[sparsityReports[0]] + "+" + sparsityReports[1] + "-" + sparsityReports[2];
                    else
                        tonceptSparsityReportsDic.Add(sparsityReports[0], sparsityReports[1] + "-" + sparsityReports[2]);  
                }
                string sparsityReport = "tonceptId,1:1sqrt,2,3,4,5,6,7,8,9,10,T:1sqrt,2,3,4,5,6,7,8,9,10\n";
                for (int i = 0; i < tonceptSparsityReportsDic.Count; i++)
                {
                    string tonceptId = tonceptSparsityReportsDic.ElementAt(i).Key, sparsity1string = "", sparsityTstring = "";
                    string[] sparsity1, sparsityT, matrices = tonceptSparsityReportsDic.ElementAt(i).Value.Split('+');
                    double[] sparsity1vals, sparsityTvals;
                    sparsity1 = matrices[0].Split('-')[0].Split(',');
                    sparsityT = matrices[0].Split('-')[1].Split(',');
                    
                    //int sparsity1size = Math.Min(sparsity1.Length, 10), sparsityTsize = Math.Min(sparsityT.Length, 10);
                    int sparsity1size = 10, sparsityTsize = 10;
                    for (int m = 0; m < matrices.Length; m++)
                    {
                        if (matrices[m].Split('-')[0].Split(',').Length-1 < sparsity1size)
                            sparsity1size = (matrices[m].Split('-')[0].Split(',').Length-1);
                        if (matrices[m].Split('-')[1].Split(',').Length-1 < sparsityTsize)
                            sparsityTsize = (matrices[m].Split('-')[1].Split(',').Length-1);
                    }
                    
                    sparsity1vals = new double[sparsity1size];
                    for (int s = 0; s < sparsity1size; s++)
                        sparsity1vals[s] = double.Parse(sparsity1[s]);
                    sparsityTvals = new double[sparsityTsize];
                    for (int s = 0; s < sparsityTsize; s++)
                        sparsityTvals[s] = double.Parse(sparsityT[s]);

                    for (int m = 1; m < matrices.Length; m++)
                    {
                        sparsity1 = matrices[m].Split('-')[0].Split(',');
                        sparsityT = matrices[m].Split('-')[1].Split(',');
                    
                        for (int s = 0; s < sparsity1size; s++)
                            sparsity1vals[s] += double.Parse(sparsity1[s]);
                        for (int s = 0; s < sparsityTsize; s++)
                            sparsityTvals[s] += double.Parse(sparsityT[s]);
                    }

                    for (int s = 0; s < sparsity1size; s++)
                    {
                        sparsity1vals[s] = sparsity1vals[s] / matrices.Length;
                        sparsity1string += (sparsity1vals[s].ToString()+"000").Substring(0,5) + ",";
                    }
                    for (int s = 0; s < sparsityTsize; s++)
                    {
                        sparsityTvals[s] = sparsityTvals[s] / matrices.Length;
                        sparsityTstring += (sparsityTvals[s].ToString() + "000").Substring(0, 5) + ",";
                    }

                    sparsityReport += tonceptId + "," + sparsity1string + sparsityTstring + "\n";
                }
                CUMC_Handler.writeToFile(sourceDataDir + startsWith + endsWith + "\\sparsityReport.csv", sparsityReport);

            }

        }

        public static string oneClassFeatureSelectAndCreaterARFFfiles(string matrixFileName, string arfFolderPath, int tirpSize, int multiplyFeatures, string arfFilesOrSparsity)
        {
            TextReader tr = new StreamReader(matrixFileName);
            string[] matrixFileProp = matrixFileName.Replace(".csv", "").Split('\\')[matrixFileName.Split('\\').Length - 1].Split('_');
            string fileDescription = "@RELATION " + matrixFileProp[0] + "_" + matrixFileProp[1].Replace("oT", "") + "_" + matrixFileProp[2].Replace("pT", "") + "_" + matrixFileProp[3].Replace("mTE", "") + "_" + matrixFileProp[4] + "_" + matrixFileProp[5] + "_" + matrixFileProp[6] + "_" + matrixFileProp[7].Replace("MATRIX", "") + "_" + matrixFileProp[9] + "_" + matrixFileProp[10].Replace("mhs", "") + "_" + matrixFileProp[12] + "_" + matrixFileProp[14] + "_" + matrixFileProp[16] + "_" + matrixFileProp[18];
            double cohortSize = double.Parse(matrixFileProp[5]);
            cohortSize = cohortSize * 2 / 3 * 2; // * 2 (cohort+control) / 3 folds * 2 folds (10XF)
            cohortSize = Math.Sqrt(cohortSize);
            int sqrtSize = Convert.ToInt32(cohortSize);
            //if (tirpSize > 0)
            //    featureSize = tirpSize;
            int featureSize = multiplyFeatures * sqrtSize; // remmeber this
            List<clsFeature> clsFeaturesList = new List<clsFeature>();

            string[] delimLine = tr.ReadLine().Split(',');
            for (int i = 4/*2*/; i < delimLine.Length - 4/*2*/; i = i + 4/*2*/)
            {
                clsFeature clsFtr = new clsFeature();
                clsFtr.tonceptId = int.Parse(matrixFileProp[0]);
                if (int.Parse(delimLine[i-2]) == 1)
                {
                    clsFtr.featureName = delimLine[i - 3/*1*/];
                    clsFtr.featureSize = 1;
                }
                else
                {
                    clsFtr.featureName = delimLine[i - 3/*1*/] + (i - 1).ToString();
                    clsFtr.featureSize = int.Parse(delimLine[i-2]);
                }
                clsFeaturesList.Add(clsFtr);
            }

            int noTirpsEntities = 0;
            int entitySize = 0;
            clsFeature classLabels = new clsFeature();
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                delimLine = tr.ReadLine().Split(',');
                int fIdx = 0, nonZeroTIRPs = 0;
                for (int i = 4/*2*/; i < delimLine.Length - 3; i = i + 4)
                {
                    clsFeaturesList[fIdx].entitiesHSValuesVector.Add(int.Parse(delimLine[i - 3/*1*/])); //HS
                    clsFeaturesList[fIdx].entitiesMeanDValuesVector.Add(double.Parse(delimLine[i-2]));  //MND
                    clsFeaturesList[fIdx].entitiesMinOffsetValuesVector.Add(int.Parse(delimLine[i - 1/*1*/])); //HS
                    clsFeaturesList[fIdx].entitiesMeanOffsetValuesVector.Add(double.Parse(delimLine[i - 0]));  //MND

                    if (clsFeaturesList[fIdx].entitiesHSValuesVector[entitySize] > 0)
                        nonZeroTIRPs++;
                    fIdx++;
                }
                if (nonZeroTIRPs == 0)
                    noTirpsEntities++;
                classLabels.entitiesHSValuesVector.Add(int.Parse(delimLine[delimLine.Length-1]));
                entitySize++;
            }
            //claculate vertical support
            double ceSize = entitySize / 2;
            for(int fIdx = 0; fIdx < clsFeaturesList.Count; fIdx++)
                for (int eIdx = 0; eIdx < ceSize; eIdx++)
                {
                    if (clsFeaturesList[fIdx].entitiesHSValuesVector[eIdx] > 0)
                        clsFeaturesList[fIdx].verticalSupport++;
                    if (clsFeaturesList[fIdx].maxHS    < clsFeaturesList[fIdx].entitiesHSValuesVector[eIdx])
                        clsFeaturesList[fIdx].maxHS    = clsFeaturesList[fIdx].entitiesHSValuesVector[eIdx];
                    if (clsFeaturesList[fIdx].maxMeanD < clsFeaturesList[fIdx].entitiesMeanDValuesVector[eIdx])
                        clsFeaturesList[fIdx].maxMeanD = clsFeaturesList[fIdx].entitiesMeanDValuesVector[eIdx];
                }
//          for(int fIdx = 0; fIdx < clsFeaturesList.Count; fIdx++)
//              clsFeaturesList[fIdx].InverseTIRPFrequency = Math.Log(ceSize / clsFeaturesList[fIdx].verticalSupport);

            clsFeaturesList.Sort(clsFeature.compareVSAndNames); // run one class feature selection on the top 100 VSs
            /*string clsFeaturesListOrder = "";
            for (int i = 0; i < clsFeaturesList.Count; i++)
            {
                if(clsFeaturesList[i].featureSize == 1)
                    clsFeaturesListOrder += (clsFeaturesList[i].featureName + "," + clsFeaturesList[i].verticalSupport.ToString() + ",\n");
            }
            CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_ORDERED_PRINT_.csv"), clsFeaturesListOrder);
            */
            printFeatureSelectedArffFiles(clsFeaturesList, featureSize, "VrSpO", matrixFileName, arfFolderPath, fileDescription, noTirpsEntities, classLabels, ceSize);
            
            if (arfFilesOrSparsity == "sparsity") //report sparsity
            {
                int ftrCntr1 = 0, ftrCntrT = 0, sqrtJump = 1;
                double sparsity1 = 0, sparsityT = 0;
                string sparsityReport1 = "", sparsityReportT = "";
                for (int ftrIdx = 0; ftrIdx < clsFeaturesList.Count; ftrIdx++)
                {
                    if (clsFeaturesList[ftrIdx].featureSize == 1)
                    {
                        sparsity1 += (ceSize - clsFeaturesList[ftrIdx].verticalSupport) / ceSize;
                        ftrCntr1++;
                        if (ftrCntr1 % (sqrtJump * sqrtSize) == 0) // sqrtSize
                            sparsityReport1 += ((sparsity1 / ftrCntr1).ToString() + "000").Substring(0,5) + ","; // (ftrCntr1 / sqrtSize) + "," + ftrCntr1 + "," + (sparsity1 / ftrCntr1) + ";";
                    }
                    else
                    {
                        sparsityT += (ceSize - clsFeaturesList[ftrIdx].verticalSupport) / ceSize;
                        ftrCntrT++;
                        if (ftrCntrT % (sqrtJump * sqrtSize) == 0) // sqrtSize
                            sparsityReportT += ((sparsityT / ftrCntrT).ToString() + "000").Substring(0, 5) + ","; // (ftrCntrT / sqrtSize) + "," + ftrCntrT + "," + (sparsityT / ftrCntrT) + ";";
                    }

                }
                
                return matrixFileProp[0] + "-" + sparsityReport1 + "-" + sparsityReportT;
            }

            
            // perform feature selection
            /*
            int topVSTIRPsize = 100;
            List<clsFeature> topVsTIRPsList = new List<clsFeature>(); // List<int> topVSTIRPsList = new List<int>();
            for (int fIdx = 0; fIdx < clsFeaturesList.Count; fIdx++)
                if (clsFeaturesList[fIdx].featureSize > 1 && topVsTIRPsList.Count < topVSTIRPsize)
                    topVsTIRPsList.Add(clsFeaturesList[fIdx]);// fIdx);
            
            // pearson E calculation
            for (int scored_fIdx = 0; scored_fIdx < topVsTIRPsList.Count; scored_fIdx++)
                topVsTIRPsList[scored_fIdx].PearsonCorrelation = oneClassPearsonMeasure(scored_fIdx, (int)ceSize, topVsTIRPsList);//, clsFeaturesList);
                //clsFeaturesList[topVSTIRPsList[scored_fIdx]].PearsonCorrelation = oneClassPearsonMeasure(scored_fIdx, (int)ceSize, topVSTIRPsList, clsFeaturesList);

            // cosine E calculation
            double fullCosineE = oneClassCosineEMeasure(-1, (int)ceSize, topVsTIRPsList); //, clsFeaturesList);
            for (int scored_fIdx = 0; scored_fIdx < topVsTIRPsList.Count; scored_fIdx++)
                topVsTIRPsList[scored_fIdx].featureCosineE = oneClassCosineEMeasure(scored_fIdx, (int)ceSize, topVsTIRPsList);//, clsFeaturesList);

            // minStDev calculation
            //for (int scored_fIdx = 0; scored_fIdx < topVsTIRPsList.Count; scored_fIdx++)
            //    topVsTIRPsList[scored_fIdx].featureStDev = oneClassMinStDevMeasure(scored_fIdx, (int)ceSize, topVsTIRPsList);//, clsFeaturesList);
                //clsFeaturesList[topVSTIRPsList[scored_fIdx]].PearsonCorrelation = oneClassPearsonMeasure(scored_fIdx, (int)ceSize, topVSTIRPsList, clsFeaturesList);
            
            topVsTIRPsList.Sort(clsFeature.compareCosinesE);
                printFeatureSelectedArffFiles(topVsTIRPsList, featureSize, "CsinE", matrixFileName, arfFolderPath, fileDescription, noTirpsEntities, classLabels, ceSize);
            topVsTIRPsList.Sort(clsFeature.comparePearsonE);
                printFeatureSelectedArffFiles(topVsTIRPsList, featureSize, "PrsnC", matrixFileName, arfFolderPath, fileDescription, noTirpsEntities, classLabels, ceSize);
            //topVsTIRPsList.Sort(clsFeature.compareMinStDevE);
            //    printFeatureSelectedArffFiles(topVsTIRPsList, featureSize, "MinStD", matrixFileName, arfFolderPath, fileDescription, noTirpsEntities, classLabels, ceSize); 
            */
            return "";
        }
          
        public static void printFeatureSelectedArffFiles(List<clsFeature> clsFeaturesList, int featureSize, string selectionType, string matrixFileName, string arfFolderPath, string fileDescription, int noTirpsEntities, clsFeature classLabels, double ceSize)
        {
            //print files with these features
            int shortCnt = 0, tirpsCnt = 0, fullCnt = 0;
            List<int> shortIdsList = new List<int>(), tirpsIdsList = new List<int>(), fullIdsList = new List<int>();
            string shortAtts = "", tirpsAtts = "", fullAtts = "";
            for (int fIdx = 0; fIdx < clsFeaturesList.Count; fIdx++)
            {
                if (clsFeaturesList[fIdx].featureSize == 1 && shortCnt++ < featureSize)
                {
                    shortAtts += "@ATTRIBUTE \"" + clsFeaturesList[fIdx].featureName + "\" NUMERIC\n";
                    shortIdsList.Add(fIdx);
                }
                if (clsFeaturesList[fIdx].featureSize > 1 && tirpsCnt++ < featureSize)
                {
                    tirpsAtts += "@ATTRIBUTE \"" + clsFeaturesList[fIdx].featureName + "\" NUMERIC\n";
                    tirpsIdsList.Add(fIdx);
                }
                if (fullCnt++ < featureSize)
                {
                    fullAtts += "@ATTRIBUTE \"" + clsFeaturesList[fIdx].featureName + "\" NUMERIC\n";
                    fullIdsList.Add(fIdx);
                }
            }
            shortAtts += "@ATTRIBUTE class {0,1}\n@DATA\n"; tirpsAtts += "@ATTRIBUTE class {0,1}\n@DATA\n"; fullAtts += "@ATTRIBUTE class {0,1}\n@DATA\n";

            string fileName = matrixFileName.Split('\\')[matrixFileName.Split('\\').Length - 1];
            matrixFileName = arfFolderPath + "\\" + fileName;
            
            //write short files
            if(shortIdsList.Count > 0)
                oneClassFSWriteFiles(matrixFileName, fileDescription, noTirpsEntities, shortAtts, "SHORT_" + selectionType, shortIdsList, clsFeaturesList, classLabels, ceSize);
            oneClassFSWriteFiles(matrixFileName, fileDescription, noTirpsEntities, tirpsAtts, "TIRPS_" + selectionType, tirpsIdsList, clsFeaturesList, classLabels, ceSize);
            //oneClassFSWriteFiles(matrixFileName, fileDescription, noTirpsEntities, fullAtts,  "FULL",  fullIdsList,  clsFeaturesList, classLabels, entitySize); 
        }

        public static double oneClassMinStDevMeasure(int scored_fIdx, int ceSize, List<clsFeature> topVsTIRPsList) //, List<clsFeature> clsFeaturesList)
        {
            double mean = 0, stDev = 0;
            for (int i = 0; i < ceSize; i++)
                mean = mean + topVsTIRPsList[scored_fIdx].entitiesMeanDValuesVector[i];
            mean = mean / ceSize;
            for (int i = 0; i < ceSize; i++)
                stDev = stDev + Math.Pow(topVsTIRPsList[scored_fIdx].entitiesMeanDValuesVector[i] - mean, 2);
            stDev = stDev / ceSize;
            stDev = Math.Sqrt(stDev);
            return stDev;
        }

        public static double oneClassCosineEMeasure(int scored_fIdx, int ceSize, List<clsFeature> topVsTIRPsList) //, List<clsFeature> clsFeaturesList)
        {
            double CosineE = 0;
            for (int i = 0; i < ceSize; i++)
                for (int j = i+1; j < ceSize; j++)
                {
                    double SumIJ = 0, SumI = 0, SumJ = 0;
                    for (int fIdx = 0; fIdx < topVsTIRPsList.Count; fIdx++)
                    {
                        if (fIdx != scored_fIdx)
                        {
                            double hsI = topVsTIRPsList[fIdx].entitiesMeanDValuesVector[i]; // .entitiesHSValuesVector[i] * Math.Log( (double)ceSize / (double)topVsTIRPsList[fIdx].verticalSupport);
                            double hsJ = topVsTIRPsList[fIdx].entitiesMeanDValuesVector[j]; // .entitiesHSValuesVector[j] * Math.Log( (double)ceSize / (double)topVsTIRPsList[fIdx].verticalSupport);
                            SumIJ += hsI * hsJ;
                            SumI  += Math.Pow(hsI, 2);
                            SumJ  += Math.Pow(hsJ, 2);
                        }
                    }
                    if (SumI == 0)
                        SumI = 0.000000001;
                    if (SumJ == 0)
                        SumJ = 0.000000001;
                    double Sij = SumIJ / (Math.Sqrt(SumI) * Math.Sqrt(SumJ));
                    if (Sij != 0)
                        CosineE += Sij; // (Sij * Math.Log(Sij) + (1 - Sij) * Math.Log(1 - Sij));
                }
            int CNT = ((ceSize * (ceSize - 1)) / 2);
            CosineE = CosineE / CNT;
            return CosineE;
        }

        public static double oneClassPearsonMeasure(int x_fIdx, int ceSize, List<clsFeature> topVsTIRPsList) //, List<clsFeature> clsFeaturesList)
        {
            double PearsonE = 0;
            double avgX = 0;
            for (int i = 0; i < ceSize; i++)
                avgX += topVsTIRPsList[x_fIdx].entitiesMeanDValuesVector[i]; // .entitiesHSValuesVector[i] * Math.Log((double)ceSize / (double)topVsTIRPsList[x_fIdx].verticalSupport); ; // clsFeaturesList[x_fIdx].entitiesHSValuesVector[i];
            avgX = avgX / ceSize;
            int cnt = 0;
            for (int y_fIdx = 0; y_fIdx < topVsTIRPsList.Count; y_fIdx++)
            {
                if (y_fIdx != x_fIdx)
                {
                    double avgY = 0, SumXiXYiY = 0, sqrtXiX = 0, sqrtYiY = 0;
                    for (int i = 0; i < ceSize; i++)
                        avgY += topVsTIRPsList[y_fIdx].entitiesMeanDValuesVector[i]; // .entitiesHSValuesVector[i] * Math.Log((double)ceSize / (double)topVsTIRPsList[y_fIdx].verticalSupport); ; // clsFeaturesList[y_fIdx].entitiesHSValuesVector[i];
                    avgY = avgY / ceSize;

                    for (int i = 0; i < ceSize; i++)
                    {
                        // REMEMBER THE math.ABS 
                        double diffX = Math.Abs(topVsTIRPsList[x_fIdx].entitiesMeanDValuesVector[i] - avgX); // .entitiesHSValuesVector[i] - avgX;
                        double diffY = Math.Abs(topVsTIRPsList[y_fIdx].entitiesMeanDValuesVector[i] - avgY); // .entitiesHSValuesVector[i] - avgY;
                        SumXiXYiY += diffX * diffY; // (clsFeaturesList[x_fIdx].entitiesHSValuesVector[i] - avgX) * (clsFeaturesList[y_fIdx].entitiesHSValuesVector[i] - avgY);
                        sqrtXiX   += Math.Pow(diffX - avgX, 2); // clsFeaturesList[x_fIdx].entitiesHSValuesVector[i] - avgX, 2);
                        sqrtYiY   += Math.Pow(diffY - avgY, 2); // clsFeaturesList[y_fIdx].entitiesHSValuesVector[i] - avgY, 2);
                    }
                    sqrtXiX = Math.Sqrt(sqrtXiX);
                    sqrtYiY = Math.Sqrt(sqrtYiY);
                    double Pearson = SumXiXYiY / (sqrtXiX * sqrtYiY);
                    PearsonE += Math.Abs(Pearson);
                    cnt++;
                }
            }
            PearsonE = PearsonE / (topVsTIRPsList.Count - 1);
            return PearsonE;
        }

        public static double oneClassStDevMeasure(int x_fIdx, int ceSize, List<clsFeature> topVsTIRPsList) //, List<clsFeature> clsFeaturesList)
        {
            double stDev = 0;
            double avgX = 0;
            for (int i = 0; i < ceSize; i++)
                avgX += topVsTIRPsList[x_fIdx].entitiesMeanDValuesVector[i]; // .entitiesHSValuesVector[i] * Math.Log((double)ceSize / (double)topVsTIRPsList[x_fIdx].verticalSupport); ; // clsFeaturesList[x_fIdx].entitiesHSValuesVector[i];
            avgX = avgX / ceSize;
            
            for (int i = 0; i < ceSize; i++)
                stDev += Math.Pow( topVsTIRPsList[x_fIdx].entitiesMeanDValuesVector[i] - avgX, 2); // .entitiesHSValuesVector[i] * Math.Log((double)ceSize / (double)topVsTIRPsList[x_fIdx].verticalSupport); ; // clsFeaturesList[x_fIdx].entitiesHSValuesVector[i];
            stDev = stDev / ceSize;

            stDev = Math.Sqrt(stDev);

            return stDev;
        }

        public static void oneClassFSWriteFiles(string matrixFileName, string fileDescription, int noTIRPsEnts, string atts, string featuresType, List<int> fIdxList, List<clsFeature> featureList,  clsFeature clsLabels, double ceSize) //, ref string binLine, ref string hsLine, ref string hsVNormLine, ref string hsHNormLine, ref string meanDLine, ref string hsmnDLine)//, List<int> shortsIdx)
        {
            string binFile = "", hsFile = "", hsIdfFile = "", hsHNormFile = "", hsVNormFile = "", meanDFile = "", hsmnDFile = "", minoffsetFile = "", meanoffsetFile = "", hsHsVNrmHsIefFile = ""; 

            fileDescription = fileDescription.Replace("@RELATION ", "@RELATION " + noTIRPsEnts + "_" + fIdxList.Count + "_");
            fileDescription += "_" + featuresType; 

            for (int eIdx = 0; eIdx < (ceSize*2); eIdx++)
            {
                // set maxval per entity
                double eMaxHS = 0, eMaxMeanD = 0, meanHS = 0, meanMeanD = 0;
                for (int fIdx = 0; fIdx < fIdxList.Count; fIdx++)
                {
                    if (eMaxHS < featureList[fIdxList[fIdx]].entitiesHSValuesVector[eIdx])
                        eMaxHS = featureList[fIdxList[fIdx]].entitiesHSValuesVector[eIdx];
                    if (eMaxMeanD < featureList[fIdxList[fIdx]].entitiesMeanDValuesVector[eIdx])
                        eMaxMeanD = featureList[fIdxList[fIdx]].entitiesMeanDValuesVector[eIdx];
                    meanHS    = meanHS    + featureList[fIdxList[fIdx]].entitiesHSValuesVector[eIdx];
                    meanMeanD = meanMeanD + featureList[fIdxList[fIdx]].entitiesMeanDValuesVector[eIdx];
                }

                for (int fIdx = 0; fIdx < fIdxList.Count; fIdx++)
                {
                    double hsVal = featureList[fIdxList[fIdx]].entitiesHSValuesVector[eIdx];
                    if (hsVal == 0)
                        binFile += "0,";
                    else
                        binFile += "1,";
                    hsFile += hsVal + ",";
                    if (eMaxHS == 0)
                        hsHNormFile += "0,";
                    else
                        hsHNormFile += (hsVal / eMaxHS).ToString() + ",";
                    hsVNormFile += (hsVal / featureList[fIdxList[fIdx]].maxHS).ToString() + ",";
                    hsIdfFile += (hsVal * Math.Log( ceSize/featureList[fIdxList[fIdx]].verticalSupport)).ToString() + ",";
                    double meanDVal = featureList[fIdxList[fIdx]].entitiesMeanDValuesVector[eIdx];
                    meanDFile += meanDVal + ",";
                    hsmnDFile += (meanDVal * hsVal) + ",";
                    minoffsetFile += featureList[fIdxList[fIdx]].entitiesMinOffsetValuesVector[eIdx] + ",";
                    meanoffsetFile += featureList[fIdxList[fIdx]].entitiesMeanOffsetValuesVector[eIdx] + ",";
                    hsHsVNrmHsIefFile += hsVal + "," + (hsVal / featureList[fIdxList[fIdx]].maxHS).ToString() + "," + (hsVal * Math.Log( ceSize/featureList[fIdxList[fIdx]].verticalSupport)).ToString() + ",";
                }
                string clsLabel = clsLabels.entitiesHSValuesVector[eIdx].ToString() + "\n";
                binFile += clsLabel; hsFile += clsLabel; hsHNormFile += clsLabel; hsVNormFile += clsLabel; hsIdfFile += clsLabel; meanDFile += clsLabel; hsmnDFile += clsLabel; minoffsetFile += clsLabel; meanoffsetFile += clsLabel; hsHsVNrmHsIefFile += clsLabel;
            }
            
            CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_BIN"    + ".arff"), fileDescription + "_BIN\n"    + atts + binFile);
            if (featuresType.StartsWith("TIRPS"))
            {
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_HS"            + ".arff"), fileDescription + "_HS\n" + atts + hsFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_HSIDF"         + ".arff"), fileDescription + "_HSIDF\n" + atts + hsIdfFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_HSHNRM"        + ".arff"), fileDescription + "_HSHNRM\n" + atts + hsHNormFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_HSVNRM"        + ".arff"), fileDescription + "_HSVNRM\n" + atts + hsVNormFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_MND"           + ".arff"), fileDescription + "_MND\n" + atts + meanDFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_HSMND"         + ".arff"), fileDescription + "_HSMND\n" + atts + hsmnDFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_MINOFST"       + ".arff"), fileDescription + "_MINOFST\n" + atts + minoffsetFile);
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_MEANOFST"      + ".arff"), fileDescription + "_MEANOFST\n" + atts + meanoffsetFile);
                string triAtts = atts.Replace("@ATTRIBUTE class {0,1}\n@DATA\n", "");
                triAtts = triAtts.Replace("@ATTRIBUTE \"", "@ATTRIBUTE \"HS-") + " " + triAtts.Replace("@ATTRIBUTE \"", "@ATTRIBUTE \"HSVNRM-") + " " + triAtts.Replace("@ATTRIBUTE \"", "@ATTRIBUTE \"HSIEF-") + "@ATTRIBUTE class {0,1}\n@DATA\n";
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_" + featuresType + "_HSHSVNRMHSIEF" + ".arff"), fileDescription + "_HSHSVNRMHSIEF\n" + triAtts + hsHsVNrmHsIefFile);
            }
        }

        public static void selectTopFeaturesAndCreaterARFFfiles(string matrixFileName, string arfFolderPath)
        {
            int shortSize = 0, tirpSize = 0, fullSize = 0;
            List<int> shortTIRPs = new List<int>();
            TextReader tr = new StreamReader(matrixFileName);
            string[] matrixFileProp = matrixFileName.Replace(".csv", "").Split('\\')[matrixFileName.Split('\\').Length - 1].Split('_');
            string fileDescription = "@RELATION " + matrixFileProp[0] + "_" + matrixFileProp[1].Replace("oT", "") + "_" + matrixFileProp[2].Replace("pT", "") + "_" + matrixFileProp[3].Replace("mE", "") + "_" + matrixFileProp[4].Replace("mTE", "") + "_" + matrixFileProp[5] + "_" + matrixFileProp[6] + "_" + matrixFileProp[7] + "_" + matrixFileProp[8].Replace("MATRIX", "") + "_" + matrixFileProp[10] + "_" + matrixFileProp[11].Replace("mhs", "") + "_" + matrixFileProp[13] + "_" + matrixFileProp[15] + "_" + matrixFileProp[17];
            double cohortSize = double.Parse(matrixFileProp[5]);
            cohortSize = cohortSize * 2 / 3;
            cohortSize = Math.Sqrt(cohortSize);
            int featureSize = Convert.ToInt32(cohortSize);
            featureSize = 3 * featureSize; // remmeber this
            
            string[] shortFileHeaderVec = { fileDescription + "_BIN_short\n", fileDescription + "_HS_short\n", fileDescription + "_MEAND_short\n", fileDescription + "_HSNRM_short\n", fileDescription + "_HSMND_short\n" };
            string[] fullFileHeaderVec = { fileDescription + "_BIN_full\n", fileDescription + "_HS_full\n", fileDescription + "_MEAND_full\n", fileDescription + "_HSNRM_full\n", fileDescription + "_HSMND_full\n" };
            string[] tirpsFileHeaderVec = { fileDescription + "_BIN_tirps\n", fileDescription + "_HS_tirps\n", fileDescription + "_MEAND_tirps\n", fileDescription + "_HSNRM_tirps\n", fileDescription + "_HSMND_tirps\n" };

            string[] delimLine = tr.ReadLine().Split(',');
            for (int i = 2; i < delimLine.Length - 2; i = i + 2)
            {
                if (int.Parse(delimLine[i]) == 1)
                {
                    shortTIRPs.Add(i);
                    shortSize++;
                    if(shortSize < featureSize)
                        addStringToeachStringVec(ref shortFileHeaderVec, "@ATTRIBUTE \"" + delimLine[i - 1] /*"attr"  shortTIRPs.Count*/ + "\" NUMERIC\n");
                }
                else
                {
                    tirpSize++;
                    if(tirpSize < featureSize)
                        addStringToeachStringVec(ref tirpsFileHeaderVec, "@ATTRIBUTE \"" + delimLine[i - 1] + (i - 1).ToString() /*"attr" + (i - shortTIRPs.Count)*/ + "\" NUMERIC\n");
                }
                if(fullSize++ < featureSize)
                addStringToeachStringVec(ref fullFileHeaderVec, "@ATTRIBUTE \"" + delimLine[i - 1] + (i - 1).ToString()/*attr" + i*/ + "\" NUMERIC\n");
                //fullSize++;
            }
            //int[,] shortHSmatrix       = new int[cohortSize, shortSize], tirpsHSmatrix = new int[cohortSize, tirpSize], fullHSmatrix = new int[cohortSize, fullSize];
            //double[,] shortMeanDmatrix = double[cohortSize, shortSize], tirpsMeanDmatrix = new double[cohortSize, tirpSize], fullMeanDmatrix = new double[cohortSize, fullSize];
            //string[] shortNames = new string[shortSize], tirpsNames = new string[tirpSize];
            //double[,] shortMetrics = new double[3,shortSize], tirpsMetrics = new double[3, tirpSize]; 


            addStringToeachStringVec(ref shortFileHeaderVec, "@ATTRIBUTE class {0,1}\n");
            addStringToeachStringVec(ref tirpsFileHeaderVec, "@ATTRIBUTE class {0,1}\n");
            addStringToeachStringVec(ref fullFileHeaderVec, "@ATTRIBUTE class {0,1}\n");

            addStringToeachStringVec(ref shortFileHeaderVec, "@DATA\n");
            addStringToeachStringVec(ref tirpsFileHeaderVec, "@DATA\n");
            addStringToeachStringVec(ref fullFileHeaderVec, "@DATA\n");

            string[] shortFeaturesVec = { "", "", "", "", "" };
            string[] fullFeaturesVec = { "", "", "", "", "" };
            string[] tirpsFeaturesVec = { "", "", "", "", "" };
            int fullCnt = 0, tirpsCnt = 0;
            //int entityIdx = 0;
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                //entityIdx++;
                delimLine = tr.ReadLine().Split(',');
                string hsLine = "", binLine = "", hsNormLine = "", meanDLine = "", hsmnDLine = "";
                double maxVal = 0;
                //full
                fullCnt = 0;
                for (int i = 2; i < delimLine.Length - 1; i = i + 2)
                    if(fullCnt++ < featureSize)
                        addToLines(i - 1, i, ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref fullFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");
                //short
                maxVal = 0; hsLine = ""; binLine = ""; hsNormLine = ""; meanDLine = ""; hsmnDLine = "";
                for (int i = 0; i < shortTIRPs.Count; i++)
                //    shortHSmatrix[entityIdx, i] = int.Parse(delimLine[shortTIRPs[i]-1]); 
                    if( i < featureSize-1 )
                        addToLines(shortTIRPs[i] - 1, shortTIRPs[i], ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref shortFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");
                //tirps
                maxVal = 0; hsLine = ""; binLine = ""; hsNormLine = ""; meanDLine = ""; hsmnDLine = "";
                tirpsCnt = 0;
                for (int i = 2; i < delimLine.Length - 1; i = i + 2)
                    if (!shortTIRPs.Contains(i))
                    {
                        if (tirpsCnt++ < featureSize-1)
                            addToLines(i - 1, i, ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                    }
                addToFeatures(ref tirpsFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");
            }
            tr.Close();

            string[] tirpMetrics = { "BIN", "HS", "MND", "NRM", "HSMND" };
            string tonceptId = matrixFileName.Split('\\')[matrixFileName.Split('\\').Length - 2];
            string fileName = matrixFileName.Split('\\')[matrixFileName.Split('\\').Length - 1];
            matrixFileName = arfFolderPath + "\\" + fileName; // matrixFileName.Replace("\\" + tonceptId + "\\", "\\arffiles\\");
            for (int i = 0; i < fullFileHeaderVec.Length; i++)
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_FULL_" + tirpMetrics[i] + "_MATRIX.arff"), fullFileHeaderVec[i].Replace("@RELATION ", "@RELATION " + featureSize + "_") + fullFeaturesVec[i]);
            if (shortTIRPs.Count > 0)
            {
                //for (int i = 0; i < fullFileHeaderVec.Length; i++)
                int i = 0;
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_SHORT_" + tirpMetrics[i] + "_MATRIX.arff"), shortFileHeaderVec[i].Replace("@RELATION ", "@RELATION " + featureSize + "_") + shortFeaturesVec[i]);
            }
            for (int i = 0; i < fullFileHeaderVec.Length; i++)
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_TIRPS_" + tirpMetrics[i] + "_MATRIX.arff"), tirpsFileHeaderVec[i].Replace("@RELATION ", "@RELATION " + featureSize + "_") + tirpsFeaturesVec[i]);

        }
        
        /*
        public static void createrARFFfiles(string matrixFileName, string arfFolderPath)
        {
            int shortSize = 0, fullSize = 0;
            List<int> shortTIRPs = new List<int>();
            TextReader tr = new StreamReader(matrixFileName);
            string[] matrixFileProp = matrixFileName.Replace(".csv", "").Split('\\')[matrixFileName.Split('\\').Length - 1].Split('_');
            string fileDescription = "@RELATION " + matrixFileProp[0] + "_" + matrixFileProp[1].Replace("oT", "") + "_" + matrixFileProp[2].Replace("pT", "") + "_" + matrixFileProp[3].Replace("mE", "") + "_" + matrixFileProp[4].Replace("mTE", "") + "_" + matrixFileProp[5] + "_" + matrixFileProp[6] + "_" + matrixFileProp[7] + "_" + matrixFileProp[8].Replace("MATRIX", "") + "_" + matrixFileProp[10] + "_" + matrixFileProp[11].Replace("mhs", "") + "_" + matrixFileProp[13] + "_" + matrixFileProp[15] + "_" + matrixFileProp[17];
            string[] shortFileHeaderVec = { fileDescription + "_BIN_short\n", fileDescription + "_HS_short\n", fileDescription + "_MEAND_short\n", fileDescription + "_HSNRM_short\n", fileDescription + "_HSMND_short\n" };
            string[] fullFileHeaderVec = { fileDescription + "_BIN_full\n", fileDescription + "_HS_full\n", fileDescription + "_MEAND_full\n", fileDescription + "_HSNRM_full\n", fileDescription + "_HSMND_full\n" };
            string[] tirpsFileHeaderVec = { fileDescription + "_BIN_tirps\n", fileDescription + "_HS_tirps\n", fileDescription + "_MEAND_tirps\n", fileDescription + "_HSNRM_tirps\n", fileDescription + "_HSMND_tirps\n" };

            string[] delimLine = tr.ReadLine().Split(',');
            for (int i = 2; i < delimLine.Length - 2; i = i + 2)
            {
                if (int.Parse(delimLine[i]) == 1)
                {
                    shortTIRPs.Add(i);
                    shortSize++;
                    addStringToeachStringVec(ref shortFileHeaderVec, "@ATTRIBUTE \"" + delimLine[i-1]  + "\" NUMERIC\n");
                }
                else
                    addStringToeachStringVec(ref tirpsFileHeaderVec, "@ATTRIBUTE \"" + delimLine[i - 1] + (i-1).ToString() + "\" NUMERIC\n");
                addStringToeachStringVec(ref fullFileHeaderVec, "@ATTRIBUTE \"" + delimLine[i - 1] + (i - 1).ToString() + "\" NUMERIC\n");
                fullSize++;
            }
            addStringToeachStringVec(ref shortFileHeaderVec, "@ATTRIBUTE class {0,1}\n");
            addStringToeachStringVec(ref tirpsFileHeaderVec, "@ATTRIBUTE class {0,1}\n");
            addStringToeachStringVec(ref fullFileHeaderVec, "@ATTRIBUTE class {0,1}\n");

            addStringToeachStringVec(ref shortFileHeaderVec, "@DATA\n");
            addStringToeachStringVec(ref tirpsFileHeaderVec, "@DATA\n");
            addStringToeachStringVec(ref fullFileHeaderVec,  "@DATA\n");

            string[] shortFeaturesVec = { "", "", "", "", "" };
            string[] fullFeaturesVec = { "", "", "", "", "" };
            string[] tirpsFeaturesVec = { "", "", "", "", "" };
            while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
            {
                delimLine = tr.ReadLine().Split(',');
                string hsLine = "", binLine = "", hsNormLine = "", meanDLine = "", hsmnDLine = "";
                double maxVal = 0;
                //full
                for (int i = 2; i < delimLine.Length - 1; i = i + 2)
                    addToLines(i - 1, i, ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref fullFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");
                //short
                maxVal = 0; hsLine = ""; binLine = ""; hsNormLine = ""; meanDLine = ""; hsmnDLine = "";
                for (int i = 0; i < shortTIRPs.Count; i++)
                    addToLines(shortTIRPs[i] - 1, shortTIRPs[i], ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref shortFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");
                //tirps
                maxVal = 0; hsLine = ""; binLine = ""; hsNormLine = ""; meanDLine = ""; hsmnDLine = "";
                for (int i = 2; i < delimLine.Length - 1; i = i + 2)
                    if (!shortTIRPs.Contains(i))
                        addToLines(i - 1, i, ref maxVal, delimLine, ref binLine, ref hsLine, ref meanDLine, ref hsmnDLine);
                addToFeatures(ref tirpsFeaturesVec, binLine, hsLine, meanDLine, hsNormLine, hsmnDLine, maxVal, delimLine, delimLine[delimLine.Length - 1] + "\n");

            }
            tr.Close();

            string[] tirpMetrics = { "BIN", "HS", "MND", "NRM", "HSMND" };
            string tonceptId = matrixFileName.Split('\\')[matrixFileName.Split('\\').Length - 2];
            string fileName = matrixFileName.Split('\\')[matrixFileName.Split('\\').Length - 1];
            matrixFileName = arfFolderPath + "\\" + fileName; // matrixFileName.Replace("\\" + tonceptId + "\\", "\\arffiles\\");
            for (int i = 0; i < fullFileHeaderVec.Length; i++)
                CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_FULL_" + tirpMetrics[i] + "_MATRIX.arff"), fullFileHeaderVec[i].Replace("@RELATION ", "@RELATION " + fullSize + "_") + fullFeaturesVec[i]);
            if (shortTIRPs.Count > 0)
            {
                //for (int i = 0; i < fullFileHeaderVec.Length; i++)
                int i = 0;
                    CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_SHORT_" + tirpMetrics[i] + "_MATRIX.arff"), shortFileHeaderVec[i].Replace("@RELATION ", "@RELATION " + shortSize + "_") + shortFeaturesVec[i]);
            }
            //for (int i = 0; i < fullFileHeaderVec.Length; i++)
            //    CUMC_Handler.writeToFile(matrixFileName.Replace(".csv", "_TIRPS_" + tirpMetrics[i] + "_MATRIX.arff"), tirpsFileHeaderVec[i] + "_" + fullSize + tirpsFeaturesVec[i]);

        }*/
        
        public static void addStringToeachStringVec(ref string[] stringVec, string stringToAdd)
        {
            for (int i = 0; i < stringVec.Length; i++)
                stringVec[i] += stringToAdd;
        }

        public static void addToFeatures(ref string[] Features, string binLine, string hsLine, string meanDLine, string normLine, string hsmnDLine, double maxVal, string[] delimLine, string str)
        {
            Features[0] /*bin*/ += (binLine + str); // delimLine[delimLine.Length - 1] + "\n"); //binLine += delimLine[delimLine.Length - 1] + "\n";
            Features[1] /*hs*/  += (hsLine + str); // delimLine[delimLine.Length - 1] + "\n"); //hsLine += delimLine[delimLine.Length - 1] + "\n";
            Features[2] /*mnD*/ += (meanDLine + str); // delimLine[delimLine.Length - 1] + "\n"); //meanDLine += delimLine[delimLine.Length - 1] + "\n";
            string[] hsVals = hsLine.Split(',');
            if (maxVal > 0)
            {
                for (int i = 0; i < hsVals.Length - 1; i++)
                    normLine += ((double.Parse(hsVals[i]) / maxVal) + ",");
                Features[3] /*hsnrm*/ += (normLine + str); // delimLine[delimLine.Length - 1] + "\n"); //normLine += delimLine[delimLine.Length - 1] + "\n";
            }
            else
                Features[3] /*hsnrm*/ += (hsLine + str); // delimLine[delimLine.Length - 1] + "\n"); //normLine += delimLine[delimLine.Length - 1] + "\n";
            Features[4] /*hsmnd*/ += (hsmnDLine + str); // delimLine[delimLine.Length - 1] + "\n"); //hsmnDLine += delimLine[delimLine.Length - 1] + "\n";
        }

        public static void addToLines(int hsIdx, int mndIdx, ref double maxVal, string[] delimLine, ref string binLine, ref string hsLine, ref string meanDLine, ref string hsmnDLine)//, List<int> shortsIdx)
        {
            /*if(shortsIdx.Contains(hsIdx)
            {
                binLine += delimLine[hsIdx].ToString()+",";
                hsLine += delimLine[hsIdx].ToString()+",";
                meanDLine += delimLine[hsIdx].ToString()+",";
                hsmnDLine += delimLine[hsIdx].ToString()+",";
            }
            else*/
            {
                if (double.Parse(delimLine[hsIdx]) == 0)
                    binLine += "0,";
                else
                    binLine += "1,";
                //binLine += delimLine[hsIdx].ToString() + ",";
                hsLine += (delimLine[hsIdx] + ",");
                if (maxVal < double.Parse(delimLine[hsIdx]))
                    maxVal = double.Parse(delimLine[hsIdx]);
                meanDLine += (delimLine[mndIdx] + ",");
                hsmnDLine += ((double.Parse(delimLine[hsIdx]) * double.Parse(delimLine[mndIdx])) + ",");
            }
        }

        //if (abs.Contains('.');        abs = abs.Split('.')[0];
        //if (abs.Contains('D'))        abs = abs.Replace("D", "400");
        //if (abs.Contains('E'))        abs = abs.Replace("E", "500");
        //if (abs.Contains('V'))        abs = abs.Replace("V", "2200");
        //if (abs.Contains('G'))        abs = abs.Replace("G", "700");
        //if (!abstractIds.Contains(int.Parse(abs)))           abstractIds.Add(int.Parse(abs));
                    
        public static void tableTIRPsIntoCSV(string folderPath, string conceptNamesPath, string conditionsMap, string proceduresMap)
        {
            Dictionary<string, string> conceptNamesDic = new Dictionary<string, string>();
            TextReader tr = new StreamReader(conceptNamesPath);
            string readLine = tr.ReadLine();
            int counter = 0;
            while (tr.Peek() >= 0)
            {
                readLine = tr.ReadLine();
                string[] readVec = readLine.Split(',');
                conceptNamesDic.Add(readVec[0], readVec[1]);
                if (readVec[2] == "")
                   readLine =  tr.ReadLine();
                counter++;
            }
            tr.Close();
            
            Dictionary<string, List<string>> conditionsMapDic = new Dictionary<string, List<string>>();
            tr = new StreamReader(conditionsMap);
            readLine = tr.ReadLine();
            while (tr.Peek() >= 0)
            {
                readLine = tr.ReadLine();
                string[] readVec = readLine.Split(',');
                //tonceptsDic.Add(readVec[0], readVec[1]);
                string icd9 = readVec[1];
                if( icd9.Contains('.') )
                    icd9 = icd9.Split('.')[0];
                if (icd9.Contains('D'))
                    icd9 = icd9.Replace("D", "400");
                if (icd9.Contains('E'))
                    icd9 = icd9.Replace("E", "500");
                if (icd9.Contains('V'))
                    icd9 = icd9.Replace("V", "2200");
                if (icd9.Contains('G'))
                    icd9 = icd9.Replace("G", "700");

                string conceptName = "";
                if (conceptNamesDic.ContainsKey(readVec[0])) //icd9))
                    conceptName = conceptNamesDic[readVec[0]]; //icd9];
                else
                    conceptName = "";
                if (!conditionsMapDic.ContainsKey(icd9))
                {
                    List<string> stringList = new List<string>();
                    stringList.Add(readVec[0] + "-" + conceptName);
                    conditionsMapDic.Add(icd9, stringList);
                
                }
                else
                    conditionsMapDic[icd9].Add(readVec[0] + "-" + conceptName);
            }
            tr.Close();
            
            Dictionary<string, List<string>> proceduresMapDic = new Dictionary<string, List<string>>();
            tr = new StreamReader(proceduresMap);
            readLine = tr.ReadLine();
            while (tr.Peek() >= 0)
            {
                readLine = tr.ReadLine();
                string[] readVec = readLine.Split(',');
                //tonceptsDic.Add(readVec[0], readVec[1]);
                string icd9 = readVec[1].Split('.')[0];
                if (icd9.Contains('D'))
                    icd9 = icd9.Replace("D", "400");
                if (icd9.Contains('E'))
                    icd9 = icd9.Replace("E", "500");
                if (icd9.Contains('V'))
                    icd9 = icd9.Replace("V", "2200");
                if (icd9.Contains('G'))
                    icd9 = icd9.Replace("G", "700");

                string conceptName = "";
                if (conceptNamesDic.ContainsKey(readVec[0]))
                    conceptName = conceptNamesDic[readVec[0]];

                if (!proceduresMapDic.ContainsKey(icd9))
                {
                    List<string> stringList = new List<string>();
                    stringList.Add(readVec[0] + "-" + conceptName);
                    proceduresMapDic.Add(icd9, stringList);
                }
                else
                    proceduresMapDic[icd9].Add(readVec[0] + "-" + conceptName);
            }
            tr.Close();

            string[] files = Directory.GetFiles(folderPath);
            foreach (string filePath in files)
            //Parallel.ForEach(Directory.GetFiles(tonceptDirPath), matrixFilePath =>
            {
                //string prevFilePath = filePath;
                //string[] fileVec = filePath.Split('\\');
                //filePath.Replace(fileVec[fileVec.Length - 1], "forClustering\\" + fileVec[fileVec.Length - 1].Replace("mxTrpSze_3", ""));
                //File.Replace(prevFilePath, filePath, filePath);
                nameTIRPsConceptsFile(filePath, conditionsMapDic, proceduresMapDic, conceptNamesDic);

            }//);

        }
        
        private static void nameTIRPsConceptsFile(string arfFile, Dictionary<string, List<string>> conditionsMapDic, Dictionary<string, List<string>> proceduresMapDic, Dictionary<string, string> conceptNamesDic)
        {
            string toFile = "", symbNames = "", relations = "";
            TextReader tr = new StreamReader(arfFile);
            string readLine = tr.ReadLine();
            while (!readLine.Contains("class"))
            {
                readLine = tr.ReadLine();
                readLine = readLine.Replace("@ATTRIBUTE \"", "");
                readLine = readLine.Replace("\" NUMERIC", "");
                string[] symbsVec = readLine.Split('-');
                string[] relsVec = symbsVec[symbsVec.Length - 1].Split('r');
                toFile += readLine.Replace(relsVec[relsVec.Length-1], "") + "\n";
                symbNames = "";
                for (int i = 0; i < symbsVec.Length-1; i++)
                {
                    List<string> listStringPtr = null;
                    if (conditionsMapDic.ContainsKey(symbsVec[i]))
                        listStringPtr = conditionsMapDic[symbsVec[i]];
                    else if( proceduresMapDic.ContainsKey(symbsVec[i]))
                            listStringPtr = proceduresMapDic[symbsVec[i]];
                    else 
                    {
                        if(conceptNamesDic.ContainsKey(symbsVec[i]))
                        {
                            listStringPtr = new List<string>();
                            listStringPtr.Add(symbsVec[i] + "-" + conceptNamesDic[symbsVec[i]]);
                        }
                    }
                    for (int j = 0; j < listStringPtr.Count; j++)
                        symbNames += listStringPtr[j] + " OR ";
                    symbNames += "\n";
                }
                relations = "";
                for (int i = 0; i < relsVec.Length - 1; i++)
                {
                    string rel = "";
                    if (relsVec[i] == "0")
                        rel = "BEFORE";
                    else
                        if (relsVec[i] == "1")
                            rel = "OVERLAP";
                        else
                            if (relsVec[i] == "2")
                                rel = "CONTAIN";
                    relations = relations += rel + " AND ";
                }
                toFile += symbNames + relations + "\n\n";    
            }
            string tonceptId = arfFile.Split('\\')[arfFile.Split('\\').Length - 1].Split('_')[0];
            string tonceptName = conceptNamesDic[tonceptId];
            arfFile.Replace(tonceptId, tonceptName);
            CUMC_Handler.writeToFile(arfFile.Replace(".arff", "____symbNames.csv"), toFile);
            
        }

        public static void changeFileNamesInFolder(string folderPath, string replaceFolder)
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string filePath in files)
            //Parallel.ForEach(Directory.GetFiles(tonceptDirPath), matrixFilePath =>
            {
                string prevFilePath = filePath;
                string[] fileVec = filePath.Split('\\');
                filePath.Replace(fileVec[fileVec.Length-1], "forClustering\\" + fileVec[fileVec.Length-1].Replace("mxTrpSze_3", ""));
                File.Replace(prevFilePath, filePath, filePath);

            }//);
        }

    }
}
