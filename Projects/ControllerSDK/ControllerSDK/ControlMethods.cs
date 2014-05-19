//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.InteropServices;

//namespace ControllerSDK
//{
//    public class ControlMethods
//    {
//void Insert_Card(Int32 loginID, int nRecrdNo)
//{
//    string strTemp;
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARD()
//    stuCard.bIsValid = true;
//    stuCard.emStatus = SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_NORMAL;
//    stuCard.emType = SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_GENERAL;
//    stuCard.nTimeSectionNum = 1;
//    stuCard.nUserTime = 10;

//    stuCard.stuCreateTime.dwYear = 2013;
//    stuCard.stuCreateTime.dwMonth = 12;
//    stuCard.stuCreateTime.dwDay = 12;
//    stuCard.stuCreateTime.dwHour = 11;
//    stuCard.stuCreateTime.dwMinute = 30;
//    stuCard.stuCreateTime.dwSecond = 30;

//    stuCard.stuValidStartTime.dwYear = 2013;
//    stuCard.stuValidStartTime.dwMonth = 12;
//    stuCard.stuValidStartTime.dwDay = 12;
//    stuCard.stuValidStartTime.dwHour = 11;
//    stuCard.stuValidStartTime.dwMinute = 30;
//    stuCard.stuValidStartTime.dwSecond = 30;
	
//    stuCard.stuValidEndTime.dwYear = 2014;
//    stuCard.stuValidEndTime.dwMonth = 12;
//    stuCard.stuValidEndTime.dwDay = 12;
//    stuCard.stuValidEndTime.dwHour = 11;
//    stuCard.stuValidEndTime.dwMinute = 30;
//    stuCard.stuValidEndTime.dwSecond = 30;

//    stuCard.szCardNo[0] = '1';
//    stuCard.szCardNo[1] = '2';
//    stuCard.szCardNo[2] = '3';
//    stuCard.szCardNo[3] = '4';
//    stuCard.szCardNo[4] = '5';
//    stuCard.szCardNo[5] = '6';
//    stuCard.nDoorNum = 2;
//    stuCard.sznDoors[0] = 1;
//    stuCard.sznDoors[1] = 2;
//    stuCard.nTimeSectionNum = 2;
//    stuCard.sznTimeSectionNo[0] = 1;
//    stuCard.sznTimeSectionNo[1] = 2;

//    stuCard.szPsw[0] = '1';
//    stuCard.szPsw[1] = '2';
//    stuCard.szPsw[2] = '3';
//    stuCard.szPsw[3] = '4';
//    stuCard.szPsw[4] = '5';

//        stuCard.szUserID[0] = '1';
//    stuCard.szUserID[1] = '2';
//    stuCard.szUserID[2] = '3';
	
//    SDKImport.NET_CTRL_RECORDSET_INSERT_PARAM stuInert = new SDKImport.NET_CTRL_RECORDSET_INSERT_PARAM();
//    stuInert.stuCtrlRecordSetInfo.dwSize = Marshal.SizeOf(typeof(SDKImport.NET_CTRL_RECORDSET_INSERT_IN));
//    stuInert.stuCtrlRecordSetInfo.emType = SDKImport.EM_NET_RECORD_TYPE.NET_RECORD_ACCESSCTLCARD;

//    int stuCardSize = Marshal.SizeOf(stuCard);
//                IntPtr stuCardPtr = Marshal.AllocHGlobal(stuCardSize);
//            Marshal.StructureToPtr(stuCard, stuCardPtr, false);

//    stuInert.stuCtrlRecordSetInfo.pBuf = stuCardPtr;
//    stuInert.stuCtrlRecordSetInfo.nBufLen = stuCardSize;
	
//    stuInert.stuCtrlRecordSetResult.dwSize = Marshal.SizeOf(typeof(SDKImport.NET_CTRL_RECORDSET_INSERT_OUT));
	
//    bool bResult = SDKImport.CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
//    nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
//    if (bResult)
//    {
//        g_nRecordNoCard = nRecrdNo;
//    }
//    return;
//}

