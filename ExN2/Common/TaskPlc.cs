using System;
using System.Windows;
using System.Threading;
using ExN2.Datablock;
using ExN2.CommPlc;
using System.Diagnostics;
using System.Windows.Media;
using ExN2.Loader;
using System.Collections.Generic;

namespace ExN2.Common {

    // --- PlcRequester: ---
    // Via one N4T channel handles the following operations:
    //  - periodically read the dbVisu datablock:
    //      - marked items are stored to SQL database, with different time periods (norm/slow/xslow/user-defined)
    //      - all items are available as "data snap" for drawing the animated screens, notifications, etc..
    //  - load "events" from PLC buffer and store them to SQL
    //  - adjust the PLC real time clock
    //  - FUTURE: periodically? read the alarm status and generate alarm archive SQL table
    //
    // Scheduler:
    //   Because the N4T channel is "single operation" oriented, we can sent only one N4T command at a time.
    // For this reason the all operations above must be implemented in one thread and sent one-by-one with some scheduler.

    public class TaskPlc : TaskBase {
        DbVisu dataBlock = null;
        Comm_N4T comPlc = null;
        EventLoader loader = null;
        TaskComProps comProps = null;

        public bool bEndReq = false;
        public Thread thread = null;
        Object Lock1 = new Object();

            
        // scheduler
        int iSCH_ReadSnapPeriod = 5;
        bool bSleeping = false;


        public TaskPlc(int aTaskNo, string aTaskName)
            : base(aTaskNo, aTaskName) {
        }

        //...............................................................................
        public void FillTestData() {

        }

        override public bool DoInit() {
            FillTestData();

            comProps = new TaskComProps();

            dataBlock = new DbVisu(iTaskNo, comProps);
            if (!dataBlock.DoInit())
                return false;

            comPlc = new Comm_N4T(iTaskNo);
            if (!comPlc.DoInit())
                return false;

            loader = new EventLoader(iTaskNo);
            //if ()

            return true;
        }

        //...............................................................................................
        public void Log(int aDesiredLevel, string aMsg) {
            Base.Log_TaskLevel(iTaskNo, aDesiredLevel, aMsg);
        }

        override public bool Edit(Window aParent) {
            return dataBlock.Edit(aParent);
        }

        public bool EditLoader(Window aParent) {
            return loader.Edit(aParent);
        }

        public bool EditN4Tprops(Window aParent) {
            Dlg_N4T_Props Dlg = new Dlg_N4T_Props();
            Dlg.Owner = aParent;
            Dlg.SetDlgData(comProps);
            if (Dlg.ShowDialog() == null)
                return false;
            Dlg.GetDlgData(comProps);
            return false;
        }

        int RoundPktime(int aPktime, int aRoundTo) {
            return (aPktime / aRoundTo) * aRoundTo;   // round to period
        }


        //...............................................................................
        /// <summary> Read the whole datablock and refresh the values stored in image </summary>
        tCommResult ReadSnap() {   // returns true, if succeeded

            Log(2, " SEND request");

            Stopwatch clock = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
            byte[] buf;
            tCommResult resCode = comPlc.comm_ReadDbVisu(out buf);
            clock.Stop();

            if (resCode == tCommResult.Timeout) {
                return resCode;
            }
            Log(2, " RECEIVED, in " + clock.ElapsedMilliseconds + " ms");

            // analyze incoming data
            dataBlock.AcceptDataBuf(buf);
            return tCommResult.OK;
        }

        //...............................................................................
        void MySleep(int iSeconds) {
            // sleeping is interrupted by EndReq
            for (int i = 0; i < iSeconds; i++) {
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                if (bEndReq)
                    break;
            }
        }

        //...............................................................................
        public void ThreadFun() {
            const int iERR_MAX = 5;
            const int iSLEEP_TIMEs = 60; // station dead - sleep time

            // Snap reading is executed most frequently of all
            int iLastTimeRead = RoundPktime( Base.PktimeNow(), iSCH_ReadSnapPeriod);   // round to period

            // create and initialize the scheduler for Archiving
            List<ArchListItem> archList = dataBlock.GetArchList();
            foreach (ArchListItem u in archList)
                u.iLastTime1 = RoundPktime(Base.PktimeNow(), u.iPeriod);

            int iConseqErrCnt = 0;    // number of consecutive timeouts = dead station detection

            // main Thread loop
            while (!bEndReq) {
                int iPktime = Base.PktimeNow();

                // - - - - snap reading - - - -
                if (iPktime >= (iLastTimeRead + iSCH_ReadSnapPeriod)) {
                    tCommResult resCode = ReadSnap();

                    // handle the timeout
                    if (resCode == tCommResult.Timeout) {
                        if (iConseqErrCnt < iERR_MAX)
                            iConseqErrCnt++;
                        if (iConseqErrCnt < iERR_MAX)
                            continue;                       // this is immediate "retry"

                        // station is dead - sleep for some time
                        bSleeping = true;
                        Log(1, " SLEEP mode entered");
                        MySleep(iSLEEP_TIMEs);
                        bSleeping = false;
                        continue;       // do "retry" after waiting
                    }

                    // handle the general error
                    if (resCode == tCommResult.GenErr) {
                        if (iConseqErrCnt < iERR_MAX)
                            iConseqErrCnt++;
                        if (iConseqErrCnt < iERR_MAX)
                            continue;                       // this is immediate "retry"

                        // connection is BAD - sleep for some time
                        bSleeping = true;
                        Log(1, " SLEEP mode entered");
                        MySleep(iSLEEP_TIMEs);
                        bSleeping = false;
                        continue;       // do "retry" after waiting
                    }

                    // here the correct reply is recieved
                    iLastTimeRead = RoundPktime(iPktime, iSCH_ReadSnapPeriod);
                    iConseqErrCnt = 0;
                }

                // - - - - Archiving - - - -
                foreach (ArchListItem u in archList) {
                    int iOptimalArchTime = u.iLastTime1 + u.iPeriod;
                    if (iPktime >= iOptimalArchTime) {
                        dataBlock.DoArchive(iOptimalArchTime, u);
                        u.iLastTime1 = RoundPktime(iPktime, u.iPeriod) + u.iOffset;  // pro jistotu znovu provedeme zaokrouhleni
                    }
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(1000));  // reduce the CPU load by some passive waiting
            }
            lock (Lock1) {
                thread = null;
            }
        }

        //...............................................................................
        override public void ThreadNew() {
            if (thread != null)
                return;
            bEndReq = false;
            thread = new Thread(ThreadFun);
            thread.Name = "ExN2_" + sTaskName;
            thread.Start();
        }

        override public void ThreadEndReq() {
            bEndReq = true;
        }

        override public tThreadState GetThreadState() {
            tThreadState state;
            
            lock (Lock1) {
                if (thread == null)
                    state = tThreadState.NULL;
                else if (thread.IsAlive)
                    state = tThreadState.RUN;
                else
                    state = tThreadState.STOP;
            }
            return state;
        }

        override public ShowTaskInfo getTaskProgress() {
            ShowTaskInfo info = comPlc.getTaskProgress();

            if (bSleeping) {
                info.sText += ", SLEEP";
                info.brush = Brushes.Orange;
            }
            if (thread == null)
                info.brush = Brushes.Yellow;
            return info;
        }
    }
}
