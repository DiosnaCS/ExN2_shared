using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.Net;
using System.Xml;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;

using ExN2.Common;

namespace ExN2.Loader {


    static public class LoaderFileOps {
        const string sFILE_NAME = "loader.xml";


        //...........................................................................
        public static OpResult SaveToXml(int aTaskNo, CommonProps aProps, List<EventDef> aEventList) {
            OpResult res = new OpResult();
            string sCfgSubdirName = Base.taskInfo[aTaskNo].sTaskSubdir;
            string sFullNameXML = Base.sPathAppRoot + Base.sConfigSubdir + sCfgSubdirName + sFILE_NAME;
            HelperLoaderSerialize hlp = new HelperLoaderSerialize();

            XmlSerializer xsSubmit = new XmlSerializer(typeof(HelperLoaderSerialize));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            // copy all into one class
            hlp.props = aProps;
            hlp.eventList = aEventList;

            using (var sww = new StreamWriter(sFullNameXML)) {
                using (XmlWriter writer = XmlWriter.Create(sww, settings)) {
                    xsSubmit.Serialize(writer, hlp);
                }
            }
            return res;
        }

        public static OpResult LoadFromXML(int aTaskNo, out CommonProps aProps, out List<EventDef> aEventList) {
            OpResult res = new OpResult();
            string sCfgSubdirName = Base.taskInfo[aTaskNo].sTaskSubdir;
            string sFullNameXML = Base.sPathAppRoot + Base.sConfigSubdir + sCfgSubdirName + sFILE_NAME;
            HelperLoaderSerialize hlp = null;

            aProps = null;
            aEventList = null;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(HelperLoaderSerialize));
            try {
                using (var sr = new System.IO.StreamReader(sFullNameXML)) {
                    using (XmlReader xr = XmlReader.Create(sr)) {
                        hlp = (HelperLoaderSerialize)(xsSubmit.Deserialize(xr));
                    }
                }
            }
            catch (Exception e) {
                res.AddErrMsg("LoadFromXML error: " + e.Message);
                return res;
            }

            // copy from helper class to real object
            if (hlp != null) {
                aProps = hlp.props;
                aEventList = hlp.eventList;
            }
            else
                res.AddErrMsg("LoadFromXML unspec. error");

            return res;
        }

