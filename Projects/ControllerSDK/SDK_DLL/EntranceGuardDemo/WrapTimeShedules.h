#if !defined(__WRAP_TIMESHEDULES_H__)
#define __WRAP_TIMESHEDULES_H__

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetDevConfig_AccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDevConfig_AccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* stuInfo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo);

#endif // !defined(__WRAP_TIMESHEDULES_H__)