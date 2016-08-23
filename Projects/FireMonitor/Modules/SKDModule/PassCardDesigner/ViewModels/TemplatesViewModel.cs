using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using Infrastructure.Common.Services;
using SKDModule.Events;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class TemplatesViewModel : OrganisationBaseViewModel<ShortPassCardTemplate, PassCardTemplateFilter, TemplateViewModel, TemplateDetailsViewModel>
	{
		public TemplatesViewModel()
		{
			OpenDesignerCommand = new RelayCommand(OnOpenDesignerCommand, CanOpenDesignerCommand);
			ServiceFactoryBase.Events.GetEvent<NewPassCardTemplateEvent>().Unsubscribe(OnNewPassCardTemplate);
			ServiceFactoryBase.Events.GetEvent<NewPassCardTemplateEvent>().Subscribe(OnNewPassCardTemplate);
		}

		private void OnNewPassCardTemplate(ShortPassCardTemplate shortPassCardTemplate)
		{
			var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == shortPassCardTemplate.OrganisationUID);
			if (organisation != null)
			{
				var viewModel = new TemplateViewModel();
				viewModel.InitializeModel(organisation.Organisation, shortPassCardTemplate, this);
				organisation.AddChild(viewModel);
			}
		}

		#region Commands

		public RelayCommand OpenDesignerCommand { get; private set; }

		public void OnOpenDesignerCommand()
		{
			var designer = new TemplateDetailsViewModel();
			designer.InitializeDesigner(SelectedItem.Organisation, SelectedItem.Model);

			if(DialogService.ShowModalWindow(designer))
				SelectedItem.Update(SelectedItem.Model);
		}

		public bool CanOpenDesignerCommand()
		{
			return CanEdit();
		}

		protected override void OnAdd()
		{
			var details = new TemplateDetailsViewModel();
			if (details.ShowPassCardPropertiesDialog(SelectedItem.Organisation))
			{
				var itemViewModel = new TemplateViewModel();
				itemViewModel.InitializeModel(SelectedItem.Organisation, details.Model, this);
				var parentViewModel = GetParentItem(details.Model);
				parentViewModel.AddChild(itemViewModel);
				SelectedItem = itemViewModel;
			}
		}

		protected override void OnEdit()
		{
			var details = new TemplateDetailsViewModel();

			if (details.ShowPassCardPropertiesDialog(SelectedItem.Organisation, SelectedItem.Model))
			{
				SelectedItem.Update(details.Model);
				UpdateSelected();
				UpdateParent();
			}
		}

		#endregion

		protected override IEnumerable<ShortPassCardTemplate> GetModels(PassCardTemplateFilter filter)
		{
			return PassCardTemplateHelper.Get(filter);
		}
		protected override IEnumerable<ShortPassCardTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			return PassCardTemplateHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ShortPassCardTemplate model)
		{
			return PassCardTemplateHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortPassCardTemplate model)
		{
			return PassCardTemplateHelper.Restore(model);
		}
		protected override bool Add(ShortPassCardTemplate item)
		{
			var passCardTemplate = PassCardTemplateHelper.GetDetails(ClipboardUID);
			passCardTemplate.UID = item.UID;
			passCardTemplate.Description = item.Description;
			passCardTemplate.Caption = item.Name;
			passCardTemplate.OrganisationUID = item.OrganisationUID;
			return PassCardTemplateHelper.Save(passCardTemplate, true);
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_PassCards_Etit; }
		}

	}
}
