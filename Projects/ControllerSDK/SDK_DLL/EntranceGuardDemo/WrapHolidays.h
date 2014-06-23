#if !defined(__WRAP_HOLYDAYS_H__)
#define __WRAP_HOLYDAYS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Holiday(int lLoginID, NET_RECORDSET_HOLIDAY* stuHoliday);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Holiday(int lLoginID, NET_RECORDSET_HOLIDAY* stuHoliday);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetHolidayInfo(int lLoginID, int nRecordNo, NET_RECORDSET_HOLIDAY* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveHoliday(int lLoginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAllHolidays(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_HolidaysCount(int lLoginID);

#endif // !defined(__WRAP_HOLYDAYS_H__)