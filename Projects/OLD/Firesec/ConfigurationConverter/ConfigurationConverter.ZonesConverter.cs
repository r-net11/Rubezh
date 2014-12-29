using System.Collections.Generic;
using System.Linq;
using Firesec.Models.CoreConfiguration;
using FiresecAPI.Models;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		void ConvertZones(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			deviceConfiguration.Zones = new List<Zone>();

			if (coreConfig.zone != null)
			{
				foreach (var innerZone in coreConfig.zone)
				{
					var zone = new Zone()
					{
						Name = innerZone.name,
						No = int.Parse(innerZone.no),
						Description = innerZone.desc
					};

					if (innerZone.shape != null)
					{
						zone.ShapeIds = new List<string>();
						foreach (var shape in innerZone.shape)
						{
							zone.ShapeIds.Add(shape.id);
						}
					}

					if (innerZone.param != null)
					{
						var zoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "ZoneType");
						if (zoneTypeParam != null)
						{
							zone.ZoneType = (zoneTypeParam.value == "0") ? ZoneType.Fire : ZoneType.Guard;
						}

						var exitTimeParam = innerZone.param.FirstOrDefault(x => x.name == "ExitTime");
						if (exitTimeParam != null)
						{
							int value;
							if (int.TryParse(exitTimeParam.value, out value))
								zone.EvacuationTime = value;
						}

						var fireDeviceCountParam = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount");
						if (fireDeviceCountParam != null)
							zone.DetectorCount = int.Parse(fireDeviceCountParam.value);

						var autoSetParam = innerZone.param.FirstOrDefault(x => x.name == "AutoSet");
						if (autoSetParam != null)
						{
							int value;
							if (int.TryParse(autoSetParam.value, out value))
								zone.AutoSet = value;
						}

						var delayParam = innerZone.param.FirstOrDefault(x => x.name == "Delay");
						if (delayParam != null)
						{
							int value;
							if (int.TryParse(delayParam.value, out value))
								zone.Delay = value;
						}

						var skippedParam = innerZone.param.FirstOrDefault(x => x.name == "Skipped");
						if (skippedParam != null)
							zone.Skipped = skippedParam.value == "1" ? true : false;

						var enableExitTimeParam = innerZone.param.FirstOrDefault(x => x.name == "EnableExitTime");
						if (enableExitTimeParam != null)
							zone.EnableExitTime = enableExitTimeParam.value == "1" ? true : false;

						var exitRestoreTypeTypeParam = innerZone.param.FirstOrDefault(x => x.name == "ExitRestoreType");
						if (exitRestoreTypeTypeParam != null)
						{
							zone.ExitRestoreType = (exitRestoreTypeTypeParam.value == "0") ? ExitRestoreType.SetTimer : ExitRestoreType.RestoreTimer;
						}

						var guardZoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "GuardZoneType");
						if (guardZoneTypeParam != null)
						{
							zone.GuardZoneType = (GuardZoneType)int.Parse(guardZoneTypeParam.value);
						}
					}
					deviceConfiguration.Zones.Add(zone);
				}
			}
		}

		void ConvertZonesBack(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			var innerZones = new List<zoneType>();
			foreach (var zone in deviceConfiguration.Zones)
			{
				var innerZone = new zoneType()
				{
					name = zone.Name,
					idx = zone.No.ToString(),
					no = zone.No.ToString(),
					desc = zone.Description
				};
				if (innerZone.name != null)
					innerZone.name = innerZone.name.Replace('№', 'N');
				if (innerZone.desc != null)
					innerZone.desc = innerZone.desc.Replace('№', 'N');

				if (zone.ShapeIds != null && zone.ShapeIds.Count > 0)
				{
					var innerShapes = new List<shapeType>();
					foreach (var shapeId in zone.ShapeIds)
					{
						var innerShape = new shapeType()
						{
							id = shapeId
						};
						innerShapes.Add(innerShape);
					}
					innerZone.shape = innerShapes.ToArray();
				}

				var zoneParams = new List<paramType>();
				zoneParams.Add(new paramType()
				{
					name = "ZoneType",
					type = "Int",
					value = (zone.ZoneType == ZoneType.Fire) ? "0" : "1"
				});
				zoneParams.Add(new paramType()
				{
					name = "Skipped",
					type = "Bool",
					value = zone.Skipped ? "1" : "0"
				});
				zoneParams.Add(new paramType()
				{
					name = "EnableExitTime",
					type = "Bool",
					value = zone.EnableExitTime ? "1" : "0"
				});
				zoneParams.Add(new paramType()
				{
					name = "ExitRestoreType",
					type = "Int",
					value = (zone.ExitRestoreType == ExitRestoreType.SetTimer) ? "0" : "1"
				});
				zoneParams.Add(new paramType()
				{
					name = "GuardZoneType",
					type = "Int",
					value = ((int)zone.GuardZoneType).ToString()
				});
				if (zone.DetectorCount > 0)
				{
					zoneParams.Add(new paramType()
					{
						name = "FireDeviceCount",
						type = "Int",
						value = zone.DetectorCount.ToString()
					});
				}
				zoneParams.Add(new paramType()
				{
					name = "ExitTime",
					type = "SmallInt",
					value = zone.EvacuationTime.ToString()
				});
				zoneParams.Add(new paramType()
				{
					name = "AutoSet",
					type = "Int",
					value = zone.AutoSet.ToString()
				});
				zoneParams.Add(new paramType()
				{
					name = "Delay",
					type = "Int",
					value = zone.Delay.ToString()
				});

				if (zoneParams.Count > 0)
					innerZone.param = zoneParams.ToArray();

				innerZones.Add(innerZone);
			}

			if (innerZones.Count > 0)
				coreConfig.zone = innerZones.ToArray();
			else
				coreConfig.zone = null;
		}
	}
}