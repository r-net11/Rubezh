﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ControllerSDK.API;

namespace ControllerSDK.SDK
{
	public static class SDKWrapper
	{
		#region Common
		public static string CharArrayToString(char[] charArray)
		{
			var result = new string(charArray);
			int i = result.IndexOf('\0');
			if (i >= 0)
				result = result.Substring(0, i);
			return result;
		}

		public static string CharArrayToStringNoTrim(char[] charArray)
		{
			var result = new string(charArray);
			//int i = result.IndexOf('\0');
			//if (i >= 0)
			//    result = result.Substring(0, i);
			return result;
		}

		public static char[] StringToCharArray(string str, int size)
		{
			var result = new char[size];
			var charArray = str.ToCharArray();
			for (int i = 0; i < Math.Min(charArray.Count(), size); i++)
			{
				result[i] = charArray[i];
			}
			return result;
		}

		public static DateTime NET_TIMEToDateTime(ControllerSDK.SDK.SDKImport.NET_TIME netTime)
		{
			DateTime dateTime = DateTime.MinValue;
			try
			{
				if (netTime.dwYear == 0 || netTime.dwMonth == 0 || netTime.dwDay == 0)
					return new DateTime();
				dateTime = new DateTime(netTime.dwYear, netTime.dwMonth, netTime.dwDay, netTime.dwHour, netTime.dwMinute, netTime.dwSecond);
			}
			catch { }
			return dateTime;
		}
		#endregion

		public static int AddCard(int loginID, Card card)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARD();
			stuCard.bIsValid = card.IsValid;
			stuCard.emStatus = card.CardStatus;;
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

			stuCard.szCardNo = SDKWrapper.StringToCharArray(card.CardNo, 32);
			stuCard.nDoorNum = card.DoorsCount;
			stuCard.sznDoors = new int[32];
			stuCard.sznDoors[0] = 1;
			stuCard.sznDoors[1] = 2;
			stuCard.nTimeSectionNum = card.TimeSectionsCount;
			stuCard.sznTimeSectionNo = new int[32];
			stuCard.sznTimeSectionNo[0] = 1;
			stuCard.sznTimeSectionNo[1] = 2;
			stuCard.szPsw = SDKWrapper.StringToCharArray(card.Password, 64);
			stuCard.szUserID = SDKWrapper.StringToCharArray(card.UserID, 32);

			var result = SDKImport.WRAP_Insert_Card(loginID, ref stuCard);
			return result;
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

		public static int AddPassword(int loginID, Password password)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = new SDKImport.NET_RECORDSET_ACCESS_CTL_PWD();
			stuAccessCtlPwd.stuCreateTime.dwYear = password.CreationDateTime.Year;
			stuAccessCtlPwd.stuCreateTime.dwMonth = password.CreationDateTime.Month;
			stuAccessCtlPwd.stuCreateTime.dwDay = password.CreationDateTime.Day;
			stuAccessCtlPwd.stuCreateTime.dwHour = password.CreationDateTime.Hour;
			stuAccessCtlPwd.stuCreateTime.dwMinute = password.CreationDateTime.Minute;
			stuAccessCtlPwd.stuCreateTime.dwSecond = password.CreationDateTime.Second;
			stuAccessCtlPwd.szUserID = SDKWrapper.StringToCharArray(password.UserID, 32);
			stuAccessCtlPwd.szDoorOpenPwd = SDKWrapper.StringToCharArray(password.DoorOpenPassword, 64);
			stuAccessCtlPwd.szAlarmPwd = SDKWrapper.StringToCharArray(password.AlarmPassword, 64);
			stuAccessCtlPwd.nDoorNum = password.DoorsCount;
			stuAccessCtlPwd.sznDoors = new int[32];
			stuAccessCtlPwd.sznDoors[0] = 1;
			stuAccessCtlPwd.sznDoors[1] = 2;

			var result = SDKImport.WRAP_Insert_Pwd(loginID, ref stuAccessCtlPwd);
			return result;
		}

		public static List<Password> GetAllPasswords(int loginID)
		{
			int structSize = Marshal.SizeOf(typeof(SDKImport.PasswordsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = SDKImport.WRAP_GetAllPasswords(loginID, intPtr);

			SDKImport.PasswordsCollection passwordsCollection = (SDKImport.PasswordsCollection)(Marshal.PtrToStructure(intPtr, typeof(SDKImport.PasswordsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var passwords = new List<Password>();

			for (int i = 0; i < Math.Min(passwordsCollection.Count, 500); i++)
			{
				var sdkPassword = passwordsCollection.Passwords[i];
				var password = new Password();
				password.RecordNo = sdkPassword.nRecNo;
				password.CreationDateTime = NET_TIMEToDateTime(sdkPassword.stuCreateTime);
				password.UserID = CharArrayToString(sdkPassword.szUserID);
				password.DoorOpenPassword = CharArrayToString(sdkPassword.szDoorOpenPwd);
				password.AlarmPassword = CharArrayToString(sdkPassword.szAlarmPwd);
				password.DoorsCount = sdkPassword.nDoorNum;
				password.Doors = sdkPassword.sznDoors;
				passwords.Add(password);
			}
			return passwords;
		}

		public static int AddHoliday(int loginID, Holiday holiday)
		{
			SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = new SDKImport.NET_RECORDSET_HOLIDAY();
			stuHoliday.nDoorNum = holiday.DoorsCount;
			stuHoliday.sznDoors = new int[32];
			stuHoliday.sznDoors[0] = 1;
			stuHoliday.sznDoors[1] = 2;

			stuHoliday.stuStartTime.dwYear = holiday.StartDateTime.Year;
			stuHoliday.stuStartTime.dwMonth = holiday.StartDateTime.Month;
			stuHoliday.stuStartTime.dwDay = holiday.StartDateTime.Day;
			stuHoliday.stuStartTime.dwHour = holiday.StartDateTime.Hour;
			stuHoliday.stuStartTime.dwMinute = holiday.StartDateTime.Minute;
			stuHoliday.stuStartTime.dwSecond = holiday.StartDateTime.Second;

			stuHoliday.stuEndTime.dwYear = holiday.EndDateTime.Year;
			stuHoliday.stuEndTime.dwMonth = holiday.EndDateTime.Month;
			stuHoliday.stuEndTime.dwDay = holiday.EndDateTime.Day;
			stuHoliday.stuEndTime.dwHour = holiday.EndDateTime.Hour;
			stuHoliday.stuEndTime.dwMinute = holiday.EndDateTime.Minute;
			stuHoliday.stuEndTime.dwSecond = holiday.EndDateTime.Second;
			stuHoliday.bEnable = holiday.IsEnabled;
			var result = SDKImport.WRAP_Insert_Holiday(loginID, ref stuHoliday);
			return result;
		}

		public static List<Holiday> GetAllHolidays(int loginID)
		{
			return new List<Holiday>();
		}
	}
}