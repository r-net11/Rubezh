using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			MPTs = new List<MPTViewModel>();
			foreach (var mpt in XManager.MPTs)
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

		public void Select(Guid mptUID)
		{
			if (mptUID != Guid.Empty)
			{
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
			}
		}
	}
}