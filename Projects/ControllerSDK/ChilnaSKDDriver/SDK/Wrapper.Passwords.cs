using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddPassword(Password password)
		{
			var nativePassword = PasswordToNativePassword(password);
			var result = NativeWrapper.WRAP_Insert_Pwd(LoginID, ref nativePassword);
			return result;
		}

		public bool EditPassword(Password password)
		{
			var nativePassword = PasswordToNativePassword(password);
			var result = NativeWrapper.WRAP_Update_Pwd(LoginID, ref nativePassword);
			return result;
		}

		public bool RemovePassword(int index)
		{
			var result = NativeWrapper.WRAP_RemovePassword(LoginID, index);
			return result;
		}

		public bool RemoveAllPasswords()
		{
			var result = NativeWrapper.WRAP_RemoveAllPasswords(LoginID);
			return result;
		}

		public Password GetPasswordInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetPasswordInfo(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD nativePassword = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var password = NativePasswordToPassword(nativePassword);
			return password;
		}

		public int GetPasswordsCount()
		{
			var passwordsCount = NativeWrapper.WRAP_Get_PasswordsCount(LoginID);
			return passwordsCount;
		}

		public List<Password> GetAllPasswords()
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.PasswordsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetAllPasswords(LoginID, intPtr);

			NativeWrapper.PasswordsCollection passwordsCollection = (NativeWrapper.PasswordsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.PasswordsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var passwords = new List<Password>();
			for (int i = 0; i < Math.Min(passwordsCollection.Count, 500); i++)
			{
				var nativePassword = passwordsCollection.Passwords[i];
				var password = NativePasswordToPassword(nativePassword);
				passwords.Add(password);
			}
			return passwords;
		}

		NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD PasswordToNativePassword(Password password)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD nativePassword = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD();
			nativePassword.stuCreateTime.dwYear = password.CreationDateTime.Year;
			nativePassword.stuCreateTime.dwMonth = password.CreationDateTime.Month;
			nativePassword.stuCreateTime.dwDay = password.CreationDateTime.Day;
			nativePassword.stuCreateTime.dwHour = password.CreationDateTime.Hour;
			nativePassword.stuCreateTime.dwMinute = password.CreationDateTime.Minute;
			nativePassword.stuCreateTime.dwSecond = password.CreationDateTime.Second;
			nativePassword.szUserID = StringToCharArray(password.UserID, 32);
			nativePassword.szDoorOpenPwd = StringToCharArray(password.DoorOpenPassword, 64);
			nativePassword.szAlarmPwd = StringToCharArray(password.AlarmPassword, 64);
			nativePassword.nDoorNum = password.DoorsCount;
			nativePassword.sznDoors = new int[32];
			for (int i = 0; i < password.Doors.Count; i++)
			{
				nativePassword.sznDoors[i] = password.Doors[i];
			}
			return nativePassword;
		}

		Password NativePasswordToPassword(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD nativePassword)
		{
			var password = new Password();
			password.RecordNo = nativePassword.nRecNo;
			password.CreationDateTime = NET_TIMEToDateTime(nativePassword.stuCreateTime);
			password.UserID = CharArrayToString(nativePassword.szUserID);
			password.DoorOpenPassword = CharArrayToString(nativePassword.szDoorOpenPwd);
			password.AlarmPassword = CharArrayToString(nativePassword.szAlarmPwd);
			password.DoorsCount = nativePassword.nDoorNum;
			password.Doors = nativePassword.sznDoors.ToList();
			return password;
		}
	}
}