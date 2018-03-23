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

    /// <summary>
    /// Interaction logic for LoaderProps.xaml
    /// </summary>
    public partial class Dlg_N4T_Props : Window, INotifyPropertyChanged {

        // implement the "Property changed" mechanism
        public event PropertyChangedEventHandler PropertyChanged;

        // normalne se tato metoda vola v kazdem setteru, my ji volame hromadne z RefreshAllDlgItem
        protected void OnPropertyChanged(string vlastnost) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(vlastnost));
        }


        // properties binded to Dlg
        public string sPLC_IPaddr_port { get; set; }
        public string sLocal_IPaddr_port { get; set; }
        public int iTimeoutMs { get; set; }

        // SQL specification
        public string sSQL_ConnectString { get; set; }
        public string sSQL_Database { get; set; }
        public string sSQL_TablePrefix { get; set; }
        public string sSQL_UserId { get; set; }
        public string sSQL_Password { get; set; }


        public Dlg_N4T_Props() {
            InitializeComponent();
            DataContext = this;    // data binding
        }

        /// <summary> force all Dlg items to be redrawn </summary>
        public void RefreshAllDlgItem() {
            var propsInfo = this.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo info in propsInfo) {
                OnPropertyChanged(info.Name);
            } 
        }

        /// <summary> copy data from Loader cfg objects to dialog </summary>
        public void SetDlgData(CommProps aEditedObj) {
            sPLC_IPaddr_port = aEditedObj.sPLC_IPaddr + ":" + aEditedObj.iPLC_Port;
            sLocal_IPaddr_port = aEditedObj.sLocal_IPaddr + ":" + aEditedObj.iLocal_Port;

            iTimeoutMs  = aEditedObj.iTimeoutMs;
            sSQL_ConnectString  = aEditedObj.sSQL_ConnectString;
            sSQL_Database = aEditedObj.sSQL_Database;
            sSQL_TablePrefix = aEditedObj.sSQL_TablePrefix;
            sSQL_UserId = aEditedObj.sSQL_UserId;
            sSQL_Password = aEditedObj.sSQL_Password;
    }

    /// <summary> read data from dialog to Loader cfg objects </summary>
    public void GetDlgData(CommProps aEditedObj) {
            aEditedObj.sPLC_IPaddr = sPLC_IPaddr_port.Split(':')[0];
            aEditedObj.iPLC_Port = int.Parse(sPLC_IPaddr_port.Split(':')[1]);
            aEditedObj.sLocal_IPaddr = sLocal_IPaddr_port.Split(':')[0];
            aEditedObj.iLocal_Port = int.Parse(sLocal_IPaddr_port.Split(':')[1]);

            aEditedObj.iTimeoutMs = iTimeoutMs;
            aEditedObj.sSQL_ConnectString = sSQL_ConnectString;
            aEditedObj.sSQL_Database = sSQL_Database;
            aEditedObj.sSQL_TablePrefix = sSQL_TablePrefix;
            aEditedObj.sSQL_UserId = sSQL_UserId;
            aEditedObj.sSQL_Password = sSQL_Password;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
