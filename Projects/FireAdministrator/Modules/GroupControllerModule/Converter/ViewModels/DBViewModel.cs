using GKModule.Converter.Binary;
using GKModule.ViewModels;
using Infrastructure.Common;

namespace GKModule.Converter
{
	public class DBViewModel : BaseViewModel
	{
		public DBViewModel(GkDB gkDB)
		{
			GkDB = gkDB;
			Device = new BinObjectViewModel(gkDB.RootDevice);
		}

		public DBViewModel(KauDB kauDB)
		{
			KauDB = kauDB;
			Device = new BinObjectViewModel(kauDB.RootDevice);
		}

		public GkDB GkDB { get; private set; }
		public KauDB KauDB { get; private set; }
		public BinObjectViewModel Device { get; private set; }
	}
}