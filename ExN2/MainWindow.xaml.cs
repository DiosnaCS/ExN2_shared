using System;
using System.ComponentModel;
using System.Windows;

using System.Windows.Threading;

using Microsoft.Win32;
using System.IO;
using System.Reflection;
using ExN2.Common;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;

namespace ExN2
{

    public class Props1 {
        public bool bSnapRead { get; set; }
        public bool bArchive { get; set; }
        public bool bAlarm { get; set; }
        public bool bLoader { get; set; }
        public bool bTimeAdj { get; set; }
    }

    public partial class MainWindow : Window {

        string sLog = "";
        static MainWindow pMainWnd;


        public MainWindow() {
            pMainWnd = this;
            InitializeComponent();

            // config subdirectory is essential for program
            string sCurrDirName = Directory.GetCurrentDirectory();
            string sCfgDirName = sCurrDirName + @"\" + Base.sConfigSubdir;
            if (! Directory.Exists(sCfgDirName) ) {
                MessageBox.Show("'config' subdirectory not found.\n\ncurrent dir: " + sCurrDirName,
                                "ExN2 - Startup error", MessageBoxButton.OK);
                Close();
                return;
            }

            Base.Log_Sys("- - - PROGRAM START, " + Base.version + " - - -", true);

            Base.SearchTaskSubdirs();
            Base.InitTasks();

            plcListView.ItemsSource = Base.Tasks;
            plcListView.SelectionMode = System.Windows.Controls.SelectionMode.Single;

            // show the version and bulid date
            DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
            label_Ver.Content = "ver " + Base.version + ", build:" + buildDate;

            Closing += tkOnWindowClosing;

            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Start();
            tmr.Interval = new TimeSpan(0, 0, 1);
            tmr.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) { 
            textbox_Log.Text = Base.GetSysLogStrings(); 

            foreach (TaskPlc task in Base.Tasks) {
                task.ThreadRefreshState();
            }

            foreach(var item in plcListView.Items) {
                //item.Background = Brushes.Red
               // GridView ob = (GridView)item;
                //int i = ob.Columns.Count;
            }
            //task.OnPropertyChanged("view_TaskState");


            plcListView.Items.Refresh();
        }

        public void tkOnWindowClosing(object sender, CancelEventArgs e) {
            Base.Log_Sys("- - - PROGRAM SHUTDOWN - - -", true);

            // ask all threads for END
            foreach (TaskBase task in Base.Tasks) {
                ((TaskPlc)task).ThreadEndReq();
            }

            // wait until all threads are ended
            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
            while (stopwatch.ElapsedMilliseconds < 5000) {
                bool bAnyRunning = false;
                foreach (TaskPlc task in Base.Tasks) {
                    if (task.GetThreadState() == tThreadState.RUN) {
                        bAnyRunning = true;
                        //task.plcRequester.thread.Join();
                    }
                }
                if (!bAnyRunning)
                    break;
            }
            stopwatch.Stop();
        }

        private void button_Exit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void button_Open_Click(object sender, RoutedEventArgs e) {
            //            treeView.ItemsSource = nodes;
        }

        private void button_Save_Click(object sender, RoutedEventArgs e) {
            //nodes[0].LeafList[1].LeafName = "Zmena";
        }

        private void btn_Run_Click(object sender, RoutedEventArgs e) {
            if (plcListView.SelectedIndex < 0)      // check if any item is selected
                return;
            TaskPlc task = (TaskPlc)plcListView.SelectedItem;
            task.DoInit();
            task.ThreadNew();
        }

        //...............................................................................
        private void btn_Stop_Click(object sender, RoutedEventArgs e) {
            if (plcListView.SelectedIndex < 0)      // check if any item is selected
                return;
            TaskPlc task = (TaskPlc)plcListView.SelectedItem;
            task.ThreadEndReq();
        }


        //...............................................................................
        private void MenuItem_Help(object sender, RoutedEventArgs e) {
            /*            Dlg_About Dlg = new Dlg_About();
                        Dlg.Owner = this;
                        Dlg.ShowDialog();*/

            String Msgs;
            //XX.LoadFromIni(@"c:\Akce\C#\Vizu_TK_02\ExN2\import\dbVizu.db", out Msgs);
            //XX.Edit(this);

            //SqlTest x = new SqlTest();
            //x.Test();
        }

        //...............................................................................
        private void MenuItem_LoadOldIni(object sender, RoutedEventArgs e) {
            /*Dlg_ConvertOldLoader Dlg = new Dlg_ConvertOldLoader();
            Dlg.Owner = this;
            Dlg.ShowDialog();
            CfgTreeNode_Loaders_VM node0 = nodes[0] as CfgTreeNode_Loaders_VM;
            if (Dlg.loadersList.CfgEventLoaderList != null)
            {
                foreach (var loader in Dlg.loadersList.CfgEventLoaderList)
                {
                    //loader.LeafName
                    node0.AddLeaf(loader);
                }
            }
            treeView.ItemsSource = nodes;  */
        }

        //...............................................................................
        private void SomeCommand(object sender, RoutedEventArgs e) {
            System.Windows.MessageBox.Show("Esc");

        }

        //...............................................................................
        private void TestVlakna_Click(object sender, RoutedEventArgs e) {
            textbox_Log.Text = "ahoj";
            //pProc = PlcRequester.MakeNewThread("plc1");
        }

        //...............................................................................
        public static void LogAdd(string aS) {
            pMainWnd.sLog += aS;
            //pMainWnd.textbox_Log.Text = "xxx"; // pMainWnd.sLog;
        }

        private void menu_Vlakno_Redraw_Click(object sender, RoutedEventArgs e) {
            //textbox_Log.Text = sLog;
            /*OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == true) {
                MessageBox.Show(theDialog.FileName.ToString());
            } */
        }

        /// <summary> Helper - return selected Task or null </summary>
        private TaskBase getSelectedTask() {
            if (plcListView.SelectedIndex < 0)      // check if any item is selected
                return null;
            return (TaskBase)plcListView.SelectedItem;
        }

        private void btn_LevelDn_Click(object sender, RoutedEventArgs e) {
            TaskBase task;
            if ((task = getSelectedTask()) == null)
                return;
            task.LogLevel_Dn();
            plcListView.Items.Refresh();
        }

        private void bth_LevelUp_Click(object sender, RoutedEventArgs e) {
            TaskBase task;
            if ((task = getSelectedTask()) == null)
                return;
            task.LogLevel_Up();
            plcListView.Items.Refresh();
        }

        //...............................................................................
        private void btn_EditMain_Click(object sender, RoutedEventArgs e) {
            if (plcListView.SelectedIndex < 0)      // check if any item is selected
                return;
            TaskBase task = (TaskBase)plcListView.SelectedItem;
            if (task.Edit(this))
                plcListView.Items.Refresh();
        }

        private void btn_EditLoader_Click(object sender, RoutedEventArgs e) {
            TaskBase task;
            if ((task = getSelectedTask()) == null)
                return;
            ((TaskPlc)task).EditLoader(this);
        }

        private void btn_EditN4T_Click(object sender, RoutedEventArgs e) {
            TaskBase task;
            if ((task = getSelectedTask()) == null)
                return;
            ((TaskPlc)task).EditN4Tprops(this);
        }

    }
}
