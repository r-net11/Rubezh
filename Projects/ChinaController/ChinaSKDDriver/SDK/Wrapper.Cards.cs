using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Common;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		/// <summary>
		/// Записывает на контроллер данные по карте
		/// </summary>
		/// <param name="card">карта</param>
		/// <returns>true - в случае удачи, false - в противном случае</returns>
		public string AddCard(Card card)
		{
			var nativeCard = CardToNativeCard(card);
			var result = NativeWrapper.WRAP_Insert_Card(LoginID, ref nativeCard);
			return result.ToString();
		}

		/// <summary>
		/// Обновляет на контроллере данные для указанной карты
		/// </summary>
		/// <param name="card">номер карты</param>
		/// <returns>true - в случае удачи, false - в противном случае</returns>
		public bool EditCard(Card card)
		{
			var nativeCard = CardToNativeCard(card);
			var result = NativeWrapper.WRAP_Update_Card(LoginID, ref nativeCard);
			return result;
		}

		/// <summary>
		/// Удаляет с контроллера данные для указанной карты
		/// </summary>
		/// <param name="index">номер карты</param>
		/// <returns>true - в случае удачи, false - в противном случае</returns>
		public bool RemoveCard(int index)
		{
			var result = NativeWrapper.WRAP_Remove_Card(LoginID, index);
			return result;
		}

		/// <summary>
		/// Сбрасывает антипессбэк для указанной карты
		/// </summary>
		/// <param name="index">номер карты</param>
		/// <returns>true - в случае удачи, false - в противном случае</returns>
		public bool ResetRepeatEnter(string index)
		{
			var result = NativeWrapper.WRAP_Card_ClearRepeatEnter(LoginID, index);
			return result;
		}

		/// <summary>
		/// Выполняет сброс антипассбэка для указанной карты
		/// </summary>
		/// <param name="index">номер карты</param>
		/// <returns>true - в случае удачи, false - в противном случае</returns>
		public bool ClearRepeatEnter(int index)
		{
			return NativeWrapper.WRAP_Card_ClearRepeatEnter(LoginID, index.ToString("X"));
		}

		/// <summary>
		/// Удаляет с контроллера данные по всем картам
		/// </summary>
		/// <returns>true - в случае удачи, false - в противном случае</returns>
		public bool RemoveAllCards()
		{
			var result = NativeWrapper.WRAP_RemoveAll_Cards(LoginID);
			return result;
		}

		/// <summary>
		/// Получает у контроллера информацию по карте с указанным номером
		/// </summary>
		/// <param name="recordNo">номер карты</param>
		/// <returns>объект типа Card</returns>
		public Card GetCardInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD));
			IntPtr intPtr = IntPtr.Zero;
			var nativeCard = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD();
			try
			{
				intPtr = Marshal.AllocCoTaskMem(structSize);

				var result = NativeWrapper.WRAP_Get_Card_Info(LoginID, recordNo, intPtr);

				if (!result) return null;

				nativeCard =
					(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD)
						(Marshal.PtrToStructure(intPtr, typeof (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD)));
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
					Marshal.FreeCoTaskMem(intPtr);
			}

			var card = NativeCardToCard(nativeCard);
			return card;
		}

		/// <summary>
		/// Получает список всех карт, записанных на контроллер
		/// </summary>
		/// <returns>списко карт</returns>
		public List<Card> GetAllCards()
		{
			var resultCards = new List<Card>();
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Cards(LoginID, ref finderID);

			if (finderID > 0)
			{
				while (true)
				{
					var structSize = Marshal.SizeOf(typeof(NativeWrapper.CardsCollection));
					var intPtr = Marshal.AllocCoTaskMem(structSize);

					var result = NativeWrapper.WRAP_GetAll_Cards(finderID, intPtr);

					var cardsCollection = (NativeWrapper.CardsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CardsCollection)));
					Marshal.FreeCoTaskMem(intPtr);
					intPtr = IntPtr.Zero;

					if (result == 0)
						break;

					var cards = new List<Card>();
					for (var i = 0; i < Math.Min(cardsCollection.Count, 10); i++)
					{
						var nativeCard = cardsCollection.Cards[i];
						if (nativeCard.nRecNo > 0)
						{
							var card = NativeCardToCard(nativeCard);
							cards.Add(card);
						}
					}
					resultCards.AddRange(cards);
				}

				NativeWrapper.WRAP_EndGetAll(finderID);
			}

			return resultCards;
		}

		/// <summary>
		/// Получает количество записанных на контроллере карт
		/// </summary>
		/// <returns>количество карт, -1 в случае неудачи</returns>
		public int GetCardsCount()
		{
			int finderID = 0;
			NativeWrapper.WRAP_BeginGetAll_Cards(LoginID, ref finderID);

			if (finderID > 0)
			{
				var result = NativeWrapper.WRAP_GetAllCount(finderID);
				NativeWrapper.WRAP_EndGetAll(finderID);
				return result;
			}

			return -1;
		}

		/// <summary>
		/// Конвентирует объект типа Card в структуру NET_RECORDSET_ACCESS_CTL_CARD
		/// для передачи его во враппер SDK
		/// </summary>
		/// <param name="card">объект типа Card</param>
		/// <returns>структура NET_RECORDSET_ACCESS_CTL_CARD</returns>
		private NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD CardToNativeCard(Card card)
		{
			var nativeCard = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD();
			nativeCard.dwSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD));
			nativeCard.stuFingerPrintInfo = new NativeWrapper.NET_ACCESSCTLCARD_FINGERPRINT_PACKET();
			nativeCard.stuFingerPrintInfo.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeWrapper.NET_ACCESSCTLCARD_FINGERPRINT_PACKET));

			nativeCard.bIsValid = true;

			// use time
			nativeCard.nUserTime = card.UserTime;

			// card type
			nativeCard.emType = (NativeWrapper.NET_ACCESSCTLCARD_TYPE)card.CardType;

			// card status
			nativeCard.emStatus = (NativeWrapper.NET_ACCESSCTLCARD_STATE)card.CardStatus;

			// create time
			var curDateTime = DateTime.Now;
			nativeCard.stuCreateTime.dwYear = curDateTime.Year;
			nativeCard.stuCreateTime.dwMonth = curDateTime.Month;
			nativeCard.stuCreateTime.dwDay = curDateTime.Day;
			nativeCard.stuCreateTime.dwHour = curDateTime.Hour;
			nativeCard.stuCreateTime.dwMinute = curDateTime.Minute;
			nativeCard.stuCreateTime.dwSecond = curDateTime.Second;

			// valid time start
			nativeCard.stuValidStartTime.dwYear = card.ValidStartDateTime.Year;
			nativeCard.stuValidStartTime.dwMonth = card.ValidStartDateTime.Month;
			nativeCard.stuValidStartTime.dwDay = card.ValidStartDateTime.Day;
			nativeCard.stuValidStartTime.dwHour = 0;
			nativeCard.stuValidStartTime.dwMinute = 0;
			nativeCard.stuValidStartTime.dwSecond = 0;

			// valid time end
			nativeCard.stuValidEndTime.dwYear = card.ValidEndDateTime.Year;
			nativeCard.stuValidEndTime.dwMonth = card.ValidEndDateTime.Month;
			nativeCard.stuValidEndTime.dwDay = card.ValidEndDateTime.Day;
			nativeCard.stuValidEndTime.dwHour = 0;
			nativeCard.stuValidEndTime.dwMinute = 0;
			nativeCard.stuValidEndTime.dwSecond = 0;

			// card no
			nativeCard.szCardNo = card.CardNo;

			// password
			nativeCard.szPsw = card.Password;

			// user id
			nativeCard.szUserID = "1";

			// doors
			nativeCard.nDoorNum = card.Doors.Count;
			nativeCard.sznDoors = new int[32];
			for (int i = 0; i < card.Doors.Count; i++)
			{
				nativeCard.sznDoors[i] = card.Doors[i];
			}

			// time sections
			nativeCard.nTimeSectionNum = card.TimeSections.Count;
			nativeCard.sznTimeSectionNo = new int[32];
			for (int i = 0; i < card.TimeSections.Count; i++)
			{
				nativeCard.sznTimeSectionNo[i] = card.TimeSections[i];
			}

			//nativeCard.bFirstEnter = card.FirstEnter; // TODO определить card.FirstEnter
			nativeCard.bFirstEnter = false;

			//nativeCard.bHandicap = card.Handicap; // TODO определить card.Handicap
			nativeCard.bHandicap = card.IsHandicappedCard;

			return nativeCard;
		}

		/// <summary>
		/// Конвертирует структуру NET_RECORDSET_ACCESS_CTL_CARD,
		/// полученную из враппера SDK, в объет типа Card
		/// </summary>
		/// <param name="nativeCard">структура NET_RECORDSET_ACCESS_CTL_CARD</param>
		/// <returns>объект типа Card</returns>
		private Card NativeCardToCard(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARD nativeCard)
		{
			var card = new Card();
			card.RecordNo = nativeCard.nRecNo;
			card.CardNo = nativeCard.szCardNo;
			card.CardType = (CardType)nativeCard.emType;
			card.CardStatus = (CardStatus)nativeCard.emStatus;
			card.Password = nativeCard.szPsw;
			card.DoorsCount = nativeCard.nDoorNum;
			card.Doors = nativeCard.sznDoors.ToList();
			card.TimeSectionsCount = nativeCard.nTimeSectionNum;
			card.TimeSections = nativeCard.sznTimeSectionNo.ToList();
			card.UserTime = nativeCard.nUserTime;
			card.ValidStartDateTime = NET_TIMEToDateTime(nativeCard.stuValidStartTime);
			card.ValidEndDateTime = NET_TIMEToDateTime(nativeCard.stuValidEndTime);
			card.IsHandicappedCard = nativeCard.bHandicap;
			return card;
		}
	}
}