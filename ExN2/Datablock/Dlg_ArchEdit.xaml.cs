using System;
using System.Collections.Generic;
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


using System.Runtime.InteropServices;
using ExN2.Common;
using ExN2.Datablock.Dlg;

namespace ExN2.Datablock {


    //=====================================================================================
    // Interaction logic for Dlg_ArchEdit.xaml
    public partial class Dlg_ArchEdit : Window {
        DbVisu Obj;

        //...............................................................................................
        public Dlg_ArchEdit(DbVisu aDatablock) {
            Obj = aDatablock;
            InitializeComponent();
            listView.ItemsSource = Obj.Items;
        }

        //...............................................................................................
        // for all selected rows changes the "archived" flag.
        void SetArchived(bool aToogle) {
            foreach (DbVisuItem u in listView.SelectedItems) {
                if (aToogle)
                    u.bArchive = !u.bArchive;
                else
                    u.bArchive = true;
            }
            if (listView.SelectedItems.Count > 0)
                listView.Items.Refresh();
        }

        //...............................................................................................
        private void btnToggleArch_Click(object sender, RoutedEventArgs e) {
            SetArchived(true);
            // tady by melo byt obnoveni focusu, ale vzdy se pri tom nastavi zvolena polozka na 0
        }

        //...............................................................................................
        private void btnSetArch_Click(object sender, RoutedEventArgs e) {
            SetArchived(false);
        }

        //...............................................................................................
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e) {
            SetArchived(true);
        }

        //...............................................................................................
        private void btnLoadFromIni_Click(object sender, RoutedEventArgs e) {
            OpResult opRes;

            opRes = Obj.LoadFromIni();
            opRes.SetTextbox(textMsg);

            listView.Items.Refresh();
        }

        //...............................................................................................
        private void btnLoadFromXML_Click(object sender, RoutedEventArgs e) {
            string sOutMsg;
            Obj.LoadFromXML(out sOutMsg);
            listView.Items.Refresh();
        }

        //...............................................................................................
        private void btnSaveXML_Click(object sender, RoutedEventArgs e) {
            string sOutMsg;
            Obj.SaveToXML(out sOutMsg);
        }

        //...............................................................................................
        private void btnClear_Click(object sender, RoutedEventArgs e) {
            Obj.RemoveAllItems();
            listView.Items.Refresh();
        }

        //...............................................................................................
        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            string sOutMsg = Obj.MakeSqlCmd_CreateTable();
            textMsg.Text = sOutMsg;
        }



        //...............................................................................................
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        //...............................................................................................
        private void btnTest1_Click(object sender, RoutedEventArgs e) {
            textMsg.Text = Obj.MakeSqlCmd_CreateTable();
        }

        //...............................................................................................
        private void btnTest2_Click(object sender, RoutedEventArgs e) {
            textMsg.Text = Obj.MakeSqlCmd_Insert(false, 0);
        }

        //...............................................................................................
        private void btnCwSchedule_Click(object sender, RoutedEventArgs e) {

            /*OpResult res1 = Obj.CW_Create_SchedulePart(@"c:\Akce\C#\ExN2\ExN2\import\cw_sched.txt");
            OpResult res2 = Obj.CW_Create_ArchivePart(@"c:\Akce\C#\ExN2\ExN2\import\cw_arch.txt");
            res1.Combine(res2);
            res1.SetTextbox(textMsg); */
        }

        //...............................................................................................
        private void btnChkStuct_Click(object sender, RoutedEventArgs e) {
            OpResult res;
            List<ColumnDiff> diffList;

            // compare structures and create the list of differences
            res = Obj.CheckTableStruct(out diffList);
            res.SetTextbox(textMsg);

            // show the difference list and allow some modification
            Dlg_ArchCompare dlg = new Dlg_ArchCompare(diffList);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true) {
            }

            res.SetTextbox(textMsg);
        }
    }
}
