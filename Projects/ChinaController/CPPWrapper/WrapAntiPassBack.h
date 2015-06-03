#if !defined(__WRAPANTIPASSBACK_H__)
#define __WRAPANTIPASSBACK_H__

#define ANTIPASSBACK_MODES_COUNT 3

#define ANTIPASSBACK_MODE_R1R2 0
#define ANTIPASSBACK_MODE_R1R3R2R4 1
#define ANTIPASSBACK_MODE_R3R4 2

typedef enum
{
	R1R2 = 0,
	R1R3R2R4 = 1,
	R3R4 = 2
} WRAP_AntiPassBackMode;

typedef struct
{
	WRAP_AntiPassBackMode AntiPassBackMode; // ����� Anti-pass Back
	BOOL bIsAvailable; // ����������� ������ Anti-pass Back
} WRAP_AntiPassBackModeAvailability;

typedef struct
{
	int nDoorsCount; // ���������� ������ �� �����������
	BOOL bCanActivate; // ����������� ��������� Anti-pass Back
	BOOL bIsActivated; // Anti-pass Back �����������?
	WRAP_AntiPassBackModeAvailability AvailableAntiPassBackModes[ANTIPASSBACK_MODES_COUNT]; // ����������� ������� Anti-pass Back
	WRAP_AntiPassBackMode CurrentAntiPassBackMode; // ������� ����� Anti-pass Back
} WRAP_AntiPassBackCfg;

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_GetAntiPassBackCfg(int loginID, WRAP_AntiPassBackCfg* result);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_SetAntiPassBackCfg(int loginID, WRAP_AntiPassBackCfg* cfg);

#endif // !defined(__WRAPANTIPASSBACK_H__)