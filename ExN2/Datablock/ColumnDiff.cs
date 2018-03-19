using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExN2.Datablock {

    //========= helper class for difference between SQL table and Datablock =====================
    public enum tDiffType {
        OnlyInSql,
        OnlyInDB,
        DiffDataType,
        ReservedItem    // reserved item - do not change it
    }

    /// <summary>helper class: for showing the differences between dtablock and SQL table</summary>
    public class ColumnDiff {
        public tDiffType diffType;

        // info about SQL column
        public string sSqlColName = "";
        public string sSqlColDispType = "";

        // info about PLC item
        public string sPlcVarName = "";
        public tDbVisuItemType PlcType;
        public int iPlcOptLen = 0;


        public int iOrderNo { get; set; }         // only for display


        public void SetSqlOnly(string aSqlColName, string aSqlColDispType, int aOrderNo) {    // helper for "only in SQL"
            diffType = tDiffType.OnlyInSql;
            sSqlColName = aSqlColName;
            sSqlColDispType = aSqlColDispType;
            iOrderNo = aOrderNo;
        }

        public void SetDBOnly(string aPlcVarName, tDbVisuItemType aType, int aPlcOptLen, int aOrderNo) {           // helper for "only in Datablock"
            diffType = tDiffType.OnlyInDB;
            sPlcVarName = aPlcVarName;
            PlcType = aType;
            iPlcOptLen = aPlcOptLen;
            iOrderNo = aOrderNo;
        }

        public string get_SqlTypeNameForCreate() {
            string S = Def.get_TypeNameSql(PlcType);
            if (PlcType == tDbVisuItemType.String) {
                return S + "(" + iPlcOptLen + ")";
            }
            else
                return S;
        }

        public string view_PlcItem {
            get {
                if (sPlcVarName != "")
                    return sPlcVarName + " : " + Def.get_TypeNamePlc(PlcType);
                else
                    return "";
            }
        }

        public string view_SQLitem {
            get {
                if (sSqlColName != "")
                    return sSqlColName + " : " + sSqlColDispType;
                else
                    return "";
            }
        }

        public string view_DiffType {
            get {
                try {
                    return Def.diffNames[(int)diffType];
                }
                catch (Exception) {
                    return "???";
                }
            }
        }
    }

}
