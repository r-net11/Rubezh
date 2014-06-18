#if !defined(__WRAP_PASSWORDS_H__)
#define __WRAP_PASSWORDS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Pwd(int lLoginID, NET_RECORDSET_ACCESS_CTL_PWD* stuAccessCtlPwd);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Pwd(int lLoginID, NET_RECORDSET_ACCESS_CTL_PWD* stuAccessCtlPwd);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetPasswordInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_PWD* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemovePassword(int lLoginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAllPasswords(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_PasswordsCount(int lLoginID);

typedef struct tagNET_PasswordsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_PWD Passwords[1000];
}PasswordsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllPasswords(int lLoginId, PasswordsCollection* result);

#endif // !defined(__WRAP_PASSWORDS_H__)