//void Insert_Pwd(Int32 lLoginID, Int32 nRecrdNo)
//{
//    string strTemp;
//    SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
//    stuAccessCtlPwd.stuCreateTime.dwYear = 2013;
//    stuAccessCtlPwd.stuCreateTime.dwMonth = 12;
//    stuAccessCtlPwd.stuCreateTime.dwDay = 12;
//    stuAccessCtlPwd.stuCreateTime.dwHour = 11;
//    stuAccessCtlPwd.stuCreateTime.dwMinute = 30;
//    stuAccessCtlPwd.stuCreateTime.dwSecond = 30;
//    //stuAccessCtlPwd.stuCreateTime;
//    strTemp = "123456";
//    memcpy(stuAccessCtlPwd.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
//    strTemp = "123456";
//    memcpy(stuAccessCtlPwd.szDoorOpenPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
//    strTemp = "123456";
//    memcpy(stuAccessCtlPwd.szAlarmPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
//    stuAccessCtlPwd.nDoorNum = 2;
//    stuAccessCtlPwd.sznDoors[0] = 1;
//    stuAccessCtlPwd.sznDoors[1] = 2;

//    SDKImport.NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
//    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLPWD;
//    stuInert.stuCtrlRecordSetInfo.pBuf = &stuAccessCtlPwd;
//    stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuAccessCtlPwd);
	
//    stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
//    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
//    nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
//    printf("CLIENT_ControlDevice result: %d, nRecrdNo:%d", bResult, nRecrdNo);
//    if (bResult)
//    {
//        g_nRecordNoPwd = nRecrdNo;
//    }
//    return;
//}

//void Insert_CardRec(Int32 lLoginID, Int32 nRecrdNo)
//{
//    string strTemp;
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
//    strTemp = "123";
//    memcpy(stuCardRec.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
//    strTemp = "123456";
//    memcpy(stuCardRec.szPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	
//    stuCardRec.stuTime.dwYear = 2013;
//    stuCardRec.stuTime.dwMonth = 12;
//    stuCardRec.stuTime.dwDay = 12;
//    stuCardRec.stuTime.dwHour = 11;
//    stuCardRec.stuTime.dwMinute = 30;
//    stuCardRec.stuTime.dwSecond = 30;

//    stuCardRec.bStatus = true;
//    stuCardRec.emMethod = NET_ACCESS_DOOROPEN_METHOD_CARD;
//    stuCardRec.nDoor = 1;

//    SDKImport.NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
//    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARDREC;
//    stuInert.stuCtrlRecordSetInfo.pBuf = &stuCardRec;
//    stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuCardRec);
	
//    stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
//    nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
//    if (bResult)
//    {
//        g_nRecordNoCardRec = nRecrdNo;
//    }
//    return;
//}
//void Insert_Holiday(Int32 lLoginID, Int32 nRecrdNo)
//{
//    SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
//    stuHoliday.nDoorNum = 2;
//    stuHoliday.sznDoors[0] = 1;
//    stuHoliday.sznDoors[1] = 2;

//    stuHoliday.stuStartTime.dwYear = 2013;
//    stuHoliday.stuStartTime.dwMonth = 10;
//    stuHoliday.stuStartTime.dwDay = 1;
//    stuHoliday.stuStartTime.dwHour = 0;
//    stuHoliday.stuStartTime.dwMinute = 0;
//    stuHoliday.stuStartTime.dwSecond = 0;
	
//    stuHoliday.stuEndTime.dwYear = 2013;
//    stuHoliday.stuEndTime.dwMonth = 10;
//    stuHoliday.stuEndTime.dwDay = 7;
//    stuHoliday.stuEndTime.dwHour = 0;
//    stuHoliday.stuEndTime.dwMinute = 0;
//    stuHoliday.stuEndTime.dwSecond = 0;
//    stuHoliday.bEnable = true;
	
//    SDKImport.NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
//    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLHOLIDAY;
//    stuInert.stuCtrlRecordSetInfo.pBuf = &stuHoliday;
//    stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuHoliday);
	
