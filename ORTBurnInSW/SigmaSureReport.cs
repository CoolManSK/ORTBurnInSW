using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SigmaSure;

namespace ORTBurnInSW
{
    class SigmaSureReport
    {
        UnitReport UR = new UnitReport();

        public SigmaSureReport(String TestType)
        {
            this.UR.Station.name = ConfigFile.GetSigmaSureParameterValue(ConfigFile.SigmaSureParameterName.StationName);
            this.UR.Station.guid = ConfigFile.GetSigmaSureParameterValue(ConfigFile.SigmaSureParameterName.GUID);
            this.UR.mode = ConfigFile.GetSigmaSureParameterValue(ConfigFile.SigmaSureParameterName.Mode);
            this.UR.TestRun = new _TestRun();
            this.UR.TestRun.name = TestType;
        }

        public void ScrapMeasurements(String TestType)
        {
            this.UR.TestRun = new _TestRun();
            this.UR.TestRun.name = TestType;
        }

        public string SerialNumber
        {
            set
            {
                this.UR.Cathegory.Product.SerialNo = value;
                this.UR.AddProperty("Work Order", value.Substring(0, 8));
            }
            get
            {
                return this.UR.Cathegory.Product.SerialNo;
            }
        }

        public string PartNo
        {
            set
            {
                this.UR.Cathegory.Product.PartNo = value;
            }
            get
            {
                return this.UR.Cathegory.Product.PartNo;
            }
        }

        public string Operator
        {
            set
            {
                this.UR.Operator.name = value;
            }
            get
            {
                return UR.Operator.name;
            }
        }

        public DateTime starttime
        {
            set
            {
                this.UR.starttime = value;
            }
            get
            {
                return this.UR.starttime;
            }
        } 

        public DateTime endtime
        {
            set
            {
                this.UR.endtime = value;
            }
            get
            {
                return this.UR.endtime;
            }
        }  
        
        public void AddMeasurement(int OutputNumber, Double MeasValue, Double LowLimit, Double HighLimit, DateTime TimeStamp)
        {
            this.UR.TestRun.AddTestRunChild(String.Concat("Vo", OutputNumber.ToString().Trim()),
                TimeStamp,
                TimeStamp,
                "",
                "V",
                MeasValue,
                LowLimit,
                HighLimit);
            this.UR.GetXMLReport("C://temp//temp//", true);
        }
    }
}
