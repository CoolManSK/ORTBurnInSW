using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using PowerOne.DataLogger;
using PowerOne;
//using SigmaSure;

namespace ORTBurnInSW
{
    class GB_BurnInShelf : GroupBox
    {
        private Boolean b_AddMeasurement = false;

        private Color _borderColor = Color.Black;
        public ComboBox cb_TestType;
        public ComboBox cb_ProductID;
        private Button btn_WriteSN;
        private Button btn_Cancel;
        private Button btn_StartBI;
        private Button btn_StopBI;
        public Array ar_Products;
        
        public GB_CommonShelfComponents gb_CommonShelfComponents;
        private System.Windows.Forms.Timer t_MainTimer;
        private bool b_StartStopResetEnabler = true;

        public int n_ShelfNumber;

        public DataLog myDataLog;
        public DataLogCardDevice myDLCardDevice;

        private GPIB myGPIB;

        private ConfigFile.BurnProfileStep[] BurnProfile;

        public Color BorderColor
        {
            get { return this._borderColor; }
            set { this._borderColor = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //get the text size in groupbox
            Size tSize = TextRenderer.MeasureText(this.Text, this.Font);

            Rectangle borderRect = this.ClientRectangle;
            borderRect.Y = (borderRect.Y + (tSize.Height / 2));
            borderRect.Height = (borderRect.Height - (tSize.Height / 2));
            ControlPaint.DrawBorder(e.Graphics, borderRect, this._borderColor, ButtonBorderStyle.Solid);

            Rectangle textRect = this.ClientRectangle;
            textRect.X = (textRect.X + 6);
            textRect.Width = tSize.Width;
            textRect.Height = tSize.Height;
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), textRect);
            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), textRect);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.gb_CommonShelfComponents.Size = new Size(this.Width - 1100, this.Height - 14);
            this.gb_CommonShelfComponents.Location = new Point(this.Width - this.gb_CommonShelfComponents.Width - 5, 10);
        }

        public GB_BurnInShelf(Point Location, ref DataLog myDataloger, ref GPIB myGPIB)
        {            
            this.myDataLog = myDataloger; 

            this.myGPIB = myGPIB;
            this.actualBISecond = 0;

            this.Location = Location;

            this.cb_TestType = new ComboBox();
            this.cb_TestType.Name = "cb_TestType";
            this.cb_TestType.Location = new Point(6, 19);
            this.cb_TestType.Size = new Size(70, 21);
            this.cb_TestType.DropDownStyle = ComboBoxStyle.DropDownList;
            Array TestTypes = ConfigFile.GetTestTypes();
            foreach (String actTestType in TestTypes)
            {
                this.cb_TestType.Items.Add(actTestType.ToString().Trim());
            }
            this.cb_TestType.SelectedIndex = 0;
            this.cb_TestType.SelectedIndexChanged += new System.EventHandler(this.cb_TestType_SelectedIndexChanged);
            this.cb_TestType.Parent = this;

            this.cb_ProductID = new ComboBox();
            this.cb_ProductID.Name = "cb_ProductID";
            this.cb_ProductID.Location = new Point(this.cb_TestType.Location.X + this.cb_TestType.Width + 10, this.cb_TestType.Location.Y);
            this.cb_ProductID.Size = new Size(198, 21);
            this.cb_ProductID.DropDownStyle = ComboBoxStyle.DropDownList;
            Array ProductList = ConfigFile.GetProductsNames(this.cb_TestType.Text.Trim());
            foreach (String actProd in ProductList)
            {
                this.cb_ProductID.Items.Add(actProd.ToString().Trim());
            }
            this.cb_ProductID.SelectedIndexChanged += new System.EventHandler(this.cb_ProductID_SelectedIndexChanged);
            this.cb_ProductID.Parent = this;

            this.btn_WriteSN = new Button();
            this.btn_WriteSN.Name = "btn_WriteSN";
            this.btn_WriteSN.Text = "Zapis seriovych cisiel";
            this.btn_WriteSN.Size = new Size(150, 21);
            this.btn_WriteSN.Location = new Point(this.cb_ProductID.Location.X + this.cb_ProductID.Width + 10, this.cb_ProductID.Location.Y);
            this.btn_WriteSN.Enabled = false;
            this.btn_WriteSN.Click += new System.EventHandler(this.btn_WriteSN_Click);
            this.btn_WriteSN.Parent = this;

            this.btn_Cancel = new Button();
            this.btn_Cancel.Name = "btn_CancelBI";
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.Size = new Size(100, 21);
            this.btn_Cancel.Location = new Point(this.btn_WriteSN.Location.X + this.btn_WriteSN.Width + 10, this.cb_ProductID.Location.Y);
            this.btn_Cancel.Enabled = false;
            this.btn_Cancel.Click += new EventHandler(this.btn_Cancel_Click);
            this.btn_Cancel.Parent = this;

            this.btn_StartBI = new Button();
            this.btn_StartBI.Name = "btn_StartBI";
            this.btn_StartBI.Text = "Start";
            this.btn_StartBI.Size = new Size(100, 21);
            this.btn_StartBI.Location = new Point(this.btn_Cancel.Location.X + this.btn_Cancel.Width + 10, this.cb_ProductID.Location.Y);
            this.btn_StartBI.Enabled = false;
            this.btn_StartBI.Click += new EventHandler(this.btn_StartBI_Click);
            this.btn_StartBI.Parent = this;

            this.btn_StopBI = new Button();
            this.btn_StopBI.Name = "btn_StopBI";
            this.btn_StopBI.Text = "Stop";
            this.btn_StopBI.Size = new Size(100, 21);
            this.btn_StopBI.Location = new Point(this.btn_StartBI.Location.X + this.btn_StartBI.Width + 10, this.cb_ProductID.Location.Y);
            this.btn_StopBI.Enabled = false;
            this.btn_StopBI.Click += new EventHandler(this.btn_StopBI_Click);
            this.btn_StopBI.Parent = this;

            this.gb_CommonShelfComponents = new GB_CommonShelfComponents();
            this.gb_CommonShelfComponents.Name = "gb_ShelfComponents";
            this.gb_CommonShelfComponents.Size = new Size(600, this.Height - 10);
            this.gb_CommonShelfComponents.Location = new Point(this.Width - 55, 5);
            this.gb_CommonShelfComponents.Parent = this;

            this.t_MainTimer = new System.Windows.Forms.Timer();
            this.t_MainTimer.Interval = 100;
            this.t_MainTimer.Tick += new System.EventHandler(this.t_MainTimer_Tick);
            this.t_MainTimer.Stop();            
        }

        private void cb_TestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            while (this.Controls.Count > 7)
            {
                foreach (Control actControl in this.Controls)
                {
                    if (actControl.Name == "") actControl.Dispose();
                }
            }

            this.cb_ProductID.Items.Clear();
            Array Products = ConfigFile.GetProductsNames(this.cb_TestType.Text);
            
            foreach (String actProd in Products)
            {
                this.cb_ProductID.Items.Add(actProd);
            }

            this.cb_ProductID.SelectedIndex = -1;
            this.btn_Cancel.Enabled = false;
            this.btn_StartBI.Enabled = false;
            this.btn_StopBI.Enabled = false;
            this.btn_WriteSN.Enabled = false;
            this.t_MainTimer.Stop();

            this.gb_CommonShelfComponents.Reset();
        }

        private void cb_ProductID_SelectedIndexChanged(object sender, EventArgs e)
        {
            while (this.Controls.Count > 7)
            {
                foreach (Control actControl in this.Controls)
                {
                    if (actControl.Name == "") actControl.Dispose();
                }
            }

            if (this.cb_ProductID.SelectedIndex < 0) return;
            this.btn_WriteSN.Enabled = true;            

            Int16 NumberOfUnits = ConfigFile.GetNumberOfUnitsInShelf(this.cb_TestType.Text, this.cb_ProductID.Text);
            this.ar_Products = Array.CreateInstance(typeof(ProductComponents), NumberOfUnits);
            for (Int16 i = 0; i < NumberOfUnits; i++)
            {
                ProductComponents actProdComps = new ProductComponents(this.cb_TestType.Text, String.Concat("Position ", (i + 1).ToString().Trim()));
                Int32 d_XLocation = (int)(5 * (i + 1) + (Convert.ToDouble(i) / Convert.ToDouble(NumberOfUnits) * (this.Width - this.gb_CommonShelfComponents.Width - 5 - (5 * (NumberOfUnits + 1)))));
                Int32 d_Width = (this.Width - this.gb_CommonShelfComponents.Width - 5 - (5 * (NumberOfUnits + 1))) / NumberOfUnits;

                actProdComps.ChangeXLocationAndWidth(d_XLocation, d_Width);
                //actProdComps.ChangeWidth((this.Width - (5 * (NumberOfUnits + 1))) / NumberOfUnits);
                
                ar_Products.SetValue(actProdComps, i);
            }
            
            this.gb_CommonShelfComponents.ProductIdSelected(this.cb_TestType.Text, this.cb_ProductID.Text);
            this.TotalBurnInTime = ConfigFile.GetBurninTime(this.cb_TestType.Text, this.cb_ProductID.Text);
            this.gb_CommonShelfComponents.UpdateTime(this.TotalBurnInTime, this.b_StartStopResetEnabler);
            if (this.cb_ProductID.SelectedIndex == -1)
            {
                this.t_MainTimer.Stop();
                this.btn_WriteSN.Enabled = false;
                this.gb_CommonShelfComponents.SetStatus("Vyber Produkt");
            }
            else
            {
                this.t_MainTimer.Start();
                this.btn_WriteSN.Enabled = true;
                this.gb_CommonShelfComponents.SetStatus("Zadaj seriove cisla");
            }

            Double d_LowLimit = ConfigFile.GetLowLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 1);
            String str_LowLimit = "";
            if (d_LowLimit != 99999999.9)
            {
                str_LowLimit = String.Concat(ConfigFile.GetLowLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 1).ToString().Trim(), " V");
            }
            else
            {
                str_LowLimit = "N/A";
            }

            Double d_HighLimit = ConfigFile.GetHighLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 1);
            String str_HighLimit = "";
            if (d_HighLimit != 99999999.9)
            {
                str_HighLimit = String.Concat(ConfigFile.GetHighLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 1).ToString().Trim(), " V");
            }
            else
            {
                str_HighLimit = "N/A";
            }

            Double d_LowLimit2 = ConfigFile.GetLowLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 2);
            String str_LowLimit2 = "";
            if (d_LowLimit2 != 99999999.9)
            {
                str_LowLimit2 = String.Concat(ConfigFile.GetLowLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 2).ToString().Trim(), " V");
            }
            else
            {
                str_LowLimit2 = "N/A";
            }

            Double d_HighLimit2 = ConfigFile.GetHighLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 2);
            String str_HighLimit2 = "";
            if (d_HighLimit2 != 99999999.9)
            {
                str_HighLimit2 = String.Concat(ConfigFile.GetHighLimit(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 2).ToString().Trim(), " V");
            }
            else
            {
                str_HighLimit2 = "N/A";
            }

            foreach (ProductComponents actPC in ar_Products)
            {
                actPC.lbl_LowLimit.Text = str_LowLimit;
                actPC.lbl_HighLimit.Text = str_HighLimit;
                actPC.lbl_LowLimit2.Text = str_LowLimit2;
                actPC.lbl_HighLimit2.Text = str_HighLimit2;
                foreach (Control actControl in actPC)
                {
                    if (actControl != null) actControl.Parent = this;
                }
                actPC.PartNo = this.cb_ProductID.Text;
            }
        }

        private void btn_WriteSN_Click(object sender, EventArgs e)
        {
            for (Int16 i = 0; i < this.ar_Products.Length; i++)
            {
                SerialNumberInsertForm myQuestionForm = new SerialNumberInsertForm(String.Concat("Zadaj SN pre poziciu ", (i + 1).ToString().Trim()), "Zadavanie SN", this.cb_ProductID.Text.Trim());
                myQuestionForm.ShowDialog();
                ProductComponents actSN = (ProductComponents)this.ar_Products.GetValue(i);
                if (myQuestionForm.CancelFlag) break;
                actSN.lbl_SerialNumber.Text = String.Concat("SN: ", myQuestionForm.Answer);
                actSN.SerialNumber = myQuestionForm.Answer;                                
                myQuestionForm.Dispose();
            }
            Boolean b_sncomplete = true;
            foreach (ProductComponents actPC in this.ar_Products)
            {
                if (actPC.lbl_SerialNumber.Text == "") b_sncomplete = false;
            }
            if (b_sncomplete)
            {
                this.cb_ProductID.Enabled = false;
                this.btn_StartBI.Enabled = true;
                this.btn_Cancel.Enabled = true;
                this.btn_WriteSN.Enabled = false;
                this.gb_CommonShelfComponents.SetStatus("Ready to start Burn In test");
            }
        }

        private void btn_StartBI_Click(object sender, EventArgs e)
        {
            this.actualCycle = 1;            
            
            this.b_StartStopResetEnabler = false;
            this.btn_StartBI.Enabled = false;
            this.btn_StopBI.Enabled = true;
            this.btn_Cancel.Enabled = false;
            
            this.BurnProfile = (ConfigFile.BurnProfileStep[])ConfigFile.GetBurninProfile(this.cb_TestType.Text, this.cb_ProductID.Text.Trim());
            this.TotalBurnInTime = ConfigFile.GetBurninTime(this.cb_TestType.Text, this.cb_ProductID.Text.Trim());
            this.n_BITimeInSeconds = Convert.ToUInt64(this.TotalBurnInTime * 3600);
            this.n_CycleTimeInSeconds = 0;
            this.secondsLastOutputVoltageCheck = 0;
            foreach (ConfigFile.BurnProfileStep actStep in this.BurnProfile)
            {
                n_CycleTimeInSeconds += actStep.Duration;
            }
            this.gb_CommonShelfComponents.SetStatus("Test is running");

            foreach (ProductComponents actSN in this.ar_Products)
            {
                
                actSN.starttime = DateTime.Now;
            }
            

            //this.myGPIB = new GPIB();

            this.t_MainTimer.Start();
        }

        private void btn_StopBI_Click(object sender, EventArgs e)
        {            
            this.btn_StopBI.Enabled = false;
            this.btn_Cancel.Enabled = true;
            //this.btn_StartBI.Enabled = true;
            this.t_MainTimer.Stop();
            this.b_StartStopResetEnabler = true;
            this.myGPIB.SetLoad(Convert.ToInt32(this.Text.Substring(5)), 0);
            this.actualOutputLoadState = 0;
            Thread.Sleep(500);
            this.myDataLog.SetRelayChannels(this.myDLCardDevice, DataLogRelayChannel.No01, 0);
            this.actualInputVoltageState = 0;
            foreach (ProductComponents actPC in this.ar_Products)
            {
                actPC.ScrapReport(this.cb_TestType.Text);
            }
            this.gb_CommonShelfComponents.SetLabelColors(0, 0, 0);
            this.gb_CommonShelfComponents.SetStatus("Test have been stoped by Operator");
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.cb_ProductID.Enabled = true;
            this.btn_StartBI.Enabled = false;
            this.btn_StopBI.Enabled = false;
            this.cb_ProductID.SelectedIndex = -1;
            this.gb_CommonShelfComponents.ResetTime();
            this.btn_Cancel.Enabled = false;
        }

        private int TotalBurnInTime; //in hours
        private UInt64 actualBISecond;
        private UInt64 n_BITimeInSeconds = 0;
        private UInt64 n_WriteMeasValuesToReportInterval = 60;
        private UInt64 n_LastReportWriteInSeconds = 0;
        private UInt64 n_CycleTimeInSeconds;        

        private void t_MainTimer_Tick(object sender, EventArgs e)
        {
            UInt64 n_actSec = this.actualBISecond;
            this.actualBISecond = this.gb_CommonShelfComponents.UpdateTime(this.TotalBurnInTime, this.b_StartStopResetEnabler);
            if (n_actSec < this.actualBISecond)
            { 
                if (((UInt64)this.actualCycle * this.n_CycleTimeInSeconds) < this.actualBISecond)
                {
                    this.actualCycle++;
                }
                if (!this.b_AddMeasurement)
                {
                    if ((this.actualInputVoltageState == 0) && (this.actualOutputLoadState == 0)) this.b_AddMeasurement = true;
                }
                this.SetInputVoltageState(this.GetActualInputVoltageFromBIProfile());
                this.SetOutputLoadState(this.GetActualOutputLoadFromBIProfile());
                if (!this.b_AddMeasurement)
                {
                    if ((this.actualInputVoltageState == 1) && (this.actualOutputLoadState == 1))
                    {
                        if ((this.n_LastReportWriteInSeconds + this.n_WriteMeasValuesToReportInterval) < this.actualBISecond)
                        {
                            this.b_AddMeasurement = true;
                        }
                    }
                }
            }            
            this.gb_CommonShelfComponents.SetLabelColors(this.actualInputVoltageState, this.actualOutputLoadState, this.actualOutputLoadValue);
            this.CheckOutputsVoltage();
            if (this.actualBISecond > (Convert.ToUInt64(this.TotalBurnInTime)*3600))
            {
                this.btn_StopBI_Click(new object(), new EventArgs());
            }
        }

        private int actualCycle = 1;
        private int actualInputVoltageState = 0;
        private UInt64 secondsLastChangeOfIV = 0;

        private int SetInputVoltageState(int InputVoltageState)
        {
            if (InputVoltageState == this.actualInputVoltageState) return 0;
            if (InputVoltageState == 0)
            {
                if (this.actualOutputLoadState == 1)
                {
                    int ret = this.SetOutputLoadState(0);                    
                }
                if (this.secondsLastChangeOfOL + 1 > this.actualBISecond) return -20;
                //if (this.secondsLastChangeOfIV + 2 > this.actualBISecond) return -30;
                this.myDataLog.SetRelayChannels(this.myDLCardDevice, DataLogRelayChannel.No01, 0);
                this.actualInputVoltageState = 0;
            }
            else if (InputVoltageState == 1)
            {                                
                this.actualInputVoltageState = 1;
                this.myDataLog.SetRelayChannels(this.myDLCardDevice, DataLogRelayChannel.No01, 1);
                int RelayChannelsState = -1;                
                while (RelayChannelsState < 1)
                {
                    this.myDataLog.SetRelayChannels(this.myDLCardDevice, DataLogRelayChannel.No01, 1);
                    this.myDataLog.GetRelayChannelsState(this.myDLCardDevice, ref RelayChannelsState);
                }
            }
            this.secondsLastChangeOfIV = this.actualBISecond;
            return 0;
        }

        private int actualOutputLoadState = 0;
        private int actualOutputLoadValue = 0;
        private UInt64 secondsLastChangeOfOL = 0;

        private int SetOutputLoadState(int OutputLoadState)
        {
            if (OutputLoadState == this.actualOutputLoadState) return 0;
            if (OutputLoadState == 0)
            {
                this.myGPIB.SetLoad(Convert.ToInt32(this.Text.Substring(5)), 0);
                Thread.Sleep(250);
                //this.myDataLog.SetRelayChannels(this.myDLCardDevice, DataLogRelayChannel.No01, 0);
                this.actualOutputLoadValue = 0;
                this.actualOutputLoadState = 0;
            }
            else
            {
                if (this.actualInputVoltageState == 0) return -10;
                if (this.secondsLastChangeOfIV + 1 > this.actualBISecond) return -20;
                
                Thread.Sleep(250);
                this.myGPIB.SetLoad(this.n_ShelfNumber, ConfigFile.GetOutputLoad(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 1));
                this.actualOutputLoadValue = ConfigFile.GetOutputLoad(this.cb_TestType.Text, this.cb_ProductID.Text.Trim(), 1);
                this.actualOutputLoadState = 1;
            }
            this.secondsLastChangeOfOL = this.actualBISecond;
            return 0;
        }

        private int GetActualInputVoltageFromBIProfile()
        { 
            UInt64 n_actualBICycleSecond = this.actualBISecond - ((UInt64)(this.actualCycle - 1) * this.n_CycleTimeInSeconds);
            UInt64 n_CumulativeTimeValueSeconds = 0;
            for (int i = 0; i < this.BurnProfile.Length; i++)
            {
                n_CumulativeTimeValueSeconds += this.BurnProfile[i].Duration;
                if (n_CumulativeTimeValueSeconds > n_actualBICycleSecond)
                {
                    return this.BurnProfile[i].InputVoltageState;
                }
            }
            return -1;
        }

        private int GetActualOutputLoadFromBIProfile()
        {            
            UInt64 n_actualBICycleSecond = this.actualBISecond - ((UInt64)(this.actualCycle - 1) * this.n_CycleTimeInSeconds);
            UInt64 n_CumulativeTimeValueSeconds = 0;
            for (int i = 0; i < this.BurnProfile.Length; i++)
            {
                n_CumulativeTimeValueSeconds += this.BurnProfile[i].Duration;
                if (n_CumulativeTimeValueSeconds > n_actualBICycleSecond)
                {
                    return this.BurnProfile[i].OutputLoadState;
                }
            }
            return -1;
        }

        private UInt64 secondsLastOutputVoltageCheck = 0;

        private int CheckOutputsVoltage()
        {
            if (this.actualInputVoltageState == 0) return -10;
            if (this.actualOutputLoadState == 0) return -20;
            if (this.secondsLastChangeOfIV + 3 > this.actualBISecond) return -30;
            if (this.secondsLastChangeOfOL + 3 > this.actualBISecond) return -40;
            if (this.secondsLastOutputVoltageCheck + 20 > this.actualBISecond) return -50;

            int RelayChannelsState = -1;            
            while (RelayChannelsState < 1)
            {
                this.myDataLog.SetRelayChannels(this.myDLCardDevice, DataLogRelayChannel.No01, 1);
                this.myDataLog.GetRelayChannelsState(this.myDLCardDevice, ref RelayChannelsState);
            }

            Array measValues = Array.CreateInstance(typeof(Double), 16);
            this.myDataLog.MeasureChannels(this.myDLCardDevice, DataLogMeasChannel.No01, (DataLogMeasChannel)(this.ar_Products.Length - 1), ref measValues);
            int n_numberOfOutputs = ConfigFile.GetNumberOfOutputs(this.cb_TestType.Text, this.cb_ProductID.Text.Trim());
            for (int i = 0; i < ar_Products.Length; i++)            
            {
                ProductComponents actProduct = (ProductComponents)ar_Products.GetValue(i);
                Double d_lowLimit = Convert.ToDouble(actProduct.lbl_LowLimit.Text.Substring(0, actProduct.lbl_LowLimit.Text.Length - 2));
                Double d_highLimit = Convert.ToDouble(actProduct.lbl_HighLimit.Text.Substring(0, actProduct.lbl_HighLimit.Text.Length - 2));
                /*
                Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                Thread.Sleep(1);
                Double measVal = (rnd.NextDouble() * 2.04f) + 10.98f;
                */

                Double measVal = (Double)measValues.GetValue(i * n_numberOfOutputs);

                actProduct.lbl_OutputVoltage.Text = String.Concat(measVal, " V");
                if (actProduct.lbl_OutputVoltage.BackColor != Color.Red)
                {
                    if ((measVal < d_lowLimit) || (measVal > d_highLimit))
                    {
                        actProduct.lbl_OutputVoltage.BackColor = Color.Red;
                        actProduct.AddMeasurement(1, DateTime.Now, Math.Round(measVal, 2));
                        this.n_LastReportWriteInSeconds = this.actualBISecond;
                    }
                    else
                    {
                        actProduct.lbl_OutputVoltage.BackColor = Color.Green;
                        if (this.b_AddMeasurement)
                        {
                            actProduct.AddMeasurement(1, DateTime.Now, Math.Round(measVal, 2));
                            this.n_LastReportWriteInSeconds = this.actualBISecond;
                            if ((n_numberOfOutputs == 1) && (i == (ar_Products.Length - 1)))
                            {
                                this.b_AddMeasurement = false;
                            }
                        }
                    }
                }
                else
                {
                    if (this.b_AddMeasurement)
                    {
                        actProduct.AddMeasurement(1, DateTime.Now, Math.Round(measVal, 2));
                        this.n_LastReportWriteInSeconds = this.actualBISecond;
                        if ((n_numberOfOutputs == 1) && (i == (ar_Products.Length - 1)))
                        {
                            this.b_AddMeasurement = false;
                        }
                    }
                }
                

                if (n_numberOfOutputs > 1)
                {
                    Double d_lowLimit2 = Convert.ToDouble(actProduct.lbl_LowLimit2.Text.Substring(0, actProduct.lbl_LowLimit2.Text.Length - 2));
                    Double d_highLimit2 = Convert.ToDouble(actProduct.lbl_HighLimit2.Text.Substring(0, actProduct.lbl_HighLimit2.Text.Length - 2));

                    Double measVal2 = (Double)measValues.GetValue((i * n_numberOfOutputs) + 1);
                    /*
                    Thread.Sleep(1);
                    Random rnd2 = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                    Double measVal2 = (rnd2.NextDouble() * 2.04f) + 10.98f;
                    */                    
                    actProduct.lbl_OutputVoltage2.Text = String.Concat(Math.Round(measVal2, 2).ToString(), " V");

                    if (actProduct.lbl_OutputVoltage2.BackColor != Color.Red)
                    {
                        if ((measVal2 < d_lowLimit2) || (measVal2 > d_highLimit2))
                        {
                            actProduct.lbl_OutputVoltage2.BackColor = Color.Red;
                            actProduct.AddMeasurement(2, DateTime.Now, Math.Round(measVal2, 2));                            
                        }
                        else
                        {
                            actProduct.lbl_OutputVoltage2.BackColor = Color.Green;
                            if (this.b_AddMeasurement)
                            {
                                actProduct.AddMeasurement(2, DateTime.Now, Math.Round(measVal2, 2));
                                if (i == (ar_Products.Length - 1))
                                {
                                    this.b_AddMeasurement = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.b_AddMeasurement)
                        {
                            actProduct.AddMeasurement(2, DateTime.Now, Math.Round(measVal2, 2));
                            if (i == (ar_Products.Length - 1))
                            {
                                this.b_AddMeasurement = false;
                            }
                        }
                    }
                }
                else
                {
                    actProduct.lbl_OutputVoltage2.Text = "N/A";
                }
            }
            this.secondsLastOutputVoltageCheck = this.actualBISecond;

            return 0;
        }
    }
}
