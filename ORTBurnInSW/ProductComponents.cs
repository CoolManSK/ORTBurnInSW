using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using PowerOne.DataLogger;

namespace ORTBurnInSW
{    
    public class ProductComponents
    {
        public Label lbl_PositionCaption;
        public Label lbl_SerialNumber;
        public Label lbl_OutputVoltage;
        public Label lbl_LowLimit;
        public Label lbl_HighLimit;
        public Label lbl_OutputVoltage2;
        public Label lbl_LowLimit2;
        public Label lbl_HighLimit2;

        
        private SigmaSureReport Report;

        public int AddMeasurement(int OutputNumber, DateTime TimeOfMeasurement, Double MeasValue)
        {
            Double LowLimit = Convert.ToDouble(this.lbl_LowLimit.Text.Replace('V', ' ').Trim());
            Double HighLimit = Convert.ToDouble(this.lbl_HighLimit.Text.Replace('V', ' ').Trim());
            if (OutputNumber == 2)
            {
                LowLimit = Convert.ToDouble(this.lbl_LowLimit2.Text.Replace('V', ' ').Trim());
                HighLimit = Convert.ToDouble(this.lbl_HighLimit2.Text.Replace('V', ' ').Trim());
            }
            this.Report.AddMeasurement(OutputNumber, MeasValue, LowLimit, HighLimit, TimeOfMeasurement);

            return 0;
        }

        public void ScrapReport(String TestType)
        {
            this.Report.ScrapMeasurements(TestType);
        }

        public IEnumerator<Control> GetEnumerator()
        {
            yield return this.lbl_PositionCaption;
            yield return this.lbl_SerialNumber;
            yield return this.lbl_OutputVoltage;
            yield return this.lbl_LowLimit;
            yield return this.lbl_HighLimit;
            yield return this.lbl_OutputVoltage2;
            yield return this.lbl_LowLimit2;
            yield return this.lbl_HighLimit2;
        }

        public ProductComponents(String TestType, String PositionCaption)
        {
            this.lbl_PositionCaption = new Label();
            this.lbl_PositionCaption.Text = PositionCaption;
            this.lbl_PositionCaption.Location = new Point(5, 45);
            this.lbl_PositionCaption.AutoSize = false;
            this.lbl_PositionCaption.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_PositionCaption.Height = 16;
            this.lbl_PositionCaption.BorderStyle = BorderStyle.None;

            this.lbl_SerialNumber = new Label();
            this.lbl_SerialNumber.Text = "";
            this.lbl_SerialNumber.Location = new Point(5, 62);
            this.lbl_SerialNumber.AutoSize = false;
            this.lbl_SerialNumber.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_SerialNumber.Height = 16;
            this.lbl_SerialNumber.BorderStyle = BorderStyle.FixedSingle;

            this.lbl_OutputVoltage = new Label();
            this.lbl_OutputVoltage.Text = "";
            this.lbl_OutputVoltage.Location = new Point(5, 77);
            this.lbl_OutputVoltage.AutoSize = false;
            this.lbl_OutputVoltage.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_OutputVoltage.Height = 16;
            this.lbl_OutputVoltage.BorderStyle = BorderStyle.FixedSingle;

            this.lbl_LowLimit = new Label();
            this.lbl_LowLimit.Text = "";
            this.lbl_LowLimit.Location = new Point(5, 77);
            this.lbl_LowLimit.AutoSize = false;
            this.lbl_LowLimit.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_LowLimit.Height = 16;
            this.lbl_LowLimit.BorderStyle = BorderStyle.FixedSingle;

            this.lbl_HighLimit = new Label();
            this.lbl_HighLimit.Text = "";
            this.lbl_HighLimit.Location = new Point(5, 77);
            this.lbl_HighLimit.AutoSize = false;
            this.lbl_HighLimit.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_HighLimit.Height = 16;
            this.lbl_HighLimit.BorderStyle = BorderStyle.FixedSingle;

            this.lbl_OutputVoltage2 = new Label();
            this.lbl_OutputVoltage2.Text = "";
            this.lbl_OutputVoltage2.Location = new Point(5, 92);
            this.lbl_OutputVoltage2.AutoSize = false;
            this.lbl_OutputVoltage2.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_OutputVoltage2.Height = 16;
            this.lbl_OutputVoltage2.BorderStyle = BorderStyle.FixedSingle;

            this.lbl_LowLimit2 = new Label();
            this.lbl_LowLimit2.Text = "";
            this.lbl_LowLimit2.Location = new Point(5, 92);
            this.lbl_LowLimit2.AutoSize = false;
            this.lbl_LowLimit2.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_LowLimit2.Height = 16;
            this.lbl_LowLimit2.BorderStyle = BorderStyle.FixedSingle;

            this.lbl_HighLimit2 = new Label();
            this.lbl_HighLimit2.Text = "";
            this.lbl_HighLimit2.Location = new Point(5, 92);
            this.lbl_HighLimit2.AutoSize = false;
            this.lbl_HighLimit2.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_HighLimit2.Height = 16;
            this.lbl_HighLimit2.BorderStyle = BorderStyle.FixedSingle;

            this.Report = new SigmaSureReport(TestType);


        }

