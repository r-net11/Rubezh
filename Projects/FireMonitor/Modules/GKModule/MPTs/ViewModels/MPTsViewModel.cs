using System;
using System.Collections.Generic;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models.Layouts;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class MPTsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public MPTsViewModel()
		{
			IsVisibleBottomPanel = true;
		}
		public void Initialize()
		{
			MPTs = new List<MPTViewModel>();
			foreach (var mpt in GKManager.MPTs.OrderBy(x => x.No))
			{
				var mptViewModel = new MPTViewModel(mpt);
				MPTs.Add(mptViewModel);
			}
			SelectedMPT = MPTs.FirstOrDefault();
		}

		List<MPTViewModel> _mpts;
		public List<MPTViewModel> MPTs
		{
			get { return _mpts; }
			set
			{
				_mpts = value;
				OnPropertyChanged(()=>MPTs);
			}
		}

		MPTViewModel _selectedMPT;
		public MPTViewModel SelectedMPT
		{
			get { return _selectedMPT; }
			set
			{
				_selectedMPT = value;
				OnPropertyChanged(() => SelectedMPT);
			}
		}
		LayoutPartAdditionalProperties _properties;
		public LayoutPartAdditionalProperties Properties
		{
			get { return _properties; }
			set
			{
				_properties = value;
				IsVisibleBottomPanel = (_properties !=null) ? _properties.IsVisibleBottomPanel : false;
			}
		}
		bool _isVisibleBottomPanel;
		public bool IsVisibleBottomPanel
		{
			get { return _isVisibleBottomPanel; }
			set
			{
				_isVisibleBottomPanel = value;
				OnPropertyChanged(() => IsVisibleBottomPanel);
			}
		}
		public void Select(Guid mptUID)
		{
			if (mptUID != Guid.Empty)
			{
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
			}
		}
	}
}