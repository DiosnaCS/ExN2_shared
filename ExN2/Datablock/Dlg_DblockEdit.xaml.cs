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
    public partial class Dlg_DblockEdit : Window {
        DbVisu Obj;


        //...............................................................................................
        public Dlg_DblockEdit(DbVisu aDatablock) {
            Obj = aDatablock;
            InitializeComponent();
            listView.ItemsSource = Obj.Items;
            listViewTables.ItemsSource = Obj.Sections;
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
            listViewTables.Items.Refresh();
        }

        //...............................................................................................
        private void btnLoadFromXML_Click(object sender, RoutedEventArgs e) {
            OpResult res;
            res = Obj.LoadFromXML();
            res.SetTextbox(textMsg);
            listView.Items.Refresh();
            listViewTables.Items.Refresh();
        }

        //...............................................................................................
        private void btnSaveXML_Click(object sender, RoutedEventArgs e) {
            OpResult res;
            res = Obj.SaveToXML();
            res.SetTextbox(textMsg);
        }

        //...............................................................................................
        private void btnClear_Click(object sender, RoutedEventArgs e) {
            OpResult res = new OpResult();
            res.AddMsg("cleared OK");

            Obj.RemoveAllItems();
            res.SetTextbox(textMsg);

            listView.Items.Refresh();
            listViewTables.Items.Refresh();
        }

        //...............................................................................................
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        //...............................................................................................
        private void btnMakeSqlCreate_Click(object sender, RoutedEventArgs e) {

            // find the item selected in ListView
            ArchListItem selTable = (ArchListItem)listViewTables.SelectedItem;
            if (selTable == null)
                return;

            // construct the SQL command
            OpResult res = Obj.MakeSqlCmd_CreateTable(selTable);
            res.SetTextbox(textMsg);
            if (! res.bOK)
                return;

            // final question box
            MessageBoxResult result = MessageBox.Show(this, "Really CREATE the new SQL table ?", "SQL command: CREATE TABLE", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result != MessageBoxResult.OK) {
                res.AddErrMsg("Operation cancelled");
                res.SetTextbox(textMsg);
                return;
            }

            // execute
            OpResult res3 = Obj.DoQuery(res.sMsg);
            res3.SetTextbox(textMsg);
        }

        //...............................................................................................
        private void btnMakeSqlInsert_Click(object sender, RoutedEventArgs e) {
            // find the item selected in ListView
            ArchListItem selTable = (ArchListItem)listViewTables.SelectedItem;
            if (selTable == null)
                return;

            textMsg.Text = Obj.MakeSqlCmd_Insert(selTable, 0, false);
        }

        //...............................................................................................
        private void btnCwSchedule_Click(object sender, RoutedEventArgs e) {

            OpResult res1 = ControlWebUtils.CW_Create_SchedulePart(Obj);
            OpResult res2 = ControlWebUtils.CW_Create_ArchivePart(Obj);
            res1.Combine(res2);
            res1.SetTextbox(textMsg);
        }


        //...............................................................................................
        private void btnTableCheck_Click(object sender, RoutedEventArgs e) {
            OpResult res;
            List<ColumnDiff> diffList;

            ArchListItem selTable = (ArchListItem)listViewTables.SelectedItem;
            if (selTable == null)
                return;

            // compare structures and create the list of differences
            res = Obj.CheckTableStruct(selTable, out diffList);
            res.SetTextbox(textMsg);

            // show the difference list and allow some modification
            var dlg = new Dlg_DblockCompare(selTable, diffList);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true) {
            }

            res.SetTextbox(textMsg);

        }
    }
}
