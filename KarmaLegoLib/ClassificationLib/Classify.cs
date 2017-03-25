using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Data;
using System.Diagnostics;


namespace ClassificationLib
{
    public class Classify
    {
        static void runWeka(string outfile, string wekaString)
        {
            try
            {

                if (File.Exists(outfile))
                    return;
                Process proc = new Process();
                proc.StartInfo.FileName = "Java";
                proc.StartInfo.Arguments = wekaString;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                StreamWriter sw = new StreamWriter(outfile);
                sw.WriteLine(proc.StandardOutput.ReadToEnd());
                proc.WaitForExit();
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public static void runXV(string WEKA_Jar_File_Path, string trainingfile, string outfile, int crossv, string classifier, string featureselection, string ranker)
        {
            string wekaString =
                    "-Xmx1300m  -cp " + WEKA_Jar_File_Path + " weka.classifiers.meta.AttributeSelectedClassifier -i -c 1 -t " + trainingfile +
                    " -x " + crossv.ToString() + " " + // cross validation
                    " -E " + '"' + "weka.attributeSelection." + featureselection + '"' +
                    " -S " + '"' + "weka.attributeSelection." + ranker + '"' +
                    " -W weka.classifiers." + classifier;
            runWeka(outfile, wekaString);
        }

        public static void runTest(string WEKA_Jar_File_Path, string trainingfile, string outfile, string testfile, string classifier, string featureselection, string ranker)
        {
            string wekaString =
                    "-Xmx1300m  -cp " + WEKA_Jar_File_Path + " weka.classifiers.meta.AttributeSelectedClassifier -i -c 1 -t " + trainingfile +
                    " -T " + testfile + // cross validation
                    " -E " + '"' + "weka.attributeSelection." + featureselection + '"' +
                    " -S " + '"' + "weka.attributeSelection." + ranker + '"' +
                    " -W weka.classifiers." + classifier;

            runWeka(outfile, wekaString);
        }


        /// run experiment including outpujting predictions
        public static void runTestPrdctn(string WEKA_Jar_File_Path, string trainingfile, string outfile, string testfile, string classifier, string featureselection, string ranker)
        {

            string wekaString =
                    "-Xmx1300m  -cp " + WEKA_Jar_File_Path + 
                    " weka.classifiers.meta.AttributeSelectedClassifier -i -c 1 -t " + trainingfile +
                    " -p 0 " +
                    " -T " + testfile + // cross validation
                    " -E " + '"' + "weka.attributeSelection." + featureselection + '"' +
                    " -S " + '"' + "weka.attributeSelection." + ranker + '"' +
                    " -W weka.classifiers." + classifier;
            runWeka(outfile, wekaString);

        } // runExperimentPrediction


        public static void runTestPrdctnNoFS(string WEKA_Jar_File_Path, string trainingfile, string outfile, string testfile, string classifier)
        {// something is wrong here
            string wekaString =
                    "-Xmx1300m  -cp " + WEKA_Jar_File_Path + //" weka.classifiers." + classifier +
                    " weka.classifiers." + classifier + " -i -c 1 -t " + trainingfile +
                    //" -i -c 1 -t " + trainingfile +
                    " -p 0" +
                    " -T " + testfile;
                    
            runWeka(outfile, wekaString);
        }

        public static void runTestNoFS(string WEKA_Jar_File_Path, string trainingfile, string outfile, string testfile, string classifier)
        {
            string wekaString =
                    "-Xmx1300m  -cp " + WEKA_Jar_File_Path + " weka.classifiers." + classifier +
                    " -i -c 1 -t " + trainingfile +
                    " -T " + testfile;

            runWeka(outfile, wekaString);
        }


    }
}
