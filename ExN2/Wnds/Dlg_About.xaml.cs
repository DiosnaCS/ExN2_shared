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
    /// Interaction logic for Dlg_About.xaml
    /// </summary>
    public partial class Dlg_About : Window {
        public Dlg_About() {
            InitializeComponent();
            textBox_version.Text = Base.version;
        }

        private void button_Cancel(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
