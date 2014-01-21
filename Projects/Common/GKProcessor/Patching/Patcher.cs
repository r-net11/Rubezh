using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;
using System.Diagnostics;

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
			AllPatches.Add(new Patch("DB.AddObjectName", () =>
			{
				GKDBHelper.AddColumn("ObjectName", "nvarchar(4000)", "Journal");
			}));
			AllPatches.Add(new Patch("DB.Journal_UpdatesSizes", () =>
			{
				GKDBHelper.AlterColumnType("Description", "nvarchar(100)", "Journal");
				GKDBHelper.AlterColumnType("Name", "nvarchar(100)", "Journal");
				GKDBHelper.AlterColumnType("ObjectName", "nvarchar(100)", "Journal");
			}));
			AllPatches.Add(new Patch("DB.Journal_UpdatesSizes2", () =>
			{
				GKDBHelper.AlterColumnType("Description", "nvarchar(100)", "Journal");
				GKDBHelper.AlterColumnType("Name", "nvarchar(100)", "Journal");
				GKDBHelper.AlterColumnType("ObjectName", "nvarchar(100)", "Journal");
			}));
			AllPatches.Add(new Patch("DB.Add_KAUno_AdditionalDescription", () =>
			{
				GKDBHelper.AddColumn("KAUNo", "int", "Journal");
				GKDBHelper.AddColumn("AdditionalDescription", "nvarchar(100)", "Journal");
			}));
            AllPatches.Add(new Patch("DB.Add_Indexes", () =>
            {
                GKDBHelper.AddIndex("No_Index", "Journal", "GKJournalRecordNo");
                GKDBHelper.AddIndex("Name_Index", "Journal", "Name");
                GKDBHelper.AddIndex("Description_Index", "Journal", "Description");
                GKDBHelper.AddIndex("SystemDateTime_Index", "Journal", "SystemDateTime");
                GKDBHelper.AddIndex("DeviceDateTime_Index", "Journal", "DeviceDateTime");

                GKDBHelper.ExecuteNonQuery(@"UPDATE STATISTICS ON Journal WITH FULLSCAN");
            }));
		}

		public static void AddPatch(string index, PatchDelegate patchDelegate)
		{
			AllPatches.Add(new Patch(index, patchDelegate));
		}

		public static void Patch()
		{
            try
            {
                var indexes = GKDBHelper.ReadAllPatches();
                foreach (var patch in AllPatches)
                {
                    if (!indexes.Any(x => x == patch.Index))
                        patch.Apply();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Patcher.Patch");
            }
        }
	}
}