using GKModule.Converter.Binary;
using Infrastructure.Common;
using XFiresecAPI;
using GKModule.ViewModels;

namespace GKModule.Converter
{
	public class DBViewModel : BaseViewModel
	{
		public DBViewModel(GkDB gkDB)
		{
			GkDB = gkDB;
			Device = new BinObjectViewModel(gkDB.GkDevice);
		}

		public DBViewModel(KauDB kauDB)
		{
			KauDB = kauDB;
			Device = new BinObjectViewModel(kauDB.KauDevice);
		}

		public GkDB GkDB { get; private set; }
		public KauDB KauDB { get; private set; }
		public BinObjectViewModel Device { get; private set; }
	}
}