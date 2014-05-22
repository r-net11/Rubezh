using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Common;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.PassCard.Designer;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardViewModel : SaveCancelDialogViewModel, IPlanDesignerViewModel
	{
		PassCardCanvas PassCardCanvas;
		Employee Employee;
		Department Department;
		Position Position;
		OrganisationDetails Organisation;
		SKDCard Card;

		public PassCardViewModel(Guid employeeUID, SKDCard card)
		{
			Title = "Печать удостоверения " + card.PresentationName;
			Card = card;
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			Employee = EmployeeHelper.GetDetails(employeeUID);
			Department = DepartmentHelper.GetDetails(Employee.Department.UID);
			Organisation = OrganisationHelper.GetDetails(Employee.OrganisationUID);
			Position = PositionHelper.GetDetails(Employee.Position.UID);
			PassCardCanvas = new PassCardCanvas();
			SKDManager.SKDPassCardLibraryConfiguration.Templates.Sort((item1, item2) => string.Compare(item1.Caption, item2.Caption));
			PassCardTemplates = new ObservableCollection<PassCardTemplate>(SKDManager.SKDPassCardLibraryConfiguration.Templates);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);

			var uid = Card.CardTemplateUID;
			SelectedPassCardTemplate = uid.HasValue ? PassCardTemplates.FirstOrDefault(item => item.UID == uid.Value) : null;
		}

		public ObservableCollection<PassCardTemplate> PassCardTemplates { get; private set; }

		PassCardTemplate _selectedPassCardTemplate;
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
				}
			}
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			var dialog = new PrintDialog();
			if (dialog.ShowDialog() == true)
			{
				var rect = LayoutInformation.GetLayoutSlot(PassCardCanvas);
				var capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);
				var origin = new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight);
				PassCardCanvas.Arrange(new Rect(origin, PassCardCanvas.DesiredSize));
				dialog.PrintVisual(PassCardCanvas, "Пропуск " + Employee.LastName);
				PassCardCanvas.Arrange(rect);
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
					using (new TimeCounter("\t\tPassCardCanvas.Initialize: {0}"))
						PassCardCanvas.Initialize(SelectedPassCardTemplate);
					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
						foreach (var elementBase in EnumerateElements())
							PassCardCanvas.CreatePresenterItem(elementBase);
					using (new TimeCounter("\t\tPassCardViewModel.OnUpdated: {0}"))
						Update();
					PassCardCanvas.LoadingFinished();
					PassCardCanvas.Refresh();
				}
		}
		void Update()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
		IEnumerable<ElementBase> EnumerateElements()
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

		void ResolveTextProperty(ElementPassCardTextProperty elementTextProperty)
		{
			switch (elementTextProperty.PropertyType)
			{
				case PassCardTextPropertyType.Birthday:
					elementTextProperty.Text = "[Пока нет в БД]";
					break;
				case PassCardTextPropertyType.Department:
					elementTextProperty.Text = Department.Name;
					break;
				case PassCardTextPropertyType.EndDate:
					elementTextProperty.Text = Card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.FirstName:
					elementTextProperty.Text = Employee.FirstName;
					break;
				case PassCardTextPropertyType.LastName:
					elementTextProperty.Text = Employee.LastName;
					break;
				case PassCardTextPropertyType.Organisation:
					elementTextProperty.Text = Organisation.Name;
					break;
				case PassCardTextPropertyType.Position:
					elementTextProperty.Text = Position.Name;
					break;
				case PassCardTextPropertyType.SecondName:
					elementTextProperty.Text = Employee.SecondName;
					break;
				case PassCardTextPropertyType.StartDate:
					elementTextProperty.Text = Card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.Additional:
					var columnValue = Employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnType.UID == elementTextProperty.AdditionalColumnUID);
					elementTextProperty.Text = columnValue == null || columnValue.TextData == null ? string.Empty : columnValue.TextData;
					break;
				default:
					elementTextProperty.Text = string.Empty;
					break;
			}
		}
		void ResolveImageProperty(ElementPassCardImageProperty elementImageProperty)
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
			get { return PassCardCanvas; }
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

		void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementPassCardImageProperty = args.Element as ElementPassCardImageProperty;
			if (elementPassCardImageProperty != null)
			{
				Photo photo = null;
				switch (elementPassCardImageProperty.PropertyType)
				{
					case PassCardImagePropertyType.DepartmentLogo:
						photo = Department.Photo;	
						break;
					case PassCardImagePropertyType.OrganisationLogo:
						photo = Organisation.Photo;
						break;
					case PassCardImagePropertyType.Photo:
						photo = Employee.Photo;
						break;
					case PassCardImagePropertyType.PositionLogo:
						photo = Position.Photo;	
						break;
					case PassCardImagePropertyType.Additional:
						var columnValue = Employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnType.UID == elementPassCardImageProperty.AdditionalColumnUID);
						if (columnValue != null)
							photo = columnValue.Photo;
						break;
					default:
						break;
				}
				args.Painter = new PassCardImagePropertyPainter(PassCardCanvas, elementPassCardImageProperty, 
					photo == null || photo.Data == null ? null : photo.Data);
			}
		}

		protected override bool Save()
		{
			var cardTemplateUID = SelectedPassCardTemplate == null ? null : (Guid?)SelectedPassCardTemplate.UID;
			Card.CardTemplateUID = cardTemplateUID;
			return CardHelper.SaveTemplate(Card);
		}
	}
}