using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using SKDModule.Events;


namespace SKDModule.ViewModels
{
	public class PositionsViewModel : OrganisationBaseViewModel<ShortPosition, PositionFilter, PositionViewModel, PositionDetailsViewModel>
	{
		public PositionsViewModel():base()
		{
			ServiceFactory.Events.GetEvent<NewPositionEvent>().Unsubscribe(OnNewPosition);
			ServiceFactory.Events.GetEvent<NewPositionEvent>().Subscribe(OnNewPosition);
		}
		
		protected override IEnumerable<ShortPosition> GetModels(PositionFilter filter)
		{
			return PositionHelper.Get(filter);
		}
		protected override IEnumerable<ShortPosition> GetModelsByOrganisation(Guid organisationUID)
		{
			return PositionHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ShortPosition model)
		{
			return PositionHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortPosition model)
		{
			return PositionHelper.Restore(model);
		}
		protected override bool Add(ShortPosition item)
		{
			var position = PositionHelper.GetDetails(_clipboardUID);
			position.UID = item.UID;
			position.Description = item.Description;
			position.Name = item.Name;
			position.OrganisationUID = item.OrganisationUID;
			return PositionHelper.Save(position, true);
		}
		
		protected override string ItemRemovingName
		{
			get { return "должность"; }
		}

		public void OnNewPosition(ShortPosition model) 
		{
			var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == model.OrganisationUID);
			if (organisation != null)
			{
				var viewModel = new PositionViewModel();
				viewModel.InitializeModel(organisation.Organisation, model, this);
				organisation.AddChild(viewModel);
			}
		}

		protected override void Remove()
		{
			if (SelectedItem.EmployeeListViewModel.Employees.Count == 0 ||
				MessageBoxService.ShowQuestion("Существуют привязанные к должности сотрудники. Продожить?"))
			{
				var employeeUIDs = SelectedItem.EmployeeListViewModel.Employees.Select(x => x.Employee.UID);
				base.Remove();
				foreach (var uid in employeeUIDs)
				{
					ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
				}
			}
		}

		protected override void Restore()
		{
			base.Restore();
			var employeeUIDs = SelectedItem.EmployeeListViewModel.Employees.Select(x => x.Employee.UID);
			foreach (var uid in employeeUIDs)
			{
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
			}
		}
	}
}