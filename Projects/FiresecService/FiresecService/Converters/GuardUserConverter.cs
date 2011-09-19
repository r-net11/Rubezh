using System.Collections.Generic;
using System.Linq;
using Firesec.CoreConfiguration;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class GuardUserConverter
    {
        public static void Convert()
        {
            ConfigurationConverter.DeviceConfiguration.GuardUsers = new List<GuardUser>();

            if (ConfigurationConverter.FiresecConfiguration.part != null)
            {
                foreach (var innerDirection in ConfigurationConverter.FiresecConfiguration.part)
                {
                    if (innerDirection.type == "guarduser")
                    {
                        var guardUser = new GuardUser()
                        {
                            Id = int.Parse(innerDirection.id),
                            Gid = innerDirection.gid,
                            Name = innerDirection.name
                        };

                        if (innerDirection.PinZ != null)
                        {
                            foreach (var partZone in innerDirection.PinZ)
                            {
                                guardUser.Zones.Add(partZone.pidz);
                            }
                        }

                        if (innerDirection.param != null)
                        {
                            var KeyTMParameter = innerDirection.param.FirstOrDefault(x => x.name == "KeyTM");
                            if (KeyTMParameter != null)
                            {
                                guardUser.KeyTM = KeyTMParameter.value;
                            }

                            var DeviceUIDParameter = innerDirection.param.FirstOrDefault(x => x.name == "DeviceUID");
                            if (DeviceUIDParameter != null)
                            {
                                guardUser.DeviceUID = DeviceUIDParameter.value;
                            }

                            var PasswordParameter = innerDirection.param.FirstOrDefault(x => x.name == "Password");
                            if (PasswordParameter != null)
                            {
                                guardUser.Password = PasswordParameter.value;
                            }

                            var FunctionParameter = innerDirection.param.FirstOrDefault(x => x.name == "Function");
                            if (FunctionParameter != null)
                            {
                                guardUser.Function = FunctionParameter.value;
                            }

                            var FIOParameter = innerDirection.param.FirstOrDefault(x => x.name == "FIO");
                            if (FIOParameter != null)
                            {
                                guardUser.FIO = FIOParameter.value;
                            }

                            var CanSetZoneParameter = innerDirection.param.FirstOrDefault(x => x.name == "SetZone");
                            if (CanSetZoneParameter != null)
                            {
                                guardUser.CanSetZone = CanSetZoneParameter.value == "1";
                            }

                            var CanUnSetZoneParameter = innerDirection.param.FirstOrDefault(x => x.name == "UnSetZone");
                            if (CanUnSetZoneParameter != null)
                            {
                                guardUser.CanUnSetZone = CanUnSetZoneParameter.value == "1";
                            }
                        }

                        ConfigurationConverter.DeviceConfiguration.GuardUsers.Add(guardUser);
                    }
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration currentConfiguration)
        {
            var innerGuardUsers = new List<partType>();
            int no = 0;

            foreach (var guardUser in currentConfiguration.GuardUsers)
            {
                var innerGuardUser = new partType()
                {
                    type = "guarduser",
                    no = no.ToString(),
                    id = guardUser.Id.ToString(),
                    gid = guardUser.Gid,
                    name = guardUser.Name
                };
                ++no;

                var zones = new List<partTypePinZ>();
                foreach (var zone in guardUser.Zones)
                {
                    zones.Add(new partTypePinZ() { pidz = zone });
                }
                innerGuardUser.PinZ = zones.ToArray();

                var innerGuardUsersParameters = new List<paramType>();

                if (guardUser.CanSetZone)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "SetZone",
                        type = "Bool",
                        value = "1"
                    });
                }

                if (guardUser.CanUnSetZone)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "UnSetZone",
                        type = "Bool",
                        value = "1"
                    });
                }

                if (string.IsNullOrEmpty(guardUser.FIO) == false)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "FIO",
                        type = "String",
                        value = guardUser.FIO
                    });
                }

                if (string.IsNullOrEmpty(guardUser.Function) == false)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "Function",
                        type = "String",
                        value = guardUser.Function
                    });
                }

                if (string.IsNullOrEmpty(guardUser.Password) == false)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "Password",
                        type = "String",
                        value = guardUser.Password
                    });
                }

                if (string.IsNullOrEmpty(guardUser.DeviceUID) == false)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "DeviceUID",
                        type = "String",
                        value = guardUser.DeviceUID
                    });
                }

                if (string.IsNullOrEmpty(guardUser.KeyTM) == false)
                {
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "KeyTM",
                        type = "String",
                        value = guardUser.KeyTM
                    });
                }

                innerGuardUser.param = innerGuardUsersParameters.ToArray();

                innerGuardUsers.Add(innerGuardUser);
            }

            var innerDirections = ConfigurationConverter.FiresecConfiguration.part.ToList();
            if (innerDirections != null)
            {
                innerGuardUsers.AddRange(innerDirections);
            }

            ConfigurationConverter.FiresecConfiguration.part = innerGuardUsers.ToArray();
        }
    }
}