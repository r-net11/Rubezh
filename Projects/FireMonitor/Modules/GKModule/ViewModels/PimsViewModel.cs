using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;

namespace GKModule.ViewModels
{
	public class PimsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Pims = new List<PimViewModel>();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var pim in gkDatabase.Pims)
				{
					var pimViewModel = new PimViewModel(pim.State);
					Pims.Add(pimViewModel);
				}
			}
			SelectedPim = Pims.FirstOrDefault();
		}

		List<PimViewModel> _pim;
		public List<PimViewModel> Pims
		{
			get { return _pim; }
			set
			{
				_pim = value;
				OnPropertyChanged("Pims");
			}
		}

		PimViewModel _selectedPim;
		public PimViewModel SelectedPim
		{
			get { return _selectedPim; }
			set
			{
				_selectedPim = value;
				OnPropertyChanged("SelectedPim");
			}
		}

		public void Select(Guid pimUID)
		{
			if (pimUID != Guid.Empty)
			{
				SelectedPim = Pims.FirstOrDefault(x => x.Pim.UID == pimUID);
			}
		}
	}
}