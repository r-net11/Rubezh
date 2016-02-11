#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GKWebService.Models.Plan;
using GKWebService.Models.Plan.PlanElement;
using GKWebService.Models.Plan.PlanElement.Hint;
using GKWebService.Utils;
using Infrastructure.Common.Services.Content;
using Infrustructure.Plans.Elements;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

#endregion

namespace GKWebService.DataProviders.Plan
{
	public class PlansDataProvider
	{
		#region Deferred Loading

		/// <summary>
		/// Get list of available plans in active configuration.
		/// </summary>
		/// <returns>Flattened list of all plans.</returns>
		public IEnumerable<PlanSimpl> GetPlansList() {
			if (ClientManager.PlansConfiguration == null
				|| ClientManager.PlansConfiguration.Plans == null) {
				return null;
			}
			var plans = ClientManager.PlansConfiguration.Plans;
			return plans.Select(GetPlanInfo).ToList();
		}

		private PlanSimpl GetPlanInfo(RubezhAPI.Models.Plan plan) {
			// Корень плана
			return new PlanSimpl {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				NestedPlans = plan.Children != null ? plan.Children.Select(GetPlanInfo) : null,
				ParentUid = plan.Parent != null ? plan.Parent.UID : (Guid?)null,
				IsFolder = plan is PlanFolder
			};
		}

		public PlanSimpl GetPlan(Guid planId) {
			var plan = ClientManager.PlansConfiguration.Plans.FirstOrDefault(p => p.UID == planId);
			// Корень плана
			if (plan == null)
				throw new KeyNotFoundException(string.Format("План с ID {0} не найден, либо недоступен.", planId));

			var planToAdd = new PlanSimpl {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				Elements = new List<PlanElement>()
			};

			// Добавляем сам план с фоном и все элементы
			var planRootElement = LoadPlanRoot(plan);
			planToAdd.Elements.Add(planRootElement);
			var planSubElements = LoadPlanSubElements(plan);
			planToAdd.Elements.AddRange(planSubElements);

			return planToAdd;
		}
		#endregion

		public void LoadPlans() {
			if (ClientManager.PlansConfiguration == null
				|| ClientManager.PlansConfiguration.Plans == null) {
				return;
			}
			var plans = ClientManager.PlansConfiguration.Plans;
			Plans = new List<PlanSimpl>();
			foreach (var plan in plans) {
				LoadPlan(plan);
			}
			SafeFiresecService.GKCallbackResultEvent += OnServiceCallback;
		}

		public void LoadPlan(RubezhAPI.Models.Plan plan) {
			// Корень плана
			var planToAdd = new PlanSimpl {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				Elements = new List<PlanElement>()
			};

			// Добавляем сам план с фоном и все элементы
			var planRootElement = LoadPlanRoot(plan);
			planToAdd.Elements.Add(planRootElement);
			var planSubElements = LoadPlanSubElements(plan);
			planToAdd.Elements.AddRange(planSubElements);

			Plans.Add(planToAdd);
		}

		private PlanElement LoadPlanRoot(RubezhAPI.Models.Plan plan) {
			return new PlanElement {
				Border = InternalConverterOld.ConvertColor(Colors.Black),
				BorderThickness = 0,
				Fill = InternalConverterOld.ConvertColor(plan.BackgroundColor),
				Id = plan.UID,
				Name = plan.Caption,
				Path =
					"M 0 0 L " + plan.Width + " 0 L " + plan.Width +
					" " + plan.Height +
					" L 0 " + plan.Height + " L 0 0 z",
				Type = ShapeTypes.Plan.ToString(),
				Image = RenderPlanBackgound(
				plan.BackgroundImageSource,
				Convert.ToInt32(plan.Width),
				Convert.ToInt32(plan.Height)),
				Width = plan.Width,
				Height = plan.Height
			};
		}

		private IEnumerable<PlanElement> LoadPlanSubElements(RubezhAPI.Models.Plan plan) {
			var rectangles = LoadRectangleElements(plan);
			//var polygons = LoadPolygonElements(plan);
			//var polylines = LoadPolyLineElements(plan);
			var ellipses = LoadEllipseElements(plan);
			var textBlocks = LoadStaticTextElements(plan);
			var doors = LoadDoorElements(plan);
			var devices = plan.ElementGKDevices.Select(PlanElement.FromDevice);

			return textBlocks.Concat(rectangles).Concat(ellipses).Concat(doors).Concat(devices);
		}

