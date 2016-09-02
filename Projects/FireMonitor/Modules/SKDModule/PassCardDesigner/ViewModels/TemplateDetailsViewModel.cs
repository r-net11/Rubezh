using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReportSystem.UI.Data;
using ReportSystem.UI.Reports;
using SKDModule.PassCardDesigner.Model;
using SKDModule.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using System;
using System.Diagnostics;
using System.Linq;
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
			var service = new PassCardTemplateReportService();
			var parentTask = new Task(() =>
			{
				Trace.WriteLine("Outer task executing.");
				TaskEx.Run(() =>
				{
					Trace.WriteLine("Get PassCardTemplate task executing.");
					var s = PassCardTemplateHelper.GetDetails(model.UID);
					Trace.WriteLine("Get PassCardTemplate task finish executing.");
					return s;
				}).ContinueWith(tt =>
				{
					Trace.WriteLine("Get PassCardTemplate task continuation start.");
					PassCardTemplate = new Template(tt.Result);
					Trace.WriteLine("Get PassCardTemplate task continuation finished.");
				}, TaskContinuationOptions.OnlyOnRanToCompletion);

				var additionalColumnTask = TaskEx.Run(() =>
				{
					Trace.WriteLine("Get AdditionalColumns task start.");
					var ss = AdditionalColumnTypeHelper.GetByOrganisation(organisation.UID);
					Trace.WriteLine("Get AdditionalColumns task finished.");
					return ss;
				});
				var additionalColumnContinuation = additionalColumnTask.ContinueWith(ttt =>
				{
					Trace.WriteLine("Get AdditionalColumns task continuation start.");
					var ss = service.GetEmptyDataSource(ttt.Result.Select(x => x.ToDataColumn()));
					Trace.WriteLine("Get AdditionalColumns task continuation finished.");
					return ss;
				}, TaskContinuationOptions.OnlyOnRanToCompletion);

				Trace.WriteLine("Parent task execution");
				PassCardTemplate.Front.Report.DataSource = additionalColumnContinuation.Result;
				PassCardTemplate.Front.Report.DataMember = additionalColumnContinuation.Result.Tables[0].TableName;
				if (PassCardTemplate.Back != null && PassCardTemplate.Back.Report != null)
				{
					PassCardTemplate.Back.Report.DataSource = additionalColumnContinuation.Result;
					PassCardTemplate.Back.Report.DataMember = additionalColumnContinuation.Result.Tables[0].TableName;
				}
				Trace.WriteLine("Parent task execution continue");
				CurrentReport = PassCardTemplate.Front.Report;
				UpdateTitle();
				Trace.WriteLine("Parent task execution finished");
			});
			parentTask.Start();
			//var service = new PassCardTemplateReportService();
			//var task1 = Task.Factory.StartNew(() => PassCardTemplateHelper.GetDetails(model.UID))
			//	.ContinueWith(t =>
			//	{
			//		PassCardTemplate = new Template(t.Result);
			//	}, TaskContinuationOptions.OnlyOnRanToCompletion);

			//var getDataSourceTask = Task.Factory.StartNew(() => AdditionalColumnTypeHelper.GetByOrganisation(organisation.UID));
			//getDataSourceTask.ContinueWith(t => service.GetEmptyDataSource(t.Result.Select(x => x.ToDataColumn()))
			//	, TaskContinuationOptions.OnlyOnRanToCompletion);

			//Task.Factory.ContinueWhenAll(new[] { task1, getDataSourceTask }, tasks =>
			//{
			//	PassCardTemplate.Front.Report.DataSource = dataSource;
			//	PassCardTemplate.Front.Report.DataMember = dataSource.Tables[0].TableName;
			//	if (PassCardTemplate.Back != null && PassCardTemplate.Back.Report != null)
			//	{
			//		PassCardTemplate.Back.Report.DataSource = getDataSourceTask.Result;
			//		PassCardTemplate.Back.Report.DataMember = getDataSourceTask.Result.Tables[0].TableName;
			//	}
			//	CurrentReport = PassCardTemplate.Front.Report;
			//	UpdateTitle();
			//});
		}

		public void In(Organisation organisation, ShortPassCardTemplate model)
		{
			var service = new PassCardTemplateReportService();
			var parentTask = new Task(() =>
			{
				Trace.WriteLine("Outer task executing.");
				TaskEx.Run(() =>
				{
					Trace.WriteLine("Get PassCardTemplate task executing.");
					var s = PassCardTemplateHelper.GetDetails(model.UID);
					Trace.WriteLine("Get PassCardTemplate task finish executing.");
					return s;
				}).ContinueWith(tt =>
				{
					Trace.WriteLine("Get PassCardTemplate task continuation start.");
					PassCardTemplate = new Template(tt.Result);
					Trace.WriteLine("Get PassCardTemplate task continuation finished.");
				}, TaskContinuationOptions.OnlyOnRanToCompletion);

				var additionalColumnTask = TaskEx.Run(() =>
				{
					Trace.WriteLine("Get AdditionalColumns task start.");
					var ss = AdditionalColumnTypeHelper.GetByOrganisation(organisation.UID);
					Trace.WriteLine("Get AdditionalColumns task finished.");
					return ss;
				});
				var additionalColumnContinuation = additionalColumnTask.ContinueWith(ttt =>
				{
					Trace.WriteLine("Get AdditionalColumns task continuation start.");
					var ss = service.GetEmptyDataSource(ttt.Result.Select(x => x.ToDataColumn()));
					Trace.WriteLine("Get AdditionalColumns task continuation finished.");
					return ss;
				}, TaskContinuationOptions.OnlyOnRanToCompletion);

				Trace.WriteLine("Parent task execution");
				PassCardTemplate.Front.Report.DataSource = additionalColumnContinuation.Result;
				PassCardTemplate.Front.Report.DataMember = additionalColumnContinuation.Result.Tables[0].TableName;
				if (PassCardTemplate.Back != null && PassCardTemplate.Back.Report != null)
				{
					PassCardTemplate.Back.Report.DataSource = additionalColumnContinuation.Result;
					PassCardTemplate.Back.Report.DataMember = additionalColumnContinuation.Result.Tables[0].TableName;
				}
				Trace.WriteLine("Parent task execution continue");
				CurrentReport = PassCardTemplate.Front.Report;
				UpdateTitle();
				Trace.WriteLine("Parent task execution finished");
			});
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
