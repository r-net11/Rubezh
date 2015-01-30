using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace JournalModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
		public VideoViewModel(Guid eventUID)
		{
			Title = "Видеофрагмент, связанный с событием";
			var fileName = RviClient.RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, null, eventUID);
		}
	}
}