		private IEnumerable<PlanElement> LoadPolygonElements(RubezhAPI.Models.Plan plan) {
			var polygons = plan.AllElements.Where(elem => elem is ElementBasePolygon);

			// Конвертим зоны-полигоны
			return polygons.Select(elem => PlanElement.FromPolygon(elem as ElementBasePolygon));
		}

		private IEnumerable<PlanElement> LoadRectangleElements(RubezhAPI.Models.Plan plan) {
			var rectangles = plan.AllElements.Where(elem => elem is ElementBaseRectangle && !(elem is IElementTextBlock));

			// Конвертим зоны-прямоугольники
			return rectangles.ToList().Select(elem => PlanElement.FromRectangle(elem as ElementBaseRectangle)).Where(elem => elem != null);
		}

		private IEnumerable<PlanElement> LoadPolyLineElements(RubezhAPI.Models.Plan plan) {
			// Конвертим зоны-полигоны
			return plan.ElementPolylines.Select(PlanElement.FromPolyline);
		}

		private IEnumerable<PlanElement> LoadEllipseElements(RubezhAPI.Models.Plan plan) {
			// Конвертим зоны-эллипсы
			return plan.ElementEllipses.ToList().Select(PlanElement.FromEllipse);
		}

		/// <summary>
		/// Преобразует статические текcтовые элементы
		/// (ElementTextBlock, ElementProcedure).
		/// </summary>
		/// <param name="plan">План</param>
		/// <returns>Элемент-группа, содержащий групповой элемент, внутри которого текст и прямоугольник.</returns>
		private IEnumerable<PlanElement> LoadStaticTextElements(RubezhAPI.Models.Plan plan) {
			var textBlockElements = plan.ElementTextBlocks;
			var procedureElements = plan.AllElements.OfType<ElementProcedure>();
			return textBlockElements.Select(
				PlanElement.FromTextBlock).Where(elem => elem != null).Union(procedureElements.Select(PlanElement.FromProcedure).Where(elem => elem != null));
		}

		private IEnumerable<PlanElement> LoadDoorElements(RubezhAPI.Models.Plan plan) {
			// Конвертим зоны-прямоугольники
			return plan.ElementGKDoors.ToList().Select(PlanElement.FromGkDoor);
		}

		private void OnServiceCallback(GKCallbackResult obj) {
			var states = obj.GKStates;
			foreach (var state in states.DeviceStates) {
				PlanElement.UpdateDeviceState(state);
			}
			//foreach (var state in states.DelayStates) {
			//}
			//foreach (var state in states.DirectionStates) {
			//}
			//foreach (var state in states.DoorStates) {
			//}
			//foreach (var state in states.GuardZoneStates) {
			//}
			//foreach (var state in states.MPTStates) {
			//}
			//foreach (var state in states.PumpStationStates) {
			//}
			//foreach (var state in states.SKDZoneStates) {
			//}
			//foreach (var state in states.ZoneStates) {
			//}
		}

		/// <summary>
		///     Получает преобразованное в Base64String png-изображение фона плана
		/// </summary>
		/// <param name="source">GUID плана</param>
		/// <param name="width">Ширина плана</param>
		/// <param name="height">Высота плана</param>
		/// <returns></returns>
		private string RenderPlanBackgound(Guid? source, int width, int height) {
			Drawing drawing = null;
			Canvas canvas = null;
			if (source.HasValue) {

				try {
					drawing = _contentService.GetDrawing(source.Value);
				}
				catch (Exception) {
					canvas = _contentService.GetObject<Canvas>(source.Value);
					if (canvas == null) {
						return string.Empty;
					}
				}
			}
			else {
				return string.Empty;
			}
			if (drawing == null) {
				if (canvas == null) {
					return string.Empty;
				}
				return InternalConverter.XamlCanvasToPngBase64(canvas, width, height);

			}
			drawing.Freeze();

			return InternalConverterOld.XamlDrawingToPngBase64String(width, height, drawing);
		}

		#region ctor, props

		private static PlansDataProvider _instance;
		private readonly ContentService _contentService;

		private PlansDataProvider() {
			Plans = new List<PlanSimpl>();
			_contentService = new ContentService("Sergey_GKOPC");
			SafeFiresecService.GKCallbackResultEvent += OnServiceCallback;
		}

		public static PlansDataProvider Instance
		{
			get
			{
				if (_instance != null) {
					return _instance;
				}
				return _instance = new PlansDataProvider();
			}
		}

		public void GetInfo() {
			Debug.WriteLine(string.Format("Get Info thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState().ToString()));

		}

		public List<PlanSimpl> Plans { get; set; }

		#endregion
	}
}
