using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExN2.Loader {

    public enum tN4T_version : int {
        n4t_undef = -1,  // nedovolena hodnota
        n4t_ver001, // 0.01 - to jeste nemelo hlavicku, prvni byl rovnou iCommand
        n4t_ver100, // 1.00 - tohle posilalo v tele zpravy jako prvni polozku i cislo evt-bufferu
        n4t_ver200  // 2.00 - tohle uz neposila cislo evt-bufferu
    };

    public enum tEventItemType : int {
        itInt8,
        itInt16,
        itInt32,
        itVarChar,
        itIntConst,
        N_tEventItemType    // pocet hodnot typu
    };


    
    //======= popis jedne polozky v sablone udalosti =======
    public class EventDataItem {
        public String sName;      // item name = field name in SQL-table
        public tEventItemType Type;       // type definuje offset v datech a zaroven typ pole v SQL-tabulce
        public bool bStore;     // false = neukladat do DB, polozka pouze posouva offset v udalosti
        //public int iLenBytes;  // delka dat v bajtech
        public int iOptLenBytes;  // delka dat v bajtech pro typ string
        public int iConstValue;// pro pseudopole - konstantni hodnota
        public double rCoef;      // nasobitel hodnoty pred ulozenim do DB

        static public int[] ItemTypeLen = new int[] { 1, 2, 4, 10, 0 };     // item length for each type, indexed by tEventItemType

        public EventDataItem() {
        }

        public EventDataItem(String qsName, tEventItemType qType, bool qbNoStore, int qiConstValue, double qrCoef) {
            bStore = !qbNoStore;        // pokud jmeno zacina "dummy", nebude se ukladat do DB
            sName = qsName;
            Type = qType;
            iConstValue = qiConstValue;
            rCoef = qrCoef;
            iOptLenBytes = 0;

            if (Type == tEventItemType.itVarChar) {
                // pro string se delka predava jako parametr Value, byla v INI uvedena v modifikatoru "len="
                // deklarovana delka je cela bajtova delka vcetne hlavicky (2 B)
                iOptLenBytes = qiConstValue;
            }
        }

        public override string ToString() {
            return sName + "Type: " + Type.ToString() + " Store: " + bStore + " bytesLen: " + iOptLenBytes + " Value: " + iConstValue + " Coef:" + rCoef;
        }
    }



    // ======= sablona cele udalosti, je to array polozek =======
    public class EventDef {
        public List<int> assocNums = new List<int>();
        public List<EventDataItem> Items = new List<EventDataItem>();

        public void AddItem(String sName, tEventItemType aType, bool bNoStore, int iConstValue, double rCoef) {       // prida na konec
            Items.Add(new EventDataItem(sName, aType, bNoStore, iConstValue, rCoef));
        }

        public override string ToString() {
            string returnString = "";
            foreach (int EventType in assocNums) {
                returnString += EventType.ToString() + ", ";
            }
            returnString = returnString.Substring(0, returnString.Length - 2);
            return returnString;
        }
    }


    /// <summary> Loader common settings class </summary>
    public class CommonProps {
        public bool bRun;

        // pripojeni k databazi PostgreSQL
        public string DB_ConnectString;
        public string DB_TableName;
        public string DB_SysTableName;

        // parametry komunikace s PLC
        public string SocketLocal;
        public string SocketRemote;

        // pro kontrolu definujem delku datoveho tela udalosti
        public int iEventBodyLenBytes;

        public int iRcvTimeoutMs;
        public bool bIntelOrder;
        public bool bIntelOrderStr;         // intel order definujeme zvlast pro stringy
        public tN4T_version N4T_version;
        public int iTypeFieldByteOffs;     // offset v bajtech, pole pro typ udalosti (pro vyber sablony)
        public int iRecnoFieldByteOffs;    // offset v bajtech, pole pro cislo udalosti (pro kontrolu preteceni)

        // false: normalne ukazuje LastPtr v PLC na posledni platny zaznam
        // true:  ve starsich programech ukazuje LastPtr v PLC na prvni volny zaznam
        public bool bLastPtrIsFreePtr;

        // perioda a offset pro serizovani casu PLC
        public int iAdjustTimePeriod_Sec;
        public int iAdjustTimeOffset_Sec;

    }


    public class HelperLoaderSerialize {
        public CommonProps props;
        public List<EventDef> eventList;
    }

}
