comment modul: International Delights - primary server modul - srv_InternDelights
end_comment;

directories
  'MainMenu.htm' = '.\web';
  '*.ico' = 'bmp';
  '*.bmp' = 'Bmp';
  '*.jpg' = 'Bmp';
  '*.par' = 'drv';
  '*.dmf' = 'drv';
  '*.lo?' = 'log';
end_directories;

settings
  operation_mode = real_time;
  hard_real_time = true;
  skip_init_outputs = true;
  startup_options
    call_procedures = false;
    activate_receivers = true;
    output_action = set_local;
  end_startup_options;
  independent_procedure_execution = true;
  log_window
    enabled = false;
    ignore_same_message = false;
    log_file_size = 1000;
  end_log_window;
  system_database
    dsn = 'main.dsn';
    compact_mdb_on_start = false;
    start_timeout = 1;
  end_system_database;
  remote_access
    default_scope = shared_remotely;
    default_access_policy = allow;
    client_access = allow, '';
  end_remote_access;
end_settings;

driver
  drv_WS {driver = 'opcdrv.dll'; map_file = 'OPC_USINT_WS.dmf'; parameter_file = 'OPC_USINT_WS.par'};
end_driver;

data

  const localconst;
    ModuleName = 'srv_InternDelights';
    Verze = '001';
    Datum = '18.04.2017';
    sDataPath = 'D:/PsqlData/0/';
    sPostgresNm = 'postgres';
    sPostgresPw = 'Nordit0276';
  end_const;

  var com_Status_USINT {scope = shared_remotely};
    enbArch_WS : boolean {init_value = false};
    iTimeLast_WS : integer;
  end_var;

  var localvars_USINT;
    bInitial : boolean;
    iIndex_WS : shortcard;
  end_var;

  schedule sch_WS_vizd_USINT {period = 10; period_offset = 3; period_origin = midnight};
    iSecond = drv_WS.10000;
    iUserGroup = drv_WS.10001;
    iUserName_0_1 = drv_WS.10003;
    iUserName_2_3 = drv_WS.10004;
    iUserName_4_5 = drv_WS.10005;
    iUserName_6_7 = drv_WS.10006;
    iUserName_8_9 = drv_WS.10007;
    iUserName_10_11 = drv_WS.10008;
    iUserName_12_13 = drv_WS.10009;
    iUserName_14_15 = drv_WS.10010;
    iUserName_16_17 = drv_WS.10011;
    iUserName_18_19 = drv_WS.10012;
    diWMU_Volume = program_Low_High_DINT.Solve_3dp( drv_WS.10014, drv_WS.10013 );
    diMF1_Weight = program_Low_High_DINT.Solve_3dp( drv_WS.10016, drv_WS.10015 );
    diSF1_Weight = program_Low_High_DINT.Solve_3dp( drv_WS.10018, drv_WS.10017 );
    diSF2_Weight = program_Low_High_DINT.Solve_3dp( drv_WS.10020, drv_WS.10019 );
    diDP1_Press = program_Low_High_DINT.Solve_2dp( drv_WS.10022, drv_WS.10021 );
    diCLN1_Press = program_Low_High_DINT.Solve_2dp( drv_WS.10024, drv_WS.10023 );
    diDP2_Press = program_Low_High_DINT.Solve_2dp( drv_WS.10026, drv_WS.10025 );
    diDP3_Press = program_Low_High_DINT.Solve_2dp( drv_WS.10028, drv_WS.10027 );
    iMF1_pH = drv_WS.10029 * 0.01;
    iSF1_pH = drv_WS.10030 * 0.01;
    iSF2_pH = drv_WS.10031 * 0.01;
    iWMU1_Temp = drv_WS.10032 * 0.1;
    iMF1_Temp = drv_WS.10033 * 0.1;
    iSF1_Temp_Bottom = drv_WS.10034 * 0.1;
    iSF1_Temp_Side = drv_WS.10035 * 0.1;
    iSF1_Temp = drv_WS.10036 * 0.1;
    iSF2_Temp_Bottom = drv_WS.10037 * 0.1;
    iSF2_Temp_Side = drv_WS.10038 * 0.1;
    iSF2_Temp = drv_WS.10039 * 0.1;
    iDP1_PumpSpeed_act = drv_WS.10040 * 0.1;
    iDP2_PumpSpeed_act = drv_WS.10041 * 0.1;
    iDP3_PumpSpeed_act = drv_WS.10042 * 0.1;
    iCLN1_Pump2TankSpeed_act = drv_WS.10043 * 0.1;
    iCLN1_Pump2PipeSpeed_act = drv_WS.10044 * 0.1;
    usiFlour_Status = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10045 );
    usiFlour_Step = program_INT_TO_LOW_HIGH.KLow( drv_WS.10045 );
    diFlour_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10047, drv_WS.10046 );
    diFlour_Amount_act = program_Low_High_DINT.Solve_3dp( drv_WS.10049, drv_WS.10048 );
    usiFlour_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10050 );
    usiWater_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10050 );
    usiWater_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10051 );
    diWater_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10053, drv_WS.10052 );
    diWater_Amount_act = program_Low_High_DINT.Solve_3dp( drv_WS.10055, drv_WS.10054 );
    usiWater_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10056 );
    usiClean_Tank_aut_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10056 );
    usiClean_Tank_aut_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10057 );
    usiClean_Tank_aut_Dest = program_INT_TO_LOW_HIGH.KLow( drv_WS.10057 );
    usiClean_Tank_aut_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10058 );
    usiClean_Tank_shower_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10058 );
    usiClean_Tank_shower_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10059 );
    usiClean_Tank_shower_Dest = program_INT_TO_LOW_HIGH.KLow( drv_WS.10059 );
    usiClean_Tank_shower_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10060 );
    usiClean_Tank_man_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10060 );
    usiClean_Tank_man_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10061 );
    usiClean_Tank_man_Dest = program_INT_TO_LOW_HIGH.KLow( drv_WS.10061 );
    usiClean_Tank_man_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10062 );
    usiClean_Pipe1_man_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10062 );
    usiClean_Pipe1_man_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10063 );
    usiClean_Pipe1_man_Owner = program_INT_TO_LOW_HIGH.KLow( drv_WS.10063 );
    usiClean_Pipe2_man_Status = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10064 );
    usiClean_Pipe2_man_Step = program_INT_TO_LOW_HIGH.KLow( drv_WS.10064 );
    usiClean_Pipe2_man_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10065 );
    usiTransferPipe1_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10065 );
    usiTransferPipe1_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10066 );
    usiTransferPipe1_Dest = program_INT_TO_LOW_HIGH.KLow( drv_WS.10066 );
    diTransferPipe1_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10068, drv_WS.10067 );
    diTransferPipe1_Amount_act = program_Low_High_DINT.Solve_3dp( drv_WS.10070, drv_WS.10069 );
    usiTransferPipe1_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10071 );
    usiTransferPipe2_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10071 );
    usiTransferPipe2_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10072 );
    usiTransferPipe2_Src = program_INT_TO_LOW_HIGH.KLow( drv_WS.10072 );
    usiTransferPipe2_Dest = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10073 );
    diTransferPipe2_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10075, drv_WS.10074 );
    diTransferPipe2_Amount_act = program_Low_High_DINT.Solve_3dp( drv_WS.10077, drv_WS.10076 );
    usiTransferPipe2_Owner = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10078 );
    usiOperMF1_Status = program_INT_TO_LOW_HIGH.KLow( drv_WS.10078 );
    usiOperMF1_Step = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10079 );
    iOperMF1_RCP = drv_WS.10080;
    iOperMF1_pH_RCP = drv_WS.10081 * 0.01;
    iOperMF1_pH_Last = drv_WS.10082 * 0.01;
    iOperMF1_TA_RCP = drv_WS.10083;
    iOperMF1_RCP_Name_0_1 = drv_WS.10085;
    iOperMF1_RCP_Name_2_3 = drv_WS.10086;
    iOperMF1_RCP_Name_4_5 = drv_WS.10087;
    iOperMF1_RCP_Name_6_7 = drv_WS.10088;
    iOperMF1_RCP_Name_8_9 = drv_WS.10089;
    iOperMF1_RCP_Name_10_11 = drv_WS.10090;
    iOperMF1_RCP_Name_12_13 = drv_WS.10091;
    iOperMF1_RCP_Name_14_15 = drv_WS.10092;
    iOperMF1_RCP_Name_16_17 = drv_WS.10093;
    iOperMF1_RCP_Name_18_19 = drv_WS.10094;
    iOperMF1_RCP_Name_20_21 = drv_WS.10095;
    iOperMF1_RCP_Name_22_23 = drv_WS.10096;
    iOperMF1_RCP_Name_24_25 = drv_WS.10097;
    iOperMF1_RCP_Name_26_27 = drv_WS.10098;
    iOperMF1_RCP_Name_28_29 = drv_WS.10099;
    iOperMF1_RCP_Name_30_31 = drv_WS.10100;
    iOperMF1_RCP_Name_32_33 = drv_WS.10101;
    iOperMF1_RCP_Name_34_35 = drv_WS.10102;
    iOperMF1_RCP_Name_36_37 = drv_WS.10103;
    iOperMF1_RCP_Name_38_39 = drv_WS.10104;
    diOperMF1_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10106, drv_WS.10105 );
    diOperMF1_RCP_Starter_need = program_Low_High_DINT.Solve_3dp( drv_WS.10108, drv_WS.10107 );
    diOperMF1_RCP_Starter_act = program_Low_High_DINT.Solve_3dp( drv_WS.10110, drv_WS.10109 );
    diOperMF1_RCP_Fill_need = program_Low_High_DINT.Solve_3dp( drv_WS.10112, drv_WS.10111 );
    diOperMF1_RCP_Fill_act = program_Low_High_DINT.Solve_3dp( drv_WS.10114, drv_WS.10113 );
    diOperMF1_RCP_Water_need = program_Low_High_DINT.Solve_3dp( drv_WS.10116, drv_WS.10115 );
    diOperMF1_RCP_Water_act = program_Low_High_DINT.Solve_3dp( drv_WS.10118, drv_WS.10117 );
    diOperMF1_RCP_Flour_need = program_Low_High_DINT.Solve_3dp( drv_WS.10120, drv_WS.10119 );
    diOperMF1_RCP_Flour_act = program_Low_High_DINT.Solve_3dp( drv_WS.10122, drv_WS.10121 );
    diOperMF1_RCP_Ferm_need = program_Low_High_DINT.Solve_Hours( drv_WS.10124, drv_WS.10123 );
    diOperMF1_RCP_Ferm_act = program_Low_High_DINT.Solve_Hours( drv_WS.10126, drv_WS.10125 );
    iOperMF1_RCP_FermTemp = drv_WS.10127 * 0.1;
    iOperMF1_RCP_CoolTemp = drv_WS.10128 * 0.1;
    diOperMF1_RCP_Storage_need = program_Low_High_DINT.Solve_3dp( drv_WS.10130, drv_WS.10129 );
    diOperMF1_RCP_Storage_act = program_Low_High_DINT.Solve_3dp( drv_WS.10132, drv_WS.10131 );
    iOperMF1_RCP_StorageTemp = drv_WS.10133 * 0.1;
    usiOperMF1_Icon_Showering = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10134 );
    usiOperMF1_Icon_Water = program_INT_TO_LOW_HIGH.KLow( drv_WS.10134 );
    usiOperMF1_Icon_Flour = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10135 );
    usiOperMF1_Icon_Agit1 = program_INT_TO_LOW_HIGH.KLow( drv_WS.10135 );
    usiOperMF1_Icon_Starter = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10136 );
    usiOperMF1_Icon_Agit2 = program_INT_TO_LOW_HIGH.KLow( drv_WS.10136 );
    usiOperMF1_Icon_Ferm = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10137 );
    usiOperMF1_Icon_Cool = program_INT_TO_LOW_HIGH.KLow( drv_WS.10137 );
    usiOperMF1_Icon_Using = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10138 );
    usiOperMF1_Icon_Clean = program_INT_TO_LOW_HIGH.KLow( drv_WS.10138 );
    diOperMF1_Age = program_Low_High_DINT.Solve_Hours( drv_WS.10140, drv_WS.10139 );
    diOperMF1_EndUsageTime = program_Low_High_DINT.Solve_Hours( drv_WS.10142, drv_WS.10141 );
    usiOperSF1_Status = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10143 );
    usiOperSF1_Step = program_INT_TO_LOW_HIGH.KLow( drv_WS.10143 );
    iOperSF1_RCP = drv_WS.10144;
    iOperSF1_pH_RCP = drv_WS.10145 * 0.01;
    iOperSF1_pH_Last = drv_WS.10146 * 0.01;
    iOperSF1_TA_RCP = drv_WS.10147;
    iOperSF1_RCP_Name_0_1 = drv_WS.10149;
    iOperSF1_RCP_Name_2_3 = drv_WS.10150;
    iOperSF1_RCP_Name_4_5 = drv_WS.10151;
    iOperSF1_RCP_Name_6_7 = drv_WS.10152;
    iOperSF1_RCP_Name_8_9 = drv_WS.10153;
    iOperSF1_RCP_Name_10_11 = drv_WS.10154;
    iOperSF1_RCP_Name_12_13 = drv_WS.10155;
    iOperSF1_RCP_Name_14_15 = drv_WS.10156;
    iOperSF1_RCP_Name_16_17 = drv_WS.10157;
    iOperSF1_RCP_Name_18_19 = drv_WS.10158;
    iOperSF1_RCP_Name_20_21 = drv_WS.10159;
    iOperSF1_RCP_Name_22_23 = drv_WS.10160;
    iOperSF1_RCP_Name_24_25 = drv_WS.10161;
    iOperSF1_RCP_Name_26_27 = drv_WS.10162;
    iOperSF1_RCP_Name_28_29 = drv_WS.10163;
    iOperSF1_RCP_Name_30_31 = drv_WS.10164;
    iOperSF1_RCP_Name_32_33 = drv_WS.10165;
    iOperSF1_RCP_Name_34_35 = drv_WS.10166;
    iOperSF1_RCP_Name_36_37 = drv_WS.10167;
    iOperSF1_RCP_Name_38_39 = drv_WS.10168;
    diOperSF1_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10170, drv_WS.10169 );
    diOperSF1_RCP_MotherDough_need = program_Low_High_DINT.Solve_3dp( drv_WS.10172, drv_WS.10171 );
    diOperSF1_RCP_MotherDough_act = program_Low_High_DINT.Solve_3dp( drv_WS.10174, drv_WS.10173 );
    diOperSF1_RCP_Fill_need = program_Low_High_DINT.Solve_3dp( drv_WS.10176, drv_WS.10175 );
    diOperSF1_RCP_Fill_act = program_Low_High_DINT.Solve_3dp( drv_WS.10178, drv_WS.10177 );
    diOperSF1_RCP_Water_need = program_Low_High_DINT.Solve_3dp( drv_WS.10180, drv_WS.10179 );
    diOperSF1_RCP_Water_act = program_Low_High_DINT.Solve_3dp( drv_WS.10182, drv_WS.10181 );
    diOperSF1_RCP_Flour_need = program_Low_High_DINT.Solve_3dp( drv_WS.10184, drv_WS.10183 );
    diOperSF1_RCP_Flour_act = program_Low_High_DINT.Solve_3dp( drv_WS.10186, drv_WS.10185 );
    diOperSF1_RCP_Mixture_need = program_Low_High_DINT.Solve_3dp( drv_WS.10188, drv_WS.10187 );
    iOperSF1_RCP_Mixture_TA = drv_WS.10189;
    diOperSF1_RCP_Ferm_need = program_Low_High_DINT.Solve_Hours( drv_WS.10191, drv_WS.10190 );
    diOperSF1_RCP_Ferm_act = program_Low_High_DINT.Solve_Hours( drv_WS.10193, drv_WS.10192 );
    iOperSF1_RCP_FermTemp = drv_WS.10194 * 0.1;
    iOperSF1_RCP_CoolTemp = drv_WS.10195 * 0.1;
    diOperSF1_RCP_Storage_need = program_Low_High_DINT.Solve_Hours( drv_WS.10197, drv_WS.10196 );
    diOperSF1_RCP_Storage_act = program_Low_High_DINT.Solve_Hours( drv_WS.10199, drv_WS.10198 );
    iOperSF1_RCP_StorageTemp = drv_WS.10200 * 0.1;
    usiOperSF1_Icon_Showering = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10201 );
    usiOperSF1_Icon_Filling = program_INT_TO_LOW_HIGH.KLow( drv_WS.10201 );
    usiOperSF1_Icon_Agit1 = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10202 );
    usiOperSF1_Icon_MotherDough = program_INT_TO_LOW_HIGH.KLow( drv_WS.10202 );
    usiOperSF1_Icon_Agit2 = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10203 );
    usiOperSF1_Icon_Ferm = program_INT_TO_LOW_HIGH.KLow( drv_WS.10203 );
    usiOperSF1_Icon_Cool = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10204 );
    usiOperSF1_Icon_Using = program_INT_TO_LOW_HIGH.KLow( drv_WS.10204 );
    usiOperSF1_Icon_Repump = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10205 );
    usiOperSF1_Icon_Clean = program_INT_TO_LOW_HIGH.KLow( drv_WS.10205 );
    diOperSF1_Age = program_Low_High_DINT.Solve_Hours( drv_WS.10207, drv_WS.10206 );
    diOperSF1_EndUsageTime = program_Low_High_DINT.Solve_Hours( drv_WS.10209, drv_WS.10208 );
    usiOperSF2_Status = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10210 );
    usiOperSF2_Step = program_INT_TO_LOW_HIGH.KLow( drv_WS.10210 );
    iOperSF2_RCP = drv_WS.10211;
    iOperSF2_pH_RCP = drv_WS.10212 * 0.01;
    iOperSF2_pH_Last = drv_WS.10213 * 0.01;
    iOperSF2_TA_RCP = drv_WS.10214;
    iOperSF2_RCP_Name_0_1 = drv_WS.10216;
    iOperSF2_RCP_Name_2_3 = drv_WS.10217;
    iOperSF2_RCP_Name_4_5 = drv_WS.10218;
    iOperSF2_RCP_Name_6_7 = drv_WS.10219;
    iOperSF2_RCP_Name_8_9 = drv_WS.10220;
    iOperSF2_RCP_Name_10_11 = drv_WS.10221;
    iOperSF2_RCP_Name_12_13 = drv_WS.10222;
    iOperSF2_RCP_Name_14_15 = drv_WS.10223;
    iOperSF2_RCP_Name_16_17 = drv_WS.10224;
    iOperSF2_RCP_Name_18_19 = drv_WS.10225;
    iOperSF2_RCP_Name_20_21 = drv_WS.10226;
    iOperSF2_RCP_Name_22_23 = drv_WS.10227;
    iOperSF2_RCP_Name_24_25 = drv_WS.10228;
    iOperSF2_RCP_Name_26_27 = drv_WS.10229;
    iOperSF2_RCP_Name_28_29 = drv_WS.10230;
    iOperSF2_RCP_Name_30_31 = drv_WS.10231;
    iOperSF2_RCP_Name_32_33 = drv_WS.10232;
    iOperSF2_RCP_Name_34_35 = drv_WS.10233;
    iOperSF2_RCP_Name_36_37 = drv_WS.10234;
    iOperSF2_RCP_Name_38_39 = drv_WS.10235;
    diOperSF2_Amount_need = program_Low_High_DINT.Solve_3dp( drv_WS.10237, drv_WS.10236 );
    diOperSF2_RCP_MotherDough_need = program_Low_High_DINT.Solve_3dp( drv_WS.10239, drv_WS.10238 );
    diOperSF2_RCP_MotherDough_act = program_Low_High_DINT.Solve_3dp( drv_WS.10241, drv_WS.10240 );
    diOperSF2_RCP_Fill_need = program_Low_High_DINT.Solve_3dp( drv_WS.10243, drv_WS.10242 );
    diOperSF2_RCP_Fill_act = program_Low_High_DINT.Solve_3dp( drv_WS.10245, drv_WS.10244 );
    diOperSF2_RCP_Water_need = program_Low_High_DINT.Solve_3dp( drv_WS.10247, drv_WS.10246 );
    diOperSF2_RCP_Water_act = program_Low_High_DINT.Solve_3dp( drv_WS.10249, drv_WS.10248 );
    diOperSF2_RCP_Flour_need = program_Low_High_DINT.Solve_3dp( drv_WS.10251, drv_WS.10250 );
    diOperSF2_RCP_Flour_act = program_Low_High_DINT.Solve_3dp( drv_WS.10253, drv_WS.10252 );
    diOperSF2_RCP_Mixture_need = program_Low_High_DINT.Solve_3dp( drv_WS.10255, drv_WS.10254 );
    iOperSF2_RCP_Mixture_TA = drv_WS.10256;
    diOperSF2_RCP_Ferm_need = program_Low_High_DINT.Solve_Hours( drv_WS.10258, drv_WS.10257 );
    diOperSF2_RCP_Ferm_act = program_Low_High_DINT.Solve_Hours( drv_WS.10260, drv_WS.10259 );
    iOperSF2_RCP_FermTemp = drv_WS.10261 * 0.1;
    iOperSF2_RCP_CoolTemp = drv_WS.10262 * 0.1;
    diOperSF2_RCP_Storage_need = program_Low_High_DINT.Solve_Hours( drv_WS.10264, drv_WS.10263 );
    diOperSF2_RCP_Storage_act = program_Low_High_DINT.Solve_Hours( drv_WS.10266, drv_WS.10265 );
    iOperSF2_RCP_StorageTemp = drv_WS.10267 * 0.1;
    usiOperSF2_Icon_Showering = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10268 );
    usiOperSF2_Icon_Filling = program_INT_TO_LOW_HIGH.KLow( drv_WS.10268 );
    usiOperSF2_Icon_Agit1 = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10269 );
    usiOperSF2_Icon_MotherDough = program_INT_TO_LOW_HIGH.KLow( drv_WS.10269 );
    usiOperSF2_Icon_Agit2 = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10270 );
    usiOperSF2_Icon_Ferm = program_INT_TO_LOW_HIGH.KLow( drv_WS.10270 );
    usiOperSF2_Icon_Cool = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10271 );
    usiOperSF2_Icon_Using = program_INT_TO_LOW_HIGH.KLow( drv_WS.10271 );
    usiOperSF2_Icon_Repump = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10272 );
    usiOperSF2_Icon_Clean = program_INT_TO_LOW_HIGH.KLow( drv_WS.10272 );
    diOperSF2_Age = program_Low_High_DINT.Solve_Hours( drv_WS.10274, drv_WS.10273 );
    diOperSF2_EndUsageTime = program_Low_High_DINT.Solve_Hours( drv_WS.10276, drv_WS.10275 );
    iRez277 = drv_WS.10277;
    iRez278 = drv_WS.10278;
    usiMF1_Y2_Clean = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10279 );
    usiMF1_Y3_Water = program_INT_TO_LOW_HIGH.KLow( drv_WS.10279 );
    usiMF1_Y4_Recirc = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10280 );
    usiMF1_Y5_Cool = program_INT_TO_LOW_HIGH.KLow( drv_WS.10280 );
    usiMF1_Y6_Outlet = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10281 );
    usiMF1_agitator = program_INT_TO_LOW_HIGH.KLow( drv_WS.10281 );
    usiSF1_Y1_Flour = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10282 );
    usiSF1_Y2_Clean = program_INT_TO_LOW_HIGH.KLow( drv_WS.10282 );
    usiSF1_Y3_Water = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10283 );
    usiSF1_Y4_Cool = program_INT_TO_LOW_HIGH.KLow( drv_WS.10283 );
    usiSF1_Y5_Outlet = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10284 );
    usiSF1_agitator = program_INT_TO_LOW_HIGH.KLow( drv_WS.10284 );
    usiSF2_Y1_Flour = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10285 );
    usiSF2_Y2_Clean = program_INT_TO_LOW_HIGH.KLow( drv_WS.10285 );
    usiSF2_Y3_Water = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10286 );
    usiSF2_Y4_Cool = program_INT_TO_LOW_HIGH.KLow( drv_WS.10286 );
    usiSF2_Y5_Outlet = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10287 );
    usiSF2_agitator = program_INT_TO_LOW_HIGH.KLow( drv_WS.10287 );
    usiMF1_Agit = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10288 );
    usiSF1_Agit = program_INT_TO_LOW_HIGH.KLow( drv_WS.10288 );
    usiSF2_Agit = program_INT_TO_LOW_HIGH.KHigh( drv_WS.10289 );
    diRestSF = program_Low_High_DINT.Solve_3dp( drv_WS.10291, drv_WS.10290 );
    diOperSF1_RCP_Rest = program_Low_High_DINT.Solve_3dp( drv_WS.10293, drv_WS.10292 );
    diOperSF2_RCP_Rest = program_Low_High_DINT.Solve_3dp( drv_WS.10295, drv_WS.10294 );
    diOperMF1_RCP_Agit1_need = program_Low_High_DINT.Solve_Minutes( drv_WS.10297, drv_WS.10296 );
    diOperMF1_RCP_Agit1_act = program_Low_High_DINT.Solve_Minutes( drv_WS.10299, drv_WS.10298 );
    diOperSF1_RCP_Agit1_need = program_Low_High_DINT.Solve_Minutes( drv_WS.10301, drv_WS.10300 );
    diOperSF1_RCP_Agit1_act = program_Low_High_DINT.Solve_Minutes( drv_WS.10303, drv_WS.10302 );
    diOperSF2_RCP_Agit1_need = program_Low_High_DINT.Solve_Minutes( drv_WS.10305, drv_WS.10304 );
    diOperSF2_RCP_Agit1_act = program_Low_High_DINT.Solve_Minutes( drv_WS.10307, drv_WS.10306 );
    diOperMF1_RCP_Agit2_need = program_Low_High_DINT.Solve_Minutes( drv_WS.10309, drv_WS.10308 );
    diOperMF1_RCP_Agit2_act = program_Low_High_DINT.Solve_Minutes( drv_WS.10311, drv_WS.10310 );
    diOperSF1_RCP_Agit2_need = program_Low_High_DINT.Solve_Minutes( drv_WS.10313, drv_WS.10312 );
    diOperSF1_RCP_Agit2_act = program_Low_High_DINT.Solve_Minutes( drv_WS.10315, drv_WS.10314 );
    diOperSF2_RCP_Agit2_need = program_Low_High_DINT.Solve_Minutes( drv_WS.10317, drv_WS.10316 );
    diOperSF2_RCP_Agit2_act = program_Low_High_DINT.Solve_Minutes( drv_WS.10319, drv_WS.10318 );
    diMF_RCP_LengthBar = program_Low_High_DINT.Solve_Hours( drv_WS.10321, drv_WS.10320 );
    diSF_RCP_LengthBar = program_Low_High_DINT.Solve_Hours( drv_WS.10323, drv_WS.10322 );
    diMF1_AgeReady = program_Low_High_DINT.Solve_Hours( drv_WS.10325, drv_WS.10324 );
    diSF1_AgeReady = program_Low_High_DINT.Solve_Hours( drv_WS.10327, drv_WS.10326 );
    diSF2_AgeReady = program_Low_High_DINT.Solve_Hours( drv_WS.10329, drv_WS.10328 );
    diMF1_AgeOld = program_Low_High_DINT.Solve_Hours( drv_WS.10331, drv_WS.10330 );
    diSF1_AgeOld = program_Low_High_DINT.Solve_Hours( drv_WS.10333, drv_WS.10332 );
    diSF2_AgeOld = program_Low_High_DINT.Solve_Hours( drv_WS.10335, drv_WS.10334 );
    iOperMF1_AgeStatus = drv_WS.10336;
    iOperSF1_AgeStatus = drv_WS.10337;
    iOperSF2_AgeStatus = drv_WS.10338;
    iSelectedTank = drv_WS.10339;
    iRez340 = drv_WS.10340;
    iRez341 = drv_WS.10341;
    iRez342 = drv_WS.10342;
    iRez343 = drv_WS.10343;
    iRez344 = drv_WS.10344;
    iRez345 = drv_WS.10345;
    iRez346 = drv_WS.10346;
    iRez347 = drv_WS.10347;
    iRez348 = drv_WS.10348;
    iRez349 = drv_WS.10349;
    iRez350 = drv_WS.10350;
    iRez351 = drv_WS.10351;
    iRez352 = drv_WS.10352;
    iRez353 = drv_WS.10353;
    iRez354 = drv_WS.10354;
    iRez355 = drv_WS.10355;
    iRez356 = drv_WS.10356;
    iRez357 = drv_WS.10357;
    iRez358 = drv_WS.10358;
    bMF1_LidOpen = bitget( drv_WS.10359, 8 );
    bSF1_LidOpen = bitget( drv_WS.10359, 9 );
    bSF2_LidOpen = bitget( drv_WS.10359, 10 );
    bFlour_IN_Ready = bitget( drv_WS.10359, 11 );
    bFlour_IN_Running = bitget( drv_WS.10359, 12 );
    bFlour_OUT_Dest_SF1 = bitget( drv_WS.10359, 13 );
    bFlour_OUT_Dest_SF2 = bitget( drv_WS.10359, 14 );
    bFlour_OUT_Dosing_Fast = bitget( drv_WS.10359, 15 );
    bFlour_OUT_Dosing_Slow = bitget( drv_WS.10359, 0 );
    bDO_IN_Dosing_Fast = bitget( drv_WS.10359, 1 );
    bDO_IN_Dosing_Slow = bitget( drv_WS.10359, 2 );
    bDO_OUT_Ready_SF1 = bitget( drv_WS.10359, 3 );
    bDO_OUT_Ready_SF2 = bitget( drv_WS.10359, 4 );
    bDO_OUT_Ready_System = bitget( drv_WS.10359, 5 );
    bCoolingUnitOn = bitget( drv_WS.10359, 6 );
    bOutdoseTank_AutoSel = bitget( drv_WS.10359, 7 );
  end_schedule;

  schedule sch_WS_alarm_USINT {period = 3};
    iW0_alarm = drv_WS.90016;
    iW1_alarm = drv_WS.90017;
    iW2_alarm = drv_WS.90018;
    iW3_alarm = drv_WS.90019;
    iW4_alarm = drv_WS.90020;
    iW5_alarm = drv_WS.90021;
    iW6_alarm = drv_WS.90022;
    iW7_alarm = drv_WS.90023;
    iW8_alarm = drv_WS.90024;
    iW9_alarm = drv_WS.90025;
    iW10_alarm = drv_WS.90026;
    iW11_alarm = drv_WS.90027;
    iW12_alarm = drv_WS.90028;
    iW13_alarm = drv_WS.90029;
    iW14_alarm = drv_WS.90030;
    iW15_alarm = drv_WS.90031;
  end_schedule;

  archive WS_norm_USINT {dsn = 'psql_USINT.dsn'; database_type = sql; table_name = 'arWS_norm'; user_name = sPostgresNm; password = sPostgresPw; period = 20; period_offset = 3; period_origin = midnight; condition = com_Status_USINT.enbArch_WS};
    iUserGroup = sch_WS_vizd_USINT.iUserGroup;
    diWMU_Volume = sch_WS_vizd_USINT.diWMU_Volume;
    diMF1_Weight = sch_WS_vizd_USINT.diMF1_Weight;
    diSF1_Weight = sch_WS_vizd_USINT.diSF1_Weight;
    diSF2_Weight = sch_WS_vizd_USINT.diSF2_Weight;
    diDP1_Press = sch_WS_vizd_USINT.diDP1_Press;
    diCLN1_Press = sch_WS_vizd_USINT.diCLN1_Press;
    diDP2_Press = sch_WS_vizd_USINT.diDP2_Press;
    diDP3_Press = sch_WS_vizd_USINT.diDP3_Press;
    iMF1_pH = sch_WS_vizd_USINT.iMF1_pH;
    iSF1_pH = sch_WS_vizd_USINT.iSF1_pH;
    iSF2_pH = sch_WS_vizd_USINT.iSF2_pH;
    iWMU1_Temp = sch_WS_vizd_USINT.iWMU1_Temp;
    iMF1_Temp = sch_WS_vizd_USINT.iMF1_Temp;
    iSF1_Temp_Bottom = sch_WS_vizd_USINT.iSF1_Temp_Bottom;
    iSF1_Temp_Side = sch_WS_vizd_USINT.iSF1_Temp_Side;
    iSF1_Temp = sch_WS_vizd_USINT.iSF1_Temp;
    iSF2_Temp_Bottom = sch_WS_vizd_USINT.iSF2_Temp_Bottom;
    iSF2_Temp_Side = sch_WS_vizd_USINT.iSF2_Temp_Side;
    iSF2_Temp = sch_WS_vizd_USINT.iSF2_Temp;
    iDP1_PumpSpeed_act = sch_WS_vizd_USINT.iDP1_PumpSpeed_act;
    iDP2_PumpSpeed_act = sch_WS_vizd_USINT.iDP2_PumpSpeed_act;
    iDP3_PumpSpeed_act = sch_WS_vizd_USINT.iDP3_PumpSpeed_act;
    iCLN1_Pump2TankSpeed_act = sch_WS_vizd_USINT.iCLN1_Pump2TankSpeed_act;
    iCLN1_Pump2PipeSpeed_act = sch_WS_vizd_USINT.iCLN1_Pump2PipeSpeed_act;
    usiFlour_Status = sch_WS_vizd_USINT.usiFlour_Status;
    usiFlour_Step = sch_WS_vizd_USINT.usiFlour_Step;
    diFlour_Amount_act = sch_WS_vizd_USINT.diFlour_Amount_act;
    usiFlour_Owner = sch_WS_vizd_USINT.usiFlour_Owner;
    usiWater_Status = sch_WS_vizd_USINT.usiWater_Status;
    usiWater_Step = sch_WS_vizd_USINT.usiWater_Step;
    diWater_Amount_act = sch_WS_vizd_USINT.diWater_Amount_act;
    usiWater_Owner = sch_WS_vizd_USINT.usiWater_Owner;
    usiClean_Tank_aut_Status = sch_WS_vizd_USINT.usiClean_Tank_aut_Status;
    usiClean_Tank_aut_Step = sch_WS_vizd_USINT.usiClean_Tank_aut_Step;
    usiClean_Tank_aut_Dest = sch_WS_vizd_USINT.usiClean_Tank_aut_Dest;
    usiClean_Tank_aut_Owner = sch_WS_vizd_USINT.usiClean_Tank_aut_Owner;
    usiClean_Tank_shower_Status = sch_WS_vizd_USINT.usiClean_Tank_shower_Status;
    usiClean_Tank_shower_Step = sch_WS_vizd_USINT.usiClean_Tank_shower_Step;
    usiClean_Tank_shower_Dest = sch_WS_vizd_USINT.usiClean_Tank_shower_Dest;
    usiClean_Tank_shower_Owner = sch_WS_vizd_USINT.usiClean_Tank_shower_Owner;
    usiClean_Tank_man_Status = sch_WS_vizd_USINT.usiClean_Tank_man_Status;
    usiClean_Tank_man_Step = sch_WS_vizd_USINT.usiClean_Tank_man_Step;
    usiClean_Tank_man_Dest = sch_WS_vizd_USINT.usiClean_Tank_man_Dest;
    usiClean_Tank_man_Owner = sch_WS_vizd_USINT.usiClean_Tank_man_Owner;
    usiClean_Pipe1_man_Status = sch_WS_vizd_USINT.usiClean_Pipe1_man_Status;
    usiClean_Pipe1_man_Step = sch_WS_vizd_USINT.usiClean_Pipe1_man_Step;
    usiClean_Pipe1_man_Owner = sch_WS_vizd_USINT.usiClean_Pipe1_man_Owner;
    usiClean_Pipe2_man_Status = sch_WS_vizd_USINT.usiClean_Pipe2_man_Status;
    usiClean_Pipe2_man_Step = sch_WS_vizd_USINT.usiClean_Pipe2_man_Step;
    usiClean_Pipe2_man_Owner = sch_WS_vizd_USINT.usiClean_Pipe2_man_Owner;
    usiTransferPipe1_Status = sch_WS_vizd_USINT.usiTransferPipe1_Status;
    usiTransferPipe1_Step = sch_WS_vizd_USINT.usiTransferPipe1_Step;
    usiTransferPipe1_Dest = sch_WS_vizd_USINT.usiTransferPipe1_Dest;
    diTransferPipe1_Amount_act = sch_WS_vizd_USINT.diTransferPipe1_Amount_act;
    usiTransferPipe1_Owner = sch_WS_vizd_USINT.usiTransferPipe1_Owner;
    usiTransferPipe2_Status = sch_WS_vizd_USINT.usiTransferPipe2_Status;
    usiTransferPipe2_Step = sch_WS_vizd_USINT.usiTransferPipe2_Step;
    usiTransferPipe2_Src = sch_WS_vizd_USINT.usiTransferPipe2_Src;
    usiTransferPipe2_Dest = sch_WS_vizd_USINT.usiTransferPipe2_Dest;
    diTransferPipe2_Amount_act = sch_WS_vizd_USINT.diTransferPipe2_Amount_act;
    usiTransferPipe2_Owner = sch_WS_vizd_USINT.usiTransferPipe2_Owner;
    usiOperMF1_Status = sch_WS_vizd_USINT.usiOperMF1_Status;
    usiOperMF1_Step = sch_WS_vizd_USINT.usiOperMF1_Step;
    iOperMF1_RCP = sch_WS_vizd_USINT.iOperMF1_RCP;
    iOperMF1_pH_RCP = sch_WS_vizd_USINT.iOperMF1_pH_RCP;
    iOperMF1_pH_Last = sch_WS_vizd_USINT.iOperMF1_pH_Last;
    iOperMF1_TA_RCP = sch_WS_vizd_USINT.iOperMF1_TA_RCP;
    diOperMF1_RCP_Starter_act = sch_WS_vizd_USINT.diOperMF1_RCP_Starter_act;
    diOperMF1_RCP_Fill_act = sch_WS_vizd_USINT.diOperMF1_RCP_Fill_act;
    diOperMF1_RCP_Water_act = sch_WS_vizd_USINT.diOperMF1_RCP_Water_act;
    diOperMF1_RCP_Flour_act = sch_WS_vizd_USINT.diOperMF1_RCP_Flour_act;
    diOperMF1_RCP_Ferm_act = sch_WS_vizd_USINT.diOperMF1_RCP_Ferm_act;
    iOperMF1_RCP_FermTemp = sch_WS_vizd_USINT.iOperMF1_RCP_FermTemp;
    iOperMF1_RCP_CoolTemp = sch_WS_vizd_USINT.iOperMF1_RCP_CoolTemp;
    diOperMF1_RCP_Storage_act = sch_WS_vizd_USINT.diOperMF1_RCP_Storage_act;
    iOperMF1_RCP_StorageTemp = sch_WS_vizd_USINT.iOperMF1_RCP_StorageTemp;
    diOperMF1_Age = sch_WS_vizd_USINT.diOperMF1_Age;
    diOperMF1_EndUsageTime = sch_WS_vizd_USINT.diOperMF1_EndUsageTime;
    usiOperSF1_Status = sch_WS_vizd_USINT.usiOperSF1_Status;
    usiOperSF1_Step = sch_WS_vizd_USINT.usiOperSF1_Step;
    iOperSF1_RCP = sch_WS_vizd_USINT.iOperSF1_RCP;
    iOperSF1_pH_RCP = sch_WS_vizd_USINT.iOperSF1_pH_RCP;
    iOperSF1_pH_Last = sch_WS_vizd_USINT.iOperSF1_pH_Last;
    iOperSF1_TA_RCP = sch_WS_vizd_USINT.iOperSF1_TA_RCP;
    diOperSF1_RCP_MotherDough_act = sch_WS_vizd_USINT.diOperSF1_RCP_MotherDough_act;
    diOperSF1_RCP_Fill_act = sch_WS_vizd_USINT.diOperSF1_RCP_Fill_act;
    diOperSF1_RCP_Water_act = sch_WS_vizd_USINT.diOperSF1_RCP_Water_act;
    diOperSF1_RCP_Flour_act = sch_WS_vizd_USINT.diOperSF1_RCP_Flour_act;
    iOperSF1_RCP_Mixture_TA = sch_WS_vizd_USINT.iOperSF1_RCP_Mixture_TA;
    diOperSF1_RCP_Ferm_act = sch_WS_vizd_USINT.diOperSF1_RCP_Ferm_act;
    iOperSF1_RCP_FermTemp = sch_WS_vizd_USINT.iOperSF1_RCP_FermTemp;
    iOperSF1_RCP_CoolTemp = sch_WS_vizd_USINT.iOperSF1_RCP_CoolTemp;
    diOperSF1_RCP_Storage_act = sch_WS_vizd_USINT.diOperSF1_RCP_Storage_act;
    iOperSF1_RCP_StorageTemp = sch_WS_vizd_USINT.iOperSF1_RCP_StorageTemp;
    diOperSF1_Age = sch_WS_vizd_USINT.diOperSF1_Age;
    diOperSF1_EndUsageTime = sch_WS_vizd_USINT.diOperSF1_EndUsageTime;
    usiOperSF2_Status = sch_WS_vizd_USINT.usiOperSF2_Status;
    usiOperSF2_Step = sch_WS_vizd_USINT.usiOperSF2_Step;
    iOperSF2_RCP = sch_WS_vizd_USINT.iOperSF2_RCP;
    iOperSF2_pH_RCP = sch_WS_vizd_USINT.iOperSF2_pH_RCP;
    iOperSF2_pH_Last = sch_WS_vizd_USINT.iOperSF2_pH_Last;
    iOperSF2_TA_RCP = sch_WS_vizd_USINT.iOperSF2_TA_RCP;
    diOperSF2_RCP_MotherDough_act = sch_WS_vizd_USINT.diOperSF2_RCP_MotherDough_act;
    diOperSF2_RCP_Fill_act = sch_WS_vizd_USINT.diOperSF2_RCP_Fill_act;
    diOperSF2_RCP_Water_act = sch_WS_vizd_USINT.diOperSF2_RCP_Water_act;
    diOperSF2_RCP_Flour_act = sch_WS_vizd_USINT.diOperSF2_RCP_Flour_act;
    iOperSF2_RCP_Mixture_TA = sch_WS_vizd_USINT.iOperSF2_RCP_Mixture_TA;
    diOperSF2_RCP_Ferm_act = sch_WS_vizd_USINT.diOperSF2_RCP_Ferm_act;
    iOperSF2_RCP_FermTemp = sch_WS_vizd_USINT.iOperSF2_RCP_FermTemp;
    iOperSF2_RCP_CoolTemp = sch_WS_vizd_USINT.iOperSF2_RCP_CoolTemp;
    diOperSF2_RCP_Storage_act = sch_WS_vizd_USINT.diOperSF2_RCP_Storage_act;
    iOperSF2_RCP_StorageTemp = sch_WS_vizd_USINT.iOperSF2_RCP_StorageTemp;
    diOperSF2_Age = sch_WS_vizd_USINT.diOperSF2_Age;
    diOperSF2_EndUsageTime = sch_WS_vizd_USINT.diOperSF2_EndUsageTime;
    usiMF1_Agit = sch_WS_vizd_USINT.usiMF1_Agit;
    usiSF1_Agit = sch_WS_vizd_USINT.usiSF1_Agit;
    usiSF2_Agit = sch_WS_vizd_USINT.usiSF2_Agit;
    bMF1_LidOpen = sch_WS_vizd_USINT.bMF1_LidOpen;
    bSF1_LidOpen = sch_WS_vizd_USINT.bSF1_LidOpen;
    bSF2_LidOpen = sch_WS_vizd_USINT.bSF2_LidOpen;
    bFlour_IN_Ready = sch_WS_vizd_USINT.bFlour_IN_Ready;
    bFlour_IN_Running = sch_WS_vizd_USINT.bFlour_IN_Running;
    bFlour_OUT_Dest_SF1 = sch_WS_vizd_USINT.bFlour_OUT_Dest_SF1;
    bFlour_OUT_Dest_SF2 = sch_WS_vizd_USINT.bFlour_OUT_Dest_SF2;
    bFlour_OUT_Dosing_Fast = sch_WS_vizd_USINT.bFlour_OUT_Dosing_Fast;
    bFlour_OUT_Dosing_Slow = sch_WS_vizd_USINT.bFlour_OUT_Dosing_Slow;
    bDO_IN_Dosing_Fast = sch_WS_vizd_USINT.bDO_IN_Dosing_Fast;
    bDO_IN_Dosing_Slow = sch_WS_vizd_USINT.bDO_IN_Dosing_Slow;
    bDO_OUT_Ready_SF1 = sch_WS_vizd_USINT.bDO_OUT_Ready_SF1;
    bDO_OUT_Ready_SF2 = sch_WS_vizd_USINT.bDO_OUT_Ready_SF2;
    bDO_OUT_Ready_System = sch_WS_vizd_USINT.bDO_OUT_Ready_System;
    bCoolingUnitOn = sch_WS_vizd_USINT.bCoolingUnitOn;
    diRestSF = sch_WS_vizd_USINT.diRestSF;
    diOperSF1_RCP_Rest = sch_WS_vizd_USINT.diOperSF1_RCP_Rest;
    diOperSF2_RCP_Rest = sch_WS_vizd_USINT.diOperSF2_RCP_Rest;
    diOperMF1_RCP_Agit1_act = sch_WS_vizd_USINT.diOperMF1_RCP_Agit1_act;
    diOperMF1_RCP_Agit2_act = sch_WS_vizd_USINT.diOperMF1_RCP_Agit2_act;
    diOperSF1_RCP_Agit1_act = sch_WS_vizd_USINT.diOperSF1_RCP_Agit1_act;
    diOperSF1_RCP_Agit2_act = sch_WS_vizd_USINT.diOperSF1_RCP_Agit2_act;
    diOperSF2_RCP_Agit1_act = sch_WS_vizd_USINT.diOperSF2_RCP_Agit1_act;
    diOperSF2_RCP_Agit2_act = sch_WS_vizd_USINT.diOperSF2_RCP_Agit2_act;
    diMF1_AgeReady = sch_WS_vizd_USINT.diMF1_AgeReady;
    diSF1_AgeReady = sch_WS_vizd_USINT.diSF1_AgeReady;
    diSF2_AgeReady = sch_WS_vizd_USINT.diSF2_AgeReady;
    diMF1_AgeOld = sch_WS_vizd_USINT.diMF1_AgeOld;
    diSF1_AgeOld = sch_WS_vizd_USINT.diSF1_AgeOld;
    diSF2_AgeOld = sch_WS_vizd_USINT.diSF2_AgeOld;
    iOperMF1_AgeStatus = sch_WS_vizd_USINT.iOperMF1_AgeStatus;
    iOperSF1_AgeStatus = sch_WS_vizd_USINT.iOperSF1_AgeStatus;
    iOperSF2_AgeStatus = sch_WS_vizd_USINT.iOperSF2_AgeStatus;
    iSelectedTank = sch_WS_vizd_USINT.iSelectedTank;
    bOutdoseTank_AutoSel = sch_WS_vizd_USINT.bOutdoseTank_AutoSel;
  end_archive;

  archive WS_slow_USINT {dsn = 'psql_USINT.dsn'; database_type = sql; table_name = 'arWS_slow'; user_name = sPostgresNm; password = sPostgresPw; period = 60; period_offset = 3; period_origin = midnight; condition = com_Status_USINT.enbArch_WS};
    diFlour_Amount_need = sch_WS_vizd_USINT.diFlour_Amount_need;
    diWater_Amount_need = sch_WS_vizd_USINT.diWater_Amount_need;
    diTransferPipe1_Amount_need = sch_WS_vizd_USINT.diTransferPipe1_Amount_need;
    diTransferPipe2_Amount_need = sch_WS_vizd_USINT.diTransferPipe2_Amount_need;
    diOperMF1_Amount_need = sch_WS_vizd_USINT.diOperMF1_Amount_need;
    diOperMF1_RCP_Starter_need = sch_WS_vizd_USINT.diOperMF1_RCP_Starter_need;
    diOperMF1_RCP_Fill_need = sch_WS_vizd_USINT.diOperMF1_RCP_Fill_need;
    diOperMF1_RCP_Water_need = sch_WS_vizd_USINT.diOperMF1_RCP_Water_need;
    diOperMF1_RCP_Flour_need = sch_WS_vizd_USINT.diOperMF1_RCP_Flour_need;
    diOperMF1_RCP_Ferm_need = sch_WS_vizd_USINT.diOperMF1_RCP_Ferm_need;
    diOperMF1_RCP_Storage_need = sch_WS_vizd_USINT.diOperMF1_RCP_Storage_need;
    diOperSF1_Amount_need = sch_WS_vizd_USINT.diOperSF1_Amount_need;
    diOperSF1_RCP_MotherDough_need = sch_WS_vizd_USINT.diOperSF1_RCP_MotherDough_need;
    diOperSF1_RCP_Fill_need = sch_WS_vizd_USINT.diOperSF1_RCP_Fill_need;
    diOperSF1_RCP_Water_need = sch_WS_vizd_USINT.diOperSF1_RCP_Water_need;
    diOperSF1_RCP_Flour_need = sch_WS_vizd_USINT.diOperSF1_RCP_Flour_need;
    diOperSF1_RCP_Mixture_need = sch_WS_vizd_USINT.diOperSF1_RCP_Mixture_need;
    diOperSF1_RCP_Ferm_need = sch_WS_vizd_USINT.diOperSF1_RCP_Ferm_need;
    diOperSF1_RCP_Storage_need = sch_WS_vizd_USINT.diOperSF1_RCP_Storage_need;
    diOperSF2_Amount_need = sch_WS_vizd_USINT.diOperSF2_Amount_need;
    diOperSF2_RCP_MotherDough_need = sch_WS_vizd_USINT.diOperSF2_RCP_MotherDough_need;
    diOperSF2_RCP_Fill_need = sch_WS_vizd_USINT.diOperSF2_RCP_Fill_need;
    diOperSF2_RCP_Water_need = sch_WS_vizd_USINT.diOperSF2_RCP_Water_need;
    diOperSF2_RCP_Flour_need = sch_WS_vizd_USINT.diOperSF2_RCP_Flour_need;
    diOperSF2_RCP_Mixture_need = sch_WS_vizd_USINT.diOperSF2_RCP_Mixture_need;
    diOperSF2_RCP_Ferm_need = sch_WS_vizd_USINT.diOperSF2_RCP_Ferm_need;
    diOperSF2_RCP_Storage_need = sch_WS_vizd_USINT.diOperSF2_RCP_Storage_need;
    diOperMF1_RCP_Agit1_need = sch_WS_vizd_USINT.diOperMF1_RCP_Agit1_need;
    diOperMF1_RCP_Agit2_need = sch_WS_vizd_USINT.diOperMF1_RCP_Agit2_need;
    diOperSF1_RCP_Agit1_need = sch_WS_vizd_USINT.diOperSF1_RCP_Agit1_need;
    diOperSF1_RCP_Agit2_need = sch_WS_vizd_USINT.diOperSF1_RCP_Agit2_need;
    diOperSF2_RCP_Agit1_need = sch_WS_vizd_USINT.diOperSF2_RCP_Agit1_need;
    diOperSF2_RCP_Agit2_need = sch_WS_vizd_USINT.diOperSF2_RCP_Agit2_need;
    diMF_RCP_LengthBar = sch_WS_vizd_USINT.diMF_RCP_LengthBar;
    diSF_RCP_LengthBar = sch_WS_vizd_USINT.diSF_RCP_LengthBar;
  end_archive;