//    stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
//    nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
//    printf("CLIENT_ControlDevice result: %d, nRecrdNo:%d", bResult, nRecrdNo);
//    if (bResult)
//    {
//        g_nRecordNoHoliday = nRecrdNo;
//    }
//    return;
//}

//void Update_Card(Int32 lLoginID)
//{
//    string strTemp;
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};
//    stuCard.nRecNo = 12;
//    stuCard.bIsValid = true;
//    stuCard.emStatus = NET_ACCESSCTLCARD_STATE_NORMAL;
//    stuCard.emType = NET_ACCESSCTLCARD_TYPE_GENERAL;
//    stuCard.nTimeSectionNum = 1;
//    stuCard.nUserTime = 10;
//    stuCard.stuValidStartTime.dwYear = 2013;
//    stuCard.stuValidStartTime.dwMonth = 12;
//    stuCard.stuValidStartTime.dwDay = 12;
//    stuCard.stuValidStartTime.dwHour = 11;
//    stuCard.stuValidStartTime.dwMinute = 30;
//    stuCard.stuValidStartTime.dwSecond = 30;
	
//    stuCard.stuValidEndTime.dwYear = 2014;
//    stuCard.stuValidEndTime.dwMonth = 12;
//    stuCard.stuValidEndTime.dwDay = 12;
//    stuCard.stuValidEndTime.dwHour = 11;
//    stuCard.stuValidEndTime.dwMinute = 30;
//    stuCard.stuValidEndTime.dwSecond = 30;
	
//    strTemp = "123456";
//    memcpy(stuCard.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
//    stuCard.nDoorNum = 2;
//    stuCard.sznDoors[0] = 1;
//    stuCard.sznDoors[1] = 2;
//    stuCard.nTimeSectionNum = 2;
//    stuCard.sznTimeSectionNo[0] = 1;
//    stuCard.sznTimeSectionNo[1] = 2;
//    strTemp = "123456";
//    memcpy(stuCard.szPsw, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
//    strTemp = "123";
//    memcpy(stuCard.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
	
//    stuCard.nRecNo = g_nRecordNoCard < 0 ? 12 : g_nRecordNoCard;
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.emType = NET_RECORD_ACCESSCTLCARD;
//    stuInert.pBuf = &stuCard;
//    stuInert.nBufLen = sizeof(stuCard);
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
//}

//void Int32(Int32 lLoginID)
//{
//    string strTemp;
//    SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
	
//    stuAccessCtlPwd.stuCreateTime.dwYear = 2013;
//    stuAccessCtlPwd.stuCreateTime.dwMonth = 12;
//    stuAccessCtlPwd.stuCreateTime.dwDay = 12;
//    stuAccessCtlPwd.stuCreateTime.dwHour = 11;
//    stuAccessCtlPwd.stuCreateTime.dwMinute = 30;
//    stuAccessCtlPwd.stuCreateTime.dwSecond = 30;
//    //stuAccessCtlPwd.stuCreateTime;
//    strTemp = "123456";
//    memcpy(stuAccessCtlPwd.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
//    strTemp = "123456";
//    memcpy(stuAccessCtlPwd.szDoorOpenPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
//    strTemp = "123456";
//    memcpy(stuAccessCtlPwd.szAlarmPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
//    stuAccessCtlPwd.nDoorNum = 2;
//    stuAccessCtlPwd.sznDoors[0] = 1;
//    stuAccessCtlPwd.sznDoors[1] = 2;
	
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.emType = NET_RECORD_ACCESSCTLPWD;
//    stuInert.pBuf = &stuAccessCtlPwd;
//    stuInert.nBufLen = sizeof(stuAccessCtlPwd);
//    stuAccessCtlPwd.nRecNo = g_nRecordNoPwd < 0 ? 12 : g_nRecordNoPwd;
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
//}

//void Update_CardRec(Int32 lLoginID)
//{
//    string strTemp;
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	
//    strTemp = "123";
//    memcpy(stuCardRec.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
//    strTemp = "123456";
//    memcpy(stuCardRec.szPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	
//    stuCardRec.stuTime.dwYear = 2013;
//    stuCardRec.stuTime.dwMonth = 12;
//    stuCardRec.stuTime.dwDay = 12;
//    stuCardRec.stuTime.dwHour = 11;
//    stuCardRec.stuTime.dwMinute = 30;
//    stuCardRec.stuTime.dwSecond = 30;
	
