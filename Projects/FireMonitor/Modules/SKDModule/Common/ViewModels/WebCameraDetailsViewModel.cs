using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace SKDModule.Common.ViewModels
{
	class WebCameraDetailsViewModel : DialogViewModel
	{
		public WebCameraDetailsViewModel()
		{
			OkCommand = new RelayCommand(OnOk);
			CancelCommand = new RelayCommand(OnCancel);
			YesCommand = new RelayCommand(OnYes);
			NoCommand = new RelayCommand(OnNo);
		}

		private void OnOk()
		{
			throw new NotImplementedException();
		}

		private void OnCancel()
		{
			throw new NotImplementedException();
		}

		private void OnYes()
		{
			throw new NotImplementedException();
		}

		private void OnNo()
		{
			throw new NotImplementedException();
		}

		public RelayCommand OkCommand { get; set; }

		public RelayCommand CancelCommand { get; set; }

		public RelayCommand YesCommand { get; set; }

		public RelayCommand NoCommand { get; set; }
	}
}
