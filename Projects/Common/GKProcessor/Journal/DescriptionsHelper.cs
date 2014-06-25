using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;

namespace GKProcessor
{
	public static class DescriptionsHelper
	{
		static List<Description> result;

		public static List<Description> GetAllDescriptions()
		{
			result = new List<Description>();
			for (int i = 1; i <= 255; i++)
			{
				AddToResult(JournalStringsHelper.ToFire((byte)i), DescriptionType.Fire);
				AddToResult(JournalStringsHelper.ToFailure((byte)i), DescriptionType.Failure);
				AddToResult(JournalStringsHelper.ToBatteryFailure((byte)i), DescriptionType.Failure);
				AddToResult(JournalStringsHelper.ToInformation((byte)i), DescriptionType.Information);
				AddToResult(JournalStringsHelper.ToUser((byte)i), DescriptionType.User);
			}
			foreach (EventDescription item in Enum.GetValues(typeof(EventDescription)))
			{
				AddToResult(item.ToDescription(), GetType(item));
			}
			result.Sort(new DescriptionComparer());
			return result;
		}

		//Для сообщений, добавляемых программой
		public static DescriptionType GetType(EventDescription description)
		{
			switch(description)
			{
				case(EventDescription.Остановка_пуска):
				case(EventDescription.Выключить_немедленно):
				case(EventDescription.Выключить):
				case(EventDescription.Включить_немедленно):
				case(EventDescription.Включить):
				case(EventDescription.Перевод_в_ручной_режим):
				case(EventDescription.Перевод_в_автоматический_режим):
				case (EventDescription.Перевод_в_отключенный_режим):
				case(EventDescription.Сброс):
					return DescriptionType.Information;
				case(EventDescription.Не_найдено_родительское_устройство_ГК):
					return DescriptionType.Failure;
				default:
					return DescriptionType.Unknown;
			}
		}

		public class DescriptionComparer : IComparer<Description>
		{
			public int Compare(Description x, Description y)
			{
				string s1 = x.Name;
				string s2 = y.Name;
				return string.Compare(s1, s2);
			}
		}

		static void AddToResult(string descriptionName, DescriptionType descriptionType)
		{
			if (descriptionName != "" && !result.Any(x => x.Name == descriptionName))
			{
				result.Add(new Description(descriptionName, descriptionType));
			}
		}
	}
}