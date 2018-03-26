using ExN2.Common;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Npgsql;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using System.Globalization;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;

namespace ExN2.Datablock {

    //=====================================================================================
    // all properties of one item in datablock
    public class DbVisuItem {
        //[XmlAttribute]
        public String sVarName { get; set; }        // item name in datablock
        //public String sColName { get; set; }        // item name in SQL table (equal to VarName by defaut)
        public tDbVisuItemType Type { get; set; }   // Siemens data type
        public int iOptLen { get; set; }            // optional length (declared): valid for String, physical length is declaredLen + 2 Bytes
        public String sComment { get; set; }        // comment from TIA up to # sign
        public String sCommentFlags { get; set; }   // comment from TIA, starting from # sign, which means the formatting commands
        public bool bArchive { get; set; }          // will be archived
        public int iOffsByte { get; set; }          // byte offset 
        public int iOffsBit { get; set; }           // bit offset

        public int iPeriod { get; set; }            // archivation period [sec] - valid only for archived items
        //public string sTable { get; set; }          // SQL table name
        public string sUnit { get; set; }           // SQL table name
        public double rScaling { get; set; }        // multiplier
        public int iDivisor { get; set; }           // divisor
        public string sTextlist { get; set; }       // textlist associated

        //[XmlIgnore]
        public long Value_Int;
        public double Value_Real;
        public string Value_String = "";

        // internals
        public tRoundAddr ForceRounding = tRoundAddr.None; // forced rounding at the strucure start

        //...............................................................................................
        public DbVisuItem() {   // constructor
            rScaling = 1.0;
            iDivisor = 1;
            sComment = "";
            sCommentFlags = "";
            bArchive = false;
            iPeriod = 20;
        }

        //...............................................................................................
        public void TakeDefaultsFrom(DbVisuItem aTemplate) {
            rScaling = aTemplate.rScaling;
            iDivisor = aTemplate.iDivisor;
            bArchive = aTemplate.bArchive;
            iPeriod = aTemplate.iPeriod;
            sUnit = aTemplate.sUnit;
        }

        //...............................................................................................
        public string View_sTableName {     // helper for ListView
            get {
                if (bArchive)
                    return MakeTableName();
                else
                    return "- - -";
            }
        }

        //...............................................................................................
        public string View_sArchiveImageUri {     // helper for ListView
            get {
                if (bArchive)
                    return "pack://application:,,,/resources/" + "bullet1.png";
                else
                    return null;  //"pack://application:,,,/resources/" + "bullet0.png";
            }
        }

        //...............................................................................................
        public string View_sOffs {     // helper for ListView
            get { return iOffsByte + "." + iOffsBit; }
        }

        //...............................................................................................
        public String View_Type {     // helper for ListView
            get {
                if (iOptLen == 0)
                    return Type.ToString();
                else
                    return Type.ToString() + "[" + iOptLen + "]";
            }
        }

        //...............................................................................................
        public string MakeTableName() {
            switch (iPeriod) {                  // some special periods have reserved names
                case 20: return "ar_norm";
                case 60: return "ar_slow";
                case 300: return "ar_xslow";
            }
            return "ar_" + iPeriod.ToString("0#");      // generic name
        }


