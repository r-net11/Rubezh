using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReportSystem.UI.Data;
using ReportSystem.UI.Reports;
using Localization.SKD.Errors;
using Localization.SKD.ViewModels;
using SKDModule.PassCardDesigner.Model;
using SKDModule.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class TemplateDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortPassCardTemplate>
	{
		#region Variables

        private static string TitleString = CommonViewModels.CreateAccessTempl;
		private static string CaptionString = CommonViewModels.NewTemplate;

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
			IsPressEnterEnabled = false;
			OrganisationUID = organisation.UID;
			LoadDesignerContent(organisation, model);
		}

		private void LoadDesignerContent(Organisation organisation, ShortPassCardTemplate model)
		{
			var service = new PassCardTemplateReportService();

			var mainTask = new Task(() =>
			{
				var getCardTemplateTask = TaskEx.Run(() => PassCardTemplateHelper.GetDetails(model.UID));

				var setCardTemplateContinuation = getCardTemplateTask.ContinueWith(tt =>
				{
					PassCardTemplate = new Template(tt.Result);
				}, TaskContinuationOptions.OnlyOnRanToCompletion);

				var additionalColumnTask = TaskEx.Run(() => AdditionalColumnTypeHelper.GetByOrganisation(organisation.UID));

				var additionalColumnContinuation = additionalColumnTask.ContinueWith(ttt => service.GetEmptyDataSource(ttt.Result.Select(x => x.ToDataColumn())),
					TaskContinuationOptions.OnlyOnRanToCompletion);

				Task.WaitAll(setCardTemplateContinuation, additionalColumnContinuation);

				PassCardTemplate.Front.Report.DataSource = additionalColumnContinuation.Result;
				PassCardTemplate.Front.Report.DataMember = additionalColumnContinuation.Result.Tables[0].TableName;

				if (PassCardTemplate.Back != null && PassCardTemplate.Back.Report != null)
				{
					PassCardTemplate.Back.Report.DataSource = additionalColumnContinuation.Result;
					PassCardTemplate.Back.Report.DataMember = additionalColumnContinuation.Result.Tables[0].TableName;
				}

				CurrentReport = PassCardTemplate.Front.Report;
				UpdateTitle();
			});

			mainTask.Start();
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

			Title = string.Format(CommonViewModels.PasscardTemplate, PassCardTemplate.Caption);
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
				MessageBoxService.ShowError(CommonErrors.AccessTemplate_Error);
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
