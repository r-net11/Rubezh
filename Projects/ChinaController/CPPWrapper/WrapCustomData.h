#if !defined(__WRAPCUSTOMDATA_H__)
#define __WRAPCUSTOMDATA_H__

#define MAX_CUSTOMDATA_LENGTH 128

typedef struct tagWRAP_CustomData
{
	// ����������� ����� ���������������� ������ (�� ����� MAX_CUSTOMDATA_LENGTH ��������)
	int nCustomDataLength;
	// ���������������� ������ (�� ����� MAX_CUSTOMDATA_LENGTH ��������)
	char szCustomData[MAX_CUSTOMDATA_LENGTH];
} WRAP_CustomData;

extern "C" SDK_CLIENT_API BOOL CALL_METHOD WRAP_GetCustomData(int loginID, WRAP_CustomData* result);
extern "C" SDK_CLIENT_API BOOL CALL_METHOD WRAP_SetCustomData(int loginID, WRAP_CustomData* result);

#endif // !defined(__WRAPCUSTOMDATA_H__)