        //...............................................................................................
        public OpResult ParseTextCmds() {
            // decode the format specifiers stated in comment and set the according attributes
            OpResult opRes = new OpResult();

            string[] cmd = sCommentFlags.Split(';');
            if (cmd.Length == 0)
                return opRes;

            // search for particular type specifiers
            int iFirstCmd = 1;
            switch (cmd[0]) {
                case "m":               // mass, 1 gramm
                    iDivisor = 1000;
                    sUnit = "kg";
                    bArchive = true;
                    break;
                case "t":               // temeperature, 0.1 °C
                    iDivisor = 10;
                    sUnit = "°C";
                    bArchive = true;
                    break;
                case "p":               // pressure, 0.01 Bar
                    iDivisor = 100;
                    sUnit = "Bar";
                    bArchive = true;
                    break;
                case "f":               // frequence, 0.1 Hz
                    iDivisor = 10;
                    sUnit = "Hz";
                    bArchive = true;
                    break;
                case "Ts":              // time in seconds
                    rScaling = 1;
                    sUnit = "s";
                    bArchive = true;
                    break;
                case "Tm":              // time in seconds, scaled to minutes
                    iDivisor = 60;
                    sUnit = "min";
                    bArchive = true;
                    break;
                case "Th":              // time in seconds, scaled to hours
                    iDivisor = 3600;
                    sUnit = "hr";
                    bArchive = true;
                    iPeriod = 300;
                    break;
                default:
                    iFirstCmd = 0;      // first item is NOT the type specifier, we have to parse also this item for general command
                    break;
            }// switch

            // search for general formatting command
            for (int i = iFirstCmd; i < cmd.Length; i++) {
                string sCmd = cmd[i].Trim().ToLower();

                if (sCmd.StartsWith("ar")) {       // 'ar' - archivation period specification [sec]
                    int iTmpPeriod;
                    if (sCmd.StartsWith("arnone")) {
                        bArchive = false;
                    }
                    else if (int.TryParse(sCmd.Substring(2), out iTmpPeriod)) {
                        bArchive = true;
                        iPeriod = iTmpPeriod;
                    }
                    else {
                        opRes.AddErrMsg("Number format error in 'ar' formatting command.");
                    }
                    continue;
                }

                if (sCmd.StartsWith("c")) {        // 'c' - scaling Constant
                    double rTmpScale;
                    if (double.TryParse(sCmd.Substring(1), out rTmpScale)) {
                        rScaling = rTmpScale;
                    }
                    else {
                        opRes.AddErrMsg("Number format error in 'c' formatting command.");
                    }
                    continue;
                }

                if (sCmd.StartsWith("d")) {        // 'c' - scaling Constant
                    int iTmpDivisor;
                    if (int.TryParse(sCmd.Substring(1), out iTmpDivisor)) {
                        iDivisor = iTmpDivisor;
                    }
                    else {
                        opRes.AddErrMsg("Number format error in 'd' formatting command.");
                    }
                    continue;
                }

                if (sCmd.StartsWith("u")) {        // 'u' - physical unit
                    sUnit = sCmd.Substring(1);
                    continue;
                }

                if (sCmd.ToLower().StartsWith("tl")) {        // 'tl - textlist
                    sTextlist = sCmd.Substring(3);
                    iDivisor = 1;
                    sUnit = "";
                    bArchive = true;
                    iPeriod = 20;
                    continue;
                }

                // unrecognized command
                if (sCmd != "") {
                    opRes.AddErrMsg("Unrecognized formatting command'" + sCmd + "'. ");
                }
            }// for
            return opRes;
        }