//    stuCardRec.bStatus = true;
//    stuCardRec.emMethod = NET_ACCESS_DOOROPEN_METHOD_CARD;
//    stuCardRec.nDoor = 1;
//    stuCardRec.nRecNo = g_nRecordNoCardRec < 0 ? 12 : g_nRecordNoCardRec;
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
//    stuInert.pBuf = &stuCardRec;
//    stuInert.nBufLen = sizeof(stuCardRec);
	
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
//}

//void Update_Holiday(Int32 lLoginID)
//{
//    SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
	
//    stuHoliday.nDoorNum = 2;
//    stuHoliday.sznDoors[0] = 1;
//    stuHoliday.sznDoors[1] = 2;
	
//    stuHoliday.stuStartTime.dwYear = 2013;
//    stuHoliday.stuStartTime.dwMonth = 10;
//    stuHoliday.stuStartTime.dwDay = 1;
//    stuHoliday.stuStartTime.dwHour = 0;
//    stuHoliday.stuStartTime.dwMinute = 0;
//    stuHoliday.stuStartTime.dwSecond = 0;
	
//    stuHoliday.stuEndTime.dwYear = 2013;
//    stuHoliday.stuEndTime.dwMonth = 10;
//    stuHoliday.stuEndTime.dwDay = 7;
//    stuHoliday.stuEndTime.dwHour = 0;
//    stuHoliday.stuEndTime.dwMinute = 0;
//    stuHoliday.stuEndTime.dwSecond = 0;
//    stuHoliday.bEnable = true;
//    stuHoliday.nRecNo = g_nRecordNoHoliday < 0 ? 12 : g_nRecordNoHoliday;
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
//    stuInert.pBuf = &stuHoliday;
//    stuInert.nBufLen = sizeof(stuHoliday);
	
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
//}

//bool DevCtrl_OpenDoor(Int32 lLoginID)
//{
//    SDKImport.NET_CTRL_ACCESS_OPEN stuInert = {sizeof(stuInert)};
//    stuInert.nChannelID = 0;
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_ACCESS_OPEN, &stuInert, SDK_API_WAITTIME);
//    return true;
//}

//bool DevCtrl_ReBoot(Int32 lLoginID)
//{
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_REBOOT, NULL, SDK_API_WAITTIME);
//    return true;
//}

//bool DevCtrl_DeleteCfgFile(Int32 lLoginID)
//{
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RESTOREDEFAULT, NULL, SDK_API_WAITTIME);
//    return true;
//}

//bool DevCtrl_GetLogCount(Int32 lLoginID)
//{
//    SDKImport.NET_IN_GETCOUNT_LOG_PARAM stuInLogCount = {sizeof(NET_IN_GETCOUNT_LOG_PARAM)};
//    SDKImport.NET_OUT_GETCOUNT_LOG_PARAM stuOutLogCount = {sizeof(NET_OUT_GETCOUNT_LOG_PARAM)};

//    SDKImport.QUERY_DEVICE_LOG_PARAM& stuGetLog = stuInLogCount.stuQueryCondition;
//    //stuGetLog.emLogType = DHLOG_ALARM;
//    stuGetLog.stuStartTime.dwYear = 2013;
//    stuGetLog.stuStartTime.dwMonth = 10;
//    stuGetLog.stuStartTime.dwDay = 1;
//    stuGetLog.stuStartTime.dwHour = 0;
//    stuGetLog.stuStartTime.dwMinute = 0;
//    stuGetLog.stuStartTime.dwSecond = 0;
	
//    stuGetLog.stuEndTime.dwYear = 2013;
//    stuGetLog.stuEndTime.dwMonth = 10;
//    stuGetLog.stuEndTime.dwDay = 7;
//    stuGetLog.stuEndTime.dwHour = 0;
//    stuGetLog.stuEndTime.dwMinute = 0;
//    stuGetLog.stuEndTime.dwSecond = 0;

