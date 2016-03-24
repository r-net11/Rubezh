using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.Door;
using GKWebService.Models.FireZone;
using GKWebService.Models.PumpStation;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.Journal;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GKWebService.Controllers
{
	[Authorize]
	public class JournalController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult JournalFilter()
		{
			return View();
		}

		[HttpPost]
		public JsonResult GetJournal(GKWebService.Models.JournalFilter filter)
		{
			var journalFilter = CreateApiFilter(filter);
			var apiItems = JournalHelper.Get(journalFilter);
			var list = apiItems.Select(x => new JournalModel(x)).ToList();
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetFilter()
		{
			var result = new JournalFilterJson
			{
				MinDate = DateTime.Now.AddDays(-7),
				MaxDate = DateTime.Now,
				Events = GetFilterEvents(),
				Objects = GetFilterObjects()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public static List<JournalFilterObject> GetFilterObjects()
		{
			var result = new List<JournalFilterObject>();
			result.Add(new JournalFilterObject(new Guid("98BEF3DB-0E77-4AB4-BCFB-917918DFC726"), "Icon/SubsystemTypes/Chip.png", "ГК", 0));
			result.Add(new JournalFilterObject(new Guid("DBC956A1-37BD-433E-97F0-99D2BC27741F"), "GKIcons/GKRele.png", "Устройства", 1));
			BuildDeviceTree(GKManager.DeviceConfiguration.RootDevice, null, result);
			result.Add(new JournalFilterObject(new Guid("F28203B1-3DE1-48B6-B0AB-C65C9DAF9AC4"), "Images/Zone.png", "Зоны", 1));
			result.AddRange(GKManager.Zones.Select(x => new JournalFilterObject(x.UID, "Images/Zone.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("D0676597-5CF8-4969-8F74-7BF2831BF8D4"), "Images/BDirection.png", "Направления", 1));
			result.AddRange(GKManager.Directions.Select(x => new JournalFilterObject(x.UID, "Images/BDirection.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("AD76FEE7-BE53-48A4-8B92-802303ECA801"), "Images/BMPT.png", "МПТ", 1));
			result.AddRange(GKManager.MPTs.Select(x => new JournalFilterObject(x.UID, "Images/BMPT.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("022EF437-56FB-4060-AB03-256B3F206F2A"), "Images/BPumpStation.png", "Насосные станции", 1));
			result.AddRange(GKManager.PumpStations.Select(x => new JournalFilterObject(x.UID, "Images/BPumpStation.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("367BC728-2997-48FD-92C0-F527C30DCF0C"), "Images/Delay.png", "Задержки", 1));
			result.AddRange(GKManager.Delays.Select(x => new JournalFilterObject(x.UID, "Images/Delay.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("37293547-A1AB-4547-8E67-D5993A628CEE"), "Images/GuardZone.png", "Охранные зоны", 1));
			result.AddRange(GKManager.GuardZones.Select(x => new JournalFilterObject(x.UID, "Images/GuardZone.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("0191FEF3-7EE5-4D88-99FC-B91B5D9C4B51"), "Images/SKDZone.png", "Зоны СКД", 1));
			result.AddRange(GKManager.SKDZones.Select(x => new JournalFilterObject(x.UID, "Images/SKDZone.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("24D0399E-7BF4-4042-82D7-1B0B79DF5CDA"), "Images/Door.png", "Точки доступа", 1));
			result.AddRange(GKManager.Doors.Select(x => new JournalFilterObject(x.UID, "Images/Door.png", x.PresentationName, 2)));
			result.Add(new JournalFilterObject(new Guid("D8997692-0858-43CA-88B5-4AFB537D2BE2"), "Images/BVideo.png", "Видео", 0));
			result.Add(new JournalFilterObject(new Guid("DBC956A1-37BD-433E-97F0-99D2BC27741F"), "Images/BVideo.png", "Видеоустройства", 1));
			result.AddRange(RubezhClient.ClientManager.SystemConfiguration.Cameras.Select(x => new JournalFilterObject(x.UID, "Images/BVideo.png", x.PresentationName, 2)));
			return result;
		}

		public static List<JournalFilterEvent> GetFilterEvents()
		{
			var result = new List<JournalFilterEvent>();
			foreach (JournalSubsystemType journalSubsystemType in Enum.GetValues(typeof(JournalSubsystemType)))
			{
				result.Add(new JournalFilterEvent(0, (int)journalSubsystemType, journalSubsystemType.ToDescription(), "Icon/SubsystemTypes/" + GetSubsystemImage(journalSubsystemType) + ".png", 0, -1));
				foreach (JournalEventNameType journalEventNameType in Enum.GetValues(typeof(JournalEventNameType)))
				{
					var nameFieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
					if (nameFieldInfo != null)
					{
						var descriptionAttributes = (EventNameAttribute[])nameFieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
						if (descriptionAttributes.Length > 0)
						{
							var eventNameAttribute = descriptionAttributes[0];
							var subsystemType = eventNameAttribute.JournalSubsystemType;
							if (subsystemType == journalSubsystemType)
							{
								result.Add(new JournalFilterEvent(1, (int)journalEventNameType, journalEventNameType.ToDescription(), "Icon/GKStateIcons/" + EventDescriptionAttributeHelper.ToStateClass(journalEventNameType) + ".png", 1, (int)journalSubsystemType));
								foreach (JournalEventDescriptionType journalEventDescriptionType in Enum.GetValues(typeof(JournalEventDescriptionType)))
								{
									var descriptionFieldInfo = journalEventDescriptionType.GetType().GetField(journalEventDescriptionType.ToString());
									if (descriptionFieldInfo != null)
									{
										var eventDescriptionAttributes = (EventDescriptionAttribute[])descriptionFieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
										if (eventDescriptionAttributes.Length > 0)
										{
											var eventDescriptionAttribute = eventDescriptionAttributes[0];
											if (eventDescriptionAttribute.JournalEventNameTypes.Any(x => x == journalEventNameType))
												result.Add(new JournalFilterEvent(2, (int)journalEventDescriptionType, journalEventDescriptionType.ToDescription(), "Images/blank.png", 2, (int)journalEventNameType));
										}

									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		static void BuildDeviceTree(RubezhAPI.GK.GKDevice apiDevice, JournalFilterObject parent, List<JournalFilterObject> devices)
		{
			if (apiDevice == null)
				return;
			var device = new JournalFilterObject(apiDevice.UID, apiDevice.ImageSource.Replace("/Controls;component/", ""), apiDevice.PresentationName, parent != null ? parent.Level + 1 : 2);
			devices.Add(device);
			foreach (var child in apiDevice.Children)
			{
				BuildDeviceTree(child, device, devices);
			}
		}

		public static string GetSubsystemImage(JournalSubsystemType journalSubsystemType)
		{
			switch (journalSubsystemType)
			{
				case JournalSubsystemType.System:
					return "PC";
				case JournalSubsystemType.GK:
					return "Chip";
				case JournalSubsystemType.SKD:
					return "Controller";
				case JournalSubsystemType.Video:
					return "Camera";
				default:
					return "no";
			}
		}

		public static RubezhAPI.Journal.JournalFilter CreateApiFilter(GKWebService.Models.JournalFilter filter, bool useDateTime = false)
		{
			var journalFilter = new RubezhAPI.Journal.JournalFilter();
			if (filter != null)
			{
				if (filter.ObjectUids != null)
					journalFilter.ObjectUIDs = filter.ObjectUids;
				if (filter.Events != null)
				{
					journalFilter.JournalSubsystemTypes.AddRange(filter.Events.Where(x => x.Type == 0).Select(x => (JournalSubsystemType)x.Value));
					journalFilter.JournalEventNameTypes.AddRange(filter.Events.Where(x => x.Type == 1).Select(x => (JournalEventNameType)x.Value));
					foreach (var item in filter.Events.Where(x => x.Type == 2).GroupBy(x => x.ParentValue))
						journalFilter.EventDescriptions.Add(new EventDescriptions
						{
							JournalEventNameType = (JournalEventNameType)item.Key,
							JournalEventDescriptionTypes = item.Select(x => (JournalEventDescriptionType)x.Value).ToList()
						});
				}
				if (useDateTime)
				{
					journalFilter.StartDate = filter.BeginDate.HasValue ? filter.BeginDate.Value : DateTime.Now.AddDays(-1);
					journalFilter.EndDate = filter.EndDate.HasValue ? filter.EndDate.Value : DateTime.Now;
				}
			};
			return journalFilter;
		}


		[HttpPost]
		public JsonResult GetModelBase(Guid uid, int objectType)
		{
			GKBaseModel result = null;
			switch ((JournalObjectType)objectType)
			{
				case JournalObjectType.GKDevice:
					var device = GKManager.Devices.FirstOrDefault(x => x.UID == uid);
					if (device != null)
						result = new Device(device);
					break;
				case JournalObjectType.GKZone:
					var gkZone = GKManager.Zones.FirstOrDefault(x => x.UID == uid);
					if (gkZone != null)
						result = new FireZone(gkZone);
					break;
				case JournalObjectType.GKDirection:
					var direction = GKManager.Directions.FirstOrDefault(x => x.UID == uid);
					if (direction != null)
						result = new Direction(direction);
					break;
				case JournalObjectType.GKMPT:
					var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == uid);
					if (mpt != null)
						result = new MPTModel(mpt);
					break;
				case JournalObjectType.GKPumpStation:
					var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == uid);
					if (pumpStation != null)
						result = new PumpStation(pumpStation);
					break;
				case JournalObjectType.GKDelay:
					var delay = GKManager.Delays.FirstOrDefault(x => x.UID == uid);
					if (delay != null)
						result = new Delay(delay);
					break;
				case JournalObjectType.GKDoor:
					var door = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
					if (door != null)
						result = new Door(door);
					break;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}