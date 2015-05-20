#if !defined(__WRAP_PASSWORDS_H__)
#define __WRAP_PASSWORDS_H__

extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_Insert_Password(int loginID, NET_RECORDSET_ACCESS_CTL_PWD* param);

extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_Update_Password(int loginID, NET_RECORDSET_ACCESS_CTL_PWD* param);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_Remove_Password(int loginID, int recordNo);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_RemoveAll_Passwords(int loginID);

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_Get_Password_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_PWD* result);

typedef struct tagNET_PasswordsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_PWD Passwords[10];
}PasswordsCollection;

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_BeginGetAll_Passwords(int loginID, int& finderID);
extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_GetAll_Passwords(int finderID, PasswordsCollection* result);

#endif // !defined(__WRAP_PASSWORDS_H__)