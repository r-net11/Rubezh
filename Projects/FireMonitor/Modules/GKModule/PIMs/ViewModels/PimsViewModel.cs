﻿using System;
using System.Collections.Generic;
using System.Linq;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PimsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Pims = new List<PimViewModel>();
			SelectedPim = Pims.FirstOrDefault();
		}

		List<PimViewModel> _pim;
		public List<PimViewModel> Pims
		{
			get { return _pim; }
			set
			{
				_pim = value;
				OnPropertyChanged(() => Pims);
			}
		}

		PimViewModel _selectedPim;
		public PimViewModel SelectedPim
		{
			get { return _selectedPim; }
			set
			{
				_selectedPim = value;
				OnPropertyChanged(() => SelectedPim);
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