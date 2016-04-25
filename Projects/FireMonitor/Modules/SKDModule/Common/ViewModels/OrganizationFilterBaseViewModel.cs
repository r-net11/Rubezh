using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class OrganisationFilterBaseViewModel<T> : SaveCancelDialogViewModel
		where T : OrganisationFilterBase, new()
	{
		public HROrganisationCheckBoxItemList<T> Organisations { get; private set; }

		public OrganisationFilterBaseViewModel(T filter)
		{
			ResetCommand = new RelayCommand(OnReset);
			Title = "Фильтр";
			Initialize(filter);
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new T();
			Initialize(Filter);
		}

		public virtual void Initialize(T filter)
		{
			Filter = filter;
			_isWithDeleted = filter.LogicalDeletationType == LogicalDeletationType.All;
			InitializeOrganisations(filter);
		}

		public T Filter { get; private set; }
		
		bool _isWithDeleted;
		public bool IsWithDeleted
		{
			get { return _isWithDeleted; }
			set
			{
				_isWithDeleted = value;
				OnPropertyChanged(() => IsWithDeleted);
				Filter.LogicalDeletationType = IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
				Filter.OrganisationUIDs = OrganisationUids;
				InitializeOrganisations(Filter);
				UpdateTabs();
			}
		}

		public void InitializeOrganisations(T filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser(filter.LogicalDeletationType);
			Organisations = new HROrganisationCheckBoxItemList<T>(this);
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationViewModel(organisation));
				}
			}

			foreach (var organisation in Organisations.Items)
			{
				organisation.IsChecked = Filter.OrganisationUIDs.Any(x => x == organisation.Organisation.UID);
			}

			OnPropertyChanged(() => Organisations);
		}

		public List<Guid> OrganisationUids { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }

		public void UpdateTabs()
		{
			foreach(var tab in HRFilterTabs.Where(x => x != null))
			{
				tab.Update(OrganisationUids, IsWithDeleted);
			}
		}

		protected virtual List<IHRFilterTab> HRFilterTabs { get { return new List<IHRFilterTab>(); } }

		protected override bool Save()
		{
			Filter.OrganisationUIDs = OrganisationUids;
			return true;
		}
	}

	public interface IHRFilterTab
	{
		void Update(List<Guid> organisationUids, bool isWithDeleted);
	}

	public class HROrganisationCheckBoxItemList<T> : CheckBoxItemList<FilterOrganisationViewModel>
		where T : OrganisationFilterBase, new()
	{
		OrganisationFilterBaseViewModel<T> _parent;

		public HROrganisationCheckBoxItemList(OrganisationFilterBaseViewModel<T> parent)
			: base()
		{
			_parent = parent;
		}

		public override void Update()
		{
			base.Update();
			_parent.UpdateTabs();
		}
	}
}