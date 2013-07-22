using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public RomDatabase RomDatabase { get; set; }
		public FlashDatabase FlashDatabase { get; set; }
		public Device ParentPanel { get; set; }

		public PanelDatabase(Device parentDevice, int offset)
		{
			ParentPanel = parentDevice;
			FlashDatabase = new FlashDatabase(parentDevice);
			RomDatabase = new RomDatabase(FlashDatabase, offset);
		}
	}
}