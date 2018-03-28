using ExN2.CommPlc;
using ExN2.Loader;
using System;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace ExN2.Common {

    // spolecne parametry
    public class TaskComPropsData {
        // UDP specification
        public string sPLC_IPaddr;
        public int iPLC_Port;
        public string sLocal_IPaddr;
        public bool bIntelOrder;
        public tN4T_version N4Tversion;
        public int iLocal_Port;
        public int iTimeoutMs;

        // SQL specification
        public string sSQL_ConnectString;
        public string sSQL_TablePrefix;


        public TaskComPropsData() {     // constructor only for Debug
            sPLC_IPaddr = "192.168.2.111";
            iPLC_Port = 2000;
            sLocal_IPaddr = "localhost";
            iLocal_Port = 0;
            iTimeoutMs = 1000;
            bIntelOrder = false;
            N4Tversion = tN4T_version.n4t_undef;

            sSQL_ConnectString = string.Format("Server={0};Port=5432;User Id={1};Password={2};Database={3}", "192.168.2.12", "postgres", "Nordit0276", "Test");
            sSQL_TablePrefix = "ml_";
        }

    }

    // zapouzdrovaci trida, kvuli editaci, Load apod..
    public class TaskComProps {
        const string sPROPS_XML = "Props.XML";      // filename

        public TaskComPropsData Data = new TaskComPropsData();


        //...............................................................................................
        public bool DoInit(int aTaskNo) {
            OpResult res = LoadFromXML(aTaskNo);
            return true;
        }

        //...............................................................................................
        public OpResult SaveToXML(int aTaskNo) {
            var res = new OpResult();

            string sFullNameXML = Base.sPathAppRoot + Base.sConfigSubdir + Base.taskInfo[aTaskNo].sTaskSubdir + sPROPS_XML;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(TaskComPropsData));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (var sww = new System.IO.StreamWriter(sFullNameXML)) {
                using (XmlWriter writer = XmlWriter.Create(sww, settings)) {
                    xsSubmit.Serialize(writer, Data);
                }
                res.AddMsg("XML file saved OK:  " + sFullNameXML);
            }
            return res;
        }

        //...............................................................................................
        public OpResult LoadFromXML(int aTaskNo) {
            var res = new OpResult();

            string sFullNameXML = Base.sPathAppRoot + Base.sConfigSubdir + Base.taskInfo[aTaskNo].sTaskSubdir + sPROPS_XML;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(TaskComPropsData));

            try {
                using (var sww = new System.IO.StreamReader(sFullNameXML)) {
                    using (XmlReader reader = XmlReader.Create(sww)) {
                        TaskComPropsData newObj = (TaskComPropsData)(xsSubmit.Deserialize(reader));
                        Data = newObj;
                    }
                    res.AddMsg("XML file loaded OK:  " + sFullNameXML);
                }
            }
            catch (Exception e) {
                res.AddErrMsg(e.Message);
            }

            return res;
        }

        //...............................................................................
        public bool Edit(Window aParent, int aTaskNo) {
            Dlg_N4T_Props Dlg = new Dlg_N4T_Props(aTaskNo, this);
            Dlg.Owner = aParent;
            if (Dlg.ShowDialog() == null)
                return false;
            return true;
        }


    }

    public enum tThreadState {
        NULL,       // not created
        RUN,        // running = alive
        STOP        // ended but not disposed
    }

    public class TaskBase /*: INotifyPropertyChanged */{
       // public event PropertyChangedEventHandler PropertyChanged;

        protected int iTaskNo;
        protected string sTaskName;
        protected tThreadState threadState;


        public TaskBase(int aTaskNo, string aTaskName) {
            iTaskNo = aTaskNo;
            sTaskName = aTaskName;
            threadState = tThreadState.NULL;
        }

        public int view_OrderNo {
            get { return iTaskNo; }
        }

        public string view_TaskName {
            get { return sTaskName; }
        }

        public string view_ThreadState {
            get { return threadState.ToString(); }
        }

        public Brush view_InfoColor {
            get {
                ShowTaskInfo info = getTaskProgress();
                return info.brush;
            }
        }

        public string view_InfoText {
            get {
                ShowTaskInfo info = getTaskProgress();
                return info.sText;
            }
        }

        public string view_IconUri {
            get { return "pack://application:,,,/resources/" + "archiver.png"; }
        }

        public int view_LogLevel {
            get { return Base.taskInfo[iTaskNo].iLogLevel; }
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            //PropertyChangedEventHandler handler = PropertyChanged;
          /*  if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));*/
        }

        virtual public bool DoInit() {
            return false;   // must be overriden
        }

        virtual public bool Edit(Window aParent) {
            return false;   // must be overriden
        }

        virtual public void ThreadNew() {
            // must be overriden
        }

        virtual public void ThreadEndReq() {
            // must be overriden
        }

        virtual public tThreadState GetThreadState() {
            return tThreadState.NULL; // must be overriden
        }

        public void ThreadRefreshState() {
            threadState = GetThreadState();
        }

        public void LogLevel_Up() {
            TaskInfo info = Base.taskInfo[iTaskNo];
            if  (info.iLogLevel < Base.iLOGLEVEL_MAX)
                info.iLogLevel++;
        }

        public void LogLevel_Dn() {
            TaskInfo info = Base.taskInfo[iTaskNo];
            if (info.iLogLevel > 0)
                info.iLogLevel--;
        }

        virtual public ShowTaskInfo getTaskProgress() {
            return new ShowTaskInfo();
        }
    }
}
