using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiscoStation
{
    class discoStateDef
    {
        public string stateID;
        public string temporalID;
        public string temporalName;
        public string methodName;
        public string score;
        public string BinID;
        public string BinLabel;
        public double BinFrom;
        public double BinTo;

        public void setValues(string line)
        {
            string[] words = line.Split( ',' );
            stateID          = words[0];
            temporalID       = words[1];
            temporalName     = words[2];
            methodName       = words[3];
            score            = words[4];
            BinID            = words[5];
            BinLabel         = words[6];
            BinFrom          = double.Parse(words[7]);
            BinTo            = double.Parse(words[8]);
        }
    }

    class discoTempDef
    {
        public string temporalID;
        public string temporalName;
        public Dictionary<string, discoStateDef> temporalStatesDefs = new Dictionary<string, discoStateDef>();
    
    }

    class DiscoDefs
    {
        public Dictionary<string, discoTempDef> tempsDefs = new Dictionary<string,discoTempDef>();

        public void PrintDefsToFile(string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath);
            sw.WriteLine("StateID,TemporalPropertyID,TemporalPropertyName,MethodName,Score,BinID,BinLabel,BinFrom,BinTo");

            for(int i = 0; i < tempsDefs.Count ; i++)
            {
                discoTempDef dTD = tempsDefs.ElementAt(i).Value;
                for(int j = 0; j < dTD.temporalStatesDefs.Count ; j++)
                {
                    discoStateDef dSD = dTD.temporalStatesDefs.ElementAt(j).Value;
                    string line = dSD.stateID + "," + dSD.temporalID + "," + dSD.temporalName + "," + dSD.methodName + "," + dSD.score + "," + dSD.BinID + "," + dSD.BinLabel + "," + dSD.BinFrom + "," + dSD.BinTo;
                    sw.WriteLine(line);
                }
            }
            sw.Close();
        }

        public void ReadDefsFromFile(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        discoStateDef dSD = new discoStateDef();
                        dSD.setValues(line);
                        if (tempsDefs.ContainsKey(dSD.temporalID))
                        {
                            tempsDefs[dSD.temporalID].temporalStatesDefs.Add(dSD.stateID, dSD);
                        }
                        else
                        {
                            discoTempDef dTD = new discoTempDef();
                            dTD.temporalID = dSD.temporalID;
                            dTD.temporalName = dSD.temporalName;
                            dTD.temporalStatesDefs.Add(dSD.stateID, dSD);
                            tempsDefs.Add(dTD.temporalID, dTD);
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
