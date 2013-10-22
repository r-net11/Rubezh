using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
    delegate void PatchDelegate();
    
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
}
