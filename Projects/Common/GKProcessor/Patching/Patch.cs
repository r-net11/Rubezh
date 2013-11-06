
namespace GKProcessor
{
	public delegate void PatchDelegate();

	class Patch
	{
		PatchDelegate PatchDelegate;
		string index;
		public string Index 
		{
			get { return index; }
			private set
			{
				if (value.Length > 20)
					index = value.Substring(0, 20);
			}
		}

		public Patch(string index, PatchDelegate patchDelegate)
		{
			Index = index;
			PatchDelegate = patchDelegate;
		}

		public void Apply()
		{
			PatchDelegate();
			GKDBHelper.AddPatchIndexToDB(Index);
		}
	}
}