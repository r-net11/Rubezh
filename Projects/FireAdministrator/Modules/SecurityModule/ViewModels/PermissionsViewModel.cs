using System.Collections.Generic;
using System.Linq;
using Localization.Security.ViewModels;
using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class PermissionsViewModel : BaseViewModel
	{
		private bool _isReadOnly;

		public PermissionsViewModel(List<string> permissionStrings, bool isReadOnly = false)
		{
			_isReadOnly = isReadOnly;
			BuildPermissionTree();
			FillAllPermissions();

			RootPermission.IsExpanded = true;
			SelectedPermission = RootPermission;
			foreach (var permission in AllPermissions)
			{
				permission.ExpandToThis();
				permission.IsChecked = permissionStrings.Contains(permission.PermissionType.ToString());
			}

			OnPropertyChanged(() => RootPermissions);
		}

		public List<PermissionViewModel> AllPermissions;

		public void FillAllPermissions()
		{
			AllPermissions = new List<PermissionViewModel>();
			AddChildPlainPermissions(RootPermission);
		}

		void AddChildPlainPermissions(PermissionViewModel parentViewModel)
		{
			AllPermissions.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainPermissions(childViewModel);
		}

		PermissionViewModel _selectedPermission;
		public PermissionViewModel SelectedPermission
		{
			get { return _selectedPermission; }
			set
			{
				_selectedPermission = value;
				OnPropertyChanged(() => SelectedPermission);
			}
		}

		PermissionViewModel _rootPermission;
		public PermissionViewModel RootPermission
		{
			get { return _rootPermission; }
			private set
			{
				_rootPermission = value;
				OnPropertyChanged(() => RootPermission);
			}
		}

		public PermissionViewModel[] RootPermissions
		{
			get { return new[] { RootPermission }; }
		}

		public void BuildPermissionTree()
		{
			RootPermission = new PermissionViewModel(CommonViewModels.Permissions_All,
				new List<PermissionViewModel>
				{
					new PermissionViewModel(CommonViewModels.Permissions_Admin,
						new List<PermissionViewModel>
						{
							new PermissionViewModel(PermissionType.Adm_ViewConfig),
							new PermissionViewModel(PermissionType.Adm_SetNewConfig),
							new PermissionViewModel(PermissionType.Adm_WriteDeviceConfig),
							new PermissionViewModel(PermissionType.Adm_ChangeDevicesSoft),
							new PermissionViewModel(PermissionType.Adm_Security),
						}),
					new PermissionViewModel(CommonViewModels.Permissions_OT,
						new List<PermissionViewModel>
						{
							new PermissionViewModel(PermissionType.Oper_Login),
							new PermissionViewModel(PermissionType.Oper_Logout),
							new PermissionViewModel(PermissionType.Oper_LogoutWithoutPassword),
							new PermissionViewModel(PermissionType.Oper_NoAlarmConfirm),
							new PermissionViewModel(PermissionType.Oper_ControlDevices),
							new PermissionViewModel(PermissionType.Oper_ChangeView),
							new PermissionViewModel(PermissionType.Oper_MayNotConfirmCommands),
							new PermissionViewModel(PermissionType.Oper_GKSchedules),
							new PermissionViewModel(CommonViewModels.Permissions_JournalArchive, new List<PermissionViewModel>
								{
									new PermissionViewModel(PermissionType.Oper_Journal_View),
									new PermissionViewModel(PermissionType.Oper_Archive_View),
									new PermissionViewModel(PermissionType.Oper_Archive_Settings),
								}),
							new PermissionViewModel(CommonViewModels.Permissions_Reports, new List<PermissionViewModel>
								{
									new PermissionViewModel(CommonViewModels.Permissions_Config, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Doors),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_Events, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Events),
											new PermissionViewModel(PermissionType.Oper_Reports_EmployeeRoot),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_Cards, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Cards),
											new PermissionViewModel(PermissionType.Oper_Reports_Employees_Access),
											new PermissionViewModel(PermissionType.Oper_Reports_Employees_Rights),
											new PermissionViewModel(PermissionType.Oper_Reports_Departments),
											new PermissionViewModel(PermissionType.Oper_Reports_Positions),
											new PermissionViewModel(PermissionType.Oper_Reports_EmployeeZone),
											new PermissionViewModel(PermissionType.Oper_Reports_Employee),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_WorkTime, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Discipline),
											new PermissionViewModel(PermissionType.Oper_Reports_Schedules),
											new PermissionViewModel(PermissionType.Oper_Reports_Documents),
											new PermissionViewModel(PermissionType.Oper_Reports_WorkTime),
											new PermissionViewModel(PermissionType.Oper_Reports_T13),
										}),
								}),
							new PermissionViewModel(CommonViewModels.Permissions_SKD, new List<PermissionViewModel>
								{
									new PermissionViewModel(CommonViewModels.Permissions_Devices, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Strazh_Devices_View),
											new PermissionViewModel(PermissionType.Oper_Strazh_Devices_Control),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_Zones, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Strazh_Zones_View),
											new PermissionViewModel(PermissionType.Oper_Strazh_Zones_Control),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_Doors, new List<PermissionViewModel>
										{
											new PermissionViewModel(PermissionType.Oper_Strazh_Doors_View),
											new PermissionViewModel(PermissionType.Oper_Strazh_Doors_Control),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_Cards, new List<PermissionViewModel>
										{
											new PermissionViewModel(CommonViewModels.Permissions_Employees, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Employees_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Employees_Edit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_Guests, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Guests_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Guests_Edit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_Departs, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Departments_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Departments_Etit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_Positions, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Positions_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Positions_Etit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_AdditionalColumns, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_AdditionalColumns_View),
												new PermissionViewModel(PermissionType.Oper_SKD_AdditionalColumns_Etit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_Pass, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Cards_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Cards_Etit),
												new PermissionViewModel(PermissionType.Oper_SKD_Cards_ResetRepeatEnter),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_AccessTemplate, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_AccessTemplates_View),
												new PermissionViewModel(PermissionType.Oper_SKD_AccessTemplates_Etit),
											}),
											//new PermissionViewModel(CommonViewModels.Permissions_PassTemplate, new List<PermissionViewModel>
											//{
											//	new PermissionViewModel(PermissionType.Oper_SKD_PassCards_View),
											//	new PermissionViewModel(PermissionType.Oper_SKD_PassCards_Etit),
											//}),
											new PermissionViewModel(CommonViewModels.Permissions_Organisations, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_Users),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_Doors),
											}),
										}),
									new PermissionViewModel(CommonViewModels.Permissions_WorkTime, new List<PermissionViewModel>
										{
											new PermissionViewModel(CommonViewModels.Permissions_DayPlans, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_DaySchedules_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_Plans, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_Holidays, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Holidays_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Holidays_Edit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_WorkPlan, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Schedules_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Schedules_Edit),
											}),
											new PermissionViewModel(CommonViewModels.Permissions_WorkTime, new List<PermissionViewModel>
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Report_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Parts_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Documents_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit),
											}),
										}),
								}),
						}),
				}) {IsReadOnly = _isReadOnly};
		}

		public List<string> GetPermissionStrings()
		{
			return AllPermissions
				.Where(permission => permission.IsChecked && permission.IsPermission)
				.Select(permission => permission.PermissionType.ToString())
				.ToList();
		}
	}
}