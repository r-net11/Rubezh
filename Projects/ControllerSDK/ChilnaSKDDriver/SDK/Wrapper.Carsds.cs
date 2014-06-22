using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddCard(Card card)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD stuCard = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD();
			stuCard.bIsValid = true;
			stuCard.emStatus = NativeWrapper.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_NORMAL;
			stuCard.emType = (NativeWrapper.NET_ACCESSCTLCARD_TYPE)card.CardType;
			stuCard.nTimeSectionNum = card.TimeSectionsCount;
			stuCard.nUserTime = card.UserTime;

			stuCard.stuCreateTime.dwYear = DateTime.Now.Year;
			stuCard.stuCreateTime.dwMonth = DateTime.Now.Month;
			stuCard.stuCreateTime.dwDay = DateTime.Now.Day;
			stuCard.stuCreateTime.dwHour = DateTime.Now.Hour;
			stuCard.stuCreateTime.dwMinute = DateTime.Now.Minute;
			stuCard.stuCreateTime.dwSecond = DateTime.Now.Second;

			stuCard.stuValidStartTime.dwYear = card.ValidStartDateTime.Year;
			stuCard.stuValidStartTime.dwMonth = card.ValidStartDateTime.Month;
			stuCard.stuValidStartTime.dwDay = card.ValidStartDateTime.Day;
			stuCard.stuValidStartTime.dwHour = 0;
			stuCard.stuValidStartTime.dwMinute = 0;
			stuCard.stuValidStartTime.dwSecond = 0;

			stuCard.stuValidEndTime.dwYear = card.ValidEndDateTime.Year;
			stuCard.stuValidEndTime.dwMonth = card.ValidEndDateTime.Month;
			stuCard.stuValidEndTime.dwDay = card.ValidEndDateTime.Day;
			stuCard.stuValidEndTime.dwHour = 0;
			stuCard.stuValidEndTime.dwMinute = 0;
			stuCard.stuValidEndTime.dwSecond = 0;

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
			stuCard.szUserID = Wrapper.StringToCharArray("1", 32);

			var result = NativeWrapper.WRAP_Insert_Card(LoginID, ref stuCard);
			return result;
		}

		public bool RemoveCard(int index)
		{
			var result = NativeWrapper.WRAP_RemoveCard(LoginID, index);
			return result;
		}

		public bool RemoveAllCards()
		{
			var result = NativeWrapper.WRAP_RemoveAllCards(LoginID);
			return result;
		}

		public Card GetCardInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetCardInfo(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD sdkCard = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var card = new Card();
			card.RecordNo = sdkCard.nRecNo;
			card.CardNo = CharArrayToString(sdkCard.szCardNo);
			card.CardType = (CardType)sdkCard.emType;
			card.Password = CharArrayToString(sdkCard.szPsw);
			card.DoorsCount = sdkCard.nDoorNum;
			card.Doors = sdkCard.sznDoors;
			card.TimeSectionsCount = sdkCard.nTimeSectionNum;
			card.TimeSections = sdkCard.sznTimeSectionNo;
			card.UserTime = sdkCard.nUserTime;
			card.ValidStartDateTime = NET_TIMEToDateTime(sdkCard.stuValidStartTime);
			card.ValidEndDateTime = NET_TIMEToDateTime(sdkCard.stuValidEndTime);

			return card;
		}

		public int GetCardsCount()
		{
			NativeWrapper.FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam = new NativeWrapper.FIND_RECORD_ACCESSCTLCARD_CONDITION();
			stuParam.szCardNo = Wrapper.StringToCharArray("1", 32);
			stuParam.szUserID = Wrapper.StringToCharArray("1", 32);
			var cardsCount = NativeWrapper.WRAP_Get_CardsCount(LoginID, ref stuParam);
			return cardsCount;
		}

		public List<Card> GetAllCards()
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.CardsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetAllCards(LoginID, intPtr);

			NativeWrapper.CardsCollection cardsCollection = (NativeWrapper.CardsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CardsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cards = new List<Card>();

			for (int i = 0; i < Math.Min(cardsCollection.Count, 500); i++)
			{
				var sdkCard = cardsCollection.Cards[i];
				var card = new Card();
				card.RecordNo = sdkCard.nRecNo;
				card.CardNo = CharArrayToString(sdkCard.szCardNo);
				card.CardType = (CardType)sdkCard.emType;
				card.Password = CharArrayToString(sdkCard.szPsw);
				card.DoorsCount = sdkCard.nDoorNum;
				card.Doors = sdkCard.sznDoors;
				card.TimeSectionsCount = sdkCard.nTimeSectionNum;
				card.TimeSections = sdkCard.sznTimeSectionNo;
				card.UserTime = sdkCard.nUserTime;
				card.ValidStartDateTime = NET_TIMEToDateTime(sdkCard.stuValidStartTime);
				card.ValidEndDateTime = NET_TIMEToDateTime(sdkCard.stuValidEndTime);
				cards.Add(card);
			}
			return cards;
		}
	}
}