using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Localization.SKD.Views;
using SKDModule.Events;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class DepartmentAccessTemplateSelectionViewModel : ItemsSelectionBaseViewModel<AccessTemplate>
	{
		#region <Конструктор>

		public DepartmentAccessTemplateSelectionViewModel()
		{
			Title = CommonViews.TitleChooseAccessTemplate;
			ReleaseItemCommandText = CommonViews.ButtonDetachAccessTemplate;
			AddItemCommandText = CommonViews.ButtonAddAccessTemplate;
		}

		#endregion </Конструктор>

		#region <Методы>

		protected override void InitializeItems(Organisation organisation, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			if(organisation == null)
				throw new ArgumentNullException("organisation");

			Items = new ObservableCollection<AccessTemplate>(AccessTemplateHelper.Get(new AccessTemplateFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = new List<Guid> { organisation.UID },
				UserUID = FiresecManager.CurrentUser.UID
			}));
		}

		protected override void OnAddItem()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel();
			accessTemplateDetailsViewModel.Initialize(CurrentOrganisation);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				Items.Add(accessTemplateDetailsViewModel.Model);
				SelectedItem = accessTemplateDetailsViewModel.Model;
				ServiceFactoryBase.Events.GetEvent<NewAccessTemplateEvent>().Publish(accessTemplateDetailsViewModel.Model);
			}
		}

		#endregion </Методы>
	}
}