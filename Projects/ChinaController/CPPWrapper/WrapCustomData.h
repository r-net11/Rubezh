#if !defined(__WRAPCUSTOMDATA_H__)
#define __WRAPCUSTOMDATA_H__

#define MAX_CUSTOMDATA_LENGTH 128

typedef struct tagWRAP_CustomData
{
	// Фактическая длина пользовательских данных (не более MAX_CUSTOMDATA_LENGTH символов)
	int nCustomDataLength;
	// Пользовательские данные (не более MAX_CUSTOMDATA_LENGTH символов)
	char szCustomData[MAX_CUSTOMDATA_LENGTH];
} WRAP_CustomData;

extern "C" SDK_CLIENT_API BOOL CALL_METHOD WRAP_GetCustomData(int loginID, WRAP_CustomData* result);
extern "C" SDK_CLIENT_API BOOL CALL_METHOD WRAP_SetCustomData(int loginID, WRAP_CustomData* result);

#endif // !defined(__WRAPCUSTOMDATA_H__)