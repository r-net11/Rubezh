using RubezhAPI.Models;

namespace VideoModule.ViewModels
{
	class RviStreamViewModel
	{
		public string Name { get { return string.Format("Поток {0}", StreamNumber); } }
		public int StreamNumber { get; private set; }
		public RviStreamViewModel(RviStream rviStream)
		{
			StreamNumber = rviStream.Number;
		}
	}
}
