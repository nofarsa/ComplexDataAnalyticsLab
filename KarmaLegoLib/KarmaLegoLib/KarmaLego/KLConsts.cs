using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//using Tonception;

namespace KarmaLegoLib
{
    public class KLC
    {
        public const int dharma_relVecSymVecSymDic  = 1; 
        public const int dharma_relVecSymSymDics    = 2;
        public const int dharma_relSymSymDics       = 3;

        public static bool parallelMining = true;
        public const bool nonParallelMining = false;

        public const int forwardMining           = 1;   // KarmaLego = 1;
        public const int backwardsMining         = 2; // KarmaLogi = 2;

        public const int KL_PRINT_TIRPS           = 3; // print only TIRPs instances
        public const int KL_PRINT_TONCEPTANDTIRPS = 2; // print toncepts and TIRPs instances 
        public const int KL_PRINT_NO_INSTANCES    = 1; // print without instances
        public const int KL_PRINT_NO              = 0; // no prtinting!

        public static bool KL_TRANS_YES           = true;
        public static bool KL_TRANS_NO            = false;

        public static int NUM_OF_SYMBOLS          = 2000; //150; //00; //150;
        public const int NUM_OF_ENTITIES         = 10000;
        public static int MAX_TIRP_TONCEPTS_SIZE  = 10;
        public static int MAX_TIRP_RELATIONS_SIZE = 45;

        public static int ERROR_NO_RELATION       = -1;

        public const int RELSTYLE_ALLEN7          = 7;
        public const int ALLEN_BEFORE             = 0; // <
        public const int ALLEN_MEET               = 1;        // m
        public const int ALLEN_OVERLAP            = 2;     // o
        public const int ALLEN_FINISHBY           = 3;    // fi
        public const int ALLEN_CONTAIN            = 4;     // c 
        public const int ALLEN_EQUAL              = 5;       // =	
        public const int ALLEN_STARTS             = 6;      // S	
        public const string ALLEN7_RELCHARS       = "<mofc=s-";

        public const int RELSTYLE_KL3             = 3;
        public const int KL_BEFORE                = 0;     // <
        public const int KL_OVERLAP               = 1;    // O
        public const int KL_CONTAIN               = 2;    // C
        public const string KL3_RELCHARS          = "<OC-----";

        public static int RELSTYLE_KL2            = 2;
        //public static int KL_BEFORE = 0;      // <m
        public const int KL_OVERCONTAIN           = 1;    // OC
        public static string KL2_RELCHARS         = "<C------";

        public static string LoadTransitionTableKL3(ref int[][][] transition)
        {
            int left, top;
            transition = new int[KLC.RELSTYLE_KL3][][];
            for (left = 0; left < KLC.RELSTYLE_KL3; left++)
            {
                transition[left] = new int[KLC.RELSTYLE_KL3][];
                for (top = 0; top < KLC.RELSTYLE_KL3; top++)
                    transition[left][top] = new int[2];
            }

            left = KLC.KL_BEFORE; // 0
            for (top = KLC.KL_BEFORE; top <= KLC.KL_CONTAIN; top++)
                setTransitions(ref transition, left, top, KLC.KL_BEFORE, KLC.KL_BEFORE);

            left = KLC.KL_OVERLAP; // 1;
            setTransitions(ref transition, left, KLC.KL_BEFORE, KLC.KL_BEFORE, KLC.KL_BEFORE);
            setTransitions(ref transition, left, KLC.KL_OVERLAP, KLC.KL_BEFORE, KLC.KL_OVERLAP);
            setTransitions(ref transition, left, KLC.KL_CONTAIN, KLC.KL_BEFORE, KLC.KL_CONTAIN);

            left = KLC.KL_CONTAIN; // 2;
            for (top = KLC.KL_BEFORE; top <= KLC.KL_CONTAIN; top++)
                setTransitions(ref transition, left, top, KLC.KL_BEFORE, KLC.KL_CONTAIN);

            return tesTransitionKL3(transition);
        }

