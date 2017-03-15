using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; // Thread
using System.IO.Ports;  // SerialPort
using PowerOne.Support;


namespace PowerOne.DataLogger
{
    public enum CardType
    {
        DATALOGGER,
        SWITCH,
    };

    

    public class DataLogCardDevice
    {
        public byte cardAddress;
        public CardType type;
        public List<double> measValues = new List<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 });
        public int relayValues = 0;

        public DataLogCardDevice(byte cardAddress, CardType type)
        {
            this.cardAddress = cardAddress;
            this.type = type;
        }

        public double GetMeasuredValue(int channelIndex)    // Get Measured Value from specified channel index
        {
            return measValues[channelIndex - 1];
        }

        public bool GetRelayState(int channelIndex)         // Get Relay State from specified channel index
        {
            bool returnVal = false;

            switch (channelIndex)
            {
                case 1: if ((relayValues & 0x0001) == 0x0001) returnVal = true; else returnVal = false; break;
                case 2: if ((relayValues & 0x0002) == 0x0002) returnVal = true; else returnVal = false; break;
                case 3: if ((relayValues & 0x0004) == 0x0004) returnVal = true; else returnVal = false; break;
                case 4: if ((relayValues & 0x0008) == 0x0008) returnVal = true; else returnVal = false; break;
                case 5: if ((relayValues & 0x0010) == 0x0010) returnVal = true; else returnVal = false; break;
                case 6: if ((relayValues & 0x0020) == 0x0020) returnVal = true; else returnVal = false; break;
                case 7: if ((relayValues & 0x0040) == 0x0040) returnVal = true; else returnVal = false; break;
                case 8: if ((relayValues & 0x0080) == 0x0080) returnVal = true; else returnVal = false; break;
                case 9: if ((relayValues & 0x0100) == 0x0100) returnVal = true; else returnVal = false; break;
                case 10: if ((relayValues & 0x0200) == 0x0200) returnVal = true; else returnVal = false; break;
                case 11: if ((relayValues & 0x0400) == 0x0400) returnVal = true; else returnVal = false; break;
                case 12: if ((relayValues & 0x0800) == 0x0800) returnVal = true; else returnVal = false; break;
                case 13: if ((relayValues & 0x1000) == 0x1000) returnVal = true; else returnVal = false; break;
                case 14: if ((relayValues & 0x2000) == 0x2000) returnVal = true; else returnVal = false; break;
                case 15: if ((relayValues & 0x4000) == 0x4000) returnVal = true; else returnVal = false; break;
                case 16: if ((relayValues & 0x8000) == 0x8000) returnVal = true; else returnVal = false; break;
            }

            return returnVal;
        }
    }

    public enum DataLogRelayChannel
    {
        No01 = 1,
        No02 = 2,
        No03 = 4,
        No04 = 8,
        No05 = 16,
        No06 = 32,
        No07 = 64,
        No08 = 128,
        No09 = 256,
        No10 = 512,
        No11 = 1024,
        No12 = 2048,
        No13 = 4096,
        No14 = 8192,
        No15 = 16384,
        No16 = 32768,
    };

    public enum DataLogMeasChannel
    {
        No01 = 1,
        No02 = 2,
        No03 = 3,
        No04 = 4,
        No05 = 5,
        No06 = 6,
        No07 = 7,
        No08 = 8,
        No09 = 9,
        No10 = 10,
        No11 = 11,
        No12 = 12,
        No13 = 13,
        No14 = 14,
        No15 = 15,
        No16 = 16,
    };

    public class DataLog
    {
        public int Counter = 0;

        //-------------------------------------------------------------------------------------------------------------------------
        public SerialPort serialPort = new SerialPort();

        private DataLogCardDevice card; // necessary for RS485 response
        public DataLogCardDevice card1 = new DataLogCardDevice(1, CardType.DATALOGGER);
        public DataLogCardDevice card2 = new DataLogCardDevice(2, CardType.DATALOGGER);
        public DataLogCardDevice card3 = new DataLogCardDevice(3, CardType.DATALOGGER);
        public DataLogCardDevice card4 = new DataLogCardDevice(4, CardType.DATALOGGER);
        public DataLogCardDevice card5 = new DataLogCardDevice(5, CardType.DATALOGGER);
        public DataLogCardDevice card6 = new DataLogCardDevice(6, CardType.DATALOGGER);

        private byte x, checkSum;
        private byte[] sendData = new byte[50];
        private byte[] recvData = new byte[50];

        public bool IsReady = true;

        public List<ErrorQueue> errorQueue = new List<ErrorQueue>();
        //-------------------------------------------------------------------------------------------------------------------------
        private void SetError(string errorMessage, bool errorCritical)
        {
            ErrorQueue eq = new ErrorQueue();

            eq.Message = DateTime.Now.ToString() + ": " + errorMessage + "\n";
            eq.Critical = errorCritical;

            errorQueue.Add(eq);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public int OpenConnection(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits)    // Open RS485 Port
        {
            try
            {
                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = dataBits;
                serialPort.Parity = parity;
                serialPort.StopBits = stopBits;

                serialPort.WriteTimeout = 20;
                serialPort.ReadTimeout = 20;

                //serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

                serialPort.Open();
            }
            catch (Exception e)
            {
                SetError(e.Message, true);
                return -100;
            }
            this.IsReady = true;
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public int CloseConnection()    // Close RS485 port
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    //serialPort.DataReceived -= serialPort_DataReceived;
                    serialPort.DiscardInBuffer();
                    serialPort.Close();
                }
            }
            catch (Exception e)
            {
                SetError(e.Message, false);
                return -100;
            }
            this.IsReady = false;
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public int SetRelayChannels(DataLogCardDevice card, DataLogRelayChannel relayNo, byte relayState) // 
        {
            int counter = 0;
            while (!this.IsReady)
            {
                if (counter > 50)
                {
                    //System.Windows.Forms.MessageBox.Show("timeout SetRelayChannel");
                    return -1000;
                }
                Thread.Sleep(100);
                counter++;
            }
            this.IsReady = false;
            checkSum = 0;
            for (int i = 0; i < 50; i++) { sendData[i] = 0; recvData[i] = 0; }

            this.card = card;
            Thread.Sleep(250);
            if (relayState == 0) { card.relayValues &= ~(int)relayNo; } else { card.relayValues |= (int)relayNo; }

            switch (card.type)
            {
                case CardType.DATALOGGER:

                    sendData[0] = 255;          // Start byte
                    sendData[1] = 10;           // Message lenght
                    sendData[2] = card.cardAddress;  // Receiver address
                    sendData[3] = 0;            // Trnasmitter address
                    sendData[4] = 1;            // Token (always 1)

                    if (relayState == 1)
                    {
                        sendData[5] = 150;          // Function code
                        sendData[6] = 150;          // Function code
                    }
                    else
                    {
                        sendData[5] = 151;          // Function code
                        sendData[6] = 151;          // Function code
                    }

                    sendData[7] = (byte)relayNo;// card.relayValues;// Relay data
                    sendData[8] = (byte)relayNo;//card.relayValues;// Relay data

                    sendData[9] = 0;            // Checksum

                    for (int i = 0; i < sendData[1] - 1; i++)
                    {
                        sendData[9] += sendData[i];
                    }
                    break;

                case CardType.SWITCH:

                    sendData[0] = 255;          // Start byte
                    sendData[1] = 12;           // Message lenght
                    sendData[2] = card.cardAddress;  // Receiver address
                    sendData[3] = 0;            // Trnasmitter address
                    sendData[4] = 1;            // Token (always 1)
                    sendData[5] = 155;          // Function code
                    sendData[6] = 155;          // Function code
                    sendData[7] = (byte)(card.relayValues % 256);   // Relay data
                    sendData[8] = (byte)(card.relayValues % 256);   // Relay data
                    sendData[9] = (byte)(card.relayValues / 256);   // Relay data
                    sendData[10] = (byte)(card.relayValues / 256);   // Relay data


                    sendData[11] = 0;            // Checksum

                    for (int i = 0; i < sendData[1] - 1; i++)
                    {
                        sendData[11] += sendData[i];
                    }

                    break;
            }

            // Write RS485 data...
            try
            {
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();
                serialPort.Write(sendData, 0, sendData[1]);
                this.IsReady = true;
            }
            catch (Exception exc)
            {
                SetError(exc.Message, false);
                this.IsReady = true;
                return -100;
            }

            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------
        public int MeasureChannels(DataLogCardDevice card, DataLogMeasChannel firstChannel, DataLogMeasChannel lastChannel, ref Array MeasValues)
        {
            int counter = 0;
            while (!this.IsReady)
            {
                if (counter > 50)
                {
                    //System.Windows.Forms.MessageBox.Show("timeout MeasureChannels");
                    return -1000;
                }
                Thread.Sleep(100);
                counter++;
            }
            this.IsReady = false;
            checkSum = 0;
            for (int i = 0; i < 50; i++) { sendData[i] = 0; recvData[i] = 0; }

            this.card = card;

            sendData[0] = 255;          // Start byte
            sendData[1] = 9;            // Message lenght
            sendData[2] = card.cardAddress;  // Receiver address
            sendData[3] = 0;            // Trnasmitter address
            sendData[4] = 1;            // Token (always 1)
            sendData[5] = 110;          // Function code

            sendData[6] = (byte)firstChannel;   // First channel
            sendData[7] = (byte)(firstChannel+9);    // Last channel

            sendData[8] = 0;            // Checksum

            for (int i = 0; i < sendData[1] - 1; i++)
            {
                sendData[8] += sendData[i];
            }

            // Write RS485 data...
            try
            {
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();
                serialPort.Write(sendData, 0, sendData[1]);
            }
            catch (Exception exc)
            {
                SetError(exc.Message, false);
                return -100;
            }
            Thread.Sleep(1000);
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Read(recvData, 0, 50);
                    serialPort.DiscardInBuffer();
                }
            }
            catch (Exception exc)
            {
                SetError(exc.Message, false);
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();
            }

            x = (byte)(10 + 2 * (sendData[7] - sendData[6] + 1));

            if (recvData[0] != 255) // Start byte
            {
                SetError("Function MeasureChannels. Invalid Start byte received. (" + recvData[0].ToString() + ")", false);                
            }
            if ((recvData[1] != x) || (x > 42))   // Message length
            {
                SetError("Function MeasureChannels. Invalid Message length received. (" + recvData[1].ToString() + ")", false);                
            }
            if (recvData[4] != 0)   // Status
            {
                SetError("Function MeasureChannels. Invalid Status byte received. (" + recvData[4].ToString() + ")", false);                
            }
            if (recvData[7] != sendData[6]) // First channel
            {
                SetError("Function MeasureChannels. Invalid First channel received. (" + recvData[7].ToString() + ")", false);                
            }
            if (recvData[8] != sendData[7]) // Last channel
            {
                SetError("Function MeasureChannels. Invalid Last channel received. (" + recvData[8].ToString() + ")", false);                
            }
            if (recvData[1] > 0)
            {
                for (int i = 0; i < recvData[1] - 1; i++)
                {
                    checkSum += recvData[i];
                }
            }
            if (recvData[x - 1] != checkSum)  // Checksum
            {
                SetError("Function MeasureChannels. Invalid Checksum received. (" + recvData[x - 1].ToString() + ")", false);
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();                
            }
            try
            {
                for (int i = 0; i < (recvData[8] - recvData[7] + 1); i++)
                {
                    card.measValues[recvData[7] - 1 + i] = (double)((short)(recvData[9 + (i * 2)] + recvData[10 + (i * 2)] * 256)) / 100.0;
                }
            }
            catch (Exception ex)
            {
                SetError("Function MeasureChannels. " + ex.Message, false);
            }

            this.IsReady = true;
            MeasValues = card.measValues.ToArray();
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        /*
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            Thread.Sleep(1300);

            try
            {
                if (sp.IsOpen)
                {
                    sp.Read(recvData, 0, 50);
                    sp.DiscardInBuffer();
                }
            }
            catch (Exception exc)
            {
                SetError(exc.Message, false);
                if (sp.IsOpen) sp.DiscardInBuffer();
            }

            switch (recvData[6])
            {
                case 150:   // SetRelayChannels Response - RBDatalogger
                    if (recvData[0] != 255) // Start byte
                    {
                        SetError("Function SetRelayChannels. Invalid Start Byte received. (" + recvData[0].ToString() + ")", false);
                    }
                    if (recvData[1] != 9) // Message length
                    {
                        SetError("Function SetRelayChannels. Invalid Message Length received. (" + recvData[1].ToString() + ")", false);
                    }
                    if (recvData[4] != 0) // Status
                    {
                        SetError("Function SetRelayChannels. Invalid Status Byte received. (" + recvData[4].ToString() + ")", false);
                    }
                    if (recvData[7] != (byte)sendData[7])   // Relay data
                    {
                        SetError("Function SetRelayChannels. Invalid Relay Data received. (" + recvData[7].ToString() + ")", false);
                    }
                    if (recvData[1] > 0)
                    {
                        for (int i = 0; i < recvData[1] - 1; i++)
                        {
                            checkSum += recvData[i];
                        }
                    }
                    if (recvData[8] != checkSum)    // Checksum
                    {
                        SetError("Function SetRelayChannels. Invalid Checksum received. (" + recvData[8].ToString() + ")", false);
                    }

                    break;

                case 155:   // SetRelayChannels Response - SWDatalogger
                    if (recvData[0] != 255) // Start byte
                    {
                        SetError("Function SetRelayChannels. Invalid Start Byte received. (" + recvData[0].ToString() + ")", false);
                    }
                    if (recvData[1] != 10) // Message length
                    {
                        SetError("Function SetRelayChannels. Invalid Message Length received. (" + recvData[1].ToString() + ")", false);
                    }
                    if (recvData[4] != 0) // Status
                    {
                        SetError("Function SetRelayChannels. Invalid Status Byte received. (" + recvData[4].ToString() + ")", false);
                    }
                    if (recvData[7] != (byte)sendData[7])   // Relay data LSB
                    {
                        SetError("Function SetRelayChannels. Invalid Relay Data LSB received. (" + recvData[7].ToString() + ")", false);
                    }
                    if (recvData[8] != (byte)sendData[9])   // Relay data MSB
                    {
                        SetError("Function SetRelayChannels. Invalid Relay Data MSB received. (" + recvData[8].ToString() + ")", false);
                    }
                    if (recvData[1] > 0)
                    {
                        for (int i = 0; i < recvData[1] - 1; i++)
                        {
                            checkSum += recvData[i];
                        }
                    }
                    if (recvData[9] != checkSum)    // Checksum
                    {
                        SetError("Function SetRelayChannels. Invalid Checksum received. (" + recvData[9].ToString() + ")", false);
                    }

                    break;

                case 151:   // ResetRelayChannels Response - RBDatalogger
                    if (recvData[0] != 255) // Start byte
                    {
                        SetError("Function ResetRelayChannels. Invalid Start Byte received. (" + recvData[0].ToString() + ")", false);
                    }
                    if (recvData[1] != 9) // Message length
                    {
                        SetError("Function ResetRelayChannels. Invalid Message Length received. (" + recvData[1].ToString() + ")", false);
                    }
                    if (recvData[4] != 0) // Status
                    {
                        SetError("Function ResetRelayChannels. Invalid Status Byte received. (" + recvData[4].ToString() + ")", false);
                    }
                    if (recvData[7] != (byte)sendData[7])   // Relay data
                    {
                        SetError("Function ResetRelayChannels. Invalid Relay Data received. (" + recvData[7].ToString() + ")", false);
                    }
                    if (recvData[1] > 0)
                    {
                        for (int i = 0; i < recvData[1] - 1; i++)
                        {
                            checkSum += recvData[i];
                        }
                    }
                    if (recvData[8] != checkSum)    // Checksum
                    {
                        SetError("Function ResetRelayChannels. Invalid Checksum received. (" + recvData[8].ToString() + ")", false);
                    }

                    break;

                case 156:   // ReResetRelayChannels Response - SWDatalogger
                    if (recvData[0] != 255) // Start byte
                    {
                        SetError("Function ResetRelayChannels. Invalid Start Byte received. (" + recvData[0].ToString() + ")", false);
                    }
                    if (recvData[1] != 10) // Message length
                    {
                        SetError("Function ResetRelayChannels. Invalid Message Length received. (" + recvData[1].ToString() + ")", false);
                    }
                    if (recvData[4] != 0) // Status
                    {
                        SetError("Function ResetRelayChannels. Invalid Status Byte received. (" + recvData[4].ToString() + ")", false);
                    }
                    if (recvData[7] != (byte)sendData[7])   // Relay data LSB
                    {
                        SetError("Function ResetRelayChannels. Invalid Relay Data LSB received. (" + recvData[7].ToString() + ")", false);
                    }
                    if (recvData[8] != (byte)sendData[9])   // Relay data MSB
                    {
                        SetError("Function ResetRelayChannels. Invalid Relay Data MSB received. (" + recvData[8].ToString() + ")", false);
                    }
                    if (recvData[1] > 0)
                    {
                        for (int i = 0; i < recvData[1] - 1; i++)
                        {
                            checkSum += recvData[i];
                        }
                    }
                    if (recvData[9] != checkSum)    // Checksum
                    {
                        SetError("Function ResetRelayChannels. Invalid Checksum received. (" + recvData[9].ToString() + ")", false);
                    }

                    break;

                case 110:   // MeasureChannels Response - RBDatalogger + SWDatalogger
                    x = (byte)(10 + 2 * (sendData[7] - sendData[6] + 1));

                    if (recvData[0] != 255) // Start byte
                    {
                        SetError("Function MeasureChannels. Invalid Start byte received. (" + recvData[0].ToString() + ")", false);
                        break;
                    }
                    if ((recvData[1] != x) || (x > 42))   // Message length
                    {
                        SetError("Function MeasureChannels. Invalid Message length received. (" + recvData[1].ToString() + ")", false);
                        break;
                    }
                    if (recvData[4] != 0)   // Status
                    {
                        SetError("Function MeasureChannels. Invalid Status byte received. (" + recvData[4].ToString() + ")", false);
                        break;
                    }
                    if (recvData[7] != sendData[6]) // First channel
                    {
                        SetError("Function MeasureChannels. Invalid First channel received. (" + recvData[7].ToString() + ")", false);
                        break;
                    }
                    if (recvData[8] != sendData[7]) // Last channel
                    {
                        SetError("Function MeasureChannels. Invalid Last channel received. (" + recvData[8].ToString() + ")", false);
                        break;
                    }
                    if (recvData[1] > 0)
                    {
                        for (int i = 0; i < recvData[1] - 1; i++)
                        {
                            checkSum += recvData[i];
                        }
                    }
                    if (recvData[x - 1] != checkSum)  // Checksum
                    {
                        SetError("Function MeasureChannels. Invalid Checksum received. (" + recvData[x - 1].ToString() + ")", false);
                        if (sp.IsOpen) sp.DiscardInBuffer();
                        break;
                    }
                    try
                    {
                        for (int i = 0; i < (recvData[8] - recvData[7] + 1); i++)
                        {
                            card.measValues[recvData[7] - 1 + i] = (double)((short)(recvData[9 + (i * 2)] + recvData[10 + (i * 2)] * 256)) / 100.0;
                        }
                    }
                    catch (Exception ex)
                    {
                        SetError("Function MeasureChannels. " + ex.Message, false);
                    }

                    break;

                default:
                    SetError("Incorrect Function Code received. (" + recvData[6].ToString() + ")", false);
                    if (sp.IsOpen) sp.DiscardInBuffer();
                    break;
            }
        }
        */

        public int GetRelayChannelsState(DataLogCardDevice card, ref int RelayChannelsState)
        {
            int counter = 0;
            while (!this.IsReady)
            {
                if (counter > 50)
                {
                    //System.Windows.Forms.MessageBox.Show("timeout SetRelayChannel");
                    return -1000;
                }
                Thread.Sleep(100);
                counter++;
            }
            this.IsReady = false;

            for (int i = 0; i < 50; i++) { sendData[i] = 0; recvData[i] = 0; }
            Thread.Sleep(500);
            sendData[0] = 255;          // Start byte
            sendData[1] = 7;           // Message lenght
            sendData[2] = card.cardAddress;  // Receiver address
            sendData[3] = 0;            // Trnasmitter address
            sendData[4] = 1;            // Token (always 1)

            
            sendData[5] = 130;          // Function code
            

            for (int i = 0; i < sendData[1] - 1; i++)
            {
                sendData[6] += sendData[i];
            }

            try
            {
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();
                serialPort.Write(sendData, 0, sendData[1]);
            }
            catch (Exception exc)
            {
                SetError(exc.Message, false);
                return -100;
            }

            Thread.Sleep(1000);

            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Read(recvData, 0, 50);
                    serialPort.DiscardInBuffer();
                }
            }
            catch (Exception exc)
            {
                SetError(exc.Message, false);
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();
            }

            RelayChannelsState = recvData[11];

            this.IsReady = true;
            return 0;
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public int RS485_F170_UnassignHcRelays(int ComPort, byte ReceiverAddr, byte TransmitterAddr)
        {
            //int State = 0;
            byte Function = 171;		    //!!!!
            byte FrameStatus = 1;		// Function confirms the correct execution		
            byte DataBlockLenght = 2;

            byte[] Data = new byte[255];
            byte[] ReceivedData = new byte[255];
            int ret_code;
            int TAddr = 0;
            int RAddr = 0;
            int Status = 0;
            int Fct = 0;
            int DBlockLenght = 0;
            int[] DataRx = new int[255];

            // Prepare Data block
            Data[0] = Function;   			// reserved Byte
            Data[1] = 1; 				    // reserved Byte
            Data[2] = 0;				    // reserved Byte
            Data[3] = 0;				    // reserved Byte
            Data[4] = 0;			    	// reserved Byte
            Data[5] = 0;					// reserved Byte 
            Data[6] = 0;					// reserved Byte 
            Data[7] = 0;					// reserved Byte 

            Thread.Sleep(100);

            ret_code = RS485SendFrame(ComPort, TransmitterAddr, ReceiverAddr, FrameStatus, Function, DataBlockLenght, Data);

            Thread.Sleep(200);

            ret_code = RS485GetFrame(ComPort, ref TAddr, ref RAddr, ref Status, ref Fct, ref DBlockLenght, ref DataRx);

            return (ret_code);
        }
        //--------------------------------------------------------------------------//
        public int RS485OpenComPort(int ComP)
        {
            if (!serialPort.IsOpen)
                serialPort.Open();
            serialPort.ReadTimeout = 500;

            return (1);
        }
        //--------------------------------------------------------------------------//
        public int RS485SendFrame(int ComPort, byte TransmitterAddr, byte ReceiverAddr, byte FrameStatus, byte Function, byte DataBlockLenght, byte[] Data)
        {
            byte StartByte = 255;
            byte[] Frame = new byte[255];
            byte CheckSum = 0;
            int i = 0;
            int NumberOfBytes;
            int Sum = 0;

            NumberOfBytes = (int)(DataBlockLenght) + 7;

            // Build Frame 
            Frame[0] = (byte)StartByte;
            Frame[1] = (byte)NumberOfBytes;
            Frame[2] = (byte)ReceiverAddr;
            Frame[3] = (byte)TransmitterAddr;
            Frame[4] = (byte)1; 					// Frame Control always 1
            Frame[5] = (byte)Function;


            // Add Data Block if available
            if (DataBlockLenght != 0)
            {
                for (i = 0; i < DataBlockLenght; i++)
                {
                    Frame[6 + i] = Data[i];
                }
            }

            // Calculate Checksum

            for (i = 0; i < NumberOfBytes - 1; i++)
            {
                Sum += Frame[i];
                CheckSum += Frame[i]; // For getting the LSB just let the variable overflow
            }

            // and add it as last Byte

            Frame[NumberOfBytes - 1] = CheckSum;

            serialPort.Write(Frame, 0, NumberOfBytes);

            serialPort.ReadTimeout = 500;

            Thread.Sleep(100);

            return (1);
        }
        //--------------------------------------------------------------------------//

        public int RS485GetFrame(int ComPort, ref int TransmitterAddr, ref int ReceiverAddr, ref int Status, ref int Function, ref int DataBlockLenght, ref int[] DataRx)
        {
            int StartByte = 0;
            int NumberOfBytes = 0;
            int FrameControl = 0;
            int CCheckSum = 0;      // The calculated Checksum
            int RCheckSum = 0;      // The read checksum
            int Value;              // for debugging only
            int i = 0;

            //Assign values before function starts
            TransmitterAddr = 0;
            ReceiverAddr = 0;
            Status = 0;
            Function = 0;
            DataBlockLenght = 0;

            //Start byte
            try
            {
                StartByte = serialPort.ReadByte();				        // Start byte
                //MessageBox.Show(StartByte.ToString(), "Start byte");
            }
            catch (TimeoutException)
            {
                return -1;
            }

            if (StartByte != 255) return (-100);
            CCheckSum = StartByte;


            //Number of bytes
            try
            {
                NumberOfBytes = serialPort.ReadByte();				    // No of bytes
                //MessageBox.Show(NumberOfBytes.ToString(), "Number of  bytes");
            }
            catch (TimeoutException)
            {
                return -2;
            }
            if (NumberOfBytes > 50) return (-101);
            CCheckSum += NumberOfBytes;

            //Receiver address
            try
            {
                ReceiverAddr = serialPort.ReadByte();				    // Receiver address
            }
            catch (TimeoutException)
            {
                return -3;
            }
            if (ReceiverAddr != 0) return (-102);
            CCheckSum += ReceiverAddr;

            //Transmitter address
            try
            {
                TransmitterAddr = serialPort.ReadByte();				// Transmitter address
            }
            catch (TimeoutException)
            {
                return -4;
            }
            if (TransmitterAddr == 0) return (-103);
            CCheckSum += TransmitterAddr;

            //Status
            try
            {
                Status = serialPort.ReadByte();				            // Status
            }
            catch (TimeoutException)
            {
                return -5;
            }
            if (Status != 0) return (-104);
            CCheckSum += Status;

            //Frame Control
            try
            {
                FrameControl = serialPort.ReadByte();				        // Status
            }
            catch (TimeoutException)
            {
                return -6;
            }
            if (FrameControl != 1) return (-105);
            CCheckSum += FrameControl;

            //Function
            try
            {
                Function = serialPort.ReadByte();				        // Function
                //MessageBox.Show(Function.ToString(), "Function");
            }
            catch (TimeoutException)
            {
                return -7;
            }
            CCheckSum += Function;

            // read data if  available
            if (NumberOfBytes > 8)
            {
                DataBlockLenght = NumberOfBytes - 8;

                for (i = 0; i < DataBlockLenght; i++)
                {
                    try
                    {
                        Value = serialPort.ReadByte();				// Value
                        //MessageBox.Show(Value.ToString(), "Data");
                    }
                    catch (TimeoutException)
                    {
                        return -8;
                    }

                    CCheckSum += Value;
                    DataRx[i] = Value;
                }
            }
            else
            {
                DataBlockLenght = 0;
            }

            CCheckSum = CCheckSum % 256;

            // read checksum 
            try
            {
                RCheckSum = serialPort.ReadByte();				// Function
            }
            catch (TimeoutException)
            {
                return -8;
            }
            if (CCheckSum != RCheckSum) return (-106);

            return (0);   //was   return 1
        }
    }
}
