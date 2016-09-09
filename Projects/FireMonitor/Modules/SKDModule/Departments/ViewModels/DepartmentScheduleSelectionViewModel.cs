using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Localization.SKD.Views;
using SKDModule.Events;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentScheduleSelectionViewModel : ItemsSelectionBaseViewModel<Schedule>
	{
		#region <Конструктор>

		public DepartmentScheduleSelectionViewModel()
		{
			Title = CommonViews.TitleChooseScheduleWork;
			ReleaseItemCommandText = CommonViews.ButtonDetachScheduleWork;
			AddItemCommandText = CommonViews.ButtonAddNewScheduleWork;
		}

		#endregion </Конструктор>

		#region <Методы>

		protected override void InitializeItems(Organisation organisation, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			if(organisation == null)
				throw new ArgumentNullException("organisation");

			Items = new ObservableCollection<Schedule>(ScheduleHelper.Get(new ScheduleFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = new List<Guid> { organisation.UID },
				UserUID = FiresecManager.CurrentUser.UID
			}));
		}

		protected override void OnAddItem()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			scheduleDetailsViewModel.Initialize(CurrentOrganisation);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				Items.Add(scheduleDetailsViewModel.Model);
				SelectedItem = scheduleDetailsViewModel.Model;
				ServiceFactoryBase.Events.GetEvent<NewScheduleEvent>().Publish(scheduleDetailsViewModel.Model);
			}
		}

		#endregion </Методы>
	}
}