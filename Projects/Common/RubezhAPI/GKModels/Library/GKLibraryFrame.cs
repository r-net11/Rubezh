using RubezhAPI.Plans.Devices;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Кадр библиотеки устройств ГК
	/// </summary>
	[DataContract]
	public class GKLibraryFrame : ILibraryFrame
	{
		public GKLibraryFrame()
		{
			Duration = 500;
			Image = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
		}

		/// <summary>
		/// Номер
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Длительность
		/// </summary>
		[DataMember]
		public int Duration { get; set; }

		/// <summary>
		/// Данные
		/// </summary>
		[DataMember]
		public string Image { get; set; }
	}
}