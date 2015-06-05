using System;
using System.Collections.Generic;
using Common;
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
		private const string TitleString = "Создание шаблона пропусков";
		private const string CaptionString = "Новый шаблон";

		private Guid _organisationUID { get; set; }
		bool _isNew;

		public PassCardTemplate PassCardTemplate { get; private set; }
		public ElementsViewModel ElementsViewModel { get; private set; }
		public PassCardDesignerViewModel PassCardDesignerViewModel { get; private set; }
		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PassCardDesignerViewModel.DesignerCanvas; }
		}

		public PassCardTemplateDetailsViewModel()
		{
			EditCommand = new RelayCommand(OnEdit);

			LayerGroupService.Instance.RegisterGroup(PassCardDesignerViewModel.PassCardTextPropertiesGroup, "Текстовые свойства", 1);
			LayerGroupService.Instance.RegisterGroup(PassCardDesignerViewModel.PassCardImagePropertiesGroup, "Графические свойства", 2);
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

		public bool Initialize(Organisation organisation, ShortPassCardTemplate model)  //TODO: refactor it
		{
			_organisationUID = organisation.UID;
			DesignerCanvas.Toolbox.RegisterInstruments(GetInstruments());

			_isNew = model == null;

			if (_isNew)
			{
				Title = TitleString;
				PassCardTemplate = new PassCardTemplate()
				{
					Caption = CaptionString,
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


		public bool Initialize(Organisation organisation, ShortPassCardTemplate model, ViewPartViewModel parentViewModel) //TODO: fake method. Temprory implemented 
		{
			throw new NotImplementedException();
		}
	}
}
