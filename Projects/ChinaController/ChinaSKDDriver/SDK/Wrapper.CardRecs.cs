using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddCardRec(CardRec cardRec)
		{
			var nativeCardRec = CardRecToNativeCardRec(cardRec);
			var result = NativeWrapper.WRAP_Insert_CardRec(LoginID, ref nativeCardRec);
			return result;
		}

		public bool EditCardRec(CardRec cardRec)
		{
			var nativeCardRec = CardRecToNativeCardRec(cardRec);
			var result = NativeWrapper.WRAP_Update_CardRec(LoginID, ref nativeCardRec);
			return result;
		}

		public CardRec GetCardRecInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_Get_CardRec_Info(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cardRec = NativeCardRecToCardRec(nativeCardRec);
			return cardRec;
		}

		public bool RemoveCardRec(int index)
		{
			var result = NativeWrapper.WRAP_Remove_CardRec(LoginID, index);
			return result;
		}

		public bool RemoveAllCardRecs()
		{
			var result = NativeWrapper.WRAP_RemoveAll_CardRecs(LoginID);
			return result;
		}

		public List<CardRec> GetAllCardRecs()
		{
			var resultCardRecs = new List<CardRec>();
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_CardRecs(LoginID, ref finderID);

			if (finderID > 0)
			{
				while (true)
				{
					int structSize = Marshal.SizeOf(typeof(NativeWrapper.CardRecsCollection));
					IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

					var result = NativeWrapper.WRAP_GetAll_CardRecs(finderID, intPtr);

					NativeWrapper.CardRecsCollection cardRecsCollection = (NativeWrapper.CardRecsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CardRecsCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					var cardRecs = new List<CardRec>();
					for (int i = 0; i < Math.Min(cardRecsCollection.Count, 10); i++)
					{
						var nativeCardRec = cardRecsCollection.CardRecs[i];
						if (nativeCardRec.nRecNo > 0)
						{
							var cardRec = NativeCardRecToCardRec(nativeCardRec);
							cardRecs.Add(cardRec);
						}
					}
					if (result == 0)
						break;
					resultCardRecs.AddRange(cardRecs);
				}

				NativeWrapper.WRAP_EndGetAll(finderID);
			}

			return resultCardRecs;
		}

		public int GetCardRecsCount()
		{
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_CardRecs(LoginID, ref finderID);

			if (finderID > 0)
			{
				var result = NativeWrapper.WRAP_GetAllCount(finderID);
				NativeWrapper.WRAP_EndGetAll(finderID);
				return result;
			}

			return -1;
		}

		NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC CardRecToNativeCardRec(CardRec cardRec)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC();
			nativeCardRec.szCardNo = cardRec.CardNo;
			nativeCardRec.szPwd = cardRec.Password;

			nativeCardRec.stuTime.dwYear = cardRec.DateTime.Year;
			nativeCardRec.stuTime.dwMonth = cardRec.DateTime.Month;
			nativeCardRec.stuTime.dwDay = cardRec.DateTime.Day;
			nativeCardRec.stuTime.dwHour = cardRec.DateTime.Hour;
			nativeCardRec.stuTime.dwMinute = cardRec.DateTime.Minute;
			nativeCardRec.stuTime.dwSecond = cardRec.DateTime.Second;

			nativeCardRec.bStatus = cardRec.IsStatus;
			nativeCardRec.emMethod = (ChinaSKDDriverNativeApi.NativeWrapper.NET_ACCESS_DOOROPEN_METHOD)cardRec.DoorOpenMethod;
			nativeCardRec.nDoor = cardRec.DoorNo;
			return nativeCardRec;
		}

		CardRec NativeCardRecToCardRec(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec)
		{
			var cardRec = new CardRec();
			cardRec.RecordNo = nativeCardRec.nRecNo;
			cardRec.CardNo = nativeCardRec.szCardNo;
			cardRec.Password = nativeCardRec.szPwd;
			cardRec.DateTime = NET_TIMEToDateTime(nativeCardRec.stuTime);
			cardRec.IsStatus = nativeCardRec.bStatus;
			cardRec.DoorOpenMethod = (CardRecDoorOpenMethod)nativeCardRec.emMethod;
			cardRec.DoorNo = nativeCardRec.nDoor;
			return cardRec;
		}
	}
}