        //...............................................................................................
        public bool AcceptValueFromBuf(byte[] aBuf) {

            // DODELAT kontrolu delky
            /*int hlp = Def.itemDefs[(int)Type].iByteSize;
            if (aBuf.Length < (iOffsByte + hlp)) {
                return false;       // out of buffer, nemelo by nastat
            }*/

            byte[] arr = new byte[8];

            switch (Type) {
                case tDbVisuItemType.Bool:
                    arr[0] = aBuf[iOffsByte];
                    //System.Collections.BitArray bits = new System.Collections.BitArray(arr[0]);
                    Value_Int = ((arr[0] & (1 << iOffsBit)) != 0) ? 1 : 0;
                    break;

                case tDbVisuItemType.Byte:
                    Value_Int = aBuf[iOffsByte];
                    break;

                case tDbVisuItemType.Int:
                    for (int i = 0; i < 2; i++)
                        arr[1 - i] = aBuf[iOffsByte + i];  // copy + reverse
                    Value_Int = BitConverter.ToInt16(arr, 0);
                    break;

                case tDbVisuItemType.Dint:
                    for (int i = 0; i < 4; i++)
                        arr[3 - i] = aBuf[iOffsByte + i];  // copy + reverse
                    Value_Int = BitConverter.ToInt32(arr, 0);
                    break;

                case tDbVisuItemType.USint:
                    Value_Int = aBuf[iOffsByte];
                    break;

                case tDbVisuItemType.UInt:
                    for (int i = 0; i < 2; i++)
                        arr[1 - i] = aBuf[iOffsByte + i];  // copy + reverse
                    Value_Int = BitConverter.ToUInt16(arr, 0);
                    break;

                case tDbVisuItemType.UDint:
                    for (int i = 0; i < 4; i++)
                        arr[3 - i] = aBuf[iOffsByte + i];  // copy + reverse
                    Value_Int = BitConverter.ToUInt32(arr, 0);
                    break;

                case tDbVisuItemType.Real:
                    for (int i = 0; i < 4; i++)
                        arr[3 - i] = aBuf[iOffsByte + i];  // copy + reverse
                    Value_Real = BitConverter.ToSingle(arr, 0);
                    break;

                case tDbVisuItemType.Long:
                    for (int i = 0; i < 8; i++)
                        arr[7 - i] = aBuf[iOffsByte + i];  // copy + reverse
                    Value_Real = BitConverter.ToDouble(arr, 0);
                    break;

                case tDbVisuItemType.String:
                    byte[] sArr = new byte[iOptLen];
                    for (int i = 2; i < iOptLen; i++)
                        sArr[i - 2] = aBuf[iOffsByte + i];  // copy + reverse
                    //Value_String = BitConverter.ToString(sArr, 0, iOptLen);
                    Value_String = System.Text.Encoding.Default.GetString(sArr, 0, iOptLen);
                    //Value_String.Replace('\0', ' ');
                    Value_String = Value_String.TrimEnd((char)0);
                    break;

                default:
                    return false;
            }

            return true;
        }

        //...............................................................................................
        public string ValueToStr() {
            // value conversion for SQL query construction
            switch (Type) {
                case tDbVisuItemType.Bool:
                    return Value_Int.ToString();

                case tDbVisuItemType.Byte:
                    return Value_Int.ToString();

                case tDbVisuItemType.Int:
                    return Value_Int.ToString();

                case tDbVisuItemType.Dint:
                    return Value_Int.ToString();

                case tDbVisuItemType.USint:
                    return Value_Int.ToString();

                case tDbVisuItemType.UInt:
                    return Value_Int.ToString();

                case tDbVisuItemType.UDint:
                    return Value_Int.ToString();

                case tDbVisuItemType.Real:
                    return Value_Real.ToString();

                case tDbVisuItemType.Long:
                    return Value_Real.ToString();

                case tDbVisuItemType.String:
                    return "'" + Value_String + "'";

                default:
                    Debug.Assert(true, "unknown type");
                    return "0"; // error
            }
            // unreachable point here
        }
    }

    //=====================================================================================
    // helper class only for Serialization
    public class DbVizu_Serial {
        public List<DbVisuItem> Items;
        public List<ArchListItem> Sections;
    }


    //=====================================================================================
    // all information about PLC visualization datablock "db_Visu"
    public class DbVisu {
        const string sFILE_NAME = @"\dbVisu.xml";
        public int iTaskNo;
        string sFullNameXML;

        public List<DbVisuItem> Items = new List<DbVisuItem>();
        public List<ArchListItem> Sections = new List<ArchListItem>();

        TaskComProps comProps;  // reference to common props

        //...............................................................................................
        public DbVisu(int aTaskNo, TaskComProps aComProps) {
            iTaskNo = aTaskNo;
            comProps = aComProps;
            string sCfgSubdirName = Base.taskInfo[iTaskNo].sTaskSubdir;
            sFullNameXML = Base.sPathAppRoot + Base.sConfigSubdir + sCfgSubdirName + sFILE_NAME;
        }