        private static string tesTransitionKL3(int[][][] trans)
        {
            string retVal = "";
            /*KLC.KL_BEFORE*/
            for (int top = KLC.KL_BEFORE; top <= KLC.KL_CONTAIN; top++)
                if (!(trans[KLC.KL_BEFORE][top][0] == KLC.KL_BEFORE && trans[KLC.KL_BEFORE][top][1] == KLC.KL_BEFORE))
                    retVal = retVal + KLC.KL_BEFORE.ToString() + top.ToString() + " ";

            /*KLC.KL_OVERLAP*/
            if (!(trans[KLC.KL_OVERLAP][KLC.KL_BEFORE][0] == KLC.KL_BEFORE && trans[KLC.KL_OVERLAP][KLC.KL_BEFORE][1] == KLC.KL_BEFORE))
                retVal = retVal + KLC.KL_OVERLAP.ToString() + KLC.KL_BEFORE.ToString() + " ";
            if (!(trans[KLC.KL_OVERLAP][KLC.KL_OVERLAP][0] == KLC.KL_BEFORE && trans[KLC.KL_OVERLAP][KLC.KL_OVERLAP][1] == KLC.KL_OVERLAP))
                retVal = retVal + KLC.KL_OVERLAP.ToString() + KLC.KL_BEFORE.ToString() + " ";
            if (!(trans[KLC.KL_OVERLAP][KLC.KL_CONTAIN][0] == KLC.KL_BEFORE && trans[KLC.KL_OVERLAP][KLC.KL_CONTAIN][1] == KLC.KL_CONTAIN))
                retVal = retVal + KLC.KL_OVERLAP.ToString() + KLC.KL_BEFORE.ToString() + " ";
            
            /*KLC.KL_CONTAIN*/
            for (int top = KLC.KL_BEFORE; top <= KLC.KL_CONTAIN; top++)
                if (!(trans[KLC.KL_CONTAIN][top][0] == KLC.KL_BEFORE && trans[KLC.KL_CONTAIN][top][1] == KLC.KL_CONTAIN))
                    retVal = retVal + KLC.KL_CONTAIN.ToString() + top.ToString() + " ";
            return retVal;

        }

