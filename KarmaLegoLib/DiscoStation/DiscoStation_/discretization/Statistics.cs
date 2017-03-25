using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoStation //Discretizer
{
    class Statistics
    {

        public class Measures
        {

            /// <summary>
            /// computes the average value of a given list of doubles
            /// </summary>
            /// <param name="vector">list of values</param>
            /// <returns>average value</returns>
            public static double getAverage(double[] vector)
            {

                double ans = 0;
                for (int i = 0; i < vector.Length; i++)
                    ans += vector[i];
                return ans / vector.Length;
            } // getAverage

            /// <summary>
            /// computes the standard deviation
            /// </summary>
            /// <param name="vector">list of values</param>
            /// <returns>average value and standard deviation</returns>
            public static double[] getAverageStandardDeviation(double[] vector)
            {
                double avg = 0, stddev = 0;
                //avg = getAverage(vector);

                for (int i = 0; i < vector.Length; i++)
                {
                    stddev += Math.Pow(vector[i], 2);
                    avg += vector[i];
                }

                avg /= vector.Length;
                stddev = Math.Sqrt(stddev / vector.Length - Math.Pow(avg, 2));

                return new double[] { avg, stddev };

            } // getAverageStandardDeviation

            /// <summary>
            /// returns the cosine similarity between 2 vectors, ranges from -1 to 1
            /// </summary>
            /// <param name="vectorA"></param>
            /// <param name="vectorB"></param>
            /// <returns></returns>
            public static double getCosineSimilarity(double[] vectorA, double[] vectorB)
            {
                double up = Vectors.scalarMultiplication(vectorA, vectorB);
                double down = Vectors.getNorm(vectorA) * Vectors.getNorm(vectorB);
                if (Math.Abs(down - up) < Const.EPS) down = up;
                return down != 0 ? up / down : 0;
            } // getCosineDistance


            public static double getCovariance(double[] vectorA, double[] vectorB)
            {

                double ans = 0;

                double meanA = Measures.getAverage(vectorA);
                double meanB = Measures.getAverage(vectorB);
                
                int i;

                for (i = 0; i < vectorA.Length; i++)
                    ans += (vectorA[i] - meanA) * (vectorB[i] - meanB) / vectorA.Length;

                return ans;

            } // getCovariance


            public static double[][] getCovarianceMatrix(double[][] vectors)
            {
                int i, j, n = vectors.Length;
                double[][] ans = new double [n][];
                for (i = 0; i < n; i++)
                {
                    ans[i] = new double[n];
                    for (j = i; j < n; j++)
                    {
                        if (i != j)
                            ans[i][j] = getCovariance(vectors[i], vectors[j]);
                        else
                            ans[i][i] = Math.Pow(getAverageStandardDeviation(vectors[i])[1], 2);
                    } // for columns

                } // for rows


                // complete lower part of matrix (as it is diagonal)
                for (j = 0; j < n; j++)
                {
                    for (i = j + 1; i < n; i++)
                    {
                        ans[i][j] = ans[j][i];
                    } // for rows
                } // for columns

                return ans;

            } // getCovarianceMatrix


            private static void swap(object[] vector, int ind1, int ind2)
            {
                object temp = vector[ind1];
                vector[ind1] = vector[ind2];
                vector[ind2] = temp;
            }


            private static void swap(double[] vector, int ind1, int ind2)
            {
                double temp = vector[ind1];
                vector[ind1] = vector[ind2];
                vector[ind2] = temp;
            }


            private static void sortByEigenvectors(double[] eigenvalues, double[][] eigenvectors)
            {

                int i, maxIndex;

                for (i = 0; i < eigenvalues.Length; i++)
                {
                    maxIndex = Vectors.getMaxIndex(eigenvalues, i);
                    swap(eigenvalues, i, maxIndex);
                    swap(eigenvectors, i, maxIndex);
                }


            } // sortByEigenvectors

            /*
            public static double[] getPCA(double[][] vectors, double featuresPercentage)
            {
                int i;
                double mean;
                // substract mean
                double[][] stage1 = new double[vectors.Length][];
                for (i = 0; i < vectors.Length; i++)
                {
                    mean = Measures.getAverage(vectors[i]);
                    stage1[i] = Vectors.substractValue(vectors[i], mean);
                }
                // find covariance matrix
                double[][] cov = getCovarianceMatrix(stage1);

                double[][] eigenvectors;

                double[] eigenvalues;

                //sort by eigenvalues
                //sortByEigenvectors(eigenvalues, eigenvectors);

                //double[] eigenvalsSubVector = Vectors.getPartialVector(eigenvalues, 0, featuresPercentage * eigenvalues.Length);
                //double[][] eigenvectorsSubMatrix = Matrices.getPartialMatrix(eigenvectors, 0, 0, featuresPercentage * eigenvalues.Length, eigenvectors[0].Length);

                return null;

            } // getPCA
            */


            // compute probability for a given list of levels, and discretized data
            public static double[] getLevelsProbabilities(int[] levels, int[] data)
            {

                int i;

                List<int> lstLevels = new List<int>();
                for (i = 0; i < levels.Length; i++)
                    lstLevels.Add(levels[i]);
                double[] probs = new double[levels.Length];
                for (i = 0; i < probs.Length; i++)
                    probs[i] = 0;
                for (i = 0; i < data.Length; i++)
                    if (lstLevels.IndexOf(data[i]) > -1)
                        probs[lstLevels.IndexOf(data[i])]++;

                for (i = 0; i < probs.Length; i++)
                    //probs[i] = (data.Length != 0) ? (probs[i] / data.Length) : 0;
                    probs[i] = (data.Length > 0) ? (probs[i] / data.Length) : 0;
                return probs;

            } // getLevesProbabilities


            // compute probability for a given list of levels, and discretized data
            public static double[] getLevelsCounts(int[] levels, int[] data)
            {
                int i;
                List<int> lstLevels = new List<int>();
                for (i = 0; i < levels.Length; i++)
                    lstLevels.Add(levels[i]);
                double[] counts = new double[levels.Length];
                for (i = 0; i < counts.Length; i++)
                    counts[i] = 0;
                for (i = 0; i < data.Length; i++)
                    if (lstLevels.IndexOf(data[i]) > -1)
                        counts[lstLevels.IndexOf(data[i])]++;

                return counts;

            } // getLevelsCount


            /// <summary>
            /// returns the entropy of discretized data, using precalculated probabilities for all states of the discretization result
            /// 
            /// </summary>
            /// <param name="probabilities"></param>
            /// <param name="logbase"></param>
            /// <returns></returns>
            public static double getEntropy(double[] probabilities, double logbase)
            {
                double ans = 0;
                for (int i = 0; i < probabilities.Length; i++)
                    ans += probabilities[i] > 0 ? probabilities[i] * Math.Log(probabilities[i], logbase) : 0;
                return ans * -1;
            } // getEntropy


            public static double getSumOfDistances(double[] arr)
            {
                int i, j;
                double ans = 0;

                for (i = 0; i < arr.Length; i++)
                    for (j = i + 1; j < arr.Length; j++)
                        ans += Math.Abs(arr[i] - arr[j]);

                return ans;

            }



            public static double getSumOfKLDivergences(double[][] arr)
            {
                int i, j;
                double ans = 0;

                for (i = 0; i < arr.Length; i++)
                    for (j = 0; j < arr.Length; j++)
                        if (j != i)
                            ans += Methods.getKLDiv(arr[i],arr[j]);

                return ans;
            }            
            
                
            /// <summary>
            /// return sum of cosine similarity between all combinations of vectors. 
            /// </summary>
            /// <param name="arr">array of vectors </param>
            /// <returns>Sum of Similarity </returns>
            public static double getSumOfDistances(double[][] arr)
            {
                int i, j;
                double ans = 0;

                for (i = 0; i < arr.Length; i++)
                    for (j = i + 1; j < arr.Length; j++)
                        ans += Vectors.getEuclideanDistance(arr[i], arr[j]);

                return ans;

            }


            public static double getSumOfCosineSimilarities(double[][] arr)
            {
                int i, j;
                double ans = 0;

                for (i = 0; i < arr.Length; i++)
                    for (j = i + 1; j < arr.Length; j++)
                        ans += Math.Acos (getCosineSimilarity(arr[i], arr[j]));

                return ans;

            }




            public static double[] getMinMax(double[] arr)
            {
                double[] ans = new double[2];
                ans[0] = ans[1] =  arr[0];

                for (int i = 1; i < arr.Length; i++)
                {
                    if (arr[i] < ans[0])
                        ans[0] = arr[i];
                    if (arr[i] > ans[1])
                        ans[1] = arr[i];
                }
                return ans;

            }

            /// <summary>
            /// returns the error 1 measure, defined as the average distance of original serie from the abstracted one
            /// </summary>
            /// <param name="data"></param>
            /// <param name="bins"></param>
            /// <returns>error 1 value</returns>
            public static double getError1(double[] data, List<Bin> bins)
            {
                int i, state;
                double error1 = 0;
                double[] cuts = Methods.Bins2Cuts(bins);
                double[] means = new double[bins.Count];
                double[] maxd = new double [bins.Count];
                double temp;
                for (i = 0; i < bins.Count; i++)
                { 
                    means[i] = (bins[i]._highlimit + bins[i]._lowlimit) / 2;
                    maxd[i] = (bins[i]._highlimit - bins[i]._lowlimit) / 2;
                }   

                for (i = 0; i < data.Length; i++)
                {
                    state = Methods.getDiscretizedValue(data[i], cuts) - 1;
                    temp = maxd[state] != 0 ? (means[state] - data[i]) / maxd[state] : 0;
                    error1 += Math.Pow(temp, 2);
                }
                return Math.Sqrt(error1);

            } // getError1



        } // class Measures


        public class Vectors
        {


            public static double[] substractValue(double[] vector, double value)
            {

                int i;
                double[] newvector = new double[vector.Length];
                for (i = 0; i < vector.Length; i++)
                    newvector[i] = vector[i] - value;

                return newvector;

            } // substractValue


            /// <summary>
            /// given a list of vectors the centroid will be returned
            /// </summary>
            /// <param name="vectors">list of vectors(assuming all of same length)</param>
            /// <returns>centroid</returns>
            public static double[] getCentroidVector(double[][] vectors)
            {
                int i, j;
                double sum;
                double[] ans = new double[vectors[0].Length];
                for (i = 0; i < vectors[0].Length; i++)
                {
                    sum = 0;
                    for (j = 0; j < vectors.Length; j++)
                        sum += vectors[j][i];
                    ans[i] = sum / vectors.Length;
                }

                return ans;
            } // getCentroidVector



            /// <summary>
            /// given 2 vectors of same size, the scalar multiplication will be returned
            /// sum of multiplication by dimension
            /// </summary>
            /// <param name="vectorA">first vector</param>
            /// <param name="vectorB">second vector</param>
            /// <returns>scalar multiplication result</returns>
            public static double scalarMultiplication(double[] vectorA, double[] vectorB)
            {

                double ans = 0;
                for (int i = 0; i < vectorA.Length; i++)
                    ans += vectorA[i] * vectorB[i];
                return ans;

            } // scalarMultiplication


            /// <summary>
            /// given a vector, its standard norm will be returned
            /// </summary>
            /// <param name="vector">given vector</param>
            /// <returns>standard norm</returns>
            public static double getNorm(double[] vector)
            {
                if (vector == null) return 0;
                double ans = 0;
                for (int i = 0; i < vector.Length; i++)
                    ans += Math.Pow(vector[i], 2);
                return Math.Sqrt(ans);
            } // getNorm


            /// <summary>
            /// given a vector of n dimensions, a new vector sized 1 will be returned.
            /// </summary>
            /// <param name="vector">given vector</param>
            /// <returns>normalized vector</returns>
            public static double[] normalize(double[] vector)
            {

                double[] ans = new double[vector.Length];
                double norm = getNorm(vector);
                int i;
                for (i = 0; i < vector.Length; i++)
                    ans[i] = norm != 0 ? vector[i] / norm : 0;

                return ans;

            } // normalize


            public static int getMaxIndex(double[] vector, int start)
            {

                int i, max = start;
                for (i = start + 1; i < vector.Length; i++)
                    if (vector[i] > max) max = i;
                return max;
            }


            /// <summary>
            /// given a vector, a new partial vector will be returned
            /// </summary>
            /// <param name="vector">given vector</param>
            /// <param name="start">starting index to copy from</param>
            /// <param name="newN">size of new vector</param>
            /// <returns>new vector</returns>
            public static double[] getPartialVector(double[] vector, int start, int newN)
            {
                int i;
                double[] ans = new double[newN];

                for (i = start; i < start + newN; i++)
                    ans[i - start] = vector[i];

                return ans;

            } // getPartialVector


            public static double getEuclideanDistance(double[] vectorA, double[] vectorB)
            {
                double ans = 0;
                for (int i = 0; i < vectorA.Length; i++)
                    ans += Math.Pow(vectorA[i] - vectorB[i], 2);
                return Math.Sqrt(ans);
            }


        } // class Vectors


        public class Matrices
        {

            /// <summary>
            /// get a partial matrix from a given NxM matrix
            /// </summary>
            /// <param name="matrix">given matrix</param>
            /// <param name="new0X">number of column to use as the first column in the new matrix</param>
            /// <param name="new0Y">number of row to use as the first row in the new matrix</param>
            /// <param name="newN">length of new matrix</param>
            /// <param name="newM">height of new matrix</param>
            /// <returns>new matrix</returns>
            public static double[][] getPartialMatrix(double[][] matrix, int new0X, int new0Y, int newN, int newM)
            {

                int i, j;
                double[][] ans = new double[newN][];

                for (i = new0X; i < new0X + newN; i++)
                {
                    ans[i - new0X] = new double[newM];
                    for (j = new0Y; j < new0Y + newM; j++)
                    {
                        ans[i - new0X][j - new0Y] = matrix[i][j];
                    }
                }


                return ans;

            } // getPartialMatrix


        } // class Matrices

    } // class Statistics
}
