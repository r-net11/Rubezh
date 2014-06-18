using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public static partial class Wrapper
	{
		public static int AddCard(int loginID, Card card)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARD();
			stuCard.bIsValid = card.IsValid;
			stuCard.emStatus = card.CardStatus;
			stuCard.emType = card.CardType;
			stuCard.nTimeSectionNum = card.TimeSectionsCount;
			stuCard.nUserTime = card.UserTime;

			stuCard.stuCreateTime.dwYear = card.CreationDateTime.Year;
			stuCard.stuCreateTime.dwMonth = card.CreationDateTime.Month;
			stuCard.stuCreateTime.dwDay = card.CreationDateTime.Day;
			stuCard.stuCreateTime.dwHour = card.CreationDateTime.Hour;
			stuCard.stuCreateTime.dwMinute = card.CreationDateTime.Minute;
			stuCard.stuCreateTime.dwSecond = card.CreationDateTime.Second;

			stuCard.stuValidStartTime.dwYear = card.ValidStartDateTime.Year;
			stuCard.stuValidStartTime.dwMonth = card.ValidStartDateTime.Month;
			stuCard.stuValidStartTime.dwDay = card.ValidStartDateTime.Day;
			stuCard.stuValidStartTime.dwHour = card.ValidStartDateTime.Hour;
			stuCard.stuValidStartTime.dwMinute = card.ValidStartDateTime.Minute;
			stuCard.stuValidStartTime.dwSecond = card.ValidStartDateTime.Second;

			stuCard.stuValidEndTime.dwYear = card.ValidEndDateTime.Year;
			stuCard.stuValidEndTime.dwMonth = card.ValidEndDateTime.Month;
			stuCard.stuValidEndTime.dwDay = card.ValidEndDateTime.Day;
			stuCard.stuValidEndTime.dwHour = card.ValidEndDateTime.Hour;
			stuCard.stuValidEndTime.dwMinute = card.ValidEndDateTime.Minute;
			stuCard.stuValidEndTime.dwSecond = card.ValidEndDateTime.Second;

			stuCard.szCardNo = StringToCharArray(card.CardNo, 32);
			stuCard.nDoorNum = card.DoorsCount;
			stuCard.sznDoors = new int[32];
			stuCard.sznDoors[0] = 1;
			stuCard.sznDoors[1] = 2;
			stuCard.nTimeSectionNum = card.TimeSectionsCount;
			stuCard.sznTimeSectionNo = new int[32];
			stuCard.sznTimeSectionNo[0] = 1;
			stuCard.sznTimeSectionNo[1] = 2;
			stuCard.szPsw = Wrapper.StringToCharArray(card.Password, 64);
			stuCard.szUserID = Wrapper.StringToCharArray(card.UserID, 32);

			var result = SDKImport.WRAP_Insert_Card(loginID, ref stuCard);
			return result;
		}

		public static Card GetCardInfo(int loginID, int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(SDKImport.NET_RECORDSET_ACCESS_CTL_CARD));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = SDKImport.WRAP_GetCardInfo(loginID, recordNo, intPtr);

			SDKImport.NET_RECORDSET_ACCESS_CTL_CARD sdkCard = (SDKImport.NET_RECORDSET_ACCESS_CTL_CARD)(Marshal.PtrToStructure(intPtr, typeof(SDKImport.NET_RECORDSET_ACCESS_CTL_CARD)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var card = new Card();
			card.RecordNo = sdkCard.nRecNo;
			card.CreationDateTime = NET_TIMEToDateTime(sdkCard.stuCreateTime);
			card.CardNo = CharArrayToString(sdkCard.szCardNo);
			card.UserID = CharArrayToString(sdkCard.szUserID);
			card.CardStatus = sdkCard.emStatus;
			card.CardType = sdkCard.emType;
			card.Password = CharArrayToString(sdkCard.szPsw);
			card.DoorsCount = sdkCard.nDoorNum;
			card.Doors = sdkCard.sznDoors;
			card.TimeSectionsCount = sdkCard.nTimeSectionNum;
			card.TimeSections = sdkCard.sznTimeSectionNo;
			card.UserTime = sdkCard.nUserTime;
			card.ValidStartDateTime = NET_TIMEToDateTime(sdkCard.stuValidStartTime);
			card.ValidEndDateTime = NET_TIMEToDateTime(sdkCard.stuValidEndTime);
			card.IsValid = sdkCard.bIsValid;

			return card;
		}

		public static List<Card> GetAllCards(int loginID)
		{
			int structSize = Marshal.SizeOf(typeof(SDKImport.CardsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = SDKImport.WRAP_GetAllCards(loginID, intPtr);

			SDKImport.CardsCollection cardsCollection = (SDKImport.CardsCollection)(Marshal.PtrToStructure(intPtr, typeof(SDKImport.CardsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cards = new List<Card>();

			for (int i = 0; i < Math.Min(cardsCollection.Count, 500); i++)
			{
				var sdkCard = cardsCollection.Cards[i];
				var card = new Card();
				card.RecordNo = sdkCard.nRecNo;
				card.CreationDateTime = NET_TIMEToDateTime(sdkCard.stuCreateTime);
				card.CardNo = CharArrayToString(sdkCard.szCardNo);
				card.UserID = CharArrayToString(sdkCard.szUserID);
				card.CardStatus = sdkCard.emStatus;
				card.CardType = sdkCard.emType;
				card.Password = CharArrayToString(sdkCard.szPsw);
				card.DoorsCount = sdkCard.nDoorNum;
				card.Doors = sdkCard.sznDoors;
				card.TimeSectionsCount = sdkCard.nTimeSectionNum;
				card.TimeSections = sdkCard.sznTimeSectionNo;
				card.UserTime = sdkCard.nUserTime;
				card.ValidStartDateTime = NET_TIMEToDateTime(sdkCard.stuValidStartTime);
				card.ValidEndDateTime = NET_TIMEToDateTime(sdkCard.stuValidEndTime);
				card.IsValid = sdkCard.bIsValid;
				cards.Add(card);
			}
			return cards;
		}
	}
}