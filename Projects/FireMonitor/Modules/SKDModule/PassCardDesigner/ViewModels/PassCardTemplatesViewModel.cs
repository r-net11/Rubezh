using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplatesViewModel : OrganisationBaseViewModel<ShortPassCardTemplate, PassCardTemplateFilter, PassCardTemplateViewModel, PassCardTemplateDetailsViewModel>
	{
		private PassCardTemplateDetailsViewModel PassCardTemplateDetailsViewModel;

		public PassCardTemplatesViewModel():base()
		{
			OpenDesignerCommand = new RelayCommand(OnOpenDesignerCommand, CanOpenDesignerCommand);
		}

		#region Commands

		public RelayCommand OpenDesignerCommand { get; private set; }

		public void OnOpenDesignerCommand()
		{
			PassCardTemplateDetailsViewModel = new PassCardTemplateDetailsViewModel();
			if (PassCardTemplateDetailsViewModel.Initialize(SelectedItem.Organisation, SelectedItem.Model) && DialogService.ShowModalWindow(PassCardTemplateDetailsViewModel))
			{
				SelectedItem.Update(SelectedItem.Model);
			}

		}

		public bool CanOpenDesignerCommand()
		{
			return CanEdit();
		}

		protected override void OnAdd()
		{
			PassCardTemplateDetailsViewModel = new PassCardTemplateDetailsViewModel();
			if (PassCardTemplateDetailsViewModel.ShowPassCardPropertiesDialog(SelectedItem.Organisation))
			{
				var itemViewModel = new PassCardTemplateViewModel();
				itemViewModel.InitializeModel(SelectedItem.Organisation, PassCardTemplateDetailsViewModel.Model, this);
				var parentViewModel = GetParentItem(PassCardTemplateDetailsViewModel.Model);
				parentViewModel.AddChild(itemViewModel);
				SelectedItem = itemViewModel;
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
			var passCardTemplate = PassCardTemplateHelper.GetDetails(_clipboardUID);
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

		protected override FiresecAPI.Models.PermissionType Permission
		{
			get { return FiresecAPI.Models.PermissionType.Oper_SKD_PassCards_Etit; }
		}

	}
}
