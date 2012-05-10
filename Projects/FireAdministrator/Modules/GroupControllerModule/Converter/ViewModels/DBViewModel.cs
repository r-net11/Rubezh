using GKModule.Converter.Binary;
using GKModule.ViewModels;
using Infrastructure.Common;

namespace GKModule.Converter
{
	public class DBViewModel : BaseViewModel
	{
		public DBViewModel(GkDatabase gkDB)
		{
			GkDB = gkDB;
			Device = new BinObjectViewModel(gkDB.RootDevice);
		}

		public DBViewModel(KauDatabase kauDB)
		{
			KauDB = kauDB;
			Device = new BinObjectViewModel(kauDB.RootDevice);
		}

		public GkDatabase GkDB { get; private set; }
		public KauDatabase KauDB { get; private set; }
		public BinObjectViewModel Device { get; private set; }
	}
}