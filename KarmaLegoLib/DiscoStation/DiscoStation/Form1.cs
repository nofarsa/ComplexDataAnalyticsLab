using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tonception;

//using KarmaLegoLib;

namespace DiscoStation
{
    public partial class Form1 : Form
    {
        string filePath;
        string vmapPath;
        string datasetName;
        string defaultOutput = @"C:\Users\nahmiasd\Documents\KL\timeIntervals";
        private void ExeDiscoSet()
        {
            string server = "DESKTOP-B73IALN" /*ROBERT-X220"*/, dbName = "DiabetesEval", tsTableName = "TemporalPropertiesValuesFAGender_F_"; // TemporalPropertiesIntervalsSAGender"; // "TemporalPropertiesValuesSA"; // "TemporalPropertiesValuesFA";
            string folderPath = "C:\\Users\\robert\\Desktop\\Maitreya\\KarmaLegoLib_06092015_Maitreya_TIRPSELandOFFSET_andTIRPSelection___ForDaniel\\";
            Directory.CreateDirectory(folderPath + "\\timeSeries\\");
            string outTSFile = folderPath + "timeSeries\\" + tsTableName + "_TSs.csv";
            DiscoMania.saveTemporalPropertiesTableToTSsFile("DESKTOP-B73IALN" /*ROBERT-X220"*/, "sa" /*"QiSecUser"*/, "CDAL1133" /*"AbV01"*/, dbName, tsTableName, 3000, outTSFile);

            string bdTableName = tsTableName.Replace("TemporalPropertiesValues", "BinsDefinitions") + Const.STR_MNAME_SAX + "3" + "NoTendencyUseClassNoCluster";
            string tiTableName = tsTableName.Replace("Values", "Intervals") + Const.STR_MNAME_SAX + "3" + "NoTendencyUseClassNoCluster";
            Directory.CreateDirectory(folderPath + "\\timeIntervals\\");
            string outTIFile = folderPath + "timeIntervals\\" + tiTableName + "_TIs.csv";

            DiscoMania.saveTmprlPrprtsIntrvlsTblToTIsFile("DESKTOP-B73IALN" /*ROBERT-X220"*/, "sa" /*"QiSecUser"*/, "CDAL1133" /*"AbV01"*/, dbName, tiTableName, bdTableName, 3000, outTIFile);
        }

        private void tonceptualize()
        {
            string tsFolder = @"C:\Users\nahmiasd\Documents\KL\timeSeries\";
            string tiFolder = @"C:\Users\nahmiasd\Documents\KL\timeIntervals\";
            string tsFile = "Waveform";
            string timeSeriesFile = tsFolder + tsFile + ".csv";
            int binsNum = 3;
            string absMethod = Const.STR_MNAME_SAX;
            //string absMethod = Const.STR_MNAME_EQW;
            //string timeIntervalsFile = tiFolder + tsFile + binsNum + absMethod + "TIs11.csv";
            string timeIntervalsFile = tiFolder + tsFile  +"_"+ absMethod + "_" + binsNum;
            //DiscoMania.tonceptlizeTimeSeriesByClass(timeSeriesFile, binsNum, absMethod, timeIntervalsFile, 11);
            DiscoMania.tonceptulizeTimeSeries(timeSeriesFile, binsNum, absMethod, timeIntervalsFile+".csv");
        }

