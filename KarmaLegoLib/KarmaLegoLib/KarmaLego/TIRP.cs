using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
//using Tonception;

namespace KarmaLegoLib
{
    public class TimeIntervalSymbol : IComparable
    {
        public int startTime;
        public int endTime;
        public int symbol;

        public TimeIntervalSymbol(int setStartTime, int setEndTime, int setSymbol)
        {
            startTime = setStartTime;
            endTime = setEndTime;
            symbol = setSymbol;
        }

        public static int compareTIS(TimeIntervalSymbol A, TimeIntervalSymbol B) // A<B?
        {
            if (A.startTime < B.startTime)
                return -1;
            else if (A.startTime == B.startTime && A.endTime < B.endTime)
                return -1;
            else if (A.startTime == B.startTime && A.endTime == B.endTime && A.symbol < B.symbol)
                return -1;
            else
                return 1;
        }

        public int CompareTo(object obj)
        {
            if (obj is TimeIntervalSymbol)
            {
                TimeIntervalSymbol other = obj as TimeIntervalSymbol;
                if (startTime < other.startTime)
                    return -1;
                else if (startTime == other.startTime && endTime < other.endTime)
                    return -1;
                else if (startTime == other.startTime && endTime == other.endTime && symbol < other.symbol)
                    return -1;
                else
                    return 1;
            }
            else
                return 0;
        }
    }

    public class TemporalConcept : IComparable
    {
        public int tonceptID;
        public int tonceptINDEX;
        
        public string tonceptName;
        public string tonceptOrig;
        //private List<int> verticalSupport = new List<int>(); public void addEntity(int entityIdx) { if (!verticalSupport.Contains(entityIdx)) verticalSupport.Add(entityIdx); } //public bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
        int totalHorizontalSupport;
        //public List<string> entIdxKeyList = new List<string>();
        private Dictionary<int, List<TimeIntervalSymbol>> verticalHorizontalSupport = new Dictionary<int, List<TimeIntervalSymbol>>();
        public int getTotalHorizontalSupport() { return totalHorizontalSupport; }
        public double getMeanHorizontalSupport() 
        {
            double mhs = (double)totalHorizontalSupport / (double)getTonceptVerticalSupport();
            return mhs; 
        }

        public void addEntityTinstance(int entityIdx, TimeIntervalSymbol instance) 
        {
            if (!verticalHorizontalSupport.ContainsKey(entityIdx))
            {
                List<TimeIntervalSymbol> strList = new List<TimeIntervalSymbol>();
                strList.Add(instance);
                verticalHorizontalSupport.Add(entityIdx, strList);
            }
            else
            {
                if(!verticalHorizontalSupport[entityIdx].Contains(instance))
                    verticalHorizontalSupport[entityIdx].Add(instance);
            }
            totalHorizontalSupport++;
        }
        public int getTonceptVerticalSupport() { return verticalHorizontalSupport.Count(); } // verticalSupport.Count; }
        public Dictionary<int, List<TimeIntervalSymbol>> getTonceptHorizontalDic() { return verticalHorizontalSupport; } //public List<int> getTonceptEntitiesList() { return verticalSupport; }

        public TemporalConcept(int symbol, int setIndex)
        {
            tonceptID = symbol;
            tonceptINDEX = setIndex;
        }

        public int CompareTo(object obj)
        {
            if (obj is TemporalConcept)
            {
                TemporalConcept other = obj as TemporalConcept;
                return verticalHorizontalSupport.Count.CompareTo(other.getTonceptVerticalSupport()); // return verticalSupport.Count.CompareTo(other.getTonceptVerticalSupport());
            }
            else
                throw new ArgumentException("Object is not a TemporalConcept.");
        }
    }

    public class pairsMATRIXrelEntry
    {
        public Dictionary<string, List<TimeIntervalSymbol>> instancesDicList = new Dictionary<string, List<TimeIntervalSymbol>>();
        public List<int> verticalSupport = new List<int>(); public void addEntity(int entityIdx) { if (!verticalSupport.Contains(entityIdx)) verticalSupport.Add(entityIdx); } //public bool[] entitieSupport = new bool[KLC.NUM_OF_ENTITIES];
        public int horizontalSupport;
    }

