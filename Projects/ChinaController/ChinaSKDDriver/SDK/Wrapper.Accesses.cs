using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		/// <summary>
		/// Получает список всех карт, записанных на контроллер
		/// </summary>
		/// <returns>списко карт</returns>
		public List<Access> GetAllAccesses()
		{
			var resultAccesses = new List<Access>();
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Accesses(LoginID, ref finderID);

			if (finderID > 0)
			{
				while (true)
				{
					int structSize = Marshal.SizeOf(typeof(NativeWrapper.AccessesCollection));
					IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

					var result = NativeWrapper.WRAP_GetAll_Accesses(finderID, intPtr);

					NativeWrapper.AccessesCollection accessesCollection = (NativeWrapper.AccessesCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.AccessesCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					var accesses = new List<Access>();
					for (int i = 0; i < Math.Min(accessesCollection.Count, 10); i++)
					{
						var nativeAccess = accessesCollection.Accesses[i];
						//if (nativeAccess.nRecNo > 0)
						//{
						var access = NativeAccessToAccess(nativeAccess);
						accesses.Add(access);
						//}
					}
					if (result == 0)
						break;
					resultAccesses.AddRange(accesses);
				}

				NativeWrapper.WRAP_EndGetAll(finderID);
			}

			return resultAccesses;
		}

		public int GetAccessesCount()
		{
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Accesses(LoginID, ref finderID);

			if (finderID > 0)
			{
				var result = NativeWrapper.WRAP_GetAllCount(finderID);
				NativeWrapper.WRAP_EndGetAll(finderID);
				return result;
			}

			return -1;
		}

		/// <summary>
		/// Конвертирует структуру NET_RECORDSET_ACCESS_CTL_CARDREC,
		/// полученную из враппера SDK, в объет типа Access
		/// </summary>
		/// <param name="nativeAccess">структура NET_RECORDSET_ACCESS_CTL_CARDREC</param>
		/// <returns>объект типа Access</returns>
		private Access NativeAccessToAccess(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeAccess)
		{
			var access = new Access();

			access.RecordNo = nativeAccess.nRecNo;
			access.CardNo = nativeAccess.szCardNo;
			access.Password = nativeAccess.szPwd;
			access.Time = NET_TIMEToDateTime(nativeAccess.stuTime);
			access.MethodType = (AccessMethodType)nativeAccess.emMethod;
			access.Status = nativeAccess.bStatus;
			access.ReaderID = nativeAccess.nReaderID;
			access.DoorNo = nativeAccess.nDoor;

			return access;
		}
	}
}