        private void testCSV()
        {
            CSVHandler handler = new CSVHandler();
            TonceptSet ts= new TonceptSet();
            ts = handler.ParallelLoadTonceptSetFromCSV(@"C:\\Users\\nahmiasd\\Documents\waveform\death_cohort.csv", ts);
            foreach (EntityData ent in ts.entities)
            {
                ent.varDic.Add(999, new EntityTempVariable(999, "class"));
                ent.varDic[999].values.Add(new TimeStampValueSymbol(0, 1));
            }
            //ts = handler.ParallelLoadTonceptSetFromCSV(@"C:\\Users\\nahmiasd\\Documents\no_sepsis_same_count.csv", ts);
            //foreach (EntityData ent in ts.entities)
            //{
            //    if (ent.varDic.Keys.Contains(999))
            //        continue;
            //    ent.varDic.Add(999, new EntityTempVariable(999, "class"));
            //    ent.varDic[999].values.Add(new TimeStampValueSymbol(0, 0));
            //}
            ts = handler.ParallelLoadTonceptSetFromCSV(@"C:\\Users\\nahmiasd\\Documents\waveform\death_controls_same_size.csv", ts);
            foreach (EntityData ent in ts.entities)
            {
                if (ent.varDic.Keys.Contains(999))
                    continue;
                ent.varDic.Add(999, new EntityTempVariable(999, "class"));
                ent.varDic[999].values.Add(new TimeStampValueSymbol(0, 0));
            }
            ts.updateVariableDicFromFile(@"C:\\Users\\nahmiasd\\Documents\waveform\variablesMapping_death.csv");
            //ts.binsOnly = true;
            int binsNum = 3;
            //string absMethod = Const.STR_MNAME_SAX;
            string absMethod = Const.STR_MNAME_EQW;
            ts.binsOnly = true;
            ts.tonceptulize(absMethod,binsNum);
            ts.classSeparator = 999;
            ts.saveTonceptSetTSsToFile(@"C:\\Users\\nahmiasd\\Documents\\KL\\timeSeries\\waveform_mult3_TS.CSV");
            ts.saveTonceptSetTIsToFile(@"C:\\Users\\nahmiasd\\Documents\\KL\\timeIntervals\\Sepsis_controls_samesize_binsonly" + absMethod + "_" + binsNum + ".csv");
        }

        private void generateWaveformTS()
        {
            string[] entities = Directory.GetFiles(@"C:\Daniel\waveform");
            TonceptSet disco = new TonceptSet();
            Dictionary<string, int> varDic = generateVarDic(entities);
            saveVarDicToFile(@"C:\Daniel\waveformTempVarProp.csv", varDic);
            Parallel.ForEach(entities, s =>
            {
                wfWorkerThread(s,ref varDic,ref disco);
            });
            disco.updateVariableDicFromFile(@"C:\Daniel\waveformTempVarProp.csv");
            disco.saveTonceptSetTSsToFile(@"C:\Users\nahmiasd\Documents\KL\timeSeries\Waveform.csv");
        }

        private void saveVarDicToFile(string v, Dictionary<string, int> varDic)
        {
            TextWriter writer = new StreamWriter(File.Create(v));
            writer.WriteLine("TemporalPropertyID,TemporalPropertyName");
            foreach (string s in varDic.Keys)
            {
                writer.WriteLine(varDic[s] + "," + s);
            }
            writer.Close();
        }

        private Dictionary<string, int> generateVarDic(string[] entities)
        {
            int idCount = 1;
            Dictionary<string, int> varDic = new Dictionary<string, int>();
            foreach (string ent in entities)
            {
                using(TextReader rdr = new StreamReader(ent))
                {
                    string line =rdr.ReadLine();
                    string[] vars = rdr.ReadLine().Split(',');
                    if (vars.Length == 1)
                        vars = rdr.ReadLine().Split(',');
                    for(int i = 1; i < vars.Length; i++)
                    {
                        string var = vars[i].Replace(" ","").Replace("_","");
                        if (!string.IsNullOrEmpty(var)&&!varDic.Keys.Contains(var))
                        {
                            varDic.Add(var, idCount++);
                        }
                    }
                }
            }
            return varDic;
        }

        private void wfWorkerThread(string s, ref Dictionary<string, int> varDic,ref TonceptSet disco)
        {
            TextReader rdr = new StreamReader(s);
            EntityData ent = new EntityData();
            ent.entityID = int.Parse(s.Split('\\')[s.Split('\\').Length-1]);
            char[] sep = new char[1] { ',' };
            rdr.ReadLine(); //first row is unused ent id
            string[] vars = rdr.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if(vars.Length<=1)
                vars = rdr.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
            while (rdr.Peek() >= 0)
            {
                string[] line = rdr.ReadLine().Split(sep);
                if (line.Length <= 1)
                    continue;
                int timestamp = int.Parse(line[0]);
                for(int i = 1; i < line.Length; i++)
                {
                    double value;
                    if(double.TryParse(line[i], out value))
                    {
                        if(value!=0 && value.ToString() != "NaN")
                        {
                            string var = vars[i].Replace(" ", "").Replace("_", "");
                            int varid = varDic[var];
                            TimeStampValueSymbol tsv = new TimeStampValueSymbol(timestamp, value);
                            if (!ent.varDic.Keys.Contains(varid)){
                                ent.varDic.Add(varid, new EntityTempVariable(varid));
                            }
                            ent.varDic[varid].values.Add(tsv);
                        }
                    }
                }
            }
            if (ent.varDic.Count == 0)
                return;
            disco.entM.WaitOne();
            disco.entities.Add(ent);
            disco.entM.ReleaseMutex();
            rdr.Close();
        }

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

