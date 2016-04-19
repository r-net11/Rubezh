using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Common;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.PassCard.Designer;
using System.Reflection;
using System.ComponentModel;
using Infrustructure.Plans.Painters;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardViewModel : DialogViewModel, IPlanDesignerViewModel
	{
		PassCardCanvas _passCardCanvas;
		Employee _employee;
		Department _department;
		Position _position;
		OrganisationDetails _organisation;
		SKDCard _card;

		public PassCardViewModel(Guid employeeUID, SKDCard card)
		{
			Title = "Печать удостоверения " + card.Number;
			_card = card;
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			_employee = EmployeeHelper.GetDetails(employeeUID);
			if (_employee.DepartmentUID != Guid.Empty && !_employee.IsDepartmentDeleted)
				_department = DepartmentHelper.GetDetails(_employee.DepartmentUID);
			if (_employee.PositionUID != Guid.Empty && !_employee.IsPositionDeleted)
				_position = PositionHelper.GetDetails(_employee.PositionUID);
			_organisation = OrganisationHelper.GetDetails(_employee.OrganisationUID);
			_passCardCanvas = new PassCardCanvas();
			PassCardTemplates = new ObservableCollection<ShortPassCardTemplate>(PassCardTemplateHelper.GetByOrganisation(_organisation.UID));
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			SelectedPassCardTemplate = PassCardTemplates.FirstOrDefault();
		}

		public ObservableCollection<ShortPassCardTemplate> PassCardTemplates { get; private set; }

		ShortPassCardTemplate _selectedPassCardTemplate;
		public ShortPassCardTemplate SelectedPassCardTemplate
		{
			get { return _selectedPassCardTemplate; }
			set
			{
				using (new TimeCounter("PassCardsDesignerViewModel.SelectedPlan: {0}", true, true))
					if (value != SelectedPassCardTemplate)
					{
						_selectedPassCardTemplate = value;
						OnPropertyChanged(() => SelectedPassCardTemplate);
						OnPropertyChanged(() => IsNotEmpty);
						InternalCreatePassCard();
					}
			}
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			var dialog = new PrintDialog();
			if (dialog.ShowDialog() == true)
			{
				var rect = LayoutInformation.GetLayoutSlot(_passCardCanvas);
				var capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);
				var origin = new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight);
				var size = new Size(_passCardCanvas.DesiredSize.Width + 2 * _passCardCanvas.CanvasBorder.Thickness, _passCardCanvas.DesiredSize.Height + 2 * _passCardCanvas.CanvasBorder.Thickness);
				_passCardCanvas.Arrange(new Rect(origin, size));
				dialog.PrintVisual(_passCardCanvas, "Пропуск " + _employee.LastName);
				_passCardCanvas.Arrange(rect);
			}
		}
		bool CanPrint()
		{
			return SelectedPassCardTemplate != null;
		}

		void InternalCreatePassCard()
		{
			using (new WaitWrapper())
				if (SelectedPassCardTemplate != null)
				{
					var passCardTemplate = PassCardTemplateHelper.GetDetails(SelectedPassCardTemplate.UID);
					foreach (var passCardImage in passCardTemplate.PassCardImages)
					{
						ServiceFactory.ContentService.AddContent(passCardImage.Image, passCardImage.ImageUID);
					}
					_passCardCanvas.Initialize(passCardTemplate);
					foreach (var elementBase in EnumerateElements(passCardTemplate))
					{
						var presenterItem = _passCardCanvas.CreatePresenterItem(elementBase);
						var primitive = elementBase as IPrimitive;
						if (primitive != null && primitive.Primitive != Primitive.NotPrimitive)
						{
							var painter = PrimitivePainterFactory.CreatePainter(_passCardCanvas, elementBase);
							presenterItem.OverridePainter(painter);
						}
					}
					Update();
					_passCardCanvas.LoadingFinished();
					_passCardCanvas.Refresh();
				}
		}
		void Update()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
		IEnumerable<ElementBase> EnumerateElements(PassCardTemplate passCardTemplate)
		{
			foreach (var elementTextProperty in passCardTemplate.ElementTextProperties)
			{
				ResolveTextProperty(elementTextProperty);
				yield return elementTextProperty;
			}
			foreach (var elementImageProperty in passCardTemplate.ElementImageProperties)
			{
				ResolveImageProperty(elementImageProperty);
				yield return elementImageProperty;
			}
			foreach (var elementRectangle in passCardTemplate.ElementRectangles)
				yield return elementRectangle;
			foreach (var elementEllipse in passCardTemplate.ElementEllipses)
				yield return elementEllipse;
			foreach (var elementTextBlock in passCardTemplate.ElementTextBlocks)
				yield return elementTextBlock;
			foreach (var elementPolygon in passCardTemplate.ElementPolygons)
				yield return elementPolygon;
			foreach (var elementPolyline in passCardTemplate.ElementPolylines)
				yield return elementPolyline;
		}

		void ResolveTextProperty(ElementPassCardTextProperty elementTextProperty)
		{
			elementTextProperty.PresentationName = GetEnumDescription(elementTextProperty.PropertyType);
			switch (elementTextProperty.PropertyType)
			{
				case PassCardTextPropertyType.Birthday:
					elementTextProperty.Text = _employee.BirthDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.Department:
					elementTextProperty.Text = _department != null ? _department.Name : "";
					break;
				case PassCardTextPropertyType.EndDate:
					elementTextProperty.Text = _card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.FirstName:
					elementTextProperty.Text = _employee.FirstName;
					break;
				case PassCardTextPropertyType.LastName:
					elementTextProperty.Text = _employee.LastName;
					break;
				case PassCardTextPropertyType.Organisation:
					elementTextProperty.Text = _organisation.Name;
					break;
				case PassCardTextPropertyType.Position:
					elementTextProperty.Text = _position != null ? _position.Name : "";
					break;
				case PassCardTextPropertyType.SecondName:
					elementTextProperty.Text = _employee.SecondName;
					break;
				case PassCardTextPropertyType.CardNumber:
					elementTextProperty.Text = _card.Number.ToString();
					break;
				case PassCardTextPropertyType.Additional:
					var columnValue = _employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnTypeUID == elementTextProperty.AdditionalColumnUID);
					elementTextProperty.Text = columnValue == null || columnValue.TextData == null ? string.Empty : columnValue.TextData;
					break;
				default:
					elementTextProperty.Text = string.Empty;
					break;
			}
		}
		void ResolveImageProperty(ElementPassCardImageProperty elementImageProperty)
		{
			elementImageProperty.PresentationName = GetEnumDescription(elementImageProperty.PropertyType);
			//elementImageProperty.BackgroundColor = Colors.Transparent;
			//elementImageProperty.BackgroundSourceName = null;
			//elementImageProperty.BackgroundImageSource = null;
		}

		string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes =
			  (DescriptionAttribute[])fi.GetCustomAttributes
			  (typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}

		#region IPlanDesignerViewModel Members

		public event EventHandler Updated;

		public bool IsNotEmpty
		{
			get { return SelectedPassCardTemplate != null; }
		}
		public object Toolbox
		{
			get { return null; }
		}
		public CommonDesignerCanvas Canvas
		{
			get { return _passCardCanvas; }
		}
		public bool AllowScalePoint
		{
			get { return false; }
		}
		public bool ShowZoomSliders
		{
			get { return true; }
		}
		public bool AllowChangePlanZoom
		{
			get { return true; }
		}
		public double DeviceZoom { get; set; }
		public bool FullScreenSize
		{
			get { return false; }
		}

		public void ChangeZoom(double zoom)
		{
		}
		public void ChangeDeviceZoom(double deviceZoom)
		{
		}
		public void ResetZoom(double zoom, double deviceZoom)
		{
			ChangeZoom(zoom);
		}

		public bool HasPermissionsToScale
		{
			get { return true; }
		}
		public bool AlwaysShowScroll
		{
			get { return false; }
		}

		public bool CanCollapse
		{
			get { return false; }
		}
		public bool IsCollapsed { get; set; }

		#endregion

		void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (args.DesignerCanvas != _passCardCanvas)
				return;
			var elementPassCardImageProperty = args.Element as ElementPassCardImageProperty;
			if (elementPassCardImageProperty != null)
			{
				Photo photo = null;
				switch (elementPassCardImageProperty.PropertyType)
				{
					case PassCardImagePropertyType.DepartmentLogo:
						photo = _department == null ? null : _department.Photo;
						break;
					case PassCardImagePropertyType.OrganisationLogo:
						photo = _organisation.Photo;
						break;
					case PassCardImagePropertyType.Photo:
						photo = _employee.Photo;
						break;
					case PassCardImagePropertyType.PositionLogo:
						photo = _position == null ? null : _position.Photo;
						break;
					case PassCardImagePropertyType.Additional:
						var columnValue = _employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnTypeUID == elementPassCardImageProperty.AdditionalColumnUID);
						if (columnValue != null)
							photo = columnValue.Photo;
						break;
					default:
						break;
				}
				args.Painter = new PassCardImagePropertyPainter(_passCardCanvas, elementPassCardImageProperty, photo == null || photo.Data == null || photo.Data.Count() == 0 ? null : photo.Data);
			}
		}
	}
}