        public static string LoadTransitionTableALLEN7(ref int[][][] transition)
        {
            int left, top;
            transition = new int[KLC.RELSTYLE_ALLEN7][][];
            for (left = 0; left < KLC.RELSTYLE_ALLEN7; left++)
            {
                transition[left] = new int[KLC.RELSTYLE_ALLEN7][];
                for (top = 0; top < KLC.RELSTYLE_ALLEN7; top++)
                    transition[left][top] = new int[2];
            }

            left = KLC.ALLEN_BEFORE; // 0
            for (top = KLC.ALLEN_BEFORE;  top <= KLC.ALLEN_STARTS; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_BEFORE, KLC.ALLEN_BEFORE);
         
            left = KLC.ALLEN_MEET; // 1;
            for (top = KLC.ALLEN_BEFORE;  top <= KLC.ALLEN_CONTAIN; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_BEFORE, KLC.ALLEN_BEFORE);
            for (top = KLC.ALLEN_EQUAL;   top <= KLC.ALLEN_STARTS; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_MEET, KLC.ALLEN_MEET);

            left = KLC.ALLEN_OVERLAP; // 2;
            setTransitions(ref transition, left, KLC.ALLEN_BEFORE,   KLC.ALLEN_BEFORE,  KLC.ALLEN_BEFORE);
            setTransitions(ref transition, left, KLC.ALLEN_MEET,     KLC.ALLEN_BEFORE,  KLC.ALLEN_BEFORE);
            setTransitions(ref transition, left, KLC.ALLEN_OVERLAP,  KLC.ALLEN_BEFORE,  KLC.ALLEN_OVERLAP);
            setTransitions(ref transition, left, KLC.ALLEN_FINISHBY, KLC.ALLEN_BEFORE,  KLC.ALLEN_OVERLAP);
            setTransitions(ref transition, left, KLC.ALLEN_CONTAIN,  KLC.ALLEN_BEFORE,  KLC.ALLEN_CONTAIN);
            setTransitions(ref transition, left, KLC.ALLEN_EQUAL,    KLC.ALLEN_OVERLAP, KLC.ALLEN_OVERLAP);
            setTransitions(ref transition, left, KLC.ALLEN_STARTS,   KLC.ALLEN_OVERLAP, KLC.ALLEN_OVERLAP);

            left = KLC.ALLEN_FINISHBY; // 3;
            for (top = KLC.ALLEN_BEFORE; top <=  KLC.ALLEN_CONTAIN; top++)
                setTransitions(ref transition, left, top, top, top);
            setTransitions(ref transition, left, KLC.ALLEN_EQUAL,    KLC.ALLEN_FINISHBY, KLC.ALLEN_FINISHBY);
            setTransitions(ref transition, left, KLC.ALLEN_STARTS,   KLC.ALLEN_OVERLAP, KLC.ALLEN_OVERLAP);

            left = KLC.ALLEN_CONTAIN; // 4;
            setTransitions(ref transition, left, KLC.ALLEN_BEFORE,   KLC.ALLEN_BEFORE,  KLC.ALLEN_CONTAIN);
            setTransitions(ref transition, left, KLC.ALLEN_MEET,     KLC.ALLEN_OVERLAP, KLC.ALLEN_CONTAIN);
            setTransitions(ref transition, left, KLC.ALLEN_OVERLAP,  KLC.ALLEN_OVERLAP, KLC.ALLEN_CONTAIN);
            for (top = KLC.ALLEN_FINISHBY;top <= KLC.ALLEN_EQUAL; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_CONTAIN, KLC.ALLEN_CONTAIN);
            setTransitions(ref transition, left, KLC.ALLEN_STARTS,   KLC.ALLEN_OVERLAP, KLC.ALLEN_CONTAIN);

            left = KLC.ALLEN_EQUAL; // 5;
            for (top = KLC.ALLEN_BEFORE; top <=  KLC.ALLEN_STARTS; top++)
                setTransitions(ref transition, left, top, top, top);

            left = KLC.ALLEN_STARTS; // 6;
            for (top = KLC.ALLEN_BEFORE; top <=  KLC.ALLEN_MEET; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_BEFORE, KLC.ALLEN_BEFORE);
            for (top = KLC.ALLEN_OVERLAP; top <= KLC.ALLEN_FINISHBY; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_BEFORE, KLC.ALLEN_OVERLAP);
            setTransitions(ref transition, left, KLC.ALLEN_CONTAIN,  KLC.ALLEN_BEFORE, KLC.ALLEN_CONTAIN);
            for (top = KLC.ALLEN_EQUAL; top <=   KLC.ALLEN_STARTS; top++)
                setTransitions(ref transition, left, top, KLC.ALLEN_STARTS, KLC.ALLEN_STARTS);

            return tesTransitionALLEN7(transition);
        }

