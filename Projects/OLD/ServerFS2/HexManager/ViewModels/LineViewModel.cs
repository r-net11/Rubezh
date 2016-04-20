using Infrastructure.Common.Windows.Windows.ViewModels;
using System;

namespace HexManager.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
		public LineViewModel(string value)
		{
			try
			{
				OriginalContent = value;
				Count = Convert.ToInt32(value.Substring(1, 2), 16);
				StringOffset = value.Substring(3, 4);
				StringLineType = value.Substring(7, 2);
				StringOffset = value.Substring(3, 4);
				Content = value.Substring(9, Count * 2);
				StringControlSumm = value.Substring(value.Length - 2, 2);
			}
			catch { }
		}

		public int Count { get; set; }
		public string StringCount { get; set; }
		public string StringOffset { get; set; }
		public string StringLineType { get; set; }
		public string Content { get; set; }
		public string StringControlSumm { get; set; }
		public string OriginalContent { get; set; }
	}
}