using System.Runtime.Serialization;
using StrazhAPI.GK;
using StrazhAPI.Journal;

namespace StrazhAPI.Models
{
	[DataContract]
	public class Sound
	{
		[DataMember]
		public JournalEventNameType JournalEventNameType { get; set; }

		public XStateClass StateClass { get; set; }

		[DataMember]
		public string SoundName { get; set; }

		public BeeperType BeeperType { get; set; }

		[DataMember]
		public bool IsContinious { get; set; }

		[DataMember]
		public SoundLibraryType SoundLibraryType { get; set; }
	}

	/// <summary>
	/// Определяет местоположение файлов библиотеки
	/// </summary>
	public enum SoundLibraryType
	{
		/// <summary>
		/// Не задано
		/// </summary>
		None,
		/// <summary>
		/// Аудио-файлы предопределенные системой
		/// </summary>
		System,
		/// <summary>
		/// Аудио-файлы загруженные пользователем
		/// </summary>
		User
	}
}