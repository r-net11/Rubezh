﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class OPCTag
	{
		public OPCTag()
		{
			Uid = Guid.NewGuid().ToString();
			Name = "Название OPC Тега";
            NodeNum = "Идентификатор ноды";
		    Value = "Какое-то значение";

		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NodeNum { get; set; }

        [DataMember]
        public string SessionUrl { get; set; }

        [DataMember]
        public string Value { get; set; }

		[DataMember]
		public string Uid { get; set; }
	}
}