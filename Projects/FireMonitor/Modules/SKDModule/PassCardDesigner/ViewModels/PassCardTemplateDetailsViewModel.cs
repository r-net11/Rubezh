using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Controls.Menu.ViewModels;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Services;
using SKDModule.PassCardDesigner.InstrumentAdorners;
using SKDModule.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplateDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortPassCardTemplate>
	{
		private Guid _organisationUID { get; set; }
		bool _isNew;

		public PassCardTemplate PassCardTemplate { get; private set; }
		public ElementsViewModel ElementsViewModel { get; private set; }
		public PassCardDesignerViewModel PassCardDesignerViewModel { get; private set; }
		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PassCardDesignerViewModel.DesignerCanvas; }
		}
		public MenuViewModel Menu { get; private set; }

		public PassCardTemplateDetailsViewModel()
		{
			LayerGroupService.Instance.RegisterGroup(PassCardDesignerViewModel.PassCardTextPropertiesGroup, "Текстовые свойства", 1);
			LayerGroupService.Instance.RegisterGroup(PassCardDesignerViewModel.PassCardImagePropertiesGroup, "Графические свойства", 2);
			PassCardDesignerViewModel = new PassCardDesignerViewModel();
			OnPropertyChanged(() => PassCardDesignerViewModel);
			PassCardDesignerViewModel.DesignerCanvas.ZoomChanged();
			EditCommand = new RelayCommand(OnEdit);
			ElementsViewModel = new ElementsViewModel(PassCardDesignerViewModel.DesignerCanvas);

			Menu = new MenuViewModel()
			{
				Items = new ObservableCollection<BaseViewModel>()
				{
					new MenuButtonViewModel(EditCommand, "Edit" , "Редактировать"),
					//new MenuSeparatorViewModel(),
					new MenuButtonViewModel(PassCardDesignerViewModel.CopyCommand, "Copy" , "Копировать"),
					new MenuButtonViewModel(PassCardDesignerViewModel.CutCommand, "Cut" , "Вырезать"),
					new MenuButtonViewModel(PassCardDesignerViewModel.PasteCommand, "Paste" , "Вставить"),
					new MenuButtonViewModel(PassCardDesignerViewModel.UndoCommand, "Undo" , "Отменить"),
					new MenuButtonViewModel(PassCardDesignerViewModel.RedoCommand, "Redo" , "Применить"),
					//new MenuSeparatorViewModel(),
					new MenuButtonViewModel(PassCardDesignerViewModel.MoveToFrontCommand, "MoveForward" , "Вверх"),
					new MenuButtonViewModel(PassCardDesignerViewModel.SendToBackCommand, "MoveBackward" , "Вниз"),
					new MenuButtonViewModel(PassCardDesignerViewModel.MoveForwardCommand, "MoveFront" , "Выше"),
					new MenuButtonViewModel(PassCardDesignerViewModel.MoveBackwardCommand, "MoveBack" , "Ниже"),
				}
			};
			Menu.Items.Add(new MenuButtonViewModel(PassCardDesignerViewModel.AlignHorizontalLeftCommand, "shapes-align-hori-left", "Выровнять по левому краю"));
			Menu.Items.Add(new MenuButtonViewModel(PassCardDesignerViewModel.AlignHorizontalCenterCommand, "shapes-align-hori-center", "Выровнять по вертикали"));
			Menu.Items.Add(new MenuButtonViewModel(PassCardDesignerViewModel.AlignHorizontalRightCommand, "shapes-align-hori-right", "Выровнять по правому краю"));
			Menu.Items.Add(new MenuButtonViewModel(PassCardDesignerViewModel.AlignVerticalTopCommand, "shapes-align-verti-top", "Выровнять по верхнему краю"));
			Menu.Items.Add(new MenuButtonViewModel(PassCardDesignerViewModel.AlignVerticalCenterCommand, "shapes-align-verti-middle", "Выровнять по горизонтали"));
			Menu.Items.Add(new MenuButtonViewModel(PassCardDesignerViewModel.AlignVerticalBottomCommand, "shapes-align-verti-bottom", "Выровнять по нижнему краю"));
		}

		#region IDetailsViewModel<ShortPassCardTemplate> Members

		public ShortPassCardTemplate Model
		{
			get
			{
				return new ShortPassCardTemplate
				{
					UID = PassCardTemplate.UID,
					Name = PassCardTemplate.Caption,
					Description = PassCardTemplate.Description,
					OrganisationUID = PassCardTemplate.OrganisationUID
				};
			}
		}

		public bool Initialize(Organisation organisation, ShortPassCardTemplate model, ViewPartViewModel parentViewModel)
		{
			_organisationUID = organisation.UID;
			DesignerCanvas.Toolbox.RegisterInstruments(GetInstruments());
			bool _isNew = model == null;
			if (_isNew)
			{
				Title = "Создание шаблона пропусков";
				PassCardTemplate = new PassCardTemplate()
				{
					Caption = "Новый шаблон",
					OrganisationUID = _organisationUID
				};
				LoadDefaultProperties();
				if (!EditProperties())
					return false;
			}
			else
			{
				PassCardTemplate = PassCardTemplateHelper.GetDetails(model.UID);
				Update();
			}
			LoadPassCardDesigner();
			return true;
		}

		#endregion

		private IEnumerable<IInstrument> GetInstruments()
		{
			yield return new InstrumentViewModel()
			{
				ImageSource = "Text",
				ToolTip = "Текстовое свойство",
				Adorner = new PassCardTextPropertyAdorner(DesignerCanvas, _organisationUID),
				Index = 300,
				Autostart = true
			};
			yield return new InstrumentViewModel()
			{
				ImageSource = "Photo",
				ToolTip = "Графическое свойство",
				Adorner = new PassCardImagePropertyAdorner(DesignerCanvas, _organisationUID),
				Index = 301,
				Autostart = true
			};
		}
		private void LoadPassCardDesigner()
		{
			using (new TimeCounter("PassCardTemplateDetailsViewModel.SelectedPlan: {0}", true, true))
			{
				OnPropertyChanged(() => PassCardTemplate);
				DesignerCanvas.Toolbox.IsEnabled = PassCardTemplate != null;
				PassCardDesignerViewModel.Initialize(PassCardTemplate);
				using (new TimeCounter("\tPassCardsDesignerViewModel.UpdateElements: {0}"))
					ElementsViewModel.Update();
				DesignerCanvas.Toolbox.SetDefault();
				DesignerCanvas.DeselectAll();
				DesignerCanvas.Toolbox.IsDialog = true;
				DesignerCanvas.Toolbox.AcceptKeyboard = true;
			}
		}
		private void LoadDefaultProperties()
		{
			var width = RegistrySettingsHelper.GetDouble("Administrator.PassCardTemplate.DefaultWidth");
			var height = RegistrySettingsHelper.GetDouble("Administrator.PassCardTemplate.DefaultHeight");
			var color = RegistrySettingsHelper.GetColor("Administrator.PassCardTemplate.DefaultColor");
			var border = RegistrySettingsHelper.GetDouble("Administrator.PassCardTemplate.DefaultBorder");
			var borderColor = RegistrySettingsHelper.GetColor("Administrator.PassCardTemplate.DefaultBorderColor");
			if (width != 0)
				PassCardTemplate.Width = width;
			if (height != 0)
				PassCardTemplate.Height = height;
			PassCardTemplate.BackgroundColor = color;
			PassCardTemplate.BorderColor = borderColor;
			PassCardTemplate.BorderThickness = border;
		}
		private void SaveDefaultProperties()
		{
			RegistrySettingsHelper.SetDouble("Administrator.PassCardTemplate.DefaultWidth", PassCardTemplate.Width);
			RegistrySettingsHelper.SetDouble("Administrator.PassCardTemplate.DefaultHeight", PassCardTemplate.Height);
			RegistrySettingsHelper.SetColor("Administrator.PassCardTemplate.DefaultColor", PassCardTemplate.BackgroundColor);
			RegistrySettingsHelper.SetDouble("Administrator.PassCardTemplate.DefaultBorder", PassCardTemplate.BorderThickness);
			RegistrySettingsHelper.SetColor("Administrator.PassCardTemplate.DefaultBorderColor", PassCardTemplate.BorderColor);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			EditProperties();
		}
		private bool EditProperties()
		{
			var dialog = new PassCardTemplatePropertiesViewModel(PassCardTemplate);
			if (DialogService.ShowModalWindow(dialog))
			{
				Update();
				DesignerCanvas.Update();
				PassCardDesignerViewModel.Update();
				DesignerCanvas.Refresh();
				DesignerCanvas.DesignerChanged();
				return true;
			}
			return false;
		}
		private void Update()
		{
			Title = string.Format("Шаблон пропусков: {0}", PassCardTemplate.Caption);
		}

		protected override bool Save()
		{
			SaveDefaultProperties();
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			return PassCardTemplateHelper.Save(PassCardTemplate, _isNew);
		}
		public override void OnClosed()
		{
			DesignerCanvas.Toolbox.AcceptKeyboard = false;
			base.OnClosed();
		}
	}
}