//    stuGetLog.nStartNum = 10;
//    stuGetLog.nLogStuType = 1;
//    stuGetLog.nEndNum = 20;
	
//    bool bResult = SDKImport.CLIENT_QueryDevLogCount(lLoginID, &stuInLogCount, &stuOutLogCount, SDK_API_WAITTIME);
//    return true;
//}

//int DevCtrl_InsertRecordSet(Int32 lLoginID, int nRecordSetType)
//{
//    int nRecordNo = -1;
//    switch (nRecordSetType)
//    {
//    case 1:
//        Insert_Card(lLoginID, nRecordNo);
//        break;
//    case 2:
//        Insert_Pwd(lLoginID, nRecordNo);
//        break;
//    case 3:
//        Insert_CardRec(lLoginID, nRecordNo);
//        break;
//    case 4:
//        Insert_Holiday(lLoginID, nRecordNo);
//        break;
//    default:
//        break;
//    }
//    return nRecordNo;
//}

//bool DevCtrl_UpdateRecordSet(Int32 lLoginID, int nRecordSetType)
//{
//    int nRecordNo = -1;
//    switch (nRecordSetType)
//    {
//    case 1:
//        Update_Card(lLoginID);
//        break;
//    case 2:
//        Update_Pwd(lLoginID);
//        break;
//    case 3:
//        Update_CardRec(lLoginID);
//        break;
//    case 4:
//        Update_Holiday(lLoginID);
//        break;
//    default:
//        break;
//    }
//    return true;
//}

//bool DevCtrl_RemoveRecordSet(Int32 lLoginID, int nRecordSetType)
//{
//    int nRecordNo = 123;
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    stuInert.pBuf = &nRecordNo;
//    stuInert.nBufLen = sizeof(nRecordNo);
//    switch (nRecordSetType)
//    {
//    case 1:
//        stuInert.emType = NET_RECORD_ACCESSCTLCARD;
//        break;
//    case 2:
//        stuInert.emType = NET_RECORD_ACCESSCTLPWD;
//        break;
//    case 3:
//        stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
//        break;
//    case 4:
//        stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
//        break;
//    default:
//        break;
//    }
//    bool bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
//    return true;
//}

//bool DevCtrl_ClearRecordSet(Int32 lLoginID, int nRecordSetType)
//{
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    switch (nRecordSetType)
//    {
//    case 1:
//        stuInert.emType = NET_RECORD_ACCESSCTLCARD;
//        break;
//    case 2:
//        stuInert.emType = NET_RECORD_ACCESSCTLPWD;
//        break;
//    case 3:
//        stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
//        break;
//    case 4:
//        stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
//        break;
//    default:
//        break;
//    }
//    bool bResult = SDKImport.CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
//    return true;
//}

////////////////////////////////////////////////////////////////////////////
////
//// card
////
////////////////////////////////////////////////////////////////////////////
//void testRecordSetFind_Card(Int32 lLoginId, LLONG& lFinderId)
//{
//    SDKImport.NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    SDKImport.NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
//    stuIn.emType = NET_RECORD_ACCESSCTLCARD;
	
//    SDKImport.FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam = {sizeof(stuParam)};
//    stuParam.bIsValid = true;
//    strcpy(stuParam.szCardNo, "123456");
//    strcpy(stuParam.szUserID, "4567890");
	
	
//    stuIn.pQueryCondition = &stuParam;
	
//    if (SDKImport.CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
//    {
//        lFinderId = stuOut.lFindeHandle;
//    }
//}

//void testRecordSetFindNext_Card(Int32 lFinderId)
//{
//    int i = 0, j = 0;
	
//    SDKImport.NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    stuIn.lFindeHandle = lFinderId;
//    stuIn.nFileCount = QUERY_COUNT;
	
//    SDKImport.NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
//    stuOut.nMaxRecordNum = stuIn.nFileCount;
	
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard[QUERY_COUNT] = {0};
//    for (i = 0; i < sizeof(stuCard)/sizeof(stuCard[0]); i++)
//    {
//        stuCard[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
//    }
//    stuOut.pRecordList = (void*)&stuCard[0];
	