        private static string tesTransitionALLEN7(int[][][] trans)
        {
            string retVal = "";
            /*KLC.ALLEN_BEFORE*/
            for(int top = KLC.ALLEN_BEFORE; top <= KLC.ALLEN_STARTS ; top++)
                if (!(trans[KLC.ALLEN_BEFORE][top][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_BEFORE][top][1] == KLC.ALLEN_BEFORE))
                    retVal = retVal + KLC.ALLEN_BEFORE.ToString() + top.ToString() + " ";
            /*KLC.ALLEN_MEET*/
            for (int top = KLC.ALLEN_BEFORE; top <= KLC.ALLEN_CONTAIN; top++)
                if (!(trans[KLC.ALLEN_MEET][top][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_MEET][top][1] == KLC.ALLEN_BEFORE))
                    retVal = retVal + KLC.ALLEN_MEET.ToString() + top.ToString() + " ";
            for (int top = KLC.ALLEN_EQUAL; top <= KLC.ALLEN_STARTS; top++)
                if (!(trans[KLC.ALLEN_MEET][top][0] == KLC.ALLEN_MEET && trans[KLC.ALLEN_MEET][top][1] == KLC.ALLEN_MEET))
                    retVal = retVal + KLC.ALLEN_MEET.ToString() + top.ToString() + " ";
            /*KLC.ALLEN_OVERLAP*/
            for (int top = KLC.ALLEN_BEFORE; top <= KLC.ALLEN_MEET; top++)
                if (!(trans[KLC.ALLEN_OVERLAP][top][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_OVERLAP][top][1] == KLC.ALLEN_BEFORE))
                    retVal = retVal + KLC.ALLEN_OVERLAP.ToString() + top.ToString() + " ";
            for (int top = KLC.ALLEN_OVERLAP; top <= KLC.ALLEN_FINISHBY; top++)
                if (!(trans[KLC.ALLEN_OVERLAP][top][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_OVERLAP][top][1] == KLC.ALLEN_OVERLAP))
                    retVal = retVal + KLC.ALLEN_OVERLAP.ToString() + top.ToString() + " ";
            if (!(trans[KLC.ALLEN_OVERLAP][KLC.ALLEN_CONTAIN][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_OVERLAP][KLC.ALLEN_CONTAIN][1] == KLC.ALLEN_CONTAIN))
                retVal = retVal + KLC.ALLEN_OVERLAP.ToString() + KLC.ALLEN_CONTAIN.ToString() + " ";
            for (int top = KLC.ALLEN_EQUAL; top <= KLC.ALLEN_STARTS; top++)
                if (!(trans[KLC.ALLEN_OVERLAP][top][0] == KLC.ALLEN_OVERLAP && trans[KLC.ALLEN_OVERLAP][top][1] == KLC.ALLEN_OVERLAP))
                    retVal = retVal + KLC.ALLEN_OVERLAP.ToString() + top.ToString() + " ";
            for (int top = KLC.ALLEN_EQUAL; top <= KLC.ALLEN_STARTS; top++)
                if (!(trans[KLC.ALLEN_OVERLAP][top][0] == KLC.ALLEN_OVERLAP && trans[KLC.ALLEN_OVERLAP][top][1] == KLC.ALLEN_OVERLAP))
                    retVal = retVal + KLC.ALLEN_OVERLAP.ToString() + top.ToString() + " ";
            /*KLC.ALLEN_FINISHBY*/
            for (int top = KLC.ALLEN_BEFORE; top <= KLC.ALLEN_CONTAIN; top++)
                if (!(trans[KLC.ALLEN_FINISHBY][top][0] == top && trans[KLC.ALLEN_FINISHBY][top][1] == top))
                    retVal = retVal + KLC.ALLEN_FINISHBY.ToString() + top.ToString() + " ";
            if (!(trans[KLC.ALLEN_FINISHBY][KLC.ALLEN_EQUAL][0] == KLC.ALLEN_FINISHBY && trans[KLC.ALLEN_FINISHBY][KLC.ALLEN_EQUAL][1] == KLC.ALLEN_FINISHBY))
                retVal = retVal + KLC.ALLEN_FINISHBY.ToString() + KLC.ALLEN_EQUAL.ToString() + " ";
            if (!(trans[KLC.ALLEN_FINISHBY][KLC.ALLEN_STARTS][0] == KLC.ALLEN_OVERLAP && trans[KLC.ALLEN_FINISHBY][KLC.ALLEN_STARTS][1] == KLC.ALLEN_OVERLAP))
                retVal = retVal + KLC.ALLEN_FINISHBY.ToString() + KLC.ALLEN_STARTS.ToString() + " ";
            /*KLC.ALLEN_CONTAIN*/
            if (!(trans[KLC.ALLEN_CONTAIN][KLC.ALLEN_BEFORE][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_CONTAIN][KLC.ALLEN_BEFORE][1] == KLC.ALLEN_CONTAIN))
                retVal = retVal + KLC.ALLEN_CONTAIN.ToString() + KLC.ALLEN_BEFORE.ToString() + " ";
            for (int top = KLC.ALLEN_MEET; top <= KLC.ALLEN_OVERLAP; top++)
                if (!(trans[KLC.ALLEN_CONTAIN][top][0] == KLC.ALLEN_OVERLAP && trans[KLC.ALLEN_CONTAIN][top][1] == KLC.ALLEN_CONTAIN))
                    retVal = retVal + KLC.ALLEN_CONTAIN.ToString() + top.ToString() + " ";
            for (int top = KLC.ALLEN_FINISHBY; top <= KLC.ALLEN_EQUAL; top++)
                if (!(trans[KLC.ALLEN_CONTAIN][top][0] == KLC.ALLEN_CONTAIN && trans[KLC.ALLEN_CONTAIN][top][1] == KLC.ALLEN_CONTAIN))
                    retVal = retVal + KLC.ALLEN_CONTAIN.ToString() + top.ToString() + " ";
            if (!(trans[KLC.ALLEN_CONTAIN][KLC.ALLEN_STARTS][0] == KLC.ALLEN_OVERLAP && trans[KLC.ALLEN_CONTAIN][KLC.ALLEN_STARTS][1] == KLC.ALLEN_CONTAIN))
                retVal = retVal + KLC.ALLEN_CONTAIN.ToString() + KLC.ALLEN_STARTS.ToString() + " ";
            /*KLC.ALLEN_EQUAL*/
            for (int top = KLC.ALLEN_BEFORE; top <= KLC.ALLEN_STARTS; top++)
                if (!(trans[KLC.ALLEN_EQUAL][top][0] == top && trans[KLC.ALLEN_EQUAL][top][1] == top))
                    retVal = retVal + KLC.ALLEN_EQUAL.ToString() + top.ToString() + " ";
            /*KLC.ALLEN_STARTS*/
            for (int top = KLC.ALLEN_BEFORE; top <= KLC.ALLEN_MEET; top++)
                if (!(trans[KLC.ALLEN_STARTS][top][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_STARTS][top][1] == KLC.ALLEN_BEFORE))
                    retVal = retVal + KLC.ALLEN_STARTS.ToString() + top.ToString() + " ";
            for (int top = KLC.ALLEN_OVERLAP; top <= KLC.ALLEN_FINISHBY; top++)
                if (!(trans[KLC.ALLEN_STARTS][top][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_STARTS][top][1] == KLC.ALLEN_OVERLAP))
                    retVal = retVal + KLC.ALLEN_STARTS.ToString() + top.ToString() + " ";
            if (!(trans[KLC.ALLEN_STARTS][KLC.ALLEN_CONTAIN][0] == KLC.ALLEN_BEFORE && trans[KLC.ALLEN_STARTS][KLC.ALLEN_CONTAIN][1] == KLC.ALLEN_CONTAIN))
                retVal = retVal + KLC.ALLEN_STARTS.ToString() + KLC.ALLEN_FINISHBY.ToString() + " ";
            for (int top = KLC.ALLEN_EQUAL; top <= KLC.ALLEN_STARTS; top++)
                if (!(trans[KLC.ALLEN_STARTS][top][0] == KLC.ALLEN_STARTS && trans[KLC.ALLEN_STARTS][top][1] == KLC.ALLEN_STARTS))
                    retVal = retVal + KLC.ALLEN_STARTS.ToString() + top.ToString() + " ";
            return retVal;
            
        }

