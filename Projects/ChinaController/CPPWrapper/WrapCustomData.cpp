#include "StdAfx.h"
#include "WrapCustomData.h"

BOOL CALL_METHOD WRAP_GetCustomData(int loginID, WRAP_CustomData* result)
{
	if (loginID == NULL)
	{
		return FALSE;
	}

	CFG_CLIENT_CUSTOM_INFO	stuInfo = { 0 };

	char szJsonBuf[1024 * 40] = { 0 };
	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig(loginID, CFG_CMD_CLIENT_CUSTOM_DATA, -1, szJsonBuf, 1024 * 40, &nerror, 5000);

	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_CLIENT_CUSTOM_DATA, szJsonBuf, (void*)&stuInfo, sizeof(stuInfo), &dwRetLen);
		if (!bRet)
		{
			return FALSE;
		}
		else
		{
			if (stuInfo.abBinary)
			{
				WRAP_CustomData customData = { 0 };
				for (int i = 0; i < stuInfo.nBinaryNum; i++)
				{
					customData.szCustomData[i] = (char)stuInfo.dwBinary[i];
				}
				customData.nCustomDataLength = stuInfo.nBinaryNum;
				memcpy(result, &customData, sizeof(customData));
			}
		}
	}
	else
	{
		return FALSE;
	}

	return TRUE;
}

BOOL CALL_METHOD WRAP_SetCustomData(int loginID, WRAP_CustomData* result)
{
	if (loginID == NULL)
	{
		return FALSE;
	}

	CFG_CLIENT_CUSTOM_INFO	stuInfo = { 0 };
	for (int i = 0; i < result->nCustomDataLength; i++)
	{
		int asciiValue = result->szCustomData[i];
		stuInfo.dwBinary[i] = asciiValue;
	}
	stuInfo.abBinary = true;
	stuInfo.nBinaryNum = result->nCustomDataLength;

	char szJsonBufSet[1024 * 40] = { 0 };

	BOOL bRet = CLIENT_PacketData(CFG_CMD_CLIENT_CUSTOM_DATA, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));
	if (!bRet)
	{
		return FALSE;
	}
	else
	{

		int nChannel = -1;
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(loginID, CFG_CMD_CLIENT_CUSTOM_DATA, nChannel, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		if (!bRet)
		{
			return FALSE;
		}
		else
		{
			return TRUE;
		}
	}
}
