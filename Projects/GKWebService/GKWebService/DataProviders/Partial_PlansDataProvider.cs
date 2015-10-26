#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Media;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKWebService.Models;
using Infrastructure.Common.Services.Content;
using Infrustructure.Plans.Elements;

#endregion

namespace GKWebService.DataProviders
{
	public partial class PlansDataProvider
	{
		private static PlansDataProvider _instance;
		private readonly ContentService _contentService;

		private PlansDataProvider() {
			Plans = new List<PlanSimpl>();
			_contentService = new ContentService("GKOPC");
		}

		public static PlansDataProvider Instance {
			get {
				if (_instance != null) {
					return _instance;
				}
				return _instance = new PlansDataProvider();
			}
		}

		public List<PlanSimpl> Plans { get; set; }

		#region plan loading

		public void LoadPlans() {
			var plans = ClientManager.PlansConfiguration.Plans;

			Plans = new List<PlanSimpl>();

			// TODO: вынести динамику плана в отдельный код
			SafeFiresecService.GKCallbackResultEvent += SafeFiresecService_GKCallbackResultEvent;

			foreach (var plan in plans) {
				LoadPlan(plan);
			}
		}

		private void LoadPlan(Plan plan) {
			// Корень плана
			var planToAdd = new PlanSimpl {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				Elements = new List<PlanElement>()
			};

			// Добавляем сам план с фоном
			var planRootElement = new PlanElement {
				Border = InernalConverter.ConvertColor(Colors.Black),
				BorderThickness = 0,
				Fill = InernalConverter.ConvertColor(plan.BackgroundColor),
				Id = plan.UID,
				Name = plan.Caption,
				Hint = plan.Description,
				Path =
					"M 0 0 L " + plan.Width + " 0 L " + plan.Width +
					" " + plan.Height +
					" L 0 " + plan.Height + " L 0 0 z",
				Type = ShapeTypes.Plan.ToString(),
				Image = GetDrawingContent(
					plan.BackgroundImageSource,
					Convert.ToInt32(plan.Width),
					Convert.ToInt32(plan.Height)),
				Width = plan.Width,
				Height = plan.Height
			};

			planToAdd.Elements.Add(planRootElement);

			// Конвертим и добавляем прямоугольные элементы
			var rectangles = LoadRectangleElements(plan);
			planToAdd.Elements.AddRange(rectangles);

			// Конвертим и добавляем полигональные элементы
			var polygons = LoadPolygonElements(plan);
			planToAdd.Elements.AddRange(polygons);

			// Конвертим и добавляем линии-элементы
			var polylines = LoadPolyLineElements(plan);
			planToAdd.Elements.AddRange(polylines);

			//var doors = LoadDoorElements(plan);
			//planToAdd.Elements.AddRange(doors);

			// Конвертим и добавляем устройства
			foreach (var planElement in plan.ElementGKDevices) {
				var elemToAdd = DeviceToShape(planElement);
				planToAdd.Elements.Add(elemToAdd);
			}

			// TODO: законвертить остальные элементы

			Plans.Add(planToAdd);
		}

		//private IEnumerable<PlanElement> LoadDoorElements(Plan plan)
		//{
		//	var result = new List<PlanElement>();
		//	foreach (var door in plan.ElementGKDoors)
		//	{
		//		var elemToAdd = DoorToShape(door);
		//		elemToAdd.Hint = GetElementHint(door);
		//		result.Add(elemToAdd);
		//	}
		//	return result;
		//}

		//private PlanElement DoorToShape(ElementGKDoor item)
		//{
		//	GKDoor door =
		//		GKManager.Doors.FirstOrDefault(d => d.UID == item.DoorUID);
		//	if (door == null)
		//		return null;

		//	var bytes = GetDeviceStatePic(device);

		//	// Создаем элемент плана
		//	// Ширину и высоту задаем 500, т.к. знаем об этом
		//	var shape = new PlanElement
		//	{
		//		Name = item.PresentationName,
		//		Id = item.DeviceUID,
		//		Image = Convert.ToBase64String(bytes),
		//		Hint = GetElementHint(item),
		//		X = item.Left - 7,
		//		Y = item.Top - 7,
		//		Height = 14,
		//		Width = 14,
		//		Type = ShapeTypes.GkDevice.ToString()
		//	};
		//	return shape;
		//}

		private IEnumerable<PlanElement> LoadPolygonElements(Plan plan) {
			var result = new List<PlanElement>();
			var polygons =
				(from rect in plan.ElementPolygons
				 select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKZones
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKDelays
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKDirections
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKGuardZones
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKMPTs
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKSKDZones
						select rect as ElementBasePolygon);

			// Конвертим зоны-полигоны
			foreach (var polygon in polygons) {
				var elemToAdd = PolygonToShape(polygon);
				elemToAdd.Hint = GetElementHint(polygon);
				result.Add(elemToAdd);
			}
			return result;
		}

