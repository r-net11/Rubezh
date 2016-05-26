using Common;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.PassCard.Designer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardViewModel : SaveCancelDialogViewModel, IPlanDesignerViewModel
	{
		private PassCardCanvas _passCardCanvas;
		private Employee _employee;
		private Department _department;
		private Position _position;
		private OrganisationDetails _organisation;
		private SKDCard _card;

		public PassCardViewModel(Guid employeeUID, SKDCard card)
		{
			Title = "Печать удостоверения " + card.Number;
			_card = card;
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			_employee = EmployeeHelper.GetDetails(employeeUID);
			var shortDepartment = _employee.Department;
			if (shortDepartment != null)
				_department = DepartmentHelper.GetDetails(shortDepartment.UID);
			var shortPosition = _employee.Position;
			if (shortPosition != null)
				_position = PositionHelper.GetDetails(shortPosition.UID);
			_organisation = OrganisationHelper.GetDetails(_employee.OrganisationUID);
			_passCardCanvas = new PassCardCanvas();
			PassCardTemplates = new ObservableCollection<ShortPassCardTemplate>(PassCardTemplateHelper.GetByOrganisation(_organisation.UID));

			ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);

			var uid = _card.PassCardTemplateUID;
			SelectedPassCardTemplate = uid.HasValue ? PassCardTemplates.FirstOrDefault(item => item.UID == uid.Value) : null;
		}

		public ObservableCollection<ShortPassCardTemplate> PassCardTemplates { get; private set; }

		private ShortPassCardTemplate _selectedPassCardTemplate;
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
		private void OnPrint()
		{
			var dialog = new PrintDialog();
			if (dialog.ShowDialog() == true)
			{
				var rect = LayoutInformation.GetLayoutSlot(_passCardCanvas);
				var capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);
				if (capabilities.PageImageableArea != null)
				{
					var origin = new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight);
					var size = new Size(_passCardCanvas.DesiredSize.Width + 2 * _passCardCanvas.CanvasBorder.Thickness, _passCardCanvas.DesiredSize.Height + 2 * _passCardCanvas.CanvasBorder.Thickness);
					_passCardCanvas.Arrange(new Rect(origin, size));
				}
				dialog.PrintVisual(_passCardCanvas, "Пропуск " + _employee.LastName);
				_passCardCanvas.Arrange(rect);
			}
		}
		private bool CanPrint()
		{
			return SelectedPassCardTemplate != null;
		}

		private void InternalCreatePassCard()
		{
			if (SelectedPassCardTemplate != null)
			{
				var template = PassCardTemplateHelper.GetDetails(SelectedPassCardTemplate.UID);
				using (new TimeCounter("\t\tPassCardCanvas.Initialize: {0}"))
					_passCardCanvas.Initialize(template);
				using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
					foreach (var elementBase in EnumerateElements(template))
						_passCardCanvas.CreatePresenterItem(elementBase);
				using (new TimeCounter("\t\tPassCardViewModel.OnUpdated: {0}"))
					Update();
				_passCardCanvas.LoadingFinished();
				_passCardCanvas.Refresh();
			}
		}
		private void Update()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
		private IEnumerable<ElementBase> EnumerateElements(PassCardTemplate passCardTemplate)
		{
			foreach (var elementTextProperty in passCardTemplate.ElementTextProperties)
			{
				ResolveTextProperty(elementTextProperty);
				yield return elementTextProperty;
			}
			foreach (var elementImageProperty in passCardTemplate.ElementImageProperties)
			{
			//	ResolveImageProperty(elementImageProperty);
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

		private void ResolveTextProperty(ElementPassCardTextProperty elementTextProperty)
		{
			switch (elementTextProperty.PropertyType)
			{
				case PassCardTextPropertyType.Birthday:
					elementTextProperty.Text = "[Пока нет в БД]";
					break;
				case PassCardTextPropertyType.Department:
					if (_department != null)
						elementTextProperty.Text = _department.Name;
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
					if (_position != null)
						elementTextProperty.Text = _position.Name;
					break;
				case PassCardTextPropertyType.SecondName:
					elementTextProperty.Text = _employee.SecondName;
					break;
				case PassCardTextPropertyType.StartDate:
					elementTextProperty.Text = _card.StartDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.CardNumber:
					elementTextProperty.Text = _card.Number.ToString();
					break;
				case PassCardTextPropertyType.Additional:
					var columnValue = _employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnType.UID == elementTextProperty.AdditionalColumnUID);
					elementTextProperty.Text = columnValue == null || columnValue.TextData == null ? string.Empty : columnValue.TextData;
					break;
				default:
					elementTextProperty.Text = string.Empty;
					break;
			}
		}
		//private void ResolveImageProperty(ElementPassCardImageProperty elementImageProperty)
		//{
		//	//elementImageProperty.BackgroundColor = Colors.Transparent;
		//	//elementImageProperty.BackgroundSourceName = null;
		//	//elementImageProperty.BackgroundImageSource = null;
		//}

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

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (!Equals(args.DesignerCanvas, _passCardCanvas))
				return;
			var elementPassCardImageProperty = args.Element as ElementPassCardImageProperty;
			if (elementPassCardImageProperty == null) return;

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
					var columnValue = _employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnType.UID == elementPassCardImageProperty.AdditionalColumnUID);
					if (columnValue != null)
						photo = columnValue.Photo;
					break;
			}
			args.Painter = new PassCardImagePropertyPainter(_passCardCanvas, elementPassCardImageProperty, photo == null || photo.Data == null || photo.Data.Count() == 0 ? null : photo.Data);
		}

		protected override bool Save()
		{
			var cardTemplateUID = SelectedPassCardTemplate == null ? null : (Guid?)SelectedPassCardTemplate.UID;
			_card.PassCardTemplateUID = cardTemplateUID;
			return CardHelper.SaveTemplate(_card);
		}
	}
}