        public void ChangeXLocationAndWidth(Int32 newXLoc, Int32 newWidth)
        {
            
            this.lbl_PositionCaption.Location = new Point(newXLoc, this.lbl_PositionCaption.Location.Y);
            this.lbl_SerialNumber.Location = new Point(newXLoc, this.lbl_SerialNumber.Location.Y);

            this.ChangeWidth(newWidth);

            this.lbl_LowLimit.Location = new Point(newXLoc, this.lbl_LowLimit.Location.Y);            
            this.lbl_OutputVoltage.Location = new Point(newXLoc + this.lbl_LowLimit.Width - 1, this.lbl_OutputVoltage.Location.Y);
            this.lbl_HighLimit.Location = new Point(newXLoc + this.lbl_LowLimit.Width + this.lbl_OutputVoltage.Width - 2, this.lbl_HighLimit.Location.Y);

            this.lbl_LowLimit2.Location = new Point(newXLoc, this.lbl_LowLimit2.Location.Y);
            this.lbl_OutputVoltage2.Location = new Point(newXLoc + this.lbl_LowLimit2.Width - 1, this.lbl_OutputVoltage2.Location.Y);
            this.lbl_HighLimit2.Location = new Point(newXLoc + this.lbl_LowLimit2.Width + this.lbl_OutputVoltage2.Width - 2, this.lbl_HighLimit2.Location.Y);

        }

        public void ChangeWidth(Int32 newWidth)
        {
            this.lbl_PositionCaption.Width = newWidth;
            this.lbl_SerialNumber.Width = newWidth;

            Int16 VoltageControlsWidth = Convert.ToInt16(Math.Round((double)newWidth / 3, MidpointRounding.ToEven));
            this.lbl_LowLimit.Width = VoltageControlsWidth;
            this.lbl_HighLimit.Width = VoltageControlsWidth;
            this.lbl_OutputVoltage.Width = newWidth - (2 * VoltageControlsWidth) + 2;
            this.lbl_LowLimit2.Width = VoltageControlsWidth;
            this.lbl_HighLimit2.Width = VoltageControlsWidth;
            this.lbl_OutputVoltage2.Width = newWidth - (2 * VoltageControlsWidth) + 2;
        }

        public String SerialNumber
        {
            set
            {
                this.Report.SerialNumber = value;
            }
            get
            {
                return this.Report.SerialNumber;
            }
        }

        public String PartNo
        {
            set
            {
                this.Report.PartNo = this.PartNo;
            }
            get
            {
                return this.Report.PartNo;
            }
        }

        public DateTime starttime
        {
            set
            {
                this.Report.starttime = value;
            }
            get
            {
                return this.Report.starttime;
            }
        }

        public DateTime endtime
        {
            set
            {
                this.Report.endtime = value;
            }
            get
            {
                return this.Report.endtime;
            }
        }
    }
}
