using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
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
		#region Variables

		private const string TitleString = "Создание шаблона пропусков";
		private const string CaptionString = "Новый шаблон";
		private const string TextPropertiesString = "Текстовые свойства";
		private const string GraphicPropertiesString = "Графические свойства";

		private Guid _organisationUID { get; set; }
		private bool _isNew;

		#endregion

		#region Properties

		public PassCardTemplate PassCardTemplate { get; private set; }

		public ElementsViewModel ElementsViewModel { get; private set; }

		public PassCardDesignerViewModel PassCardDesignerViewModel { get; private set; }

		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PassCardDesignerViewModel.DesignerCanvas; }
		}

		#endregion


		public PassCardTemplateDetailsViewModel()
		{
			EditCommand = new RelayCommand(OnEdit);

			LayerGroupService.Instance.RegisterGroup(PassCardDesignerViewModel.PassCardTextPropertiesGroup, TextPropertiesString, 1);
			LayerGroupService.Instance.RegisterGroup(PassCardDesignerViewModel.PassCardImagePropertiesGroup, GraphicPropertiesString, 2);
			PassCardDesignerViewModel = new PassCardDesignerViewModel();
			OnPropertyChanged(() => PassCardDesignerViewModel);
			PassCardDesignerViewModel.DesignerCanvas.ZoomChanged();
			ElementsViewModel = new ElementsViewModel(DesignerCanvas);
		}

		#region Commands

		public RelayCommand EditCommand { get; private set; }

		private void OnEdit()
		{
			EditProperties();
		}

		#endregion

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

		public void InitializeDesigner(Organisation organisation, ShortPassCardTemplate model)
		{
			_organisationUID = organisation.UID;
			DesignerCanvas.Toolbox.RegisterInstruments(GetInstruments());

			PassCardTemplate = PassCardTemplateHelper.GetDetails(model.UID);

			UpdateTitle();

			LoadPassCardDesigner();
		}

		#endregion

		#region Methods

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
			OnPropertyChanged(() => PassCardTemplate);
			DesignerCanvas.Toolbox.IsEnabled = PassCardTemplate != null;
			PassCardDesignerViewModel.Initialize(PassCardTemplate);
			ElementsViewModel.Update();
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

		private void EditProperties()
		{
			var dialog = new PassCardTemplatePropertiesViewModel(PassCardTemplate);
			if (DialogService.ShowModalWindow(dialog))
			{
				UpdateTitle();
				DesignerCanvas.Update();
				PassCardDesignerViewModel.Update();
				DesignerCanvas.Refresh();
				DesignerCanvas.DesignerChanged();
			}
		}

		private void UpdateTitle()
		{
			if (PassCardTemplate == null) return;

			Title = string.Format("Шаблон пропусков: {0}", PassCardTemplate.Caption);
		}

		public override void OnClosed()
		{
			DesignerCanvas.Toolbox.AcceptKeyboard = false;
			base.OnClosed();
		}

		public bool ShowPassCardPropertiesDialog(Organisation organisation, ShortPassCardTemplate model = null)
		{
			_organisationUID = organisation.UID;
			_isNew = model == null;

			PassCardTemplate = GetPassCardTemplate(_isNew, model);
			InitializePassCardTemplate(_isNew);

			var dialogResult = DialogService.ShowModalWindow(new PassCardTemplatePropertiesViewModel(PassCardTemplate));

			if (!dialogResult) return false;

			return Save();
		}

		protected override bool Save()
		{
			SaveDefaultProperties();
			return PassCardTemplateHelper.Save(PassCardTemplate, _isNew);
		}

		private void InitializePassCardTemplate(bool isNew)
		{
			if (isNew)
				LoadDefaultProperties();
			else
				UpdateTitle();
		}

		private PassCardTemplate GetPassCardTemplate(bool isNew, ShortPassCardTemplate model)
		{
			return isNew
				? new PassCardTemplate
				{
					Caption = CaptionString,
					OrganisationUID = _organisationUID
				}
				: PassCardTemplateHelper.GetDetails(model.UID);
		}

		#endregion

		public bool Initialize(Organisation organisation, ShortPassCardTemplate model, ViewPartViewModel parentViewModel)
		{
			throw new NotImplementedException();
		}
	}
}
