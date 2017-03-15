using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Deployment.Application;
using System.IO;
using PowerOne.DataLogger;
using PowerOne;

namespace ORTBurnInSW
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }        

        GB_BurnInShelf Shelf01;
        GB_BurnInShelf Shelf02;
        GB_BurnInShelf Shelf03;
        GB_BurnInShelf Shelf04;
        GB_BurnInShelf Shelf05;
        GB_BurnInShelf Shelf06;

        public DataLog myDataloger;

        public GPIB myGPIB;

        private void CheckForUpdateAndInstallIt()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    try
                    {
                        ad.Update();
                        //MessageBox.Show("The application has been upgraded, and will now restart.");
                        Application.Restart();
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        MessageBox.Show("Nemoze sa nainstalovat najnovsia verzia programu. \n\nProsim zavolajte testovacieho inziniera.\n\n" + dde);
                        return;
                    }

                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.CheckForUpdateAndInstallIt();

            if (!File.Exists(@"C:/Users/Public/ORTBurnIn/StationConfig.xml"))
            {
                if (!Directory.Exists(@"C:/Users/Public/ORTBurnIn"))
                {
                    Directory.CreateDirectory(@"C:/Users/Public/ORTBurnIn");
                }
                File.Copy(String.Concat(Path.GetDirectoryName(Application.ExecutablePath), "\\StationConfig.xml"), @"C:/Users/Public/ORTBurnIn/StationConfig.xml");
            }

            this.myDataloger = new DataLog();
            String str_ComPort = ConfigFile.GetDatalogComPort();
            this.myDataloger.OpenConnection(str_ComPort, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
            //this.myDataloger.OpenConnection("COM4", 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);

            this.myGPIB = new GPIB();

            this.WindowState = FormWindowState.Maximized;
            Int16 ShelfIndent = 5;
            Size DefaultShelfSize = new Size(1345, 115);

            this.Shelf01 = new GB_BurnInShelf(new Point(12, 12), ref this.myDataloger, ref this.myGPIB);
            this.Shelf01.Text = "Shelf01";
            this.Shelf01.Size = DefaultShelfSize;
            this.Shelf01.n_ShelfNumber = 1;
            this.Shelf01.myDLCardDevice = this.myDataloger.card1;
            this.Shelf01.Parent = this;
            if (!ConfigFile.GetShelfEnableStatus(1)) this.Shelf01.Enabled = false;

            this.Shelf02 = new GB_BurnInShelf(new Point(12, ShelfIndent + this.Shelf01.Location.Y + this.Shelf01.Height), ref this.myDataloger, ref this.myGPIB);
            this.Shelf02.Text = "Shelf02";
            this.Shelf02.Size = DefaultShelfSize;
            this.Shelf02.n_ShelfNumber = 2;
            this.Shelf02.myDLCardDevice = this.myDataloger.card2;
            this.Shelf02.Parent = this;
            if (!ConfigFile.GetShelfEnableStatus(2)) this.Shelf02.Enabled = false;

            this.Shelf03 = new GB_BurnInShelf(new Point(12, ShelfIndent + this.Shelf02.Location.Y + this.Shelf02.Height), ref this.myDataloger, ref this.myGPIB);
            this.Shelf03.Text = "Shelf03";
            this.Shelf03.Size = DefaultShelfSize;
            this.Shelf03.n_ShelfNumber = 3;
            this.Shelf03.myDLCardDevice = this.myDataloger.card3;
            this.Shelf03.Parent = this;
            if (!ConfigFile.GetShelfEnableStatus(3)) this.Shelf03.Enabled = false;

            this.Shelf04 = new GB_BurnInShelf(new Point(12, ShelfIndent + this.Shelf03.Location.Y + this.Shelf03.Height), ref this.myDataloger, ref this.myGPIB);
            this.Shelf04.Text = "Shelf04";
            this.Shelf04.Size = DefaultShelfSize;
            this.Shelf04.n_ShelfNumber = 4;
            this.Shelf04.myDLCardDevice = this.myDataloger.card4;
            this.Shelf04.Parent = this;
            if (!ConfigFile.GetShelfEnableStatus(4)) this.Shelf04.Enabled = false;

            this.Shelf05 = new GB_BurnInShelf(new Point(12, ShelfIndent + this.Shelf04.Location.Y + this.Shelf04.Height), ref this.myDataloger, ref this.myGPIB);
            this.Shelf05.Text = "Shelf05";
            this.Shelf05.Size = DefaultShelfSize;
            this.Shelf05.n_ShelfNumber = 5;
            this.Shelf05.myDLCardDevice = this.myDataloger.card5;
            this.Shelf05.Parent = this;
            if (!ConfigFile.GetShelfEnableStatus(5)) this.Shelf05.Enabled = false;

            this.Shelf06 = new GB_BurnInShelf(new Point(12, ShelfIndent + this.Shelf05.Location.Y + this.Shelf05.Height), ref this.myDataloger, ref this.myGPIB);
            this.Shelf06.Text = "Shelf06";
            this.Shelf06.Size = DefaultShelfSize;
            this.Shelf06.n_ShelfNumber = 6;
            this.Shelf06.myDLCardDevice = this.myDataloger.card6;
            this.Shelf06.Parent = this;
            if (!ConfigFile.GetShelfEnableStatus(6)) this.Shelf06.Enabled = false;

            //Hiding application Bar
            //this.WindowState = FormWindowState.Normal;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.Bounds = Screen.PrimaryScreen.Bounds;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            try
            {
                this.myDataloger.CloseConnection();
                this.myGPIB.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(String.Concat("Error -100/n", ex.Message));
            }            
        }        
    }
}