        private static void setTransitions(ref int[][][] transition, int left, int top, int set0, int set1)
        {
            transition[left][top][0] = set0; transition[left][top][1] = set1;
        }

        public static bool checkRelationAmongTwoTIs(TimeIntervalSymbol A, TimeIntervalSymbol B, int relation, int epsilon, int max_gap)
        {
            switch (relation)
            {
                case KLC.ALLEN_BEFORE: // ALBEFORE: //'b':
                    {
                        int BsMnsAe = B.startTime - A.endTime;
                        return (BsMnsAe > epsilon && BsMnsAe < max_gap);
                    }
                case KLC.ALLEN_MEET: // ALMEET: //'m':
                    {
                        int BsMnsAs = B.startTime - A.startTime, AeMnsBs = A.endTime - B.startTime, AeMnsBe = A.endTime - B.endTime;
                        return (BsMnsAs > epsilon && CheckEShivyonOperator(AeMnsBs, epsilon) && AeMnsBe < epsilon);
                    }
                case KLC.ALLEN_OVERLAP: // ALOVERLAP: //'o':
                    {
                        int BsMnsAs = B.startTime - A.startTime, AeMnsBs = A.endTime - B.startTime, AeMnsBe = A.endTime - B.endTime;
                        return (BsMnsAs > epsilon && AeMnsBs > epsilon && AeMnsBe < epsilon);
                    }
                case KLC.ALLEN_FINISHBY: // ALFINISHBY: //'F(fi)':
                    {
                        int BsMnsAs = B.startTime - A.startTime, AeMnsBe = A.endTime - B.endTime;
                        return (BsMnsAs > epsilon && CheckEShivyonOperator(AeMnsBe, epsilon));
                    }
                case KLC.ALLEN_CONTAIN: // ALCONTAIN: //'c':
                    {
                        int BsMnsAs = B.startTime - A.startTime, AeMnsBe = A.endTime - B.endTime;
                        return (BsMnsAs > epsilon && AeMnsBe > epsilon);
                    }
                case KLC.ALLEN_EQUAL: // ALEQUAL: //'e':
                    {
                        int BsMnsAs = B.startTime - A.startTime, AeMnsBe = A.endTime - B.endTime;
                        return (CheckEShivyonOperator(BsMnsAs, epsilon) && CheckEShivyonOperator(AeMnsBe, epsilon));
                    }
                case KLC.ALLEN_STARTS: // ALSTART: //'s':
                    {
                        int BsMnsAs = B.startTime - A.startTime, AeMnsBe = A.endTime - B.endTime;
                        return (CheckEShivyonOperator(BsMnsAs, epsilon) && AeMnsBe < epsilon);
                    }
            }
            return false;
        }

