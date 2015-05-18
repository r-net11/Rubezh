using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.ServiceModel;
using FiresecAPI;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TestCommand = new RelayCommand(OnTest);
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
		}
	}
}