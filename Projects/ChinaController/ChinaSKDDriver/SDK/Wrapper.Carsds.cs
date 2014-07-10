using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddCard(Card card)
		{
			var nativeCard = CardToNativeCard(card);
			var result = NativeWrapper.WRAP_Insert_Card(LoginID, ref nativeCard);
			return result;
		}

		public bool EditCard(Card card)
		{
			var nativeCard = CardToNativeCard(card);
			var result = NativeWrapper.WRAP_Update_Card(LoginID, ref nativeCard);
			return result;
		}

		public bool RemoveCard(int index)
		{
			var result = NativeWrapper.WRAP_Remove_Card(LoginID, index);
			return result;
		}

		public bool RemoveAllCards()
		{
			var result = NativeWrapper.WRAP_RemoveAll_Cards(LoginID);
			return result;
		}

		public Card GetCardInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_Get_Card_Info(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD nativeCard = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var card = NativeCardToCard(nativeCard);
			return card;
		}

		public int GetCardsCount()
		{
			var cardsCount = NativeWrapper.WRAP_Get_Cards_Count(LoginID);
			return cardsCount;
		}

		public List<Card> GetAllCards()
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.CardsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetAll_Cards(LoginID, intPtr);

			NativeWrapper.CardsCollection cardsCollection = (NativeWrapper.CardsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CardsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cards = new List<Card>();
			for (int i = 0; i < Math.Min(cardsCollection.Count, 10); i++)
			{
				var nativeCard = cardsCollection.Cards[i];
				var card = NativeCardToCard(nativeCard);
				cards.Add(card);
			}
			return cards;
		}

		NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD CardToNativeCard(Card card)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD nativeCard = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD();
			nativeCard.bIsValid = true;
			nativeCard.nUserTime = card.UserTime;
			nativeCard.emStatus = NativeWrapper.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_NORMAL;
			nativeCard.emType = (NativeWrapper.NET_ACCESSCTLCARD_TYPE)card.CardType;

			nativeCard.stuCreateTime.dwYear = DateTime.Now.Year;
			nativeCard.stuCreateTime.dwMonth = DateTime.Now.Month;
			nativeCard.stuCreateTime.dwDay = DateTime.Now.Day;
			nativeCard.stuCreateTime.dwHour = DateTime.Now.Hour;
			nativeCard.stuCreateTime.dwMinute = DateTime.Now.Minute;
			nativeCard.stuCreateTime.dwSecond = DateTime.Now.Second;

			nativeCard.stuValidStartTime.dwYear = card.ValidStartDateTime.Year;
			nativeCard.stuValidStartTime.dwMonth = card.ValidStartDateTime.Month;
			nativeCard.stuValidStartTime.dwDay = card.ValidStartDateTime.Day;
			nativeCard.stuValidStartTime.dwHour = 0;
			nativeCard.stuValidStartTime.dwMinute = 0;
			nativeCard.stuValidStartTime.dwSecond = 0;

			nativeCard.stuValidEndTime.dwYear = card.ValidEndDateTime.Year;
			nativeCard.stuValidEndTime.dwMonth = card.ValidEndDateTime.Month;
			nativeCard.stuValidEndTime.dwDay = card.ValidEndDateTime.Day;
			nativeCard.stuValidEndTime.dwHour = 0;
			nativeCard.stuValidEndTime.dwMinute = 0;
			nativeCard.stuValidEndTime.dwSecond = 0;

			nativeCard.szCardNo = StringToCharArray(card.CardNo, 32);
			nativeCard.szPsw = Wrapper.StringToCharArray(card.Password, 64);
			nativeCard.szUserID = Wrapper.StringToCharArray("1", 32);

			nativeCard.nDoorNum = card.Doors.Count;
			nativeCard.sznDoors = new int[32];
			for (int i = 0; i < card.Doors.Count; i++)
			{
				nativeCard.sznDoors[i] = card.Doors[i];
			}

			nativeCard.nTimeSectionNum = card.TimeSections.Count;
			nativeCard.sznTimeSectionNo = new int[32];
			for (int i = 0; i < card.TimeSections.Count; i++)
			{
				nativeCard.sznTimeSectionNo[i] = card.TimeSections[i];
			}

			return nativeCard;
		}

		Card NativeCardToCard(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD nativeCard)
		{
			var card = new Card();
			card.RecordNo = nativeCard.nRecNo;
			card.CardNo = CharArrayToString(nativeCard.szCardNo);
			card.CardType = (CardType)nativeCard.emType;
			card.Password = CharArrayToString(nativeCard.szPsw);
			card.DoorsCount = nativeCard.nDoorNum;
			card.Doors = nativeCard.sznDoors.ToList();
			card.TimeSectionsCount = nativeCard.nTimeSectionNum;
			card.TimeSections = nativeCard.sznTimeSectionNo.ToList();
			card.UserTime = nativeCard.nUserTime;
			card.ValidStartDateTime = NET_TIMEToDateTime(nativeCard.stuValidStartTime);
			card.ValidEndDateTime = NET_TIMEToDateTime(nativeCard.stuValidEndTime);
			return card;
		}
	}
}