    public class twoSizedTIRPsMATRIXEntry
    {
        public pairsMATRIXrelEntry[] prsMxRelVec;
        public twoSizedTIRPsMATRIXEntry(int relations_style)        { prsMxRelVec = new pairsMATRIXrelEntry[relations_style]; }
    }

    public class TIsInstance
    {
        public int entityIdx;
        public TimeIntervalSymbol[] tis;
        public int armadaTisIdx;

        public TIsInstance(TimeIntervalSymbol setFirst, TimeIntervalSymbol setSecond, int setEntityIdx, int setArmadaIdx = -1)
        {
            entityIdx = setEntityIdx;
            tis = new TimeIntervalSymbol[2];
            tis[0] = setFirst;
            tis[1] = setSecond;
            armadaTisIdx = setArmadaIdx;
        }

        public TIsInstance(int size, TimeIntervalSymbol[] tisVec, TimeIntervalSymbol lastTis, int setEntityIdx, int setArmadaIdx = -1)
        {
            entityIdx = setEntityIdx;
            tis = new TimeIntervalSymbol[size];
            for (int i = 0; i < size-1; i++)
                tis[i] = tisVec[i];
            tis[size - 1] = lastTis;
            armadaTisIdx = setArmadaIdx;
        }

        public TIsInstance(TimeIntervalSymbol[] setTisVec, int entID) // for SingleE11
        {
            tis = setTisVec;
            entityIdx = entID;
        }
    }

    public class TIRP
    {
        public int size;
        public int relSize;
        public int[] toncepts;
        public int[] rels;
        public string tncptsRels;
        public List<TIsInstance> tinstancesList = new List<TIsInstance>();
        public List<int> entitieVerticalSupport = new List<int>(); public void addEntity(int entityIdx) { if (!entitieVerticalSupport.Contains(entityIdx)) entitieVerticalSupport.Add(entityIdx); }
        public double meanHorizontalSupport;
        public double selScore_VS;
        public double selScore_MnHS;
        public double selScore_MnDr;

        public Dictionary<int, List<TIsInstance>> entInstancesDic = new Dictionary<int, List<TIsInstance>>(); // for the Single11 matrix build
        public int getEntityHS(int entityID) { return entInstancesDic[entityID].Count(); }
        // get mean duration

        public override int GetHashCode()
        {
            int hash = 13, relSize = (size*(size-1))/2;
            for (int i = 0; i < relSize; i++)
                hash = (hash * 7) + rels[i].GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            int relSize = (size*(size-1))/2;
            TIRP other = obj as TIRP;
            for(int rIdx = 0; rIdx < relSize; rIdx++)
                if(rels[rIdx] != other.rels[rIdx])
                    return false;
            return true;
        }

        public static int compareTIRPs(TIRP A, TIRP B)
        {
            for (int i = 0; i < A.size; i++)
                if (A.toncepts[i] < B.toncepts[i])
                    return -1;
            return 1;
        }

        public TIRP(int setSize)
        {
            size = setSize;
            toncepts = new int[size];
            if (size > 1)
                relSize = size * (size - 1) / 2;
            else
                relSize = 1;
            rels = new int[relSize];
        }

        public TIRP(TIRP tirp, int setNewSymbol, int seedNewRleation, int[] setCandRels)//, int karmalegologi)
        {
            size = tirp.size + 1;
            int relsSize = size * (size - 1) / 2;
            int btmRelIdx = relsSize - 1;
            int topRelIdx = (size - 1) * (size - 2) / 2;
            
            toncepts = new int[size];
            for (int tIdx = 0; tIdx < (size - 1); tIdx++)
                toncepts[tIdx] = tirp.toncepts[tIdx];
            toncepts[size - 1] = setNewSymbol;
            rels = new int[relsSize];
            for (int relIdx = 0; relIdx < topRelIdx ; relIdx++)
                rels[relIdx] = tirp.rels[relIdx];
            for (int relIdx = topRelIdx; relIdx < btmRelIdx; relIdx++)
                rels[relIdx] = setCandRels[relIdx - topRelIdx];
            rels[(relsSize - 1)] = seedNewRleation;
        }

