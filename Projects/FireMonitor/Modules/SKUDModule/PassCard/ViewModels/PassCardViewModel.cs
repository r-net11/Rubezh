using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Common;
using FiresecAPI;
using FiresecAPI.SKD.PassCardLibrary;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.PassCard.Designer;
using SKDModule.ViewModels;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		private PassCardCanvas _passCardCanvas;
		private EmployeesViewModel _employeesViewMode;
		private bool _isLoaded;

		public PassCardViewModel(EmployeesViewModel employeesViewModel)
		{
			_employeesViewMode = employeesViewModel;
			_passCardCanvas = new PassCardCanvas();
			SKDManager.SKDPassCardLibraryConfiguration.Templates.Sort((item1, item2) => string.Compare(item1.Caption, item2.Caption));
			PassCardTemplates = new ObservableCollection<PassCardTemplate>(SKDManager.SKDPassCardLibraryConfiguration.Templates);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
		}

		public ObservableCollection<PassCardTemplate> PassCardTemplates { get; private set; }

		private PassCardTemplate _selectedPassCardTemplate;
		public PassCardTemplate SelectedPassCardTemplate
		{
			get { return _selectedPassCardTemplate; }
			set
			{
				using (new TimeCounter("PassCardsDesignerViewModel.SelectedPlan: {0}", true, true))
				{
					_selectedPassCardTemplate = value;
					OnPropertyChanged(() => SelectedPassCardTemplate);
					OnPropertyChanged(() => IsNotEmpty);
					InternalCreatePassCard();
					if (_isLoaded)
						_employeesViewMode.SelectedCard.SaveCardTemplate(SelectedPassCardTemplate == null ? null : (Guid?)SelectedPassCardTemplate.UID);
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
				var origin = new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight);
				_passCardCanvas.Arrange(new Rect(origin, _passCardCanvas.DesiredSize));
				dialog.PrintVisual(_passCardCanvas, "Пропуск " + _employeesViewMode.SelectedCard.EmployeeViewModel.EmployeeListItem.LastName);
				_passCardCanvas.Arrange(rect);
			}
		}
		private bool CanPrint()
		{
			return SelectedPassCardTemplate != null;
		}

		public void CreatePassCard()
		{
			_isLoaded = false;
			var uid = _employeesViewMode.IsCard && _employeesViewMode.SelectedCard != null ? _employeesViewMode.SelectedCard.Card.CardTemplateUID : null;
			SelectedPassCardTemplate = uid.HasValue ? PassCardTemplates.FirstOrDefault(item => item.UID == uid.Value) : null;
			_isLoaded = true;
		}

		private void InternalCreatePassCard()
		{
			using (new WaitWrapper())
				if (SelectedPassCardTemplate != null)
				{
					using (new TimeCounter("\t\tPassCardCanvas.Initialize: {0}"))
						_passCardCanvas.Initialize(SelectedPassCardTemplate);
					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
						foreach (var elementBase in EnumerateElements())
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
		private IEnumerable<ElementBase> EnumerateElements()
		{
			foreach (var elementTextProperty in SelectedPassCardTemplate.ElementTextProperties)
			{
				ResolveTextProperty(elementTextProperty);
				yield return elementTextProperty;
			}
			foreach (var elementImageProperty in SelectedPassCardTemplate.ElementImageProperties)
			{
				ResolveImageProperty(elementImageProperty);
				yield return elementImageProperty;
			}
			foreach (var elementRectangle in SelectedPassCardTemplate.ElementRectangles)
				yield return elementRectangle;
			foreach (var elementEllipse in SelectedPassCardTemplate.ElementEllipses)
				yield return elementEllipse;
			foreach (var elementTextBlock in SelectedPassCardTemplate.ElementTextBlocks)
				yield return elementTextBlock;
			foreach (var elementPolygon in SelectedPassCardTemplate.ElementPolygons)
				yield return elementPolygon;
			foreach (var elementPolyline in SelectedPassCardTemplate.ElementPolylines)
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
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.DepartmentName;
					break;
				case PassCardTextPropertyType.EndDate:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.Card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.FirstName:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.EmployeeListItem.FirstName;
					break;
				case PassCardTextPropertyType.LastName:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.EmployeeListItem.LastName;
					break;
				case PassCardTextPropertyType.Organization:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.Organization.Name;
					break;
				case PassCardTextPropertyType.Position:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.PositionName;
					break;
				case PassCardTextPropertyType.SecondName:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.EmployeeListItem.SecondName;
					break;
				case PassCardTextPropertyType.StartDate:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.Card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.Additional:
					var columnValue = AdditionalColumnHelper.GetValue(_employeesViewMode.SelectedEmployee.EmployeeListItem, elementTextProperty.AdditionalColumnUID);
					elementTextProperty.Text = columnValue == null ? string.Empty : columnValue;
					break;
				default:
					elementTextProperty.Text = string.Empty;
					break;
			}
		}
		private void ResolveImageProperty(ElementPassCardImageProperty elementImageProperty)
		{
			elementImageProperty.BackgroundColor = Colors.Transparent;
			elementImageProperty.BackgroundSourceName = null;
			elementImageProperty.BackgroundImageSource = null;
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
			var elementPassCardImageProperty = args.Element as ElementPassCardImageProperty;
			if (elementPassCardImageProperty != null)
			{
				Photo photo = null;
				switch (elementPassCardImageProperty.PropertyType)
				{
					case PassCardImagePropertyType.DepartmentLogo:
						photo = PhotoHelper.GetSingle(_employeesViewMode.SelectedCard.EmployeeViewModel.DepartmentPhotoUID);
						break;
					case PassCardImagePropertyType.OrganizationLogo:
						photo = PhotoHelper.GetSingle(_employeesViewMode.SelectedCard.Organization.PhotoUID);
						break;
					case PassCardImagePropertyType.Photo:
						//photo = PhotoHelper.GetSingle(_employeesViewMode.SelectedCard.EmployeeViewModel.Employee.PhotoUID);
						break;
					case PassCardImagePropertyType.PositionLogo:
						photo = PhotoHelper.GetSingle(_employeesViewMode.SelectedCard.EmployeeViewModel.PositionPhotoUID);
						break;
					case PassCardImagePropertyType.Additional:
						//var columnValue = AdditionalColumnHelper.GetValue(_employeesViewMode.SelectedEmployee.Employee, elementPassCardImageProperty.AdditionalColumnUID);
						//if (columnValue != null)
							//photo = columnValue.Photo;
						break;
					default:
						break;
				}
				args.Painter = new PassCardImagePropertyPainter(_passCardCanvas, elementPassCardImageProperty, photo == null ? null : photo.Data);
			}
		}
	}
}