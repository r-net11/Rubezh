#if !defined(__WRAP_HOLYDAYS_H__)
#define __WRAP_HOLYDAYS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Holiday(int loginID, NET_RECORDSET_HOLIDAY* stuHoliday);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Holiday(int loginID, NET_RECORDSET_HOLIDAY* stuHoliday);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Remove_Holiday(int loginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAll_Holidays(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_Holiday_Info(int loginID, int nRecordNo, NET_RECORDSET_HOLIDAY* result);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_Holidays_Count(int loginID);

typedef struct tagNET_HolidaysCollection
{
	int Count;
	NET_RECORDSET_HOLIDAY Holidays[10];
}HolidaysCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAll_Holidays(int loginID, HolidaysCollection* result);

#endif // !defined(__WRAP_HOLYDAYS_H__)