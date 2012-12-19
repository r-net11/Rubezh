using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MuliclientAPI
{
	[DataContract]
	public class MulticlientConfiguration
	{
		public MulticlientConfiguration()
		{
			MulticlientDatas = new List<MulticlientData>();
		}

		[DataMember]
		public List<MulticlientData> MulticlientDatas { get; set; }
	}
}