using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Localization.SKD.ViewModels;
using SKDModule.Events;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentScheduleSelectionViewModel : ItemsSelectionBaseViewModel<Schedule>
	{
		#region <Конструктор>

		public DepartmentScheduleSelectionViewModel()
		{
			Title = CommonViewModels.SelectWorkSchedule;
			ReleaseItemCommandText = CommonViewModels.UnpinWorkSchedule;
			AddItemCommandText = CommonViewModels.AddNewWorkSchedule;
		}

		#endregion </Конструктор>

		#region <Методы>

		protected override void InitializeItems(Guid organisationUID, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			Items = new ObservableCollection<Schedule>(ScheduleHelper.Get(new ScheduleFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = new List<Guid> { organisationUID },
				UserUID = FiresecManager.CurrentUser.UID
			}));
		}

		protected override void OnAddItem()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			scheduleDetailsViewModel.Initialize(_organisationUID);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				Items.Add(scheduleDetailsViewModel.Model);
				ServiceFactoryBase.Events.GetEvent<NewScheduleEvent>().Publish(scheduleDetailsViewModel.Model);
			}
		}

		#endregion </Методы>
	}
}