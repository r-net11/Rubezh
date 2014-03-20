using System;
using System.Linq;
using System.Collections.ObjectModel;
using Common;
using FiresecAPI;
using FiresecAPI.SKD.PassCardLibrary;
using FiresecClient;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using SKDModule.PassCard.Designer;
using SKDModule.ViewModels;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using System.Collections.Generic;
using Infrustructure.Plans.Events;
using Infrastructure;
using System.Windows.Media;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		private PassCardCanvas _passCardCanvas;
		private bool _passCardCreated;
		private EmployeesViewModel _employeesViewMode;
		public PassCardViewModel(EmployeesViewModel employeesViewModel)
		{
			_employeesViewMode = employeesViewModel;
			_passCardCanvas = new PassCardCanvas();
			_passCardCreated = false;
			SKDManager.SKDPassCardLibraryConfiguration.Templates.Sort((item1, item2) => string.Compare(item1.Caption, item2.Caption));
			PassCardTemplates = new ObservableCollection<PassCardTemplate>(SKDManager.SKDPassCardLibraryConfiguration.Templates);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
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
					if (_passCardCreated)
						InternalCreatePassCard();
				}
			}
		}

		public void CreatePassCard()
		{
			_passCardCreated = false;
			//SelectedPassCardTemplate = PassCardTemplates.FirstOrDefault();
			InternalCreatePassCard();
			_passCardCreated = true;
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
					elementTextProperty.Text = _employeesViewMode.SelectedCard.Card.ValidTo.ToShortDateString();
					break;
				case PassCardTextPropertyType.FirstName:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.Employee.FirstName;
					break;
				case PassCardTextPropertyType.LastName:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.Employee.LastName;
					break;
				case PassCardTextPropertyType.Organization:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.Organization.Name;
					break;
				case PassCardTextPropertyType.Position:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.PositionName;
					break;
				case PassCardTextPropertyType.SecondName:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.EmployeeViewModel.Employee.SecondName;
					break;
				case PassCardTextPropertyType.StartDate:
					elementTextProperty.Text = _employeesViewMode.SelectedCard.Card.ValidTo.ToShortDateString();
					break;
				case PassCardTextPropertyType.Additional:
					var columnUID = _employeesViewMode.AdditionalColumnNames.FirstOrDefault(item => item.UID == elementTextProperty.AdditionalColumn);
					var index = _employeesViewMode.AdditionalColumnNames.IndexOf(columnUID);
					elementTextProperty.Text = index == -1 ? string.Empty : _employeesViewMode.SelectedCard.EmployeeViewModel.AdditionalColumnValues[index];
					break;
				default:
					elementTextProperty.Text = string.Empty;
					break;
			}
		}
		private void ResolveImageProperty(ElementPassCardImageProperty elementImageProperty)
		{
			elementImageProperty.BackgroundColor = Colors.Transparent;
			elementImageProperty.BackgroundPixels = null;
			elementImageProperty.BackgroundSourceName = null;
			switch (elementImageProperty.PropertyType)
			{
				case PassCardImagePropertyType.DepartmentLogo:
					elementImageProperty.BackgroundImageSource = _employeesViewMode.SelectedCard.EmployeeViewModel.DepartmentPhotoUID;
					break;
				case PassCardImagePropertyType.OrganizationLogo:
					elementImageProperty.BackgroundImageSource = _employeesViewMode.SelectedCard.Organization.PhotoUID;
					break;
				case PassCardImagePropertyType.Photo:
					elementImageProperty.BackgroundImageSource = _employeesViewMode.SelectedCard.EmployeeViewModel.Employee.PhotoUID;
					break;
				case PassCardImagePropertyType.PositionLogo:
					elementImageProperty.BackgroundImageSource = _employeesViewMode.SelectedCard.EmployeeViewModel.PositionPhotoUID;
					break;
				case PassCardImagePropertyType.Additional:
					var columnUID = _employeesViewMode.AdditionalColumnNames.FirstOrDefault(item => item.UID == elementImageProperty.AdditionalColumn);
					var index = _employeesViewMode.AdditionalColumnNames.IndexOf(columnUID);
					//elementImageProperty.BackgroundImageSource = index == -1 ? string.Empty : _employeeCardViewModel.EmployeeViewModel.AdditionalColumnValues[index];
					elementImageProperty.BackgroundImageSource = null;
					break;
				default:
					elementImageProperty.BackgroundImageSource = null;
					break;
			}
			//elementImageProperty.BackgroundPixels
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
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ChangeView); }
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
				args.Painter = new PassCardImagePropertyPainter(_passCardCanvas, elementPassCardImageProperty);
		}
	}
}