        //...............................................................................................
        public void Log(int aDesiredLevel, string aMsg) {
            Base.Log_TaskLevel(iTaskNo, aDesiredLevel, aMsg);
        }

        //...............................................................................................
        public bool DoInit() {
            OpResult res = new OpResult();

            res = LoadFromXML();
            if (!res.bOK) {
                Log(0, res.sMsg);
                return false;
            }
            return true;
        }

        //...............................................................................................
        tDbVisuItemType DecodeTypeName(string aSearchName) {
            try {
                return Def.itemDefs.First(p => (p.sTypeNamePlc == aSearchName)).Type;
            }
            catch (Exception) {
                return tDbVisuItemType.NONE;
            }
        }

        //...............................................................................................
        string MakeSqlTypeName(tDbVisuItemType aPlcType, int aOptLen) {
            if (aPlcType == tDbVisuItemType.NONE)
                return "";
            string S = Def.itemDefs[(int)aPlcType].sTypeNameSQL;
            if (aPlcType == tDbVisuItemType.String)
                S += "(" + aOptLen + ")";
            return S;
        }

        //...............................................................................................
        // load the datablock export file (created by TIA command: copy to txt)
        public OpResult LoadFromIni() {
            System.IO.StreamReader file = null;
            String line;
            String[] splitted;
            bool lexicalBlockIsOpen = false;
            int iNesting = 0;
            string sLevel2Prefix = "";
            string sLevel3Prefix = "";  // only 2 levels of nesting is allowed 
            OpResult opRes = new OpResult();
            DbVisuItem Level2_Defaults = new DbVisuItem();
            DbVisuItem Level3_Defaults = new DbVisuItem();
            bool bDelayedRounding = false;

            /* ---- file format example -----------
            DATA_BLOCK "dbVizu"
            { S7_Optimized_Access := 'FALSE' }
            VERSION : 0.1
            NON_RETAIN
               STRUCT 
                  iSecond : Int;   // #arNone
                  iEnd : Int;   // #arNone
                  Part1 : Struct
                     Subpart0 : Int;
                  END_STRUCT;
               END_STRUCT;
            BEGIN
            END_DATA_BLOCK
             - - - - - - - - - - - - -  */

            // select import file - start in proper subdirectory
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Import TIA portal file: 'copy as text'";
            dlg.Filter = "TXT files|*.txt|all files|*.*";
            dlg.InitialDirectory = Base.sPathAppRoot + Base.sConfigSubdir + Base.taskInfo[iTaskNo].sTaskSubdir;
            if (dlg.ShowDialog() != true) {
                opRes.AddErrMsg("Import cancelled");
                return opRes;
            }

            RemoveAllItems();   // clear the old list

            // determine current culture to properly read the comment part
            CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Encoding enc = Encoding.GetEncoding(culture.TextInfo.ANSICodePage);

            try {
                file = new System.IO.StreamReader(dlg.FileName, enc);
                while ((line = file.ReadLine()) != null) {
                    // find the first word = keyword
                    splitted = line.Trim().Split(new char[] { ' ', ';' });
                    if (splitted == null)
                        continue;

                    // catch only the first STRUCT keyword here = real start of parsing
                    if (splitted[0].ToLower().Trim().Equals("struct")) {
                        lexicalBlockIsOpen = true;
                        iNesting++;
                        continue;
                    }

                    // catch all "end_struct" clauses here: closing the subpart and closing of file too
                    if (splitted[0].ToLower().Trim().Equals("end_struct")) {
                        iNesting--;
                        if (iNesting == 0)
                            lexicalBlockIsOpen = false;
                        continue;
                    }

                    if (!lexicalBlockIsOpen)
                        continue;

                    // create new item and fill it with values from current line subsequently
                    DbVisuItem NewItem = new DbVisuItem();

                    //line example:      sSF1_RCP_Name : String[40];   // jmeno RCP
                    splitted = line.Split(new String[] { @"//" }, StringSplitOptions.None);        // separate the comment
                    string sBody = splitted[0].Trim(); ;
                    if (splitted.Length >= 2) {
                        string[] SplitComment = splitted[1].Split('#');         // separate the directive part of comment
                        NewItem.sComment = SplitComment[0];
                        if (SplitComment.Length >= 2)
                            NewItem.sCommentFlags = SplitComment[1];
                    }

                    // isolate var name and type
                    splitted = sBody.Split(new char[] { ':', ';', '[' });        // VariableName : TypeName[20]; 
                    if (splitted.Length < 2)
                        continue;

                    // decode the data type
                    tDbVisuItemType ItemType = DecodeTypeName(splitted[1].Trim());
                    if (ItemType == tDbVisuItemType.NONE) {
                        opRes.AddErrMsg("Type error at line: " + line);
                        continue;
                    }
                    NewItem.Type = ItemType;

                    // opening of new nested level
                    if (ItemType == tDbVisuItemType.Struct) {       // nested Struct clause
                        bDelayedRounding = true;
                        iNesting++;
                        opRes.CombineIfErr(NewItem.ParseTextCmds(), line);
                        if (iNesting == 2) {
                            sLevel2Prefix = splitted[0].Trim();
                            // decode the format specifiers stated in comment
                            Level2_Defaults = new DbVisuItem();
                            Level2_Defaults.TakeDefaultsFrom(NewItem);
                        }
                        if (iNesting == 3) {
                            sLevel3Prefix = splitted[0].Trim();
                            // decode the format specifiers stated in comment
                            Level3_Defaults = new DbVisuItem();
                            Level3_Defaults.TakeDefaultsFrom(Level2_Defaults);
                            Level3_Defaults.TakeDefaultsFrom(NewItem);
                        }
                        continue;   // next cycle - "struct" clause does not generate the output item
                    }

                    // construct the full var name, including nesting
                    if (iNesting <= 1)
                        NewItem.sVarName = splitted[0].Trim();
                    else if (iNesting == 2)
                        NewItem.sVarName = sLevel2Prefix + "_" + splitted[0].Trim();
                    else if (iNesting == 3)
                        NewItem.sVarName = sLevel2Prefix + "_" + sLevel3Prefix + "_" + splitted[0].Trim();

                    // string length is handled separately
                    if (ItemType == tDbVisuItemType.String) {
                        string[] Splitted2 = line.Split(new char[] { '[', ']' });    // isolate the length from type name
                        if (Splitted2.Length < 2) {
                            opRes.AddErrMsg("Format error at line: " + line);
                            continue;
                        }
                        NewItem.iOptLen = Int16.Parse(Splitted2[1]);
                    }

                    // for nested structure use presets placed at header as defaults
                    if (iNesting >= 2)
                        NewItem.TakeDefaultsFrom(Level2_Defaults);
                    if (iNesting >= 3)
                        NewItem.TakeDefaultsFrom(Level3_Defaults);

                    // decode the format specifiers stated in comment
                    opRes.CombineIfErr(NewItem.ParseTextCmds(), line);

                    if (bDelayedRounding) {     // first item of each sub-structure is stronger rounded
                        NewItem.ForceRounding = tRoundAddr.Byte2;
                        bDelayedRounding = false;
                    }

                    Items.Add(NewItem);     // append the new item to list
                }
            }
            catch (System.IO.FileNotFoundException e) {
                opRes.AddErrMsg("Input file not found: " + dlg.FileName);
            }
            catch (Exception e) {
                opRes.AddErrMsg("General exception: " + e.ToString());
            }
            finally {
                if (file != null)
                    file.Close();
            }

            // calculate the physical addresses
            CalcOffsets();

            Sections.AddRange(GetArchList());

            if (opRes.bOK)
                opRes.AddMsg("INI file loaded OK:  " + dlg.FileName);

            return opRes;
        }

