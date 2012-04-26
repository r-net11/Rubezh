using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.CoreConfiguration;
using FiresecAPI.Models;

namespace FiresecService
{
	public partial class ConfigurationManager
    {
		void ConvertDirections()
        {
			DeviceConfiguration.Directions = new List<Direction>();

			if (FiresecConfiguration.part != null)
            {
				foreach (var innerDirection in FiresecConfiguration.part)
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
                                    direction.Zones.Add(ulong.Parse(item.pidz));
                            }
                        }

                        if (innerDirection.param != null)
                        {
                            var rmParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_RM");
                            direction.DeviceRm = GuidHelper.ToGuid(rmParameter.value);
                            var buttonParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_AM");
                            direction.DeviceButton = GuidHelper.ToGuid(buttonParameter.value);
                        }

						DeviceConfiguration.Directions.Add(direction);
                    }
                }
            }
        }

		void ConvertDirectionsBack()
        {
            var innerDirections = new List<partType>();
            int no = 0;

			foreach (var direction in DeviceConfiguration.Directions)
            {
                var innerDirection = new partType()
                {
                    type = "direction",
                    no = no.ToString(),
                    id = direction.Id.ToString(),
					gid = Gid++.ToString(),
                    name = direction.Name
                };
                ++no;

                var zones = new List<partTypePinZ>();
                foreach (var zone in direction.Zones)
                {
                    zones.Add(new partTypePinZ() { pidz = zone.ToString() });
                }
                innerDirection.PinZ = zones.ToArray();

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

			FiresecConfiguration.part = innerDirections.ToArray();
        }
    }
}