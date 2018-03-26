using System.Windows;
using System.Windows.Media;

namespace ExN2.Common {

    public class TaskComProps {
        // UDP specification
        public string sPLC_IPaddr;
        public int iPLC_Port;
        public string sLocal_IPaddr;
        public int iLocal_Port;
        public int iTimeoutMs;

        // SQL specification
        public string sSQL_ConnectString;
        public string sSQL_TablePrefix;

        public TaskComProps() {     // constructor only for Debug
            sPLC_IPaddr = "192.168.2.111";
            iPLC_Port = 2000;
            sLocal_IPaddr = "";
            iLocal_Port = 0;
            iTimeoutMs = 1000;

            sSQL_ConnectString = string.Format("Server={0};Port=5432;User Id={1};Password={2};Database={3}", "127.0.0.1", "postgres", "Nordit0276", "Test");
            sSQL_TablePrefix = "ml_";
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
