using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using SKDModule.Events;
using SKDModule.PassCardDesigner.ViewModels;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentPassCardTemplateSelectionViewModel : ItemsSelectionBaseViewModel<ShortPassCardTemplate>
	{
		#region <Конструктор>

		public DepartmentPassCardTemplateSelectionViewModel()
		{
			Title = "Выбор шаблона доступа";
			ReleaseItemCommandText = "Открепить шаблон доступа";
			AddItemCommandText = "Добавить новый шаблон доступа";
		}

		#endregion </Конструктор>

		#region <Методы>

		protected override void InitializeItems(Guid organisationUID, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			Items = new ObservableCollection<ShortPassCardTemplate>(PassCardTemplateHelper.Get(new PassCardTemplateFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = new List<Guid> { organisationUID },
				UserUID = FiresecManager.CurrentUser.UID
			}));
		}

		protected override void OnAddItem()
		{
			var templateDetailsViewModel = new TemplateDetailsViewModel();
			if (templateDetailsViewModel.ShowPassCardPropertiesDialog(OrganisationHelper.GetSingle(_organisationUID)))
			{
				Items.Add(templateDetailsViewModel.Model);
				ServiceFactoryBase.Events.GetEvent<NewPassCardTemplateEvent>().Publish(templateDetailsViewModel.Model);
			}
		}

		#endregion </Методы>
	}
}