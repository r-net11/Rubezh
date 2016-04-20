using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class DependencyItemsViewModel : DialogViewModel
	{
		public ObservableCollection<DependencyItemViewModel> DependencyItemViewModel { get; set; }
		public DependencyItemsViewModel(List <GKBase> gkBase)
		{
			DependencyItemViewModel = new ObservableCollection<DependencyItemViewModel>();
			gkBase.ForEach(x => DependencyItemViewModel.Add(new DependencyItemViewModel(x)));
			Title = "Список зависящих объектов";
		}
	}
}
