using ExN2.Common;
using Npgsql;
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

namespace ExN2.Datablock.Dlg {
    /// <summary>
    /// Interaction logic for Dlg_ArchCompare.xaml
    /// </summary>
    public partial class Dlg_DblockCompare : Window {
        List<ColumnDiff>    diffList;
        ArchListItem        tableSpec;
        TaskComPropsData    comProps;

        public Dlg_DblockCompare(ArchListItem aTableSpec, List<ColumnDiff> aDiffList, TaskComPropsData aComProps) {
            InitializeComponent();

            diffList = aDiffList;
            tableSpec = aTableSpec;
            comProps = aComProps;

            listView.ItemsSource = aDiffList;
            btnExec.IsEnabled = true;

        }

        private void btnCancel_Close(object sender, RoutedEventArgs e) {
            Close();
        }

        /// <summary>Create SQL command and show ONLY</summary>
        private void btnMakeSql_Click(object sender, RoutedEventArgs e) {
        OpResult res;
            string sDropCmd;
            string sAddCmd;

            // prepare statements ALTER TABLE ADD/DROP COLUMN
            res = DbVisu.MakeSqlCmd_Modify(tableSpec, diffList, out sDropCmd, out sAddCmd);
            res.SetTextbox(textMsg);
        }

        /// <summary>Create SQL command and EXECUTE</summary>
        private void btnExecSql_Click(object sender, RoutedEventArgs e) {
            OpResult res;
            string sDropCmd;
            string sAddCmd;

            // prepare statements ALTER TABLE ADD/DROP COLUMN
            res = DbVisu.MakeSqlCmd_Modify(tableSpec, diffList, out sDropCmd, out sAddCmd);
            if (!res.bOK) {
                return;
            }

            // nothing to do
            res.Init();
            if ((sDropCmd == null) & (sAddCmd == null)) {
                res.AddErrMsg("NO operation executed");
                res.SetTextbox(textMsg);
                btnExec.IsEnabled = false;
                return;
            }

            // confirmnation of DROP operation
            res.Init();
            if (sDropCmd != null) {
                MessageBoxResult result = MessageBox.Show(this, "The data in dropped columns will be LOST ?", "Table DROP COLUMN command", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result != MessageBoxResult.OK) {
                    res.AddErrMsg("Operation cancelled");
                    res.SetTextbox(textMsg);
                    return;
                }
            }

            //string sTabName = "tabul1";
            NpgsqlConnection conn;
            conn = new NpgsqlConnection(comProps.sSQL_ConnectString);
            try {
                conn.Open();
                // do the DROP COLUMN command
                if (sDropCmd != null) {
                    NpgsqlCommand cmd = new NpgsqlCommand(sDropCmd, conn);
                    int iRowsAffected = cmd.ExecuteNonQuery();
                    res.AddMsg("DROP command OK");
                }

                // do the ADD COLUMN command
                if (sAddCmd != null) {
                    NpgsqlCommand cmd = new NpgsqlCommand(sAddCmd, conn);
                    int iRowsAffected = cmd.ExecuteNonQuery();
                    res.AddMsg("ADD command OK");
                }

            }
            catch (Exception ex) {
                res.AddErrMsg("btnExecSql_Click error: " + ex.Message);
            }
            conn.Close();

            btnExec.IsEnabled = false;
            res.SetTextbox(textMsg);
        }

    }
}
