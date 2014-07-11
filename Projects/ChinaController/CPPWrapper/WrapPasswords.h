#if !defined(__WRAP_PASSWORDS_H__)
#define __WRAP_PASSWORDS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Password(int loginID, NET_RECORDSET_ACCESS_CTL_PWD* param);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Password(int loginID, NET_RECORDSET_ACCESS_CTL_PWD* param);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Remove_Password(int loginID, int recordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAll_Passwords(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_Password_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_PWD* result);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_Passwords_Count(int loginID);

typedef struct tagNET_PasswordsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_PWD Passwords[10];
}PasswordsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAll_Passwords(int loginID, PasswordsCollection* result);

#endif // !defined(__WRAP_PASSWORDS_H__)