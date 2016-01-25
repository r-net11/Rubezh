﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Дневной график
	/// </summary>
	[DataContract]
	public class GKDaySchedule : ModelBase
	{
		public GKDaySchedule()
		{
			DayScheduleParts = new List<GKDaySchedulePart>();
		}

		/// <summary>
		/// Составные части дневного графика
		/// </summary>
		[DataMember]
		public List<GKDaySchedulePart> DayScheduleParts { get; set; }

		public bool IsEnabled
		{
			get
			{
				return Name != "<Никогда>" && Name != "<Всегда>";
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is GKDaySchedule)
				return ((GKDaySchedule)obj).UID == UID;
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return No.GetHashCode();
		}
	}
}