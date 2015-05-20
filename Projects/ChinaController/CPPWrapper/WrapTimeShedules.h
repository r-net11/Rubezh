#if !defined(__WRAP_TIMESHEDULES_H__)
#define __WRAP_TIMESHEDULES_H__

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_GetTimeSchedule(int loginID, int index, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_SetTimeSchedule(int loginID, int index, CFG_ACCESS_TIMESCHEDULE_INFO* param);

#endif // !defined(__WRAP_TIMESHEDULES_H__)