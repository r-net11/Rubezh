using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class IPFilterViewModel : BaseViewModel
	{
		public IPFilterViewModel(List<string> hostNameOrAddressList)
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);

			HostNameOrAddressList = new ObservableCollection<string>(hostNameOrAddressList);
			SelectedHostNameOrAddress = HostNameOrAddressList.FirstOrDefault();
		}

		public ObservableCollection<string> HostNameOrAddressList { get; private set; }

		string _selectedHostNameOrAddress;
		public string SelectedHostNameOrAddress
		{
			get { return _selectedHostNameOrAddress; }
			set
			{
				_selectedHostNameOrAddress = value;
				OnPropertyChanged(() => SelectedHostNameOrAddress);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var ipFilterDetails = new IPFilterDetailsViewModel();
			if (DialogService.ShowModalWindow(ipFilterDetails))
			{
				if (string.IsNullOrEmpty(ipFilterDetails.HostNameOrAddress) == false &&
					HostNameOrAddressList.Any(x => x == ipFilterDetails.HostNameOrAddress) == false)
				{
					HostNameOrAddressList.Add(ipFilterDetails.HostNameOrAddress);

				}
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			HostNameOrAddressList.Remove(SelectedHostNameOrAddress);
		}

		bool CanRemove()
		{
			return SelectedHostNameOrAddress != null;
		}

		public List<string> GetModel()
		{
			return HostNameOrAddressList.ToList();
		}

	}
}