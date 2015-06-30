using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class CodeViewModel : BaseViewModel
	{
		public CodeViewModel(GKCode code)
		{
			Code = code;
		}

		GKCode _code;
		public GKCode Code
		{
			get { return _code; }
			set
			{
				_code = value;
				OnPropertyChanged(() => Code);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Code);
		}
	}
}