using RubezhAPI.Plans.Devices;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Состояние библиотеки устройств ГК
	/// </summary>
	[DataContract]
	public class GKLibraryState : ILibraryState<GKLibraryFrame>
	{
		public GKLibraryState()
		{
			Frames = new List<GKLibraryFrame>();
			Layer = 0;
		}

		/// <summary>
		/// Класс состояния
		/// </summary>
		[DataMember]
		public XStateClass StateClass { get; set; }

		/// <summary>
		/// Кадры
		/// </summary>
		[DataMember]
		public List<GKLibraryFrame> Frames { get; set; }

		/// <summary>
		/// Номер слоя
		/// </summary>
		[DataMember]
		public int Layer { get; set; }

		#region ILibraryState<SKDLibraryFrame,XStateClass> Members

		List<GKLibraryFrame> ILibraryState<GKLibraryFrame>.Frames
		{
			get { return Frames; }
			set { Frames = value; }
		}

		XStateClass ILibraryState<GKLibraryFrame>.StateType
		{
			get { return StateClass; }
		}

		#endregion
	}
}