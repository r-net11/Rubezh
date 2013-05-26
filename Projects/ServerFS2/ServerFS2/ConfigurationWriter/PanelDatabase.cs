using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public RomDatabase RomDatabase { get; set; }
		public FlashDatabase FlashDatabase { get; set; }
		public Device ParentPanel { get; set; }

		public PanelDatabase(Device parentDevice)
		{
			ParentPanel = parentDevice;
			FlashDatabase = new FlashDatabase(parentDevice);
			RomDatabase = new RomDatabase(FlashDatabase, 0x2000);
		}
	}
}