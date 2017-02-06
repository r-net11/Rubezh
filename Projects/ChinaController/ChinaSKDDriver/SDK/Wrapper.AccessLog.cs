using StrazhAPI.Journal;
using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StrazhDeviceSDK
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
					if (result == 0)
						break;

					var accessesCollection = (NativeWrapper.AccessesCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.AccessesCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					for (var i = 0; i < Math.Min(accessesCollection.Count, 10); i++)
					{
						var access = NativeAccessToAccessLogItem(accessesCollection.Accesses[i]);
						resultAccesses.Add(access);
					}
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
		private static AccessLogItem NativeAccessToAccessLogItem(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeAccess)
		{
			var access = new AccessLogItem
			{
				RecordNo = nativeAccess.nRecNo,
				CardNo = nativeAccess.szCardNo,
				Password = nativeAccess.szPwd,
				Time = NET_TIMEToDateTime(nativeAccess.stuTime),
				MethodType = (AccessMethodType)nativeAccess.emMethod,
				Status = nativeAccess.bStatus,
				ReaderID = nativeAccess.szReaderID,
				DoorNo = nativeAccess.nDoor,
				CardType = (CardType)nativeAccess.emCardType,
				ErrorCode = (ErrorCode)nativeAccess.nErrorCode
			};

			return access;
		}


		/// <summary>
		/// Получает список событий доступа, записанных на контроллер, возраст которых превышает указанную дату
		/// </summary>
		/// <param name="dateTime">Дата</param>
		/// <returns>Список событий доступа</returns>
		public List<AccessLogItem> GetAccessLogItemsOlderThan(DateTime dateTime)
		{
			var resultAccesses = new List<AccessLogItem>();
			var finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Accesses(LoginID, ref finderID);

			if (finderID > 0)
			{
				var continueSearch = true;
				while (continueSearch)
				{
					var structSize = Marshal.SizeOf(typeof(NativeWrapper.AccessesCollection));
					var intPtr = Marshal.AllocCoTaskMem(structSize);

					var result = NativeWrapper.WRAP_GetAll_Accesses(finderID, intPtr);
					if (result == 0)
						break;

					var accessesCollection = (NativeWrapper.AccessesCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.AccessesCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					for (var i = 0; i < Math.Min(accessesCollection.Count, 10); i++)
					{
						var access = NativeAccessToAccessLogItem(accessesCollection.Accesses[i]);
						if (access.Time > dateTime)
							resultAccesses.Add(access);
						else
						{
							continueSearch = false;
							break;
						}
					}
				}

				NativeWrapper.WRAP_EndGetAll(finderID);
			}

			return resultAccesses;
		}

		public SKDJournalItem NativeAccessToSKDJournalItem(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeAccess)
		{
			var journalItem = new SKDJournalItem();
			journalItem.JournalItemType = JournalItemType.Offline;
			journalItem.LoginID = LoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = NET_TIMEToDateTime(nativeAccess.stuTime);
			journalItem.JournalEventNameType = nativeAccess.bStatus ? JournalEventNameType.Проход_разрешен : JournalEventNameType.Проход_запрещен;
			journalItem.CardNo = nativeAccess.szCardNo;
			journalItem.DoorNo = nativeAccess.nDoor;
			journalItem.bStatus = nativeAccess.bStatus;
			journalItem.emCardType = nativeAccess.emCardType;
			journalItem.emOpenMethod = nativeAccess.emMethod;
			journalItem.szPwd = nativeAccess.szPwd;
			journalItem.szReaderID = nativeAccess.szReaderID;
			journalItem.ErrorCode = (ErrorCode)nativeAccess.nErrorCode;
			journalItem.No = nativeAccess.nRecNo;
			return journalItem;
		}
	}
}