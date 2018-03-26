using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

using ExN2.Common;

namespace ExN2 {
    public class TaskInfo {
        public string sTaskName;
        public string sTaskSubdir;  // with ending backslash
        public int iLogLevel = 1;
    }
    
    
    //-------------------------------------------------------------------
    // veci spolecne pro cely program
    //-------------------------------------------------------------------
    public static class Base {
        // all paths must end with backslash
        public static string sPathAppRoot   = @"c:\Akce\C#\ExN2\ExN2\";
        public static string sConfigSubdir  = @"config\";

        //public static string cfgFilesPath   = @"c:\Akce\C#\ExN2\ExN2\import\";
        public static string bmpFilesPath   = @"c:\Akce\C#\ExN2\ExN2\bmp\";
        public static string version        = "1.06";
        static DateTime dtPktimeOrigin = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);    // for Pktime conversions

        public static List<TaskBase> Tasks = new List<TaskBase>();

        // info array for fast access to task using task number
        public const int iTASK_MAX = 999;
        public static TaskInfo[] taskInfo = new TaskInfo[iTASK_MAX];

        public static int iLOGLEVEL_MAX = 5;

        public const int iDISP_LOG_LINES = 13;
        static List<string>  sLogArr = new List<string>();

        //...............................................................................
        /// <summary> Search for subdirectories with numerical name and create corresponding tasks </summary>
        public static void SearchTaskSubdirs() {
            IEnumerable<string> dirList = Directory.EnumerateDirectories(sPathAppRoot + sConfigSubdir, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string subdir in dirList) {
                int iTaskNum;
                string sDirName = System.IO.Path.GetFileNameWithoutExtension(subdir);
                if (int.TryParse(sDirName.Substring(0, 3), out iTaskNum)) {
                    if ((iTaskNum <= 0) | (iTaskNum > iTASK_MAX))
                        continue;
                    string sTaskName = sDirName.Length >= 4   ?   sDirName.Substring(4)  :  "noname";

                    // create new task object an append to tasklist
                    TaskBase newItem = new TaskPlc(iTaskNum, sTaskName);
                    Tasks.Add(newItem);

                    // create new tank info and put in array
                    TaskInfo newTaskInfo = new TaskInfo();
                    newTaskInfo.sTaskName = sTaskName;
                    newTaskInfo.sTaskSubdir = sDirName + @"\";
                    taskInfo[iTaskNum] = newTaskInfo;
                }

            }
        }

        //...............................................................................
        /// <summary> Initialize all tasks </summary>
        public static void InitTasks() {
            foreach (TaskBase task in Tasks) {
                task.DoInit();
            }
        }

        //...............................................................................
        public static void SysMsgBox(string Msg) {
            // DODELAT
            System.Windows.MessageBox.Show(Msg, "Internal error");
        }

        /// <summary> Append message to logfile in "process" subdirectory </summary>
        public static void Log_Task(int aTaskNo, string aMsg, bool bTimestamp = false) {
            string sLogFileName = Base.sPathAppRoot + sConfigSubdir + taskInfo[aTaskNo].sTaskSubdir + "log";    // without extension
            LogHelper(aTaskNo, sLogFileName, aMsg, bTimestamp);
        }

        /// <summary> Append message to logfile in "process" subdirectory, depending on LogLevel </summary>
        public static void Log_TaskLevel(int aTaskNo, int aDesiredLogLevel, string aMsg, bool bTimestamp = false) {
            TaskInfo info = taskInfo[aTaskNo];

            if (info.iLogLevel < aDesiredLogLevel)
                return;
            string sLogFileName = Base.sPathAppRoot + sConfigSubdir + info.sTaskSubdir + "log";    // without extension
            LogHelper(aTaskNo, sLogFileName, aMsg, bTimestamp);
        }

        /// <summary> Append message to main logfile </summary>
        public static void Log_Sys(string aMsg, bool bTimestamp = false) {
            string sLogFileName = Base.sPathAppRoot + sConfigSubdir + "log";    // without extension
            LogHelper(0, sLogFileName, aMsg, bTimestamp);
        }

        static void LogHelper(int aTaskNo, string aLogFileNameWithoutExt, string aMsg, bool bTimestamp = false) {
            string sLogFileName = aLogFileNameWithoutExt + ".txt";

            // if main file full, rename the main logfile to backup file
            long iFileSize = 0;
            try {
                iFileSize = new FileInfo(sLogFileName).Length;
            }
            catch {
            }
            if (iFileSize > 1000000) {
                string sLogFileNameOld = Base.sPathAppRoot + sConfigSubdir + "1.txt";
                System.IO.File.Delete(sLogFileNameOld);
                System.IO.File.Move(sLogFileName, sLogFileNameOld);
            }

            // append a messge to logfile
            using (StreamWriter writer = new StreamWriter(sLogFileName, true)) {
                string sTime;
                if (bTimestamp)
                    sTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                else
                    sTime = "";
                
                writer.WriteLine(sTime + aMsg);
            }

            // insert a message to display log buffer
            if (sLogArr.Count >= iDISP_LOG_LINES)
                sLogArr.RemoveAt(0);

            string sShortTime = DateTime.Now.ToString("mm:ss");
            if (aTaskNo == 0)
                sLogArr.Add(String.Format("{0}   {1}", sShortTime, aMsg));
            else
                sLogArr.Add(String.Format("{0}   {1:000}: {2}", sShortTime, aTaskNo, aMsg));
        }

        public static string GetSysLogStrings() {
            string sConcat = "";
            foreach (string S in sLogArr)
                sConcat += S + '\n';
            return sConcat;
        }

        //...............................................................................
        public static Bitmap CreateBitmapFromResource(String Name) {
            Assembly _assembly;
            Stream _imageStream;
            Bitmap Bmp;
            try {
                _assembly = Assembly.GetExecutingAssembly();
                _imageStream = _assembly.GetManifestResourceStream("Resources." + Name);
            }
            catch {
                SysMsgBox("CreateBitmapFromResource / Resource access error: " + Name);
                return null;
            };

            try {
                Bmp = new Bitmap(_imageStream);
            }
            catch {
                SysMsgBox("CreateBitmapFromResource / Resource access error: " + Name);
                return null;
            }

            return Bmp;
        }

        //...............................................................................
        public static DateTime PktimeToDatetime(int iPktime) {
            return dtPktimeOrigin.AddSeconds(iPktime);
        }

        //...............................................................................
        public static int DatetimeToPktime(DateTime dtDate) {
            TimeSpan diff = dtDate.ToUniversalTime() - dtPktimeOrigin;
            return (int)Math.Floor(diff.TotalSeconds);
        }

        //...............................................................................
        public static int PktimeNow() {
            TimeSpan diff = DateTime.UtcNow - dtPktimeOrigin;
            return (int)Math.Floor(diff.TotalSeconds);
        }

    }

   

}
