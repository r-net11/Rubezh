using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeCardIndexFilter
	{
		[DataMember]
		public int Id { get; set; }
	}
}
