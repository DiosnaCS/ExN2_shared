using Microsoft.Win32;
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

namespace ExN2 {
    /// <summary>
    /// Interaction logic for Convert_OldLoader.xaml
    /// </summary>
    public partial class Dlg_ConvertOldLoader : Window {

//        public CfgLoaderConfig loadersList; 

        public Dlg_ConvertOldLoader() {
            InitializeComponent();
        }

        private void btn_Convert_Click(object sender, RoutedEventArgs e) {
            /*CfgEventLoader Ldr = new CfgEventLoader();
          
            var CLC =  Ldr.LoadFromOldIni(textBox_SrcFile.Text);
            loadersList = CLC;
            Ldr.SaveToXml(textBox_DstFile.Text, CLC);  */
        }

        private void btn_SelSrcFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Text INI files (*.ini)|*.ini|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Base.sPathAppRoot;
            if (openFileDialog.ShowDialog() == true) {
                textBox_SrcFile.Text = openFileDialog.FileName;
                textBox_Log.Text += openFileDialog.FileName + " ... file selected as SOURCE\n";
            }
        }

        private void btn_SelDstFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = false;     // new name entered
            openFileDialog.Filter = "Text XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Base.sPathAppRoot;
            if (openFileDialog.ShowDialog() == true) {
                textBox_DstFile.Text = openFileDialog.FileName;
                textBox_Log.Text += openFileDialog.FileName + " ... file selected as DESTINATION\n";
            }
        }
    }
}
