#if !defined(__WRAP_TIMESHEDULES_H__)
#define __WRAP_TIMESHEDULES_H__

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetTimeSchedule(int loginID, int index, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetTimeSchedule(int loginID, int index, CFG_ACCESS_TIMESCHEDULE_INFO* param);

#endif // !defined(__WRAP_TIMESHEDULES_H__)