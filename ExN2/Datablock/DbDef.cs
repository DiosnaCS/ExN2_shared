using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExN2.Datablock {

    public enum tDbVisuItemType {   // datablock Visualisation - Item types in TIA portal
        Bool,
        Byte,
        Int,
        Dint,
        USint,
        UInt,
        UDint,
        Real,
        Long,   // LongReal
        String,
        Struct,
        NONE = -1
    }

    public enum tRoundAddr {   // rounding type of datablock offset
        None = 0,       // ordinal values are used for finding the stronger rounding
        Byte1 = 1,
        Byte2 = 2,
        Byte4 = 3
    }

    public enum tPostgresDataTypeOID {
        smallint = 21,
        integer = 23,
        real = 700,
        doubleprecision = 701,
        charvar = 1043
    }

 
    //=====================================================================================
    // auxiliary class: for type definition table below
    class DbItemDef {
        public tDbVisuItemType Type;
        public int iByteSize;
        public int iBitSize;
        public tRoundAddr Rounding;
        public string sTypeNamePlc;     // type names used in export file from TIA portal
        public string sTypeNameSQL;     // corresponding type in SQL as string
        public uint uiTypeOID_SQL;      // corresponding type in SQL as OID number
    }

    //=====================================================================================
    // definition table: properties of each TIA portal type
    static class Def {
        public static DbItemDef[] itemDefs = new DbItemDef[] {
            new DbItemDef() { Type = tDbVisuItemType.Bool,     iByteSize = 0,   iBitSize = 1,   Rounding = tRoundAddr.None,    sTypeNamePlc="Bool",     sTypeNameSQL="smallint",           uiTypeOID_SQL = 21,    },
            new DbItemDef() { Type = tDbVisuItemType.Byte,     iByteSize = 1,   iBitSize = 0,   Rounding = tRoundAddr.Byte1,   sTypeNamePlc="Byte",     sTypeNameSQL="smallint",           uiTypeOID_SQL = 21,    },
            new DbItemDef() { Type = tDbVisuItemType.Int,      iByteSize = 2,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="Int",      sTypeNameSQL="integer",            uiTypeOID_SQL = 23,    },
            new DbItemDef() { Type = tDbVisuItemType.Dint,     iByteSize = 4,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="DInt",     sTypeNameSQL="integer",            uiTypeOID_SQL = 23,    },
            new DbItemDef() { Type = tDbVisuItemType.USint,    iByteSize = 1,   iBitSize = 0,   Rounding = tRoundAddr.Byte1,   sTypeNamePlc="USInt",    sTypeNameSQL="integer",            uiTypeOID_SQL = 23,    },
            new DbItemDef() { Type = tDbVisuItemType.UInt,     iByteSize = 2,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="UInt",     sTypeNameSQL="integer",            uiTypeOID_SQL = 23,    },
            new DbItemDef() { Type = tDbVisuItemType.UDint,    iByteSize = 4,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="UDInt",    sTypeNameSQL="integer",            uiTypeOID_SQL = 23,    },
            new DbItemDef() { Type = tDbVisuItemType.Real,     iByteSize = 4,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="Real",     sTypeNameSQL="Real",               uiTypeOID_SQL = 700,   },
            new DbItemDef() { Type = tDbVisuItemType.Long,     iByteSize = 8,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="LReal",    sTypeNameSQL="double precision",   uiTypeOID_SQL = 701,   },
            new DbItemDef() { Type = tDbVisuItemType.String,   iByteSize = 2,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="String",   sTypeNameSQL="character varying",  uiTypeOID_SQL = 1043,  }, // delka v SQL: character varying[40] ma 44
            new DbItemDef() { Type = tDbVisuItemType.Struct,   iByteSize = 0,   iBitSize = 0,   Rounding = tRoundAddr.Byte2,   sTypeNamePlc="Struct",   sTypeNameSQL="Struct",             uiTypeOID_SQL = 0,     }
        };

        public static string[] diffNames = new string[] {
            "SQL only",             // OnlyInSql
            "PLC only",             // OnlyInDB
            "type diff.",           // DiffDataType
            "Reserved"              // ReservedItem  
        };

        public static string get_TypeNameSql(tDbVisuItemType itemType) {
            return itemDefs[(int)itemType].sTypeNameSQL;
        }

        public static string get_TypeNamePlc(tDbVisuItemType itemType) {
            return itemDefs[(int)itemType].sTypeNamePlc;
        }
    }


    //========= helper class for storing the one SQL table column info =====================
    class SQLColumnInfo {
        public string colName;
        public uint colTypeOID;

        public SQLColumnInfo(string aColName, uint aColTypeOID) {
            colName = aColName;
            colTypeOID = aColTypeOID;
        }

        public string GetSqlTypeName() {   // return the type name as string
            try {
                return Def.itemDefs.First(p => (p.uiTypeOID_SQL == colTypeOID)).sTypeNameSQL;
            }
            catch (Exception) {
                return "typeName not found";
            }
        }
    }


    //...............................................................................................
    class ArchListItem {  // pomocna trida pro predani seznamu archivovanych tabulek
        public string sTableName;
        public int iPeriod;
    }


}