end_data;

instrument

  panel panel_1;
    gui
      owner = background;
      position = 580, 300, 85, 100;
    end_gui;
    mode = window_less;
    colors
      color = cyan;
    end_colors;
  end_panel;

  label label_3;
    gui
      owner = panel_1;
      position = 9, 44;
      window
        disable = zoom, maximize;
      end_window;
    end_gui;
    text_list
      font = 'Tahoma (Central European)', 9, normal;
      text = 'WS';
    end_text_list;
    colors
      paper = cyan;
      ink = white;
    end_colors;
  end_label;

  meter meter_1;
    activity
      period = 10;
      period_offset = 2;
      period_origin = midnight;
    end_activity;
    gui
      owner = panel_1;
      position = 39, 43, 20, 15;
    end_gui;
    expression = drv_WS.10000;
    mode = text_display;
    low_limit = 0;
    high_limit = 100;
    dec_places = 0;
    frame = 0;
    font = 'Segoe UI (Central European)', 10, normal;
    transparent = true;
    colors
      paper = cyan;
      value = cyan;
      low_limit = cyan;
      high_limit = cyan;
    end_colors;
  end_meter;

  indicator indicator_1;
    activity
      period = 1;
    end_activity;
    gui
      owner = panel_1;
      position = 60, 48;
      window
        disable = zoom, maximize;
      end_window;
    end_gui;
    expression = com_Status_USINT.enbArch_WS;
    true_icon = 'gledon.ico';
    false_icon = 'led_off00.ico';
    transparent = true;
  end_indicator;

  switch switch_3;
    gui
      clipboard_root = true;
      owner = panel_1;
      position = 62, 8, 16, 24;
      window
        disable = zoom, maximize;
      end_window;
    end_gui;
    mode = text_button;
    font = 'Arial (Central European)', 10, bold;
    true_text = 'x';
    false_text = 'x';
    colors
      true_paper = lgray;
      true_ink = dgray;
      false_paper = lgray;
      false_ink = dgray;
    end_colors;

    procedure OnActivate();
    begin
      core.StopApplication();
    end_procedure;

    procedure OnMouseDown( MouseX, MouseY : longint; LeftButton, MiddleButton, RightButton : boolean );
    begin
      core.StopApplication();
    end_procedure;

  end_switch;

  label label_1;
    gui
      owner = panel_1;
      position = 6, 8;
      window
        disable = zoom, maximize;
      end_window;
    end_gui;
    text_list
      font = 'Microsoft Sans Serif (Central European)', 8, bold;
      text = 'US_INT';
    end_text_list;
    colors
      paper = dgray;
      ink = lgray;
    end_colors;
  end_label;

  box box_1;
    gui
      owner = panel_1;
      position = 5, 5, 75, 30;
    end_gui;
    colors
      interior = dgray;
      border = lgray;
    end_colors;
  end_box;

  box box_1;
    gui
      owner = panel_1;
      position = 5, 5, 75, 90;
    end_gui;
    mode = border_only;
    colors
      border = lgray;
    end_colors;
  end_box;

  program utils;

    procedure PocetDniMesice( Rok : integer; MesOd1 : integer ): integer;
    var Res : integer;
    begin
      switch MesOd1 of
        case  1; Res := 31;
        case  2; Res := 28;
        case  3; Res := 31;
        case  4; Res := 30;
        case  5; Res := 31;
        case  6; Res := 30;
        case  7; Res := 31;
        case  8; Res := 31;
        case  9; Res := 30;
        case 10; Res := 31;
        case 11; Res := 30;
        case 12; Res := 31;
        else Res := 31;
       end;
    
      (* prestupny rok *)
      if (Res = 28) then
        if (Rok % 4) = 0 then
          Res := 29;
        end;
      end;
    
      return Res
    end_procedure;

  end_program;

  program lib;

    procedure PackedToJD( diPackedTime : longint ): real;
    var rTmp : real;
    begin
      rTmp := diPackedTime;
      rTmp := rTmp / 86400.0;
      rTmp := rTmp + 2451544.5;
      return rTmp;
    end_procedure;

    procedure PocetDniMesice( iRok : integer; iMes : integer ): shortint;
    begin
      if (iMes = 1) or (iMes = 3) or (iMes = 5) or (iMes = 7) or (iMes = 8) or (iMes = 10) or (iMes = 12) then
        return 31;
      end;
      if (iMes = 4) or (iMes = 4) or (iMes = 9) or (iMes = 11) then
        return 30;
      end;
      (* unor *)
      if (iRok % 4) = 0 then
        return 29;
      else
        return 28;
      end;
    end_procedure;

  end_program;

  program program_INT_TO_LOW_HIGH;
    static
      itest : integer;
      stest : string;
    end_static;


    procedure KHigh( K : cardinal ): real;
     (* WORD na HighBYTE *)
        begin
          return (bitand(32512,K))/256;
    end_procedure;

    procedure KLow( K : cardinal ): real;
     (* WORD na LowBYTE *)
        begin
          return bitand(255,K);
    end_procedure;

    procedure pole_TO_str( pole : array of integer ): string;
    var
     m, Low, High : integer;
     user : string;
    label End_Low, End_High;
    begin
    itest:=0;
    
    for m = 0 to 4 do
    
       if (KLow(pole[m]) <> 0) then
       Low := KLow(pole[m]);
       user:=user+char(Low);
       else goto End_Low;
       end;
       
    End_Low:
    
       if (KHigh(pole[m]) <> 0) then
       High := KHigh(pole[m]);
       user:=user+char(High);
       else goto End_High;
       end;
    end;
    
    End_High:
    
    return user;
    end_procedure;

  end_program;

  file file_log1;
  end_file;

  program program_Comm_Test;
    static
      iTimeNow_WS : integer;
      i : integer;
    end_static;

    activity
      period = 10;
      period_offset = 2;
      period_origin = midnight;
    end_activity;

    procedure OnActivate();
    begin
    iTimeNow_WS:= drv_WS.10000;
    
    (* test komunikace BF *)
    
    if (iTimeNow_WS <> com_Status_USINT.iTimeLast_WS) and (i <= 3) then i:=i+1;
    elsif (iTimeNow_WS = com_Status_USINT.iTimeLast_WS) and (i >= 1) then i:=i-1;
    end;
    
    (* povolení archivace BF *)
    
    if i > 2 then com_Status_USINT.enbArch_WS := true
    elsif i <= 2 then com_Status_USINT.enbArch_WS := false;
    end;
    
    com_Status_USINT.iTimeLast_WS := iTimeNow_WS;
    end_procedure;

  end_program;

  sql sql_USINT;
    gui
      position = 21, 20, 97, 89;
    end_gui;

    procedure OnStartup();
    var async: boolean;
    begin
      async := false;
      OpenDatabase('psql_USINT.dsn', '', sPostgresPw, async);
    end_procedure;

    procedure OnTerminate( TerminatedDueToFailure : boolean );
    begin
      CloseDatabase();
    end_procedure;

  end_sql;

  program program_Datum;

    procedure PackedToJD( diPackedTime : longint ): real;
     var rTmp : real;
     begin
      rTmp := diPackedTime;
      rTmp := rTmp / 86400.0;
      rTmp := rTmp + 2451544.5;
      return trunc(rTmp);
    end_procedure;

    procedure JDToPacked( JD : real ): longint;
     var rTmp : real;
     begin
      rTmp := JD - 2451544.5;
      return trunc(rTmp * 86400.0);
    end_procedure;

  end_program;

  program program_StartUp;
    activity
      period = infinite;
    end_activity;

    procedure OnStartup();
    begin
     localvars_USINT.bInitial := true;
    end_procedure;

  end_program;

  program program_Low_High_DINT;

    procedure Solve( High, Low : longint ): longint;
    begin
     if (bitget(Low,15) = 1 ) then 
        return(High);
      else
        return(bitor( bitand( High, 65535 ), bitshl( Low, 16 )));
      end;
    end_procedure;

    procedure Solve_3dp( High, Low : longint ): real;
    begin
     if (bitget(Low,15) = 1 ) then 
        return(High * 0.001);
      else
        return(bitor( bitand( High, 65535 ), bitshl( Low, 16 )) * 0.001);
      end;
    end_procedure;

    procedure Solve_Hours( High, Low : longint ): longint;
    begin
     if (bitget(Low,15) = 1 ) then 
        return(High / 3600);
      else
        return(bitor( bitand( High, 65535 ), bitshl( Low, 16 )) / 3600);
      end;
    end_procedure;

    procedure Solve_Minutes( High, Low : longint ): longint;
    begin
     if (bitget(Low,15) = 1 ) then 
        return(High / 60);
      else
        return(bitor( bitand( High, 65535 ), bitshl( Low, 16 )) / 60);
      end;
    end_procedure;

    procedure Solve_2dp( High, Low : longint ): real;
    begin
     if (bitget(Low,15) = 1 ) then 
        return(High * 0.01);
      else
        return(bitor( bitand( High, 65535 ), bitshl( Low, 16 )) * 0.01);
      end;
    end_procedure;

  end_program;

  program program_WS_Alm;
    activity
      period = 5;
      period_offset = 1;
      condition = com_Status_USINT.enbArch_WS;
    end_activity;

    procedure OnActivate();
    var
     iLastAlms : array [0..15] of cardinal;
     iActualAlms : array [0..15] of cardinal;
     iVal : cardinal;
     i, j, iVersion : integer;
     rUTC, rDST_Bias : real;
     sQry : string;
     bRes, bIsNull : boolean;
     diNowDateTime : longint;
     iPlcId : shortcard;
    begin
    
    (*return;*)
    
     rUTC := date.GetDateTimeJD();
     diNowDateTime := program_Datum.JDToPacked(rUTC);
     iPlcId := 1;
     
     (* nabytí pole aktuálními alarmy *)
     iActualAlms[0]:= bitand( sch_WS_alarm_USINT.iW0_alarm, 65535 );
     iActualAlms[1]:= bitand( sch_WS_alarm_USINT.iW1_alarm, 65535 );
     iActualAlms[2]:= bitand( sch_WS_alarm_USINT.iW2_alarm, 65535 );
     iActualAlms[3]:= bitand( sch_WS_alarm_USINT.iW3_alarm, 65535 );
     iActualAlms[4]:= bitand( sch_WS_alarm_USINT.iW4_alarm, 65535 );
     iActualAlms[5]:= bitand( sch_WS_alarm_USINT.iW5_alarm, 65535 );
     iActualAlms[6]:= bitand( sch_WS_alarm_USINT.iW6_alarm, 65535 );
     iActualAlms[7]:= bitand( sch_WS_alarm_USINT.iW7_alarm, 65535 );
     iActualAlms[8]:= bitand( sch_WS_alarm_USINT.iW8_alarm, 65535 );
     iActualAlms[9]:= bitand( sch_WS_alarm_USINT.iW9_alarm, 65535 );
     iActualAlms[10]:= bitand( sch_WS_alarm_USINT.iW10_alarm, 65535 );
     iActualAlms[11]:= bitand( sch_WS_alarm_USINT.iW11_alarm, 65535 );
     iActualAlms[12]:= bitand( sch_WS_alarm_USINT.iW12_alarm, 65535 );
     iActualAlms[13]:= bitand( sch_WS_alarm_USINT.iW13_alarm, 65535 );
     iActualAlms[14]:= bitand( sch_WS_alarm_USINT.iW14_alarm, 65535 );
     iActualAlms[15]:= bitand( sch_WS_alarm_USINT.iW15_alarm, 65535 );
    
     (* naètení vektoru alarmù z DB *)
     sQry:='SELECT * from "Alm_Sys" WHERE "idx" = ' + str(iPlcId,10);
     bRes := sql_USINT.OpenRecordset( 'rs01', sQry);
     if (bRes) then
       bRes := sql_USINT.MoveToFirstRecord('rs01');
       if (bRes) then
         sql_USINT.GetData('rs01', 'W0', iLastAlms[0], bIsNull);
         sql_USINT.GetData('rs01', 'W1', iLastAlms[1], bIsNull);
         sql_USINT.GetData('rs01', 'W2', iLastAlms[2], bIsNull);
         sql_USINT.GetData('rs01', 'W3', iLastAlms[3], bIsNull);
         sql_USINT.GetData('rs01', 'W4', iLastAlms[4], bIsNull);
         sql_USINT.GetData('rs01', 'W5', iLastAlms[5], bIsNull);
         sql_USINT.GetData('rs01', 'W6', iLastAlms[6], bIsNull);
         sql_USINT.GetData('rs01', 'W7', iLastAlms[7], bIsNull);
         sql_USINT.GetData('rs01', 'W8', iLastAlms[8], bIsNull);
         sql_USINT.GetData('rs01', 'W9', iLastAlms[9], bIsNull);
         sql_USINT.GetData('rs01', 'W10', iLastAlms[10], bIsNull);
         sql_USINT.GetData('rs01', 'W11', iLastAlms[11], bIsNull);
         sql_USINT.GetData('rs01', 'W12', iLastAlms[12], bIsNull);
         sql_USINT.GetData('rs01', 'W13', iLastAlms[13], bIsNull);
         sql_USINT.GetData('rs01', 'W14', iLastAlms[14], bIsNull);
         sql_USINT.GetData('rs01', 'W15', iLastAlms[15], bIsNull);
       end;
     end;
     sql_USINT.CloseRecordset('rs01');
    
     if not (localvars_USINT.bInitial) then
      for j = 0 to 15 do
      (* zjistìní novì vzniklých alarmù a ulození do DB *)
       if (iActualAlms[j]) <> 0 then  
         iVal := bitxor(iActualAlms[j], iLastAlms[j]);
         iVal := bitand(iVal, iActualAlms[j]);
         localvars_USINT.iIndex_WS := 0;
         for i := 8 to 15 do
           if bitget(iVal,i) = 1 then
             sQry:='INSERT INTO "alarm_history" ("origin_pktime", "expiry_pktime", "plc_id", "alarm_id") VALUES (''' + str(diNowDateTime,10) + ''', 0 ,' + str(iPlcId,10) + ',' + str((16*j+(i-7)),10) + ')';
             sql_USINT.Execute(sQry);
             localvars_USINT.iIndex_WS := localvars_USINT.iIndex_WS + 1;
           end;
          end;
          for i := 0 to 7 do
           if bitget(iVal,i) = 1 then
             sQry:='INSERT INTO "alarm_history" ("origin_pktime", "expiry_pktime", "plc_id", "alarm_id") VALUES (''' + str(diNowDateTime,10) + ''', 0 ,' + str(iPlcId,10) + ',' + str((16*j+(i+9)),10) + ')';
             sql_USINT.Execute(sQry);
             localvars_USINT.iIndex_WS := localvars_USINT.iIndex_WS + 1;
           end;
          end;
       end;
    
       (* zjistìní potvrzených alarmù a UPDATE DB *)
         iVal := bitxor(iActualAlms[j], iLastAlms[j]);
         iVal := bitand(iVal, iLastAlms[j]);
         for i := 8 to 15 do
           if bitget(iVal,i) = 1 then
             sQry:='UPDATE "alarm_history" SET "expiry_pktime" = '  + str(diNowDateTime,10) + ' WHERE "alarm_id" = ' + str((16*j+(i-7)),10) + ' AND "plc_id" = ' + str(iPlcId,10) + ' AND "expiry_pktime" = 0 ';
             sql_USINT.Execute(sQry);
           end;
          end;
          for i := 0 to 7 do
           if bitget(iVal,i) = 1 then
             sQry:='UPDATE "alarm_history" SET "expiry_pktime" = '  + str(diNowDateTime,10) + ' WHERE "alarm_id" = ' + str((16*j+(i+9)),10) + ' AND "plc_id" = ' + str(iPlcId,10) + ' AND "expiry_pktime" = 0 ';
             sql_USINT.Execute(sQry);
           end;
          end;
         sQry:='UPDATE "Alm_Sys" SET "W' + str(j,10) + '" = ' + str(iActualAlms[j],10) + ' WHERE "idx" = ' + str(iPlcId,10);
         sql_USINT.Execute(sQry);
      end;
    
     else
    
       (* pøi inicializaci pouze ulozeni aktualnich alarmù *)
       for j := 0 to 15 do      
         sQry:='UPDATE "Alm_Sys" SET "W' + str(j,10) + '" = ' + str(iActualAlms[j],10) + ' WHERE "idx" = ' + str(iPlcId,10);
         sql_USINT.Execute(sQry);
       end;
       localvars_USINT.bInitial := false;
     end;
    end_procedure;

  end_program;

end_instrument;

