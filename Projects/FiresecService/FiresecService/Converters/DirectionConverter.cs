using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.CoreConfig;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public class DirectionConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.DeviceConfiguration.Directions = new List<Direction>();

            if (FiresecManager.CoreConfig.part != null)
            {
                foreach (var innerDirection in FiresecManager.CoreConfig.part)
                {
                    if (innerDirection.type == "direction")
                    {
                        var direction = new Direction()
                        {
                            Id = int.Parse(innerDirection.id),
                            Gid = innerDirection.gid,
                            Name = innerDirection.name,
                            Description = innerDirection.desc
                        };

                        if (innerDirection.PinZ != null)
                        {
                            foreach (var partZone in innerDirection.PinZ)
                            {
                                direction.Zones.Add(partZone.pidz);
                            }
                        }

                        if (innerDirection.param != null)
                        {
                            var rmParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_RM");
                            direction.DeviceRm = GuidHelper.ToGuid(rmParameter.value);
                            var buttonParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_AM");
                            direction.DeviceButton = GuidHelper.ToGuid(buttonParameter.value);
                        }

                        FiresecManager.DeviceConfiguration.Directions.Add(direction);
                    }
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            var innerDirections = new List<Firesec.CoreConfig.partType>();
            int no = 0;

            foreach (var direction in FiresecManager.DeviceConfiguration.Directions)
            {
                var innerDirection = new Firesec.CoreConfig.partType()
                {
                    type = "direction",
                    no = no.ToString(),
                    id = direction.Id.ToString(),
                    gid = direction.Gid,
                    name = direction.Name
                };
                ++no;

                var zones = new List<Firesec.CoreConfig.partTypePinZ>();
                foreach (var zone in direction.Zones)
                {
                    zones.Add(new Firesec.CoreConfig.partTypePinZ() { pidz = zone });
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

            FiresecManager.CoreConfig.part = innerDirections.ToArray();
        }
    }
}