//    if (SDKImport.CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
//    {
//        char szDoorTemp[QUERY_COUNT][MAX_NAME_LEN] = {0};
//        for (i = 0; i <  __min(QUERY_COUNT, stuOut.nRetRecordNum); i++)
//        {
//            NET_RECORDSET_ACCESS_CTL_CARD* pCard = (NET_RECORDSET_ACCESS_CTL_CARD*)stuOut.pRecordList;
//            for (j = 0; j < pCard[i].nDoorNum; j++)
//            {
//            }
//        }
		
//        for (j = 0; j < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); j++)
//        {
//        }
//    }
//}
////////////////////////////////////////////////////////////////////////////
////
//// Pwd
////
////////////////////////////////////////////////////////////////////////////
//void testRecordSetFind_Pwd(Int32 lLoginId, LLONG& lFinderId)
//{
//    SDKImport.NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    SDKImport.NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
//    stuIn.emType = NET_RECORD_ACCESSCTLPWD;
	
//    SDKImport.FIND_RECORD_ACCESSCTLPWD_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLPWD_CONDITION)};
//    strcpy(stuParam.szUserID, "1357924680");
	
//    stuIn.pQueryCondition = &stuParam;
	
//    if (SDKImport.CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
//    {
//        lFinderId = stuOut.lFindeHandle;
//    }
//}

//void testRecordSetFindNext_Pwd(Int32 lFinderId)
//{
//    int i = 0, j = 0;
	
//    SDKImport.NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    stuIn.lFindeHandle = lFinderId;
//    stuIn.nFileCount = QUERY_COUNT;
	
//    SDKImport.NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
//    stuOut.nMaxRecordNum = stuIn.nFileCount;
	
//    SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuPwd[QUERY_COUNT] = {0};
//    for (i = 0; i < sizeof(stuPwd)/sizeof(stuPwd[0]); i++)
//    {
//        stuPwd[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
//    }
//    stuOut.pRecordList = (void*)&stuPwd[0];
	
//    if (SDKImport.CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
//    {
//        char szDoorTemp[QUERY_COUNT][MAX_NAME_LEN] = {0};
//        for (i = 0; i <  __min(QUERY_COUNT, stuOut.nRetRecordNum); i++)
//        {
//            SDKImport.NET_RECORDSET_ACCESS_CTL_PWD* pPwd = (NET_RECORDSET_ACCESS_CTL_PWD*)stuOut.pRecordList;
//            for (j = 0; j < pPwd[i].nDoorNum; j++)
//            {
//                sprintf(szDoorTemp[i], "%s%d", szDoorTemp[i], pPwd[i].sznDoors[j]);
//            }
//        }
		
//        for (j = 0; j < stuOut.nRetRecordNum; j++)
//        {
//        }
//    }
//}
////////////////////////////////////////////////////////////////////////////
////
//// Rec
////
////////////////////////////////////////////////////////////////////////////
//void testRecordSetFind_CardRec(Int32 lLoginId, LLONG& lFinderId)
//{
//    SDKImport.NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    SDKImport.NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
//    stuIn.emType = NET_RECORD_ACCESSCTLCARDREC;
	
//    SDKImport.FIND_RECORD_ACCESSCTLCARDREC_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLCARDREC_CONDITION)};
//    strcpy(stuParam.szCardNo, "987654321");
//    stuParam.stStartTime.dwYear = 2013;
//    stuParam.stStartTime.dwMonth = 1;
//    stuParam.stStartTime.dwDay = 2;
//    stuParam.stStartTime.dwHour = 3;
//    stuParam.stStartTime.dwMinute = 4;
//    stuParam.stStartTime.dwSecond = 5;
//    stuParam.stEndTime.dwYear = 2014;
//    stuParam.stEndTime.dwMonth = 2;
//    stuParam.stEndTime.dwDay = 3;
//    stuParam.stEndTime.dwHour = 4;
//    stuParam.stEndTime.dwMinute = 5;
//    stuParam.stEndTime.dwSecond = 6;
	
