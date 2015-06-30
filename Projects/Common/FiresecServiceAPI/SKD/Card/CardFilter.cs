﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardFilter : OrganisationFilterBase
	{
		public CardFilter()
			: base()
		{
			DeactivationType = LogicalDeletationType.All;
			CardTypes = new List<CardType>();
			EndDate = DateTime.Now.Date;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }

		[DataMember]
		public bool IsWithEndDate { get; set; }
		
		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public List<CardType> CardTypes { get; set; }

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		[DataMember]
		public bool IsWithInactive { get; set; }
	}
}