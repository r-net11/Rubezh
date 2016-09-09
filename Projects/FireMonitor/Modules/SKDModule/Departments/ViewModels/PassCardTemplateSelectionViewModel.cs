using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using Localization.SKD.Views;
using SKDModule.Events;
using SKDModule.PassCardDesigner.ViewModels;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SKDModule.ViewModels
{
	public class PassCardTemplateSelectionViewModel : ItemsSelectionBaseViewModel<ShortPassCardTemplate>
	{
		public PassCardTemplateSelectionViewModel()
		{
			Title = CommonViews.TitleChoosePassCardTemplate;
			ReleaseItemCommandText = CommonViews.ButtonDetachPassCardTemplate;
			AddItemCommandText = CommonViews.ButtonAddPassCardTemplate;
		}

		protected override void InitializeItems(Organisation organisation, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			if(organisation == null)
				throw new ArgumentNullException("organisation");

			Task.Factory.StartNew(() => PassCardTemplateHelper.Get(new PassCardTemplateFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = new List<Guid> { organisation.UID },
				UserUID = FiresecManager.CurrentUser.UID
			})).ContinueWith(t =>
			{
				Items = new ObservableCollection<ShortPassCardTemplate>(t.Result);
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		protected override void OnAddItem()
		{
			var templateDetailsViewModel = new TemplateDetailsViewModel();
			if (templateDetailsViewModel.ShowPassCardPropertiesDialog(CurrentOrganisation))
			{
				Items.Add(templateDetailsViewModel.Model);
				SelectedItem = templateDetailsViewModel.Model;
				ServiceFactoryBase.Events.GetEvent<NewPassCardTemplateEvent>().Publish(templateDetailsViewModel.Model);
			}
		}
	}
}