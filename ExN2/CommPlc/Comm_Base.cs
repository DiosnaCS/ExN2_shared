using ExN2.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;

namespace ExN2.CommPlc {

    public class CommProps {
        // UDP specification
        public string sPLC_IPaddr;
        public int iPLC_Port;
        public string sLocal_IPaddr;
        public int iLocal_Port;
        public int iTimeoutMs;

        // SQL specification
        public string sSQL_ConnectString;
        public string sSQL_Database;
        public string sSQL_TablePrefix;
        public string sSQL_UserId;
        public string sSQL_Password;
    }

    //-------------------------------------------------------------------
    // Comm_Base = base class for communication with PLC, must be overrided
    //-------------------------------------------------------------------
    public class Comm_Base {
        protected int iTaskNo;

        // error counting
        public long lCnt_Send;
        public long lCnt_Err;
        public long lCnt_OK;
        public int iErrBalance;

        // abstract methods
        virtual public bool DoInit() { return false; }
        virtual public byte[] comm_ReadDbVisu() { return null; }

        //...............................................................................
        public Comm_Base(int aTaskNo) {
            iTaskNo = aTaskNo;
            lCnt_Send = lCnt_OK = lCnt_Err = 0;
            iErrBalance = 0;
        }
    }


    //-------------------------------------------------------------------
    // Comm_N4T = base for reading data via N4T Diosna proprietary protocol
    //-------------------------------------------------------------------
    public class Comm_N4T : Comm_Base {
        Socket socket;
        IPEndPoint target;
        const int iMAX_PACKET_SIZE = 1800;


        //...............................................................................
        public Comm_N4T(int aTaskNo)
                        : base(aTaskNo) 
        {
        }

        //...............................................................................
        override public bool DoInit() {

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Bind vaze lokalni stranu, musel by byt svazan pokazde s jinym portem, takze to necham automaticky
            //socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"),

            target = new IPEndPoint(IPAddress.Parse("192.168.2.111"), 2000);

            return true;
        }

        //...............................................................................
        public void DoExit() {
            if (socket != null) {
                socket.Close();
                socket = null;
            }
        }

        void Log(string aMsg, bool bTimestamp) {
            Base.Log_Task(iTaskNo, aMsg, bTimestamp);
        }

        void CountOK() {
            lCnt_OK++;
            if (iErrBalance < 3)
                iErrBalance++;
        }

        void CountErr() {
            lCnt_Err++;
            if (iErrBalance > -3)
                iErrBalance--;
        }

        public TaskShowInfo getTaskProgress() {
            TaskShowInfo res = new TaskShowInfo();

            // generate info text
            res.sText = "ok:" + lCnt_OK + ", err:" + lCnt_Err;

            // generate info color
            if (iErrBalance >= 3)
                res.brush = Brushes.LightGreen;
            else if (iErrBalance <= -3)
                res.brush = Brushes.Red;
            else
                res.brush = Brushes.Yellow;
            return res;
        }

        //...............................................................................
        /// <summary> Read the data from remote site, Returns NULL in case of error </summary>
        override public byte[] comm_ReadDbVisu() {
            byte[] abRcvBuf = new byte[iMAX_PACKET_SIZE];

            try {
                byte[] snd = new byte[14] { 0x4e, 0x34, 01, 00, 0x54, 0x00, 03, 03, 00, 00, 01, 00, 00, 00 };
                lCnt_Send++;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);   // fixed value
                socket.SendTo(snd, target);
            }
            catch (Exception ex) {
                Log(" ... send error: " + ex.Message, false);
                CountErr();
                return null;
            }

            int N = 0;
            try {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
                N = socket.Receive(abRcvBuf);
            }
            catch (SocketException ex) {
                string S;
                if (ex.NativeErrorCode == 10060)
                    S = " ... recieve timeout";
                else 
                    S = " ... recieve error: " + ex.Message;
                CountErr();
                Log(S, false);
            }

            if (N <= 0) {         // no data recieved
                CountErr();
                return null;
            }

            CountOK();

            // remove 10 bytes - N4T header
            byte[] abResBuf = new byte[iMAX_PACKET_SIZE];
            for (int i = 0; i < iMAX_PACKET_SIZE-10; i++)
                abResBuf[i] = abRcvBuf[i+10];
            return abResBuf;
        }


    }
}


