using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace FiresecAPI.GK
{
	public class LibraryXFrame : ILibraryFrame
	{
		public LibraryXFrame()
		{
			Duration = 500;
			Image = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
		}

		public int Id { get; set; }
		public int Duration { get; set; }
		public string Image { get; set; }
	}
}