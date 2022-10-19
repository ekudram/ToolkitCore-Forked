using UnityEngine;
using Verse;

namespace ToolkitCore
{
    public class ToolkitCore : Mod
    {
        public static ToolkitCoreSettings settings;

        public ToolkitCore(ModContentPack content)
          : base(content)
        {
            ToolkitCore.settings = this.GetSettings<ToolkitCoreSettings>();
        }

        public override string SettingsCategory() => nameof(ToolkitCore);

        public override void DoSettingsWindowContents(Rect inRect) => ToolkitCore.settings.DoWindowContents(inRect);
    }
}