        public static int WhichRelationEpsilon(TimeIntervalSymbol A, TimeIntervalSymbol B, int relations_style, int epsilon, int max_gap) // int f_s, int f_e, int s_s, int s_e)
        { // A earlier lexicographically than B
            int relation = -1;
            int AeMnsBs = A.endTime - B.startTime, BsMnsAs = B.startTime - A.startTime;
            int AeMnsBe = A.endTime - B.endTime, BsMnsAe = B.startTime - A.endTime;

            if (BsMnsAe > epsilon && BsMnsAe < max_gap)
                relation = KLC.ALLEN_BEFORE;   // ALBEFORE;
            else if (BsMnsAs > epsilon && CheckEShivyonOperator(AeMnsBs, epsilon) && AeMnsBe < epsilon)
                relation = KLC.ALLEN_MEET;     // ALMEET;
            else if (BsMnsAs > epsilon && AeMnsBs > epsilon && AeMnsBe < epsilon)
                relation = KLC.ALLEN_OVERLAP;  // ALOVERLAP;
            else if (CheckEShivyonOperator(BsMnsAs, epsilon) && AeMnsBe < epsilon)
                relation = KLC.ALLEN_STARTS;   // ALSTARTS;
            else if (BsMnsAs > epsilon && AeMnsBe > epsilon)
                relation = KLC.ALLEN_CONTAIN;  // ALCONTAIN;
            else if (CheckEShivyonOperator(BsMnsAs, epsilon) && CheckEShivyonOperator(AeMnsBe, epsilon))
                relation = KLC.ALLEN_EQUAL;    // ALEQUAL;
            else if (BsMnsAs > epsilon && CheckEShivyonOperator(AeMnsBe, epsilon))
                relation = KLC.ALLEN_FINISHBY; // ALFINISHBY;

            if (relations_style == KLC.RELSTYLE_KL3)
            {
                if (relation == KLC.ALLEN_BEFORE || relation == KLC.ALLEN_MEET)
                    relation = KLC.KL_BEFORE;
                else if (relation == KLC.ALLEN_OVERLAP)
                    relation = KLC.KL_OVERLAP;
                else if (relation == KLC.ALLEN_STARTS || relation == KLC.ALLEN_CONTAIN || relation == KLC.ALLEN_FINISHBY || relation == KLC.ALLEN_EQUAL)
                    relation = KLC.KL_CONTAIN;
            }

            /*if (relations_style == KLC.KL2_RELATIONS)
            {
                if (relation == KLC.ALLEN_BEFORE)
                    relation = KLC.KL_BEFORE;
                else if (relation == KLC.ALLEN_MEET || relation == KLC.ALLEN_OVERLAP || relation == KLC.ALLEN_STARTS || relation == KLC.ALLEN_CONTAIN || relation == KLC.ALLEN_FINISHBY || relation == KLC.ALLEN_EQUAL)
                    relation = KLC.KL_OVERCONTAIN;
            }*/

            return relation; // 0;
        }

