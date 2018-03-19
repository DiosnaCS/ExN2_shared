using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExN2.Loader {
    class LoaderTask {
    }

    public class EventLoader {
        public int iTaskNo;

        public CommonProps props = new CommonProps();
        public List<EventDef> eventList = new List<EventDef>();

        //...........................................................................
        public EventLoader(int aTaskNo) {
            iTaskNo = aTaskNo;
        }

        //...........................................................................
        public bool Edit(Window Parent) {
            Dlg_LoaderProps Dlg = new Dlg_LoaderProps(this);

            Dlg.SetDlgData(props, eventList);
            Dlg.Owner = Parent;
            if (Dlg.ShowDialog() == true) {   // vraci true, pokud bylo stisknuto OK
                Dlg.GetDlgData(props, eventList);
                return true;
            }
            return false;
        }
    }


}
