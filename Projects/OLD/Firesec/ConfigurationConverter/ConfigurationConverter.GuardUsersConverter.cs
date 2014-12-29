using System;
using System.Collections.Generic;
using System.Linq;
using Firesec.Models.CoreConfiguration;
using FiresecAPI.Models;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		void ConvertGuardUsers(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			deviceConfiguration.GuardUsers = new List<GuardUser>();

			if (coreConfig.part != null)
			{
				foreach (var innerGuardUser in coreConfig.part)
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
								var zoneNo = int.Parse(partZone.pidz);
								var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
								guardUser.ZoneUIDs.Add(zone.UID);
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

						deviceConfiguration.GuardUsers.Add(guardUser);
					}
				}
			}
		}

		void ConvertGuardUsersBack(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig, ref int gid)
		{
			var innerGuardUsers = new List<partType>();
			int no = 0;

			foreach (var guardUser in deviceConfiguration.GuardUsers)
			{
				var innerGuardUser = new partType()
				{
					type = "guarduser",
					no = no.ToString(),
					id = guardUser.Id.ToString(),
					gid = gid++.ToString(),
					name = guardUser.Name
				};
				++no;

				var innerZones = new List<partTypePinZ>();
				foreach (var zoneUID in guardUser.ZoneUIDs)
				{
					var zone = deviceConfiguration.Zones.FirstOrDefault(x=>x.UID == zoneUID);
					if (zone != null)
					{
						innerZones.Add(new partTypePinZ() { pidz = zone.No.ToString() });
					}
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

			var innerDirections = coreConfig.part.ToList();
			if (innerDirections != null)
			{
				innerGuardUsers.AddRange(innerDirections);
			}

			coreConfig.part = innerGuardUsers.ToArray();
		}
	}
}