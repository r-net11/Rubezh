using System.Collections.Generic;
using RubezhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class PermissionsViewModel : BaseViewModel
	{
		public PermissionsViewModel(List<string> permissionStrings)
		{
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

		void BuildPermissionTree()
		{
			RootPermission = new PermissionViewModel("Все",
				new List<PermissionViewModel>()
				{
					new PermissionViewModel("Администратор",
						new List<PermissionViewModel>()
						{
							new PermissionViewModel(PermissionType.Adm_ViewConfig),
							new PermissionViewModel(PermissionType.Adm_SetNewConfig),
							new PermissionViewModel(PermissionType.Adm_WriteDeviceConfig),
							//new PermissionViewModel(PermissionType.Adm_ChangeDevicesSoft),
							new PermissionViewModel(PermissionType.Adm_Security),

							}),

					new PermissionViewModel("ОЗ",
						new List<PermissionViewModel>()
						{
							new PermissionViewModel(PermissionType.Oper_Login),
							new PermissionViewModel(PermissionType.Oper_Logout),
							new PermissionViewModel(PermissionType.Oper_LogoutWithoutPassword),
							new PermissionViewModel(PermissionType.Oper_NoAlarmConfirm),
							new PermissionViewModel(PermissionType.Oper_MayNotConfirmCommands),
							new PermissionViewModel("Управления ГК", new List<PermissionViewModel>()
							{
								new PermissionViewModel(PermissionType.Oper_ScheduleSKD),
							    new PermissionViewModel(PermissionType.Oper_Device_Control),
								new PermissionViewModel(PermissionType.Oper_Zone_Control),
								new PermissionViewModel(PermissionType.Oper_ZonesSKD),
								new PermissionViewModel("Охранные зоны", new List<PermissionViewModel>()
								{
									new PermissionViewModel(PermissionType.Oper_GuardZone_Control),
									new PermissionViewModel(PermissionType.Oper_ExtraGuardZone),
								}),
								new PermissionViewModel(PermissionType.Oper_Directions_Control),
								new PermissionViewModel("Точки доступа", new List<PermissionViewModel>()
								{
									new PermissionViewModel(PermissionType.Oper_Door_Control),
									new PermissionViewModel(PermissionType.Oper_Full_Door_Control),
								}),
								new PermissionViewModel(PermissionType.Oper_MPT_Control),
								new PermissionViewModel(PermissionType.Oper_GlobalPIM_Control),
								new PermissionViewModel(PermissionType.Oper_Delay_Control),
								new PermissionViewModel(PermissionType.Oper_NS_Control),
								new PermissionViewModel(PermissionType.Oper_ChangeView),
								}),
							new PermissionViewModel("Отчеты", new List<PermissionViewModel>()
								{
									new PermissionViewModel("Конфигурация", new List<PermissionViewModel>()
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Doors),
											new PermissionViewModel(PermissionType.Oper_Reports_Mirror),
											new PermissionViewModel(PermissionType.Oper_Reports_Devices),
										}),
									new PermissionViewModel("События", new List<PermissionViewModel>()
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Events),
											new PermissionViewModel(PermissionType.Oper_Reports_EmployeeRoot),
										}),
									new PermissionViewModel("Картотека", new List<PermissionViewModel>()
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Cards),
											new PermissionViewModel(PermissionType.Oper_Reports_Employees_Access),
											new PermissionViewModel(PermissionType.Oper_Reports_Employees_Rights),
											new PermissionViewModel(PermissionType.Oper_Reports_Departments),
											new PermissionViewModel(PermissionType.Oper_Reports_Positions),
											new PermissionViewModel(PermissionType.Oper_Reports_EmployeeZone),
											new PermissionViewModel(PermissionType.Oper_Reports_Employee),
										}),
									new PermissionViewModel("Учет рабочего времени", new List<PermissionViewModel>()
										{
											new PermissionViewModel(PermissionType.Oper_Reports_Discipline),
											new PermissionViewModel(PermissionType.Oper_Reports_Schedules),
											new PermissionViewModel(PermissionType.Oper_Reports_Documents),
											new PermissionViewModel(PermissionType.Oper_Reports_WorkTime),
											new PermissionViewModel(PermissionType.Oper_Reports_T13),
										}),
								}),
							new PermissionViewModel("СКД", new List<PermissionViewModel>()
								{
									new PermissionViewModel("Картотека", new List<PermissionViewModel>()
										{
											new PermissionViewModel("Сотрудники", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Employees_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Employees_Edit),
											}),
											new PermissionViewModel("Посетители", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Guests_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Guests_Edit),
											}),
											new PermissionViewModel("Подразделения", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Departments_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Departments_Etit),
											}),
											new PermissionViewModel("Должности", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Positions_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Positions_Etit),
											}),
											new PermissionViewModel("Дополнительные колонки", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_AdditionalColumns_View),
												new PermissionViewModel(PermissionType.Oper_SKD_AdditionalColumns_Etit),
											}),
											new PermissionViewModel("Пропуска", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Cards_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Cards_Etit),
												new PermissionViewModel(PermissionType.Oper_SKD_Employees_Edit_CardType),
											}),
											new PermissionViewModel("Шаблоны доступа", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_AccessTemplates_View),
												new PermissionViewModel(PermissionType.Oper_SKD_AccessTemplates_Etit),
											}),
											new PermissionViewModel("Шаблоны пропусков", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_PassCards_View),
												new PermissionViewModel(PermissionType.Oper_SKD_PassCards_Etit),
											}),
											new PermissionViewModel("Организации", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_View),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_Users),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_Doors),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_Organisations_AddRemove),
											}),
										}),
									new PermissionViewModel("Учет рабочего времени", new List<PermissionViewModel>()
										{
											new PermissionViewModel("Дневные графики", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_DaySchedules_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit),
											}),
											new PermissionViewModel("Графики", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit),
											}),
											new PermissionViewModel("Праздничные дни", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Holidays_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Holidays_Edit),
											}),
											new PermissionViewModel("Графики работ", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Schedules_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Schedules_Edit),
											}),
											new PermissionViewModel("Учет рабочего времени ", new List<PermissionViewModel>()
											{
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Report_View),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Parts_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_Documents_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit),
												new PermissionViewModel(PermissionType.Oper_SKD_TimeTrack_NightSettings_Edit),
											}),
										}),
								}),
						}),
				});
		}

		public List<string> GetPermissionStrings()
		{
			var result = new List<string>();
			foreach (var permission in AllPermissions)
			{
				if (permission.IsChecked && permission.IsPermission)
					result.Add(permission.PermissionType.ToString());
			}
			return result;
		}
	}
}