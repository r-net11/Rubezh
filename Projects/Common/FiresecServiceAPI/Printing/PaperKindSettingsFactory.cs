using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrazhAPI.Printing
{
	public class PaperKindSettingsFactory
	{
		public IPaperKindSetting GetAlbumPaperKind()
		{
			return new AlbumKindSetting();
		}

		public IPaperKindSetting GetBookPaperKindSetting()
		{
			return new BookOrientationSettings();
		}

		public IPaperKindSetting GetUserKindSettings()
		{
			return new UserKindSetting();
		}

		public List<IPaperKindSetting> GetAllPaperKindSettings()
		{
			return new List<IPaperKindSetting>
			{
				GetAlbumPaperKind(),
				GetBookPaperKindSetting(),
				GetUserKindSettings()
			};
		}
	}
}
