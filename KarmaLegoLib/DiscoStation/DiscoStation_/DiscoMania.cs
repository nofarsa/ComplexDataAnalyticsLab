using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoStation
{
    class DiscoMania
    {
        public static List<Bin> getBinsPerMethodAndData(string methodName, int binsNumber, double[] vals, string kbFileName)
        {
            DiscretizationMethod method = getMethodObject(methodName, binsNumber, kbFileName); //, intTopK, progparameters);
            List<Bin> binsList = method.CalculateBins(vals);

            return binsList;
        }
        
        /// <summary>
        /// analyzes given method name and creates the respected object
        /// </summary>
        /// <param name="strMethodName"></param>
        /// <param name="intBins"></param>
        /// <param name="intTopK"></param>
        /// <param name="progparameters"></param>
        /// <returns></returns>
        private static DiscretizationMethod getMethodObject(string strMethodName, int intBins, string kb_bins_filename) //, int intTopK, Parameters progparameters)
        {
            DiscretizationMethod method = null;
            if (strMethodName.ToUpper() == Const.STR_MNAME_EQW.ToUpper())
            {
                EQW eqw = new EQW();
                eqw.parameters = new Parameters();
                eqw.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                method = eqw;
            } // if eqw
            else if (strMethodName.ToUpper() == Const.STR_MNAME_SAX.ToUpper())
            {
                SAX sax = new SAX();
                sax.parameters = new Parameters();
                sax.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                method = sax;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_EQF.ToUpper())
            {
                EQF eqf = new EQF();
                eqf.parameters = new Parameters();
                eqf.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                method = eqf;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_PERSIST.ToUpper())
            {
                Persist persist = new Persist();
                persist.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                persist.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = persist;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_FILE.ToUpper())
            {
                BinsFile binsfile = new BinsFile(kb_bins_filename); // progparameters.getParam(Const.STR_BINS_FILENAME).ToString());
                binsfile.parameters.Add(Const.STR_CURRENT_PROPERTY, 0);
                method = binsfile;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C.ToUpper())
            {
                TD4C_Ent d4c = new TD4C_Ent();
                d4c.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C2.ToUpper())
            {
                TD4C_Cos d4c2 = new TD4C_Cos();
                d4c2.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c2.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c2;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C3.ToUpper())
            {
                TD4C_KL d4c3 = new TD4C_KL();
                d4c3.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c3.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c3;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C4.ToUpper())
            {
                TD4C_DiffSum d4c = new TD4C_DiffSum();
                d4c.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C5.ToUpper())
            {
                TD4C_DEnt d4c2 = new TD4C_DEnt();
                d4c2.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c2.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c2;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C6.ToUpper())
            {
                TD4C_DiffSumMax d4c3 = new TD4C_DiffSumMax();
                d4c3.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4c3.parameters.Add(Const.STR_SCAN_RANGE, "false");
                method = d4c3;
            }
            /*else if (strMethodName.ToUpper() == Const.STR_MNAME_D4CLA.ToUpper())
            {
                TD4CLA_Ent d4cla = new TD4CLA_Ent();
                d4cla.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4cla.parameters.Add(Const.STR_SCAN_RANGE, "false");
                d4cla.parameters.Add(Const.STR_TOP_K, intTopK);
                method = d4cla;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C2LA.ToUpper())
            {
                TD4CLA_Cos d4cla2 = new TD4CLA_Cos();
                d4cla2.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4cla2.parameters.Add(Const.STR_SCAN_RANGE, "false");
                d4cla2.parameters.Add(Const.STR_TOP_K, intTopK);
                method = d4cla2;
            }
            else if (strMethodName.ToUpper() == Const.STR_MNAME_D4C3LA.ToUpper())
            {
                TD4CLA_KL d4cla3 = new TD4CLA_KL();
                d4cla3.parameters.Add(Const.STR_NUM_OF_BINS, intBins);
                d4cla3.parameters.Add(Const.STR_SCAN_RANGE, "false");
                d4cla3.parameters.Add(Const.STR_TOP_K, intTopK);
                method = d4cla3;
            }*/
            else
                Error.hardError(Const.INT_ERR_METHOD_NAME);
            return method;
        } // getMethodObject
    }
}