//    stuIn.pQueryCondition = &stuParam;
	
//    if (SDKImport.CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
//    {
//        lFinderId = stuOut.lFindeHandle;
//    }
//}

//void testRecordSetFindNext_CardRec(Int32 lFinderId)
//{
//    SDKImport.NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    stuIn.lFindeHandle = lFinderId;
//    stuIn.nFileCount = QUERY_COUNT;
	
//    SDKImport.NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
//    stuOut.nMaxRecordNum = stuIn.nFileCount;
	
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec[QUERY_COUNT] = {0};
//    for (int i = 0; i < sizeof(stuCardRec)/sizeof(stuCardRec[0]); i++)
//    {
//        stuCardRec[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
//    }
//    stuOut.pRecordList = (void*)&stuCardRec[0];
	
//    if (SDKImport.CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
//    {
//        for (int j = 0; j < stuOut.nRetRecordNum; j++)
//        {
//        }
//    }	
//}
////////////////////////////////////////////////////////////////////////////
////
//// Holiday
////
////////////////////////////////////////////////////////////////////////////
//void testRecordSetFind_Holiday(Int32 lLoginId, LLONG& lFinderId)
//{
//    SDKImport.NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    SDKImport.NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
//    stuIn.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	

//    stuIn.pQueryCondition = NULL;
	
//    if (SDKImport.CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
//    {
//        lFinderId = stuOut.lFindeHandle;
//    }	
//}

//void testRecordSetFindNext_Holiday(Int32 lFinderId)
//{
//    SDKImport.NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
//    stuIn.lFindeHandle = lFinderId;
//    stuIn.nFileCount = QUERY_COUNT;
	
//    SDKImport.NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
//    stuOut.nMaxRecordNum = stuIn.nFileCount;
	
//    SDKImport.NET_RECORDSET_HOLIDAY stuHoliday[QUERY_COUNT] = {0};
//    for (int i = 0; i < sizeof(stuHoliday)/sizeof(stuHoliday[0]); i++)
//    {
//        stuHoliday[i].dwSize = sizeof(NET_RECORDSET_HOLIDAY);
//    }
//    stuOut.pRecordList = (void*)&stuHoliday[0];
	
//    if (SDKImport.CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
//    {
//        for (int j = 0; j < stuOut.nRetRecordNum; j++)
//        {
//        }
//    }		
//}

//void test_GetCountRecordSetFind(LLONG& lFinderId)
//{
//    SDKImport.NET_IN_QUEYT_RECORD_COUNT_PARAM stuIn = {sizeof(stuIn)};
//    SDKImport.NET_OUT_QUEYT_RECORD_COUNT_PARAM stuOut = {sizeof(stuOut)};
//    stuIn.lFindeHandle = lFinderId;
//    if (SDKImport.CLIENT_QueryRecordCount(&stuIn, &stuOut, SDK_API_WAITTIME))
//    {
//    }
//}

////////////////////////////////////////////////////////////////////////////
////
//// closeFind
////
////////////////////////////////////////////////////////////////////////////
//void test_RecordSetFindClose(Int32 lFinderId)
//{
//    if (SDKImport.CLIENT_FindRecordClose(lFinderId))
//    {
//    }
//}

//void testRecordSetFinder_Card(Int32 lLoginId)
//{
//    Int32 lFinderID = 0;
//    testRecordSetFind_Card(lLoginId, lFinderID);
//    if (lFinderID != 0)
//    {
//        testRecordSetFindNext_Card(lFinderID);
//        test_RecordSetFindClose(lFinderID);
//    }
//}

//void testRecordSetFinder_Pwd(Int32 lLoginId)
//{
//    Int32 lFinderID = 0;
//    testRecordSetFind_Pwd(lLoginId, lFinderID);
//    if (lFinderID != 0)
//    {
//        testRecordSetFindNext_Pwd(lFinderID);
//        test_RecordSetFindClose(lFinderID);
//    }
//}

