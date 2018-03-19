using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExN2.Common {


    //=====================================================================================
    // The class encapsulates the result of some complex user operation:
    //  - the text error message
    //  - the boolean result
    public class OpResult {
        public string sMsg;
        public bool bOK;

        //...............................................................................................
        public OpResult() {
            Init();
        }

        //...............................................................................................
        public void Init() {
            sMsg = "";      // no error message
            bOK = true;     // no errror by default
        }

        //...............................................................................................
        /// <summary> add message and set ERROR </summary>
        public void AddErrMsg(string sNewMsg) {
            sMsg += sNewMsg + "\n";
            bOK = false;
        }

        //...............................................................................................
        /// <summary> add message only </summary>
        public void AddMsg(string sNewMsg) {
            sMsg += sNewMsg + "\n";
        }

        //...............................................................................................
        /// <summary> combines two results together </summary>
        public void Combine(OpResult opRes2) {
            bOK &= opRes2.bOK;      // AND operation
            AddMsg(opRes2.sMsg); // concatenate
        }

        //...............................................................................................
        /// <summary> combines two results together </summary>
        public void CombineIfErr(OpResult opRes2, string sOrigLine) {
            bOK &= opRes2.bOK;      // AND operation
            if (! opRes2.bOK)
                AddMsg(opRes2.sMsg + " AT LINE: " + sOrigLine); // concatenate
        }

        //...............................................................................................
        /// <summary> color for displaying the result </summary>
        public void SetTextbox(TextBox tb) {
            tb.Text = sMsg;
            tb.Background = getBkBrush;
        }

        //...............................................................................................
        /// <summary> color for displaying the result </summary>
        public Brush getBkBrush {
            get {
                if (bOK)
                    return Brushes.AliceBlue;
                else
                    return Brushes.LightSalmon;
            }
        }

    }
}
