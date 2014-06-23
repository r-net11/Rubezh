#if !defined(__WRAP_TIMESHEDULES_H__)
#define __WRAP_TIMESHEDULES_H__

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetTimeSchedule(int lLoginId, int index, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetTimeSchedule(int lLoginId, int index, CFG_ACCESS_TIMESCHEDULE_INFO* stuInfo);

#endif // !defined(__WRAP_TIMESHEDULES_H__)