        //...........................................................................
        // z INI souboru nacte konfiguraci jednoho loaderu do dodane struktury pCfg.
        // Jednotlivy loader je v INI identifikovan cislem (zero based).
        public static OpResult LoadFromOldIni(int aTaskNo, out CommonProps aProps, out List<EventDef> aEventList) {
            OpResult res = new OpResult();
            bool bEventIsOpen = false;
            bool bInsideTaskSection = false;
            EventDef evtCurr = null;

            //N4T_version = tN4T_version.n4t_undef;
            string[] partsVal = null;
            string[] partsLen = null;
            string[] address = null;
            int[] timeAdjust = null;

            aProps = null;
            aEventList = null;

            // select import file - start in proper subdirectory, use OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Import old Loader INI file";
            dlg.Filter = "INI files|*.ini|all files|*.*";
            dlg.InitialDirectory = Base.sPathAppRoot + Base.sConfigSubdir + Base.taskInfo[aTaskNo].sTaskSubdir;
            if (dlg.ShowDialog() != true) {
                res.AddErrMsg("operation canceled");
                return res;
            }

            // create new objects which will be used during loading
            aProps = new CommonProps();
            aEventList = new List<EventDef>();

            // load the file into memory
            List<string> lines = System.IO.File.ReadAllLines(dlg.FileName).Select(p => p.Trim()).ToList();
            foreach (string line in lines) {
                if (line == "") 
                    continue;
                if (line.Contains("[Common]")) {
                    bInsideTaskSection = false;
                    continue;
                    // nothing to parse
                }
                if (line.Contains("[Task")) {
                    bInsideTaskSection = true;
                    continue;
                    // nothing to parse
                }
                if (! bInsideTaskSection)
                    continue;

                // - - - -   here standard items (not event) are parsed   - - - - -
                if (line.Contains("=") && !bEventIsOpen)  {
                    string[] splitedLine = line.Split('=');
                    string keyword = splitedLine[0];

                    if(keyword.Contains("Run"))
                        aProps.bRun = splitedLine[1].Equals("Yes");
                            
                    /*if(keyword.Contains("TaskName"))
                        sTaskName = splitedLine[1];  */
                            
                    if(keyword.Contains("ConnectString"))
                        aProps.DB_ConnectString = line.Substring(line.IndexOf("\"")+1, (line.Length - line.IndexOf("\"")-2));
                            
                    if(keyword.Contains("DB_TableName"))
                        aProps.DB_TableName = splitedLine[1];
                            
                    if(keyword.Contains("SysTableName"))
                        aProps.DB_SysTableName = splitedLine[1];

                    if (keyword.Contains("SocketLocal"))
                    {
                        address = splitedLine[1].Split(':');
                        if (address[0] == "")
                            address[0] = "127.0.0.1";
                        aProps.SocketLocal = address[0] + address[1];
                        //SocketLocal = new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
                    }
                    if (keyword.Contains("SocketRemote"))
                    {
                        address = splitedLine[1].Split(':');
                        if (address[0] == "")
                            address[0] = "127.0.0.1";
                        aProps.SocketRemote = address[0] + address[1];
                        //SocketRemote = new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
                    }
                    if(keyword.Contains("ReceiveTimeoutMs"))
                        aProps.iRcvTimeoutMs = int.Parse(splitedLine[1]);
                            
                    if(keyword.Contains("IntelOrder"))
                        aProps.bIntelOrder = splitedLine[1].Equals("1");
                            
                    if(keyword.Contains("N4T_version"))
                        aProps.N4T_version = (tN4T_version) (int)double.Parse(splitedLine[1]);
                            
                    if(keyword.Contains("LastPtrIsFreePtr"))
                        aProps.bLastPtrIsFreePtr = splitedLine[1].Equals("1");
                            
                    if(keyword.Contains("EventBodyLenBytes"))
                        aProps.iEventBodyLenBytes = int.Parse(splitedLine[1]);
                            
                    if(keyword.Contains("TypeFieldByteOffs"))
                        aProps.iTypeFieldByteOffs = int.Parse(splitedLine[1]);
                            
                    if(keyword.Contains("RecnoFieldByteOffs"))
                        aProps.iRecnoFieldByteOffs = int.Parse(splitedLine[1]);

                    if (keyword.Contains("AdjustPlcTime_Sec")) {
                        timeAdjust = splitedLine[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p.Trim())).ToArray();
                        aProps.iAdjustTimePeriod_Sec = timeAdjust[0];
                        aProps.iAdjustTimeOffset_Sec = timeAdjust[1];
                    }

                    if (keyword.Contains("Event_begin")) {
                        // open the new event, this object will be filled step-by-step
                        evtCurr = new EventDef();

                        // that wil trim the every number and give it into the list (EventTypesList)
                        evtCurr.assocNums = splitedLine[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p.Trim())).ToList();
                    }
                }  // end of standard item pasring

                // - - - -   here Event items are parsed   - - - - - 
                if (bEventIsOpen) {
                    if (line.Contains("Event_end")) {
                        bEventIsOpen = false;
                        aEventList.Add(evtCurr);   // add the currently loaded event into eventlist
                        evtCurr = null;
                        continue;
                    }

                    // here one event item is parsed
                    List<string> splittedEventLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
                    EventDataItem evtLineCurr = new EventDataItem();
                    bool bFirst = true;
                    foreach (string eventAttribute in splittedEventLine)
                    {
                        if (bFirst) {   // first item is always Field name
                            evtLineCurr.sName = eventAttribute;
                            bFirst = false;
                            continue;
                        }
                        //coef i dont know how it should be look like
                        if (eventAttribute.Contains("int8")) {
                            evtLineCurr.Type = tEventItemType.itInt8;
                            continue;
                        }
                        if (eventAttribute.Contains("int16")) {
                            evtLineCurr.Type = tEventItemType.itInt8;
                            continue;
                        }
                        if (eventAttribute.Contains("int32")) {
                            evtLineCurr.Type = tEventItemType.itInt32;
                            continue;
                        }
                        if (eventAttribute.Contains("s7str")) {
                            evtLineCurr.Type = tEventItemType.itVarChar;
                            continue;
                        }
                        if (eventAttribute.Contains("const")) {
                            evtLineCurr.Type = tEventItemType.itIntConst;
                            continue;
                        }
                        if (eventAttribute.Contains("noStore")) {
                            evtLineCurr.bStore = false;
                            continue;
                        }
                        if (eventAttribute.Contains("len=")) {
                            partsLen = eventAttribute.Split('=');
                            evtLineCurr.iConstValue = int.Parse(partsLen[1]);
                            continue;
                        }
                        if (eventAttribute.Contains("value=")) {
                            partsVal = eventAttribute.Split('=');
                            evtLineCurr.iConstValue = int.Parse(partsVal[1]);
                            continue;
                        }
                    }
                    evtCurr.Items.Add(evtLineCurr);   // add parsed event line to event
                }  //event is open
            } // foreach lines

            // use the newly made objects

            return res;
        }  // end of method LoadIni

    }

}


