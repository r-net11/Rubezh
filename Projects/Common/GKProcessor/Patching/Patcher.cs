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
				GKDBHelper.DropColumn("Type", "Patches");
			}));
			AllPatches.Add(new Patch("DB.Journal_RemoveYesNoColumn", () =>
			{
				GKDBHelper.DropColumn("YesNo", "Journal");
			}));
			AllPatches.Add(new Patch("DB.Journal_ChangeDescriptionColumnSize", () =>
			{
				GKDBHelper.AlterColumnType("Description", "nvarchar(4000)", "Journal");
			}));
			AllPatches.Add(new Patch("DB.Patches_ChangeDescriptionColumnSize", () =>
			{
				GKDBHelper.AlterColumnType("Id", "nvarchar(4000)", "Patches");
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