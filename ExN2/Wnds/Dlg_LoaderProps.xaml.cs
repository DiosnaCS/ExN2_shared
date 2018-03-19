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

namespace ExN2 {

    /// <summary>
    /// Interaction logic for LoaderProps.xaml
    /// </summary>
    public partial class Dlg_LoaderProps : Window, INotifyPropertyChanged {

        // implement the "Property changed" mechanism
        public event PropertyChangedEventHandler PropertyChanged;

        // normalne se tato metoda vola v kazdem setteru, my ji volame hromadne z RefreshAllDlgItem
        protected void OnPropertyChanged(string vlastnost) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(vlastnost));
        }


        // properties binded to Dlg
        public bool Run { get; set; }
        public string DbConnStr { get; set; }
        public string TableName { get; set; }
        public string SysTableName { get; set; }
        public string UDPSocketLocal { get; set; }          
        public string UDPSocketRemote { get; set; }         
        public int ReceiveTimeoutMs { get; set; }           
        public bool IntelOrder { get; set; }                
        public bool LastPtrIsFreePtr { get; set; }          
        public tN4T_version N4T_Version { get; set; }       
        public int EventBodyLenBytes { get; set; }          
        public int TypeFieldByteOffs { get; set; }          
        public int AdjustTimePeriod_Sec { get; set; }       
        public int AdjustTimeOffset_Sec { get; set; }       
        public List<EventDef> EventsList { get; set; }

        // reference to edited Loader object
        EventLoader editedLoader;


        public Dlg_LoaderProps(EventLoader aEditedLoader) {
            editedLoader = aEditedLoader;       // save reference to original Loader object
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
        public void SetDlgData(CommonProps cfg, List<EventDef> evtCfg) {
            Run = cfg.bRun;
            DbConnStr = cfg.DB_ConnectString;
            TableName = cfg.DB_TableName;
            SysTableName = cfg.DB_SysTableName;
            UDPSocketLocal = cfg.SocketLocal;
            UDPSocketRemote = cfg.SocketRemote;
            ReceiveTimeoutMs = cfg.iRcvTimeoutMs;
            IntelOrder = cfg.bIntelOrder;
            N4T_Version = cfg.N4T_version;
            LastPtrIsFreePtr = cfg.bLastPtrIsFreePtr;
            EventBodyLenBytes = cfg.iEventBodyLenBytes;
            TypeFieldByteOffs = cfg.iTypeFieldByteOffs;
            AdjustTimePeriod_Sec = cfg.iAdjustTimePeriod_Sec;
            AdjustTimeOffset_Sec = cfg.iAdjustTimeOffset_Sec;
            EventsList = evtCfg;
        }

        /// <summary> read data from dialog to Loader cfg objects </summary>
        public void GetDlgData(CommonProps cfg, List<EventDef> evtCfg) {
            cfg.bRun = Run;
            cfg.DB_ConnectString = DbConnStr;
            cfg.DB_TableName = TableName;
            cfg.DB_SysTableName = SysTableName;
            cfg.SocketLocal = UDPSocketLocal;
            cfg.SocketRemote = UDPSocketRemote;
            cfg.iRcvTimeoutMs = ReceiveTimeoutMs;
            cfg.bIntelOrder = IntelOrder;
            cfg.N4T_version = N4T_Version;
            cfg.bLastPtrIsFreePtr = LastPtrIsFreePtr;
            cfg.iEventBodyLenBytes = EventBodyLenBytes;
            cfg.iTypeFieldByteOffs = TypeFieldByteOffs;
            cfg.iAdjustTimePeriod_Sec = AdjustTimePeriod_Sec;
            cfg.iAdjustTimeOffset_Sec = AdjustTimeOffset_Sec;
            evtCfg = EventsList;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void Button_AddEvent_Click(object sender, RoutedEventArgs e)
        {
            Wnds.Dlg_AddEvent Dlg2 = new Wnds.Dlg_AddEvent();
            bool done = (bool)Dlg2.ShowDialog();
            if (done == true)
            {
                if (EventsList == null)
                    EventsList = new List<EventDef>();
                EventsList.Add(new EventDef() { assocNums = Dlg2.EventTypes, Items = Dlg2.eventLineList});
                EventsListView.ItemsSource = EventsList;
            }
        }

        private void Button_DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            EventDef itemForEdit = (EventDef)EventsListView.SelectedItem;
            
            Wnds.Dlg_AddEvent Dlg3 = new Wnds.Dlg_AddEvent();
            bool done = (bool)Dlg3.ShowDialog();
            Dlg3.EventTypes = itemForEdit.assocNums;
            Dlg3.eventLineList = itemForEdit.Items;
            if (done == true)
            {
                if (EventsList == null)
                    EventsList = new List<EventDef>();
                EventsList.Remove(itemForEdit);
                EventsList.Add(new EventDef() { assocNums = Dlg3.EventTypes, Items = Dlg3.eventLineList });
                EventsListView.ItemsSource = EventsList;
            }
        }

        private void Button_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            if (EventsList == null)
                EventsList = new List<EventDef>();
            EventDef itemForDelete = (EventDef)EventsListView.SelectedItem;
            EventsList.Remove(itemForDelete);
            EventsListView.ItemsSource = EventsList;
        }

        /// <summary> Load data from ond INI file into Loader object and refresh the dialog </summary>
        private void button_LoadIni_Click(object sender, RoutedEventArgs e) {
            OpResult res = LoaderFileOps.LoadFromOldIni(editedLoader.iTaskNo, out editedLoader.props, out editedLoader.eventList);
            res.SetTextbox(textMsg);

            SetDlgData(editedLoader.props, editedLoader.eventList);
            RefreshAllDlgItem();
        }

        private void button_LoadXML_Click(object sender, RoutedEventArgs e) {
            OpResult res = LoaderFileOps.LoadFromXML(editedLoader.iTaskNo, out editedLoader.props, out editedLoader.eventList);
            res.SetTextbox(textMsg);

            SetDlgData(editedLoader.props, editedLoader.eventList);
            RefreshAllDlgItem();
        }

        private void button_SaveXML_Click(object sender, RoutedEventArgs e) {
            LoaderFileOps.SaveToXml(editedLoader.iTaskNo, editedLoader.props, editedLoader.eventList);
        }
    }
}
