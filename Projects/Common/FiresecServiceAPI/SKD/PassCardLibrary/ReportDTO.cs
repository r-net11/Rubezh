﻿using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ReportDTO
	{
		[DataMember]
		public byte[] Report { get; set; }

		[DataMember]
		public CardTemplatePrintDataDTO Data { get; set; }

		[DataMember]
		public byte[] BackgroundImage { get; set; }
	}
}
