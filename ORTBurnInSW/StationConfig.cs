using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ORTBurnInSW
{
    class StationConfig
    {
        private String StationConfigFileName = "StationConfig.xml";
        private XmlDocument StationConfigFile = new XmlDocument();
        public StationConfig()
        {
            this.StationConfigFile.Load(this.StationConfigFileName);
        }
        /*
        public static bool ShelfIsActive(int ShelfNumber)
        {
            StationConfig mySC = new StationConfig();
            String ShelfName = String.Concat("S0", ShelfNumber.ToString().Trim());
            XmlNode actShelfNode = mySC.StationConfigFile.SelectSingleNode("Configuration\")
            return false;
        }
        */
    }
}
