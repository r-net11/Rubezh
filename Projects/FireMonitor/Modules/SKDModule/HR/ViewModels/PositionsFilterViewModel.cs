﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class PositionsFilterViewModel : OrganisationBaseViewModel<ShortPosition, PositionFilter, PositionFilterItemViewModel, PositionDetailsViewModel>
	{
		public PositionsFilterViewModel()
			: base()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		public override void Initialize(PositionFilter filter)
		{
			var emptyFilter = new PositionFilter { LogicalDeletationType = filter.LogicalDeletationType, OrganisationUIDs = filter.OrganisationUIDs };
			base.Initialize(emptyFilter);
			if (filter.UIDs == null)
				return;
			var models = Organisations.SelectMany(x => x.Children).Where(x => filter.UIDs.Any(y => y == x.Model.UID));
			foreach (var model in models)
				model.IsChecked = true;
		}

		public void Initialize(List<Guid> uids, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			var filter = new PositionFilter { LogicalDeletationType = logicalDeletationType };
			Initialize(filter);
		}

		public void Initialize(List<Guid> uids, List<Guid> organisationUIDs, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			var filter = new PositionFilter { LogicalDeletationType = logicalDeletationType, UIDs = uids, OrganisationUIDs = organisationUIDs };
			Initialize(filter);
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			var models = Organisations.SelectMany(x => x.Children);
			foreach (var model in models)
				model.IsChecked = true;
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			var models = Organisations.SelectMany(x => x.Children);
			foreach (var model in models)
				model.IsChecked = false;
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
			throw new NotImplementedException();
		}

		protected override FiresecAPI.Models.PermissionType Permission
		{
			get { return FiresecAPI.Models.PermissionType.Oper_SKD_Positions_Etit; }
		}

		public List<Guid> UIDs { get { return Organisations.SelectMany(x => x.Children).Where(x => x.IsChecked).Select(x => x.Model.UID).ToList(); } }
	}
}