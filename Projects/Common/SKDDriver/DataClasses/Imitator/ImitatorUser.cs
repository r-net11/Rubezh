using FiresecAPI.GK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SKDDriver.DataClasses
{
	public class ImitatorUser
	{
		public ImitatorUser()
		{
			UID = Guid.NewGuid();
			ImitatorUserDevices = new List<ImitatorUserDevice>();
		}

		[Key]
		public Guid UID { get; set; }

		public int GKNo { get; set; }

		public int Number { get; set; }

		[MaxLength(32)]
		public string Name { get; set; }

		public DateTime EndDateTime { get; set; }

		public int TotalSeconds { get; set; }

		public GKCardType CardType { get; set; }

		public int Level { get; set; }

		public int ScheduleNo { get; set; }

		public bool IsBlocked { get; set; }

		public ICollection<ImitatorUserDevice> ImitatorUserDevices { get; set; }
	}

	public class ImitatorUserDevice
	{
		public ImitatorUserDevice()
		{
			UID = Guid.NewGuid();
		}
		
		[Key]
		public Guid UID { get; set; }

		public int DescriptorNo { get; set; }

		public int ScheduleNo { get; set; }

		public Guid ImitatorUserUID { get; set; }
	}
}