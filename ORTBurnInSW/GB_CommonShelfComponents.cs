using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ORTBurnInSW
{
    public partial class GB_CommonShelfComponents : GroupBox
    {
        private Color _borderColor = Color.Gray;
        private Label lbl_Status;
        private Label lbl_InputVoltage;
        private Label lbl_Temperature;
        private Label lbl_Output;
        private Label lbl_Time_Start;
        private Label lbl_Time_Actual;
        private Label lbl_Time_Stop;        

        public Color BorderColor
        {
            get { return this._borderColor; }
            set { this._borderColor = value; }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int n_ControlWidth = this.Width - 6;
            int n_Xposition = 3;

            this.lbl_Status.Size = new Size(n_ControlWidth, 18);
            this.lbl_Status.Location = new Point(n_Xposition, 3);
            this.lbl_InputVoltage.Size = new Size(n_ControlWidth, 18);
            this.lbl_InputVoltage.Location = new Point(n_Xposition, 20);
            this.lbl_Temperature.Size = new Size(n_ControlWidth / 2 + 1, 18);
            this.lbl_Temperature.Location = new Point(n_Xposition, 37);
            this.lbl_Output.Size = new Size(n_ControlWidth / 2 + 1, 18);
            this.lbl_Output.Location = new Point(n_Xposition + this.lbl_Output.Width - 1, 37);
            this.lbl_Time_Start.Size = new Size(n_ControlWidth, 15);
            this.lbl_Time_Start.Location = new Point(n_Xposition, 56);
            this.lbl_Time_Actual.Size = new Size(n_ControlWidth, 15);
            this.lbl_Time_Actual.Location = new Point(n_Xposition, 70);
            this.lbl_Time_Stop.Size = new Size(n_ControlWidth, 15);
            this.lbl_Time_Stop.Location = new Point(n_Xposition, 84);
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
        public GB_CommonShelfComponents()
        {
            Font font_Default = new Font(this.Font.FontFamily, 9.0f);

            this.lbl_Status = new Label();
            this.lbl_Status.AutoSize = false;            
            this.lbl_Status.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_Status.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_Status.Font = new Font(font_Default.FontFamily, 7.0f); ;
            this.lbl_Status.Text = "Status: Unknown product type";
            this.lbl_Status.Parent = this;

            this.lbl_InputVoltage = new Label();
            this.lbl_InputVoltage.AutoSize = false;
            this.lbl_InputVoltage.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_InputVoltage.TextAlign = ContentAlignment.TopCenter;
            this.lbl_InputVoltage.Font = font_Default;
            this.lbl_InputVoltage.Text = "Input Voltage: N/A";
            this.lbl_InputVoltage.Parent = this;

            this.lbl_Temperature = new Label();
            this.lbl_Temperature.AutoSize = false;
            this.lbl_Temperature.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_Temperature.TextAlign = ContentAlignment.TopCenter;
            this.lbl_Temperature.Font = font_Default;
            this.lbl_Temperature.Text = "0 °C";
            this.lbl_Temperature.Parent = this;

            this.lbl_Output = new Label();
            this.lbl_Output.AutoSize = false;
            this.lbl_Output.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_Output.TextAlign = ContentAlignment.TopCenter;
            this.lbl_Output.Font = font_Default;
            this.lbl_Output.Text = "0 A";
            this.lbl_Output.Parent = this;

            this.lbl_Time_Start = new Label();
            this.lbl_Time_Start.AutoSize = false;
            this.lbl_Time_Start.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_Time_Start.TextAlign = ContentAlignment.TopCenter;
            this.lbl_Time_Start.Font = new Font(font_Default.FontFamily, 8.0f);
            this.lbl_Time_Start.Text = "Start:";
            this.lbl_Time_Start.Parent = this;

            this.lbl_Time_Actual = new Label();
            this.lbl_Time_Actual.AutoSize = false;
            this.lbl_Time_Actual.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_Time_Actual.TextAlign = ContentAlignment.TopCenter;
            this.lbl_Time_Actual.Font = new Font(font_Default.FontFamily, 8.0f);
            this.lbl_Time_Actual.Text = "Actual:";
            this.lbl_Time_Actual.Parent = this;

            this.lbl_Time_Stop = new Label();
            this.lbl_Time_Stop.AutoSize = false;
            this.lbl_Time_Stop.BorderStyle = BorderStyle.FixedSingle;
            this.lbl_Time_Stop.TextAlign = ContentAlignment.TopCenter;
            this.lbl_Time_Stop.Font = new Font(font_Default.FontFamily, 8.0f);
            this.lbl_Time_Stop.Text = "Stop:";
            this.lbl_Time_Stop.Parent = this;            

            InitializeComponent();
        }

        public GB_CommonShelfComponents(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void SetStatus(String newStatus)
        {
            this.lbl_Status.Text = String.Concat("Status: ", newStatus);
        }

        public void ProductIdSelected(String TestType, String ProductID)
        {
            this.lbl_InputVoltage.Text = String.Concat("Input Voltage: ", ConfigFile.GetInputVoltage(TestType, ProductID));
        }

        public UInt64 UpdateTime(int BurnInTime, bool resetStartStopTime)
        {
            String str_StartTime = DateTime.Now.ToString();
            if (resetStartStopTime)
            {
                str_StartTime = DateTime.Now.ToString();
                String str_StopTime = DateTime.Now.AddHours(BurnInTime).ToString();
                this.lbl_Time_Start.Text = String.Concat("Start: ", str_StartTime);
                this.lbl_Time_Stop.Text = String.Concat("Stop: ", str_StopTime);               
            }
            this.lbl_Time_Actual.Text = String.Concat("Actual: ", str_StartTime);
            try
            {
                return Convert.ToUInt64((Convert.ToDateTime(this.lbl_Time_Actual.Text.Trim().Substring(8)) - Convert.ToDateTime(this.lbl_Time_Start.Text.Trim().Substring(7))).TotalSeconds);
            }
            catch
            {
                return 0;
            }
        }

        public void ResetTime()
        {
            this.lbl_Time_Start.Text = "Start: ";
            this.lbl_Time_Stop.Text = "Stop: ";
            this.lbl_Time_Actual.Text = "Actual: ";
        }

        public int SetLabelColors(int InputVoltageState, int OutputLoadState, int OutputLoadValue)
        {
            if (InputVoltageState == 0)
            {
                this.lbl_InputVoltage.BackColor = Color.Blue;
            }
            else if (InputVoltageState == 1)
            {
                this.lbl_InputVoltage.BackColor = Color.Green;
            }
            if (OutputLoadState == 0)
            {
                this.lbl_Output.BackColor = Color.Blue;
            }
            else if (OutputLoadState == 1)
            {
                this.lbl_Output.BackColor = Color.Green;
            }
            this.lbl_Output.Text = String.Concat(OutputLoadValue.ToString(), " mA");
            return 0;
        }
        
        
        public void Reset()
        {
            this.lbl_Status.Text = "Status: Unknown product type";
            this.lbl_Status.BackColor = this.Parent.BackColor;
            
            this.lbl_InputVoltage.Text = "Input Voltage: N/A";
            this.lbl_InputVoltage.BackColor = this.Parent.BackColor;

            this.lbl_Temperature.Text = "0 °C";
            this.lbl_Temperature.BackColor = this.Parent.BackColor;

            this.lbl_Output.Text = "0 A";
            this.lbl_Output.BackColor = this.Parent.BackColor;

            this.lbl_Time_Start.Text = "Start:";
            this.lbl_Time_Start.BackColor = this.Parent.BackColor;

            this.lbl_Time_Actual.Text = "Actual:";
            this.lbl_Time_Actual.BackColor = this.Parent.BackColor;

            this.lbl_Time_Stop.Text = "Stop:";
            this.lbl_Time_Stop.BackColor = this.Parent.BackColor;
        } 
               
    }
}
