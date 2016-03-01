namespace RviCommonClient
{
	public class RviChannel : IRviChannel
	{
		#region <Реализация интерфейса IRviChannel>

		public string Name { set; get; }

		public int Number { set; get; }

		public IRviStream[] Streams { set; get; }

		public int CountPresets { set; get; }

		public int CountTemplateBypass { set; get; }

		public int CountTemplatesAutoscan { set; get; }

		public int Vendor { set; get; }

		#endregion </Реализация интерфейса IRviChannel>
	}
}