        public TIRP(int setFirstSymbol, int setSecondSymbol, int setFirstRelation)
        {
            size        = 2;
            toncepts    = new int[2];
            toncepts[0] = setFirstSymbol;
            toncepts[1] = setSecondSymbol;
            rels        = new int[1];
            rels[0]     = setFirstRelation;
        }

        public double getVerticalSupport()
        {
            return (double)entInstancesDic.Count;
        }

        public double getMeanHorizontalSupport()
        {
            double hsSum = 0;
            for (int i = 0; i < entInstancesDic.Count; i++)
                hsSum = hsSum + entInstancesDic.Values.Count;
            return (hsSum / entInstancesDic.Count);
        }

        public double getSumHorizontalSupport()
        {
            double hsSum = 0;
            for (int i = 0; i < entInstancesDic.Count; i++)
                hsSum = hsSum + entInstancesDic.Values.Count;
            return hsSum;
        }

        public void returnListOfHSAndMDs(ref List<int> listOfHSs, ref List<double> listOfMDs)
        {
            for (int eIdx = 0; eIdx < entInstancesDic.Count; eIdx++)
            {
                listOfHSs.Add(entInstancesDic.ElementAt(eIdx).Value.Count);
                double meanDur = 0;
                for (int iIdx = 0; iIdx < entInstancesDic.ElementAt(eIdx).Value.Count; iIdx++)
                {
                    int lastEndTime = entInstancesDic.ElementAt(eIdx).Value.ElementAt(iIdx).tis[0].endTime;
                    for (int tiIdx = 1; tiIdx < entInstancesDic.ElementAt(eIdx).Value.ElementAt(iIdx).tis.Length; tiIdx++)
                        if (entInstancesDic.ElementAt(eIdx).Value.ElementAt(iIdx).tis[tiIdx].endTime > lastEndTime)
                            lastEndTime = entInstancesDic.ElementAt(eIdx).Value.ElementAt(iIdx).tis[tiIdx].endTime;
                    meanDur += lastEndTime - entInstancesDic.ElementAt(eIdx).Value.ElementAt(iIdx).tis[0].startTime;
                }
                meanDur = meanDur / entInstancesDic.ElementAt(eIdx).Value.Count;
                listOfMDs.Add(meanDur);
            }
        }

        public double returnMeanHSAndMND(ref double meanHS, ref double stdevHS, ref double stdevMND, ref int hsSize, ref int mndSize)
        {
            List<int>    hsList = new List<int>();
            List<double> mndList = new List<double>();
            returnListOfHSAndMDs(ref hsList, ref mndList);
            hsSize = hsList.Count;
            mndSize = mndList.Count;
            meanHS = CUMC_Handler.calculateMeanStDev(ref stdevHS, hsList);
            double meanMND = CUMC_Handler.calculateMeanStDev(ref stdevMND, mndList);

            return meanMND;
        }

