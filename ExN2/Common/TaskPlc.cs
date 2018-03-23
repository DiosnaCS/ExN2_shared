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
        CommProps comProps = null;

        public bool bEndReq = false;
        public Thread thread = null;
        Object Lock1 = new Object();

            
        // PLC specification
        string sPLC_IPaddr;
        int iPLC_Port;

        string sSQL_Database;
        string sSQL_TablePrefix;
        string sSQL_UserId;
        string sSQL_Password;

        // scheduler
        int iSCH_ReadSnapPeriod;
        int iSCH_Period_Arch;


        public TaskPlc(int aTaskNo, string aTaskName)
            : base(aTaskNo, aTaskName) {
        }

        //...............................................................................
        public void FillTestData() {
            sPLC_IPaddr = "192.168.2.99";
            iPLC_Port = 2000;

            sSQL_Database = "test";
            sSQL_TablePrefix = "ml_";
            sSQL_UserId = "postgres";
            sSQL_Password = "Nordit0276";

            iSCH_ReadSnapPeriod = 5;
            iSCH_Period_Arch = 20;
        }

        override public bool DoInit() {
            FillTestData();

            dataBlock = new DbVisu(iTaskNo);
            if (!dataBlock.DoInit())
                return false;

            comPlc = new Comm_N4T(iTaskNo);
            if (!comPlc.DoInit())
                return false;

            loader = new EventLoader(iTaskNo);
            //if ()

            comProps = new CommProps();

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
        public void ThreadFun() {
            int iLastTimeRead = RoundPktime( Base.PktimeNow(), iSCH_ReadSnapPeriod);   // round to period
            int iLastTimeArch = RoundPktime(Base.PktimeNow(), iSCH_Period_Arch);
            List<ArchListItem> archList = new List<ArchListItem>();

            archList = dataBlock.GetArchList();
            foreach (ArchListItem u in archList)


            while (!bEndReq) {
                int iPktime = Base.PktimeNow();

                if ((iPktime - iLastTimeRead) < iSCH_ReadSnapPeriod) {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                    continue;
                }

                Log(2, " SEND request");
                Stopwatch clock = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
                byte[] buf = comPlc.comm_ReadDbVisu();
                if (buf == null) {
                    continue;
                }
                clock.Stop();
                Log(2, " RECEIVED, in " + clock.ElapsedMilliseconds + " ms");
                iLastTimeRead = RoundPktime(Base.PktimeNow(), iSCH_ReadSnapPeriod);

                // analyze incoming data
                dataBlock.AcceptDataBuf(buf);

                if ((iPktime - iLastTimeArch) < iSCH_Period_Arch) {
                    continue;
                }
                iLastTimeArch = RoundPktime(Base.PktimeNow(), iSCH_Period_Arch);
                dataBlock.DoArchive(iLastTimeArch);

                //MainWindow.LogAdd(sCfgSubdirName + "  " + iPktime + ".\n");
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

        override public TaskShowInfo getTaskProgress() {
            TaskShowInfo info = comPlc.getTaskProgress();
            if (thread == null)
                info.brush = Brushes.Yellow;
            return info;
        }
    }
}
