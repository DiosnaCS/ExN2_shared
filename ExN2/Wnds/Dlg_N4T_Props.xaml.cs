using ExN2.Common;
using ExN2.Loader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExN2.CommPlc {

    public partial class Dlg_N4T_Props : Window, INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;         // implement the "Property changed" mechanism
        
        protected void OnPropertyChanged(string vlastnost) {            // normalne se tato metoda vola v kazdem setteru, my ji volame hromadne z RefreshAllDlgItem
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(vlastnost));
        }

        /// <summary> force all Dlg items to be redrawn </summary>
        public void RefreshAllDlgItems() {
            var propsInfo = this.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo info in propsInfo) {
                OnPropertyChanged(info.Name);
            }
        }


        // properties binded to Dlg
        public string sPLC_IPaddr_port { get; set; }
        public string sLocal_IPaddr_port { get; set; }
        public int iTimeoutMs { get; set; }
        public bool bIntelOrder { get; set; }
        public tN4T_version N4Tversion { get; set; }
        public string sSQL_ConnectString { get; set; }
        public string sSQL_TablePrefix { get; set; }

        int iTaskNo = 0;    // taskNo of edited object
        TaskComProps editedObj = null;

      
        //...............................................................................
        public Dlg_N4T_Props(int aTaskNo, TaskComProps aProps) {
            InitializeComponent();
            DataContext = this;    // data binding
            editedObj = aProps;
            iTaskNo = aTaskNo;
            SetDlgData(editedObj);
        }

        //...............................................................................
        /// <summary> copy data from Loader cfg objects to dialog </summary>
        public void SetDlgData(TaskComProps aObj) {
            sPLC_IPaddr_port = aObj.Data.sPLC_IPaddr + ":" + aObj.Data.iPLC_Port;
            sLocal_IPaddr_port = aObj.Data.sLocal_IPaddr + ":" + aObj.Data.iLocal_Port;
            bIntelOrder = aObj.Data.bIntelOrder;
            N4Tversion = aObj.Data.N4Tversion;
            iTimeoutMs = aObj.Data.iTimeoutMs;
            sSQL_ConnectString  = aObj.Data.sSQL_ConnectString;
            sSQL_TablePrefix = aObj.Data.sSQL_TablePrefix;
        }

        //...............................................................................
        /// <summary> read data from dialog to Loader cfg objects </summary>
        public bool GetDlgData(TaskComProps aObj) {
            try {
                aObj.Data.sPLC_IPaddr = sPLC_IPaddr_port.Split(':')[0];
                aObj.Data.iPLC_Port = int.Parse(sPLC_IPaddr_port.Split(':')[1]);
                aObj.Data.sLocal_IPaddr = sLocal_IPaddr_port.Split(':')[0];
                aObj.Data.iLocal_Port = int.Parse(sLocal_IPaddr_port.Split(':')[1]);
            }
            catch (Exception){  // catch Splitting errors
                return false;
            }
            aObj.Data.bIntelOrder = bIntelOrder;
            aObj.Data.N4Tversion = N4Tversion;
            aObj.Data.iTimeoutMs = iTimeoutMs;
            aObj.Data.sSQL_ConnectString = sSQL_ConnectString;
            aObj.Data.sSQL_TablePrefix = sSQL_TablePrefix;
            return true;
        }

        //...............................................................................
        private void btn_OK_Click(object sender, RoutedEventArgs e) {
            TaskComProps tmpObj = new TaskComProps();

            if (! GetDlgData(tmpObj)) {
                MessageBoxResult result = MessageBox.Show(this, "Format error", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            editedObj.Data = tmpObj.Data;
            DialogResult = true;
        }

        //...............................................................................
        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        //...............................................................................
        private void btn_LoadXML_Click(object sender, RoutedEventArgs e) {
            TaskComProps tmpObj = new TaskComProps();
            OpResult res = tmpObj.LoadFromXML(iTaskNo);
            SetDlgData(tmpObj);
            RefreshAllDlgItems();
        }

        //...............................................................................
        private void btn_SaveXML_Click(object sender, RoutedEventArgs e) {
            TaskComProps tmpObj = new TaskComProps();
            GetDlgData(tmpObj);
            tmpObj.SaveToXML(iTaskNo);
        }
    }
}
