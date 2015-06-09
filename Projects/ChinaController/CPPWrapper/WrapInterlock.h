#if !defined(__WRAPINTERLOCK_H__)
#define __WRAPINTERLOCK_H__

#define INTERLOCK_MODES_COUNT 7

#define INTERLOCK_MODE_L1L2			0
#define INTERLOCK_MODE_L1L2L3		1
#define INTERLOCK_MODE_L1L2L3L4		2
#define INTERLOCK_MODE_L2L3L4		3
#define INTERLOCK_MODE_L1L3_L2L4	4
#define INTERLOCK_MODE_L1L4_L2L3	5
#define INTERLOCK_MODE_L3L4			6

typedef enum
{
	L1L2 = 0,
	L1L2L3 = 1,
	L1L2L3L4 = 2,
	L2L3L4 = 3,
	L1L3_L2L4 = 4,
	L1L4_L2L3 = 5,
	L3L4 = 6
} WRAP_InterlockMode;

typedef struct
{
	WRAP_InterlockMode InterlockMode; // Режим Interlock
	BOOL bIsAvailable; // Доступность режима Interlock
} WRAP_InterlockModeAvailability;

typedef struct
{
	int nDoorsCount; // Количество дверей на контроллере
	BOOL bCanActivate; // Возможность активации Interlock
	BOOL bIsActivated; // Interlock активирован?
	WRAP_InterlockModeAvailability AvailableInterlockModes[INTERLOCK_MODES_COUNT]; // Доступность режимов Interlock
	WRAP_InterlockMode CurrentInterlockMode; // Текущий режим Interlock
} WRAP_InterlockCfg;

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_GetInterlockCfg(int loginID, WRAP_InterlockCfg* result);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_SetInterlockCfg(int loginID, WRAP_InterlockCfg* cfg);

#endif // !defined(__WRAPINTERLOCK_H__)