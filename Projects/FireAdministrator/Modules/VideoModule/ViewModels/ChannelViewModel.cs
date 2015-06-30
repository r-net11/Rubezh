using Entities.DeviceOriented;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class ChannelViewModel : BaseViewModel
	{
		public ChannelViewModel(int no, IChannel channel)
		{
			No = no;
			Channel = channel;
			Name = channel.Name;
		}

		public ChannelViewModel(int no)
		{
			No = no;
			Name = "Канал " + no;
		}

		public IChannel Channel { get; private set; }
		public string Name { get; private set; }
		public int No { get; private set; }
	}
}