        //...............................................................................................
        public bool AcceptDataBuf(byte[] aBuf) {
            // binary data from communication are converted into values
            foreach (DbVisuItem u in Items) {
                u.AcceptValueFromBuf(aBuf);
            }
            return true;
        }

        //...............................................................................................
        public bool Edit(Window aWnd) {
            Dlg_DblockEdit Dlg = new Dlg_DblockEdit(this);
            Dlg.Owner = aWnd;
            Dlg.ShowDialog();
            return false;
        }

        //...............................................................................................
        public OpResult SaveToXML() {
            var res = new OpResult();

            DbVizu_Serial serial = new DbVizu_Serial();
            serial.Items = Items;
            serial.Sections = Sections;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(DbVizu_Serial));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (var sww = new System.IO.StreamWriter(sFullNameXML)) {
                using (XmlWriter writer = XmlWriter.Create(sww, settings)) {
                    xsSubmit.Serialize(writer, serial);
                }
                res.AddMsg("XML file saved OK:  " + sFullNameXML);
            }
            return res;
        }

        //...............................................................................................
        public OpResult LoadFromXML() {
            var res = new OpResult();

            Items.RemoveAll(p => true);
            Sections.RemoveAll(p => true);
            XmlSerializer xsSubmit = new XmlSerializer(typeof(DbVizu_Serial));

            try {
                using (var sww = new System.IO.StreamReader(sFullNameXML)) {
                    using (XmlReader reader = XmlReader.Create(sww)) {
                        DbVizu_Serial serial = (DbVizu_Serial)(xsSubmit.Deserialize(reader));
                        Items.AddRange(serial.Items);
                        Sections.AddRange(serial.Sections);
                    }
                    res.AddMsg("XML file loaded OK:  " + sFullNameXML);
                }
            }
            catch (Exception e) {
                res.AddErrMsg(e.Message);
            }

            return res;
        }

