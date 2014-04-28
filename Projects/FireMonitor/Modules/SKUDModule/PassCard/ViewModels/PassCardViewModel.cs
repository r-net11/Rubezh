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
	public class PassCardViewModel : SaveCancelDialogViewModel, IPlanDesignerViewModel
	{
		PassCardCanvas PassCardCanvas;
		EmployeeViewModel EmployeeViewModel;
		SKDCard Card;

		public PassCardViewModel(EmployeeViewModel employeeViewModel, SKDCard card)
		{
			Title = "Печать удостоверения " + card.PresentationName;
			EmployeeViewModel = employeeViewModel;
			Card = card;
			PrintCommand = new RelayCommand(OnPrint, CanPrint);

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
				dialog.PrintVisual(PassCardCanvas, "Пропуск " + EmployeeViewModel.ShortEmployee.LastName);
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
					elementTextProperty.Text = EmployeeViewModel.DepartmentName;
					break;
				case PassCardTextPropertyType.EndDate:
					elementTextProperty.Text = Card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.FirstName:
					elementTextProperty.Text = EmployeeViewModel.ShortEmployee.FirstName;
					break;
				case PassCardTextPropertyType.LastName:
					elementTextProperty.Text = EmployeeViewModel.ShortEmployee.LastName;
					break;
				case PassCardTextPropertyType.Organisation:
					elementTextProperty.Text = EmployeeViewModel.Organisation.Name;
					break;
				case PassCardTextPropertyType.Position:
					elementTextProperty.Text = EmployeeViewModel.PositionName;
					break;
				case PassCardTextPropertyType.SecondName:
					elementTextProperty.Text = EmployeeViewModel.ShortEmployee.SecondName;
					break;
				case PassCardTextPropertyType.StartDate:
					elementTextProperty.Text = Card.EndDate.ToShortDateString();
					break;
				case PassCardTextPropertyType.Additional:
					//var columnValue = AdditionalColumnHelper.GetValue(EmployeeViewModel.ShortEmployee, elementTextProperty.AdditionalColumnUID);
					//elementTextProperty.Text = columnValue == null ? string.Empty : columnValue;
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
						//photo = PhotoHelper.GetSingle(EmployeeViewModel.DepartmentPhotoUID);
						break;
					case PassCardImagePropertyType.OrganisationLogo:
						//photo = PhotoHelper.GetSingle(EmployeeViewModel.Organisation.PhotoUID);
						break;
					case PassCardImagePropertyType.Photo:
						//photo = PhotoHelper.GetSingle(EmployeeViewModel.ShortEmployee.PhotoUID);
						break;
					case PassCardImagePropertyType.PositionLogo:
						//photo = PhotoHelper.GetSingle(EmployeeViewModel.PositionPhotoUID);
						break;
					case PassCardImagePropertyType.Additional:
						//var columnValue = AdditionalColumnHelper.GetValue(EmployeeViewModel.ShortEmployee, elementPassCardImageProperty.AdditionalColumnUID);
						//if (columnValue != null)
						//photo = columnValue.Photo;
						break;
					default:
						break;
				}
				args.Painter = new PassCardImagePropertyPainter(PassCardCanvas, elementPassCardImageProperty, photo == null ? null : photo.Data);
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