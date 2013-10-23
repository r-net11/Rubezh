using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace Common.GK
{
    public static class Patcher
    {
        static List<Patch> AllPatches;

        static Patcher()
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

        public static void AddPatchToList(string type, string id, PatchDelegate patchDelegate)
        {
            AllPatches.Add(new Patch(type, id, patchDelegate));
        }

        public static void Patch()
        {
            try
            {
                AllPatches.Where(x => !GKDBHelper.ReadAllPatches().Any(y => y.Equals(x.PatchIndex))).ToList().ForEach(x => x.Apply());
            }
            catch (Exception e)
            {
                Logger.Error(e, "Patcher.Patch");
            }
        }
    }
}
