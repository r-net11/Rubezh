using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.Models.CoreConfiguration;
using FiresecAPI.Models;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
        void ConvertDirections(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			deviceConfiguration.Directions = new List<Direction>();

			if (coreConfig.part != null)
			{
				foreach (var innerDirection in coreConfig.part)
				{
					if (innerDirection.type == "direction")
					{
						var direction = new Direction()
						{
							Id = int.Parse(innerDirection.id),
							Name = innerDirection.name,
							Description = innerDirection.desc
						};

						if (innerDirection.PinZ != null)
						{
							foreach (var item in innerDirection.PinZ)
							{
                                if (string.IsNullOrWhiteSpace(item.pidz) == false)
                                {
                                    var zoneNo = int.Parse(item.pidz);
                                    var zone = deviceConfiguration.Zones.FirstOrDefault(x=>x.No == zoneNo);
                                    direction.ZoneUIDs.Add(zone.UID);
                                }
							}
						}

						if (innerDirection.param != null)
						{
							var rmParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_RM");
							direction.DeviceRm = GuidHelper.ToGuid(rmParameter.value);
							var buttonParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_AM");
							direction.DeviceButton = GuidHelper.ToGuid(buttonParameter.value);
						}

						deviceConfiguration.Directions.Add(direction);
					}
				}
			}
		}

        void ConvertDirectionsBack(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig, ref int gid)
		{
			var innerDirections = new List<partType>();
			int no = 0;

			foreach (var direction in deviceConfiguration.Directions)
			{
				var innerDirection = new partType()
				{
					type = "direction",
					no = no.ToString(),
					id = direction.Id.ToString(),
					gid = gid++.ToString(),
					name = direction.Name
				};
				++no;

				var zonesPartTypePinZ = new List<partTypePinZ>();
                foreach (var zoneUID in direction.ZoneUIDs)
				{
                    var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
                    if (zone != null)
                    {
                        zonesPartTypePinZ.Add(new partTypePinZ() { pidz = zone.No.ToString() });
                    }
				}
				innerDirection.PinZ = zonesPartTypePinZ.ToArray();

				if (direction.DeviceRm != Guid.Empty || direction.DeviceButton != Guid.Empty)
				{
					var innerDirectionParameters = new List<paramType>();
					innerDirectionParameters.Add(new paramType()
					{
						name = "Device_RM",
						type = "String",
						value = GuidHelper.ToString(direction.DeviceRm)
					});

					innerDirectionParameters.Add(new paramType()
					{
						name = "Device_AM",
						type = "String",
						value = GuidHelper.ToString(direction.DeviceRm)
					});

					innerDirection.param = innerDirectionParameters.ToArray();
				}

				if (innerDirection.param != null)
				{
					var rmParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_RM");
					direction.DeviceRm = GuidHelper.ToGuid(rmParameter.value);
					var buttonParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_AM");
					direction.DeviceButton = GuidHelper.ToGuid(buttonParameter.value);
				}

				innerDirections.Add(innerDirection);
			}

			coreConfig.part = innerDirections.ToArray();
		}
	}
}