using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;

namespace DiscoStation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            DiscoDefs dD = new DiscoDefs();
            //dD.ReadDefsFromFile("D:\\KarmaLegoBook\\Disco\\disco.csv");
            //dD.PrintDefsToFile("D:\\KarmaLegoBook\\Disco\\discoPy.csv");
            Tonception.DataBase dbase = new Tonception.DataBase("ROBERT-X220", "QiSecUser", "AbV01", "Diabetes_TDW", true, 30);
            //KarmaLegoLib.KarmaLego.test();
            
            KarmaLegoLib.TonceptSet disco = new KarmaLegoLib.TonceptSet();
            /*
            disco = Kiosk.LoadTimeIntervalsTableToDiscoSet("TemporalPropertiesIntervalsFAGenderKB0TendencyUseClassNoCluster", 3000, dbase);
            disco.saveTonceptSetToFile("D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\savedTonceptSet.txt");
            KarmaLegoLib.TonceptSet discoNew = new KarmaLegoLib.TonceptSet();
            discoNew.readTonceptSetFile("D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\savedTonceptSet.txt");
            if (disco == discoNew)
                i = 10;
            
            KarmaLegoLib.KarmaLego KL = new KarmaLegoLib.KarmaLego(KarmaLegoLib.KLC.KL_TRANS_YES, KarmaLegoLib.KLC.KL_PRINT_YES, KarmaLegoLib.KLC.ALLEN7_RELATIONS, 0, 80, 3, disco);
            KL.Karma(disco);
            KL.KarmaAndLego(KarmaLegoLib.KLC.KL_TRANS_NO, false, disco, "D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\noTransLego"+DateTime.Now.Hour+DateTime.Now.Minute+ ".txt");
            KL.KarmaAndLego(KarmaLegoLib.KLC.KL_TRANS_YES, false, disco, "D:\\KarmaLegoBook\\OgelAmrak-AmrakLego-KarmaOgel\\transLego" + DateTime.Now.Hour + DateTime.Now.Minute + ".txt");
             */
            
            disco = Tonception.Kiosk.LoadTimestampsValuesTableToDiscoSet("TemporalPropertiesValuesSA", 13, dbase);
            double[] vals = disco.getValuesAsDoubleVectorOfVariable(3);

            //DiscretizationMethod method = getMethodObject(progparameters.getParam(Const.STR_METHOD_NAME).ToString(), intBins, intTopK, progparameters);
            List<Tonception.Bin> binsListPRSST = Tonception.DiscoMania.getBinsPerMethodAndData(Tonception.Const.STR_MNAME_PERSIST, 3, vals, "");
            List<Tonception.Bin> binsListSAX = Tonception.DiscoMania.getBinsPerMethodAndData(Tonception.Const.STR_MNAME_SAX, 3, vals, "");

            double[][] databyclass = new double[2][];
            double[][][] databyclassuser = new double[2][][];
            databyclass[0] = new double[] { 1, 2, 3, 1, 2, 2, 43, 4, 965, 07, 9, 887, 7, 8564, 36, 5323, 4243, 786, 86, 789, 76, 2, 4, 65, 56, 57, 2, 54, 4235, 345, 23, 5, 43, 5132, 32 };
            databyclass[1] = new double[] { 68, 6, 7, 62, 5, 87, 97, 162, 5, 7, 868, 60, 31573, 2, 432, 5, 345, 432, 54, 235, 345, 5, 2345, 75, 564, 56, 90, 5, 786, 4, 54, 525687, 98, 97, 45365 };
            databyclassuser[0] = new double[2][];
            databyclassuser[0][0] = new double[] { 1, 2, 3, 1, 2, 2, 43, 4, 965, 07, 9, 887, 7, 8564, 36, 5323, 4243, 786 };
            databyclassuser[0][1] = new double[] { 86, 789, 76, 2, 4, 65, 56, 57, 2, 54, 4235, 345, 23, 5, 43, 5132, 32 };
            databyclassuser[1] = new double[2][];
            databyclassuser[1][0] = new double[] { 68, 6, 7, 62, 5, 87, 97, 162, 5, 7, 868, 60, 31573, 2, 432, 5, 345, 432 };
            databyclassuser[1][1] = new double[] { 54, 235, 345, 5, 2345, 75, 564, 56, 90, 5, 786, 4, 54, 525687, 98, 97, 45365 };
            double[] data = new double[] { 1, 2, 3, 1, 2, 2, 43, 4, 965, 07, 9, 887, 7, 8564, 36, 5323, 4243, 786, 86, 789, 76, 2, 4, 65, 56, 57, 2, 54, 4235, 345, 23, 5, 43, 5132, 32, 68, 6, 7, 62, 5, 87, 97, 162, 5, 7, 868, 60, 31573, 2, 432, 5, 345, 432, 54, 235, 345, 5, 2345, 75, 564, 56, 90, 5, 786, 4, 54, 525687, 98, 97, 45365 };
            Tonception.TD4CLA_Ent d = new Tonception.TD4CLA_Ent();
            d.parameters.Add("TopK", 10);
            d.parameters.Add(Tonception.Const.STR_NUM_OF_BINS, 5);
            d.parameters.Add(Tonception.Const.STR_DATA_BY_CLASS, databyclass);
            d.parameters.Add(Tonception.Const.STR_DATA_BY_CLASS_USER, databyclassuser);
            List<Tonception.Bin> bins = d.CalculateBins(data);


            Tonception.TD4C_Ent d1 = new Tonception.TD4C_Ent();
            d1.parameters.Add(Tonception.Const.STR_NUM_OF_BINS, 5);
            d1.parameters.Add(Tonception.Const.STR_DATA_BY_CLASS, databyclass);
            bins = d1.CalculateBins(data);
            bins.Add(new Tonception.Bin(0, 4));
             
            

        }


    }
}
