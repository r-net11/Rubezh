using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace GKProcessor
{
	public static class Patcher
	{
		static List<Patch> AllPatches;

		static Patcher()
		{
			AllPatches = new List<Patch>();
			AllPatches.Add(new Patch("DB.Patcher_RemoveTypeColumn", () =>
			{
				GKDBHelper.ExecuteNonQuery("alter table Patches drop column Type");
			}));
		}

		public static void AddPatchToList(string index, PatchDelegate patchDelegate)
        {
            AllPatches.Add(new Patch(index, patchDelegate));
        }

		public static void Patch()
		{
			try
			{
				var indexes = GKDBHelper.ReadAllPatches();
				var patchesToApply = AllPatches.Where(x => !indexes.Any(index => index == x.Index)).ToList();
				patchesToApply.ForEach(x => x.Apply());
			}
			catch (Exception e)
			{
				Logger.Error(e, "Patcher.Patch");
			}
		}
	}
}