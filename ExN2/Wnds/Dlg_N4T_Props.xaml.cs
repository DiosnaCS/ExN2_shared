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

    public partial class Dlg_N4T_Props : Window {

        // properties binded to Dlg
        public string sPLC_IPaddr_port { get; set; }
        public string sLocal_IPaddr_port { get; set; }
        public int iTimeoutMs { get; set; }
        public bool bIntelOrder { get; set; }
        public tN4T_version N4Tversion { get; set; }
        public string sSQL_ConnectString { get; set; }
        public string sSQL_TablePrefix { get; set; }

        TaskComProps editedObj;

        //...............................................................................
        public Dlg_N4T_Props(TaskComProps aComProps) {
            editedObj = aComProps;
            InitializeComponent();
            DataContext = this;    // data binding
            SetDlgData();
        }

        //...............................................................................
        /// <summary> copy data from Loader cfg objects to dialog </summary>
        public void SetDlgData() {
            sPLC_IPaddr_port = editedObj.sPLC_IPaddr + ":" + editedObj.iPLC_Port;
            sLocal_IPaddr_port = editedObj.sLocal_IPaddr + ":" + editedObj.iLocal_Port;
            bIntelOrder = editedObj.bIntelOrder;
            N4Tversion = editedObj.N4Tversion;
            iTimeoutMs = editedObj.iTimeoutMs;
            sSQL_ConnectString  = editedObj.sSQL_ConnectString;
            sSQL_TablePrefix = editedObj.sSQL_TablePrefix;
        }

        //...............................................................................
        /// <summary> read data from dialog to Loader cfg objects </summary>
        public bool GetDlgData() {
            try {
                editedObj.sPLC_IPaddr = sPLC_IPaddr_port.Split(':')[0];
                editedObj.iPLC_Port = int.Parse(sPLC_IPaddr_port.Split(':')[1]);
                editedObj.sLocal_IPaddr = sLocal_IPaddr_port.Split(':')[0];
                editedObj.iLocal_Port = int.Parse(sLocal_IPaddr_port.Split(':')[1]);
            }
            catch (Exception){  // catch Splitting errors
                return false;
            }
            editedObj.bIntelOrder = bIntelOrder;
            editedObj.N4Tversion = N4Tversion;
            editedObj.iTimeoutMs = iTimeoutMs;
            editedObj.sSQL_ConnectString = sSQL_ConnectString;
            editedObj.sSQL_TablePrefix = sSQL_TablePrefix;
            return true;
        }

        //...............................................................................
        private void btn_OK_Click(object sender, RoutedEventArgs e) {
            if (! GetDlgData()) {
                MessageBoxResult result = MessageBox.Show(this, "Format error", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            DialogResult = true;
        }

        //...............................................................................
        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
