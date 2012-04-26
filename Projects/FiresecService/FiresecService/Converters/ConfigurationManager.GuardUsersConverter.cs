using System;
using System.Collections.Generic;
using System.Linq;
using Firesec.CoreConfiguration;
using FiresecAPI.Models;

namespace FiresecService
{
	public partial class ConfigurationManager
    {
		void ConvertGuardUsers()
        {
			DeviceConfiguration.GuardUsers = new List<GuardUser>();

			if (FiresecConfiguration.part != null)
            {
				foreach (var innerGuardUser in FiresecConfiguration.part)
                {
                    if (innerGuardUser.type == "guarduser")
                    {
                        var guardUser = new GuardUser()
                        {
                            Id = int.Parse(innerGuardUser.id),
                            Name = innerGuardUser.name
                        };

                        if (innerGuardUser.PinZ != null)
                        {
                            foreach (var partZone in innerGuardUser.PinZ)
                            {
                                guardUser.Zones.Add(ulong.Parse(partZone.pidz));
                            }
                        }

                        if (innerGuardUser.param != null)
                        {
                            var KeyTMParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "KeyTM");
                            if (KeyTMParameter != null)
                            {
                                guardUser.KeyTM = KeyTMParameter.value;
                            }

                            var DeviceUIDParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "DeviceUID");
                            if (DeviceUIDParameter != null)
                            {
                                guardUser.DeviceUID = new Guid(DeviceUIDParameter.value);
                            }

                            var PasswordParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "Password");
                            if (PasswordParameter != null)
                            {
                                guardUser.Password = PasswordParameter.value;
                            }

                            var FunctionParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "Function");
                            if (FunctionParameter != null)
                            {
                                guardUser.Function = FunctionParameter.value;
                            }

                            var FIOParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "FIO");
                            if (FIOParameter != null)
                            {
                                guardUser.FIO = FIOParameter.value;
                            }

                            var CanSetZoneParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "SetZone");
                            if (CanSetZoneParameter != null)
                            {
                                guardUser.CanSetZone = CanSetZoneParameter.value == "1";
                            }

                            var CanUnSetZoneParameter = innerGuardUser.param.FirstOrDefault(x => x.name == "UnSetZone");
                            if (CanUnSetZoneParameter != null)
                            {
                                guardUser.CanUnSetZone = CanUnSetZoneParameter.value == "1";
                            }
                        }

						DeviceConfiguration.GuardUsers.Add(guardUser);
                    }
                }
            }
        }

		void ConvertGuardUsersBack()
        {
            var innerGuardUsers = new List<partType>();
            int no = 0;

			foreach (var guardUser in DeviceConfiguration.GuardUsers)
            {
                var innerGuardUser = new partType()
                {
                    type = "guarduser",
                    no = no.ToString(),
                    id = guardUser.Id.ToString(),
					gid = Gid++.ToString(),
                    name = guardUser.Name
                };
                ++no;

                var innerZones = new List<partTypePinZ>();
                foreach (var zone in guardUser.Zones)
                {
                    innerZones.Add(new partTypePinZ() { pidz = zone.ToString() });
                }
                innerGuardUser.PinZ = innerZones.ToArray();

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

                if (guardUser.DeviceUID != Guid.Empty)
                    innerGuardUsersParameters.Add(new paramType()
                    {
                        name = "DeviceUID",
                        type = "String",
                        value = guardUser.DeviceUID.ToString()
                    });

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

			var innerDirections = FiresecConfiguration.part.ToList();
            if (innerDirections != null)
            {
                innerGuardUsers.AddRange(innerDirections);
            }

			FiresecConfiguration.part = innerGuardUsers.ToArray();
        }
    }
}