//void testRecordSetFinder_CardRec(Int32 lLoginId)
//{
//    Int32 lFinderID = 0;
//    testRecordSetFind_CardRec(lLoginId, lFinderID);
//    if (lFinderID != 0)
//    {
//        testRecordSetFindNext_CardRec(lFinderID);
//        test_RecordSetFindClose(lFinderID);
//    }	
//}

//bool DevCtrl_GetRecordSetCount(Int32 lLoginID, int nRecordSetType)
//{
//    Int32 lFindID = 0;
//    switch (nRecordSetType)
//    {
//    case 1:
//        testRecordSetFind_Card(lLoginID, lFindID);
//        break;
//    case 2:
//        testRecordSetFind_Pwd(lLoginID, lFindID);
//        break;
//    case 3:
//        testRecordSetFind_CardRec(lLoginID, lFindID);
//        break;
//    case 4:
//        testRecordSetFind_Holiday(lLoginID, lFindID);
//        break;
//    default:
//        break;
//    }
//    if (NULL != lFindID)
//    {
//        int nInput = -1;
//        do 
//        {
//            test_GetCountRecordSetFind(lFindID);
//            std::cin >> nInput;
//        } while (0 != nInput);
//        test_RecordSetFindClose(lFindID);
//    }
//    return true;
//}

//bool DevCtrl_GetRecordSetInfo(Int32 lLoginID, int nRecordSetType)
//{
//    SDKImport.NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};
//    SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
//    SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
//    SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
//    switch (nRecordSetType)
//    {
//    case 1:
//        stuCard.nRecNo = g_nRecordNoCard < 0 ? 12 : g_nRecordNoCard;
//        stuInert.emType = NET_RECORD_ACCESSCTLCARD;
//        stuInert.pBuf = &stuCard;
//        break;
//    case 2:
//        stuAccessCtlPwd.nRecNo = g_nRecordNoPwd < 0 ? 12 : g_nRecordNoPwd;
//        stuInert.emType = NET_RECORD_ACCESSCTLPWD;
//        stuInert.pBuf = &stuAccessCtlPwd;
//        break;
//    case 3:
//        stuCardRec.nRecNo = g_nRecordNoCardRec < 0 ? 12 : g_nRecordNoCardRec;
//        stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
//        stuInert.pBuf = &stuCardRec;
//        break;
//    case 4:
//        stuHoliday.nRecNo = g_nRecordNoHoliday < 0 ? 12 : g_nRecordNoHoliday;
//        stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
//        stuInert.pBuf = &stuHoliday;
//        break;
//    default:
//        break;
//    }
//    int nRet = 0;
//    bool bRet = SDKImport.CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);
//    if (bRet)
//    {
//        switch (nRecordSetType)
//        {
//            case 1:
//                ShowCardInfo(stuCard);
//                break;
//            case 2:
//                ShowPwdInfo(stuAccessCtlPwd);
//                break;
//            case 3:
//                ShowCardRecInfo(stuCardRec);
//                break;
//            case 4:
//                ShowHolidayInfo(stuHoliday);
//                break;
//            default:
//                break;
//        }
//    }

//    return true;
//}

//bool DevCtrl_CloseDoor(Int32 lLoginId)
//{
//    SDKImport.NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
//    stuParam.nChannelID = 12;
//    bool bResult = SDKImport.CLIENT_ControlDevice(lLoginId, DH_CTRL_ACCESS_CLOSE, &stuParam, SDK_API_WAITTIME);
//    return true;
//}

//void DevState_DoorStatus(Int32 lLoginId)
//{
//    SDKImport.NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
//    stuParam.nChannel = 14;
//    int nRetLen = 0;
//    bool bResult = SDKImport.CLIENT_QueryDevState(lLoginId, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, SDK_API_WAITTIME);
//    if (bResult)
//    {
//        switch (stuParam.emStateType)
//        {
//        case EM_NET_DOOR_STATUS_OPEN:
//            break;
//        case EM_NET_DOOR_STATUS_CLOSE:
//            break;
//        default:
//            break;
//        }
//    }
//}

//    }
//}