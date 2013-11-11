namespace GKProcessor
{
	public delegate void PatchDelegate();

	class Patch
	{
		PatchDelegate PatchDelegate;
		public string Index{ get; private set; }
		
		public Patch(string index, PatchDelegate patchDelegate)
		{
			Index = index;
			PatchDelegate = patchDelegate;
		}

		public void Apply()
		{
			try
			{
				PatchDelegate();
				GKDBHelper.AddPatchIndexToDB(Index);
			}
			catch { }
		}
	}
}