            //ExeKarma();

            //runCUMC();
            //runMaitreya();

            //runDharmaLegoRunTime();

            //string root = @"C:\\Users\\nahmiasd\\Documents\\";
            //ClassTIDivider div = new ClassTIDivider();
            //div.start(root + "Class0IDS.csv", root + "Class1IDS.csv", root + "ICUTICSV.csv", "ICU_TD4C");
            //ExeKarmaLego();
            //ExeKarmaLeogi();
            //exeWeka();
            //compareFiles();
            //ExeDiscoSet();
            //testCSV();
            testBins();
            //tonceptualize();
            //generateWaveformTS();
            //Tonception.DiscoMania.testTonceptions();

        }

        private void testBins()
        {
            TonceptSet ts = new TonceptSet();
            ts.readTonceptSetTSsFile(@"C:\Users\nahmiasd\Documents\KL\timeSeries\Hepatitis.csv");
            ts.binsOnly = true;
            ts.tonceptulize(Const.STR_MNAME_SAX, 3);
            ts.saveTonceptSetTIsToFile(@"C:\Users\nahmiasd\Documents\KL\binsOnly\Hepatitis_bins_SAX_3.csv");
            ts.binsOnly = false;
            ts.tonceptulize(Const.STR_MNAME_SAX, 3);
            ts.saveTonceptSetTIsToFile(@"C:\Users\nahmiasd\Documents\KL\binsOnly\Hepatitis_SAX_3.csv");
        }

        private void run_btn_Click(object sender, EventArgs e)
        {
            if (!checkValidity())
                return;
            datasetName = nameBox.Text;
            CSVHandler handler = new CSVHandler();
            TonceptSet ts = new TonceptSet();
            ts.datasetName = datasetName;
            ts.binsOnly = checkBox1.Checked;
            int binsNum = int.Parse(bins_box.Text);
            ts = handler.ParallelLoadTonceptSetFromCSV(filePath, ts);
            if (!string.IsNullOrEmpty(vmapPath))
                ts.updateVariableDicFromFile(vmapPath);
            else
                ts.updateVarDic();
            string absMethod = methodBox.SelectedItem.ToString();
            ts.tonceptulize(absMethod, binsNum);
            int separator = -1;
            if(int.TryParse(class_box.Text,out separator))
            {
                ts.classSeparator = separator;
            }
            if (ts.binsOnly)
                ts.saveTonceptSetTIsToFile(defaultOutput + "\\" + datasetName + "_" + "AbstractedTS" + "_" + absMethod + "_" + binsNum+".csv");
            else
                ts.saveTonceptSetTIsToFile(defaultOutput + "\\" + datasetName + "_" + absMethod + "_" + binsNum);
            MessageBox.Show("Done!");
        }

        private bool checkValidity()
        {
            int bins;
            if(!int.TryParse(bins_box.Text,out bins)){
                MessageBox.Show("invalid number of bins");
                return false;
            }
            if (methodBox.SelectedItem.ToString() == "")
            {
                MessageBox.Show("No method selected");
                return false;
            }
            if (nameBox.Text == "" || nameBox.Text == null)
            {
                MessageBox.Show("Please submit dataset name");
                return false;
            }
            return true;
        }

        private void methodBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void browse_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string filename="";
            ofd.Filter = "CSV Files|*.CSV;";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            if (filename != "")
            {
                filePath = filename;
                fname_lbl.Text = filename;
            }
        }

        private void vmap_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string filename = "";
            ofd.Filter = "CSV Files|*.CSV;";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            if (filename != "")
            {
                vmapPath = filename;
            }
        }
    }
}