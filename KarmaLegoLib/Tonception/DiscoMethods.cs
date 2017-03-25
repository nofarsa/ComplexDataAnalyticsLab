using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoStation
{
    class DiscoMethods
    {

    }

    
    // Describes a bin (discretization parameter)
    public class Bin
    {
        public long _ID;
        // description
        public string _label;
        // lower limit ( >= )
        public double _lowlimit;
        // higher limit  ( < )
        public double _highlimit;
        // Error1
        //public double _error1;


        public double mean;
        public double variance;
        public int size;

        public static double MinValue = -1000000;
        public static double MaxValue = 1000000;

        public Bin(long ID, double lowlimit)
        {
            _ID = ID;
            _label = ID.ToString();
            _lowlimit = lowlimit;
            mean = 0;
        } // Bin  constructor


        /// <summary>
        /// Bin constructor
        /// </summary>
        /// <param name="ID">bin's id</param>
        /// <param name="label">bin's label</param>
        /// <param name="lowlimit">lower limit of bin</param>
        /// <param name="highlimit">higher limit of bin</param>
        public Bin(long ID, string label, double lowlimit, double highlimit)
        {
            _ID = ID;
            _label = label;
            _lowlimit = lowlimit;
            _highlimit = highlimit;
        } // Bin  constructor




        /// <summary>
        /// returns a list of bins labels by given number of bins
        /// for example: NumOfBins = 3 -> returns {"Low", "Medium" , "High"}
        /// </summary>
        /// <param name="NumOfBins"></param>
        /// <returns></returns>
        public static string[] getBinsLabels(int NumOfBins, string[] BaseLabels)
        {
            int i = 2;
            string[] ans = new string[NumOfBins];
            if (NumOfBins < 2) return new string[] { "All" };
            ans[0] = BaseLabels[0];
            if (NumOfBins == 3)
                ans[1] = BaseLabels[1];
            else
                for (i = 1; i < NumOfBins - 1; i++)
                    ans[i] = BaseLabels[1] + i.ToString();
            ans[i] = BaseLabels[2];
            return ans;
        } // getBinsLabels

    } // Class Bin

    /// <summary>
    /// Every Discretization method has to implement this interface!
    /// </summary>
    public interface DiscretizationMethod
    {
        long getID();
        void setID(long ID);
        //Parameters getParameters();
        //void setParameters(Parameters _parameters);

        // calculate bins using the current discretization method
        List<Bin> CalculateBins(double[] list);
        // returns a list of discretized data using cuts
        int[] DiscretizeData(double[] data, double[] cuts);
        // returns a list of discretized values (1,2,3..) for the given list of values
        int[] getDiscretizedValues(double[] data, double[] cuts);
    } // interface DiscretizationMethod

    class Persist : DiscretizationMethod
    {
        long _ID;
        //public Parameters parameters;
        //private double eps = Math.Pow (10,-6);

        public Persist()
        {
            parameters = new Parameters();
        } // basic constructor

        public long getID()
        {
            return _ID;
        } // getID

        public void setID(long ID)
        {
            _ID = ID;
        } // setID

        /*
        public Parameters getParameters()
        {
            return parameters;
        } // getParameters

        public void setParameters(Parameters _parameters)
        {
            parameters = _parameters;
        } // setParameters
        */

        /// <summary>
        /// Discretize data by persist
        /// </summary>
        /// <param name="data">data array of double values</param>
        /// <param name="cuts">list of cuts to level by</param>
        /// <returns>a list of integers stating bin for each time stamp</returns>
        public int[] DiscretizeData(double[] data, double[] cuts)
        {
            return Methods.DiscretizeData(data, cuts);
        } // DiscretizeData


        /// <summary>
        /// Checks the probability arrays, used in Persistence calculation
        /// </summary>
        /// <param name="p">p group</param>
        /// <param name="q">q group</param>
        /// <returns>false if groups have any 0 probabilities, else true</returns>
        private bool checkPQ(double[] p, double[] q)
        {
            if (p[0] == 0 || p[1] == 0 || q[0] == 0 || q[1] == 0)
                return false;
            else
                return true;
        } // checkPQ

        /// <summary>
        /// Calculates persistence of data by given cuts
        /// </summary>
        /// <param name="data">data to be examined</param>
        /// <param name="cuts">cuts used for discretization</param>
        /// <returns>The persistence score of the given data discretized</returns>
        public double getPersistence(double[] data, double[] cuts, int[] forbiddenList)
        {
            int i;
            // number of states
            int k = cuts.Length + 1;
            // number of time points
            int n = data.Length;
            // result
            int sCount;
            double pscore;
            // discretize data by cuts
            int[] DiscretizedData = Methods.DiscretizeData(data, cuts);
            // get markov chain diagonal ( the probability to remain in the same
            // state for each state
            double[] diagonal = Methods.MCML(DiscretizedData, k, forbiddenList);
            double[] p = new double[2];
            double[] q = new double[2];

            /*
            // make sure there are no zeros or ones
            // eps is given in class
            for (i = 0; i < k; i++)
                if (diagonal[i] == 1) diagonal[i] = 1.0 - eps;
            for (i = 0; i < k; i++)
                if (diagonal[i] == 0) diagonal[i] = eps;
            */

            // calculate start probabilities
            double[] prob = new double[k];
            for (i = 0; i < k; i++)
                prob[i] = 0;
            for (i = 0; i < n; i++)
                prob[DiscretizedData[i] - 1]++;
            for (i = 0; i < k; i++)
                prob[i] = (n != 0) ? prob[i] / n : 0;

            // make sure there are no zeros or ones
            // eps is given in class
            /*            for (i = 0; i < k; i++)
                            if (prob[i] == 1) prob[i] = 1.0 - Const.EPS;
                        for (i = 0; i < k; i++)
                            if (prob[i] == 0) prob[i] = Const.EPS;
              */
            sCount = 0;
            pscore = 0;
            for (i = 0; i < k; i++)
            {
                if (prob[i] != 0)
                {
                    // prepare transition probabilities for current state
                    // self vs. non self
                    p[0] = diagonal[i] > 0 ? diagonal[i] : Const.EPS;
                    p[1] = diagonal[i] > 0 ? 1 - diagonal[i] : 1 - Const.EPS;
                    // prepare start probabilities for current state
                    q[0] = prob[i];// > 0 ? prob[i] : Const.EPS;
                    q[1] = 1 - prob[i]; // > 0 ? 1 - prob[i] : 1 - Const.EPS;

                    // add to or substract from score 
                    if (checkPQ(p, q))
                    {
                        sCount++;
                        pscore += Math.Sign(diagonal[i] - prob[i]) * Methods.getSKLDiv(p, q);
                    } // if legal
                } // if state is used
            } // for calculate score

            // return mean
            if (sCount != 0)
                return pscore / ((double)(sCount));
            else
                return 0;
        } // getPersistence


        /// <summary>
        /// Persist discretization algorithm
        /// In general, this method finds cuts by estimating the serie's persistence
        /// when discretized using the tested cuts, when persistence means the probability
        /// to stay in a certain state vs the probability to be in the state.
        /// </summary>
        /// <param name="list"> time serie values</param>
        /// <param name="parameters"> given parameters used in parameters (MinBins, MaxBins, SkipLow, SkipHigh, Percentiles)</param>
        /// <returns>Bins list</returns>
        public List<Bin> CalculateBins(double[] list)
        {
            //input 
            double[] Serie = list;
            // all available cuts (at the start using percentiles
            double[] CandidateCuts;
            // available cuts at this stage (after filtering)
            double[] CandidateFree;
            // used for current tested cut (joined with found cuts
            List<double> CandidateTemp;
            // used for percentiles
            double[] range;
            //double prod;
            // list of best scores
            List<double> BestScores = new List<double>();
            // temporary list when looking for the best cut
            List<double> PersistenceScores;
            // list of best bins
            List<List<double>> BestBins = new List<List<double>>();

            // used when finding new cuts with filtered Candidates list
            List<int> TempIndexes = new List<int>();
            List<double> TempCuts = new List<double>();
            List<double> TemporalBins = new List<double>();
            double[] cuts;

            List<Bin> Bins;

            // Results Variables
            double BestCut = 0;
            double BestScore = 0;
            int BestIndex = 0;

            // loop variables
            int i, j, k;

            // set default parameters values
            bool ScanRange = false;
            int NumOfBins = 3;
            int MinBins = 3;
            int MaxBins = 7;
            int SkipLow = 4;
            int SkipHigh = 4;
            int Percentiles = 99;
            int[] ForbiddenList = new int[0];

            // get parameters
            if (parameters.getParam("NumOfBins") != Parameters.NotFound)
                NumOfBins = int.Parse(parameters.getParam("NumOfBins").ToString());
            if (parameters.getParam("ScanRange") != Parameters.NotFound)
                ScanRange = bool.Parse(parameters.getParam("ScanRange").ToString().ToLower());
            if (parameters.getParam("MinBins") != Parameters.NotFound)
                MinBins = int.Parse(parameters.getParam("MinBins").ToString());
            if (parameters.getParam("MaxBins") != Parameters.NotFound)
                MaxBins = int.Parse(parameters.getParam("MaxBins").ToString());
            if (parameters.getParam("TrimRight") != Parameters.NotFound)
                SkipLow = int.Parse(parameters.getParam("TrimRight").ToString());
            if (parameters.getParam("TrimLeft") != Parameters.NotFound)
                SkipHigh = int.Parse(parameters.getParam("TrimLeft").ToString());
            if (parameters.getParam("Percentiles") != Parameters.NotFound)
                Percentiles = int.Parse(parameters.getParam("Percentiles").ToString());
            if (parameters.getParam("ForbiddenTransitions") != Parameters.NotFound)
                ForbiddenList = ((int[])parameters.getParam("ForbiddenTransitions"));

            if (!ScanRange)
            {
                MinBins = NumOfBins;
                MaxBins = NumOfBins;
            }
            // used for percentiles
            range = new double[Percentiles];
            // Available candidate cuts (marked as 1)
            CandidateFree = new double[range.Length];
            //for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
            for (i = 1; i <= range.Length; i++)
            {
                range[i - 1] = ((double)i) / 100;
                // cut list's ends - List of indexes of available cuts
            } // for            
            for (i = SkipLow + 1; i <= range.Length - SkipHigh; i++)
                CandidateFree[i - 1] = 1;

            // get candidate cuts using percentiles of the serie, from 1 to Percentiles percent (d=1)
            CandidateCuts = Methods.getListPrctile(Methods.sortList(Serie), range);


            for (j = 0; j < MaxBins - 1; j++)
            {
                CandidateTemp = new List<double>();
                TempIndexes = new List<int>();
                // get current free cuts (signed with 1)
                for (k = 0; k < CandidateFree.Length; k++)
                    if (CandidateFree[k] == 1)
                    {
                        CandidateTemp.Add(CandidateCuts[k]);
                        TempIndexes.Add(k);
                    } // if

                // still got cuts to check
                if (CandidateTemp.Count > 0)
                {
                    // zeroize persistence scores
                    PersistenceScores = new List<double>();

                    for (i = 0; i < CandidateTemp.Count; i++)
                    {
                        // prepare cuts list
                        TemporalBins.Clear();
                        // add best found cuts (valued by persistence)
                        TemporalBins.AddRange(TempCuts);
                        // add current tested cut
                        TemporalBins.Add(CandidateTemp[i]);
                        TemporalBins.Sort();
                        cuts = TemporalBins.ToArray();
                        // calculate and save persistence score
                        PersistenceScores.Add(getPersistence(Serie, cuts, ForbiddenList));
                    } // Calculate Persistence

                    // find best score and its index
                    BestIndex = 0;
                    BestScore = PersistenceScores[0];
                    for (i = 0; i < PersistenceScores.Count; i++)
                    {
                        if (PersistenceScores[i] > BestScore)
                        {
                            BestScore = PersistenceScores[i];
                            BestIndex = i;
                        } // if
                    } // for find max

                    BestCut = CandidateTemp[BestIndex];
                    // Add cut with highest persistence to found cuts
                    TempCuts.Add(BestCut);
                    // remove cuts in radius skiplow and skiphigh around best cut
                    // from list of cuts to be tested 
                    for (i = TempIndexes[BestIndex] - SkipLow; i <= TempIndexes[BestIndex] + SkipHigh; i++)
                        CandidateFree[i] = 0;

                    // if number of bins is sufficient save bins and their scores
                    if (j + 2 >= MinBins)
                    {
                        BestScores.Add(BestScore);
                        BestBins.Add(TempCuts.GetRange(0, TempCuts.Count));
                    } // if

                } // if Main - still have cuts to check

            } // for Main Loop

            // set Best bins list (in case there's only one)
            BestIndex = 0;

            if (MinBins < MaxBins)
            {
                BestScore = BestScores[0];
                for (i = 0; i < BestScores.Count; i++)
                    if (BestScores[i] > BestScore)
                    {
                        BestScore = BestScores[i];
                        BestIndex = i;
                    } // find best bins
            } // if several bins numbers were tested
            BestBins[BestIndex].Sort();
            // create bins list
            Bins = Methods.CreateBins(BestBins[BestIndex].ToArray(), Const.STR_ARR_BIN_BASE_LABELS);
            return Bins;

        } // CalculateBins -  Persist

        /// <summary>
        /// Returns a list of discretized data using given cuts
        /// </summary>
        /// <param name="data">list of raw data</param>
        /// <param name="cuts">given cuts</param>
        /// <returns>list of levels</returns>
        public int[] getDiscretizedValues(double[] data, double[] cuts)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                res[i] = Methods.getDiscretizedValue(data[i], cuts);
            return res;
        } // getDiscretizedValue
    } // Class Persist

}
