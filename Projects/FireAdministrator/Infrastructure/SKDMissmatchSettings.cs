using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Infrastructure.Models
{
	public class SKDMissmatchSettings
	{
		public SKDMissmatchSettings()
		{
			MissmatchControllerUIDs = new List<Guid>();
		}

		public List<Guid> MissmatchControllerUIDs { get; set; }

		public bool HasMissmatch(Guid deviceUID)
		{
			return MissmatchControllerUIDs.Any(x => x == deviceUID);
		}

		public void Set(Guid deviceUID)
		{
			if (!MissmatchControllerUIDs.Any(x => x == deviceUID))
				MissmatchControllerUIDs.Add(deviceUID);
		}

		public void Reset(Guid deviceUID)
		{
			MissmatchControllerUIDs.Remove(deviceUID);
		}
	}
}