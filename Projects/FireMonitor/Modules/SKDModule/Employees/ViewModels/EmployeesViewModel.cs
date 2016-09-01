using System.Collections;
using System.Data;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using ReportSystem;
using ReportSystem.DataSets;
using SKDModule.Employees.ViewModels.DialogWindows;
using SKDModule.Events;
using SKDModule.Reports;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : OrganisationBaseViewModel<ShortEmployee, EmployeeFilter, EmployeeViewModel, EmployeeDetailsViewModel>
	{
		public List<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }

		public EmployeesViewModel()
		{
			ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
			ServiceFactoryBase.Events.GetEvent<EditAdditionalColumnEvent>().Unsubscribe(OnUpdateIsInGrid);
			ServiceFactoryBase.Events.GetEvent<EditAdditionalColumnEvent>().Subscribe(OnUpdateIsInGrid);
			PrintTemplatesCommand = new RelayCommand(OnPrintTemplates);
		}

		public override void Initialize(EmployeeFilter filter)
		{
			base.Initialize(filter);
			PersonType = filter.PersonType;
			InitializeAdditionalColumns();
			ServiceFactoryBase.Events.GetEvent<ChangeEmployeeGuestEvent>().Publish(null);
		}

		protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			base.OnOrganisationUsersChanged(newOrganisation);
			InitializeAdditionalColumns();
		}

		public void OnPrintTemplates()
		{
			var vm = new PrintingTemplatesDialogViewModel(SelectedItem.OrganisationUID);
			if (DialogService.ShowModalWindow(vm))
			{
				var reports = FiresecManager.FiresecService.GetCardTemplateReportsForPrint(Filter, vm.Settings.SelectedTemplate.Item1);

				var resultXtraReports = new List<XtraReport>();
				foreach (var xReport in reports.Result)
				{
					var report = xReport.Report.ToXtraReport(xReport.BackgroundImage);
					var ds = new PassCardTemplateDataSource();
					foreach (var column in xReport.Data.AdditionalColumns)
					{
						var r = new DataColumn(column.Name, typeof (string));
						ds.Employee.Columns.Add(column.GraphicValue != null
							? new DataColumn(column.Name, typeof (byte[]))
							: new DataColumn(column.Name, typeof (string)));
					}
					var row = ds.Employee.NewEmployeeRow();
					row.UID = Guid.NewGuid();
					row.FirstName = xReport.Data.FirstName;
					row.MiddleName = xReport.Data.MiddleName;
					row.LastName = xReport.Data.LastName;
					row.Image = xReport.BackgroundImage;
					foreach (var column in xReport.Data.AdditionalColumns)
					{
						if (column.GraphicValue != null)
							row[column.Name] = column.GraphicValue;
						else
							row[column.Name] = column.TextValue;
					}
					ds.Employee.AddEmployeeRow(row);
					report.DataSource = ds;
					report.DataMember = ds.Tables[0].TableName;

					resultXtraReports.Add(report);
				}
				//Print
				var mergedReport = new MergedReport(resultXtraReports.ToArray(), vm.Settings.SelectedPaperKindSetting);
				mergedReport.ShowPreviewDialog();

			}
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var columnTypes = AdditionalColumnTypeHelper.GetByCurrentUser();
			if (columnTypes == null)
				return;
			AdditionalColumnTypes = columnTypes.Where(x => x.DataType == AdditionalColumnDataType.Text && x.IsInGrid).ToList();
			foreach (var columnType in AdditionalColumnTypes)
			{
				AdditionalColumnNames.Add(columnType.Name);
			}
			ServiceFactoryBase.Events.GetEvent<UpdateAdditionalColumns>().Publish(null);
		}

		protected override void Remove()
		{
			if (!SelectedItem.Cards.Any() || MessageBoxService.ShowQuestion("Привязанные к сотруднику пропуска будут деактивированы. Продожить?"))
			{
				var cardUIDs = SelectedItem.Cards.Select(x => x.UID);
				base.Remove();
				foreach (var uid in cardUIDs)
				{
					ServiceFactoryBase.Events.GetEvent<BlockCardEvent>().Publish(uid);
				}

				SelectedItem.EmployeeCardsViewModel.Cards.Clear();
			}
		}

		protected override void OnRemoveOrganisation(Guid organisationUID)
		{
			var cards = CardHelper.GetOrganisationCards(organisationUID);
			if (cards != null)
			{
				foreach (var uid in cards.Select(x => x.UID))
				{
					ServiceFactoryBase.Events.GetEvent<BlockCardEvent>().Publish(uid);
				}
			}
			base.OnRemoveOrganisation(organisationUID);
			SelectedItem = Organisations.FirstOrDefault();
		}

		protected override IEnumerable<ShortEmployee> GetModels(EmployeeFilter filter)
		{
			return EmployeeHelper.Get(filter);
		}

		protected override IEnumerable<ShortEmployee> GetModelsByOrganisation(Guid organisationUID)
		{
			return EmployeeHelper.GetShortByOrganisation(organisationUID);
		}

		protected override bool MarkDeleted(ShortEmployee model)
		{
			return EmployeeHelper.MarkDeleted(model);
		}

		protected override bool Restore(ShortEmployee model)
		{
			return EmployeeHelper.Restore(model);
		}

		protected override void AfterRemove(ShortEmployee model)
		{
			base.AfterRemove(model);
			ServiceFactoryBase.Events.GetEvent<EditEmployee2Event>().Publish(model.UID);
		}

		protected override void AfterRestore(ShortEmployee model)
		{
			base.AfterRestore(model);
			ServiceFactoryBase.Events.GetEvent<EditEmployee2Event>().Publish(model.UID);
		}

		public bool IsEmployeeSelected
		{
			get { return SelectedItem != null && !SelectedItem.IsOrganisation; }
		}

		protected override void UpdateSelected()
		{
			OnPropertyChanged(() => IsEmployeeSelected);
			if (SelectedItem != null)
			{
				SelectedItem.UpdatePhoto();
				SelectedItem.InitializeCards();
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }

		PersonType _personType;
		public PersonType PersonType
		{
			get { return _personType; }
			private set
			{
				_personType = value;
				OnPropertyChanged(() => PersonType);
				OnPropertyChanged(() => ItemRemovingName);
				OnPropertyChanged(() => AddCommandToolTip);
				OnPropertyChanged(() => RemoveCommandToolTip);
				OnPropertyChanged(() => EditCommandToolTip);
				OnPropertyChanged(() => TabItemHeader);
			}
		}

		protected override string ItemRemovingName
		{
			get { return PersonType == PersonType.Employee ? "сотрудника" : "посетителя"; }
		}

		public string AddCommandToolTip
		{
			get { return "Добавить " + ItemRemovingName; }
		}

		public string RemoveCommandToolTip
		{
			get { return "Удалить " + ItemRemovingName; }
		}

		public string EditCommandToolTip
		{
			get { return "Редактировать " + ItemRemovingName; }
		}

		public string TabItemHeader
		{
			get { return PersonType == PersonType.Employee ? "Сотрудники" : "Посетители"; }
		}

		protected override bool Add(ShortEmployee item)
		{
			throw new NotImplementedException();
		}

		void OnEditEmployee(Guid employeeUID)
		{
			var viewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model.UID == employeeUID);
			if (viewModel != null)
			{
				var model = EmployeeHelper.GetSingleShort(employeeUID);
				if(model != null)
					viewModel.Update(model);
			}
		}

		protected override void OnEdit()
		{
			base.OnEdit();
			ServiceFactoryBase.Events.GetEvent<EditEmployee2Event>().Publish(SelectedItem.Model.UID);
		}

		void OnUpdateIsInGrid(object obj)
		{
			InitializeAdditionalColumns();
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_Employees_Edit; }
		}

		public RelayCommand PrintTemplatesCommand { get; private set; }
	}
}