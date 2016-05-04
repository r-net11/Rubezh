﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class AccessTemplateDeactivatingReader : SKDModelBase
	{
		[DataMember]
		public Guid AccessTemplateUID { get; set; }

		[DataMember]
		public Guid DeactivatingReaderUID { get; set; }
	}
}