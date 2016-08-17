using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReportSystem.Reports;
using SKDModule.PassCardDesigner.Model;
using SKDModule.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.SKD;
using System;
using System.Threading.Tasks;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class TemplateDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortPassCardTemplate>
	{
		#region Variables

		private const string TitleString = "Создание шаблона пропуска";
		private const string CaptionString = "Новый шаблон";

		private PassCardTemplateSideType _selectedTemplateSide;
		private Template _passCardTemplate;
		private PassCardTemplateReport _currentReport;
		private readonly TemplateConfigurationProvider _configurationProvider;

		private Guid OrganisationUID { get; set; }
		private bool _isNew;

		#endregion

		#region Properties

		public PassCardTemplateReport CurrentReport
		{
			get { return _currentReport; }
			set
			{
				_currentReport = value;
				OnPropertyChanged(() => CurrentReport);
			}
		}
		public PassCardTemplateSideType SelectedTemplateSide
		{
			get { return _selectedTemplateSide; }
			set
			{
				if (_selectedTemplateSide == value) return;
				_selectedTemplateSide = value;
				OnPropertyChanged(() => SelectedTemplateSide);
			}
		}

		public Template PassCardTemplate
		{
			get { return _passCardTemplate; }
			set
			{
				_passCardTemplate = value;
				OnPropertyChanged(() => PassCardTemplate);
			}
		}

		#endregion

		public TemplateDetailsViewModel()
		{
			_configurationProvider = new TemplateConfigurationProvider();
			Title = TitleString;
			EditCommand = new RelayCommand(OnEdit);
			ReloadTemplateSideCommand = new RelayCommand(OnReloadTemplateSide);
		}

		#region Commands
		public RelayCommand ReloadTemplateSideCommand { get; private set; }

		public RelayCommand EditCommand { get; private set; }

		private void OnEdit()
		{
			EditProperties();
		}

		private void OnReloadTemplateSide()
		{
			if (PassCardTemplate == null || PassCardTemplate.Front == null)
				return;

			CurrentReport = SelectedTemplateSide == PassCardTemplateSideType.Front
				? PassCardTemplate.Front.Report
				: PassCardTemplate.Back.Report;
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
			OrganisationUID = organisation.UID;

			PassCardTemplate = new Template(PassCardTemplateHelper.GetDetails(model.UID));

			CurrentReport = PassCardTemplate.Front.Report;

			UpdateTitle();
		}

		#endregion

		#region Methods

		private void EditProperties()
		{
			var dialog = new TemplatePropertiesViewModel(PassCardTemplate);
			if (DialogService.ShowModalWindow(dialog))
			{
				UpdateTitle();
			}
		}

		private void UpdateTitle()
		{
			if (PassCardTemplate == null) return;

			Title = string.Format("Шаблон пропусков: {0}", PassCardTemplate.Caption);
		}

		public bool ShowPassCardPropertiesDialog(Organisation organisation, ShortPassCardTemplate model = null)
		{
			OrganisationUID = organisation.UID;
			_isNew = model == null;

			if (_isNew)
			{
				PassCardTemplate = new Template
				{
					Caption = CaptionString,
					OrganisationUID = OrganisationUID
				};
				_configurationProvider.InitializeWithDefaults(PassCardTemplate);
			}
			else
			{
				PassCardTemplate = new Template(PassCardTemplateHelper.GetDetails(model.UID));
				UpdateTitle();
			}

			var dialogResult = DialogService.ShowModalWindow(new TemplatePropertiesViewModel(PassCardTemplate));

			return dialogResult && Save();
		}

		protected override bool Save()
		{
			var task = Task.Factory.StartNew(() => PassCardTemplateHelper.Save(PassCardTemplate.ToDTO(), _isNew));
			Task.Factory.StartNew(() => _configurationProvider.SaveDefaultPropertiesFrom(PassCardTemplate));

			if (!task.Result)
			{
				MessageBoxService.ShowError("Ошибка при добавлении шаблона пропуска");
				return false;
			}

			return true;
		}

		#endregion

		public bool Initialize(Organisation organisation, ShortPassCardTemplate model, ViewPartViewModel parentViewModel)
		{
			throw new NotImplementedException();
		}
	}
}
