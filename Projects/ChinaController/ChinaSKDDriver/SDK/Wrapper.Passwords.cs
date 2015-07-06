﻿using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddPassword(Password password)
		{
			var nativePassword = PasswordToNativePassword(password);
			var result = NativeWrapper.WRAP_Insert_Password(LoginID, ref nativePassword);
			return result;
		}

		public bool EditPassword(Password password)
		{
			var nativePassword = PasswordToNativePassword(password);
			var result = NativeWrapper.WRAP_Update_Password(LoginID, ref nativePassword);
			return result;
		}

		public bool RemovePassword(int index)
		{
			var result = NativeWrapper.WRAP_Remove_Password(LoginID, index);
			return result;
		}

		public bool RemoveAllPasswords()
		{
			var result = NativeWrapper.WRAP_RemoveAll_Passwords(LoginID);
			return result;
		}

		public Password GetPasswordInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_Get_Password_Info(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD nativePassword = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var password = NativePasswordToPassword(nativePassword);
			return password;
		}

		public List<Password> GetAllPasswords()
		{
			var resultPasswords = new List<Password>();
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Passwords(LoginID, ref finderID);

			if (finderID > 0)
			{
				while (true)
				{
					int structSize = Marshal.SizeOf(typeof(NativeWrapper.PasswordsCollection));
					IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

					var result = NativeWrapper.WRAP_GetAll_Passwords(finderID, intPtr);

					NativeWrapper.PasswordsCollection passwordsCollection = (NativeWrapper.PasswordsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.PasswordsCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					var passwords = new List<Password>();
					for (int i = 0; i < Math.Min(passwordsCollection.Count, 10); i++)
					{
						var nativePassword = passwordsCollection.Passwords[i];
						if (nativePassword.nRecNo > 0)
						{
							var password = NativePasswordToPassword(nativePassword);
							passwords.Add(password);
						}
					}
					if (result == 0)
						break;
					resultPasswords.AddRange(passwords);
				}

				NativeWrapper.WRAP_EndGetAll(finderID);
			}

			return resultPasswords;
		}

		public int GetPasswordsCount()
		{
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Passwords(LoginID, ref finderID);

			if (finderID > 0)
			{
				var result = NativeWrapper.WRAP_GetAllCount(finderID);
				NativeWrapper.WRAP_EndGetAll(finderID);
				return result;
			}

			return -1;
		}

		private NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD PasswordToNativePassword(Password password)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD nativePassword = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD();
			nativePassword.stuCreateTime.dwYear = password.CreationDateTime.Year;
			nativePassword.stuCreateTime.dwMonth = password.CreationDateTime.Month;
			nativePassword.stuCreateTime.dwDay = password.CreationDateTime.Day;
			nativePassword.stuCreateTime.dwHour = password.CreationDateTime.Hour;
			nativePassword.stuCreateTime.dwMinute = password.CreationDateTime.Minute;
			nativePassword.stuCreateTime.dwSecond = password.CreationDateTime.Second;
			nativePassword.szUserID = password.UserID;
			nativePassword.szDoorOpenPwd = password.DoorOpenPassword;
			nativePassword.szAlarmPwd = password.AlarmPassword;
			nativePassword.nDoorNum = password.DoorsCount;
			nativePassword.sznDoors = new int[32];
			for (int i = 0; i < password.Doors.Count; i++)
			{
				nativePassword.sznDoors[i] = password.Doors[i];
			}
			return nativePassword;
		}

		private Password NativePasswordToPassword(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD nativePassword)
		{
			var password = new Password();
			password.RecordNo = nativePassword.nRecNo;
			password.CreationDateTime = NET_TIMEToDateTime(nativePassword.stuCreateTime);
			password.UserID = nativePassword.szUserID;
			password.DoorOpenPassword = nativePassword.szDoorOpenPwd;
			password.AlarmPassword = nativePassword.szAlarmPwd;
			password.DoorsCount = nativePassword.nDoorNum;
			password.Doors = nativePassword.sznDoors.ToList();
			return password;
		}
	}
}