        public void printTIRP(TextWriter tw, entityKarma[] entitiesKarmaVec, int karmalegologi, int[][] logiRelsInxs, int print, int relationStyle) // = KLC.KL_PRINT_TIRPS)
        {
            string writeLine = size + " ", tonceptList = "", relList = "";
            string relChars = "";
            if (relationStyle == KLC.RELSTYLE_ALLEN7)
                relChars = KLC.ALLEN7_RELCHARS;
            else
                relChars = KLC.KL3_RELCHARS;
            if (karmalegologi == KLC.forwardMining) // .KarmaLego)
                for (int i = 0; i < size; i++)
                    tonceptList = tonceptList + toncepts[i] + "-";
            else
                for (int i = size - 1; i >= 0; i--)
                    tonceptList = tonceptList + toncepts[i] + "-";
            writeLine = writeLine + tonceptList + " ";

            if (karmalegologi == KLC.forwardMining) // .KarmaLego)
                for (int rIdx = 0; rIdx < size * (size - 1) / 2; rIdx++)
                {
                    writeLine = writeLine + relChars[rels[rIdx]] + "."; //writeLine = writeLine + KLC.ALLEN7_RELCHARS[rels[rIdx]] + ".";
                    relList = relList + rels[rIdx];
                }
            else
                for (int rIdx = 0; rIdx < size * (size - 1) / 2; rIdx++)
                {
                    int logiRelIdx = logiRelsInxs[size - 2 /*3*/][rIdx];
                    writeLine = writeLine + relChars[rels[logiRelIdx]] + "."; //writeLine = writeLine + KLC.ALLEN7_RELCHARS[rels[logiRelIdx]] + ".";
                    relList = relList + rels[logiRelIdx];
                }

            writeLine = writeLine + " " + entitieVerticalSupport.Count + " " + (double)tinstancesList.Count / (double)entitieVerticalSupport.Count + " "; // tinstancesList.Count + " " + entitieVerticalSupport.Count + " ";
            if (print > KLC.KL_PRINT_NO_INSTANCES)
            {
                for (int ins = 0; ins < tinstancesList.Count; ins++)
                {
                    writeLine = writeLine + entitiesKarmaVec[tinstancesList[ins].entityIdx].entityID + " ";
                    if (karmalegologi == KLC.forwardMining) // .KarmaLego)
                        for (int ti = 0; ti < size; ti++)
                            writeLine = writeLine + "[" + tinstancesList[ins].tis[ti].startTime + "-" + tinstancesList[ins].tis[ti].endTime + "]";
                    else
                        for (int ti = size - 1; ti >= 0; ti--)
                            writeLine = writeLine + "[" + tinstancesList[ins].tis[ti].startTime + "-" + tinstancesList[ins].tis[ti].endTime + "]";
                    writeLine = writeLine + " ";
                }
            }
            tw.WriteLine(writeLine);
        }
        
        public void printTIRP( TextWriter tw, int[] entitiesVec, int karmalegologi, int[][] logiRelsInxs, int relationStyle)
        {
            string writeLine = size + " ";
            string relChars = "";
            if (relationStyle == KLC.RELSTYLE_ALLEN7)
                relChars = KLC.ALLEN7_RELCHARS;
            else
                relChars = KLC.KL3_RELCHARS;

            if (karmalegologi == KLC.forwardMining) // .KarmaLego)
                for (int i = 0; i < size; i++)
                    writeLine = writeLine + toncepts[i] + "-";
            else
                for (int i = size-1; i >= 0; i--)
                    writeLine = writeLine + toncepts[i] + "-";
            writeLine = writeLine + " ";
            if (karmalegologi == KLC.forwardMining) // .KarmaLego)
                for (int rIdx = 0; rIdx < size * (size - 1) / 2; rIdx++)
                    writeLine = writeLine + relChars[rels[rIdx]] + "."; //writeLine = writeLine + KLC.ALLEN7_RELCHARS[rels[rIdx]] + ".";
            else
                for (int rIdx = 0; rIdx < size * (size - 1) / 2; rIdx++)
                {
                    int logiRelIdx = logiRelsInxs[size - 2/*3*/][rIdx];
                    writeLine = writeLine + relChars[rels[logiRelIdx]] + ".";  //writeLine = writeLine + KLC.ALLEN7_RELCHARS[rels[logiRelIdx]] + ".";
                }
       
            writeLine = writeLine + " " + entitieVerticalSupport.Count + " " + (double)tinstancesList.Count / (double)entitieVerticalSupport.Count + " "; // tinstancesList.Count + " " + entitieVerticalSupport.Count + " ";
            for (int ins = 0; ins < tinstancesList.Count; ins++)
            {
                writeLine = writeLine + entitiesVec[tinstancesList[ins].entityIdx] + " ";
                if (karmalegologi == KLC.forwardMining) // .KarmaLego)
                    for (int ti = 0; ti < size; ti++)
                        writeLine = writeLine + "[" + tinstancesList[ins].tis[ti].startTime + "-" + tinstancesList[ins].tis[ti].endTime + "]";
                else
                    for (int ti = size-1; ti >= 0; ti--)
                        writeLine = writeLine + "[" + tinstancesList[ins].tis[ti].startTime + "-" + tinstancesList[ins].tis[ti].endTime + "]";
                writeLine = writeLine + " ";
            }
            tw.WriteLine(writeLine);
        }
    }
}