        private static bool CheckEShivyonOperator(int dif, int epsilon)
        {
            return (dif <= epsilon && dif >= -epsilon);
        }

        public static Dictionary<int, List<TimeIntervalSymbol>> read_KL_file(string filePath)
        {
            try
            {
                //if (!File.Exists(filePath))
                //    return null;
                Dictionary<int, List<TimeIntervalSymbol>> entityTISs = new Dictionary<int, List<TimeIntervalSymbol>>();
                TextReader tr = new StreamReader(filePath);
                string readLine = tr.ReadLine();
                //read the variables dictionary?
                while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
                {
                    readLine = tr.ReadLine();
                    if (readLine.StartsWith("NumberOfEntities"))
                        break; //int entitieSize = int.Parse(readLine.Split(' ')[1]);
                }

                while (tr.Peek() >= 0) //read the entities and their symbolic time intervals
                {
                    readLine = tr.ReadLine();
                    string[] mainDelimited = readLine.Split(';');
                    string entityID = mainDelimited[0].Split(',')[0];
                    List<TimeIntervalSymbol> tisList = new List<TimeIntervalSymbol>();
                    for (int i = 1; i < mainDelimited.Length - 1; i++)
                    {
                        string[] tisDelimited = mainDelimited[i].Split(',');
                        TimeIntervalSymbol tis = new TimeIntervalSymbol(int.Parse(tisDelimited[0]), int.Parse(tisDelimited[1]), int.Parse(tisDelimited[2]));
                        tisList.Add(tis);
                    }
                    entityTISs.Add(int.Parse(entityID), tisList);
                }
                tr.Close();

                return entityTISs;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return null;
            }
        }

        public static void compareTIRPfiles(string tirpsFile, string tirpsContainFile)
        {
            string missingKeys = "", missingVals = "";
            Dictionary<string, string> tirpsDic = loadTIRPsFromFile(tirpsFile);
            Dictionary<string, string> tirpsContainDic = loadTIRPsFromFile(tirpsContainFile);

            for (int tIdx = 0; tIdx < tirpsDic.Count; tIdx++)
            {
                string line = tirpsDic.Keys.ElementAt(tIdx);
                if (tirpsContainDic.ContainsKey(line))
                {
                    if (tirpsContainDic[line] != tirpsDic.Values.ElementAt(tIdx))
                        missingVals = missingVals + line + ";";
                }
                else
                    missingKeys = missingKeys + line + ";";
            }
            
        }

        private static Dictionary<string, string> loadTIRPsFromFile(string tirpsPath)
        {
            TextReader tr = new StreamReader(tirpsPath);
            Dictionary<string, string> stringsDic = new Dictionary<string, string>();
            while (tr.Peek() >= 0)
            {
                string line = tr.ReadLine();
                string[] delimitedLine = line.Split(' ');
                if (delimitedLine[0] == "1")
                {
                    stringsDic.Add(line, line);
                }
                else
                {
                    string instances = tr.ReadLine();
                    stringsDic.Add(line, instances);
                }
            }
            return stringsDic;
        }

    }

}
