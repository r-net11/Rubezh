using System.Collections.Generic;
using System.Linq;
using FiresecClient.Models;
using Firesec.CoreConfig;

namespace FiresecClient.Converters
{
    public class DirectionConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.Configuration.Directions = new List<Direction>();

            if (FiresecManager.CoreConfig.part != null)
            {
                foreach (var innerDirection in FiresecManager.CoreConfig.part)
                {
                    if (innerDirection.type == "direction")
                    {
                        Direction direction = new Direction();
                        direction.Id = System.Convert.ToInt32(innerDirection.id);
                        direction.Gid = innerDirection.gid;
                        direction.Name = innerDirection.name;
                        direction.Description = innerDirection.desc;

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
                            direction.DeviceRm = rmParameter.value;
                            var buttonParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_AM");
                            direction.DeviceButton = buttonParameter.value;
                        }

                        FiresecManager.Configuration.Directions.Add(direction);
                    }
                }
            }
        }

        public static void ConvertBack(CurrentConfiguration currentConfiguration)
        {
            List<Firesec.CoreConfig.partType> innerDirections = new List<Firesec.CoreConfig.partType>();
            int no = 0;

            foreach (var direction in FiresecManager.Configuration.Directions)
            {
                Firesec.CoreConfig.partType innerDirection = new Firesec.CoreConfig.partType();
                innerDirection.type = "direction";
                innerDirection.no = no.ToString();
                no++;
                innerDirection.id = direction.Id.ToString();
                innerDirection.gid = direction.Gid;
                innerDirection.name = direction.Name;

                List<Firesec.CoreConfig.partTypePinZ> zones = new List<Firesec.CoreConfig.partTypePinZ>();
                foreach (var zone in direction.Zones)
                {
                    zones.Add(new Firesec.CoreConfig.partTypePinZ() { pidz = zone });
                }
                innerDirection.PinZ = zones.ToArray();

                if ((direction.DeviceRm != null) || (direction.DeviceButton != null))
                {
                    List<paramType> innerDirectionParameters = new List<paramType>();
                    paramType rmParameter = new paramType();
                    rmParameter.name = "Device_RM";
                    rmParameter.type = "String";
                    rmParameter.value = direction.DeviceRm;
                    innerDirectionParameters.Add(rmParameter);
                    paramType buttonParameter = new paramType();
                    buttonParameter.name = "Device_AM";
                    buttonParameter.type = "String";
                    buttonParameter.value = direction.DeviceRm;
                    innerDirectionParameters.Add(buttonParameter);
                    innerDirection.param = innerDirectionParameters.ToArray();
                }

                if (innerDirection.param != null)
                {
                    var rmParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_RM");
                    direction.DeviceRm = rmParameter.value;
                    var buttonParameter = innerDirection.param.FirstOrDefault(x => x.name == "Device_AM");
                    direction.DeviceButton = buttonParameter.value;
                }

                innerDirections.Add(innerDirection);
            }

            FiresecManager.CoreConfig.part = innerDirections.ToArray();
        }
    }
}
