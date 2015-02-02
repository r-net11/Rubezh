using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
		public VideoViewModel(string fileName)
		{
			Title = "Видео из архива";
			SavedVideoSource = fileName;
		}

		public string SavedVideoSource { get; private set; }
	}
}