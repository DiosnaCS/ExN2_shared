using System;
using System.Collections.Generic;
using ExN2.Common;



namespace ExN2.Datablock {

    static class ControlWebUtils {


        //...............................................................................................
        // creates ControlWeb part "schedule"
        public static OpResult CW_Create_SchedulePart(string aFileName) {
            OpResult res = new OpResult();
           /* System.IO.StreamWriter file;

            string sDrvName = "drv_XXX";
            int iChanBase = 1990000;

            try {
                file = new System.IO.StreamWriter(aFileName);
                file.WriteLine("schedule sch_vizd {period = 10; period_offset = 0; period_origin = midnight};");

                foreach (DbVisuItem u in Items) {
                    string sRes;
                    OpResult res1 = CW_GetDataFormula(u, sDrvName, iChanBase, out sRes);
                    if (res1.bOK) {
                        file.WriteLine("    " + u.sVarName + " = " + sRes + ";");
                    }
                    else {
                        res.AddErrMsg("CW_CreateSchedulePart: " + res1.sMsg + ", AT ITEM: " + u.sVarName);
                    }
                }

                file.WriteLine("end_schedule;");
                file.Flush();
                file.Close();
            }
            catch (Exception e) {
                res.AddErrMsg("CW_CreateSchedulePart: General exception: " + e.Message);
            }   */
            return res;
        }

        //...............................................................................................
        static bool CW_GetArchFormula(DbVisuItem aItem, out string aOutStr) {
            aOutStr = "";
            bool bRes = true;

            aOutStr = aItem.sVarName + " := " + aItem.sVarName + ";";
            return bRes;
        }


