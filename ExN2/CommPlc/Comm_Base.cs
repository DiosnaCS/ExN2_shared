using ExN2.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;

namespace ExN2.CommPlc {

    // result of communication request
    public enum tCommResult {
        OK,         // received OK
        Timeout,    // no reply
        GenErr      // other error
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
        virtual public tCommResult comm_ReadDbVisu(out byte[] aReturnBuff) { aReturnBuff = null; return tCommResult.GenErr; }   // only placeholder

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

            target = new IPEndPoint(IPAddress.Parse("192.168.92.210"), 2000);

            return true;
        }

        //...............................................................................
        public void DoExit() {
            if (socket != null) {
                socket.Close();
                socket = null;
            }
        }

        void Log(int aLevel, string aMsg, bool bTimestamp) {
            Base.Log_TaskLevel(iTaskNo, aLevel, aMsg, bTimestamp);
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

        public ShowTaskInfo getTaskProgress() {
            ShowTaskInfo res = new ShowTaskInfo();

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
        override public tCommResult comm_ReadDbVisu(out byte[] aReturnBuff) {
            aReturnBuff = null;

            byte[] abRcvBuf = new byte[iMAX_PACKET_SIZE];
            try {
                byte[] snd = new byte[14] { 0x4e, 0x34, 01, 00, 0x54, 0x00, 03, 03, 00, 00, 01, 00, 00, 00 };
                lCnt_Send++;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);   // fixed value
                socket.SendTo(snd, target);
            }
            catch (Exception ex) {
                Log(1, " ... send error: " + ex.Message, false);
                CountErr();
                return tCommResult.GenErr;
            }

            int N = 0;
            tCommResult resultCode = tCommResult.GenErr;
            try {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
                N = socket.Receive(abRcvBuf);
            }
            catch (SocketException ex) {
                string S;
                if (ex.NativeErrorCode == 10060) {
                    S = " ... recieve timeout";
                    resultCode = tCommResult.Timeout;
                }
                else {
                    S = " ... recieve error: " + ex.Message;
                    resultCode = tCommResult.GenErr;
                }
                Log(1, S, false);
            }

            if (N <= 0) {         // no data recieved for any reason
                CountErr();
                return resultCode;
            }

            CountOK();

            // remove 10 bytes - N4T header
            byte[] abResBuf = new byte[iMAX_PACKET_SIZE];
            for (int i = 0; i < iMAX_PACKET_SIZE-10; i++)
                abResBuf[i] = abRcvBuf[i+10];
            aReturnBuff = abResBuf;
            return tCommResult.OK;
        }


    }
}