		private IEnumerable<PlanElement> LoadRectangleElements(Plan plan) {
			var result = new List<PlanElement>();
			var rectangles =
				(from rect in plan.ElementRectangles
				 select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKZones
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKDelays
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKDirections
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKGuardZones
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKMPTs
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKSKDZones
						select rect as ElementBaseRectangle);

			// Конвертим зоны-прямоугольники
			foreach (var rectangle in rectangles.ToList()) {
				var elemToAdd = RectangleToShape(rectangle);

				elemToAdd.Hint = GetElementHint(rectangle);

				result.Add(elemToAdd);
			}
			return result;
		}

		private string GetElementHint(ElementBase element) {
			if (element is IElementZone) {
				var asZone = element as IElementZone;
				if (element is ElementRectangleGKZone
				    || element is ElementPolygonGKZone) {
					var zone = GKManager.Zones.FirstOrDefault(z => asZone != null && z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							return zone.PresentationName;
						}
					}
				}
				if (element is ElementRectangleGKGuardZone
				    || element is ElementPolygonGKGuardZone) {
					var zone = GKManager.GuardZones.FirstOrDefault(z => asZone != null && z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							return zone.PresentationName;
						}
					}
				}
				if (element is ElementRectangleGKSKDZone
				    || element is ElementPolygonGKSKDZone) {
					var zone = GKManager.SKDZones.FirstOrDefault(z => asZone != null && z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							return zone.PresentationName;
						}
					}
				}
			}
			if (element is IElementMPT) {
				var asMPT = element as IElementMPT;
				var MPT = GKManager.MPTs.FirstOrDefault(m => asMPT != null && m.UID == asMPT.MPTUID);
				if (MPT != null) {
					if (MPT.PresentationName != null) {
						return MPT.PresentationName;
					}
				}
			}
			if (element is IElementDelay) {
				var asDelay = element as IElementDelay;
				var delay = GKManager.Delays.FirstOrDefault(m => asDelay != null && m.UID == asDelay.DelayUID);
				if (delay != null) {
					if (delay.PresentationName != null) {
						return delay.PresentationName;
					}
				}
			}
			if (element is IElementDirection) {
				var asDirection = element as IElementDirection;
				var direction = GKManager.Directions.FirstOrDefault(
					d => asDirection != null && d.UID == asDirection.DirectionUID);
				if (direction != null) {
					if (direction.PresentationName != null) {
						return direction.PresentationName;
					}
				}
			}
			if (element is ElementGKDevice) {
				var device = GKManager.Devices.FirstOrDefault(
					d => element != null && d.UID == (element as ElementGKDevice).DeviceUID);
				if (device != null) {
					if (device.PresentationName != null) {
						return device.PresentationName;
					}
				}
			}
			return string.Empty;
		}

		private IEnumerable<PlanElement> LoadPolyLineElements(Plan plan) {
			var result = new List<PlanElement>();
			var polylines = (from line in plan.ElementPolylines
			                 select line as ElementBasePolyline);

			// Конвертим зоны-полигоны
			foreach (var polyline in polylines) {
				var elemToAdd = PolylineToShape(polyline);
				elemToAdd.Hint = GetElementHint(polyline);
				result.Add(elemToAdd);
			}
			return result;
		}

		#endregion

		#region dynamic behaviour

		private void SafeFiresecService_GKCallbackResultEvent(GKCallbackResult obj) {
			var states = obj.GKStates;
			foreach (var state in states.DelayStates) {
			}
			foreach (var state in states.DeviceStates) {
				var updated = UpdateDeviceState(state);
			}
			foreach (var state in states.DirectionStates) {
			}
			foreach (var state in states.DoorStates) {
			}
			foreach (var state in states.GuardZoneStates) {
			}
			foreach (var state in states.MPTStates) {
			}
			foreach (var state in states.PumpStationStates) {
			}
			foreach (var state in states.SKDZoneStates) {
			}
			foreach (var state in states.ZoneStates) {
			}
		}

		private bool UpdateDeviceState(GKState state) {
			if (state.BaseObjectType != GKBaseObjectType.Device) {
				return false;
			}
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == state.UID);
			// Получаем обновленную картинку устройства
			var pictureTask = Task.Factory.StartNewSta(() => GetDeviceStatePic(device));
			Task.WaitAll();
			var pic = pictureTask.Result;
			if (pic == null) {
				return false;
			}
			var statusUpdate = new {
				Id = state.UID,
				Picture = Convert.ToBase64String(pic),
				StateClass = state.StateClass.ToString(),
				state.StateClasses,
				state.AdditionalStates
			};
			PlansUpdater.Instance.UpdateDeviceState(statusUpdate);
			return true;
		}

		#endregion
	}
}
