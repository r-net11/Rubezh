#if !defined(__WRAP_HOLYDAYS_H__)
#define __WRAP_HOLYDAYS_H__

extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_Insert_Holiday(int loginID, NET_RECORDSET_HOLIDAY* param);

extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_Update_Holiday(int loginID, NET_RECORDSET_HOLIDAY* param);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_Remove_Holiday(int loginID, int recordNo);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_RemoveAll_Holidays(int loginID);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_Get_Holiday_Info(int loginID, int recordNo, NET_RECORDSET_HOLIDAY* result);

extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_Get_Holidays_Count(int loginID);

typedef struct tagNET_HolidaysCollection
{
	int Count;
	NET_RECORDSET_HOLIDAY Holidays[10];
}HolidaysCollection;

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_GetAll_Holidays(int loginID, HolidaysCollection* result);

#endif // !defined(__WRAP_HOLYDAYS_H__)