        //...............................................................................................
        public void RemoveAllItems() {
            Items.RemoveAll(p => true);
            Sections.RemoveAll(p => true);
        }


        //...............................................................................................
        public OpResult MakeSqlCmd_CreateTable(ArchListItem tableSpec) {
            OpResult res = new OpResult();
            string sCols = "";

            // example:
            //    CREATE TABLE tabul1 ("Pktime" integer, "iValue1" integer, "sName" character varying(40), CONSTRAINT pk1 PRIMARY KEY ("Pktime") )

            sCols = "\"pktime\" integer";
            foreach (DbVisuItem u in Items) {
                if (!u.bArchive)
                    continue;
                if (!tableSpec.sTableName.Equals(u.MakeTableName()))     // not our table
                    continue;
                sCols += ",\"" + u.sVarName + "\" " + MakeSqlTypeName(u.Type, u.iOptLen);
            }

            res.AddMsg(String.Format("CREATE TABLE {0} ({1},CONSTRAINT {0}_pkey PRIMARY KEY (\"pktime\"))", tableSpec.sTableName, sCols));
            return res;
        }

        //...............................................................................................
        public string MakeSqlCmd_Insert(ArchListItem tableSpec, int aPktime, bool aRealData = true ) {
            string sCols = "";
            string sVals = "";

            // example:
            //    INSERT INTO tabul1 ( "pktime", "iWMU_Ticks", "diWMU_Dose", "sSF1_RCP_Name" ) VALUES (123456789, 1, 111, 'ahoj' )

            sCols = "\"pktime\"";
            if (aRealData) {
                sVals = aPktime.ToString();
            }
            else
                sVals = "123456";

            foreach (DbVisuItem u in Items) {
                if (!u.bArchive)
                    continue;
                if (!tableSpec.sTableName.Equals(u.MakeTableName()))     // not our table
                    continue;
                sCols += ",";
                sVals += ",";
                sCols += "\"" + u.sVarName + "\"";
                if (aRealData)
                    sVals += u.ValueToStr();
                else if (u.Type != tDbVisuItemType.String)
                    sVals += 123;
                else
                    sVals += "'" + 12345 + "'";
            }

            string S = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableSpec.sTableName, sCols, sVals);
            return S;
        }

        // modifikace struktury tabulky:
        //   ALTER TABLE tabul2 ADD COLUMN "iValue2" integer
        //   ALTER TABLE tabul2 DROP COLUMN "iValue2"

        //...............................................................................................
        void CalcOffsets() {
            DatablockAddr Adr = new DatablockAddr();

            foreach (DbVisuItem u in Items) {
                Adr.DoAddrRounding(u);
                u.iOffsByte = Adr.iOffsByte;    // store the curent addres into item
                u.iOffsBit = Adr.iOffsBit;
                Adr.IncrementAddr(u.Type, u.iOptLen);
            }
        }


        public List<ArchListItem> GetArchList() {
            // create a list of all used "arch table names"
            List<ArchListItem> list = new List<ArchListItem>();
            foreach (DbVisuItem u in Items) {
                int idx = list.FindIndex(p => u.MakeTableName().Equals(p.sTableName));
                if (idx < 0)
                    list.Add(new ArchListItem { sTableName = u.MakeTableName(), iPeriod = u.iPeriod });
            }
            return list;
        }

        //...............................................................................................
        public OpResult CheckTableStruct(ArchListItem tableSpec, out List<ColumnDiff> aDiffList) {
            OpResult res = new OpResult();
            NpgsqlConnection conn;

            // - - - create the list of fields in a current table - - -
            // at least one record must be present in a table
            List<SQLColumnInfo> colList = new List<SQLColumnInfo>();
            SQLColumnInfo newInfo;
            conn = new NpgsqlConnection(comProps.sSQL_ConnectString);
            try {
                conn.Open();
                string sqlCmd = String.Format("SELECT * FROM {0} LIMIT 1", tableSpec.sTableName);   // select any row
                NpgsqlCommand cmd = new NpgsqlCommand(sqlCmd, conn);
                NpgsqlDataReader r = cmd.ExecuteReader();
                bool bAnyRecord = false;
                while (r.Read()) {
                    bAnyRecord = true;
                    int iFldCnt = r.FieldCount;
                    for (int i = 0; i < iFldCnt; i++) {
                        newInfo = new SQLColumnInfo(
                                        r.GetName(i),
                                        r.GetDataTypeOID(i)
                            );
                        colList.Add(newInfo);
                    }
                }
                r.Close();
                cmd.Dispose();
                if (!bAnyRecord)
                    res.AddErrMsg("CheckTableStruct: table is EMPTY, structure check not possible");
            }
            catch (Exception e) {
                res.AddErrMsg("CheckTableStruct error: " + e.Message);
            }
            conn.Close();

            // - - - Create list of differences - - -
            List<ColumnDiff> diffList = new List<ColumnDiff>();
            int iOrderNo = 0;

            // find the items only in Datablock
            foreach (DbVisuItem u in Items) {
                if (!u.bArchive)   // items marked as "not archived" are not checked
                    continue;
                if (!tableSpec.sTableName.Equals(u.MakeTableName()))   // this is not our table
                    continue;
                if (colList.Exists(p => p.colName.Equals(u.sVarName)))
                    continue;
                ColumnDiff newItem = new ColumnDiff();
                newItem.SetDBOnly(u.sVarName, u.Type, u.iOptLen, ++iOrderNo);
                diffList.Add(newItem);
            }

            // find the items only in SQL table
            foreach (SQLColumnInfo u in colList) {
                if (Items.Exists(p => p.sVarName.Equals(u.colName) & p.bArchive))
                    continue;

                ColumnDiff newItem = new ColumnDiff();
                newItem.SetSqlOnly(u.colName, u.GetSqlTypeName(), ++iOrderNo);
                diffList.Add(newItem);
                // skip the special service items
                if (newItem.sSqlColName.ToLower().Equals("pktime"))
                    newItem.diffType = tDiffType.ReservedItem;
            }

            aDiffList = diffList;   // return the difference list to caller
            return res;
        }

        //...............................................................................................
        public static OpResult MakeSqlCmd_Modify(ArchListItem tableSpec, List<ColumnDiff> aDiffList, out string aDropSqlCmd, out string aAddSqlCmd) {
            OpResult res = new OpResult();
            //NpgsqlConnection conn;

            /* --- PostGreSQL example
             *  ALTER TABLE tabul1
                  DROP COLUMN "1", 
                  DROP COLUMN "2";
  
                ALTER TABLE tabul1
                  ADD COLUMN "1" integer, 
                  ADD COLUMN "2" integer;*/

            aDropSqlCmd = null; // prepare outputs
            aAddSqlCmd = null;

            string sAddCols = "";
            string sDropCols = "";
            bool bDoADD, bDoDROP;
            foreach (ColumnDiff diff in aDiffList) {
                bDoADD = bDoDROP = false;
                switch (diff.diffType) {
                    case tDiffType.OnlyInDB:
                        bDoADD = true;
                        break;
                    case tDiffType.OnlyInSql:
                        bDoDROP = true;
                        break;
                    case tDiffType.DiffDataType:
                        bDoDROP = true;
                        bDoADD = true;
                        break;
                    case tDiffType.ReservedItem:
                        break;      // no operation - item not changed
                }
                if (bDoDROP)
                    sDropCols += "DROP COLUMN \"" + diff.sSqlColName + "\",";
                if (bDoADD)
                    sAddCols += "ADD COLUMN \"" + diff.sPlcVarName + "\" " + diff.get_SqlTypeNameForCreate() + ",";
            }

            // make the result string
            res.bOK = true;
            if ((sDropCols.Length == 0) & (sAddCols.Length == 0))
                res.AddMsg("OK: NO changes necesary");

            if (sDropCols.Length > 0) {
                sDropCols = sDropCols.Remove(sDropCols.Length - 1);   // remove trailning ','
                aDropSqlCmd = "ALTER TABLE " + tableSpec.sTableName + " " + sDropCols + ";";
                res.AddMsg(aDropSqlCmd);
            }

            if (sAddCols.Length > 0) {
                sAddCols = sAddCols.Remove(sAddCols.Length - 1);   // remove trailning ','
                aAddSqlCmd = "ALTER TABLE " + tableSpec.sTableName + " " + sAddCols + ";";
                res.AddMsg(aAddSqlCmd);
            }

            return res;
        }

        // store data related to specification in "aItem" = typically data in one table
        public void DoArchive(int aPktime, ArchListItem aItem) {
            string sSqlCmd;
            Stopwatch clock = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

            sSqlCmd = MakeSqlCmd_Insert(aItem, aPktime, true);

            NpgsqlConnection conn = new NpgsqlConnection(comProps.sSQL_ConnectString);
            try {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sSqlCmd, conn);
                int iRowsAffected = cmd.ExecuteNonQuery();
                cmd.Dispose();
                if (iRowsAffected < 1)
                    Log(0, "DoArchive: no record inserted");
            }
            catch (Exception e) {
                Log(0, "DoArchive exception: " + e.Message);
            }
            conn.Close();

            clock.Stop();
            Log(2, " SQL INSERT, in " + clock.ElapsedMilliseconds + " ms");
        }

        //...............................................................................................
        public OpResult DoQuery(string sQuery) {
            OpResult res = new OpResult();

            //string sTabName = "tabul1";
            NpgsqlConnection conn;
            conn = new NpgsqlConnection(comProps.sSQL_ConnectString);
            try {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn);
                int iRowsAffected = cmd.ExecuteNonQuery();
                res.AddMsg(" SQL command OK");
            }
            catch (Exception ex) {
                res.AddErrMsg(" SQL command error: " + ex.Message);
            }
            conn.Close();
            return res;
        } 

    }

}
