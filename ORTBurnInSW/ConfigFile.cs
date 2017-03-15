using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ORTBurnInSW
{
    class ConfigFile
    {
        private String ProductsConfigFileName = "ProductsConfiguration.xml";
        private String StationConfigFileName = @"C:/Users/Public/ORTBurnIn/StationConfig.xml";
        
        private XmlDocument ProductsConfig = new XmlDocument();
        private XmlDocument StationConfig = new XmlDocument();

        public struct BurnProfileStep
        {
            public uint Duration; //in seconds
            public uint StartTime; //in seconds
            public UInt16 InputVoltageState; //0-OFF 1-ON
            public UInt16 OutputLoadState; //0-Unloaded 1-Loaded
        }

        public ConfigFile()
        {
            this.ProductsConfig.Load(this.ProductsConfigFileName);
            this.StationConfig.Load(this.StationConfigFileName);
        }

        public static Array GetTestTypes()
        {
            String[] retArray = { };
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode("./Configuration");
            foreach (XmlNode actTestNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actTestNode.SelectSingleNode("./TestName");
                if (actNameNode != null)
                {
                    Array.Resize(ref retArray, retArray.Length + 1);
                    retArray.SetValue(actNameNode.InnerText, retArray.Length - 1);
                }
            }
            return retArray;
        }
        public static Array GetProductsNames(String TestType)
        {
            String[] retArray = { };
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode != null)
                {
                    Array.Resize(ref retArray, retArray.Length + 1);
                    retArray.SetValue(actNameNode.InnerText, retArray.Length - 1);
                }
            }
            return retArray;
        }
        public static Int16 GetNumberOfUnitsInShelf(String TestType, String ProductID)
        {
            Int16 retValue = 0;
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    return Convert.ToInt16(actProductNode.SelectSingleNode("./UnitsPerShelf").InnerText);
                }                
            }
            return retValue;
        }
        public static Int16 GetNumberOfOutputs(String TestType, String ProductID)
        {
            Int16 retValue = 0;
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    return Convert.ToInt16(actProductNode.SelectSingleNode("./Outputs/NumberOfOutputs").InnerText);
                }
            }
            return retValue;
        }
        public static Double GetLowLimit(String TestType, String ProductID, Int16 OutputNumber)
        {
            Double retValue = 0.0;
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    XmlNode OutputsNode = actProductNode.SelectSingleNode("./Outputs");
                    XmlNode OutputNode = OutputsNode.SelectSingleNode(String.Concat("./Output", OutputNumber.ToString().Trim()));
                    if (OutputNode != null)
                    {
                        return Convert.ToDouble(OutputNode.SelectSingleNode("./LowLimit").InnerText.Replace('.', ','));
                    }
                    else
                    {
                        return 99999999.9;
                    }
                }
            }
            return retValue;
        }
        public static Double GetHighLimit(String TestType, String ProductID, Int16 OutputNumber)
        {
            Double retValue = 0.0;
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    XmlNode OutputsNode = actProductNode.SelectSingleNode("./Outputs");
                    XmlNode OutputNode = OutputsNode.SelectSingleNode(String.Concat("./Output", OutputNumber.ToString().Trim()));
                    if (OutputNode != null)
                    {
                        return Convert.ToDouble(OutputNode.SelectSingleNode("./HighLimit").InnerText.Replace('.',','));
                    }
                    else
                    {
                        return 99999999.9;
                    }                    
                }
            }
            return retValue;
        }
        public static String GetInputVoltage(String TestType, String ProductID)
        {
            String retValue = "N/A";
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    XmlNode InputVoltageNode = actProductNode.SelectSingleNode("./InputVoltage");
                    if (InputVoltageNode != null)
                    {
                        return InputVoltageNode.InnerText.Trim();
                    }
                }
            }
            return retValue;
        }
        public static Int16 GetBurninTime(String TestType, String ProductID)
        {
            String BurnInProfile = "";
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    XmlNode BurnInProfileNode = actProductNode.SelectSingleNode("./BIProfile");
                    if (BurnInProfileNode != null)
                    {
                        BurnInProfile = BurnInProfileNode.InnerText;
                        break;
                    }
                }
            }

            XmlNode BIProfilesNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/BIProfiles"));
            foreach (XmlNode actProfileNode in BIProfilesNode)
            {
                if (actProfileNode.Name == BurnInProfile)
                {
                    return Convert.ToInt16(actProfileNode.SelectSingleNode("./BITime").InnerText.ToString());
                }
            }
            return 0;
        }
        public static Array GetBurninProfile(String TestType, String ProductID)
        {
            BurnProfileStep[] retArray = { };

            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    XmlNode BIProfileNode = actProductNode.SelectSingleNode("./BIProfile");
                    if (BIProfileNode != null)
                    {
                        String str_BIProfileName = BIProfileNode.InnerText;
                        XmlNode BIProfilesNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/BIProfiles"));
                        if (BIProfilesNode == null) return retArray;
                        XmlNode actBIProfileNameNode = BIProfilesNode.SelectSingleNode(String.Concat("./", str_BIProfileName));
                        if (actBIProfileNameNode == null) return retArray;
                        uint n_BufferStartTimeInSeconds = 0;
                        foreach (XmlNode actBIPhaseNode in actBIProfileNameNode.SelectSingleNode("./Profile"))
                        {
                            BurnProfileStep actStep = new BurnProfileStep();
                            actStep.Duration = Convert.ToUInt16(actBIPhaseNode.SelectSingleNode("./Duration").InnerText.Trim());
                            actStep.StartTime = n_BufferStartTimeInSeconds;
                            n_BufferStartTimeInSeconds += actStep.Duration;
                            actStep.InputVoltageState = Convert.ToUInt16(actBIPhaseNode.SelectSingleNode("./InputVoltage").InnerText.Trim());
                            actStep.OutputLoadState = Convert.ToUInt16(actBIPhaseNode.SelectSingleNode("./OutputLoad").InnerText.Trim());
                            Array.Resize(ref retArray, retArray.Length + 1);
                            retArray.SetValue(actStep, retArray.GetUpperBound(0));
                        }
                    }
                    else return retArray;
                }
            }            

            return retArray;
        }
        public static Int32 GetOutputLoad(String TestType, String ProductID, Int16 OutputNumber)
        {
            Int16 retValue = 0;
            ConfigFile CS = new ConfigFile();
            XmlNode ProductsNode = CS.ProductsConfig.SelectSingleNode(String.Concat("./Configuration/", TestType, "/Assemblies"));
            foreach (XmlNode actProductNode in ProductsNode.ChildNodes)
            {
                XmlNode actNameNode = actProductNode.SelectSingleNode("./ProductID");
                if (actNameNode.InnerText.Trim() == ProductID)
                {
                    XmlNode OutputsNode = actProductNode.SelectSingleNode("./Outputs");
                    XmlNode OutputNode = OutputsNode.SelectSingleNode(String.Concat("./Output", OutputNumber.ToString().Trim()));
                    if (OutputNode != null)
                    {
                        return Convert.ToInt32(OutputNode.SelectSingleNode("./Load").InnerText.Replace('.', ','));
                    }
                    else
                    {
                        return -100;
                    }
                }
            }
            return retValue;
        }
        public static String GetDatalogComPort()
        {
            String retString = "";
            ConfigFile CS = new ConfigFile();
            XmlNode DatalogerComPortNode = CS.StationConfig.SelectSingleNode("./Configuration/DatalogerCOMPort");
            retString = DatalogerComPortNode.InnerText.Trim();
            return retString;
        }
        public static Boolean GetShelfEnableStatus(Int16 ShelfNumber)
        {
            Boolean retValue = false;
            ConfigFile CS = new ConfigFile();
            XmlNode ShelvesNode = CS.StationConfig.SelectSingleNode("./Configuration/Shelves");
            String ShelfNodeName = String.Concat("S0", ShelfNumber.ToString().Trim());
            if (ShelvesNode.SelectSingleNode(String.Concat("./", ShelfNodeName, "/active")).InnerText.Trim() == "1")
                retValue = true;
            else
                retValue = false;
            return retValue;
        }

        public enum SigmaSureParameterName
        {
            StationName = 1,
            GUID = 2,
            Mode = 3
        }

        public static String GetSigmaSureParameterValue(SigmaSureParameterName SSParName)
        {
            String retValue = "";
            ConfigFile CS = new ConfigFile();
            XmlNode SigmaSureNode = CS.StationConfig.SelectSingleNode("./Configuration/SigmaSure");
            retValue = SigmaSureNode.SelectSingleNode(String.Concat("./", Enum.GetName(typeof(SigmaSureParameterName), SSParName))).InnerText.Trim();
            return retValue;
        }
    }
}
