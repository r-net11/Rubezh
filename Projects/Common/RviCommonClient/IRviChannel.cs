namespace RviCommonClient
{
	public interface IRviChannel
	{
		string Name { set; get; }
	
		int Number { set; get; }

		IRviStream[] Streams { set; get; }

		int CountPresets { set; get; }

		int CountTemplateBypass { set; get; }

		int CountTemplatesAutoscan { set; get; }

		int Vendor { set; get; }
	}
}