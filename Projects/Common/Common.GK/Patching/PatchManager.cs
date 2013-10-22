using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace Common.GK
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
}
