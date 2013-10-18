using System;
using System.Linq;
using System.IO;
using Common;
using Infrastructure.Common;
using Common.GK;
using System.Collections.Generic;
using System.Diagnostics;

namespace FireAdministrator
{
	public static class PatchManager
	{
        static List<Patch> AllPatches;

        static PatchManager()
        {
            AllPatches = new List<Patch>();
            AllPatches.Add(new Patch("DB", "SubsystemTypePatch", () => 
            {
                GKDBHelper.AddColumnToJournal("Subsystem", "tinyint"); 
            }));
            AllPatches.Add(new Patch("DB", "ObjectStateClass", () =>
            {
                GKDBHelper.AddColumnToJournal("ObjectStateClass", "tinyint");
            }));
        }
        
        public static void Patch()
		{
			try
			{
				AllPatches.Where(x => !GKDBHelper.ReadAllPatches().Any(y => y.Equals(x.PatchIndex))).ToList().ForEach(x => x.Apply());
            }
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}
    }

    class Patch
    {
        PatchDelegate PatchDelegate;
        public PatchIndex PatchIndex { get; private set; }

        public Patch(string type, string id, PatchDelegate patchDelegate)
        {
            PatchIndex = new PatchIndex(type, id);
            PatchDelegate = patchDelegate;
        }

        public void Apply()
        {
            PatchDelegate();
            GKDBHelper.AddPatchToDB(PatchIndex);
        }
    }

    delegate void PatchDelegate();
}