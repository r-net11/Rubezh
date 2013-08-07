using Infrastructure.Common.Windows.ViewModels;

namespace HexManager.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
		public LineViewModel(string line)
		{
			Content = line;
		}

		public string Content { get; set; }
	}
}