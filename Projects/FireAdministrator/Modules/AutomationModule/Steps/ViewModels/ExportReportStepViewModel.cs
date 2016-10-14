using FiresecClient;
using Infrastructure.Common;
using Localization.Automation.ViewModels;
using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.Enums;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace AutomationModule.ViewModels
{
	public class ExportReportStepViewModel : BaseStepViewModel
	{
		private bool _isUseArchiveSection;
		private bool _isUseExpirationDateSection;
		private bool _isUsePeriodOfExecuteSection;
		private bool _isDatePeriodEnabled;
		private bool _isUseDateTimeNow;

		#region Properties
		public List<ReportType> ReportTypes { get; set; }
		public bool IsUseDateTimeNowSection
		{
			get { return ExportReportArguments.IsUseDateTimeNow; }
			set
			{
				ExportReportArguments.IsUseDateTimeNow = value;
				OnPropertyChanged(() => IsUseDateTimeNowSection);
			}
		}

		public bool IsUseDateInFileName
		{
			get { return ExportReportArguments.IsUseDateInFileName; }
			set
			{
				ExportReportArguments.IsUseDateInFileName = value;
				OnPropertyChanged(() => IsUseDateInFileName);
			}
		}

		public bool IsUseArchiveSection
		{
			get { return _isUseArchiveSection; }
			set
			{
				if (_isUseArchiveSection == value) return;
				_isUseArchiveSection = value;
				OnPropertyChanged(() => IsUseArchiveSection);
			}
		}

		public bool UseExpirationDateSection
		{
			get { return _isUseExpirationDateSection; }
			set
			{
				if (_isUseExpirationDateSection == value) return;
				_isUseExpirationDateSection = value;
				OnPropertyChanged(() => UseExpirationDateSection);
			}
		}

		public bool IsUsePeriodOfExecuteSection
		{
			get { return _isUsePeriodOfExecuteSection; }
			set
			{
				if (_isUsePeriodOfExecuteSection == value) return;
				_isUsePeriodOfExecuteSection = value;
				OnPropertyChanged(() => IsUsePeriodOfExecuteSection);
			}
		}

		public bool IsDatePeriodEnabled
		{
			get { return _isDatePeriodEnabled; }
			set
			{
				if (_isDatePeriodEnabled == value) return;
				_isDatePeriodEnabled = value;
				OnPropertyChanged(() => IsDatePeriodEnabled);
			}
		}

		public SKDReportFilter SelectedFilter
		{
			get { return ExportReportArguments.ReportFilter; }
			set
			{
				ExportReportArguments.ReportFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		public EndDateType SelectedEndDateType
		{
			get { return ExportReportArguments.ReportEndDateType; }
			set
			{
				ExportReportArguments.ReportEndDateType = value;
				OnPropertyChanged(() => SelectedEndDateType);
			}
		}

		public ReportType SelectedReportType
		{
			get { return ExportReportArguments.ReportType; }
			set
			{
				ExportReportArguments.ReportType = value;
				OnPropertyChanged(() => SelectedReportType);
			}
		}

		public ICollectionView AvailableFiltersCollection { get; private set; }
		private List<SKDReportFilter> CachedFilters { get; set; }
		private ExportReportArguments ExportReportArguments { get; set; }
		public ArgumentViewModel FilePath { get; private set; }
		public ArgumentViewModel StartDate { get; private set; }
		public ArgumentViewModel EndDate { get; private set; }

		public bool IsFilterNameInHeader
		{
			get { return ExportReportArguments.IsFilterNameInHeader; }
			set
			{
				ExportReportArguments.IsFilterNameInHeader = value;
				OnPropertyChanged(() => IsFilterNameInHeader);
			}
		}

		public bool IsUseExpiredDate
		{
			get { return ExportReportArguments.IsUseExpirationDate; }
			set
			{
				ExportReportArguments.IsUseExpirationDate = value;
				OnPropertyChanged(() => IsUseExpiredDate);
			}
		}

		public bool IsShowArchive
		{
			get { return ExportReportArguments.IsUseArchive; }
			set
			{
				if (ExportReportArguments.IsUseArchive == value) return;
				ExportReportArguments.IsUseArchive = value;
				OnPropertyChanged(() => IsShowArchive);
			}
		}

		public bool IsUseDateTimeNow
		{
			get { return _isUseDateTimeNow; }
			set
			{
				_isUseDateTimeNow = value;
				OnPropertyChanged(() => IsUseDateTimeNow);
			}
		}

		public ReportFormatEnum SelectedReportFormat
		{
			get { return ExportReportArguments.ReportFormat; }
			set
			{
				ExportReportArguments.ReportFormat = value;
				OnPropertyChanged(() => SelectedReportFormat);
			}
		}

		public ReportPeriodType SelectedReportPeriod
		{
			get { return ExportReportArguments.ReportPeriodType; }
			set
			{
				ExportReportArguments.ReportPeriodType = value;
				OnPropertyChanged(() => SelectedReportPeriod);
			}
		}
		#endregion

		public ExportReportStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ReportChangedCommand = new RelayCommand(OnReportChanged);
			PeriodChangedCommand = new RelayCommand(OnPeriodChanged);
			IsUseExpiredDateChangedCommand = new RelayCommand(OnIsUseExpiredDateChanged);

			ExportReportArguments = stepViewModel.Step.ExportReportArguments;
			FilePath = new ArgumentViewModel(ExportReportArguments.FilePath, stepViewModel.Update, UpdateContent);
			StartDate = new ArgumentViewModel(ExportReportArguments.StartDate, stepViewModel.Update, UpdateContent);
			EndDate = new ArgumentViewModel(ExportReportArguments.EndDate, stepViewModel.Update, UpdateContent);
			ReportTypes = ((ReportType[])Enum.GetValues(typeof(ReportType))).OrderBy(x => x.ToDescription()).ToList();
			CachedFilters = FiresecManager.FiresecService.GetAllFilters().Result;
			AvailableFiltersCollection = CollectionViewSource.GetDefaultView(CachedFilters);
			AvailableFiltersCollection.Filter = ReportFilter;
			OnReportChanged();
		}

		private bool ReportFilter(object obj)
		{
			var filter = obj as SKDReportFilter;
			return filter != null && filter.ReportType == SelectedReportType;
		}

		public void OnReportChanged()
		{
			AvailableFiltersCollection.Refresh();
			SelectedFilter = (SKDReportFilter)AvailableFiltersCollection.CurrentItem;
			SelectUISection(SelectedReportType);
		}

		public void OnIsUseExpiredDateChanged()
		{
			if (!IsUseExpiredDate)
			{
				IsDatePeriodEnabled = false;
			}
			else if ((IsUsePeriodOfExecuteSection && IsUseExpiredDate && SelectedReportPeriod == ReportPeriodType.Arbitrary)
					 || (UseExpirationDateSection && IsUseExpiredDate && SelectedEndDateType == EndDateType.Arbitrary))
			{
				IsDatePeriodEnabled = true;
			}
		}

		public void OnPeriodChanged()
		{
			IsDatePeriodEnabled = (IsUsePeriodOfExecuteSection && SelectedReportPeriod == ReportPeriodType.Arbitrary)
								|| (UseExpirationDateSection && SelectedEndDateType == EndDateType.Arbitrary);
		}

		private void SelectUISection(ReportType reportType)
		{
			switch (reportType)
			{
				case ReportType.CardsReport:
					ShowUseExpirationDateUISection();
					break;
				case ReportType.DepartmentsReport:
				case ReportType.PositionsReport:
				case ReportType.EmployeeReport:
					ShowIsUseArchiveUISection();
					break;
				case ReportType.EmployeeZonesReport:
					ShowIsUseDateTimeNowUISection();
					break;
				case ReportType.EventsReport:
				case ReportType.EmployeeRootReport:
				case ReportType.DisciplineReport:
				case ReportType.DocumentsReport:
				case ReportType.WorkingTimeReport:
					ShowIsUsePeriodOfExecuteUISection();
					OnPeriodChanged();
					break;
				default:
					HideAllCustomUISections();
					break;
			}
		}

		private void ShowUseExpirationDateUISection()
		{
			UseExpirationDateSection = true;
			IsUseExpiredDate = false;
			IsUseArchiveSection = false;
			IsUseDateTimeNowSection = false;
			IsUsePeriodOfExecuteSection = false;
			IsDatePeriodEnabled = false;
		}

		private void ShowIsUseArchiveUISection()
		{
			IsUseArchiveSection = true;
			UseExpirationDateSection = false;
			IsUseDateTimeNowSection = false;
			IsUsePeriodOfExecuteSection = false;
		}

		private void ShowIsUseDateTimeNowUISection()
		{
			IsUseDateTimeNowSection = true;
			IsUseDateTimeNow = true;
			UseExpirationDateSection = false;
			IsUseArchiveSection = false;
			IsUsePeriodOfExecuteSection = false;
		}

		private void ShowIsUsePeriodOfExecuteUISection()
		{
			IsUsePeriodOfExecuteSection = true;
			IsUseDateTimeNowSection = false;
			UseExpirationDateSection = false;
			IsUseArchiveSection = false;
			IsDatePeriodEnabled = false;
		}

		private void HideAllCustomUISections()
		{
			UseExpirationDateSection = false;
			IsUseArchiveSection = false;
			IsUseDateTimeNowSection = false;
			IsUsePeriodOfExecuteSection = false;
		}

		public override void UpdateContent()
		{
			FilePath.Update(Procedure, ExplicitType.String);
			StartDate.Update(Procedure, ExplicitType.DateTime);
			EndDate.Update(Procedure, ExplicitType.DateTime);
		}

		public override string Description
		{
			get { return string.Format(StepCommonViewModel.ExportReport, SelectedReportType.ToDescription(), SelectedFilter); }
		}

		public RelayCommand ReportChangedCommand { get; private set; }
		public RelayCommand PeriodChangedCommand { get; private set; }
		public RelayCommand IsUseExpiredDateChangedCommand { get; private set; }
	}
}