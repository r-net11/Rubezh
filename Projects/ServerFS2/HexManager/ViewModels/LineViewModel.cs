using Infrastructure.Common.Windows.ViewModels;
using System;

namespace HexManager.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
		public LineViewModel(string value)
		{
			FullContent = value;
			Count = Convert.ToInt32(value.Substring(1, 2), 16);
			Offset = Convert.ToInt32(value.Substring(3, 6), 16);
			StringOffset = value.Substring(3, 6);
			Content = value.Substring(9, Count * 2);
		}

		public int Count { get; set; }
		public int Offset { get; set; }
		public string StringOffset { get; set; }
		public string Content { get; set; }
		public string FullContent { get; set; }
	}
}