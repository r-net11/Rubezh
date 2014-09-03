using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalsViewModel : CartothequeTabItemCopyPasteBase<DayInterval, DayIntervalFilter, DayIntervalViewModel, DayIntervalDetailsViewModel> , ISelectable<Guid>
	{
		bool _isInitialized;

		public DayIntervalsViewModel()
			:base()
		{
			_isInitialized = false;
		}

		public override void OnShow()
		{
			base.OnShow();
			var filter = new DayIntervalFilter() { UserUID = FiresecManager.CurrentUser.UID };
			if (!_isInitialized)
			{
				Initialize(filter);
				_isInitialized = true;
			}
		}

        protected override void OnEditOrganisation(Organisation newOrganisation)
        {
            if (_isInitialized)
                base.OnEditOrganisation(newOrganisation);
        }

        protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
        {
            if (_isInitialized)
                base.OnOrganisationUsersChanged(newOrganisation);
        }

		public void Select(Guid dayIntervalUID)
		{
			if (dayIntervalUID != Guid.Empty)
			{
				var dayIntervalViewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model != null && x.Model.UID == dayIntervalUID);
				if (dayIntervalViewModel != null)
					dayIntervalViewModel.ExpandToThis();
				SelectedItem = dayIntervalViewModel;
			}
		}

		protected override void UpdateSelected()
		{
			SelectedItem.Initialize();
		}

		protected override IEnumerable<DayInterval> GetModels(DayIntervalFilter filter)
		{
			return DayIntervalHelper.Get(filter);
		}
		protected override IEnumerable<DayInterval> GetModelsByOrganisation(Guid organisationUID)
		{
			return DayIntervalHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(Guid uid)
		{
			return DayIntervalHelper.MarkDeleted(uid);
		}
		protected override bool Save(DayInterval item)
		{
			return DayIntervalHelper.Save(item);
		}

		protected override DayInterval CopyModel(DayInterval source, bool newName = true)
		{
			var copy = base.CopyModel(source, newName);
			copy.Description = source.Description;
            copy.DayIntervalParts = source.DayIntervalParts;
			return copy;
		}

        protected override string ItemRemovingName
        {
            get { return "дневной график"; }
        }
	}
}