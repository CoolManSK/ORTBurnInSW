using System;
using System.Windows.Forms;
using System.Drawing;
using SigmaSureManualReportGenerator;

namespace ORTBurnInSW
{
    class SerialNumberInsertForm : Form
    {
        public String Message;
        public String Caption;
        public String Answer;
        public String ProductID;
        public bool CancelFlag;
        private Label QuestionLabel;
        private TextBox AnswerTextBox;
        private Button btn_OK;
        private Button btn_Cancel;

        public SerialNumberInsertForm(String Message, String Caption, String ProductID)
        {
            this.Message = Message;
            this.Caption = Caption;
            this.ProductID = ProductID;
            this.CancelFlag = false;
            this.Size = new Size(300, 130);
            this.Text = this.Caption;
            this.WindowState = FormWindowState.Normal;
            this.ShowIcon = false;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterParent;

            this.QuestionLabel = new Label();
            this.QuestionLabel.Text = this.Message;
            this.QuestionLabel.AutoSize = true;
            this.QuestionLabel.Location = new Point(10, 10);
            this.QuestionLabel.Parent = this;

            this.AnswerTextBox = new TextBox();
            this.AnswerTextBox.Location = new Point(10, 35);
            this.AnswerTextBox.Size = new Size(this.Width - 40, 21);
            this.AnswerTextBox.KeyPress += new KeyPressEventHandler(AnswerTextBox_OnKeyPress);
            this.AnswerTextBox.Parent = this;

            this.btn_OK = new Button();
            this.btn_OK.Text = "OK";
            this.btn_OK.Size = new Size(60, 20);
            this.btn_OK.Location = new Point(10, 60);
            this.btn_OK.Click += new EventHandler(btn_OK_OnClick);
            this.btn_OK.Parent = this;

            this.btn_Cancel = new Button();
            this.btn_Cancel.Text = "CANCEL";
            this.btn_Cancel.Size = new Size(60, 20);
            this.btn_Cancel.Location = new Point(this.Width - 30 - this.btn_Cancel.Width, 60);
            this.btn_Cancel.Click += new EventHandler(btn_CANCEL_OnClick);
            this.btn_Cancel.Parent = this;
        }

        private bool BarcodeCheckForSerialNumber(String BarcodeString)
        {
            String SNfromBC = ProductBarcode.GetSerialNumberFromBarcode(this.ProductID, BarcodeString);
            if (SNfromBC.Length != 13) return false;
            foreach (char actChar in SNfromBC)
            {
                if (!Char.IsNumber(actChar)) return false;
            }
            this.Answer = SNfromBC;
            return true;               
        }

        private void AnswerTextBox_OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.btn_OK_OnClick(sender, new EventArgs());
            }
        }

        private void btn_OK_OnClick(object sender, EventArgs e)
        {
            if (this.BarcodeCheckForSerialNumber(this.AnswerTextBox.Text.Trim()))
            {
                this.Close();
            }
            else
            {
                this.AnswerTextBox.SelectAll();
            }
        }

        private void btn_CANCEL_OnClick(object sender, EventArgs e)
        {
            this.Answer = "";
            this.CancelFlag = true;
            this.Close();
        }
    }
}
