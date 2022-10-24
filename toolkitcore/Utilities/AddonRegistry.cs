using System.Collections.Generic;
using System.Linq;
using ToolkitCore.Models;
using Verse;

namespace ToolkitCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class AddonRegistry
    {
        public static List<ToolkitAddon> ToolkitAddons { get; set; }

        static AddonRegistry() => ToolkitAddons = DefDatabase<ToolkitAddon>.AllDefs.ToList();
    }
}
