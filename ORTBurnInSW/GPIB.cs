using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NationalInstruments.NI4882;

namespace ORTBurnInSW
{
    public class GPIB
    {
        private Device dev_Load1;
        private Device dev_Load2;
        private Device dev_Load3;
        private Device dev_Load4;
        private Device dev_Load5;
        private Device dev_Load6;

        private Boolean b_working;

        public GPIB()
        {
            this.b_working = true;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (ConfigFile.GetShelfEnableStatus(1))
                {
                    this.dev_Load1 = new Device(0, 1);
                    this.dev_Load1.Write("*RST; VOLT 60/n");
                }
                if (ConfigFile.GetShelfEnableStatus(2))
                {
                    this.dev_Load2 = new Device(0, 2);
                    this.dev_Load2.Write("*RST; VOLT 60/n");
                }
                if (ConfigFile.GetShelfEnableStatus(3))
                {
                    this.dev_Load3 = new Device(0, 3);
                    this.dev_Load3.Write("*RST; VOLT 60/n");
                }
                if (ConfigFile.GetShelfEnableStatus(4))
                {
                    this.dev_Load4 = new Device(0, 4);
                    this.dev_Load4.Write("*RST; VOLT 60/n");
                }
                if (ConfigFile.GetShelfEnableStatus(5))
                {
                    this.dev_Load5 = new Device(0, 5);
                    this.dev_Load5.Write("*RST; VOLT 60/n");
                }
                if (ConfigFile.GetShelfEnableStatus(6))
                {
                    this.dev_Load6 = new Device(0, 6);
                    this.dev_Load6.Write("*RST; VOLT 60/n");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                b_working = false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                b_working = false;
            }
        }        

        public int SetLoad(int ShelfNr, int ValueInMiliAmps)
        {
            int counter = 0;
            while (b_working)
            {
                Thread.Sleep(10);
                if (counter > 100)
                {
                    //MessageBox.Show("SetLoad Timeout");
                }
                counter++;
            }
            Device actLoad = this.dev_Load1; ;
            switch (ShelfNr)
            {
                case 1:
                    actLoad = this.dev_Load1;
                    break;
                case 2:
                    actLoad = this.dev_Load2;
                    break;
                case 3:
                    actLoad = this.dev_Load3;
                    break;
                case 4:
                    actLoad = this.dev_Load4;
                    break;
                case 5:
                    actLoad = this.dev_Load5;
                    break;
                case 6:
                    actLoad = this.dev_Load6;
                    break;
                default:
                    break;
            }

            try
            {
                actLoad.Write(String.Concat("CURR ", this.CurrentValue(ValueInMiliAmps), "/n"));

                if (ValueInMiliAmps == 0) actLoad.Write("*RST; VOLT 60/n");                                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                this.b_working = false;
            }
            return 0;
        }

        public int Close()
        {
            try
            {
                if (ConfigFile.GetShelfEnableStatus(1)) this.dev_Load1.Dispose();
                if (ConfigFile.GetShelfEnableStatus(2)) this.dev_Load2.Dispose();
                if (ConfigFile.GetShelfEnableStatus(3)) this.dev_Load3.Dispose();
                if (ConfigFile.GetShelfEnableStatus(4)) this.dev_Load4.Dispose();
                if (ConfigFile.GetShelfEnableStatus(5)) this.dev_Load5.Dispose();
                if (ConfigFile.GetShelfEnableStatus(6)) this.dev_Load6.Dispose();
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return 0;
        }

        private String CurrentValue(Int32 miliAmps)
        {
            String buffer = miliAmps.ToString().Trim();
            switch (buffer.Length)
            {
                case 1:
                case 2:
                case 3:
                    while (buffer.Length != 5)
                    {
                        if (buffer.Length == 3)
                        {
                            buffer = String.Concat(".", buffer);
                        }
                        else
                        {
                            buffer = String.Concat("0", buffer);
                        }
                    }
                    break;
                default:
                    buffer = buffer.Insert(buffer.Length - 3, ".");                    
                    break;
            }            
            while (buffer.Substring(buffer.Length - 1) == "0")
                buffer = buffer.Substring(0, buffer.Length - 1);
            if (buffer.Substring(buffer.Length - 1) == ".")
                buffer = buffer.Substring(0, buffer.Length - 1);
            return buffer;
        }
    }
}