        //...............................................................................................
        public static OpResult CW_GetDataFormula(DbVisuItem aItem, string aDrvName, int aChanBase, out string aOutStr) {
            OpResult res = new OpResult();
            string sResult = "";
            bool bRes = true;
            bool bAddMulDiv = true;
            int iChNo;

            switch (aItem.Type) {
                case tDbVisuItemType.Byte:
                    if ((aItem.iOffsByte % 2) == 0)     // sudy bajt = High
                        sResult = String.Format("program_INT_TO_LOW_HIGH.KHigh({0}.{1})", aDrvName, aChanBase + aItem.iOffsByte / 2);
                    else
                        sResult = String.Format("program_INT_TO_LOW_HIGH.KLow({0}.{1})", aDrvName, aChanBase + aItem.iOffsByte / 2);
                    break;

                case tDbVisuItemType.USint:
                    if ((aItem.iOffsByte % 2) == 0)     // sudy bajt = High
                        sResult = String.Format("program_INT_TO_LOW_HIGH.KHigh({0}.{1})", aDrvName, aChanBase + aItem.iOffsByte / 2);
                    else
                        sResult = String.Format("program_INT_TO_LOW_HIGH.KLow({0}.{1})", aDrvName, aChanBase + aItem.iOffsByte / 2);
                    break;

                case tDbVisuItemType.Int:
                    sResult = String.Format("{0}.{1}", aDrvName, aChanBase + aItem.iOffsByte / 2);
                    break;

                case tDbVisuItemType.Dint:
                    iChNo = aChanBase + aItem.iOffsByte / 2;
                    if (aItem.rScaling == 1.0) {
                        // special cases - dedicated conversion functions
                        if (aItem.iDivisor == 1000) {
                            sResult = String.Format("program_Low_High_DINT.Solve_3dp({0}.{1}, {0}.{2})", aDrvName, iChNo + 1, iChNo);
                            bAddMulDiv = false;
                            break;
                        }
                        if (aItem.iDivisor == 60) {
                            sResult = String.Format("program_Low_High_DINT.Solve_Minutes({0}.{1}, {0}.{2})", aDrvName, iChNo + 1, iChNo);
                            bAddMulDiv = false;
                            break;
                        }
                        if (aItem.iDivisor == 3600) {
                            sResult = String.Format("program_Low_High_DINT.Solve_Hours({0}.{1}, {0}.{2})", aDrvName, iChNo + 1, iChNo);
                            bAddMulDiv = false;
                            break;
                        }
                    }
                    sResult = String.Format("program_Low_High_DINT.Solve({0}.{1}, {0}.{2}, {3})", aDrvName, iChNo + 1, iChNo, aItem.rScaling / aItem.iDivisor);
                    bAddMulDiv = false;
                    break;

                case tDbVisuItemType.String:
                    iChNo = aChanBase + aItem.iOffsByte / 2;
                    /*if ((aItem.iOptLen != 40)) {
                        bRes = false;
                        aOutStr = "Type 'String' has unsupported length " + aItem.iOptLen;
                        break;
                    }*/
                    string S = String.Format("MakeString{0}(", aItem.iOptLen);
                    for (int i = 0; i < aItem.iOptLen / 2 + 1; i++) {
                        if (i > 0)
                            S += ",";
                        S += aDrvName + "." + (iChNo + i);
                    }
                    S += ")";
                    sResult = S;
                    break;

                case tDbVisuItemType.Bool:
                    if ((aItem.iOffsByte % 2) == 0)     // sudy bajt = High
                        sResult = String.Format("bitget({0}.{1}, {2})", aDrvName, aChanBase + aItem.iOffsByte / 2, aItem.iOffsBit + 8);
                    else
                        sResult = String.Format("bitget({0}.{1}, {2})", aDrvName, aChanBase + aItem.iOffsByte / 2, aItem.iOffsBit + 0);
                    break;


                case tDbVisuItemType.UInt:
                    sResult = "not implemented";
                    res.AddErrMsg("UInt not implemented");
                    break;

                case tDbVisuItemType.UDint:
                    sResult = "not implemented";
                    res.AddErrMsg("UDint not implemented");
                    break;

                case tDbVisuItemType.Real:
                    sResult = "not implemented";
                    res.AddErrMsg("Real not implemented");
                    break;

                case tDbVisuItemType.Long:
                    sResult = "not implemented";
                    res.AddErrMsg("Long not implemented");
                    break;

            }

            // optionally add the multiplier / divisor
            string sMulDiv = "";
            if (bAddMulDiv) {
                if (aItem.rScaling != 1)
                    sMulDiv = "*" + aItem.rScaling.ToString().Replace(',', '.');
                if (aItem.iDivisor != 1)
                    sMulDiv = "/" + aItem.iDivisor.ToString();
            }

            aOutStr = sResult + sMulDiv;
            return res;
        }

        //...............................................................................................
        // creates ControlWeb part "archive"
        public static OpResult CW_Create_ArchivePart(string aFileName) {
            OpResult res = new OpResult();
           /* System.IO.StreamWriter file;
            bool bOK = true;

            try {
                List<ArchListItem> tableList = CW_GetArchList();

                file = new System.IO.StreamWriter(aFileName);

                foreach (ArchListItem tableItem in tableList) {
                    string sTableName = tableItem.sTableName;
                    int iPeriod = tableItem.iPeriod;
                    file.WriteLine("archive " + sTableName + " {period = " + iPeriod + "; period_offset = 0; period_origin = midnight};");

                    foreach (DbVisuItem u in Items) {
                        if (!u.MakeTableName().Equals(sTableName))
                            continue;

                        if (u.bArchive)
                            file.WriteLine("    " + u.sVarName + " = " + "sch_vizd" + "." + u.sVarName + ";");
                        else {
                            //res.AddErrMsg("CW_Create_ArchivePart: general error");
                        }
                    }

                    file.WriteLine("end_archive;");
                    file.WriteLine();
                }

                file.Flush();
                file.Close();
            }
            catch (Exception e) {
                res.AddErrMsg("CW_Create_ArchivePart, General exception: " + e.Message);
            }     */
            return res;
        }



    }
}
