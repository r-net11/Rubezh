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
		/// Получает список всех событий доступа, записанных на контроллер
		/// </summary>
		/// <returns>список событий доступа</returns>
		public List<AccessLogItem> GetAllAccessLogItems()
		{
			var resultAccesses = new List<AccessLogItem>();
			var finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Accesses(LoginID, ref finderID);

			if (finderID > 0)
			{
				while (true)
				{
					var structSize = Marshal.SizeOf(typeof(NativeWrapper.AccessesCollection));
					var intPtr = Marshal.AllocCoTaskMem(structSize);

					var result = NativeWrapper.WRAP_GetAll_Accesses(finderID, intPtr);

					var accessesCollection = (NativeWrapper.AccessesCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.AccessesCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					var accesses = new List<AccessLogItem>();
					for (var i = 0; i < Math.Min(accessesCollection.Count, 10); i++)
					{
						var nativeAccess = accessesCollection.Accesses[i];
						//if (nativeAccess.nRecNo > 0)
						//{
						var access = NativeAccessToAccessLogItem(nativeAccess);
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

		public int GetAccessLogItemsCount()
		{
			var finderID = 0;
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
		/// полученную из враппера SDK, в объет типа AccessLogItem
		/// </summary>
		/// <param name="nativeAccess">структура NET_RECORDSET_ACCESS_CTL_CARDREC</param>
		/// <returns>объект типа AccessLogItem</returns>
		private AccessLogItem NativeAccessToAccessLogItem(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeAccess)
		{
			var access = new AccessLogItem
			{
				RecordNo = nativeAccess.nRecNo,
				CardNo = nativeAccess.szCardNo,
				Password = nativeAccess.szPwd,
				Time = NET_TIMEToDateTime(nativeAccess.stuTime),
				MethodType = (AccessMethodType) nativeAccess.emMethod,
				Status = nativeAccess.bStatus,
				ReaderID = nativeAccess.nReaderID,
				DoorNo = nativeAccess.nDoor
			};

			return access;
		}
	}
}