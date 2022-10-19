using System;
using System.Collections.Generic;
using ToolkitCore.Models;
using Verse;

namespace ToolkitCore.Database
{
    public class GlobalDatabase : ModSettings
    {
        public List<Viewer> viewers = new List<Viewer>();

        public override void ExposeData() => Scribe_Collections.Look<Viewer>(ref this.viewers, "viewers", (LookMode)2, Array.Empty<object>());
    }
}
