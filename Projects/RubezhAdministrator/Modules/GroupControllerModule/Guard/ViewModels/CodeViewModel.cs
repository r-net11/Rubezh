using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class CodeViewModel : BaseViewModel
	{
		public GKCode Code { get; private set; }

		public CodeViewModel(GKCode code)
		{
			Code = code;
		}

		public void Update()
		{
			OnPropertyChanged(() => Code);
		}
	}
}